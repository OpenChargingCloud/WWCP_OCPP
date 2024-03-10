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
using cloud.charging.open.protocols.OCPPv2_1.CS2;
using cloud.charging.open.protocols.OCPPv2_1.LC;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS2
{

    /// <summary>
    /// Extension methods for all CSMS2.
    /// </summary>
    public static class ICSMS2Extensions
    {

        #region as CSMS

        #region Reset                       (DestinationNodeId, ResetType, EVSEId = null, ...)

        /// <summary>
        /// Reset the given charging station/networking node.
        /// </summary>
        /// <param name="DestinationNodeId">The charging station/networking node identification.</param>
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

            Reset(this ICSMS2                   NetworkingNode,
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

                => NetworkingNode.OCPP.OUT.Reset(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateFirmware              (DestinationNodeId, Firmware, UpdateFirmwareRequestId, ...)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            UpdateFirmware(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.UpdateFirmware(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region PublishFirmware             (DestinationNodeId, PublishFirmwareRequestId, DownloadLocation, MD5Checksum, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            PublishFirmware(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.PublishFirmware(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UnpublishFirmware           (DestinationNodeId, MD5Checksum, ...)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            UnpublishFirmware(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.UnpublishFirmware(
                       new UnpublishFirmwareRequest(
                           DestinationNodeId,
                           MD5Checksum,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetBaseReport               (DestinationNodeId, GetBaseReportRequestId, ReportBase, ...)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetBaseReport(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetBaseReport(
                       new GetBaseReportRequest(
                           DestinationNodeId,
                           GetBaseReportRequestId,
                           ReportBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetReport                   (DestinationNodeId, GetReportRequestId, ComponentCriteria, ComponentVariables, ...)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetReport(this ICSMS2                     NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetReport(
                       new GetReportRequest(
                           DestinationNodeId,
                           GetReportRequestId,
                           ComponentCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetLog                      (DestinationNodeId, LogType, LogRequestId, Log, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetLog(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetLog(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SetVariables                (DestinationNodeId, VariableData, DataConsistencyModel = null, ...)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetVariables(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetVariables(
                       new SetVariablesRequest(
                           DestinationNodeId,
                           VariableData,
                           DataConsistencyModel,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetVariables                (DestinationNodeId, VariableData, ...)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetVariables(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetVariables(
                       new GetVariablesRequest(
                           DestinationNodeId,
                           VariableData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringBase           (DestinationNodeId, MonitoringBase, ...)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetMonitoringBase(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetMonitoringBase(
                       new SetMonitoringBaseRequest(
                           DestinationNodeId,
                           MonitoringBase,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetMonitoringReport         (DestinationNodeId, GetMonitoringReportRequestId, MonitoringCriteria, ComponentVariables, ...)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetMonitoringReport(this ICSMS2                       NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetMonitoringReport(
                       new GetMonitoringReportRequest(
                           DestinationNodeId,
                           GetMonitoringReportRequestId,
                           MonitoringCriteria,
                           ComponentVariables,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetMonitoringLevel          (DestinationNodeId, Severity, ...)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetMonitoringLevel(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetMonitoringLevel(
                       new SetMonitoringLevelRequest(
                           DestinationNodeId,
                           Severity,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetVariableMonitoring       (DestinationNodeId, MonitoringData, ...)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetVariableMonitoring(this ICSMS2                     NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetVariableMonitoring(
                       new SetVariableMonitoringRequest(
                           DestinationNodeId,
                           MonitoringData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearVariableMonitoring     (DestinationNodeId, VariableMonitoringIds, ...)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ClearVariableMonitoring(this ICSMS2                         NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ClearVariableMonitoring(
                       new ClearVariableMonitoringRequest(
                           DestinationNodeId,
                           VariableMonitoringIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetNetworkProfile           (DestinationNodeId, ConfigurationSlot, NetworkConnectionProfile, ...)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetNetworkProfile(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetNetworkProfile(
                       new SetNetworkProfileRequest(
                           DestinationNodeId,
                           ConfigurationSlot,
                           NetworkConnectionProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeAvailability          (DestinationNodeId, OperationalStatus, EVSE = null, ...)

        /// <summary>
        /// Change the availability of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ChangeAvailability(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           DestinationNodeId,
                           OperationalStatus,
                           EVSE,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region TriggerMessage              (DestinationNodeId, RequestedMessage, EVSEId = null, CustomTrigger = null, ...)

        /// <summary>
        /// Create a trigger for the given message at the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            TriggerMessage(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.TriggerMessage(
                       new TriggerMessageRequest(
                           DestinationNodeId,
                           RequestedMessage,
                           EVSE,
                           CustomTrigger,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region SendSignedCertificate       (DestinationNodeId, CertificateChain, CertificateType = null, ...)

        /// <summary>
        /// Send the signed certificate to the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SendSignedCertificate(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.CertificateSigned(
                       new CertificateSignedRequest(
                           DestinationNodeId,
                           CertificateChain,
                           CertificateType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region InstallCertificate          (DestinationNodeId, CertificateType, Certificate, ...)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            InstallCertificate(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.InstallCertificate(
                       new InstallCertificateRequest(
                           DestinationNodeId,
                           CertificateType,
                           Certificate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetInstalledCertificateIds  (DestinationNodeId, CertificateTypes, ...)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetInstalledCertificateIds(this ICSMS2                        NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetInstalledCertificateIds(
                       new GetInstalledCertificateIdsRequest(
                           DestinationNodeId,
                           CertificateTypes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteCertificate           (DestinationNodeId, CertificateHashData, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            DeleteCertificate(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.DeleteCertificate(
                       new DeleteCertificateRequest(
                           DestinationNodeId,
                           CertificateHashData,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region NotifyCRLAvailability       (DestinationNodeId, NotifyCRLRequestId, Availability, Location, ...)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            NotifyCRLAvailability(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.NotifyCRL(
                       new NotifyCRLRequest(
                           DestinationNodeId,
                           NotifyCRLRequestId,
                           Availability,
                           Location,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region GetLocalListVersion         (DestinationNodeId, ...)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetLocalListVersion(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetLocalListVersion(
                       new GetLocalListVersionRequest(
                           DestinationNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendLocalList               (DestinationNodeId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SendLocalList(this ICSMS2                      NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SendLocalList(
                       new SendLocalListRequest(
                           DestinationNodeId,
                           ListVersion,
                           UpdateType,
                           LocalAuthorizationList,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearCache                  (DestinationNodeId, ...)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ClearCache(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ClearCache(
                       new ClearCacheRequest(
                           DestinationNodeId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        #region ReserveNow                  (DestinationNodeId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a charging reservation of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ReserveNow(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ReserveNow(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region CancelReservation           (DestinationNodeId, ReservationId, ...)

        /// <summary>
        /// Cancel the given charging reservation at the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            CancelReservation(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.CancelReservation(
                       new CancelReservationRequest(
                           DestinationNodeId,
                           ReservationId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region StartCharging               (DestinationNodeId, RequestStartTransactionRequestId, IdToken, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            StartCharging(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.RequestStartTransaction(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region StopCharging                (DestinationNodeId, TransactionId, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            StopCharging(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.RequestStopTransaction(
                       new RequestStopTransactionRequest(
                           DestinationNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetTransactionStatus        (DestinationNodeId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetTransactionStatus(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetTransactionStatus(
                       new GetTransactionStatusRequest(
                           DestinationNodeId,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SetChargingProfile          (DestinationNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SetChargingProfile(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetChargingProfile(
                       new SetChargingProfileRequest(
                           DestinationNodeId,
                           EVSEId,
                           ChargingProfile,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetChargingProfiles         (DestinationNodeId, EVSEId, ChargingProfile, ...)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetChargingProfiles(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetChargingProfiles(
                       new GetChargingProfilesRequest(
                           DestinationNodeId,
                           GetChargingProfilesRequestId,
                           ChargingProfile,
                           EVSEId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ClearChargingProfile        (DestinationNodeId, ChargingProfileId, ChargingProfileCriteria, ...)

        /// <summary>
        /// Remove the charging profile at the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ClearChargingProfile(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ClearChargingProfile(
                       new ClearChargingProfileRequest(
                           DestinationNodeId,
                           ChargingProfileId,
                           ChargingProfileCriteria,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetCompositeSchedule        (DestinationNodeId, Duration, EVSEId, ChargingRateUnit = null, ...)

        /// <summary>
        /// Return the charging schedule of the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetCompositeSchedule(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetCompositeSchedule(
                       new GetCompositeScheduleRequest(
                           DestinationNodeId,
                           Duration,
                           EVSEId,
                           ChargingRateUnit,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UpdateDynamicSchedule       (DestinationNodeId, ChargingProfileId, Limit = null, ...)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            UpdateDynamicSchedule(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.UpdateDynamicSchedule(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyAllowedEnergyTransfer (DestinationNodeId, AllowedEnergyTransferModes, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            NotifyAllowedEnergyTransfer(this ICSMS2                      NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.NotifyAllowedEnergyTransfer(
                       new NotifyAllowedEnergyTransferRequest(
                           DestinationNodeId,
                           AllowedEnergyTransferModes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UsePriorityCharging         (DestinationNodeId, TransactionId, Activate, ...)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            UsePriorityCharging(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.UsePriorityCharging(
                       new UsePriorityChargingRequest(
                           DestinationNodeId,
                           TransactionId,
                           Activate,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region UnlockConnector             (DestinationNodeId, EVSEId, ConnectorId, ...)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            UnlockConnector(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.UnlockConnector(
                       new UnlockConnectorRequest(
                           DestinationNodeId,
                           EVSEId,
                           ConnectorId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SendAFRRSignal(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.AFRRSignal(
                       new AFRRSignalRequest(
                           DestinationNodeId,
                           ActivationTimestamp,
                           Signal,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            SetDisplayMessage(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetDisplayMessage(
                       new SetDisplayMessageRequest(
                           DestinationNodeId,
                           Message,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            GetDisplayMessages(this ICSMS2                      NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetDisplayMessages(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            ClearDisplayMessage(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.ClearDisplayMessage(
                       new ClearDisplayMessageRequest(
                           DestinationNodeId,
                           DisplayMessageId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            SendCostUpdated(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.CostUpdated(
                       new CostUpdatedRequest(
                           DestinationNodeId,
                           TotalCost,
                           TransactionId,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            RequestCustomerInformation(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.CustomerInformation(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            TransferData(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId         ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp  ?? Timestamp.Now,
                           RequestTimeout    ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId   ?? EventTracking_Id.New,
                           NetworkPath       ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken

                       )
                   );

        #endregion



        // Binary Data Streams Extensions

        #region TransferBinaryData          (DestinationNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            TransferBinaryData(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.BinaryDataTransfer(
                       new BinaryDataTransferRequest(
                           DestinationNodeId,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetFile                     (DestinationNodeId, Filename, Priority = null, ...)

        /// <summary>
        /// Request to download the given file from the given networking node.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            GetFile(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.GetFile(
                       new OCPP.CSMS.GetFileRequest(
                           DestinationNodeId,
                           Filename,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFile                    (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the given networking node.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            SendFile(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.SendFile(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteFile                  (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Delete the given file from the given networking node.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            DeleteFile(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.DeleteFile(
                       new OCPP.CSMS.DeleteFileRequest(
                           DestinationNodeId,
                           FileName,
                           FileSHA256,
                           FileSHA512,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ListDirectory               (DestinationNodeId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// List the given directory of the given networking node.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
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

            ListDirectory(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.ListDirectory(
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

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion



        // E2E Security Extensions

        #region TransferSecureData          (DestinationNodeId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given binary data to the given charging station.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification.</param>
        /// <param name="Parameter">Encryption parameters.</param>
        /// <param name="KeyId">The unique identification of the encryption key.</param>
        /// <param name="Payload">The unencrypted encapsulated security payload.</param>
        /// <param name="Key"></param>
        /// <param name="Nonce"></param>
        /// <param name="Counter"></param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SecureDataTransferResponse>

            TransferSecureData(this ICSMS2                   CSMS,

                               NetworkingNode_Id             DestinationNodeId,
                               UInt16                        Parameter,
                               Byte[]                        Payload,
                               UInt16?                       KeyId               = null,
                               Byte[]?                       Key                 = null,
                               UInt64?                       Nonce               = null,
                               UInt64?                       Counter             = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<OCPP.Signature>?  Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)

        {

            try
            {

                return CSMS.OCPP.OUT.SecureDataTransfer(
                           SecureDataTransferRequest.Encrypt(
                               DestinationNodeId,
                               Parameter,
                               KeyId   ?? 0,
                               Key     ?? CSMS.GetEncryptionKey    (DestinationNodeId, KeyId),
                               Nonce   ?? CSMS.GetEncryptionNonce  (DestinationNodeId, KeyId),
                               Counter ?? CSMS.GetEncryptionCounter(DestinationNodeId, KeyId),
                               Payload,

                               SignKeys,
                               SignInfos,
                               Signatures,

                               RequestId        ?? CSMS.OCPP.NextRequestId,
                               RequestTimestamp ?? Timestamp.Now,
                               RequestTimeout   ?? CSMS.OCPP.DefaultRequestTimeout,
                               EventTrackingId  ?? EventTracking_Id.New,
                               NetworkPath.From(CSMS.Id),
                               CancellationToken
                           )
                       );

            }
            catch (Exception e)
            {
                return Task.FromResult(
                           SecureDataTransferResponse.ExceptionOccured(e)
                       );
            }

        }

        #endregion



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

            SetDefaultChargingTariff(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.SetDefaultChargingTariff(
                       new SetDefaultChargingTariffRequest(
                           DestinationNodeId,
                           ChargingTariff,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            GetDefaultChargingTariff(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.GetDefaultChargingTariff(
                       new GetDefaultChargingTariffRequest(
                           DestinationNodeId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
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

            RemoveDefaultChargingTariff(this ICSMS2                   NetworkingNode,
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


                => NetworkingNode.OCPP.OUT.RemoveDefaultChargingTariff(
                       new RemoveDefaultChargingTariffRequest(
                           DestinationNodeId,
                           ChargingTariffId,
                           EVSEIds,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion



        // Overlay Networking Extensions

        #region NotifyNetworkTopology                 (NetworkingNode, ...)

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

            NotifyNetworkTopology(this ICSMS2                   NetworkingNode,

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


                => NetworkingNode.OCPP.OUT.NotifyNetworkTopology(
                       new OCPP.NN.NotifyNetworkTopologyRequest(

                           DestinationNodeId ?? NetworkingNode_Id.CSMS,

                           NetworkTopologyInformation,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyNetworkTopology                 (NetworkingNode, ...)

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

        //    NotifyNetworkTopology(this INetworkingNode2         NetworkingNode,

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


        //        => NetworkingNode.ocppOUT.NotifyNetworkTopology(
        //               new OCPP.NN.NotifyNetworkTopologyRequest(

        //                   DestinationNodeId,
        //                   NetworkTopologyInformation,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   CustomData,

        //                   RequestId        ?? NetworkingNode.OCPPAdapter.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
        //                   CancellationToken

        //               )
        //           );

        #endregion



    }

}
