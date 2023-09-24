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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{


    public class ChargingStationConnector
    {

        public Connector_Id  Id    { get; }


        public ChargingStationConnector(Connector_Id Id)
        {
            this.Id = Id;
        }

    }


    /// <summary>
    /// A charging station connector.
    /// </summary>
    public class ChargingStationEVSE
    {

        public EVSE_Id            Id                       { get; }

        public Reservation_Id?    ReservationId            { get; set; }

        public OperationalStatus  AdminStatus              { get; set; }

        public ConnectorStatus    Status                   { get; set; }


        public String?            MeterType                { get; set; }
        public String?            MeterSerialNumber        { get; set; }
        public String?            MeterPublicKey           { get; set; }


        public Boolean            IsReserved               { get; set; }

        public Boolean            IsCharging               { get; set; }

        public IdToken?           IdToken                  { get; set; }

        public IdToken?           GroupIdToken             { get; set; }

        public Transaction_Id?    TransactionId            { get; set; }

        public RemoteStart_Id?    RemoteStartId            { get; set; }

        public ChargingProfile?   ChargingProfile          { get; set; }


        public DateTime?          StartTimestamp           { get; set; }

        public Decimal?           MeterStartValue          { get; set; }

        public String?            SignedStartMeterValue    { get; set; }

        public DateTime?          StopTimestamp            { get; set; }

        public Decimal?           MeterStopValue           { get; set; }

        public String?            SignedStopMeterValue     { get; set; }


        public ChargingStationEVSE(EVSE_Id                                 Id,
                                   OperationalStatus                       Status,
                                   String?                                 MeterType           = null,
                                   String?                                 MeterSerialNumber   = null,
                                   String?                                 MeterPublicKey      = null,
                                   IEnumerable<ChargingStationConnector>?  Connectors          = null)
        {

            this.Id                 = Id;
            this.AdminStatus             = Status;
            this.MeterType          = MeterType;
            this.MeterSerialNumber  = MeterSerialNumber;
            this.MeterPublicKey     = MeterPublicKey;
            this.connectors         = Connectors is not null && Connectors.Any()
                                          ? new HashSet<ChargingStationConnector>(Connectors)
                                          : new HashSet<ChargingStationConnector>();

        }


        private HashSet<ChargingStationConnector>  connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }


    }



    /// <summary>
    /// A charging station for testing.
    /// </summary>
    public class TestChargingStation : IChargingStationClientEvents,
                                       IEventSender
    {

        public class EnqueuedRequest
        {

            public enum EnqueuedStatus
            {
                New,
                Processing,
                Finished
            }

            public String          Command           { get; }

            public IRequest        Request           { get; }

            public JObject         RequestJSON       { get; }

            public DateTime        EnqueTimestamp    { get; }

            public EnqueuedStatus  Status            { get; set; }

            public Action<Object>  ResponseAction    { get; }

            public EnqueuedRequest(String          Command,
                                   IRequest        Request,
                                   JObject         RequestJSON,
                                   DateTime        EnqueTimestamp,
                                   EnqueuedStatus  Status,
                                   Action<Object>  ResponseAction)
            {

                this.Command         = Command;
                this.Request         = Request;
                this.RequestJSON     = RequestJSON;
                this.EnqueTimestamp  = EnqueTimestamp;
                this.Status          = Status;
                this.ResponseAction  = ResponseAction;

            }

        }



        #region Data

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly             TimeSpan                    DefaultSendHeartbeatEvery   = TimeSpan.FromSeconds(30);

        protected static readonly   TimeSpan                    SemaphoreSlimTimeout        = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public readonly             TimeSpan                    DefaultMaintenanceEvery     = TimeSpan.FromSeconds(1);
        private static readonly     SemaphoreSlim               MaintenanceSemaphore        = new (1, 1);
        private readonly            Timer                       MaintenanceTimer;

        private readonly            Timer                       SendHeartbeatTimer;


        private readonly            List<EnqueuedRequest>         EnqueuedRequests;

        public                      IHTTPAuthentication?        HTTPAuthentication          { get; }
        public                      DNSClient?                  DNSClient                   { get; }

        private                     Int64                       internalRequestId           = 100000;

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a CSMS.
        /// </summary>
        public IChargingStationClient?      CSClient                    { get; private set; }


        public String? ClientCloseMessage
            => CSClient?.ClientCloseMessage;


        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxId.ToString();



        /// <summary>
        /// The charge box identification.
        /// </summary>
        public ChargeBox_Id             ChargeBoxId                 { get; }

        /// <summary>
        /// The charging station vendor identification.
        /// </summary>
        [Mandatory]
        public String                   VendorName                  { get; }

        /// <summary>
        ///  The charging station model identification.
        /// </summary>
        [Mandatory]
        public String                   Model                       { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        [Optional]
        public I18NString?              Description                 { get; }

        /// <summary>
        /// The optional serial number of the charging station.
        /// </summary>
        [Optional]
        public String?                  SerialNumber                { get; }

        /// <summary>
        /// The optional firmware version of the charging station.
        /// </summary>
        [Optional]
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The modem of the charging station.
        /// </summary>
        [Optional]
        public Modem?                   Modem                       { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charging station.
        /// </summary>
        [Optional]
        public String?                  MeterPublicKey              { get; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; set; }

        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                CSMSTime           { get; private set; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                 DefaultRequestTimeout       { get; }




        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan                 MaintenanceEvery            { get; }

        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean                  DisableMaintenanceTasks     { get; set; }

        /// <summary>
        /// Disable all heartbeats.
        /// </summary>
        public Boolean                  DisableSendHeartbeats       { get; set; }






        // Controlled by the CSMS!

        private readonly Dictionary<EVSE_Id, ChargingStationEVSE> evses;

        public IEnumerable<ChargingStationEVSE> EVSEs
            => evses.Values;

        #endregion

        #region Events

        #region Charging Station -> CSMS

        #region SendBootNotification

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the CSMS.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region SendFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region SendPublishFirmwareStatusNotification

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region SendHeartbeat

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion

        #region NotifyEvent

        /// <summary>
        /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        #endregion

        #region SendSecurityEventNotification

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region NotifyReport

        /// <summary>
        /// An event fired whenever a NotifyReport request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        #endregion

        #region NotifyMonitoringReport

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        #endregion

        #region SendLogStatusNotification

        /// <summary>
        /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region TransferData

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region SignCertificate

        /// <summary>
        /// An event fired whenever a SignCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion

        #region Get15118EVCertificate

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        #endregion

        #region GetCertificateStatus

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        #endregion

        #region GetCRL

        /// <summary>
        /// An event fired whenever a GetCRL request will be sent to the CSMS.
        /// </summary>
        public event OnGetCRLRequestDelegate?   OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OnGetCRLResponseDelegate?  OnGetCRLResponse;

        #endregion


        #region SendReservationStatusUpdate

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        #endregion

        #region Authorize

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region NotifyEVChargingNeeds

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        #endregion

        #region SendTransactionEvent

        /// <summary>
        /// An event fired whenever a TransactionEvent will be sent to the CSMS.
        /// </summary>
        public event OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        #endregion

        #region SendStatusNotification

        /// <summary>
        /// An event fired whenever a StatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region SendMeterValues

        /// <summary>
        /// An event fired whenever a MeterValues request will be sent to the CSMS.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region NotifyChargingLimit

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        #endregion

        #region SendClearedChargingLimit

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        #endregion

        #region ReportChargingProfiles

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        #endregion

        #region NotifyEVChargingSchedule

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        #endregion

        #region NotifyPriorityCharging

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        #endregion

        #region PullDynamicScheduleUpdate

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region NotifyDisplayMessages

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        #endregion

        #region NotifyCustomerInformation

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        #endregion

        #endregion

        #region Charging Station <- CSMS

        #region Reset

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the charging station.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the charging station.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the charging station.
        /// </summary>
        public event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the charging station.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the charging station.
        /// </summary>
        public event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the charging station.
        /// </summary>
        public event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the charging station.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the charging station.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the charging station.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the charging station.
        /// </summary>
        public event OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the charging station.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the charging station.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the charging station.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the charging station.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UpdateDynamicSchedule

        /// <summary>
        /// An event fired whenever an UpdateDynamicSchedule request will be sent to the charging station.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region UsePriorityCharging

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the charging station.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request will be sent to the charging station.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was received.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the charging station.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the charging station.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorName">The charging station vendor identification.</param>
        /// <param name="Model">The charging station model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charge box description.</param>
        /// <param name="SerialNumber">An optional serial number of the charging station.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charging station.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charging station.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charging station.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charging station.</param>
        /// 
        /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
        public TestChargingStation(ChargeBox_Id                       ChargeBoxId,
                                   String                             VendorName,
                                   String                             Model,

                                   I18NString?                        Description               = null,
                                   String?                            SerialNumber              = null,
                                   String?                            FirmwareVersion           = null,
                                   Modem?                             Modem                     = null,

                                   IEnumerable<ChargingStationEVSE>?  EVSEs                     = null,

                                   String?                            MeterType                 = null,
                                   String?                            MeterSerialNumber         = null,
                                   String?                            MeterPublicKey            = null,

                                   Boolean                            DisableSendHeartbeats     = false,
                                   TimeSpan?                          SendHeartbeatEvery        = null,

                                   Boolean                            DisableMaintenanceTasks   = false,
                                   TimeSpan?                          MaintenanceEvery          = null,

                                   TimeSpan?                          DefaultRequestTimeout     = null,
                                   IHTTPAuthentication?               HTTPAuthentication        = null,
                                   DNSClient?                         DNSClient                 = null)

        {

            if (ChargeBoxId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxId),  "The given charge box identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),   "The given charging station vendor must not be null or empty!");

            if (Model.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),        "The given charging station model must not be null or empty!");


            this.ChargeBoxId              = ChargeBoxId;

            this.evses                    = EVSEs is not null && EVSEs.Any()
                                                ? EVSEs.ToDictionary(evse => evse.Id, evse => evse)
                                                : new Dictionary<EVSE_Id, ChargingStationEVSE>();

            //this.Configuration = new Dictionary<String, ConfigurationData> {
            //    { "hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false) },
            //    { "changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false) },
            //    { "doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false) },
            //    { "password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false) }
            //};
            this.EnqueuedRequests           = new List<EnqueuedRequest>();

            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.Description              = Description;
            this.SerialNumber             = SerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Modem                    = Modem;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

            this.DisableSendHeartbeats    = DisableSendHeartbeats;
            this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;
            this.SendHeartbeatTimer       = new Timer(
                                                DoSendHeartbeatSync,
                                                null,
                                                this.SendHeartbeatEvery,
                                                this.SendHeartbeatEvery
                                            );

            this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
            this.MaintenanceEvery         = MaintenanceEvery      ?? DefaultMaintenanceEvery;
            this.MaintenanceTimer         = new Timer(
                                                DoMaintenanceSync,
                                                null,
                                                this.MaintenanceEvery,
                                                this.MaintenanceEvery
                                            );

            this.HTTPAuthentication       = HTTPAuthentication;
            this.DNSClient                = DNSClient;

        }

        #endregion


        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }


        #region ConnectWebSocket(...)

        public async Task<HTTPResponse?> ConnectWebSocket(String                               From,
                                                          String                               To,

                                                          URL                                  RemoteURL,
                                                          HTTPHostname?                        VirtualHostname              = null,
                                                          String?                              Description                  = null,
                                                          RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                          X509Certificate?                     ClientCert                   = null,
                                                          SslProtocols?                        TLSProtocol                  = null,
                                                          Boolean?                             PreferIPv4                   = null,
                                                          String?                              HTTPUserAgent                = null,
                                                          IHTTPAuthentication?                 HTTPAuthentication           = null,
                                                          TimeSpan?                            RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                                          UInt16?                              MaxNumberOfRetries           = null,
                                                          UInt32?                              InternalBufferSize           = null,

                                                          IEnumerable<String>?                 SecWebSocketProtocols        = null,

                                                          Boolean                              DisableMaintenanceTasks      = false,
                                                          TimeSpan?                            MaintenanceEvery             = null,
                                                          Boolean                              DisableWebSocketPings        = false,
                                                          TimeSpan?                            WebSocketPingEvery           = null,
                                                          TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                          String?                              LoggingPath                  = null,
                                                          String?                              LoggingContext               = null,
                                                          LogfileCreatorDelegate?              LogfileCreator               = null,
                                                          HTTPClientLogger?                    HTTPLogger                   = null,
                                                          DNSClient?                           DNSClient                    = null)

        {

            var chargingStationWSClient = new ChargingStationWSClient(
                                              ChargeBoxId,
                                              From,
                                              To,

                                              RemoteURL,
                                              VirtualHostname,
                                              Description,
                                              PreferIPv4,
                                              RemoteCertificateValidator,
                                              ClientCertificateSelector,
                                              ClientCert,
                                              TLSProtocol,
                                              HTTPUserAgent,
                                              HTTPAuthentication ?? this.HTTPAuthentication,
                                              RequestTimeout,
                                              TransmissionRetryDelay,
                                              MaxNumberOfRetries,
                                              InternalBufferSize,

                                              SecWebSocketProtocols ?? new[] { Version.WebSocketSubProtocolId },

                                              DisableWebSocketPings,
                                              WebSocketPingEvery,
                                              SlowNetworkSimulationDelay,

                                              DisableMaintenanceTasks,
                                              MaintenanceEvery,

                                              LoggingPath,
                                              LoggingContext,
                                              LogfileCreator,
                                              HTTPLogger,
                                              DNSClient ?? this.DNSClient
                                          );

            this.CSClient  = chargingStationWSClient;

            WireEvents(chargingStationWSClient);

            var response = await chargingStationWSClient.Connect();

            return response;

        }

        #endregion

        #region WireEvents(ChargingStationServer)


        private readonly ConcurrentDictionary<DisplayMessage_Id, MessageInfo>     displayMessages   = new ();
        private readonly ConcurrentDictionary<Reservation_Id,    Reservation_Id>  reservations      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Transaction>     transactions      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Decimal>         totalCosts        = new ();
        private readonly ConcurrentDictionary<CertificateUse,    Certificate>     certificates      = new ();

        public void WireEvents(IChargingStationServer ChargingStationServer)
        {

            #region OnReset

            ChargingStationServer.OnReset += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnResetRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnResetRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnResetRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnResetRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ResetType

                ResetResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid reset request for charge box '{Request.ChargeBoxId}'!"));

                    response = new ResetResponse(
                                   Request:      Request,
                                   Status:       ResetStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    var success = CryptoUtils.VerifyMessage(Request, Request.ToJSON(), true);


                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming '", Request.ResetType, "' reset request accepted."));

                    response = new ResetResponse(
                                   Request:      Request,
                                   Status:       ResetStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnResetResponse event

                var responseLogger = OnResetResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnResetResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnResetResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateFirmware

            ChargingStationServer.OnUpdateFirmware += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnUpdateFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUpdateFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUpdateFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Firmware,
                // UpdateFirmwareRequestId
                // Retries
                // RetryIntervals

                UpdateFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UpdateFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new UpdateFirmwareResponse(
                                   Request:      Request,
                                   Status:       UpdateFirmwareStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UpdateFirmware request for '" + Request.Firmware.FirmwareURL + "'.");

                    response = new UpdateFirmwareResponse(
                                   Request:      Request,
                                   Status:       UpdateFirmwareStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnUpdateFirmwareResponse event

                var responseLogger = OnUpdateFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUpdateFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnPublishFirmware

            ChargingStationServer.OnPublishFirmware += async (LogTimestamp,
                                                              Sender,
                                                              Request,
                                                              CancellationToken) => {

                #region Send OnPublishFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPublishFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPublishFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnPublishFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion

                // PublishFirmwareRequestId
                // DownloadLocation
                // MD5Checksum
                // Retries
                // RetryInterval

                PublishFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid PublishFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new PublishFirmwareResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming PublishFirmware request for '" + Request.DownloadLocation + "'.");

                    response = new PublishFirmwareResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnPublishFirmwareResponse event

                var responseLogger = OnPublishFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPublishFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnPublishFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnpublishFirmware

            ChargingStationServer.OnUnpublishFirmware += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnUnpublishFirmwareRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUnpublishFirmwareRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUnpublishFirmwareRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnpublishFirmwareRequest),
                                  e
                              );
                    }

                }

                #endregion

                // MD5Checksum

                UnpublishFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UnpublishFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new UnpublishFirmwareResponse(
                                   Request:      Request,
                                   Status:       UnpublishFirmwareStatus.Unknown,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UnpublishFirmware request for '" + Request.MD5Checksum + "'.");

                    response = new UnpublishFirmwareResponse(
                                   Request:      Request,
                                   Status:       UnpublishFirmwareStatus.Unpublished,
                                   CustomData:   null
                               );

                }


                #region Send OnUnpublishFirmwareResponse event

                var responseLogger = OnUnpublishFirmwareResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUnpublishFirmwareResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnpublishFirmwareResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetBaseReport

            ChargingStationServer.OnGetBaseReport += async (LogTimestamp,
                                                            Sender,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnGetBaseReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetBaseReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetBaseReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetBaseReportRequest),
                                  e
                              );
                    }

                }

                                                                #endregion

                // GetBaseReportRequestId
                // ReportBase

                GetBaseReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetBaseReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetBaseReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetBaseReport request accepted.");

                    response = new GetBaseReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnGetBaseReportResponse event

                var responseLogger = OnGetBaseReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetBaseReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetBaseReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetReport

            ChargingStationServer.OnGetReport += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnGetReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetReportRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GetReportRequestId
                // ComponentCriteria
                // ComponentVariables

                GetReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetReport request accepted.");

                    response = new GetReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnGetReportResponse event

                var responseLogger = OnGetReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetLog

            ChargingStationServer.OnGetLog += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnGetLogRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetLogRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetLogRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLogRequest),
                                  e
                              );
                    }

                }

                #endregion

                // LogType
                // LogRequestId
                // Log
                // Retries
                // RetryInterval

                GetLogResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetLog request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetLogResponse(
                                   Request:      Request,
                                   Status:       LogStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetLog request accepted.");

                    response = new GetLogResponse(
                                   Request:      Request,
                                   Status:       LogStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnGetLogResponse event

                var responseLogger = OnGetLogResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetLogResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLogResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariables

            ChargingStationServer.OnSetVariables += async (LogTimestamp,
                                                           Sender,
                                                           Request,
                                                           CancellationToken) => {

                #region Send OnSetVariablesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetVariablesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetVariablesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariablesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VariableData

                SetVariablesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetVariables request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetVariablesResponse(
                                   Request:              Request,
                                   SetVariableResults:   Array.Empty<SetVariableResult>(),
                                   CustomData:           null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetVariables request accepted.");

                    response = new SetVariablesResponse(
                                   Request:              Request,
                                   SetVariableResults:   Request.VariableData.Select(variableData => new SetVariableResult(
                                                                                                         Status:                SetVariableStatus.Accepted,
                                                                                                         Component:             variableData.Component,
                                                                                                         Variable:              variableData.Variable,
                                                                                                         AttributeType:         variableData.AttributeType,
                                                                                                         AttributeStatusInfo:   null,
                                                                                                         CustomData:            null
                                                                                                     )),
                                   CustomData:           null
                               );

                }


                #region Send OnSetVariablesResponse event

                var responseLogger = OnSetVariablesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetVariablesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariablesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetVariables

            ChargingStationServer.OnGetVariables += async (LogTimestamp,
                                                           Sender,
                                                           Request,
                                                           CancellationToken) => {

                #region Send OnGetVariablesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetVariablesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetVariablesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetVariablesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VariableData

                GetVariablesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetVariables request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetVariablesResponse(
                                   Request:      Request,
                                   Results:      Array.Empty<GetVariableResult>(),
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetVariables request accepted.");

                    response = new GetVariablesResponse(
                                   Request:      Request,
                                   Results:      Request.VariableData.Select(variableData => new GetVariableResult(
                                                                                                 AttributeStatus:       GetVariableStatus.Accepted,
                                                                                                 Component:             variableData.Component,
                                                                                                 Variable:              variableData.Variable,
                                                                                                 AttributeValue:        "",
                                                                                                 AttributeType:         variableData.AttributeType,
                                                                                                 AttributeStatusInfo:   null,
                                                                                                 CustomData:            null
                                                                                             )),
                                   CustomData:   null
                               );

                }


                #region Send OnGetVariablesResponse event

                var responseLogger = OnGetVariablesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetVariablesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetVariablesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringBase

            ChargingStationServer.OnSetMonitoringBase += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnSetMonitoringBaseRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetMonitoringBaseRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetMonitoringBaseRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringBaseRequest),
                                  e
                              );
                    }

                }

                #endregion

                // MonitoringBase

                SetMonitoringBaseResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetMonitoringBase request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetMonitoringBaseResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetMonitoringBase request accepted.");

                    response = new SetMonitoringBaseResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnSetMonitoringBaseResponse event

                var responseLogger = OnSetMonitoringBaseResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetMonitoringBaseResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringBaseResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetMonitoringReport

            ChargingStationServer.OnGetMonitoringReport += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnGetMonitoringReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetMonitoringReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetMonitoringReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetMonitoringReportRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GetMonitoringReportRequestId
                // MonitoringCriteria
                // ComponentVariables

                GetMonitoringReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetMonitoringReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetMonitoringReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetMonitoringReport request accepted.");

                    response = new GetMonitoringReportResponse(
                                   Request:      Request,
                                   Status:       GenericDeviceModelStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnGetMonitoringReportResponse event

                var responseLogger = OnGetMonitoringReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetMonitoringReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetMonitoringReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringLevel

            ChargingStationServer.OnSetMonitoringLevel += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnSetMonitoringLevelRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetMonitoringLevelRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetMonitoringLevelRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringLevelRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Severity

                SetMonitoringLevelResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetMonitoringLevel request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetMonitoringLevelResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetMonitoringLevel request accepted.");

                    response = new SetMonitoringLevelResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnSetMonitoringLevelResponse event

                var responseLogger = OnSetMonitoringLevelResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetMonitoringLevelResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetMonitoringLevelResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariableMonitoring

            ChargingStationServer.OnSetVariableMonitoring += async (LogTimestamp,
                                                                    Sender,
                                                                    Request,
                                                                    CancellationToken) => {

                #region Send OnSetVariableMonitoringRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetVariableMonitoringRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetVariableMonitoringRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariableMonitoringRequest),
                                  e
                              );
                    }

                }

                #endregion

                // MonitoringData

                SetVariableMonitoringResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetVariableMonitoring request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetVariableMonitoringResponse(
                                   Request:                Request,
                                   SetMonitoringResults:   Array.Empty<SetMonitoringResult>(),
                                   CustomData:             null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetVariableMonitoring request accepted.");

                    response = new SetVariableMonitoringResponse(
                                   Request:                Request,
                                   SetMonitoringResults:   Request.MonitoringData.Select(setMonitoringData => new SetMonitoringResult(
                                                                                                                  Status:                 SetMonitoringStatus.Accepted,
                                                                                                                  MonitorType:            setMonitoringData.MonitorType,
                                                                                                                  Severity:               setMonitoringData.Severity,
                                                                                                                  Component:              setMonitoringData.Component,
                                                                                                                  Variable:               setMonitoringData.Variable,
                                                                                                                  VariableMonitoringId:   setMonitoringData.VariableMonitoringId,
                                                                                                                  StatusInfo:             null,
                                                                                                                  CustomData:             null
                                                                                                              )),
                                   CustomData:             null
                               );

                }


                #region Send OnSetVariableMonitoringResponse event

                var responseLogger = OnSetVariableMonitoringResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetVariableMonitoringResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetVariableMonitoringResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearVariableMonitoring

            ChargingStationServer.OnClearVariableMonitoring += async (LogTimestamp,
                                                                      Sender,
                                                                      Request,
                                                                      CancellationToken) => {

                #region Send OnClearVariableMonitoringRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearVariableMonitoringRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearVariableMonitoringRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearVariableMonitoringRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VariableMonitoringIds

                ClearVariableMonitoringResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearVariableMonitoring request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearVariableMonitoringResponse(
                                   Request:                  Request,
                                   ClearMonitoringResults:   Array.Empty<ClearMonitoringResult>(),
                                   CustomData:               null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearVariableMonitoring request accepted.");

                    response = new ClearVariableMonitoringResponse(
                                   Request:                  Request,
                                   ClearMonitoringResults:   Request.VariableMonitoringIds.Select(variableMonitoringId => new ClearMonitoringResult(
                                                                                                                              Status:       ClearMonitoringStatus.Accepted,
                                                                                                                              Id:           variableMonitoringId,
                                                                                                                              StatusInfo:   null,
                                                                                                                              CustomData:   null
                                                                                                                          )),
                                   CustomData:               null
                               );

                }


                #region Send OnClearVariableMonitoringResponse event

                var responseLogger = OnClearVariableMonitoringResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearVariableMonitoringResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearVariableMonitoringResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetNetworkProfile

            ChargingStationServer.OnSetNetworkProfile += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnSetNetworkProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetNetworkProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetNetworkProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetNetworkProfileRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ConfigurationSlot
                // NetworkConnectionProfile

                SetNetworkProfileResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetNetworkProfile request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetNetworkProfileResponse(
                                   Request:      Request,
                                   Status:       SetNetworkProfileStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetNetworkProfile request accepted.");

                    response = new SetNetworkProfileResponse(
                                   Request:      Request,
                                   Status:       SetNetworkProfileStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnSetNetworkProfileResponse event

                var responseLogger = OnSetNetworkProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetNetworkProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetNetworkProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnChangeAvailability

            ChargingStationServer.OnChangeAvailability += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnChangeAvailabilityRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnChangeAvailabilityRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnChangeAvailabilityRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnChangeAvailabilityRequest),
                                  e
                              );
                    }

                }

                #endregion

                // OperationalStatus
                // EVSE

                ChangeAvailabilityResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid ChangeAvailability request for charge box '{Request.ChargeBoxId}'!"));

                    response = new ChangeAvailabilityResponse(
                                   Request,
                                   ChangeAvailabilityStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming ChangeAvailability '", Request.OperationalStatus, "' request for EVSE '", Request.EVSE?.Id.ToString() ?? "?", "'."));

                    if (Request.EVSE is not null &&
                        evses.ContainsKey(Request.EVSE.Id))
                    {

                        evses[Request.EVSE.Id].AdminStatus = Request.OperationalStatus;

                        response = new ChangeAvailabilityResponse(
                                       Request:      Request,
                                       Status:       ChangeAvailabilityStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }
                    else
                        response = new ChangeAvailabilityResponse(
                                       Request,
                                       ChangeAvailabilityStatus.Rejected
                                   );
                }


                #region Send OnChangeAvailabilityResponse event

                var responseLogger = OnChangeAvailabilityResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnChangeAvailabilityResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnChangeAvailabilityResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnTriggerMessage

            ChargingStationServer.OnTriggerMessage += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnTriggerMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnTriggerMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnTriggerMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnTriggerMessageRequest),
                                  e
                              );
                    }

                }

                #endregion

                // RequestedMessage
                // EVSEId

                TriggerMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid TriggerMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new TriggerMessageResponse(
                                   Request,
                                   TriggerMessageStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at EVSE '" + (Request.EVSE?.Id.ToString() ?? "-") + "'.");

                    _ = Task.Run(async () => {

                        switch (Request.RequestedMessage) {

                            case MessageTriggers.BootNotification:
                                await SendBootNotification(
                                          BootReason:   BootReason.Triggered,
                                          CustomData:   null
                                      );
                                break;

                            case MessageTriggers.Heartbeat:
                                await SendHeartbeat(
                                          CustomData:   null
                                      );
                                break;

                            case MessageTriggers.StatusNotification:
                                if (Request.EVSE is not null)
                                    await SendStatusNotification(
                                              EVSEId:        Request.EVSE.Id,
                                              ConnectorId:   Connector_Id.Parse(1),
                                              Timestamp:     Timestamp.Now,
                                              Status:        evses[Request.EVSE.Id].Status,
                                              CustomData:    null
                                          );
                                break;

                        }

                    },
                    CancellationToken.None);



                    response = Request.RequestedMessage switch {

                                   MessageTriggers.BootNotification              or
                                   MessageTriggers.LogStatusNotification         or
                                   MessageTriggers.DiagnosticsStatusNotification or
                                   MessageTriggers.FirmwareStatusNotification    or
                                   MessageTriggers.Heartbeat                     or
                                   MessageTriggers.SignChargingStationCertificate

                                       => new TriggerMessageResponse(
                                              Request,
                                              TriggerMessageStatus.Accepted
                                          ),


                                   MessageTriggers.MeterValues or
                                   MessageTriggers.StatusNotification

                                       => Request.EVSE is not null
                                              ? new TriggerMessageResponse(
                                                    Request,
                                                    TriggerMessageStatus.Accepted
                                                )
                                              : new TriggerMessageResponse(
                                                    Request,
                                                    TriggerMessageStatus.Rejected
                                                ),


                                   _   => new TriggerMessageResponse(
                                              Request,
                                              TriggerMessageStatus.Rejected
                                          ),

                               };
                }


                #region Send OnTriggerMessageResponse event

                var responseLogger = OnTriggerMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnTriggerMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnTriggerMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnIncomingDataTransfer

            ChargingStationServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                                   Sender,
                                                                   Request,
                                                                   CancellationToken) => {

                #region Send OnDataTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnIncomingDataTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnIncomingDataTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnIncomingDataTransferRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VendorId
                // MessageId
                // Data

                DataTransferResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid data transfer request for charge box '{Request.ChargeBoxId}'!"));

                    response = new DataTransferResponse(
                                   Request,
                                   DataTransferStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming data transfer request: ", Request.VendorId, ".", Request.MessageId ?? "-", ": ", Request.Data ?? "-"));

                    var responseData = Request.Data;

                    if (Request.Data is not null)
                    {

                        if      (Request.Data.Type == JTokenType.String)
                            responseData = Request.Data.ToString().Reverse();

                        else if (Request.Data.Type == JTokenType.Object) {

                            var responseObject = new JObject();

                            foreach (var property in (Request.Data as JObject)!)
                            {
                                if (property.Value?.Type == JTokenType.String)
                                    responseObject.Add(property.Key,
                                                       property.Value.ToString().Reverse());
                            }

                            responseData = responseObject;

                        }

                        else if (Request.Data.Type == JTokenType.Array) {

                            var responseArray = new JArray();

                            foreach (var element in (Request.Data as JArray)!)
                            {
                                if (element?.Type == JTokenType.String)
                                    responseArray.Add(element.ToString().Reverse());
                            }

                            responseData = responseArray;

                        }

                    }

                    if (Request.VendorId.ToString() == "GraphDefined OEM")
                    {
                        response = new DataTransferResponse(
                                       Request,
                                       DataTransferStatus.Accepted,
                                       responseData
                                   );
                    }
                    else
                        response = new DataTransferResponse(
                                       Request,
                                       DataTransferStatus.Rejected
                                   );

                }


                #region Send OnDataTransferResponse event

                var responseLogger = OnIncomingDataTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnIncomingDataTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnIncomingDataTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnCertificateSigned

            ChargingStationServer.OnCertificateSigned += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnCertificateSignedRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCertificateSignedRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCertificateSignedRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCertificateSignedRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CertificateChain
                // CertificateType

                CertificateSignedResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid CertificateSigned request for charge box '{Request.ChargeBoxId}'!"));

                    response = new CertificateSignedResponse(
                                   Request:      Request,
                                   Status:       CertificateSignedStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming CertificateSigned request accepted."));

                    response = new CertificateSignedResponse(
                                   Request:      Request,
                                   Status:       Request.CertificateChain.FirstOrDefault()?.Parsed is not null
                                                     ? CertificateSignedStatus.Accepted
                                                     : CertificateSignedStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnCertificateSignedResponse event

                var responseLogger = OnCertificateSignedResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCertificateSignedResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCertificateSignedResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnInstallCertificate

            ChargingStationServer.OnInstallCertificate += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnInstallCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnInstallCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnInstallCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnInstallCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CertificateType
                // Certificate

                InstallCertificateResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid InstallCertificate request for charge box '{Request.ChargeBoxId}'!"));

                    response = new InstallCertificateResponse(
                                   Request:      Request,
                                   Status:       CertificateStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming InstallCertificate request accepted."));

                    var success = certificates.AddOrUpdate(Request.CertificateType,
                                                            a    => Request.Certificate,
                                                           (b,c) => Request.Certificate);

                    response = new InstallCertificateResponse(
                                   Request:      Request,
                                   Status:       Request.Certificate?.Parsed is not null
                                                     ? CertificateStatus.Accepted
                                                     : CertificateStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnInstallCertificateResponse event

                var responseLogger = OnInstallCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnInstallCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnInstallCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetInstalledCertificateIds

            ChargingStationServer.OnGetInstalledCertificateIds += async (LogTimestamp,
                                                                         Sender,
                                                                         Request,
                                                                         CancellationToken) => {

                #region Send OnGetInstalledCertificateIdsRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetInstalledCertificateIdsRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetInstalledCertificateIdsRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetInstalledCertificateIdsRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CertificateTypes

                GetInstalledCertificateIdsResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid GetInstalledCertificateIds request for charge box '{Request.ChargeBoxId}'!"));
                    response = new GetInstalledCertificateIdsResponse(Request:                    Request,
                                                                      Status:                     GetInstalledCertificateStatus.NotFound,
                                                                      CertificateHashDataChain:   Array.Empty<CertificateHashData>(),
                                                                      StatusInfo:                 null,
                                                                      CustomData:                 null);
                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming GetInstalledCertificateIds request accepted."));

                    var certs = new List<CertificateHashData>();

                    foreach (var certificateType in Request.CertificateTypes)
                    {

                        if (certificates.TryGetValue(certificateType, out var cert))
                            certs.Add(new CertificateHashData(
                                          HashAlgorithm:         HashAlgorithms.SHA256,
                                          IssuerNameHash:        cert.Parsed?.Issuer               ?? "-",
                                          IssuerPublicKeyHash:   cert.Parsed?.GetPublicKeyString() ?? "-",
                                          SerialNumber:          cert.Parsed?.SerialNumber         ?? "-",
                                          CustomData:            null
                                      ));

                    }

                    response = new GetInstalledCertificateIdsResponse(
                                   Request:                    Request,
                                   Status:                     GetInstalledCertificateStatus.Accepted,
                                   CertificateHashDataChain:   certs,
                                   StatusInfo:                 null,
                                   CustomData:                 null
                               );

                }


                #region Send OnGetInstalledCertificateIdsResponse event

                var responseLogger = OnGetInstalledCertificateIdsResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetInstalledCertificateIdsResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetInstalledCertificateIdsResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnDeleteCertificate

            ChargingStationServer.OnDeleteCertificate += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnDeleteCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnDeleteCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnDeleteCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnDeleteCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CertificateHashData

                DeleteCertificateResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid DeleteCertificate request for charge box '{Request.ChargeBoxId}'!"));
                    response = new DeleteCertificateResponse(
                                   Request:      Request,
                                   Status:       DeleteCertificateStatus.Failed,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );
                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming DeleteCertificate request accepted."));

                    var certKV  = certificates.FirstOrDefault(certificateKV => Request.CertificateHashData.SerialNumber == certificateKV.Value.Parsed?.SerialNumber);

                    var success = certificates.TryRemove(certKV);

                    response = new DeleteCertificateResponse(
                                   Request:      Request,
                                   Status:       success
                                                     ? DeleteCertificateStatus.Accepted
                                                     : DeleteCertificateStatus.NotFound,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnDeleteCertificateResponse event

                var responseLogger = OnDeleteCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnDeleteCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnDeleteCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyCRL

            ChargingStationServer.OnNotifyCRL += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnNotifyCRLRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyCRLRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyCRLRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyCRLRequest),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyCRLRequestId
                // Availability
                // Location

                NotifyCRLResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Invalid NotifyCRL request for charge box '{Request.ChargeBoxId}'!"));

                    //Note: No proper error response is defined within OCPP!
                    response = new NotifyCRLResponse(
                                   Request:      Request,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log(String.Concat($"ChargeBox[{ChargeBoxId}] Incoming NotifyCRL request accepted."));

                    response = new NotifyCRLResponse(
                                   Request:      Request,
                                   CustomData:   null
                               );

                }


                #region Send OnNotifyCRLResponse event

                var responseLogger = OnNotifyCRLResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyCRLResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyCRLResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnGetLocalListVersion

            ChargingStationServer.OnGetLocalListVersion += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnGetLocalListVersionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetLocalListVersionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetLocalListVersionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLocalListVersionRequest),
                                  e
                              );
                    }

                }

                #endregion

                // none

                GetLocalListVersionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetLocalListVersion request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetLocalListVersionResponse(
                                   Request,
                                   0
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetLocalListVersion request.");

                    response = new GetLocalListVersionResponse(
                                   Request,
                                   0
                               );

                }


                #region Send OnGetLocalListVersionResponse event

                var responseLogger = OnGetLocalListVersionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetLocalListVersionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetLocalListVersionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSendLocalList

            ChargingStationServer.OnSendLocalList += async (LogTimestamp,
                                                            Sender,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnSendLocalListRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSendLocalListRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSendLocalListRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSendLocalListRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VersionNumber
                // UpdateType
                // LocalAuthorizationList

                SendLocalListResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SendLocalList request for charge box '{Request.ChargeBoxId}'!");

                    response = new SendLocalListResponse(
                                   Request,
                                   SendLocalListStatus.NotSupported
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.VersionNumber + "'.");

                    response = new SendLocalListResponse(
                                   Request,
                                   SendLocalListStatus.Accepted
                               );

                }


                #region Send OnSendLocalListResponse event

                var responseLogger = OnSendLocalListResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSendLocalListResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSendLocalListResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearCache

            ChargingStationServer.OnClearCache += async (LogTimestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) => {

                #region Send OnClearCacheRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearCacheRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearCacheRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearCacheRequest),
                                  e
                              );
                    }

                }

                #endregion

                // none

                ClearCacheResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearCache request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearCacheResponse(
                                   Request,
                                   ClearCacheStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearCache request.");

                    response = new ClearCacheResponse(
                                   Request,
                                   ClearCacheStatus.Accepted
                               );

                }


                #region Send OnClearCacheResponse event

                var responseLogger = OnClearCacheResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearCacheResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearCacheResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnReserveNow

            ChargingStationServer.OnReserveNow += async (LogTimestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) => {

                #region Send OnReserveNowRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReserveNowRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReserveNowRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnReserveNowRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ReservationId
                // ExpiryDate
                // IdToken
                // ConnectorType
                // EVSEId
                // GroupIdToken

                ReserveNowResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ReserveNow request for charge box '{Request.ChargeBoxId}'!");

                    response = new ReserveNowResponse(
                                   Request:      Request,
                                   Status:       ReservationStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    var success = reservations.TryAdd(Request.Id,
                                                      Request.Id);

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ReserveNow request " + (success
                                                                                               ? "accepted"
                                                                                               : "rejected") + ".");

                    response = new ReserveNowResponse(
                                   Request:      Request,
                                   Status:       success
                                                     ? ReservationStatus.Accepted
                                                     : ReservationStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnReserveNowResponse event

                var responseLogger = OnReserveNowResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReserveNowResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnReserveNowResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCancelReservation

            ChargingStationServer.OnCancelReservation += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnCancelReservationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCancelReservationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCancelReservationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCancelReservationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ReservationId

                CancelReservationResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CancelReservation request for charge box '{Request.ChargeBoxId}'!");

                    response = new CancelReservationResponse(
                                   Request:      Request,
                                   Status:       CancelReservationStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    var success = reservations.TryRemove(Request.ReservationId, out _);

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming CancelReservation request " + (success
                                                                                                           ? "accepted"
                                                                                                           : "rejected") + ".");

                    response = new CancelReservationResponse(
                                   Request:      Request,
                                   Status:       success
                                                     ? CancelReservationStatus.Accepted
                                                     : CancelReservationStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnCancelReservationResponse event

                var responseLogger = OnCancelReservationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCancelReservationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCancelReservationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStartTransaction

            ChargingStationServer.OnRequestStartTransaction += async (LogTimestamp,
                                                                      Sender,
                                                                      Request,
                                                                      CancellationToken) => {

                #region Send OnRequestStartTransactionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnRequestStartTransactionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnRequestStartTransactionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStartTransactionRequest),
                                  e
                              );
                    }

                }

                #endregion


                RequestStartTransactionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid RequestStartTransaction request for charge box '{Request.ChargeBoxId}'!");

                    response = new RequestStartTransactionResponse(
                                   Request,
                                   RequestStartStopStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming RequestStartTransaction for '" + (Request.EVSEId?.ToString() ?? "-") + "'.");

                    // ToDo: lock(evses)

                    if (Request.EVSEId.HasValue &&
                        evses.TryGetValue(Request.EVSEId.Value, out var evse) &&
                        !evse.IsCharging)
                    {

                        evse.IsCharging              = true;
                        evse.TransactionId           = Transaction_Id.NewRandom;
                        evse.RemoteStartId           = Request.RequestStartTransactionRequestId;

                        evse.StartTimestamp          = Timestamp.Now;
                        evse.MeterStartValue         = 0;
                        evse.SignedStartMeterValue   = "0";

                        evse.StopTimestamp           = null;
                        evse.MeterStopValue          = null;
                        evse.SignedStopMeterValue    = null;

                        evse.IdToken                 = Request.IdToken;
                        evse.GroupIdToken            = Request.GroupIdToken;
                        evse.ChargingProfile         = Request.ChargingProfile;

                        _ = Task.Run(async () => {

                            await Task.Delay(500);

                            await SendTransactionEvent(

                                      EventType:            TransactionEvents.Started,
                                      Timestamp:            evse.StartTimestamp.Value,
                                      TriggerReason:        TriggerReasons.RemoteStart,
                                      SequenceNumber:       1,
                                      TransactionInfo:      new Transaction(
                                                                TransactionId:       evse.TransactionId.Value,
                                                                ChargingState:       ChargingStates.Charging,
                                                                TimeSpentCharging:   TimeSpan.Zero,
                                                                StoppedReason:       null,
                                                                RemoteStartId:       Request.RequestStartTransactionRequestId,
                                                                CustomData:          null
                                                            ),

                                      Offline:              false,
                                      NumberOfPhasesUsed:   3,
                                      CableMaxCurrent:      Ampere.Parse(32),
                                      ReservationId:        evse.ReservationId,
                                      IdToken:              evse.IdToken,
                                      EVSE:                 new EVSE(
                                                                Id:            evse.Id,
                                                                ConnectorId:   evse.Connectors.First().Id,
                                                                CustomData:    null
                                                            ),
                                      MeterValues:          new[] {
                                                                new MeterValue(
                                                                    Timestamp:       evse.StartTimestamp.Value,
                                                                    SampledValues:   new[] {
                                                                                         new SampledValue(
                                                                                             Value:              evse.MeterStartValue.Value,
                                                                                             Context:            ReadingContexts.TransactionBegin,
                                                                                             Measurand:          Measurands.Current_Export,
                                                                                             Phase:              null,
                                                                                             Location:           MeasurementLocations.Outlet,
                                                                                             SignedMeterValue:   new SignedMeterValue(
                                                                                                                     SignedMeterData:   evse.SignedStartMeterValue,
                                                                                                                     SigningMethod:     "secp256r1",
                                                                                                                     EncodingMethod:    "base64",
                                                                                                                     PublicKey:         "04cafebabe",
                                                                                                                     CustomData:        null
                                                                                                                 ),
                                                                                             UnitOfMeasure:      null,
                                                                                             CustomData:         null
                                                                                         )
                                                                                     }
                                                                )
                                                            },
                                      CustomData:           null

                                  );

                        },
                        CancellationToken.None);

                        response = new RequestStartTransactionResponse(
                                       Request:         Request,
                                       Status:          RequestStartStopStatus.Accepted,
                                       TransactionId:   evse.TransactionId,
                                       StatusInfo:      null,
                                       CustomData:      null
                                   );

                    }
                    else
                        response = new RequestStartTransactionResponse(
                                       Request,
                                       RequestStartStopStatus.Rejected
                                   );

                }


                #region Send OnRequestStartTransactionResponse event

                var responseLogger = OnRequestStartTransactionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnRequestStartTransactionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStartTransactionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStopTransaction

            ChargingStationServer.OnRequestStopTransaction += async (LogTimestamp,
                                                                     Sender,
                                                                     Request,
                                                                     CancellationToken) => {

                #region Send OnRequestStopTransactionRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnRequestStopTransactionRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnRequestStopTransactionRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStopTransactionRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TransactionId

                RequestStopTransactionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid RequestStopTransaction request for charge box '{Request.ChargeBoxId}'!");

                    response = new RequestStopTransactionResponse(
                                   Request,
                                   RequestStartStopStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming RequestStopTransaction for '" + Request.TransactionId + "'.");

                    // ToDo: lock(evses)
                    var evse = evses.Values.FirstOrDefault(evse => Request.TransactionId == evse.TransactionId);

                    if (evse is not null)
                    {

                        evse.IsCharging             = false;

                        evse.StopTimestamp          = Timestamp.Now;
                        evse.MeterStopValue         = 123;
                        evse.SignedStopMeterValue   = "123";

                        _ = Task.Run(async () => {

                            await SendTransactionEvent(

                                      EventType:            TransactionEvents.Ended,
                                      Timestamp:            evse.StopTimestamp.Value,
                                      TriggerReason:        TriggerReasons.RemoteStop,
                                      SequenceNumber:       2,
                                      TransactionInfo:      new Transaction(
                                                                TransactionId:       evse.TransactionId!.Value,
                                                                ChargingState:       ChargingStates.Idle,
                                                                TimeSpentCharging:   evse.StopTimestamp - evse.StartTimestamp,
                                                                StoppedReason:       StopTransactionReasons.Remote,
                                                                RemoteStartId:       evse.RemoteStartId,
                                                                CustomData:          null
                                                            ),

                                      Offline:              false,
                                      NumberOfPhasesUsed:   3,
                                      CableMaxCurrent:      Ampere.Parse(32),
                                      ReservationId:        evse.ReservationId,
                                      IdToken:              evse.IdToken,
                                      EVSE:                 new EVSE(
                                                                Id:            evse.Id,
                                                                ConnectorId:   evse.Connectors.First().Id,
                                                                CustomData:    null
                                                            ),
                                      MeterValues:          new[] {
                                                                new MeterValue(
                                                                    Timestamp:       evse.StopTimestamp.Value,
                                                                    SampledValues:   new[] {
                                                                                         new SampledValue(
                                                                                             Value:              evse.MeterStopValue.Value,
                                                                                             Context:            ReadingContexts.TransactionEnd,
                                                                                             Measurand:          Measurands.Current_Export,
                                                                                             Phase:              null,
                                                                                             Location:           MeasurementLocations.Outlet,
                                                                                             SignedMeterValue:   new SignedMeterValue(
                                                                                                                     SignedMeterData:   evse.SignedStopMeterValue,
                                                                                                                     SigningMethod:     "secp256r1",
                                                                                                                     EncodingMethod:    "base64",
                                                                                                                     PublicKey:         "04cafebabe",
                                                                                                                     CustomData:        null
                                                                                                                 ),
                                                                                             UnitOfMeasure:      null,
                                                                                             CustomData:         null
                                                                                         )
                                                                                     }
                                                                )
                                                            },
                                      CustomData:           null

                                  );

                        },
                        CancellationToken.None);

                        response = new RequestStopTransactionResponse(
                                       Request,
                                       RequestStartStopStatus.Accepted
                                   );

                    }
                    else
                        response = new RequestStopTransactionResponse(
                                       Request,
                                       RequestStartStopStatus.Rejected
                                   );

                }


                #region Send OnRequestStopTransactionResponse event

                var responseLogger = OnRequestStopTransactionResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnRequestStopTransactionResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnRequestStopTransactionResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetTransactionStatus

            ChargingStationServer.OnGetTransactionStatus += async (LogTimestamp,
                                                                   Sender,
                                                                   Request,
                                                                   CancellationToken) => {

                #region Send OnGetTransactionStatusRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetTransactionStatusRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetTransactionStatusRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetTransactionStatusRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TransactionId

                GetTransactionStatusResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetTransactionStatus request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetTransactionStatusResponse(
                                   Request,
                                   MessagesInQueue:    false,
                                   OngoingIndicator:   true
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetTransactionStatus for '" + Request.TransactionId + "'.");

                    if (Request.TransactionId.HasValue)
                    {

                        var foundEVSE =  evses.Values.FirstOrDefault(evse => Request.TransactionId == evse.TransactionId);

                        if (foundEVSE is not null)
                        {

                            response = new GetTransactionStatusResponse(
                                           Request,
                                           MessagesInQueue:    false,
                                           OngoingIndicator:   true
                                       );

                        }
                        else
                        {

                            response = new GetTransactionStatusResponse(
                                           Request,
                                           MessagesInQueue:    false,
                                           OngoingIndicator:   true
                                       );

                        }

                    }
                    else
                    {

                        response = new GetTransactionStatusResponse(
                                       Request,
                                       MessagesInQueue:    false,
                                       OngoingIndicator:   true
                                   );

                    }

                }


                #region Send OnGetTransactionStatusResponse event

                var responseLogger = OnGetTransactionStatusResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetTransactionStatusResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetTransactionStatusResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetChargingProfile

            ChargingStationServer.OnSetChargingProfile += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnSetChargingProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetChargingProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetChargingProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetChargingProfileRequest),
                                  e
                              );
                    }

                }

                #endregion

                // EVSEId
                // ChargingProfile

                SetChargingProfileResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetChargingProfile request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetChargingProfileResponse(
                                   Request,
                                   ChargingProfileStatus.Rejected
                               );

                }
                else if (Request.ChargingProfile is null)
                {

                    response = new SetChargingProfileResponse(
                                   Request,
                                   ChargingProfileStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetChargingProfile for '" + Request.EVSEId + "'.");

                    // ToDo: lock(connectors)

                    if (Request.EVSEId.Value == 0)
                    {

                        foreach (var evse in evses.Values)
                        {

                            if (!Request.ChargingProfile.TransactionId.HasValue)
                                evse.ChargingProfile = Request.ChargingProfile;

                            else if (evse.TransactionId == Request.ChargingProfile.TransactionId.Value)
                                evse.ChargingProfile = Request.ChargingProfile;

                        }

                        response = new SetChargingProfileResponse(
                                       Request,
                                       ChargingProfileStatus.Accepted
                                   );

                    }
                    else if (evses.ContainsKey(Request.EVSEId))
                    {

                        evses[Request.EVSEId].ChargingProfile = Request.ChargingProfile;

                        response = new SetChargingProfileResponse(
                                       Request,
                                       ChargingProfileStatus.Accepted
                                   );

                    }
                    else
                        response = new SetChargingProfileResponse(
                                       Request,
                                       ChargingProfileStatus.Rejected
                                   );

                }


                #region Send OnSetChargingProfileResponse event

                var responseLogger = OnSetChargingProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetChargingProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetChargingProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetChargingProfiles

            ChargingStationServer.OnGetChargingProfiles += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnGetChargingProfilesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetChargingProfilesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetChargingProfilesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetChargingProfilesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GetChargingProfilesRequestId
                // ChargingProfile
                // EVSEId

                GetChargingProfilesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetChargingProfiles request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetChargingProfilesResponse(
                                   Request,
                                   GetChargingProfileStatus.Unknown
                               );

                }
                else if (Request.EVSEId.HasValue && evses.ContainsKey(Request.EVSEId.Value))
                {

                    //evses[Request.EVSEId.Value].ChargingProfile = Request.ChargingProfile;

                    response = new GetChargingProfilesResponse(
                                   Request,
                                   GetChargingProfileStatus.Accepted
                               );

                }
                else
                   response = new GetChargingProfilesResponse(
                                  Request,
                                  GetChargingProfileStatus.Unknown
                              );


                #region Send OnGetChargingProfilesResponse event

                var responseLogger = OnGetChargingProfilesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetChargingProfilesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetChargingProfilesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearChargingProfile

            ChargingStationServer.OnClearChargingProfile += async (LogTimestamp,
                                                                   Sender,
                                                                   Request,
                                                                   CancellationToken) => {

                #region Send OnClearChargingProfileRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearChargingProfileRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearChargingProfileRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearChargingProfileRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingProfileId
                // ChargingProfileCriteria

                var response = new ClearChargingProfileResponse(
                                   Request:      Request,
                                   Status:       ClearChargingProfileStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );


                #region Send OnClearChargingProfileResponse event

                var responseLogger = OnClearChargingProfileResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearChargingProfileResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearChargingProfileResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCompositeSchedule

            ChargingStationServer.OnGetCompositeSchedule += async (LogTimestamp,
                                                                   Sender,
                                                                   Request,
                                                                   CancellationToken) => {

                #region Send OnGetCompositeScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCompositeScheduleRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCompositeScheduleRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetCompositeScheduleRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Duration,
                // EVSEId,
                // ChargingRateUnit

                var response = new GetCompositeScheduleResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Accepted,
                                   Schedule:     null,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                #region Send OnGetCompositeScheduleResponse event

                var responseLogger = OnGetCompositeScheduleResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCompositeScheduleResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetCompositeScheduleResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateDynamicSchedule

            ChargingStationServer.OnUpdateDynamicSchedule += async (LogTimestamp,
                                                                    Sender,
                                                                    Request,
                                                                    CancellationToken) => {

                #region Send OnUpdateDynamicScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUpdateDynamicScheduleRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUpdateDynamicScheduleRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateDynamicScheduleRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingProfileId

                // Limit
                // Limit_L2
                // Limit_L3

                // DischargeLimit
                // DischargeLimit_L2
                // DischargeLimit_L3

                // Setpoint
                // Setpoint_L2
                // Setpoint_L3

                // SetpointReactive
                // SetpointReactive_L2
                // SetpointReactive_L3

                var response = new UpdateDynamicScheduleResponse(
                                   Request:      Request,
                                   Status:       ChargingProfileStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );


                #region Send OnUpdateDynamicScheduleResponse event

                var responseLogger = OnUpdateDynamicScheduleResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUpdateDynamicScheduleResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUpdateDynamicScheduleResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            ChargingStationServer.OnNotifyAllowedEnergyTransfer += async (LogTimestamp,
                                                                          Sender,
                                                                          Request,
                                                                          CancellationToken) => {

                #region Send OnNotifyAllowedEnergyTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyAllowedEnergyTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyAllowedEnergyTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyAllowedEnergyTransferRequest),
                                  e
                              );
                    }

                }

                #endregion

                // AllowedEnergyTransferModes

                NotifyAllowedEnergyTransferResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid NotifyAllowedEnergyTransfer request for charge box '{Request.ChargeBoxId}'!");

                    response = new NotifyAllowedEnergyTransferResponse(
                                   Request:      Request,
                                   Status:       NotifyAllowedEnergyTransferStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming NotifyAllowedEnergyTransfer");

                    response = new NotifyAllowedEnergyTransferResponse(
                                   Request:      Request,
                                   Status:       NotifyAllowedEnergyTransferStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnNotifyAllowedEnergyTransferResponse event

                var responseLogger = OnNotifyAllowedEnergyTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyAllowedEnergyTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnNotifyAllowedEnergyTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUsePriorityCharging

            ChargingStationServer.OnUsePriorityCharging += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnUsePriorityChargingRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUsePriorityChargingRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUsePriorityChargingRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUsePriorityChargingRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TransactionId
                // Activate

                var response = new UsePriorityChargingResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );


                #region Send OnUsePriorityChargingResponse event

                var responseLogger = OnUsePriorityChargingResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUsePriorityChargingResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUsePriorityChargingResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnlockConnector

            ChargingStationServer.OnUnlockConnector += async (LogTimestamp,
                                                              Sender,
                                                              Request,
                                                              CancellationToken) => {

                #region Send OnUnlockConnectorRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnUnlockConnectorRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnUnlockConnectorRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnlockConnectorRequest),
                                  e
                              );
                    }

                }

                #endregion

                // EVSEId
                // ConnectorId

                UnlockConnectorResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UnlockConnector request for charge box '{Request.ChargeBoxId}'!");

                    response = new UnlockConnectorResponse(
                                   Request:      Request,
                                   Status:       UnlockStatus.UnlockFailed,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

                    // ToDo: lock(connectors)

                    if (evses.TryGetValue    (Request.EVSEId,      out var evse) &&
                        evse. TryGetConnector(Request.ConnectorId, out var connector))
                    {

                        // What to do here?!

                        response = new UnlockConnectorResponse(
                                       Request:      Request,
                                       Status:       UnlockStatus.Unlocked,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }
                    else
                        response = new UnlockConnectorResponse(
                                       Request:      Request,
                                       Status:       UnlockStatus.UnlockFailed,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                }


                #region Send OnUnlockConnectorResponse event

                var responseLogger = OnUnlockConnectorResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnUnlockConnectorResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnUnlockConnectorResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnAFRRSignal

            ChargingStationServer.OnAFRRSignal += async (LogTimestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) => {

                #region Send OnAFRRSignalRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnAFRRSignalRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnAFRRSignalRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnAFRRSignalRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ActivationTimestamp
                // Signal

                AFRRSignalResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid AFRRSignal request for charge box '{Request.ChargeBoxId}'!");

                    response = new AFRRSignalResponse(
                                   Request:      Request,
                                   Status:       GenericStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming AFRRSignal '{Request.Signal}' @ '{Request.ActivationTimestamp}'.");

                    response = new AFRRSignalResponse(
                                   Request:      Request,
                                   Status:       Request.ActivationTimestamp < Timestamp.Now - TimeSpan.FromDays(1)
                                                     ? GenericStatus.Rejected
                                                     : GenericStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }


                #region Send OnAFRRSignalResponse event

                var responseLogger = OnAFRRSignalResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnAFRRSignalResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnAFRRSignalResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnSetDisplayMessage

            ChargingStationServer.OnSetDisplayMessage += async (LogTimestamp,
                                                                Sender,
                                                                Request,
                                                                CancellationToken) => {

                #region Send OnSetDisplayMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSetDisplayMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSetDisplayMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Message

                SetDisplayMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetDisplayMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetDisplayMessageResponse(
                                   Request:   Request,
                                   Result:    Result.GenericError("")
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetDisplayMessage request.");

                    if (displayMessages.TryAdd(Request.Message.Id,
                                               Request.Message)) {

                        response = new SetDisplayMessageResponse(
                                       Request:      Request,
                                       Status:       DisplayMessageStatus.Accepted,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                    }

                    else
                        response = new SetDisplayMessageResponse(
                                       Request:      Request,
                                       Status:       DisplayMessageStatus.Rejected,
                                       StatusInfo:   null,
                                       CustomData:   null
                                   );

                }


                #region Send OnSetDisplayMessageResponse event

                var responseLogger = OnSetDisplayMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSetDisplayMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnSetDisplayMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetDisplayMessages

            ChargingStationServer.OnGetDisplayMessages += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnGetDisplayMessagesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetDisplayMessagesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetDisplayMessagesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetDisplayMessagesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GetDisplayMessagesRequestId
                // Ids
                // Priority
                // State

                GetDisplayMessagesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetDisplayMessages request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetDisplayMessagesResponse(
                                   Request,
                                   Result.GenericError("")
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetDisplayMessage request.");

                    _ = Task.Run(async () => {

                        var filteredDisplayMessages = displayMessages.Values.
                                                          Where(displayMessage => Request.Ids is null || !Request.Ids.Any() || Request.Ids.Contains(displayMessage.Id)).
                                                          Where(displayMessage => !Request.State.   HasValue || (displayMessage.State.HasValue && displayMessage.State.Value == Request.State.Value)).
                                                          Where(displayMessage => !Request.Priority.HasValue || displayMessage.Priority == Request.Priority.Value).
                                                          ToArray();

                        await NotifyDisplayMessages(
                                  NotifyDisplayMessagesRequestId:   Request.GetDisplayMessagesRequestId,
                                  MessageInfos:                     filteredDisplayMessages,
                                  ToBeContinued:                    false,
                                  CustomData:                       null
                              );

                    },
                    CancellationToken.None);

                    response = new GetDisplayMessagesResponse(
                                   Request,
                                   GetDisplayMessagesStatus.Accepted
                               );

                }


                #region Send OnGetDisplayMessagesResponse event

                var responseLogger = OnGetDisplayMessagesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetDisplayMessagesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnGetDisplayMessagesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearDisplayMessage

            ChargingStationServer.OnClearDisplayMessage += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnClearDisplayMessageRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearDisplayMessageRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearDisplayMessageRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion

                // DisplayMessageId

                ClearDisplayMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearDisplayMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearDisplayMessageResponse(
                                   Request,
                                   Result.GenericError("")
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearDisplayMessage request.");

                    if (displayMessages.TryGetValue(Request.DisplayMessageId, out var messageInfo) &&
                        displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(Request.DisplayMessageId, messageInfo))) {

                        response = new ClearDisplayMessageResponse(
                                       Request,
                                       ClearMessageStatus.Accepted
                                   );

                    }

                    else
                        response = new ClearDisplayMessageResponse(
                                       Request,
                                       ClearMessageStatus.Unknown
                                   );

                }


                #region Send OnClearDisplayMessageResponse event

                var responseLogger = OnClearDisplayMessageResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearDisplayMessageResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnClearDisplayMessageResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCostUpdated

            ChargingStationServer.OnCostUpdated += async (LogTimestamp,
                                                          Sender,
                                                          Request,
                                                          CancellationToken) => {

                #region Send OnCostUpdatedRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCostUpdatedRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCostUpdatedRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCostUpdatedRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TotalCost
                // TransactionId

                CostUpdatedResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CostUpdated request for charge box '{Request.ChargeBoxId}'!");

                    response = new CostUpdatedResponse(
                                   Request,
                                   Result.GenericError("")
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming CostUpdated request.");


                    if (transactions.ContainsKey(Request.TransactionId)) {

                        totalCosts.AddOrUpdate(Request.TransactionId,
                                               Request.TotalCost,
                                               (transactionId, totalCost) => Request.TotalCost);

                        response = new CostUpdatedResponse(
                                       Request
                                   );

                    }

                    else
                        response = new CostUpdatedResponse(
                                       Request,
                                       Result.GenericError($"Unknown transaction identification '{Request.TransactionId}'!")
                                   );

                }


                #region Send OnCostUpdatedResponse event

                var responseLogger = OnCostUpdatedResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCostUpdatedResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCostUpdatedResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnCustomerInformation

            ChargingStationServer.OnCustomerInformation += async (LogTimestamp,
                                                                  Sender,
                                                                  Request,
                                                                  CancellationToken) => {

                #region Send OnCustomerInformationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnCustomerInformationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnCustomerInformationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCustomerInformationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CustomerInformationRequestId
                // Report,
                // Clear,
                // CustomerIdentifier
                // IdToken
                // CustomerCertificate

                CustomerInformationResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CustomerInformation request for charge box '{Request.ChargeBoxId}'!");

                    response = new CustomerInformationResponse(
                                   Request,
                                   Result.GenericError("")
                               );

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming CustomerInformation request.");


                    _ = Task.Run(async () => {

                        //var filteredDisplayMessages = displayMessages.Values.
                        //                                  Where(displayMessage => Request.Ids is null || !Request.Ids.Any() || Request.Ids.Contains(displayMessage.Id)).
                        //                                  Where(displayMessage => !Request.State.   HasValue || (displayMessage.State.HasValue && displayMessage.State.Value == Request.State.Value)).
                        //                                  Where(displayMessage => !Request.Priority.HasValue || displayMessage.Priority == Request.Priority.Value).
                        //                                  ToArray();

                        await NotifyCustomerInformation(
                                  NotifyCustomerInformationRequestId:   Request.CustomerInformationRequestId,
                                  Data:                                 "",
                                  SequenceNumber:                       1,
                                  GeneratedAt:                          Timestamp.Now,
                                  ToBeContinued:                        false,
                                  CustomData:                           null
                              );

                    },
                    CancellationToken.None);

                    response = new CustomerInformationResponse(
                                   Request,
                                   CustomerInformationStatus.Accepted
                               );

                }


                #region Send OnCustomerInformationResponse event

                var responseLogger = OnCustomerInformationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnCustomerInformationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                Request,
                                                                                                                response,
                                                                                                                responseTime - startTime)).
                                                              ToArray();

                    try
                    {
                        await Task.WhenAll(responseLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestChargingStation),
                                  nameof(OnCustomerInformationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

        }

        #endregion


        #region (Timer) DoMaintenance(State)

        private void DoMaintenanceSync(Object? State)
        {
            if (!DisableMaintenanceTasks)
                DoMaintenance(State).Wait();
        }

        protected internal virtual async Task _DoMaintenance(Object State)
        {

            foreach (var enquedRequest in EnqueuedRequests.ToArray())
            {
                if (CSClient is ChargingStationWSClient wsClient)
                {

                    var response = await wsClient.SendRequest(
                                             enquedRequest.Command,
                                             enquedRequest.Request.RequestId,
                                             enquedRequest.RequestJSON
                                         );

                    enquedRequest.ResponseAction(response);

                    EnqueuedRequests.Remove(enquedRequest);

                }
            }

        }

        private async Task DoMaintenance(Object State)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    await _DoMaintenance(State);

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }
            else
                DebugX.LogT("Could not aquire the maintenance tasks lock!");

        }

        #endregion

        #region (Timer) DoSendHeartbeatSync(State)

        private void DoSendHeartbeatSync(Object? State)
        {
            if (!DisableSendHeartbeats)
            {
                try
                {
                    SendHeartbeat().Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(DoSendHeartbeatSync));
                }
            }
        }

        #endregion


        #region (private) NextRequestId

        private Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion


        #region SendBootNotification                 (BootReason, ...)

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
        public async Task<CSMS.BootNotificationResponse>

            SendBootNotification(BootReason         BootReason,
                                 CustomData?        CustomData          = null,

                                 Request_Id?        RequestId           = null,
                                 DateTime?          RequestTimestamp    = null,
                                 TimeSpan?          RequestTimeout      = null,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new BootNotificationRequest(
                                 ChargeBoxId,
                                 new ChargingStation(
                                     Model,
                                     VendorName,
                                     SerialNumber,
                                     Modem,
                                     FirmwareVersion,
                                     new CustomData(
                                         Vendor_Id.Parse("GraphDefined"),
                                         new JObject()
                                     )
                                 ),
                                 BootReason,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(startTime,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            CSMS.BootNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendBootNotification(request);

            if (response is not null)
            {
                switch (response.Status)
                {

                    case RegistrationStatus.Accepted:
                        this.CSMSTime               = response.CurrentTime;
                        this.SendHeartbeatEvery     = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                        this.SendHeartbeatTimer.Change(this.SendHeartbeatEvery, this.SendHeartbeatEvery);
                        this.DisableSendHeartbeats  = false;
                        break;

                    case RegistrationStatus.Pending:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                    case RegistrationStatus.Rejected:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                }
            }

            response ??= new CSMS.BootNotificationResponse(request,
                                                           Result.Server("Response is null!"));


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBootNotificationResponse?.Invoke(endTime,
                                                   this,
                                                   request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatus     Status,
                                           Int64?             UpdateFirmwareRequestId   = null,
                                           CustomData?        CustomData                = null,

                                           Request_Id?        RequestId                 = null,
                                           DateTime?          RequestTimestamp          = null,
                                           TimeSpan?          RequestTimeout            = null,
                                           EventTracking_Id?  EventTrackingId           = null,
                                           CancellationToken  CancellationToken         = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new FirmwareStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,
                                 UpdateFirmwareRequestId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                            this,
                                                            request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            CSMS.FirmwareStatusNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendFirmwareStatusNotification(request);

            response ??= new CSMS.FirmwareStatusNotificationResponse(request,
                                                                     Result.Server("Response is null!"));


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                             this,
                                                             request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(PublishFirmwareStatus  Status,
                                                  Int32?                 PublishFirmwareStatusNotificationRequestId,
                                                  IEnumerable<URL>?      DownloadLocations,
                                                  CustomData?            CustomData          = null,

                                                  Request_Id?            RequestId           = null,
                                                  DateTime?              RequestTimestamp    = null,
                                                  TimeSpan?              RequestTimeout      = null,
                                                  EventTracking_Id?      EventTrackingId     = null,
                                                  CancellationToken      CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new PublishFirmwareStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,
                                 PublishFirmwareStatusNotificationRequestId,
                                 DownloadLocations,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnPublishFirmwareStatusNotificationRequest event

            try
            {

                OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                   this,
                                                                   request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            CSMS.PublishFirmwareStatusNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendPublishFirmwareStatusNotification(request);

            response ??= new CSMS.PublishFirmwareStatusNotificationResponse(request,
                                                                            Result.Server("Response is null!"));


            #region Send OnPublishFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.HeartbeatResponse>

            SendHeartbeat(CustomData?        CustomData          = null,

                          Request_Id?        RequestId           = null,
                          DateTime?          RequestTimestamp    = null,
                          TimeSpan?          RequestTimeout      = null,
                          EventTracking_Id?  EventTrackingId     = null,
                          CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new HeartbeatRequest(
                                 ChargeBoxId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(startTime,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            CSMS.HeartbeatResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendHeartbeat(request);

            if (response is not null)
            {
                this.CSMSTime = response.CurrentTime;
            }

            response ??= new CSMS.HeartbeatResponse(request,
                                                    Result.Server("Response is null!"));


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            this,
                                            request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyEventResponse>

            NotifyEvent(DateTime                GeneratedAt,
                        UInt32                  SequenceNumber,
                        IEnumerable<EventData>  EventData,
                        Boolean?                ToBeContinued       = null,
                        CustomData?             CustomData          = null,

                        Request_Id?             RequestId           = null,
                        DateTime?               RequestTimestamp    = null,
                        TimeSpan?               RequestTimeout      = null,
                        EventTracking_Id?       EventTrackingId     = null,
                        CancellationToken       CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyEventRequest(
                                 ChargeBoxId,
                                 GeneratedAt,
                                 SequenceNumber,
                                 EventData,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyEventRequest event

            try
            {

                OnNotifyEventRequest?.Invoke(startTime,
                                             this,
                                             request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            CSMS.NotifyEventResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyEvent(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyEventResponse(request,
                                                      Result.Server("Response is null!"));


            #region Send OnNotifyEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEventResponse?.Invoke(endTime,
                                              this,
                                              request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.SecurityEventNotificationResponse>

            SendSecurityEventNotification(SecurityEventType      Type,
                                          DateTime           Timestamp,
                                          String?            TechInfo            = null,
                                          CustomData?        CustomData          = null,

                                          Request_Id?        RequestId           = null,
                                          DateTime?          RequestTimestamp    = null,
                                          TimeSpan?          RequestTimeout      = null,
                                          EventTracking_Id?  EventTrackingId     = null,
                                          CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            var request    = new SecurityEventNotificationRequest(
                                 ChargeBoxId,
                                 Type,
                                 Timestamp,
                                 TechInfo,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnSecurityEventNotificationRequest event

            try
            {

                OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                           this,
                                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSecurityEventNotificationRequest));
            }

            #endregion


            CSMS.SecurityEventNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendSecurityEventNotification(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.SecurityEventNotificationResponse(request,
                                                                    Result.Server("Response is null!"));


            #region Send OnSecurityEventNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnSecurityEventNotificationResponse?.Invoke(endTime,
                                                            this,
                                                            request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSecurityEventNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyReportResponse>

            NotifyReport(Int32                    NotifyReportRequestId,
                         UInt32                   SequenceNumber,
                         DateTime                 GeneratedAt,
                         IEnumerable<ReportData>  ReportData,
                         Boolean?                 ToBeContinued       = null,
                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyReportRequest(
                                 ChargeBoxId,
                                 NotifyReportRequestId,
                                 SequenceNumber,
                                 GeneratedAt,
                                 ReportData,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyReportRequest event

            try
            {

                OnNotifyReportRequest?.Invoke(startTime,
                                              this,
                                              request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            CSMS.NotifyReportResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyReport(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyReportResponse(request,
                                                       Result.Server("Response is null!"));


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponse?.Invoke(endTime,
                                               this,
                                               request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyMonitoringReportResponse>

            NotifyMonitoringReport(Int32                        NotifyMonitoringReportRequestId,
                                   UInt32                       SequenceNumber,
                                   DateTime                     GeneratedAt,
                                   IEnumerable<MonitoringData>  MonitoringData,
                                   Boolean?                     ToBeContinued       = null,
                                   CustomData?                  CustomData          = null,

                                   Request_Id?                  RequestId           = null,
                                   DateTime?                    RequestTimestamp    = null,
                                   TimeSpan?                    RequestTimeout      = null,
                                   EventTracking_Id?            EventTrackingId     = null,
                                   CancellationToken            CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyMonitoringReportRequest(
                                 ChargeBoxId,
                                 NotifyMonitoringReportRequestId,
                                 SequenceNumber,
                                 GeneratedAt,
                                 MonitoringData,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyMonitoringReportRequest event

            try
            {

                OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                        this,
                                                        request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            CSMS.NotifyMonitoringReportResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyMonitoringReport(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyMonitoringReportResponse(request,
                                                                 Result.Server("Response is null!"));


            #region Send OnNotifyMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                         this,
                                                         request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.LogStatusNotificationResponse>

            SendLogStatusNotification(UploadLogStatus    Status,
                                      Int32?             LogRequestId        = null,
                                      CustomData?        CustomData          = null,

                                      Request_Id?        RequestId           = null,
                                      DateTime?          RequestTimestamp    = null,
                                      TimeSpan?          RequestTimeout      = null,
                                      EventTracking_Id?  EventTrackingId     = null,
                                      CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new LogStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,
                                 LogRequestId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnLogStatusNotificationRequest event

            try
            {

                OnLogStatusNotificationRequest?.Invoke(startTime,
                                                       this,
                                                       request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnLogStatusNotificationRequest));
            }

            #endregion


            CSMS.LogStatusNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendLogStatusNotification(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.LogStatusNotificationResponse(request,
                                                                    Result.Server("Response is null!"));


            #region Send OnLogStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationResponse?.Invoke(endTime,
                                                        this,
                                                        request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnLogStatusNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.DataTransferResponse>

            TransferData(Vendor_Id          VendorId,
                         String?            MessageId           = null,
                         JToken?            Data                = null,
                         CustomData?        CustomData          = null,

                         Request_Id?        RequestId           = null,
                         DateTime?          RequestTimestamp    = null,
                         TimeSpan?          RequestTimeout      = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new DataTransferRequest(
                                 ChargeBoxId,
                                 VendorId,
                                 MessageId,
                                 Data,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CSMS.DataTransferResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.TransferData(request);

            response ??= new CSMS.DataTransferResponse(request,
                                                       Result.Server("Response is null!"));


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendCertificateSigningRequest        (CSR, CertificateType = null, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.SignCertificateResponse>

            SendCertificateSigningRequest(String                  CSR,
                                          CertificateSigningUse?  CertificateType     = null,
                                          CustomData?             CustomData          = null,

                                          Request_Id?             RequestId           = null,
                                          DateTime?               RequestTimestamp    = null,
                                          TimeSpan?               RequestTimeout      = null,
                                          EventTracking_Id?       EventTrackingId     = null,
                                          CancellationToken       CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new SignCertificateRequest(
                                 ChargeBoxId,
                                 CSR,
                                 CertificateType,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnSignCertificateRequest event

            try
            {

                OnSignCertificateRequest?.Invoke(startTime,
                                                 this,
                                                 request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            CSMS.SignCertificateResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendCertificateSigningRequest(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.SignCertificateResponse(request,
                                                          Result.Server("Response is null!"));


            #region Send OnSignCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignCertificateResponse?.Invoke(endTime,
                                                  this,
                                                  request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.Get15118EVCertificateResponse>

            Get15118EVCertificate(ISO15118SchemaVersion  ISO15118SchemaVersion,
                                  CertificateAction      CertificateAction,
                                  EXIData                EXIRequest,
                                  UInt32?                MaximumContractCertificateChains   = 1,
                                  IEnumerable<EMA_Id>?   PrioritizedEMAIds                  = null,
                                  CustomData?            CustomData                         = null,

                                  Request_Id?            RequestId                          = null,
                                  DateTime?              RequestTimestamp                   = null,
                                  TimeSpan?              RequestTimeout                     = null,
                                  EventTracking_Id?      EventTrackingId                    = null,
                                  CancellationToken      CancellationToken                  = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new Get15118EVCertificateRequest(
                                 ChargeBoxId,
                                 ISO15118SchemaVersion,
                                 CertificateAction,
                                 EXIRequest,
                                 MaximumContractCertificateChains,
                                 PrioritizedEMAIds,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGet15118EVCertificateRequest event

            try
            {

                OnGet15118EVCertificateRequest?.Invoke(startTime,
                                                       this,
                                                       request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGet15118EVCertificateRequest));
            }

            #endregion


            CSMS.Get15118EVCertificateResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.Get15118EVCertificate(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.Get15118EVCertificateResponse(request,
                                                                Result.Server("Response is null!"));


            #region Send OnGet15118EVCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateResponse?.Invoke(endTime,
                                                        this,
                                                        request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGet15118EVCertificateResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.GetCertificateStatusResponse>

            GetCertificateStatus(OCSPRequestData    OCSPRequestData,
                                 CustomData?        CustomData          = null,

                                 Request_Id?        RequestId           = null,
                                 DateTime?          RequestTimestamp    = null,
                                 TimeSpan?          RequestTimeout      = null,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetCertificateStatusRequest(
                                 ChargeBoxId,
                                 OCSPRequestData,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetCertificateStatusRequest event

            try
            {

                OnGetCertificateStatusRequest?.Invoke(startTime,
                                                      this,
                                                      request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCertificateStatusRequest));
            }

            #endregion


            CSMS.GetCertificateStatusResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.GetCertificateStatus(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.GetCertificateStatusResponse(request,
                                                               Result.Server("Response is null!"));


            #region Send OnGetCertificateStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusResponse?.Invoke(endTime,
                                                       this,
                                                       request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCertificateStatusResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.GetCRLResponse>

            GetCRLRequest(UInt32               GetCRLRequestId,
                          CertificateHashData  CertificateHashData,
                          CustomData?          CustomData          = null,

                          Request_Id?          RequestId           = null,
                          DateTime?            RequestTimestamp    = null,
                          TimeSpan?            RequestTimeout      = null,
                          EventTracking_Id?    EventTrackingId     = null,
                          CancellationToken    CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new GetCRLRequest(
                                 ChargeBoxId,
                                 GetCRLRequestId,
                                 CertificateHashData,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnGetCRLRequest event

            try
            {

                OnGetCRLRequest?.Invoke(startTime,
                                        this,
                                        request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCRLRequest));
            }

            #endregion


            CSMS.GetCRLResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.GetCRL(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.GetCRLResponse(request,
                                                 Result.Server("Response is null!"));


            #region Send OnGetCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCRLResponse?.Invoke(endTime,
                                         this,
                                         request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCRLResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(Reservation_Id           ReservationId,
                                        ReservationUpdateStatus  ReservationUpdateStatus,
                                        CustomData?              CustomData          = null,

                                        Request_Id?              RequestId           = null,
                                        DateTime?                RequestTimestamp    = null,
                                        TimeSpan?                RequestTimeout      = null,
                                        EventTracking_Id?        EventTrackingId     = null,
                                        CancellationToken        CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ReservationStatusUpdateRequest(
                                 ChargeBoxId,
                                 ReservationId,
                                 ReservationUpdateStatus,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnReservationStatusUpdateRequest event

            try
            {

                OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                         this,
                                                         request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            CSMS.ReservationStatusUpdateResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendReservationStatusUpdate(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.ReservationStatusUpdateResponse(request,
                                                                  Result.Server("Response is null!"));


            #region Send OnReservationStatusUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                          this,
                                                          request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.AuthorizeResponse>

            Authorize(IdToken                        IdToken,
                      Certificate?                   Certificate                   = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,
                      CustomData?                    CustomData                    = null,

                      Request_Id?                    RequestId                     = null,
                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      CancellationToken              CancellationToken             = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new AuthorizeRequest(
                                 ChargeBoxId,
                                 IdToken,
                                 Certificate,
                                 ISO15118CertificateHashData,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(startTime,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            CSMS.AuthorizeResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.Authorize(request);

            response ??= new CSMS.AuthorizeResponse(request,
                                                    Result.Server("Response is null!"));


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            this,
                                            request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(EVSE_Id            EVSEId,
                                  ChargingNeeds      ChargingNeeds,
                                  DateTime?          ReceivedTimestamp   = null,
                                  UInt16?            MaxScheduleTuples   = null,
                                  CustomData?        CustomData          = null,

                                  Request_Id?        RequestId           = null,
                                  DateTime?          RequestTimestamp    = null,
                                  TimeSpan?          RequestTimeout      = null,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyEVChargingNeedsRequest(
                                 ChargeBoxId,
                                 EVSEId,
                                 ChargingNeeds,
                                 ReceivedTimestamp,
                                 MaxScheduleTuples,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyEVChargingNeedsRequest event

            try
            {

                OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                       this,
                                                       request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            CSMS.NotifyEVChargingNeedsResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyEVChargingNeeds(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyEVChargingNeedsResponse(request,
                                                                Result.Server("Response is null!"));


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                        this,
                                                        request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.TransactionEventResponse>

            SendTransactionEvent(TransactionEvents         EventType,
                                 DateTime                  Timestamp,
                                 TriggerReasons            TriggerReason,
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
                                 CustomData?               CustomData              = null,

                                 Request_Id?               RequestId               = null,
                                 DateTime?                 RequestTimestamp        = null,
                                 TimeSpan?                 RequestTimeout          = null,
                                 EventTracking_Id?         EventTrackingId         = null,
                                 CancellationToken         CancellationToken       = default)

        {

            #region Create request

            var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            var request    = new TransactionEventRequest(
                                 ChargeBoxId,

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
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnTransactionEventRequest event

            try
            {

                OnTransactionEventRequest?.Invoke(startTime,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            CSMS.TransactionEventResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendTransactionEvent(request);

            response ??= new CSMS.TransactionEventResponse(request,
                                                           Result.Server("Response is null!"));


            #region Send OnTransactionEventResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnTransactionEventResponse?.Invoke(endTime,
                                                   this,
                                                   request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.StatusNotificationResponse>

            SendStatusNotification(EVSE_Id            EVSEId,
                                   Connector_Id       ConnectorId,
                                   DateTime           Timestamp,
                                   ConnectorStatus    Status,
                                   CustomData?        CustomData          = null,

                                   Request_Id?        RequestId           = null,
                                   DateTime?          RequestTimestamp    = null,
                                   TimeSpan?          RequestTimeout      = null,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            var request    = new StatusNotificationRequest(
                                 ChargeBoxId,
                                 Timestamp,
                                 Status,
                                 EVSEId,
                                 ConnectorId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(startTime,
                                                    this,
                                                    request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            CSMS.StatusNotificationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendStatusNotification(request);

            response ??= new CSMS.StatusNotificationResponse(request,
                                                             Result.Server("Response is null!"));


            #region Send OnStatusNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                     this,
                                                     request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.MeterValuesResponse>

            SendMeterValues(EVSE_Id                  EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>  MeterValues,
                            CustomData?              CustomData          = null,

                            Request_Id?              RequestId           = null,
                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new MeterValuesRequest(
                                 ChargeBoxId,
                                 EVSEId,
                                 MeterValues,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(startTime,
                                             this,
                                             request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            CSMS.MeterValuesResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendMeterValues(request);

            response ??= new CSMS.MeterValuesResponse(request,
                                                      Result.Server("Response is null!"));


            #region Send OnMeterValuesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnMeterValuesResponse?.Invoke(endTime,
                                              this,
                                              request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyChargingLimitResponse>

            NotifyChargingLimit(ChargingLimit                  ChargingLimit,
                                IEnumerable<ChargingSchedule>  ChargingSchedules,
                                EVSE_Id?                       EVSEId              = null,
                                CustomData?                    CustomData          = null,

                                Request_Id?                    RequestId           = null,
                                DateTime?                      RequestTimestamp    = null,
                                TimeSpan?                      RequestTimeout      = null,
                                EventTracking_Id?              EventTrackingId     = null,
                                CancellationToken              CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyChargingLimitRequest(
                                 ChargeBoxId,
                                 ChargingLimit,
                                 ChargingSchedules,
                                 EVSEId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyChargingLimitRequest event

            try
            {

                OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                     this,
                                                     request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            CSMS.NotifyChargingLimitResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyChargingLimit(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyChargingLimitResponse(request,
                                                              Result.Server("Response is null!"));


            #region Send OnNotifyChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                      this,
                                                      request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.ClearedChargingLimitResponse>

            SendClearedChargingLimit(ChargingLimitSources  ChargingLimitSource,
                                     EVSE_Id?              EVSEId,
                                     CustomData?           CustomData          = null,

                                     Request_Id?           RequestId           = null,
                                     DateTime?             RequestTimestamp    = null,
                                     TimeSpan?             RequestTimeout      = null,
                                     EventTracking_Id?     EventTrackingId     = null,
                                     CancellationToken     CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ClearedChargingLimitRequest(
                                 ChargeBoxId,
                                 ChargingLimitSource,
                                 EVSEId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnClearedChargingLimitRequest event

            try
            {

                OnClearedChargingLimitRequest?.Invoke(startTime,
                                                      this,
                                                      request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            CSMS.ClearedChargingLimitResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.SendClearedChargingLimit(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.ClearedChargingLimitResponse(request,
                                                               Result.Server("Response is null!"));


            #region Send OnClearedChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitResponse?.Invoke(endTime,
                                                       this,
                                                       request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.ReportChargingProfilesResponse>

            ReportChargingProfiles(Int32                         ReportChargingProfilesRequestId,
                                   ChargingLimitSources          ChargingLimitSource,
                                   EVSE_Id                       EVSEId,
                                   IEnumerable<ChargingProfile>  ChargingProfiles,
                                   Boolean?                      ToBeContinued       = null,
                                   CustomData?                   CustomData          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken             CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new ReportChargingProfilesRequest(
                                 ChargeBoxId,
                                 ReportChargingProfilesRequestId,
                                 ChargingLimitSource,
                                 EVSEId,
                                 ChargingProfiles,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnReportChargingProfilesRequest event

            try
            {

                OnReportChargingProfilesRequest?.Invoke(startTime,
                                                        this,
                                                        request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            CSMS.ReportChargingProfilesResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.ReportChargingProfiles(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.ReportChargingProfilesResponse(request,
                                                                 Result.Server("Response is null!"));


            #region Send OnReportChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesResponse?.Invoke(endTime,
                                                         this,
                                                         request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyEVChargingScheduleResponse>

            NotifyEVChargingSchedule(Int32              NotifyEVChargingScheduleRequestId,
                                     DateTime           TimeBase,
                                     EVSE_Id            EVSEId,
                                     ChargingSchedule   ChargingSchedule,
                                     Byte?              SelectedScheduleTupleId    = null,
                                     Boolean?           PowerToleranceAcceptance   = null,
                                     CustomData?        CustomData                 = null,

                                     Request_Id?        RequestId                  = null,
                                     DateTime?          RequestTimestamp           = null,
                                     TimeSpan?          RequestTimeout             = null,
                                     EventTracking_Id?  EventTrackingId            = null,
                                     CancellationToken  CancellationToken          = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyEVChargingScheduleRequest(
                                 ChargeBoxId,
                                 TimeBase,
                                 EVSEId,
                                 ChargingSchedule,
                                 SelectedScheduleTupleId,
                                 PowerToleranceAcceptance,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyEVChargingScheduleRequest event

            try
            {

                OnNotifyEVChargingScheduleRequest?.Invoke(startTime,
                                                          this,
                                                          request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingScheduleRequest));
            }

            #endregion


            CSMS.NotifyEVChargingScheduleResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyEVChargingSchedule(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyEVChargingScheduleResponse(request,
                                                                   Result.Server("Response is null!"));


            #region Send OnNotifyEVChargingScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleResponse?.Invoke(endTime,
                                                           this,
                                                           request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyEVChargingScheduleResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyPriorityChargingResponse>

            NotifyPriorityCharging(Int32              NotifyPriorityChargingRequestId,
                                   Transaction_Id     TransactionId,
                                   Boolean            Activated,
                                   CustomData?        CustomData          = null,

                                   Request_Id?        RequestId           = null,
                                   DateTime?          RequestTimestamp    = null,
                                   TimeSpan?          RequestTimeout      = null,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyPriorityChargingRequest(
                                 ChargeBoxId,
                                 TransactionId,
                                 Activated,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyPriorityChargingRequest event

            try
            {

                OnNotifyPriorityChargingRequest?.Invoke(startTime,
                                                        this,
                                                        request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyPriorityChargingRequest));
            }

            #endregion


            CSMS.NotifyPriorityChargingResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyPriorityCharging(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.NotifyPriorityChargingResponse(request,
                                                                 Result.Server("Response is null!"));


            #region Send OnNotifyPriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingResponse?.Invoke(endTime,
                                                         this,
                                                         request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyPriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PullDynamicScheduleUpdate            (PullDynamicScheduleUpdateRequestId, ChargingProfileId, ...)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting PullDynamicScheduleUpdateRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.PullDynamicScheduleUpdateResponse>

            PullDynamicScheduleUpdate(Int32               PullDynamicScheduleUpdateRequestId,
                                      ChargingProfile_Id  ChargingProfileId,
                                      CustomData?         CustomData          = null,

                                      Request_Id?         RequestId           = null,
                                      DateTime?           RequestTimestamp    = null,
                                      TimeSpan?           RequestTimeout      = null,
                                      EventTracking_Id?   EventTrackingId     = null,
                                      CancellationToken   CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new PullDynamicScheduleUpdateRequest(
                                 ChargeBoxId,
                                 ChargingProfileId,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnPullDynamicScheduleUpdateRequest event

            try
            {

                OnPullDynamicScheduleUpdateRequest?.Invoke(startTime,
                                                           this,
                                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
            }

            #endregion


            CSMS.PullDynamicScheduleUpdateResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.PullDynamicScheduleUpdate(request);

            if (response is not null)
            {
                
            }

            response ??= new CSMS.PullDynamicScheduleUpdateResponse(request,
                                                                    Result.Server("Response is null!"));


            #region Send OnPullDynamicScheduleUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateResponse?.Invoke(endTime,
                                                            this,
                                                            request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(Int32                     NotifyDisplayMessagesRequestId,
                                  IEnumerable<MessageInfo>  MessageInfos,
                                  Boolean?                  ToBeContinued       = null,
                                  CustomData?               CustomData          = null,

                                  Request_Id?               RequestId           = null,
                                  DateTime?                 RequestTimestamp    = null,
                                  TimeSpan?                 RequestTimeout      = null,
                                  EventTracking_Id?         EventTrackingId     = null,
                                  CancellationToken         CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyDisplayMessagesRequest(
                                 ChargeBoxId,
                                 NotifyDisplayMessagesRequestId,
                                 MessageInfos,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyDisplayMessagesRequest event

            try
            {

                OnNotifyDisplayMessagesRequest?.Invoke(startTime,
                                                       this,
                                                       request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyDisplayMessagesRequest));
            }

            #endregion


            CSMS.NotifyDisplayMessagesResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyDisplayMessages(request);

            response ??= new CSMS.NotifyDisplayMessagesResponse(request,
                                                                Result.Server("Response is null!"));


            #region Send OnNotifyDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                        this,
                                                        request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

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
        public async Task<CSMS.NotifyCustomerInformationResponse>

            NotifyCustomerInformation(Int64              NotifyCustomerInformationRequestId,
                                      String             Data,
                                      UInt32             SequenceNumber,
                                      DateTime           GeneratedAt,
                                      Boolean?           ToBeContinued       = null,
                                      CustomData?        CustomData          = null,

                                      Request_Id?        RequestId           = null,
                                      DateTime?          RequestTimestamp    = null,
                                      TimeSpan?          RequestTimeout      = null,
                                      EventTracking_Id?  EventTrackingId     = null,
                                      CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyCustomerInformationRequest(
                                 ChargeBoxId,
                                 NotifyCustomerInformationRequestId,
                                 Data,
                                 SequenceNumber,
                                 GeneratedAt,
                                 ToBeContinued,
                                 null,
                                 CustomData,

                                 RequestId        ?? NextRequestId,
                                 RequestTimestamp ?? startTime,
                                 RequestTimeout   ?? DefaultRequestTimeout,
                                 EventTrackingId,
                                 CancellationToken
                             );

            #endregion

            #region Send OnNotifyCustomerInformationRequest event

            try
            {

                OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                           this,
                                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            CSMS.NotifyCustomerInformationResponse? response = null;

            if (CSClient is not null)
                response = await CSClient.NotifyCustomerInformation(request);

            response ??= new CSMS.NotifyCustomerInformationResponse(request,
                                                                    Result.Server("Response is null!"));


            #region Send OnNotifyCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                            this,
                                                            request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
