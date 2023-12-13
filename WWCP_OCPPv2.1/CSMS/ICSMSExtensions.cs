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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// Extension methods for all CSMS
    /// </summary>
    public static class ICSMSExtensions
    {

        #region Reset                      (NetworkingNodeId, ResetType, EVSEId = null, ...)

        /// <summary>
        /// Reset the given charging station/networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
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

            Reset(this ICSMS                    CSMS,
                  NetworkingNode_Id             NetworkingNodeId,
                  ResetType                     ResetType,
                  EVSE_Id?                      EVSEId              = null,

                  IEnumerable<OCPP.Signature>?  Signatures          = null,
                  CustomData?                   CustomData          = null,

                  Request_Id?                   RequestId           = null,
                  DateTime?                     RequestTimestamp    = null,
                  TimeSpan?                     RequestTimeout      = null,
                  EventTracking_Id?             EventTrackingId     = null,

                  IEnumerable<KeyPair>?         SignKeys            = null,
                  IEnumerable<SignInfo>?        SignInfos           = null,

                  CancellationToken             CancellationToken   = default)

                => CSMS.Reset(
                       new ResetRequest(
                           NetworkingNodeId,
                           ResetType,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,
                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateFirmware             (NetworkingNodeId, Firmware, UpdateFirmwareRequestId, ...)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                           NetworkingNode_Id        NetworkingNodeId,
                           Firmware                 Firmware,
                           Int32                    UpdateFirmwareRequestId,
                           Byte?                    Retries             = null,
                           TimeSpan?                RetryInterval       = null,

                           IEnumerable<KeyPair>?    SignKeys            = null,
                           IEnumerable<SignInfo>?   SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.UpdateFirmware(
                       new UpdateFirmwareRequest(
                           NetworkingNodeId,
                           Firmware,
                           UpdateFirmwareRequestId,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region PublishFirmware            (NetworkingNodeId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                            NetworkingNode_Id        NetworkingNodeId,
                            Int32                    PublishFirmwareRequestId,
                            URL                      DownloadLocation,
                            String                   MD5Checksum,
                            Byte?                    Retries             = null,
                            TimeSpan?                RetryInterval       = null,

                            IEnumerable<KeyPair>?    SignKeys            = null,
                            IEnumerable<SignInfo>?   SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.PublishFirmware(
                       new PublishFirmwareRequest(
                           NetworkingNodeId,
                           PublishFirmwareRequestId,
                           DownloadLocation,
                           MD5Checksum,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region UnpublishFirmware          (NetworkingNodeId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                              NetworkingNode_Id        NetworkingNodeId,
                              String                   MD5Checksum,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.UnpublishFirmware(
                       new UnpublishFirmwareRequest(
                           NetworkingNodeId,
                           MD5Checksum,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetBaseReport              (NetworkingNodeId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                          NetworkingNode_Id        NetworkingNodeId,
                          Int64                    GetBaseReportRequestId,
                          ReportBase               ReportBase,

                          IEnumerable<KeyPair>?    SignKeys            = null,
                          IEnumerable<SignInfo>?   SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => CSMS.GetBaseReport(
                       new GetBaseReportRequest(
                           NetworkingNodeId,
                           GetBaseReportRequestId,
                           ReportBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetReport                  (NetworkingNodeId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="GetReportRequestId">The networking node identification.</param>
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
                      NetworkingNode_Id               NetworkingNodeId,
                      Int32                           GetReportRequestId,
                      IEnumerable<ComponentCriteria>  ComponentCriteria,
                      IEnumerable<ComponentVariable>  ComponentVariables,

                      IEnumerable<KeyPair>?           SignKeys            = null,
                      IEnumerable<SignInfo>?          SignInfos           = null,
                      IEnumerable<OCPP.Signature>?    Signatures          = null,

                      CustomData?                     CustomData          = null,

                      Request_Id?                     RequestId           = null,
                      DateTime?                       RequestTimestamp    = null,
                      TimeSpan?                       RequestTimeout      = null,
                      EventTracking_Id?               EventTrackingId     = null,
                      CancellationToken               CancellationToken   = default)


                => CSMS.GetReport(
                       new GetReportRequest(
                           NetworkingNodeId,
                           GetReportRequestId,
                           ComponentCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetLog                     (NetworkingNodeId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                   NetworkingNode_Id        NetworkingNodeId,
                   LogType                  LogType,
                   Int32                    LogRequestId,
                   LogParameters            Log,
                   Byte?                    Retries             = null,
                   TimeSpan?                RetryInterval       = null,

                   IEnumerable<KeyPair>?    SignKeys            = null,
                   IEnumerable<SignInfo>?   SignInfos           = null,
                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                   CustomData?              CustomData          = null,

                   Request_Id?              RequestId           = null,
                   DateTime?                RequestTimestamp    = null,
                   TimeSpan?                RequestTimeout      = null,
                   EventTracking_Id?        EventTrackingId     = null,
                   CancellationToken        CancellationToken   = default)


                => CSMS.GetLog(
                       new GetLogRequest(
                           NetworkingNodeId,
                           LogType,
                           LogRequestId,
                           Log,
                           Retries,
                           RetryInterval,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SetVariables               (NetworkingNodeId, VariableData, ...)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                         NetworkingNode_Id             NetworkingNodeId,
                         IEnumerable<SetVariableData>  VariableData,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => CSMS.SetVariables(
                       new SetVariablesRequest(
                           NetworkingNodeId,
                           VariableData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetVariables               (NetworkingNodeId, VariableData, ...)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                         NetworkingNode_Id             NetworkingNodeId,
                         IEnumerable<GetVariableData>  VariableData,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => CSMS.GetVariables(
                       new GetVariablesRequest(
                           NetworkingNodeId,
                           VariableData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringBase          (NetworkingNodeId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                              NetworkingNode_Id        NetworkingNodeId,
                              MonitoringBase           MonitoringBase,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.SetMonitoringBase(
                       new SetMonitoringBaseRequest(
                           NetworkingNodeId,
                           MonitoringBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetMonitoringReport        (NetworkingNodeId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
        /// <param name="GetMonitoringReportRequestId">The networking node identification.</param>
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

            GetMonitoringReport(this ICSMS                        CSMS,
                                NetworkingNode_Id                 NetworkingNodeId,
                                Int32                             GetMonitoringReportRequestId,
                                IEnumerable<MonitoringCriterion>  MonitoringCriteria,
                                IEnumerable<ComponentVariable>    ComponentVariables,

                                IEnumerable<KeyPair>?             SignKeys            = null,
                                IEnumerable<SignInfo>?            SignInfos           = null,
                                IEnumerable<OCPP.Signature>?      Signatures          = null,

                                CustomData?                       CustomData          = null,

                                Request_Id?                       RequestId           = null,
                                DateTime?                         RequestTimestamp    = null,
                                TimeSpan?                         RequestTimeout      = null,
                                EventTracking_Id?                 EventTrackingId     = null,
                                CancellationToken                 CancellationToken   = default)


                => CSMS.GetMonitoringReport(
                       new GetMonitoringReportRequest(
                           NetworkingNodeId,
                           GetMonitoringReportRequestId,
                           MonitoringCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringLevel         (NetworkingNodeId, Severity, ...)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                               NetworkingNode_Id        NetworkingNodeId,
                               Severities               Severity,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.SetMonitoringLevel(
                       new SetMonitoringLevelRequest(
                           NetworkingNodeId,
                           Severity,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetVariableMonitoring      (NetworkingNodeId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                  NetworkingNode_Id               NetworkingNodeId,
                                  IEnumerable<SetMonitoringData>  MonitoringData,

                                  IEnumerable<KeyPair>?           SignKeys            = null,
                                  IEnumerable<SignInfo>?          SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?    Signatures          = null,

                                  CustomData?                     CustomData          = null,

                                  Request_Id?                     RequestId           = null,
                                  DateTime?                       RequestTimestamp    = null,
                                  TimeSpan?                       RequestTimeout      = null,
                                  EventTracking_Id?               EventTrackingId     = null,
                                  CancellationToken               CancellationToken   = default)


                => CSMS.SetVariableMonitoring(
                       new SetVariableMonitoringRequest(
                           NetworkingNodeId,
                           MonitoringData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearVariableMonitoring    (NetworkingNodeId, VariableMonitoringIds, ...)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                    NetworkingNode_Id                   NetworkingNodeId,
                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

                                    IEnumerable<KeyPair>?               SignKeys            = null,
                                    IEnumerable<SignInfo>?              SignInfos           = null,
                                    IEnumerable<OCPP.Signature>?        Signatures          = null,

                                    CustomData?                         CustomData          = null,

                                    Request_Id?                         RequestId           = null,
                                    DateTime?                           RequestTimestamp    = null,
                                    TimeSpan?                           RequestTimeout      = null,
                                    EventTracking_Id?                   EventTrackingId     = null,
                                    CancellationToken                   CancellationToken   = default)


                => CSMS.ClearVariableMonitoring(
                       new ClearVariableMonitoringRequest(
                           NetworkingNodeId,
                           VariableMonitoringIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetNetworkProfile          (NetworkingNodeId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                              NetworkingNode_Id         NetworkingNodeId,
                              Int32                     ConfigurationSlot,
                              NetworkConnectionProfile  NetworkConnectionProfile,

                              IEnumerable<KeyPair>?     SignKeys            = null,
                              IEnumerable<SignInfo>?    SignInfos           = null,
                              IEnumerable<OCPP.Signature>?   Signatures          = null,

                              CustomData?               CustomData          = null,

                              Request_Id?               RequestId           = null,
                              DateTime?                 RequestTimestamp    = null,
                              TimeSpan?                 RequestTimeout      = null,
                              EventTracking_Id?         EventTrackingId     = null,
                              CancellationToken         CancellationToken   = default)


                => CSMS.SetNetworkProfile(
                       new SetNetworkProfileRequest(
                           NetworkingNodeId,
                           ConfigurationSlot,
                           NetworkConnectionProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeAvailability         (NetworkingNodeId, OperationalStatus, EVSE = null, ...)

        /// <summary>
        /// Change the availability of the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            ChangeAvailability(this ICSMS               CSMS,
                               NetworkingNode_Id        NetworkingNodeId,
                               OperationalStatus        OperationalStatus,

                               EVSE?                    EVSE                = null,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           NetworkingNodeId,
                           OperationalStatus,
                           EVSE,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region TriggerMessage             (NetworkingNodeId, RequestedMessage, EVSEId = null, CustomTrigger = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            TriggerMessage(this ICSMS               CSMS,
                           NetworkingNode_Id        NetworkingNodeId,
                           MessageTrigger           RequestedMessage,
                           EVSE?                    EVSE                = null,
                           String?                  CustomTrigger       = null,

                           IEnumerable<KeyPair>?    SignKeys            = null,
                           IEnumerable<SignInfo>?   SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.TriggerMessage(
                       new TriggerMessageRequest(
                           NetworkingNodeId,
                           RequestedMessage,
                           EVSE,
                           CustomTrigger,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region TransferData               (NetworkingNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
        public static Task<OCPP.CS.DataTransferResponse>

            TransferData(this ICSMS                    CSMS,
                         NetworkingNode_Id             NetworkingNodeId,
                         Vendor_Id                     VendorId,
                         Message_Id?                   MessageId           = null,
                         JToken?                       Data                = null,

                         IEnumerable<KeyPair>?         SignKeys            = null,
                         IEnumerable<SignInfo>?        SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         CustomData?                   CustomData          = null,

                         Request_Id?                   RequestId           = null,
                         DateTime?                     RequestTimestamp    = null,
                         TimeSpan?                     RequestTimeout      = null,
                         EventTracking_Id?             EventTrackingId     = null,
                         CancellationToken             CancellationToken   = default)


                => CSMS.TransferData(
                       new OCPP.CSMS.DataTransferRequest(
                           NetworkingNodeId,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SendSignedCertificate      (NetworkingNodeId, CertificateChain, CertificateType = null, ...)

        /// <summary>
        /// Send the signed certificate to the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                  NetworkingNode_Id        NetworkingNodeId,
                                  CertificateChain         CertificateChain,
                                  CertificateSigningUse?   CertificateType     = null,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null,

                                  Request_Id?              RequestId           = null,
                                  DateTime?                RequestTimestamp    = null,
                                  TimeSpan?                RequestTimeout      = null,
                                  EventTracking_Id?        EventTrackingId     = null,
                                  CancellationToken        CancellationToken   = default)


                => CSMS.SendSignedCertificate(
                       new CertificateSignedRequest(
                           NetworkingNodeId,
                           CertificateChain,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region InstallCertificate         (NetworkingNodeId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                               NetworkingNode_Id        NetworkingNodeId,
                               InstallCertificateUse    CertificateType,
                               Certificate              Certificate,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.InstallCertificate(
                       new InstallCertificateRequest(
                           NetworkingNodeId,
                           CertificateType,
                           Certificate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetInstalledCertificateIds (NetworkingNodeId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            GetInstalledCertificateIds(this ICSMS                         CSMS,
                                       NetworkingNode_Id                  NetworkingNodeId,
                                       IEnumerable<GetCertificateIdUse>?  CertificateTypes    = null,

                                       IEnumerable<KeyPair>?              SignKeys            = null,
                                       IEnumerable<SignInfo>?             SignInfos           = null,
                                       IEnumerable<OCPP.Signature>?       Signatures          = null,

                                       CustomData?                        CustomData          = null,

                                       Request_Id?                        RequestId           = null,
                                       DateTime?                          RequestTimestamp    = null,
                                       TimeSpan?                          RequestTimeout      = null,
                                       EventTracking_Id?                  EventTrackingId     = null,
                                       CancellationToken                  CancellationToken   = default)


                => CSMS.GetInstalledCertificateIds(
                       new GetInstalledCertificateIdsRequest(
                           NetworkingNodeId,
                           CertificateTypes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteCertificate          (NetworkingNodeId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                              NetworkingNode_Id        NetworkingNodeId,
                              CertificateHashData      CertificateHashData,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.DeleteCertificate(
                       new DeleteCertificateRequest(
                           NetworkingNodeId,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyCRLAvailability      (NetworkingNodeId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                  NetworkingNode_Id        NetworkingNodeId,
                                  Int32                    NotifyCRLRequestId,
                                  NotifyCRLStatus          Availability,
                                  URL?                     Location,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null,

                                  Request_Id?              RequestId           = null,
                                  DateTime?                RequestTimestamp    = null,
                                  TimeSpan?                RequestTimeout      = null,
                                  EventTracking_Id?        EventTrackingId     = null,
                                  CancellationToken        CancellationToken   = default)


                => CSMS.NotifyCRLAvailability(
                       new NotifyCRLRequest(
                           NetworkingNodeId,
                           NotifyCRLRequestId,
                           Availability,
                           Location,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region GetLocalListVersion        (NetworkingNodeId, ...)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                NetworkingNode_Id        NetworkingNodeId,

                                IEnumerable<KeyPair>?    SignKeys            = null,
                                IEnumerable<SignInfo>?   SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.GetLocalListVersion(
                       new GetLocalListVersionRequest(
                           NetworkingNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendLocalList              (NetworkingNodeId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                          NetworkingNode_Id                NetworkingNodeId,
                          UInt64                           ListVersion,
                          UpdateTypes                      UpdateType,
                          IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                          IEnumerable<KeyPair>?            SignKeys                 = null,
                          IEnumerable<SignInfo>?           SignInfos                = null,
                          IEnumerable<OCPP.Signature>?     Signatures               = null,

                          CustomData?                      CustomData               = null,

                          Request_Id?                      RequestId                = null,
                          DateTime?                        RequestTimestamp         = null,
                          TimeSpan?                        RequestTimeout           = null,
                          EventTracking_Id?                EventTrackingId          = null,
                          CancellationToken                CancellationToken        = default)


                => CSMS.SendLocalList(
                       new SendLocalListRequest(
                           NetworkingNodeId,
                           ListVersion,
                           UpdateType,
                           LocalAuthorizationList,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearCache                 (NetworkingNodeId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                       NetworkingNode_Id        NetworkingNodeId,

                       IEnumerable<KeyPair>?    SignKeys            = null,
                       IEnumerable<SignInfo>?   SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       CustomData?              CustomData          = null,

                       Request_Id?              RequestId           = null,
                       DateTime?                RequestTimestamp    = null,
                       TimeSpan?                RequestTimeout      = null,
                       EventTracking_Id?        EventTrackingId     = null,
                       CancellationToken        CancellationToken   = default)


                => CSMS.ClearCache(
                       new ClearCacheRequest(
                           NetworkingNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region ReserveNow                 (NetworkingNodeId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                       NetworkingNode_Id        NetworkingNodeId,
                       Reservation_Id           ReservationId,
                       DateTime                 ExpiryDate,
                       IdToken                  IdToken,
                       ConnectorType?          ConnectorType       = null,
                       EVSE_Id?                 EVSEId              = null,
                       IdToken?                 GroupIdToken        = null,

                       IEnumerable<KeyPair>?    SignKeys            = null,
                       IEnumerable<SignInfo>?   SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       CustomData?              CustomData          = null,

                       Request_Id?              RequestId           = null,
                       DateTime?                RequestTimestamp    = null,
                       TimeSpan?                RequestTimeout      = null,
                       EventTracking_Id?        EventTrackingId     = null,
                       CancellationToken        CancellationToken   = default)


                => CSMS.ReserveNow(
                       new ReserveNowRequest(
                           NetworkingNodeId,
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

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region CancelReservation          (NetworkingNodeId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                              NetworkingNode_Id        NetworkingNodeId,
                              Reservation_Id           ReservationId,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.CancelReservation(
                       new CancelReservationRequest(
                           NetworkingNodeId,
                           ReservationId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region StartCharging              (NetworkingNodeId, RequestStartTransactionRequestId, IdToken, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            StartCharging(this ICSMS               CSMS,
                          NetworkingNode_Id        NetworkingNodeId,
                          RemoteStart_Id           RequestStartTransactionRequestId,
                          IdToken                  IdToken,
                          EVSE_Id?                 EVSEId              = null,
                          ChargingProfile?         ChargingProfile     = null,
                          IdToken?                 GroupIdToken        = null,
                          TransactionLimits?       TransactionLimits   = null,

                          IEnumerable<KeyPair>?    SignKeys            = null,
                          IEnumerable<SignInfo>?   SignInfos           = null,
                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                          CustomData?              CustomData          = null,

                          Request_Id?              RequestId           = null,
                          DateTime?                RequestTimestamp    = null,
                          TimeSpan?                RequestTimeout      = null,
                          EventTracking_Id?        EventTrackingId     = null,
                          CancellationToken        CancellationToken   = default)


                => CSMS.StartCharging(
                       new RequestStartTransactionRequest(
                           NetworkingNodeId,
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

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region StopCharging               (NetworkingNodeId, TransactionId, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                         NetworkingNode_Id        NetworkingNodeId,
                         Transaction_Id           TransactionId,

                         IEnumerable<KeyPair>?    SignKeys            = null,
                         IEnumerable<SignInfo>?   SignInfos           = null,
                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => CSMS.StopCharging(
                       new RequestStopTransactionRequest(
                           NetworkingNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetTransactionStatus       (NetworkingNodeId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                 NetworkingNode_Id        NetworkingNodeId,
                                 Transaction_Id?          TransactionId       = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 CancellationToken        CancellationToken   = default)


                => CSMS.GetTransactionStatus(
                       new GetTransactionStatusRequest(
                           NetworkingNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SetChargingProfile         (NetworkingNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                               NetworkingNode_Id        NetworkingNodeId,
                               EVSE_Id                  EVSEId,
                               ChargingProfile          ChargingProfile,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.SetChargingProfile(
                       new SetChargingProfileRequest(
                           NetworkingNodeId,
                           EVSEId,
                           ChargingProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetChargingProfiles        (NetworkingNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                NetworkingNode_Id         NetworkingNodeId,
                                Int64                     GetChargingProfilesRequestId,
                                ChargingProfileCriterion  ChargingProfile,
                                EVSE_Id?                  EVSEId              = null,

                                IEnumerable<KeyPair>?     SignKeys            = null,
                                IEnumerable<SignInfo>?    SignInfos           = null,
                                IEnumerable<OCPP.Signature>?   Signatures          = null,

                                CustomData?               CustomData          = null,

                                Request_Id?               RequestId           = null,
                                DateTime?                 RequestTimestamp    = null,
                                TimeSpan?                 RequestTimeout      = null,
                                EventTracking_Id?         EventTrackingId     = null,
                                CancellationToken         CancellationToken   = default)


                => CSMS.GetChargingProfiles(
                       new GetChargingProfilesRequest(
                           NetworkingNodeId,
                           GetChargingProfilesRequestId,
                           ChargingProfile,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearChargingProfile       (NetworkingNodeId, ChargingProfileId, ChargingProfileCriteria, ...)

        /// <summary>
        /// Remove the charging profile at the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                 NetworkingNode_Id        NetworkingNodeId,
                                 ChargingProfile_Id?      ChargingProfileId         = null,
                                 ClearChargingProfile?    ChargingProfileCriteria   = null,

                                 IEnumerable<KeyPair>?    SignKeys                  = null,
                                 IEnumerable<SignInfo>?   SignInfos                 = null,
                                 IEnumerable<OCPP.Signature>?  Signatures                = null,

                                 CustomData?              CustomData                = null,

                                 Request_Id?              RequestId                 = null,
                                 DateTime?                RequestTimestamp          = null,
                                 TimeSpan?                RequestTimeout            = null,
                                 EventTracking_Id?        EventTrackingId           = null,
                                 CancellationToken        CancellationToken         = default)


                => CSMS.ClearChargingProfile(
                       new ClearChargingProfileRequest(
                           NetworkingNodeId,
                           ChargingProfileId,
                           ChargingProfileCriteria,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCompositeSchedule       (NetworkingNodeId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                 NetworkingNode_Id        NetworkingNodeId,
                                 TimeSpan                 Duration,
                                 EVSE_Id                  EVSEId,
                                 ChargingRateUnits?       ChargingRateUnit    = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 CancellationToken        CancellationToken   = default)


                => CSMS.GetCompositeSchedule(
                       new GetCompositeScheduleRequest(
                           NetworkingNodeId,
                           Duration,
                           EVSEId,
                           ChargingRateUnit,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateDynamicSchedule      (NetworkingNodeId, ChargingProfileId, Limit = null, ...)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                  NetworkingNode_Id        NetworkingNodeId,
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

                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                  IEnumerable<OCPP.Signature>?  Signatures            = null,

                                  CustomData?              CustomData            = null,

                                  Request_Id?              RequestId             = null,
                                  DateTime?                RequestTimestamp      = null,
                                  TimeSpan?                RequestTimeout        = null,
                                  EventTracking_Id?        EventTrackingId       = null,
                                  CancellationToken        CancellationToken     = default)


                => CSMS.UpdateDynamicSchedule(
                       new UpdateDynamicScheduleRequest(

                           NetworkingNodeId,
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

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyAllowedEnergyTransfer(NetworkingNodeId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            NotifyAllowedEnergyTransfer(this ICSMS                       CSMS,
                                        NetworkingNode_Id                NetworkingNodeId,
                                        IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes,

                                        IEnumerable<KeyPair>?            SignKeys            = null,
                                        IEnumerable<SignInfo>?           SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?     Signatures          = null,

                                        CustomData?                      CustomData          = null,

                                        Request_Id?                      RequestId           = null,
                                        DateTime?                        RequestTimestamp    = null,
                                        TimeSpan?                        RequestTimeout      = null,
                                        EventTracking_Id?                EventTrackingId     = null,
                                        CancellationToken                CancellationToken   = default)


                => CSMS.NotifyAllowedEnergyTransfer(
                       new NotifyAllowedEnergyTransferRequest(
                           NetworkingNodeId,
                           AllowedEnergyTransferModes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region UsePriorityCharging        (NetworkingNodeId, TransactionId, Activate, ...)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                                NetworkingNode_Id        NetworkingNodeId,
                                Transaction_Id           TransactionId,
                                Boolean                  Activate,

                                IEnumerable<KeyPair>?    SignKeys            = null,
                                IEnumerable<SignInfo>?   SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.UsePriorityCharging(
                       new UsePriorityChargingRequest(
                           NetworkingNodeId,
                           TransactionId,
                           Activate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region UnlockConnector            (NetworkingNodeId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                            NetworkingNode_Id        NetworkingNodeId,
                            EVSE_Id                  EVSEId,
                            Connector_Id             ConnectorId,

                            IEnumerable<KeyPair>?    SignKeys            = null,
                            IEnumerable<SignInfo>?   SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.UnlockConnector(
                       new UnlockConnectorRequest(
                           NetworkingNodeId,
                           EVSEId,
                           ConnectorId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SendAFRRSignal             (NetworkingNodeId, ActivationTimestamp, Signal, ...)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
                           NetworkingNode_Id        NetworkingNodeId,
                           DateTime                 ActivationTimestamp,
                           AFRR_Signal              Signal,

                           IEnumerable<KeyPair>?    SignKeys            = null,
                           IEnumerable<SignInfo>?   SignInfos           = null,
                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                           CustomData?              CustomData          = null,

                           Request_Id?              RequestId           = null,
                           DateTime?                RequestTimestamp    = null,
                           TimeSpan?                RequestTimeout      = null,
                           EventTracking_Id?        EventTrackingId     = null,
                           CancellationToken        CancellationToken   = default)


                => CSMS.SendAFRRSignal(
                       new AFRRSignalRequest(
                           NetworkingNodeId,
                           ActivationTimestamp,
                           Signal,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        #region SetDisplayMessage          (NetworkingNodeId, Message, ...)

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
                              NetworkingNode_Id        NetworkingNodeId,
                              MessageInfo              Message,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<OCPP.Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null,

                              Request_Id?              RequestId           = null,
                              DateTime?                RequestTimestamp    = null,
                              TimeSpan?                RequestTimeout      = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              CancellationToken        CancellationToken   = default)


                => CSMS.SetDisplayMessage(
                       new SetDisplayMessageRequest(
                           NetworkingNodeId,
                           Message,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetDisplayMessages         (NetworkingNodeId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null, ...)

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
                               NetworkingNode_Id                NetworkingNodeId,
                               Int32                            GetDisplayMessagesRequestId,
                               IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                               MessagePriority?                 Priority            = null,
                               MessageState?                    State               = null,

                               IEnumerable<KeyPair>?            SignKeys            = null,
                               IEnumerable<SignInfo>?           SignInfos           = null,
                               IEnumerable<OCPP.Signature>?     Signatures          = null,

                               CustomData?                      CustomData          = null,

                               Request_Id?                      RequestId           = null,
                               DateTime?                        RequestTimestamp    = null,
                               TimeSpan?                        RequestTimeout      = null,
                               EventTracking_Id?                EventTrackingId     = null,
                               CancellationToken                CancellationToken   = default)


                => CSMS.GetDisplayMessages(
                       new GetDisplayMessagesRequest(
                           NetworkingNodeId,
                           GetDisplayMessagesRequestId,
                           Ids,
                           Priority,
                           State,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearDisplayMessage        (NetworkingNodeId, DisplayMessageId, ...)

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
                                NetworkingNode_Id        NetworkingNodeId,
                                DisplayMessage_Id        DisplayMessageId,

                                IEnumerable<KeyPair>?    SignKeys            = null,
                                IEnumerable<SignInfo>?   SignInfos           = null,
                                IEnumerable<OCPP.Signature>?  Signatures          = null,

                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)


                => CSMS.ClearDisplayMessage(
                       new ClearDisplayMessageRequest(
                           NetworkingNodeId,
                           DisplayMessageId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendCostUpdated            (NetworkingNodeId, TotalCost, TransactionId, ...)

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
                            NetworkingNode_Id        NetworkingNodeId,
                            Decimal                  TotalCost,
                            Transaction_Id           TransactionId,

                            IEnumerable<KeyPair>?    SignKeys            = null,
                            IEnumerable<SignInfo>?   SignInfos           = null,
                            IEnumerable<OCPP.Signature>?  Signatures          = null,

                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)


                => CSMS.SendCostUpdated(
                       new CostUpdatedRequest(
                           NetworkingNodeId,
                           TotalCost,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region RequestCustomerInformation (NetworkingNodeId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

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
                                       NetworkingNode_Id        NetworkingNodeId,
                                       Int64                    CustomerInformationRequestId,
                                       Boolean                  Report,
                                       Boolean                  Clear,
                                       CustomerIdentifier?      CustomerIdentifier    = null,
                                       IdToken?                 IdToken               = null,
                                       CertificateHashData?     CustomerCertificate   = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<OCPP.Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTime?                RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       CancellationToken        CancellationToken     = default)


                => CSMS.RequestCustomerInformation(
                       new CustomerInformationRequest(
                           NetworkingNodeId,
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

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData         (NetworkingNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given binary data to the given charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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
        public static Task<OCPP.CS.BinaryDataTransferResponse>

            TransferBinaryData(this ICSMS               CSMS,
                               NetworkingNode_Id        NetworkingNodeId,
                               Vendor_Id                VendorId,
                               Message_Id?              MessageId           = null,
                               Byte[]?                  Data                = null,
                               BinaryFormats?           Format              = null,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)


                => CSMS.BinaryDataTransfer(
                       new OCPP.CSMS.BinaryDataTransferRequest(
                           NetworkingNodeId,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetFile                    (NetworkingNodeId, Filename, Priority = null, ...)

        /// <summary>
        /// Request to download the given file from the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            GetFile(this ICSMS               CSMS,
                    NetworkingNode_Id        NetworkingNodeId,
                    FilePath                 Filename,
                    Byte?                    Priority            = null,

                    IEnumerable<KeyPair>?    SignKeys            = null,
                    IEnumerable<SignInfo>?   SignInfos           = null,
                    IEnumerable<OCPP.Signature>?  Signatures          = null,

                    CustomData?              CustomData          = null,

                    Request_Id?              RequestId           = null,
                    DateTime?                RequestTimestamp    = null,
                    TimeSpan?                RequestTimeout      = null,
                    EventTracking_Id?        EventTrackingId     = null,
                    CancellationToken        CancellationToken   = default)


                => CSMS.GetFile(
                       new OCPP.CSMS.GetFileRequest(
                           NetworkingNodeId,
                           Filename,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFile                   (NetworkingNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            SendFile(this ICSMS               CSMS,
                     NetworkingNode_Id        NetworkingNodeId,
                     FilePath                 FileName,
                     Byte[]                   FileContent,
                     ContentType?             FileContentType     = null,
                     Byte[]?                  FileSHA256          = null,
                     Byte[]?                  FileSHA512          = null,
                     IEnumerable<OCPP.Signature>?  FileSignatures      = null,
                     Byte?                    Priority            = null,

                     IEnumerable<KeyPair>?    SignKeys            = null,
                     IEnumerable<SignInfo>?   SignInfos           = null,
                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                     CustomData?              CustomData          = null,

                     Request_Id?              RequestId           = null,
                     DateTime?                RequestTimestamp    = null,
                     TimeSpan?                RequestTimeout      = null,
                     EventTracking_Id?        EventTrackingId     = null,
                     CancellationToken        CancellationToken   = default)


                => CSMS.SendFile(
                       new OCPP.CSMS.SendFileRequest(
                           NetworkingNodeId,
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

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteFile                   (NetworkingNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="NetworkingNodeId">The networking node identification.</param>
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

            DeleteFile(this ICSMS               CSMS,
                       NetworkingNode_Id        NetworkingNodeId,
                       FilePath                 FileName,
                       Byte[]?                  FileSHA256          = null,
                       Byte[]?                  FileSHA512          = null,

                       IEnumerable<KeyPair>?    SignKeys            = null,
                       IEnumerable<SignInfo>?   SignInfos           = null,
                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                       CustomData?              CustomData          = null,

                       Request_Id?              RequestId           = null,
                       DateTime?                RequestTimestamp    = null,
                       TimeSpan?                RequestTimeout      = null,
                       EventTracking_Id?        EventTrackingId     = null,
                       CancellationToken        CancellationToken   = default)


                => CSMS.DeleteFile(
                       new OCPP.CSMS.DeleteFileRequest(
                           NetworkingNodeId,
                           FileName,
                           FileSHA256,
                           FileSHA512,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion



        // E2E Security Extensions





        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff   (NetworkingNodeId, ChargingTariff,          EVSEIds = null, ...)

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

            SetDefaultChargingTariff(this ICSMS               CSMS,
                                     NetworkingNode_Id        NetworkingNodeId,
                                     ChargingTariff           ChargingTariff,
                                     IEnumerable<EVSE_Id>?    EVSEIds             = null,

                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     CustomData?              CustomData          = null,

                                     Request_Id?              RequestId           = null,
                                     DateTime?                RequestTimestamp    = null,
                                     TimeSpan?                RequestTimeout      = null,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     CancellationToken        CancellationToken   = default)


                => CSMS.SetDefaultChargingTariff(
                       new SetDefaultChargingTariffRequest(
                           NetworkingNodeId,
                           ChargingTariff,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetDefaultChargingTariff   (NetworkingNodeId,                          EVSEIds = null, ...)

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

            GetDefaultChargingTariff(this ICSMS               CSMS,
                                     NetworkingNode_Id        NetworkingNodeId,
                                     IEnumerable<EVSE_Id>?    EVSEIds             = null,

                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     CustomData?              CustomData          = null,

                                     Request_Id?              RequestId           = null,
                                     DateTime?                RequestTimestamp    = null,
                                     TimeSpan?                RequestTimeout      = null,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     CancellationToken        CancellationToken   = default)


                => CSMS.GetDefaultChargingTariff(
                       new GetDefaultChargingTariffRequest(
                           NetworkingNodeId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion

        #region RemoveDefaultChargingTariff(NetworkingNodeId, ChargingTariffId = null, EVSEIds = null, ...)

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

            RemoveDefaultChargingTariff(this ICSMS               CSMS,
                                        NetworkingNode_Id        NetworkingNodeId,
                                        ChargingTariff_Id?       ChargingTariffId    = null,
                                        IEnumerable<EVSE_Id>?    EVSEIds             = null,

                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                        CustomData?              CustomData          = null,

                                        Request_Id?              RequestId           = null,
                                        DateTime?                RequestTimestamp    = null,
                                        TimeSpan?                RequestTimeout      = null,
                                        EventTracking_Id?        EventTrackingId     = null,
                                        CancellationToken        CancellationToken   = default)


                => CSMS.RemoveDefaultChargingTariff(
                       new RemoveDefaultChargingTariffRequest(
                           NetworkingNodeId,
                           ChargingTariffId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CSMS.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CSMS.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.Empty,
                           CancellationToken
                       )
                   );

        #endregion



    }

}
