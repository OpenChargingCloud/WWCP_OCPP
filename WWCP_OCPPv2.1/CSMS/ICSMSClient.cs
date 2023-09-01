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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// Extention methods for all CSMS clients
    /// </summary>
    public static class ICSMSClientExtensions
    {

        #region Reset                      (ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charging station should perform.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ResetResponse> Reset(this ICSMSClient   ICSMSClient,
                                                ChargeBox_Id       ChargeBoxId,
                                                ResetTypes         ResetType,
                                                CustomData?        CustomData          = null,

                                                Request_Id?        RequestId           = null,
                                                DateTime?          RequestTimestamp    = null,
                                                TimeSpan?          RequestTimeout      = null,
                                                EventTracking_Id?  EventTrackingId     = null,
                                                CancellationToken  CancellationToken   = default)

            => ICSMSClient.Reset(
                   new ResetRequest(
                       ChargeBoxId,
                       ResetType,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region UpdateFirmware             (ChargeBoxId, FirmwareURL, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="Firmware">The firmware image to be installed at the charging station.</param>
        /// <param name="UpdateFirmwareRequestId">The update firmware request identification.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UpdateFirmwareResponse> UpdateFirmware(this ICSMSClient   ICSMSClient,
                                                                  ChargeBox_Id       ChargeBoxId,
                                                                  Firmware           Firmware,
                                                                  Int32              UpdateFirmwareRequestId,
                                                                  Byte?              Retries             = null,
                                                                  TimeSpan?          RetryInterval       = null,
                                                                  CustomData?        CustomData          = null,

                                                                  Request_Id?        RequestId           = null,
                                                                  DateTime?          RequestTimestamp    = null,
                                                                  TimeSpan?          RequestTimeout      = null,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  CancellationToken  CancellationToken   = default)

            => ICSMSClient.UpdateFirmware(
                   new UpdateFirmwareRequest(
                       ChargeBoxId,
                       Firmware,
                       UpdateFirmwareRequestId,
                       Retries,
                       RetryInterval,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region PublishFirmware            (ChargeBoxId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware image.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="PublishFirmwareRequestId">The unique identification of this publish firmware request</param>
        /// <param name="DownloadLocation">An URL for downloading the firmware.onto the local controller.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
        /// <param name="Retries">The optional number of retries of a charging station for trying to download the firmware before giving up. If this field is not present, it is left to the charging station to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charging station to decide how long to wait between attempts.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<PublishFirmwareResponse> PublishFirmware(this ICSMSClient   ICSMSClient,
                                                                    ChargeBox_Id       ChargeBoxId,
                                                                    Int32              PublishFirmwareRequestId,
                                                                    URL                DownloadLocation,
                                                                    String             MD5Checksum,
                                                                    Byte?              Retries             = null,
                                                                    TimeSpan?          RetryInterval       = null,
                                                                    CustomData?        CustomData          = null,

                                                                    Request_Id?        RequestId           = null,
                                                                    DateTime?          RequestTimestamp    = null,
                                                                    TimeSpan?          RequestTimeout      = null,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    CancellationToken  CancellationToken   = default)

            => ICSMSClient.PublishFirmware(
                   new PublishFirmwareRequest(
                       ChargeBoxId,
                       PublishFirmwareRequestId,
                       DownloadLocation,
                       MD5Checksum,
                       Retries,
                       RetryInterval,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region UnpublishFirmware          (ChargeBoxId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware image.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UnpublishFirmwareResponse> UnpublishFirmware(this ICSMSClient   ICSMSClient,
                                                                        ChargeBox_Id       ChargeBoxId,
                                                                        String             MD5Checksum,
                                                                        CustomData?        CustomData          = null,

                                                                        Request_Id?        RequestId           = null,
                                                                        DateTime?          RequestTimestamp    = null,
                                                                        TimeSpan?          RequestTimeout      = null,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        CancellationToken  CancellationToken   = default)

            => ICSMSClient.UnpublishFirmware(
                   new UnpublishFirmwareRequest(
                       ChargeBoxId,
                       MD5Checksum,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetBaseReport              (ChargeBoxId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Get a base report.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="GetBaseReportRequestId">An unique identification of the get base report request.</param>
        /// <param name="ReportBase">The requested reporting base.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetBaseReportResponse> GetBaseReport(this ICSMSClient   ICSMSClient,
                                                                ChargeBox_Id       ChargeBoxId,
                                                                Int64              GetBaseReportRequestId,
                                                                ReportBases        ReportBase,
                                                                CustomData?        CustomData          = null,

                                                                Request_Id?        RequestId           = null,
                                                                DateTime?          RequestTimestamp    = null,
                                                                TimeSpan?          RequestTimeout      = null,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                CancellationToken  CancellationToken   = default)

            => ICSMSClient.GetBaseReport(
                   new GetBaseReportRequest(
                       ChargeBoxId,
                       GetBaseReportRequestId,
                       ReportBase,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetReport                  (ChargeBoxId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get a report.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="GetReportRequestId">The charge box identification.</param>
        /// <param name="ComponentCriteria">An optional enumeration of criteria for components for which a report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a report is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetReportResponse> GetReport(this ICSMSClient                ICSMSClient,
                                                        ChargeBox_Id                    ChargeBoxId,
                                                        Int32                           GetReportRequestId,
                                                        IEnumerable<ComponentCriteria>  ComponentCriteria,
                                                        IEnumerable<ComponentVariable>  ComponentVariables,
                                                        CustomData?                     CustomData          = null,

                                                        Request_Id?                     RequestId           = null,
                                                        DateTime?                       RequestTimestamp    = null,
                                                        TimeSpan?                       RequestTimeout      = null,
                                                        EventTracking_Id?               EventTrackingId     = null,
                                                        CancellationToken               CancellationToken   = default)

            => ICSMSClient.GetReport(
                   new GetReportRequest(
                       ChargeBoxId,
                       GetReportRequestId,
                       ComponentCriteria,
                       ComponentVariables,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetLog                     (ChargeBoxId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Get a log(file).
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="LogType">The type of the certificates requested.</param>
        /// <param name="LogRequestId">The unique identification of this request.</param>
        /// <param name="Log">This field specifies the requested log and the location to which the log should be sent.</param>
        /// <param name="Retries">This specifies how many times the Charge Point must try to upload the log before giving up. If this field is not present, it is left to Charge Point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to Charge Point to decide how long to wait between attempts.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetLogResponse> GetLog(this ICSMSClient   ICSMSClient,
                                                  ChargeBox_Id       ChargeBoxId,
                                                  LogTypes           LogType,
                                                  Int32              LogRequestId,
                                                  LogParameters      Log,
                                                  Byte?              Retries             = null,
                                                  TimeSpan?          RetryInterval       = null,
                                                  CustomData?        CustomData          = null,

                                                  Request_Id?        RequestId           = null,
                                                  DateTime?          RequestTimestamp    = null,
                                                  TimeSpan?          RequestTimeout      = null,
                                                  EventTracking_Id?  EventTrackingId     = null,
                                                  CancellationToken  CancellationToken   = default)

            => ICSMSClient.GetLog(
                   new GetLogRequest(
                       ChargeBoxId,
                       LogType,
                       LogRequestId,
                       Log,
                       Retries,
                       RetryInterval,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetVariables               (ChargeBoxId, VariableData, ...)

        /// <summary>
        /// Set variables.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="VariableData">An enumeration of set variable data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetVariablesResponse> SetVariables(this ICSMSClient              ICSMSClient,
                                                              ChargeBox_Id                  ChargeBoxId,
                                                              IEnumerable<SetVariableData>  VariableData,
                                                              CustomData?                   CustomData          = null,

                                                              Request_Id?                   RequestId           = null,
                                                              DateTime?                     RequestTimestamp    = null,
                                                              TimeSpan?                     RequestTimeout      = null,
                                                              EventTracking_Id?             EventTrackingId     = null,
                                                              CancellationToken             CancellationToken   = default)

            => ICSMSClient.SetVariables(
                   new SetVariablesRequest(
                       ChargeBoxId,
                       VariableData,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetVariables               (ChargeBoxId, VariableData, ...)

        /// <summary>
        /// Get variables.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="VariableData">An enumeration of requested variable data sets.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetVariablesResponse> GetVariables(this ICSMSClient              ICSMSClient,
                                                              ChargeBox_Id                  ChargeBoxId,
                                                              IEnumerable<GetVariableData>  VariableData,
                                                              CustomData?                   CustomData          = null,

                                                              Request_Id?                   RequestId           = null,
                                                              DateTime?                     RequestTimestamp    = null,
                                                              TimeSpan?                     RequestTimeout      = null,
                                                              EventTracking_Id?             EventTrackingId     = null,
                                                              CancellationToken             CancellationToken   = default)

            => ICSMSClient.GetVariables(
                   new GetVariablesRequest(
                       ChargeBoxId,
                       VariableData,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetMonitoringBase          (ChargeBoxId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="MonitoringBase">The monitoring base to be set.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetMonitoringBaseResponse> SetMonitoringBase(this ICSMSClient   ICSMSClient,
                                                                        ChargeBox_Id       ChargeBoxId,
                                                                        MonitoringBases    MonitoringBase,
                                                                        CustomData?        CustomData          = null,

                                                                        Request_Id?        RequestId           = null,
                                                                        DateTime?          RequestTimestamp    = null,
                                                                        TimeSpan?          RequestTimeout      = null,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        CancellationToken  CancellationToken   = default)

            => ICSMSClient.SetMonitoringBase(
                   new SetMonitoringBaseRequest(
                       ChargeBoxId,
                       MonitoringBase,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetMonitoringReport        (ChargeBoxId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get a monitoring report.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="GetMonitoringReportRequestId">The charge box identification.</param>
        /// <param name="MonitoringCriteria">An optional enumeration of criteria for components for which a monitoring report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a monitoring report is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetMonitoringReportResponse> GetMonitoringReport(this ICSMSClient                 ICSMSClient,
                                                                            ChargeBox_Id                     ChargeBoxId,
                                                                            Int32                            GetMonitoringReportRequestId,
                                                                            IEnumerable<MonitoringCriteria>  MonitoringCriteria,
                                                                            IEnumerable<ComponentVariable>   ComponentVariables,
                                                                            CustomData?                      CustomData          = null,

                                                                            Request_Id?                      RequestId           = null,
                                                                            DateTime?                        RequestTimestamp    = null,
                                                                            TimeSpan?                        RequestTimeout      = null,
                                                                            EventTracking_Id?                EventTrackingId     = null,
                                                                            CancellationToken                CancellationToken   = default)

            => ICSMSClient.GetMonitoringReport(
                   new GetMonitoringReportRequest(
                       ChargeBoxId,
                       GetMonitoringReportRequestId,
                       MonitoringCriteria,
                       ComponentVariables,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetMonitoringLevel         (ChargeBoxId, Severity, ...)

        /// <summary>
        /// Set the monitoring level.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="Severity">The charging station SHALL only report events with a severity number lower than or equal to this severity.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetMonitoringLevelResponse> SetMonitoringLevel(this ICSMSClient   ICSMSClient,
                                                                          ChargeBox_Id       ChargeBoxId,
                                                                          Severities         Severity,
                                                                          CustomData?        CustomData          = null,

                                                                          Request_Id?        RequestId           = null,
                                                                          DateTime?          RequestTimestamp    = null,
                                                                          TimeSpan?          RequestTimeout      = null,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.SetMonitoringLevel(
                   new SetMonitoringLevelRequest(
                       ChargeBoxId,
                       Severity,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetVariableMonitoring      (ChargeBoxId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="MonitoringData">An enumeration of monitoring data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetVariableMonitoringResponse> SetVariableMonitoring(this ICSMSClient                ICSMSClient,
                                                                                ChargeBox_Id                    ChargeBoxId,
                                                                                IEnumerable<SetMonitoringData>  MonitoringData,
                                                                                CustomData?                     CustomData          = null,

                                                                                Request_Id?                     RequestId           = null,
                                                                                DateTime?                       RequestTimestamp    = null,
                                                                                TimeSpan?                       RequestTimeout      = null,
                                                                                EventTracking_Id?               EventTrackingId     = null,
                                                                                CancellationToken               CancellationToken   = default)

            => ICSMSClient.SetVariableMonitoring(
                   new SetVariableMonitoringRequest(
                       ChargeBoxId,
                       MonitoringData,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearVariableMonitoring    (ChargeBoxId, VariableMonitoringIds, ...)

        /// <summary>
        /// Remove the given variable monitoring.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="VariableMonitoringIds">An enumeration of variable monitoring identifications to clear.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(this ICSMSClient                    ICSMSClient,
                                                                                    ChargeBox_Id                        ChargeBoxId,
                                                                                    IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,
                                                                                    CustomData?                         CustomData          = null,

                                                                                    Request_Id?                         RequestId           = null,
                                                                                    DateTime?                           RequestTimestamp    = null,
                                                                                    TimeSpan?                           RequestTimeout      = null,
                                                                                    EventTracking_Id?                   EventTrackingId     = null,
                                                                                    CancellationToken                   CancellationToken   = default)

            => ICSMSClient.ClearVariableMonitoring(
                   new ClearVariableMonitoringRequest(
                       ChargeBoxId,
                       VariableMonitoringIds,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetNetworkProfile          (ChargeBoxId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ConfigurationSlot">The slot in which the configuration should be stored.</param>
        /// <param name="NetworkConnectionProfile">The network connection configuration.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetNetworkProfileResponse> SetNetworkProfile(this ICSMSClient          ICSMSClient,
                                                                        ChargeBox_Id              ChargeBoxId,
                                                                        Int32                     ConfigurationSlot,
                                                                        NetworkConnectionProfile  NetworkConnectionProfile,
                                                                        CustomData?               CustomData          = null,

                                                                        Request_Id?               RequestId           = null,
                                                                        DateTime?                 RequestTimestamp    = null,
                                                                        TimeSpan?                 RequestTimeout      = null,
                                                                        EventTracking_Id?         EventTrackingId     = null,
                                                                        CancellationToken         CancellationToken   = default)

            => ICSMSClient.SetNetworkProfile(
                   new SetNetworkProfileRequest(
                       ChargeBoxId,
                       ConfigurationSlot,
                       NetworkConnectionProfile,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ChangeAvailability         (ChargeBoxId, OperationalStatus, EVSE, ...)

        /// <summary>
        /// Change the availability of the given charging station or EVSE.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="OperationalStatus">A new operational status of the charging station or EVSE.</param>
        /// <param name="EVSE">Optional identification of an EVSE/connector for which the operational status should be changed.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ChangeAvailabilityResponse> ChangeAvailability(this ICSMSClient   ICSMSClient,
                                                                          ChargeBox_Id       ChargeBoxId,
                                                                          OperationalStatus  OperationalStatus,
                                                                          EVSE?              EVSE                = null,
                                                                          CustomData?        CustomData          = null,

                                                                          Request_Id?        RequestId           = null,
                                                                          DateTime?          RequestTimestamp    = null,
                                                                          TimeSpan?          RequestTimeout      = null,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.ChangeAvailability(
                   new ChangeAvailabilityRequest(
                       ChargeBoxId,
                       OperationalStatus,
                       EVSE,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region TriggerMessage             (ChargeBoxId, RequestedMessage, ConnectorId = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charging station or EVSE.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="EVSEId">Optional connector identification whenever the message applies to a specific EVSE.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<TriggerMessageResponse> TriggerMessage(this ICSMSClient   ICSMSClient,
                                                                  ChargeBox_Id       ChargeBoxId,
                                                                  MessageTriggers    RequestedMessage,
                                                                  EVSE_Id?           EVSEId              = null,
                                                                  CustomData?        CustomData          = null,

                                                                  Request_Id?        RequestId           = null,
                                                                  DateTime?          RequestTimestamp    = null,
                                                                  TimeSpan?          RequestTimeout      = null,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  CancellationToken  CancellationToken   = default)

            => ICSMSClient.TriggerMessage(
                   new TriggerMessageRequest(
                       ChargeBoxId,
                       RequestedMessage,
                       EVSEId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region TransferData               (ChargeBoxId, VendorId, MessageId, Data, ...)

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CS.DataTransferResponse> TransferData(this ICSMSClient   ICSMSClient,
                                                                 ChargeBox_Id       ChargeBoxId,
                                                                 String             VendorId,
                                                                 String             MessageId,
                                                                 String             Data,
                                                                 CustomData?        CustomData          = null,

                                                                 Request_Id?        RequestId           = null,
                                                                 DateTime?          RequestTimestamp    = null,
                                                                 TimeSpan?          RequestTimeout      = null,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 CancellationToken  CancellationToken   = default)

            => ICSMSClient.TransferData(
                   new DataTransferRequest(
                       ChargeBoxId,
                       VendorId,
                       MessageId,
                       Data,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region SendSignedCertificate      (ChargeBoxId, CertificateChain, CertificateType, ...)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
        /// <param name="CertificateType">The certificate/key usage.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CertificateSignedResponse> SendSignedCertificate(this ICSMSClient        ICSMSClient,
                                                                            ChargeBox_Id            ChargeBoxId,
                                                                            CertificateChain        CertificateChain,
                                                                            CertificateSigningUse?  CertificateType     = null,
                                                                            CustomData?             CustomData          = null,

                                                                            Request_Id?             RequestId           = null,
                                                                            DateTime?               RequestTimestamp    = null,
                                                                            TimeSpan?               RequestTimeout      = null,
                                                                            EventTracking_Id?       EventTrackingId     = null,
                                                                            CancellationToken       CancellationToken   = default)

            => ICSMSClient.SendSignedCertificate(
                   new CertificateSignedRequest(
                       ChargeBoxId,
                       CertificateChain,
                       CertificateType,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region InstallCertificate         (ChargeBoxId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<InstallCertificateResponse> InstallCertificate(this ICSMSClient   ICSMSClient,
                                                                          ChargeBox_Id       ChargeBoxId,
                                                                          CertificateUse     CertificateType,
                                                                          Certificate        Certificate,
                                                                          CustomData?        CustomData          = null,

                                                                          Request_Id?        RequestId           = null,
                                                                          DateTime?          RequestTimestamp    = null,
                                                                          TimeSpan?          RequestTimeout      = null,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.InstallCertificate(
                   new InstallCertificateRequest(
                       ChargeBoxId,
                       CertificateType,
                       Certificate,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetInstalledCertificateIds (ChargeBoxId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateTypes">An optional enumeration of certificate types requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(this ICSMSClient              ICSMSClient,
                                                                                          ChargeBox_Id                  ChargeBoxId,
                                                                                          IEnumerable<CertificateUse>?  CertificateTypes    = null,
                                                                                          CustomData?                   CustomData          = null,

                                                                                          Request_Id?                   RequestId           = null,
                                                                                          DateTime?                     RequestTimestamp    = null,
                                                                                          TimeSpan?                     RequestTimeout      = null,
                                                                                          EventTracking_Id?             EventTrackingId     = null,
                                                                                          CancellationToken             CancellationToken   = default)

            => ICSMSClient.GetInstalledCertificateIds(
                   new GetInstalledCertificateIdsRequest(
                       ChargeBoxId,
                       CertificateTypes,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region DeleteCertificate          (ChargeBoxId, CertificateHashData, ...)

        /// <summary>
        /// Remove the given certificate from the charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<DeleteCertificateResponse> DeleteCertificate(this ICSMSClient     ICSMSClient,
                                                                        ChargeBox_Id         ChargeBoxId,
                                                                        CertificateHashData  CertificateHashData,
                                                                        CustomData?          CustomData          = null,

                                                                        Request_Id?          RequestId           = null,
                                                                        DateTime?            RequestTimestamp    = null,
                                                                        TimeSpan?            RequestTimeout      = null,
                                                                        EventTracking_Id?    EventTrackingId     = null,
                                                                        CancellationToken    CancellationToken   = default)

            => ICSMSClient.DeleteCertificate(
                   new DeleteCertificateRequest(
                       ChargeBoxId,
                       CertificateHashData,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region NotifyCRLAvailability      (ChargeBoxId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Notify the charging station about the status of a certificate revocation list.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyCRLRequestId">An unique identification of this request.</param>
        /// <param name="Availability">An availability status of the certificate revocation list.</param>
        /// <param name="Location">An optional location of the certificate revocation list.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<NotifyCRLResponse> NotifyCRLAvailability(this ICSMSClient     ICSMSClient,
                                                                    ChargeBox_Id         ChargeBoxId,
                                                                    Int32                NotifyCRLRequestId,
                                                                    NotifyCRLStatus      Availability,
                                                                    URL?                 Location,
                                                                    CustomData?          CustomData          = null,

                                                                    Request_Id?          RequestId           = null,
                                                                    DateTime?            RequestTimestamp    = null,
                                                                    TimeSpan?            RequestTimeout      = null,
                                                                    EventTracking_Id?    EventTrackingId     = null,
                                                                    CancellationToken    CancellationToken   = default)

            => ICSMSClient.NotifyCRL(
                   new NotifyCRLRequest(
                       ChargeBoxId,
                       NotifyCRLRequestId,
                       Availability,
                       Location,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region GetLocalListVersion        (ChargeBoxId, ...)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetLocalListVersionResponse> GetLocalListVersion(this ICSMSClient   ICSMSClient,
                                                                            ChargeBox_Id       ChargeBoxId,
                                                                            CustomData?        CustomData          = null,

                                                                            Request_Id?        RequestId           = null,
                                                                            DateTime?          RequestTimestamp    = null,
                                                                            TimeSpan?          RequestTimeout      = null,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            CancellationToken  CancellationToken   = default)

            => ICSMSClient.GetLocalListVersion(
                   new GetLocalListVersionRequest(
                       ChargeBoxId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SendLocalList              (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charging station. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SendLocalListResponse> SendLocalList(this ICSMSClient                 ICSMSClient,
                                                                ChargeBox_Id                     ChargeBoxId,
                                                                UInt64                           ListVersion,
                                                                UpdateTypes                      UpdateType,
                                                                IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,
                                                                CustomData?                      CustomData               = null,

                                                                Request_Id?                      RequestId                = null,
                                                                DateTime?                        RequestTimestamp         = null,
                                                                TimeSpan?                        RequestTimeout           = null,
                                                                EventTracking_Id?                EventTrackingId          = null,
                                                                CancellationToken                CancellationToken        = default)

            => ICSMSClient.SendLocalList(
                   new SendLocalListRequest(
                       ChargeBoxId,
                       ListVersion,
                       UpdateType,
                       LocalAuthorizationList,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearCache                 (ChargeBoxId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearCacheResponse> ClearCache(this ICSMSClient   ICSMSClient,
                                                          ChargeBox_Id       ChargeBoxId,
                                                          CustomData?        CustomData          = null,

                                                          Request_Id?        RequestId           = null,
                                                          DateTime?          RequestTimestamp    = null,
                                                          TimeSpan?          RequestTimeout      = null,
                                                          EventTracking_Id?  EventTrackingId     = null,
                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.ClearCache(
                   new ClearCacheRequest(
                       ChargeBoxId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region ReserveNow                 (ChargeBoxId, ReservationId, ExpiryDate, IdToken, ConnectorType = null, EVSEId = null, GroupIdToken = null, ...)

        /// <summary>
        /// Create a charging reservation at the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdToken">The unique token identification for which the reservation is being made.</param>
        /// <param name="ConnectorType">An optional connector type to be reserved..</param>
        /// <param name="EVSEId">The identification of the EVSE to be reserved. A value of 0 means that the reservation is not for a specific EVSE.</param>
        /// <param name="GroupIdToken">An optional group identifier for which the reservation is being made.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ReserveNowResponse> ReserveNow(this ICSMSClient   ICSMSClient,
                                                          ChargeBox_Id       ChargeBoxId,
                                                          Reservation_Id     ReservationId,
                                                          DateTime           ExpiryDate,
                                                          IdToken            IdToken,
                                                          ConnectorTypes?    ConnectorType       = null,
                                                          EVSE_Id?           EVSEId              = null,
                                                          IdToken?           GroupIdToken        = null,
                                                          CustomData?        CustomData          = null,

                                                          Request_Id?        RequestId           = null,
                                                          DateTime?          RequestTimestamp    = null,
                                                          TimeSpan?          RequestTimeout      = null,
                                                          EventTracking_Id?  EventTrackingId     = null,
                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.ReserveNow(
                   new ReserveNowRequest(
                       ChargeBoxId,
                       ReservationId,
                       ExpiryDate,
                       IdToken,
                       ConnectorType,
                       EVSEId,
                       GroupIdToken,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region CancelReservation          (ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ReservationId">The unique identification of the reservation to cancel.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CancelReservationResponse> CancelReservation(this ICSMSClient   ICSMSClient,
                                                                        ChargeBox_Id       ChargeBoxId,
                                                                        Reservation_Id     ReservationId,
                                                                        CustomData?        CustomData          = null,

                                                                        Request_Id?        RequestId           = null,
                                                                        DateTime?          RequestTimestamp    = null,
                                                                        TimeSpan?          RequestTimeout      = null,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        CancellationToken  CancellationToken   = default)

            => ICSMSClient.CancelReservation(
                   new CancelReservationRequest(
                       ChargeBoxId,
                       ReservationId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region StartCharging              (ChargeBoxId, RequestStartTransactionRequestId, IdToken, EVSEId = null, ChargingProfile = null, GroupIdToken = null, ...)

        /// <summary>
        /// Start a charging process (transaction).
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="RequestStartTransactionRequestId">Request identification given by the server to this start request. The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.</param>
        /// <param name="IdToken">The identification token to start the charging transaction.</param>
        /// <param name="EVSEId">An optional EVSE identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charging station for the requested charging transaction.</param>
        /// <param name="GroupIdToken">An optional group identifier.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RequestStartTransactionResponse> StartCharging(this ICSMSClient   ICSMSClient,
                                                                          ChargeBox_Id       ChargeBoxId,
                                                                          RemoteStart_Id     RequestStartTransactionRequestId,
                                                                          IdToken            IdToken,
                                                                          EVSE_Id?           EVSEId              = null,
                                                                          ChargingProfile?   ChargingProfile     = null,
                                                                          IdToken?           GroupIdToken        = null,
                                                                          CustomData?        CustomData          = null,

                                                                          Request_Id?        RequestId           = null,
                                                                          DateTime?          RequestTimestamp    = null,
                                                                          TimeSpan?          RequestTimeout      = null,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.StartCharging(
                   new RequestStartTransactionRequest(
                       ChargeBoxId,
                       RequestStartTransactionRequestId,
                       IdToken,
                       EVSEId,
                       ChargingProfile,
                       GroupIdToken,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region StopCharging               (ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Start a charging process (transaction).
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="TransactionId">The identification of the transaction which the charging station is requested to stop.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<RequestStopTransactionResponse> StopCharging(this ICSMSClient   ICSMSClient,
                                                                        ChargeBox_Id       ChargeBoxId,
                                                                        Transaction_Id     TransactionId,
                                                                        CustomData?        CustomData          = null,

                                                                        Request_Id?        RequestId           = null,
                                                                        DateTime?          RequestTimestamp    = null,
                                                                        TimeSpan?          RequestTimeout      = null,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        CancellationToken  CancellationToken   = default)

            => ICSMSClient.StopCharging(
                   new RequestStopTransactionRequest(
                       ChargeBoxId,
                       TransactionId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetTransactionStatus       (ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Get the status of a charging process (transaction).
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="TransactionId">The identification of the transaction which the charging station is requested to stop.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetTransactionStatusResponse> GetTransactionStatus(this ICSMSClient   ICSMSClient,
                                                                              ChargeBox_Id       ChargeBoxId,
                                                                              Transaction_Id     TransactionId,
                                                                              CustomData?        CustomData          = null,

                                                                              Request_Id?        RequestId           = null,
                                                                              DateTime?          RequestTimestamp    = null,
                                                                              TimeSpan?          RequestTimeout      = null,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              CancellationToken  CancellationToken   = default)

            => ICSMSClient.GetTransactionStatus(
                   new GetTransactionStatusRequest(
                       ChargeBoxId,
                       TransactionId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region SetChargingProfile         (ChargeBoxId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given EVSE at the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="EVSEId">The EVSE identification to which the charging profile applies.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetChargingProfileResponse> SetChargingProfile(this ICSMSClient   ICSMSClient,
                                                                          ChargeBox_Id       ChargeBoxId,
                                                                          EVSE_Id            EVSEId,
                                                                          ChargingProfile    ChargingProfile,
                                                                          CustomData?        CustomData          = null,

                                                                          Request_Id?        RequestId           = null,
                                                                          DateTime?          RequestTimestamp    = null,
                                                                          TimeSpan?          RequestTimeout      = null,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          CancellationToken  CancellationToken   = default)

            => ICSMSClient.SetChargingProfile(
                   new SetChargingProfileRequest(
                       ChargeBoxId,
                       EVSEId,
                       ChargingProfile,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetChargingProfiles        (ChargeBoxId, GetChargingProfilesRequestId, ChargingProfile, EVSEId = null, ...)

        /// <summary>
        /// Get all charging profiles from the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="GetChargingProfilesRequestId">An unique identification of the get charging profiles request.</param>
        /// <param name="ChargingProfile">Machting charging profiles.</param>
        /// <param name="EVSEId">Optional EVSE identification of the EVSE for which the installed charging profiles SHALL be reported. If 0, only charging profiles installed on the charging station itself (the grid connection) SHALL be reported.If omitted, all installed charging profiles SHALL be reported.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetChargingProfilesResponse> GetChargingProfiles(this ICSMSClient          ICSMSClient,
                                                                            ChargeBox_Id              ChargeBoxId,
                                                                            Int64                     GetChargingProfilesRequestId,
                                                                            ChargingProfileCriterion  ChargingProfile,
                                                                            EVSE_Id?                  EVSEId              = null,
                                                                            CustomData?               CustomData          = null,

                                                                            Request_Id?               RequestId           = null,
                                                                            DateTime?                 RequestTimestamp    = null,
                                                                            TimeSpan?                 RequestTimeout      = null,
                                                                            EventTracking_Id?         EventTrackingId     = null,
                                                                            CancellationToken         CancellationToken   = default)

            => ICSMSClient.GetChargingProfiles(
                   new GetChargingProfilesRequest(
                       ChargeBoxId,
                       GetChargingProfilesRequestId,
                       ChargingProfile,
                       EVSEId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearChargingProfile       (ChargeBoxId, ChargingProfileId, ClearChargingProfile, ...)

        /// <summary>
        /// Remove matching charging profiles from the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearChargingProfileResponse> ClearChargingProfile(this ICSMSClient       ICSMSClient,
                                                                              ChargeBox_Id           ChargeBoxId,
                                                                              ChargingProfile_Id?    ChargingProfileId      = null,
                                                                              ClearChargingProfile?  ClearChargingProfile   = null,
                                                                              CustomData?            CustomData             = null,

                                                                              Request_Id?            RequestId              = null,
                                                                              DateTime?              RequestTimestamp       = null,
                                                                              TimeSpan?              RequestTimeout         = null,
                                                                              EventTracking_Id?      EventTrackingId        = null,
                                                                              CancellationToken      CancellationToken      = default)

            => ICSMSClient.ClearChargingProfile(
                   new ClearChargingProfileRequest(
                       ChargeBoxId,
                       ChargingProfileId,
                       ClearChargingProfile,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetCompositeSchedule       (ChargeBoxId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule at the given charging station and EVSE
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="EVSEId">The EVSE identification for which the schedule is requested. EVSE identification is 0, the charging station will calculate the expected consumption for the grid connection.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetCompositeScheduleResponse> GetCompositeSchedule(this ICSMSClient    ICSMSClient,
                                                                              ChargeBox_Id        ChargeBoxId,
                                                                              TimeSpan            Duration,
                                                                              EVSE_Id             EVSEId,
                                                                              ChargingRateUnits?  ChargingRateUnit    = null,
                                                                              CustomData?         CustomData          = null,

                                                                              Request_Id?         RequestId           = null,
                                                                              DateTime?           RequestTimestamp    = null,
                                                                              TimeSpan?           RequestTimeout      = null,
                                                                              EventTracking_Id?   EventTrackingId     = null,
                                                                              CancellationToken   CancellationToken   = default)

            => ICSMSClient.GetCompositeSchedule(
                   new GetCompositeScheduleRequest(
                       ChargeBoxId,
                       Duration,
                       EVSEId,
                       ChargingRateUnit,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region NotifyAllowedEnergyTransfer(ChargeBoxId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Update the list of authorized energy services.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AllowedEnergyTransferModes">An enumeration of allowed energy transfer modes.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(this ICSMSClient                  ICSMSClient,
                                                                                            ChargeBox_Id                      ChargeBoxId,
                                                                                            IEnumerable<EnergyTransferModes>  AllowedEnergyTransferModes,
                                                                                            CustomData?                       CustomData          = null,

                                                                                            Request_Id?                       RequestId           = null,
                                                                                            DateTime?                         RequestTimestamp    = null,
                                                                                            TimeSpan?                         RequestTimeout      = null,
                                                                                            EventTracking_Id?                 EventTrackingId     = null,
                                                                                            CancellationToken                 CancellationToken   = default)

            => ICSMSClient.NotifyAllowedEnergyTransfer(
                   new NotifyAllowedEnergyTransferRequest(
                       ChargeBoxId,
                       AllowedEnergyTransferModes,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region UnlockConnector            (ChargeBoxId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given EVSE/connector at the given charging station.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="EVSEId">The identifier of the EVSE to be unlocked.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<UnlockConnectorResponse> UnlockConnector(this ICSMSClient   ICSMSClient,
                                                                    ChargeBox_Id       ChargeBoxId,
                                                                    EVSE_Id            EVSEId,
                                                                    Connector_Id       ConnectorId,
                                                                    CustomData?        CustomData          = null,

                                                                    Request_Id?        RequestId           = null,
                                                                    DateTime?          RequestTimestamp    = null,
                                                                    TimeSpan?          RequestTimeout      = null,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    CancellationToken  CancellationToken   = default)

            => ICSMSClient.UnlockConnector(
                   new UnlockConnectorRequest(
                       ChargeBoxId,
                       EVSEId,
                       ConnectorId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region SendAFRRSignal             (ChargeBoxId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Send an automatic frequency restoration reserve (AFRR) signal.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
        /// <param name="Signal">The value of the AFRR signal in v2xSignalWattCurve. Usually between -1 and 1.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<AFRRSignalResponse> SendAFRRSignal(this ICSMSClient   ICSMSClient,
                                                              ChargeBox_Id       ChargeBoxId,
                                                              DateTime           ActivationTimestamp,
                                                              AFRR_Signal        Signal,
                                                              CustomData?        CustomData          = null,

                                                              Request_Id?        RequestId           = null,
                                                              DateTime?          RequestTimestamp    = null,
                                                              TimeSpan?          RequestTimeout      = null,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              CancellationToken  CancellationToken   = default)

            => ICSMSClient.AFRRSignal(
                   new AFRRSignalRequest(
                       ChargeBoxId,
                       ActivationTimestamp,
                       Signal,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


        #region SetDisplayMessage          (ChargeBoxId, Message, ...)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Message">A display message to be shown at the charging station.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<SetDisplayMessageResponse> SetDisplayMessage(this ICSMSClient   ICSMSClient,
                                                                        ChargeBox_Id       ChargeBoxId,
                                                                        MessageInfo        Message,
                                                                        CustomData?        CustomData          = null,

                                                                        Request_Id?        RequestId           = null,
                                                                        DateTime?          RequestTimestamp    = null,
                                                                        TimeSpan?          RequestTimeout      = null,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        CancellationToken  CancellationToken   = default)

            => ICSMSClient.SetDisplayMessage(
                   new SetDisplayMessageRequest(
                       ChargeBoxId,
                       Message,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region GetDisplayMessages         (ChargeBoxId, GetDisplayMessagesRequestId, Ids = null, Priority = null, State = null,...)

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetDisplayMessagesRequestId">The unique identification of this get display messages request.</param>
        /// <param name="Ids">An optional filter on display message identifications. This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.</param>
        /// <param name="Priority">The optional filter on message priorities.</param>
        /// <param name="State">The optional filter on message states.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<GetDisplayMessagesResponse> GetDisplayMessages(this ICSMSClient                 ICSMSClient,
                                                                          ChargeBox_Id                     ChargeBoxId,
                                                                          Int32                            GetDisplayMessagesRequestId,
                                                                          IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                                                                          MessagePriorities?               Priority            = null,
                                                                          MessageStates?                   State               = null,
                                                                          CustomData?                      CustomData          = null,

                                                                          Request_Id?                      RequestId           = null,
                                                                          DateTime?                        RequestTimestamp    = null,
                                                                          TimeSpan?                        RequestTimeout      = null,
                                                                          EventTracking_Id?                EventTrackingId     = null,
                                                                          CancellationToken                CancellationToken   = default)

            => ICSMSClient.GetDisplayMessages(
                   new GetDisplayMessagesRequest(
                       ChargeBoxId,
                       GetDisplayMessagesRequestId,
                       Ids,
                       Priority,
                       State,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region ClearDisplayMessage        (ChargeBoxId, DisplayMessageId,...)

        /// <summary>
        /// Remove the given display message.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DisplayMessageId">The identification of the display message to be removed.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<ClearDisplayMessageResponse> ClearDisplayMessage(this ICSMSClient   ICSMSClient,
                                                                            ChargeBox_Id       ChargeBoxId,
                                                                            DisplayMessage_Id  DisplayMessageId,
                                                                            CustomData?        CustomData          = null,

                                                                            Request_Id?        RequestId           = null,
                                                                            DateTime?          RequestTimestamp    = null,
                                                                            TimeSpan?          RequestTimeout      = null,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            CancellationToken  CancellationToken   = default)

            => ICSMSClient.ClearDisplayMessage(
                   new ClearDisplayMessageRequest(
                       ChargeBoxId,
                       DisplayMessageId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region CostUpdated                (ChargeBoxId, TotalCost, TransactionId, ...)

        /// <summary>
        /// Send updated cost(s).
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TotalCost">The current total cost, based on the information known by the CSMS, of the transaction including taxes. In the currency configured with the configuration Variable: [Currency]</param>
        /// <param name="TransactionId">The unique transaction identification the costs are asked for.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CostUpdatedResponse> SendCostUpdated(this ICSMSClient   ICSMSClient,
                                                                ChargeBox_Id       ChargeBoxId,
                                                                Decimal            TotalCost,
                                                                Transaction_Id     TransactionId,
                                                                CustomData?        CustomData          = null,

                                                                Request_Id?        RequestId           = null,
                                                                DateTime?          RequestTimestamp    = null,
                                                                TimeSpan?          RequestTimeout      = null,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                CancellationToken  CancellationToken   = default)

            => ICSMSClient.SendCostUpdated(
                   new CostUpdatedRequest(
                       ChargeBoxId,
                       TotalCost,
                       TransactionId,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion

        #region RequestCustomerInformation (ChargeBoxId, CustomerInformationRequestId, Report, Clear, CustomerIdentifier = null, IdToken = null, CustomerCertificate = null, ...)

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="ICSMSClient">A CSMS client.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomerInformationRequestId">An unique identification of the customer information request.</param>
        /// <param name="Report">Whether the charging station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.</param>
        /// <param name="Clear">Whether the charging station should clear all information about the customer referred to.</param>
        /// <param name="CustomerIdentifier">An optional e.g. vendor specific identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.</param>
        /// <param name="IdToken">An optional IdToken of the customer this request refers to.</param>
        /// <param name="CustomerCertificate">An optional certificate of the customer this request refers to.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        public static Task<CustomerInformationResponse> RequestCustomerInformation(this ICSMSClient      ICSMSClient,
                                                                                   ChargeBox_Id          ChargeBoxId,
                                                                                   Int64                 CustomerInformationRequestId,
                                                                                   Boolean               Report,
                                                                                   Boolean               Clear,
                                                                                   CustomerIdentifier?   CustomerIdentifier    = null,
                                                                                   IdToken?              IdToken               = null,
                                                                                   CertificateHashData?  CustomerCertificate   = null,
                                                                                   CustomData?           CustomData            = null,

                                                                                   Request_Id?           RequestId             = null,
                                                                                   DateTime?             RequestTimestamp      = null,
                                                                                   TimeSpan?             RequestTimeout        = null,
                                                                                   EventTracking_Id?     EventTrackingId       = null,
                                                                                   CancellationToken     CancellationToken     = default)

            => ICSMSClient.RequestCustomerInformation(
                   new CustomerInformationRequest(
                       ChargeBoxId,
                       CustomerInformationRequestId,
                       Report,
                       Clear,
                       CustomerIdentifier,
                       IdToken,
                       CustomerCertificate,
                       CustomData,

                       RequestId,
                       RequestTimestamp,
                       RequestTimeout,
                       EventTrackingId,
                       CancellationToken
                   )
               );

        #endregion


    }


    /// <summary>
    /// The common interface of all CSMS clients.
    /// </summary>
    public interface ICSMSClientEvents
    {

        #region Reset

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the CSMS.
        /// </summary>
        event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the CSMS.
        /// </summary>
        event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the CSMS.
        /// </summary>
        event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the CSMS.
        /// </summary>
        event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the CSMS.
        /// </summary>
        event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the CSMS.
        /// </summary>
        event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the CSMS.
        /// </summary>
        event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the CSMS.
        /// </summary>
        event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the CSMS.
        /// </summary>
        event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the CSMS.
        /// </summary>
        event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the CSMS.
        /// </summary>
        event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the CSMS.
        /// </summary>
        event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the CSMS.
        /// </summary>
        event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the CSMS.
        /// </summary>
        event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the CSMS.
        /// </summary>
        event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region TransferData

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the CSMS.
        /// </summary>
        event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the CSMS.
        /// </summary>
        event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the CSMS.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the CSMS.
        /// </summary>
        event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the CSMS.
        /// </summary>
        event OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        event OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the CSMS.
        /// </summary>
        event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the CSMS.
        /// </summary>
        event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the CSMS.
        /// </summary>
        event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the CSMS.
        /// </summary>
        event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the CSMS.
        /// </summary>
        event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the CSMS.
        /// </summary>
        event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the CSMS.
        /// </summary>
        event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the CSMS.
        /// </summary>
        event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the CSMS.
        /// </summary>
        event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the CSMS.
        /// </summary>
        event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the CSMS.
        /// </summary>
        event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the CSMS.
        /// </summary>
        event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the CSMS.
        /// </summary>
        event OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        event OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        // UsePriorityChargingRequest

        // UpdateDynamicScheduleRequest

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the CSMS.
        /// </summary>
        event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to the CSMS.
        /// </summary>
        event OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        event OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the CSMS.
        /// </summary>
        event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the CSMS.
        /// </summary>
        event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the CSMS.
        /// </summary>
        event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the CSMS.
        /// </summary>
        event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the CSMS.
        /// </summary>
        event OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        event OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        #endregion


    }


    /// <summary>
    /// The common interface of all CSMS clients.
    /// </summary>
    public interface ICSMSClient : ICSMSClientEvents
    {

        ChargeBox_Id                            ChargeBoxIdentity    { get; }
        String                                  From                 { get; }
        String                                  To                   { get; }

        //     CSMSSOAPClient.CSClientLogger  Logger               { get; }


        #region Reset

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request);

        #endregion

        #region PublishFirmware

        /// <summary>
        /// Publish a firmware.
        /// </summary>
        /// <param name="Request">A publish firmware request.</param>
        Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request);

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// Unpublish a firmware.
        /// </summary>
        /// <param name="Request">An unpublish firmware request.</param>
        Task<UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request);

        #endregion

        #region GetBaseReport

        /// <summary>
        /// Get a base report.
        /// </summary>
        /// <param name="Request">A get base report request.</param>
        Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request);

        #endregion

        #region GetReport

        /// <summary>
        /// Get a report.
        /// </summary>
        /// <param name="Request">A get report request.</param>
        Task<GetReportResponse> GetReport(GetReportRequest Request);

        #endregion

        #region GetLog

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        Task<GetLogResponse> GetLog(GetLogRequest Request);

        #endregion

        #region SetVariables

        /// <summary>
        /// Set variables.
        /// </summary>
        /// <param name="Request">A set variables request.</param>
        Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request);

        #endregion

        #region GetVariables

        /// <summary>
        /// Get all variables.
        /// </summary>
        /// <param name="Request">A get variables request.</param>
        Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request);

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// Set the monitoring base.
        /// </summary>
        /// <param name="Request">A set monitoring base request.</param>
        Task<SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request);

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// Get a monitoring report.
        /// </summary>
        /// <param name="Request">A get monitoring report request.</param>
        Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request);

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// Set the monitoring level.
        /// </summary>
        /// <param name="Request">A set monitoring level request.</param>
        Task<SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request);

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// Set a variable monitoring.
        /// </summary>
        /// <param name="Request">A set variable monitoring request.</param>
        Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request);

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// Remove the given variable monitoring.
        /// </summary>
        /// <param name="Request">A clear variable monitoring request.</param>
        Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request);

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// Set the network profile.
        /// </summary>
        /// <param name="Request">A set network profile request.</param>
        Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request);

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// Change the availability of the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request);

        #endregion

        #region TriggerMessage

        /// <summary>
        /// Create a trigger for the given message at the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request);

        #endregion

        #region TransferData

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        Task<CS.DataTransferResponse> TransferData(DataTransferRequest Request);

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        Task<CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request);

        #endregion

        #region InstallCertificate

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request);

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request);

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// Remove the given certificate from the charging station.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request);

        #endregion

        #region NotifyCRL

        /// <summary>
        /// Notify the charging station about the status of a certificate revocation list.
        /// </summary>
        /// <param name="Request">A NotifyCRL request.</param>
        Task<NotifyCRLResponse> NotifyCRL(NotifyCRLRequest Request);

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request);

        #endregion

        #region SendLocalList

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request);

        #endregion

        #region ClearCache

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request);

        #endregion


        #region ReserveNow

        /// <summary>
        /// Create a charging reservation at the given charging station.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request);

        #endregion

        #region CancelReservation

        /// <summary>
        /// Cancel the given charging reservation.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request);

        #endregion

        #region StartCharging

        /// <summary>
        /// Start a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request start transaction request.</param>
        Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request);

        #endregion

        #region StopCharging

        /// <summary>
        /// Stop a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request stop transaction request.</param>
        Task<RequestStopTransactionResponse> StopCharging(RequestStopTransactionRequest Request);

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// Get the status of a charging process (transaction).
        /// </summary>
        /// <param name="Request">A get transaction status request.</param>
        Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request);

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// Set the charging profile of the given EVSE at the given charging station.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request);

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// Get all charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A get charging profiles request.</param>
        Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request);

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// Remove matching charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request);

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// Return the charging schedule at the given charging station and EVSE.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request);

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// Update the list of authorized energy services.
        /// </summary>
        /// <param name="Request">A notify allowed energy transfer request.</param>
        Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request);

        #endregion

        #region UnlockConnector

        /// <summary>
        /// Unlock the given EVSE/connector at the given charging station.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request);

        #endregion


        #region AFRRSignal

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<AFRRSignalResponse> AFRRSignal(AFRRSignalRequest Request);

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A set display message request.</param>
        Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request);

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="Request">A get display messages request.</param>
        Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request);

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// Remove the given display message.
        /// </summary>
        /// <param name="Request">A clear display message request.</param>
        Task<ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request);

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// Send updated cost(s).
        /// </summary>
        /// <param name="Request">A cost updated request.</param>
        Task<CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request);

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="Request">A customer information request.</param>
        Task<CustomerInformationResponse> RequestCustomerInformation(CustomerInformationRequest Request);

        #endregion


    }

}
