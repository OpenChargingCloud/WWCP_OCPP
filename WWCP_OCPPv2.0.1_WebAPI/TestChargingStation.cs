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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_0_1.CS;
using System.Text;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{


    public class ChargingStationConnector
    {

        public Connector_Id       Id              { get; }


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
            this.AdminStatus        = Status;
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
                throw new ArgumentNullException(nameof(ChargeBoxId),        "The given charge box identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given charging station vendor must not be null or empty!");

            if (Model.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),   "The given charging station model must not be null or empty!");


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

            var WSClient   = new ChargingStationWSClient(
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

            this.CSClient  = WSClient;

            WireEvents(WSClient);

            var response = await WSClient.Connect();

            return response;

        }

        #endregion

        #region WireEvents(CPServer)


        private readonly ConcurrentDictionary<DisplayMessage_Id, MessageInfo>     displayMessages   = new ();
        private readonly ConcurrentDictionary<Reservation_Id,    Reservation_Id>  reservations      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Transaction>     transactions      = new ();
        private readonly ConcurrentDictionary<Transaction_Id,    Decimal>         totalCosts        = new ();
        private readonly ConcurrentDictionary<CertificateUse,    Certificate>     certificates      = new ();

        public void WireEvents(IChargingStationServer CPServer)
        {

            #region OnReset

            CPServer.OnReset += async (LogTimestamp,
                                       Sender,
                                       Request,
                                       CancellationToken) => {

                #region Send OnResetRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnResetRequest?.Invoke(startTime,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnResetRequest));
                }

                #endregion


                await Task.Delay(10);


                ResetResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid reset request for charge box '", Request.ChargeBoxId, "'!"));
                    response = new ResetResponse(Request:      Request,
                                                 Status:       ResetStatus.Rejected,
                                                 StatusInfo:   null,
                                                 CustomData:   null);
                }
                else
                {
                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming '", Request.ResetType, "' reset request accepted."));
                    response = new ResetResponse(Request:      Request,
                                                 Status:       ResetStatus.Accepted,
                                                 StatusInfo:   null,
                                                 CustomData:   null);
                }


                #region Send OnResetResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnResetResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnResetResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateFirmware

            CPServer.OnUpdateFirmware += async (LogTimestamp,
                                                Sender,
                                                Request,
                                                CancellationToken) => {

                #region Send OnUpdateFirmwareRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnUpdateFirmwareRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUpdateFirmwareRequest));
                }

                #endregion


                await Task.Delay(10);


                UpdateFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UpdateFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new UpdateFirmwareResponse(Request:      Request,
                                                          Status:       UpdateFirmwareStatus.Rejected,
                                                          StatusInfo:   null,
                                                          CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UpdateFirmware request for '" + Request.Firmware.FirmwareURL + "'.");

                    response = new UpdateFirmwareResponse(Request:      Request,
                                                          Status:       UpdateFirmwareStatus.Accepted,
                                                          StatusInfo:   null,
                                                          CustomData:   null);

                }


                #region Send OnUpdateFirmwareResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnUpdateFirmwareResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     Request,
                                                     response,
                                                     responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUpdateFirmwareResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnPublishFirmware

            CPServer.OnPublishFirmware += async (LogTimestamp,
                                                 Sender,
                                                 Request,
                                                 CancellationToken) => {

                #region Send OnPublishFirmwareRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnPublishFirmwareRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareRequest));
                }

                #endregion


                await Task.Delay(10);


                PublishFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid PublishFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new PublishFirmwareResponse(Request:      Request,
                                                           Status:       GenericStatus.Rejected,
                                                           StatusInfo:   null,
                                                           CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming PublishFirmware request for '" + Request.DownloadLocation + "'.");

                    response = new PublishFirmwareResponse(Request:      Request,
                                                           Status:       GenericStatus.Accepted,
                                                           StatusInfo:   null,
                                                           CustomData:   null);

                }


                #region Send OnPublishFirmwareResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnPublishFirmwareResponse?.Invoke(responseTimestamp,
                                                      this,
                                                      Request,
                                                      response,
                                                      responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnPublishFirmwareResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnpublishFirmware

            CPServer.OnUnpublishFirmware += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnUnpublishFirmwareRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnUnpublishFirmwareRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnpublishFirmwareRequest));
                }

                #endregion


                await Task.Delay(10);


                UnpublishFirmwareResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UnpublishFirmware request for charge box '{Request.ChargeBoxId}'!");

                    response = new UnpublishFirmwareResponse(Request:      Request,
                                                             Status:       UnpublishFirmwareStatus.Unknown,
                                                             CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UnpublishFirmware request for '" + Request.MD5Checksum + "'.");

                    response = new UnpublishFirmwareResponse(Request:      Request,
                                                             Status:       UnpublishFirmwareStatus.Unpublished,
                                                             CustomData:   null);

                }


                #region Send OnUnpublishFirmwareResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnUnpublishFirmwareResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnpublishFirmwareResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetBaseReport

            CPServer.OnGetBaseReport += async (LogTimestamp,
                                               Sender,
                                               Request,
                                               CancellationToken) => {

                #region Send OnGetBaseReportRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetBaseReportRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetBaseReportRequest));
                }

                #endregion


                await Task.Delay(10);


                GetBaseReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetBaseReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetBaseReportResponse(Request:      Request,
                                                         Status:       GenericDeviceModelStatus.Rejected,
                                                         StatusInfo:   null,
                                                         CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetBaseReport request accepted.");

                    response = new GetBaseReportResponse(Request:      Request,
                                                         Status:       GenericDeviceModelStatus.Accepted,
                                                         StatusInfo:   null,
                                                         CustomData:   null);

                }


                #region Send OnGetBaseReportResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetBaseReportResponse?.Invoke(responseTimestamp,
                                                    this,
                                                    Request,
                                                    response,
                                                    responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetBaseReportResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetReport

            CPServer.OnGetReport += async (LogTimestamp,
                                           Sender,
                                           Request,
                                           CancellationToken) => {

                #region Send OnGetReportRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetReportRequest?.Invoke(startTime,
                                               this,
                                               Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetReportRequest));
                }

                #endregion


                await Task.Delay(10);


                GetReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetReportResponse(Request:      Request,
                                                     Status:       GenericDeviceModelStatus.Rejected,
                                                     StatusInfo:   null,
                                                     CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetReport request accepted.");

                    response = new GetReportResponse(Request:      Request,
                                                     Status:       GenericDeviceModelStatus.Accepted,
                                                     StatusInfo:   null,
                                                     CustomData:   null);

                }


                #region Send OnGetReportResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetReportResponse?.Invoke(responseTimestamp,
                                                this,
                                                Request,
                                                response,
                                                responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetReportResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetLog

            CPServer.OnGetLog += async (LogTimestamp,
                                        Sender,
                                        Request,
                                        CancellationToken) => {

                #region Send OnGetLogRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetLogRequest?.Invoke(startTime,
                                            this,
                                            Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLogRequest));
                }

                #endregion


                await Task.Delay(10);


                GetLogResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetLog request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetLogResponse(Request:      Request,
                                                  Status:       LogStatus.Rejected,
                                                  StatusInfo:   null,
                                                  CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetLog request accepted.");

                    response = new GetLogResponse(Request:      Request,
                                                  Status:       LogStatus.Accepted,
                                                  StatusInfo:   null,
                                                  CustomData:   null);

                }


                #region Send OnGetLogResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetLogResponse?.Invoke(responseTimestamp,
                                             this,
                                             Request,
                                             response,
                                             responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLogResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariables

            CPServer.OnSetVariables += async (LogTimestamp,
                                              Sender,
                                              Request,
                                              CancellationToken) => {

                #region Send OnSetVariablesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetVariablesRequest?.Invoke(startTime,
                                                  this,
                                                  Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetVariablesRequest));
                }

                #endregion


                await Task.Delay(10);


                SetVariablesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetVariables request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetVariablesResponse(Request:              Request,
                                                        SetVariableResults:   Array.Empty<SetVariableResult>(),
                                                        CustomData:           null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetVariables request accepted.");

                    response = new SetVariablesResponse(Request:              Request,
                                                        SetVariableResults:   Request.VariableData.Select(variableData => new SetVariableResult(
                                                                                                                              Status:                SetVariableStatus.Accepted,
                                                                                                                              Component:             variableData.Component,
                                                                                                                              Variable:              variableData.Variable,
                                                                                                                              AttributeType:         variableData.AttributeType,
                                                                                                                              AttributeStatusInfo:   null,
                                                                                                                              CustomData:            null
                                                                                                                          )),
                                                        CustomData:           null);

                }


                #region Send OnSetVariablesResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetVariablesResponse?.Invoke(responseTimestamp,
                                                   this,
                                                   Request,
                                                   response,
                                                   responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetVariablesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetVariables

            CPServer.OnGetVariables += async (LogTimestamp,
                                              Sender,
                                              Request,
                                              CancellationToken) => {

                #region Send OnGetVariablesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetVariablesRequest?.Invoke(startTime,
                                                  this,
                                                  Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetVariablesRequest));
                }

                #endregion


                await Task.Delay(10);


                GetVariablesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetVariables request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetVariablesResponse(Request:      Request,
                                                        Results:      Array.Empty<GetVariableResult>(),
                                                        CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetVariables request accepted.");

                    response = new GetVariablesResponse(Request:      Request,
                                                        Results:      Request.VariableData.Select(variableData => new GetVariableResult(
                                                                                                                      AttributeStatus:       GetVariableStatus.Accepted,
                                                                                                                      Component:             variableData.Component,
                                                                                                                      Variable:              variableData.Variable,
                                                                                                                      AttributeValue:        "",
                                                                                                                      AttributeType:         variableData.AttributeType,
                                                                                                                      AttributeStatusInfo:   null,
                                                                                                                      CustomData:            null
                                                                                                                  )),
                                                        CustomData:   null);

                }


                #region Send OnGetVariablesResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetVariablesResponse?.Invoke(responseTimestamp,
                                                   this,
                                                   Request,
                                                   response,
                                                   responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetVariablesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringBase

            CPServer.OnSetMonitoringBase += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnSetMonitoringBaseRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetMonitoringBaseRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetMonitoringBaseRequest));
                }

                #endregion


                await Task.Delay(10);


                SetMonitoringBaseResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetMonitoringBase request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetMonitoringBaseResponse(Request:      Request,
                                                             Status:       GenericDeviceModelStatus.Rejected,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetMonitoringBase request accepted.");

                    response = new SetMonitoringBaseResponse(Request:      Request,
                                                             Status:       GenericDeviceModelStatus.Accepted,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }


                #region Send OnSetMonitoringBaseResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetMonitoringBaseResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetMonitoringBaseResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetMonitoringReport

            CPServer.OnGetMonitoringReport += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnGetMonitoringReportRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetMonitoringReportRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetMonitoringReportRequest));
                }

                #endregion


                await Task.Delay(10);


                GetMonitoringReportResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetMonitoringReport request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetMonitoringReportResponse(Request:      Request,
                                                               Status:       GenericDeviceModelStatus.Rejected,
                                                               StatusInfo:   null,
                                                               CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetMonitoringReport request accepted.");

                    response = new GetMonitoringReportResponse(Request:      Request,
                                                               Status:       GenericDeviceModelStatus.Accepted,
                                                               StatusInfo:   null,
                                                               CustomData:   null);

                }


                #region Send OnGetMonitoringReportResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetMonitoringReportResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetMonitoringReportResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetMonitoringLevel

            CPServer.OnSetMonitoringLevel += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnSetMonitoringLevelRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetMonitoringLevelRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetMonitoringLevelRequest));
                }

                #endregion


                await Task.Delay(10);


                SetMonitoringLevelResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetMonitoringLevel request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetMonitoringLevelResponse(Request:      Request,
                                                              Status:       GenericStatus.Rejected,
                                                              StatusInfo:   null,
                                                              CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetMonitoringLevel request accepted.");

                    response = new SetMonitoringLevelResponse(Request:      Request,
                                                              Status:       GenericStatus.Accepted,
                                                              StatusInfo:   null,
                                                              CustomData:   null);

                }


                #region Send OnSetMonitoringLevelResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetMonitoringLevelResponse?.Invoke(responseTimestamp,
                                                         this,
                                                         Request,
                                                         response,
                                                         responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetMonitoringLevelResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetVariableMonitoring

            CPServer.OnSetVariableMonitoring += async (LogTimestamp,
                                                       Sender,
                                                       Request,
                                                       CancellationToken) => {

                #region Send OnSetVariableMonitoringRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetVariableMonitoringRequest?.Invoke(startTime,
                                                           this,
                                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetVariableMonitoringRequest));
                }

                #endregion


                await Task.Delay(10);


                SetVariableMonitoringResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetVariableMonitoring request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetVariableMonitoringResponse(Request:                Request,
                                                                 SetMonitoringResults:   Array.Empty<SetMonitoringResult>(),
                                                                 CustomData:             null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetVariableMonitoring request accepted.");

                    response = new SetVariableMonitoringResponse(Request:                Request,
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
                                                                 CustomData:             null);

                }


                #region Send OnSetVariableMonitoringResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetVariableMonitoringResponse?.Invoke(responseTimestamp,
                                                            this,
                                                            Request,
                                                            response,
                                                            responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetVariableMonitoringResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearVariableMonitoring

            CPServer.OnClearVariableMonitoring += async (LogTimestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) => {

                #region Send OnClearVariableMonitoringRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnClearVariableMonitoringRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearVariableMonitoringRequest));
                }

                #endregion


                await Task.Delay(10);


                ClearVariableMonitoringResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearVariableMonitoring request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearVariableMonitoringResponse(Request:                  Request,
                                                                   ClearMonitoringResults:   Array.Empty<ClearMonitoringResult>(),
                                                                   CustomData:               null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearVariableMonitoring request accepted.");

                    response = new ClearVariableMonitoringResponse(Request:                  Request,
                                                                   ClearMonitoringResults:   Request.VariableMonitoringIds.Select(variableMonitoringId => new ClearMonitoringResult(
                                                                                                                                                              Status:       ClearMonitoringStatus.Accepted,
                                                                                                                                                              Id:           variableMonitoringId,
                                                                                                                                                              StatusInfo:   null,
                                                                                                                                                              CustomData:   null
                                                                                                                                                          )),
                                                                   CustomData:               null);

                }


                #region Send OnClearVariableMonitoringResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnClearVariableMonitoringResponse?.Invoke(responseTimestamp,
                                                              this,
                                                              Request,
                                                              response,
                                                              responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearVariableMonitoringResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetNetworkProfile

            CPServer.OnSetNetworkProfile += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnSetNetworkProfileRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetNetworkProfileRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetNetworkProfileRequest));
                }

                #endregion


                await Task.Delay(10);


                SetNetworkProfileResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetNetworkProfile request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetNetworkProfileResponse(Request:      Request,
                                                             Status:       SetNetworkProfileStatus.Rejected,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetNetworkProfile request accepted.");

                    response = new SetNetworkProfileResponse(Request:      Request,
                                                             Status:       SetNetworkProfileStatus.Accepted,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }


                #region Send OnSetNetworkProfileResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetNetworkProfileResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetNetworkProfileResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnChangeAvailability

            CPServer.OnChangeAvailability += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnChangeAvailabilityRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnChangeAvailabilityRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnChangeAvailabilityRequest));
                }

                #endregion


                await Task.Delay(10);


                ChangeAvailabilityResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid ChangeAvailability request for charge box '", Request.ChargeBoxId, "'!"));

                    response = new ChangeAvailabilityResponse(Request,
                                                              ChangeAvailabilityStatus.Rejected);

                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming ChangeAvailability '", Request.OperationalStatus, "' request for EVSE '", Request.EVSE?.Id.ToString() ?? "?", "'."));

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
                        response = new ChangeAvailabilityResponse(Request,
                                                                  ChangeAvailabilityStatus.Rejected);
                }


                #region Send OnChangeAvailabilityResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnChangeAvailabilityResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnChangeAvailabilityResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnTriggerMessage

            CPServer.OnTriggerMessage += async (LogTimestamp,
                                                Sender,
                                                Request,
                                                CancellationToken) => {

                #region Send OnTriggerMessageRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnTriggerMessageRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTriggerMessageRequest));
                }

                #endregion


                await Task.Delay(10);


                TriggerMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid TriggerMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new TriggerMessageResponse(Request,
                                                          TriggerMessageStatus.Rejected);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at EVSE '" + Request.EVSEId + "'.");

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
                                if (!Request.EVSEId.HasValue)
                                    await SendStatusNotification(
                                              EVSEId:        Request.EVSEId!.Value,
                                              ConnectorId:   Connector_Id.Parse(1),
                                              Timestamp:     Timestamp.Now,
                                              Status:        evses[Request.EVSEId!.Value].Status,
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
                                   MessageTriggers.SignChargePointCertificate

                                       => new TriggerMessageResponse(Request,
                                                                     TriggerMessageStatus.Accepted),


                                   MessageTriggers.MeterValues or
                                   MessageTriggers.StatusNotification

                                       => Request.EVSEId.HasValue
                                              ? new TriggerMessageResponse(Request, TriggerMessageStatus.Accepted)
                                              : new TriggerMessageResponse(Request, TriggerMessageStatus.Rejected),


                                   _   => new TriggerMessageResponse(Request,
                                                                     TriggerMessageStatus.Rejected),

                               };
                }


                #region Send OnTriggerMessageResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnTriggerMessageResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     Request,
                                                     response,
                                                     responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnTriggerMessageResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnIncomingDataTransfer

            CPServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnDataTransferRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferRequest));
                }

                #endregion


                await Task.Delay(10);


                DataTransferResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid data transfer request for charge box '", Request.ChargeBoxId, "'!"));

                    response = new DataTransferResponse(
                                   Request,
                                   DataTransferStatus.Rejected
                               );

                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming data transfer request: ", Request.VendorId, ".", Request.MessageId ?? "-", ": ", Request.Data ?? "-"));

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

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnIncomingDataTransferResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           Request,
                                                           response,
                                                           responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnCertificateSigned

            CPServer.OnCertificateSigned += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnCertificateSignedRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnCertificateSignedRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCertificateSignedRequest));
                }

                #endregion


                await Task.Delay(10);


                CertificateSignedResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid CertificateSigned request for charge box '", Request.ChargeBoxId, "'!"));

                    response = new CertificateSignedResponse(
                                   Request:      Request,
                                   Status:       CertificateSignedStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming CertificateSigned request accepted."));

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

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnCertificateSignedResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCertificateSignedResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnInstallCertificate

            CPServer.OnInstallCertificate += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnInstallCertificateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnInstallCertificateRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnInstallCertificateRequest));
                }

                #endregion


                await Task.Delay(10);


                InstallCertificateResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid InstallCertificate request for charge box '", Request.ChargeBoxId, "'!"));

                    response = new InstallCertificateResponse(
                                   Request:      Request,
                                   Status:       CertificateStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming InstallCertificate request accepted."));

                    var success = certificates.AddOrUpdate(Request.CertificateType,
                                                           a     => Request.Certificate,
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

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnInstallCertificateResponse?.Invoke(responseTimestamp,
                                                         this,
                                                         Request,
                                                         response,
                                                         responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnInstallCertificateResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetInstalledCertificateIds

            CPServer.OnGetInstalledCertificateIds += async (LogTimestamp,
                                                            Sender,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnGetInstalledCertificateIdsRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
                                                                this,
                                                                Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetInstalledCertificateIdsRequest));
                }

                #endregion


                await Task.Delay(10);


                GetInstalledCertificateIdsResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid GetInstalledCertificateIds request for charge box '", Request.ChargeBoxId, "'!"));
                    response = new GetInstalledCertificateIdsResponse(Request:                    Request,
                                                                      Status:                     GetInstalledCertificateStatus.NotFound,
                                                                      CertificateHashDataChain:   Array.Empty<CertificateHashData>(),
                                                                      StatusInfo:                 null,
                                                                      CustomData:                 null);
                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming GetInstalledCertificateIds request accepted."));

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

                    response = new GetInstalledCertificateIdsResponse(Request:                    Request,
                                                                      Status:                     GetInstalledCertificateStatus.Accepted,
                                                                      CertificateHashDataChain:   certs,
                                                                      StatusInfo:                 null,
                                                                      CustomData:                 null);

                }


                #region Send OnGetInstalledCertificateIdsResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetInstalledCertificateIdsResponse?.Invoke(responseTimestamp,
                                                                 this,
                                                                 Request,
                                                                 response,
                                                                 responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetInstalledCertificateIdsResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnDeleteCertificate

            CPServer.OnDeleteCertificate += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnDeleteCertificateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnDeleteCertificateRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDeleteCertificateRequest));
                }

                #endregion


                await Task.Delay(10);


                DeleteCertificateResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Invalid DeleteCertificate request for charge box '", Request.ChargeBoxId, "'!"));
                    response = new DeleteCertificateResponse(
                                   Request:      Request,
                                   Status:       DeleteCertificateStatus.Failed,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );
                }
                else
                {

                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming DeleteCertificate request accepted."));

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

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnDeleteCertificateResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnDeleteCertificateResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnGetLocalListVersion

            CPServer.OnGetLocalListVersion += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnGetLocalListVersionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetLocalListVersionRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLocalListVersionRequest));
                }

                #endregion


                await Task.Delay(10);


                GetLocalListVersionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetLocalListVersion request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetLocalListVersionResponse(Request,
                                                               0);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetLocalListVersion request.");

                    response = new GetLocalListVersionResponse(Request,
                                                               0);

                }


                #region Send OnGetLocalListVersionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetLocalListVersionResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetLocalListVersionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSendLocalList

            CPServer.OnSendLocalList += async (LogTimestamp,
                                               Sender,
                                               Request,
                                               CancellationToken) => {

                #region Send OnSendLocalListRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSendLocalListRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSendLocalListRequest));
                }

                #endregion


                await Task.Delay(10);


                SendLocalListResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SendLocalList request for charge box '{Request.ChargeBoxId}'!");

                    response = new SendLocalListResponse(Request,
                                                         SendLocalListStatus.NotSupported);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.VersionNumber + "'.");

                    response = new SendLocalListResponse(Request,
                                                         SendLocalListStatus.Accepted);

                }


                #region Send OnSendLocalListResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSendLocalListResponse?.Invoke(responseTimestamp,
                                                    this,
                                                    Request,
                                                    response,
                                                    responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSendLocalListResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearCache

            CPServer.OnClearCache += async (LogTimestamp,
                                            Sender,
                                            Request,
                                            CancellationToken) => {

                #region Send OnClearCacheRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnClearCacheRequest?.Invoke(startTime,
                                                this,
                                                Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearCacheRequest));
                }

                #endregion


                await Task.Delay(10);


                ClearCacheResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearCache request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearCacheResponse(Request,
                                                      ClearCacheStatus.Rejected);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearCache request.");

                    response = new ClearCacheResponse(Request,
                                                      ClearCacheStatus.Accepted);

                }


                #region Send OnClearCacheResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnClearCacheResponse?.Invoke(responseTimestamp,
                                                 this,
                                                 Request,
                                                 response,
                                                 responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearCacheResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnReserveNow

            CPServer.OnReserveNow += async (LogTimestamp,
                                            Sender,
                                            Request,
                                            CancellationToken) => {

                #region Send OnReserveNowRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnReserveNowRequest?.Invoke(startTime,
                                                this,
                                                Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReserveNowRequest));
                }

                #endregion


                ReserveNowResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ReserveNow request for charge box '{Request.ChargeBoxId}'!");

                    response = new ReserveNowResponse(Request:      Request,
                                                      Status:       ReservationStatus.Rejected,
                                                      StatusInfo:   null,
                                                      CustomData:   null);

                }
                else
                {

                    var success = reservations.TryAdd(Request.ReservationId,
                                                      Request.ReservationId);

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ReserveNow request " + (success
                                                                                                    ? "accepted"
                                                                                                    : "rejected") + ".");

                    response = new ReserveNowResponse(Request:      Request,
                                                      Status:       success
                                                                        ? ReservationStatus.Accepted
                                                                        : ReservationStatus.Rejected,
                                                      StatusInfo:   null,
                                                      CustomData:   null);

                }


                #region Send OnReserveNowResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnReserveNowResponse?.Invoke(responseTimestamp,
                                                 this,
                                                 Request,
                                                 response,
                                                 responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnReserveNowResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnCancelReservation

            CPServer.OnCancelReservation += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnCancelReservationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnCancelReservationRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCancelReservationRequest));
                }

                #endregion


                CancelReservationResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CancelReservation request for charge box '{Request.ChargeBoxId}'!");

                    response = new CancelReservationResponse(Request:      Request,
                                                             Status:       CancelReservationStatus.Rejected,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }
                else
                {

                    var success = reservations.TryRemove(Request.ReservationId, out _);

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming CancelReservation request " + (success
                                                                                                           ? "accepted"
                                                                                                           : "rejected") + ".");

                    response = new CancelReservationResponse(Request:      Request,
                                                             Status:       success
                                                                               ? CancelReservationStatus.Accepted
                                                                               : CancelReservationStatus.Rejected,
                                                             StatusInfo:   null,
                                                             CustomData:   null);

                }


                #region Send OnCancelReservationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnCancelReservationResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCancelReservationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStartTransaction

            CPServer.OnRequestStartTransaction += async (LogTimestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) => {

                #region Send OnRequestStartTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnRequestStartTransactionRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnRequestStartTransactionRequest));
                }

                #endregion


                RequestStartTransactionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid RequestStartTransaction request for charge box '{Request.ChargeBoxId}'!");

                    response = new RequestStartTransactionResponse(Request,
                                                                   RequestStartStopStatus.Rejected);

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
                                      CableMaxCurrent:      32,
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
                                       CustomData:      null);

                    }
                    else
                        response = new RequestStartTransactionResponse(Request,
                                                                       RequestStartStopStatus.Rejected);

                }


                #region Send OnRequestStartTransactionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnRequestStartTransactionResponse?.Invoke(responseTimestamp,
                                                              this,
                                                              Request,
                                                              response,
                                                              responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnRequestStartTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnRequestStopTransaction

            CPServer.OnRequestStopTransaction += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnRequestStopTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnRequestStopTransactionRequest?.Invoke(startTime,
                                                            this,
                                                            Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnRequestStopTransactionRequest));
                }

                #endregion


                RequestStopTransactionResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid RequestStopTransaction request for charge box '{Request.ChargeBoxId}'!");

                    response = new RequestStopTransactionResponse(Request,
                                                                   RequestStartStopStatus.Rejected);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming RequestStopTransaction for '" + Request.TransactionId + "'.");

                    // ToDo: lock(evses)
                    var evse = evses.Values.FirstOrDefault(evse => Request.TransactionId == evse.TransactionId);

                    if (evse is not null)
                    {

                        evse.IsCharging              = false;

                        evse.StopTimestamp           = Timestamp.Now;
                        evse.MeterStopValue          = 123;
                        evse.SignedStopMeterValue    = "123";

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
                                      CableMaxCurrent:      32,
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

                        response = new RequestStopTransactionResponse(Request,
                                                                      RequestStartStopStatus.Accepted);

                    }
                    else
                        response = new RequestStopTransactionResponse(Request,
                                                                      RequestStartStopStatus.Rejected);

                }


                #region Send OnRequestStopTransactionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnRequestStopTransactionResponse?.Invoke(responseTimestamp,
                                                             this,
                                                             Request,
                                                             response,
                                                             responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnRequestStopTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetTransactionStatus

            CPServer.OnGetTransactionStatus += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnGetTransactionStatusRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetTransactionStatusRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetTransactionStatusRequest));
                }

                                                          #endregion


                GetTransactionStatusResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetTransactionStatus request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetTransactionStatusResponse(Request,
                                                                MessagesInQueue:    false,
                                                                OngoingIndicator:   true);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetTransactionStatus for '" + Request.TransactionId + "'.");

                    if (Request.TransactionId.HasValue)
                    {

                        var foundEVSE =  evses.Values.FirstOrDefault(evse => Request.TransactionId == evse.TransactionId);

                        if (foundEVSE is not null)
                        {

                            response = new GetTransactionStatusResponse(Request,
                                                                        MessagesInQueue:    false,
                                                                        OngoingIndicator:   true);

                        }
                        else
                        {

                            response = new GetTransactionStatusResponse(Request,
                                                                        MessagesInQueue:    false,
                                                                        OngoingIndicator:   true);

                        }

                    }
                    else
                    {

                        response = new GetTransactionStatusResponse(Request,
                                                                    MessagesInQueue:    false,
                                                                    OngoingIndicator:   true);

                    }

                }


                #region Send OnGetTransactionStatusResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetTransactionStatusResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           Request,
                                                           response,
                                                           responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetTransactionStatusResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSetChargingProfile

            CPServer.OnSetChargingProfile += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnSetChargingProfileRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetChargingProfileRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetChargingProfileRequest));
                }

                #endregion


                SetChargingProfileResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetChargingProfile request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetChargingProfileResponse(Request,
                                                              ChargingProfileStatus.Rejected);

                }
                else if (Request.ChargingProfile is null)
                {

                    response = new SetChargingProfileResponse(Request,
                                                              ChargingProfileStatus.Rejected);

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

                        response = new SetChargingProfileResponse(Request,
                                                                  ChargingProfileStatus.Accepted);

                    }
                    else if (evses.ContainsKey(Request.EVSEId))
                    {

                        evses[Request.EVSEId].ChargingProfile = Request.ChargingProfile;

                        response = new SetChargingProfileResponse(Request,
                                                                  ChargingProfileStatus.Accepted);

                    }
                    else
                        response = new SetChargingProfileResponse(Request,
                                                                  ChargingProfileStatus.Rejected);

                }


                #region Send OnSetChargingProfileResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetChargingProfileResponse?.Invoke(responseTimestamp,
                                                         this,
                                                         Request,
                                                         response,
                                                         responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetChargingProfileResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetChargingProfiles

            CPServer.OnGetChargingProfiles += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnGetChargingProfilesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetChargingProfilesRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetChargingProfilesRequest));
                }

                #endregion

                // GetChargingProfilesRequestId
                // ChargingProfile
                // EVSEId

                GetChargingProfilesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetChargingProfiles request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetChargingProfilesResponse(Request,
                                                               GetChargingProfileStatus.Unknown);

                }
                else if (Request.EVSEId.HasValue && evses.ContainsKey(Request.EVSEId.Value))
                {

                    //evses[Request.EVSEId.Value].ChargingProfile = Request.ChargingProfile;

                    response = new GetChargingProfilesResponse(Request,
                                                               GetChargingProfileStatus.Accepted);

                }
                else
                   response = new GetChargingProfilesResponse(Request,
                                                              GetChargingProfileStatus.Unknown);

                #region Send OnGetChargingProfilesResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetChargingProfilesResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetChargingProfilesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearChargingProfile

            CPServer.OnClearChargingProfile += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnClearChargingProfileRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnClearChargingProfileRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearChargingProfileRequest));
                }

                #endregion

                // ChargingProfileId
                // ChargingProfileCriteria

                var response = new ClearChargingProfileResponse(Request,
                                                                ClearChargingProfileStatus.Accepted);

                #region Send OnClearChargingProfileResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnClearChargingProfileResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           Request,
                                                           response,
                                                           responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearChargingProfileResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCompositeSchedule

            CPServer.OnGetCompositeSchedule += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnGetCompositeScheduleRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetCompositeScheduleRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCompositeScheduleRequest));
                }

                #endregion

                // Status
                // Schedule
                // StatusInfo

                var response = new GetCompositeScheduleResponse(Request,
                                                                Status:     GenericStatus.Accepted,
                                                                Schedule:   null);

                #region Send OnGetCompositeScheduleResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetCompositeScheduleResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           Request,
                                                           response,
                                                           responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetCompositeScheduleResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnlockConnector

            CPServer.OnUnlockConnector += async (LogTimestamp,
                                                 Sender,
                                                 Request,
                                                 CancellationToken) => {

                #region Send OnUnlockConnectorRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnUnlockConnectorRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnlockConnectorRequest));
                }

                #endregion


                UnlockConnectorResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid UnlockConnector request for charge box '{Request.ChargeBoxId}'!");

                    response = new UnlockConnectorResponse(Request,
                                                           UnlockStatus.UnlockFailed);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

                    // ToDo: lock(connectors)

                    if (evses.TryGetValue    (Request.EVSEId,      out var evse) &&
                        evse. TryGetConnector(Request.ConnectorId, out var connector))
                    {

                        // What to do here?!

                        response = new UnlockConnectorResponse(Request,
                                                               UnlockStatus.Unlocked);

                    }
                    else
                        response = new UnlockConnectorResponse(Request,
                                                               UnlockStatus.UnlockFailed);

                }


                #region Send OnUnlockConnectorResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnUnlockConnectorResponse?.Invoke(responseTimestamp,
                                                      this,
                                                      Request,
                                                      response,
                                                      responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnUnlockConnectorResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnSetDisplayMessage

            CPServer.OnSetDisplayMessage += async (LogTimestamp,
                                                   Sender,
                                                   Request,
                                                   CancellationToken) => {

                #region Send OnSetDisplayMessageRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSetDisplayMessageRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetDisplayMessageRequest));
                }

                #endregion


                await Task.Delay(10);


                SetDisplayMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid SetDisplayMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new SetDisplayMessageResponse(Request,
                                                             Result.GenericError(""));

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetDisplayMessage request.");


                    if (displayMessages.TryAdd(Request.Message.Id,
                                               Request.Message)) {

                        response = new SetDisplayMessageResponse(Request,
                                                                 DisplayMessageStatus.Accepted);

                    }

                    else
                        response = new SetDisplayMessageResponse(Request,
                                                                 DisplayMessageStatus.Rejected);

                }


                #region Send OnSetDisplayMessageResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnSetDisplayMessageResponse?.Invoke(responseTimestamp,
                                                        this,
                                                        Request,
                                                        response,
                                                        responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnSetDisplayMessageResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetDisplayMessages

            CPServer.OnGetDisplayMessages += async (LogTimestamp,
                                                    Sender,
                                                    Request,
                                                    CancellationToken) => {

                #region Send OnGetDisplayMessagesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetDisplayMessagesRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetDisplayMessagesRequest));
                }

                #endregion


                await Task.Delay(10);


                GetDisplayMessagesResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid GetDisplayMessages request for charge box '{Request.ChargeBoxId}'!");

                    response = new GetDisplayMessagesResponse(Request,
                                                              Result.GenericError(""));

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

                    response = new GetDisplayMessagesResponse(Request,
                                                              GetDisplayMessagesStatus.Accepted);

                }


                #region Send OnGetDisplayMessagesResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetDisplayMessagesResponse?.Invoke(responseTimestamp,
                                                         this,
                                                         Request,
                                                         response,
                                                         responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnGetDisplayMessagesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearDisplayMessage

            CPServer.OnClearDisplayMessage += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnClearDisplayMessageRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnClearDisplayMessageRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearDisplayMessageRequest));
                }

                #endregion


                await Task.Delay(10);


                ClearDisplayMessageResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid ClearDisplayMessage request for charge box '{Request.ChargeBoxId}'!");

                    response = new ClearDisplayMessageResponse(Request,
                                                               Result.GenericError(""));

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearDisplayMessage request.");

                    if (displayMessages.TryGetValue(Request.DisplayMessageId, out var messageInfo) &&
                        displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(Request.DisplayMessageId, messageInfo))) {

                        response = new ClearDisplayMessageResponse(Request,
                                                                   ClearMessageStatus.Accepted);

                    }

                    else
                        response = new ClearDisplayMessageResponse(Request,
                                                                   ClearMessageStatus.Unknown);

                }


                #region Send OnClearDisplayMessageResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnClearDisplayMessageResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnClearDisplayMessageResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnCostUpdated

            CPServer.OnCostUpdated += async (LogTimestamp,
                                             Sender,
                                             Request,
                                             CancellationToken) => {

                #region Send OnCostUpdatedRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnCostUpdatedRequest?.Invoke(startTime,
                                                 this,
                                                 Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCostUpdatedRequest));
                }

                #endregion


                await Task.Delay(10);


                CostUpdatedResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CostUpdated request for charge box '{Request.ChargeBoxId}'!");

                    response = new CostUpdatedResponse(Request,
                                                       Result.GenericError(""));

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming CostUpdated request.");


                    if (transactions.ContainsKey(Request.TransactionId)) {

                        totalCosts.AddOrUpdate(Request.TransactionId,
                                               Request.TotalCost,
                                               (transactionId, totalCost) => Request.TotalCost);

                        response = new CostUpdatedResponse(Request);

                    }

                    else
                        response = new CostUpdatedResponse(Request,
                                                           Result.GenericError($"Unknown transaction identification '{Request.TransactionId}'!"));

                }


                #region Send OnCostUpdatedResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnCostUpdatedResponse?.Invoke(responseTimestamp,
                                                  this,
                                                  Request,
                                                  response,
                                                  responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCostUpdatedResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnCustomerInformation

            CPServer.OnCustomerInformation += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnCustomerInformationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnCustomerInformationRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCustomerInformationRequest));
                }

                #endregion


                await Task.Delay(10);


                CustomerInformationResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Invalid CustomerInformation request for charge box '{Request.ChargeBoxId}'!");

                    response = new CustomerInformationResponse(Request,
                                                               Result.GenericError(""));

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

                    response = new CustomerInformationResponse(Request,
                                                               CustomerInformationStatus.Accepted);

                }


                #region Send OnCustomerInformationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnCustomerInformationResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargingStation) + "." + nameof(OnCustomerInformationResponse));
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
                                  CustomData?            CustomData          = null,

                                  Request_Id?            RequestId           = null,
                                  DateTime?              RequestTimestamp    = null,
                                  TimeSpan?              RequestTimeout      = null,
                                  EventTracking_Id?      EventTrackingId     = null,
                                  CancellationToken      CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new Get15118EVCertificateRequest(
                                 ChargeBoxId,
                                 ISO15118SchemaVersion,
                                 CertificateAction,
                                 EXIRequest,
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

        #region NotifyEVChargingNeeds                (EVSEId, ChargingNeeds, MaxScheduleTuples = null, ...)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="MaxScheduleTuples">The optional maximum schedule tuples the car supports per schedule.</param>
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
                                 MaxScheduleTuples,
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

                                 Boolean?                  Offline              = null,
                                 Byte?                     NumberOfPhasesUsed   = null,
                                 Int16?                    CableMaxCurrent      = null,
                                 Reservation_Id?           ReservationId        = null,
                                 IdToken?                  IdToken              = null,
                                 EVSE?                     EVSE                 = null,
                                 IEnumerable<MeterValue>?  MeterValues          = null,
                                 CustomData?               CustomData           = null,

                                 Request_Id?               RequestId            = null,
                                 DateTime?                 RequestTimestamp     = null,
                                 TimeSpan?                 RequestTimeout       = null,
                                 EventTracking_Id?         EventTrackingId      = null,
                                 CancellationToken         CancellationToken    = default)

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

        #region NotifyEVChargingSchedule             (NotifyEVChargingScheduleRequestId, TimeBase, EVSEId, ChargingSchedule, ...)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
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
                                     CustomData?        CustomData          = null,

                                     Request_Id?        RequestId           = null,
                                     DateTime?          RequestTimestamp    = null,
                                     TimeSpan?          RequestTimeout      = null,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new NotifyEVChargingScheduleRequest(
                                 ChargeBoxId,
                                 TimeBase,
                                 EVSEId,
                                 ChargingSchedule,
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
