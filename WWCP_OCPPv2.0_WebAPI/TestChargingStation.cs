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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_0.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A charging station for testing.
    /// </summary>
    public class TestChargingStation : IChargingStationClientEvents,
                                       IEventSender
    {

        /// <summary>
        /// A charging station connector.
        /// </summary>
        public class ChargingStationConnector
        {

            public Connector_Id       Id                       { get; }

            public OperationalStatus  OperationalStatus        { get; set; }


            public Boolean            IsReserved               { get; set; }

            public Boolean            IsCharging               { get; set; }

            public IdToken            IdToken                  { get; set; }

            //public IdTagInfo          IdTagInfo                { get; set; }

            public Transaction_Id     TransactionId            { get; set; }

            public ChargingProfile    ChargingProfile          { get; set; }


            public DateTime           StartTimestamp           { get; set; }

            public UInt64             MeterStartValue          { get; set; }

            public String             SignedStartMeterValue    { get; set; }


            public DateTime           StopTimestamp            { get; set; }

            public UInt64             MeterStopValue           { get; set; }

            public String             SignedStopMeterValue     { get; set; }


            public ChargingStationConnector(Connector_Id       Id,
                                            OperationalStatus  OperationalStatus)
            {

                this.Id                 = Id;
                this.OperationalStatus  = OperationalStatus;

            }


        }




        public class EnquedRequest
        {

            public enum EnquedStatus
            {
                New,
                Processing,
                Finished
            }

            public String          Command           { get; }

            public IRequest        Request           { get; }

            public JObject         RequestJSON       { get; }

            public DateTime        EnqueTimestamp    { get; }

            public EnquedStatus    Status            { get; set; }

            public Action<Object>  ResponseAction    { get; }

            public EnquedRequest(String          Command,
                                 IRequest        Request,
                                 JObject         RequestJSON,
                                 DateTime        EnqueTimestamp,
                                 EnquedStatus    Status,
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


        private readonly            List<EnquedRequest>         EnquedRequests;

        public                      Tuple<String, String>?      HTTPBasicAuth               { get; }
        public                      DNSClient?                  DNSClient                   { get; }

        private                     Int64                       internalRequestId           = 100000;

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a CSMS.
        /// </summary>
        public IChargingStationClient?      CSClient                    { get; private set; }


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
        public String                   VendorName           { get; }

        /// <summary>
        ///  The charging station model identification.
        /// </summary>
        [Mandatory]
        public String                   Model            { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        [Optional]
        public I18NString?              Description                 { get; }

        /// <summary>
        /// The optional serial number of the charging station.
        /// </summary>
        [Optional]
        public String?                  SerialNumber     { get; }

        ///// <summary>
        ///// The optional serial number of the charging station.
        ///// </summary>
        //[Optional]
        //public String?                  ChargeBoxSerialNumber       { get; }

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

        private readonly Dictionary<Connector_Id, ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors.Values;

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
        /// An event fired whenever a Reset request will be sent to the CSMS.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the CSMS.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the CSMS.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the CSMS.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the CSMS.
        /// </summary>
        public event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the CSMS.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the CSMS.
        /// </summary>
        public event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the CSMS.
        /// </summary>
        public event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the CSMS.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the CSMS.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the CSMS.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the CSMS.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the CSMS.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the CSMS.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the CSMS.
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
        /// An event fired whenever a SignedCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the CSMS.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the CSMS.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the CSMS.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the CSMS.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the CSMS.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the CSMS.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the CSMS.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region StartCharging

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the CSMS.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region StopCharging

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the CSMS.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the CSMS.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the CSMS.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the CSMS.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the CSMS.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the CSMS.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the CSMS.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the CSMS.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the CSMS.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the CSMS.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the CSMS.
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
        /// <param name="NumberOfConnectors">Number of available connectors.</param>
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
        public TestChargingStation(ChargeBox_Id            ChargeBoxId,
                                   Byte                    NumberOfConnectors,
                                   String                  VendorName,
                                   String                  Model,

                                   I18NString?             Description               = null,
                                   String?                 SerialNumber              = null,
                                   String?                 FirmwareVersion           = null,
                                   Modem?                  Modem                     = null,
                                   String?                 MeterType                 = null,
                                   String?                 MeterSerialNumber         = null,
                                   String?                 MeterPublicKey            = null,

                                   Boolean                 DisableSendHeartbeats     = false,
                                   TimeSpan?               SendHeartbeatEvery        = null,

                                   Boolean                 DisableMaintenanceTasks   = false,
                                   TimeSpan?               MaintenanceEvery          = null,

                                   TimeSpan?               DefaultRequestTimeout     = null,
                                   Tuple<String, String>?  HTTPBasicAuth             = null,
                                   DNSClient?              DNSClient                 = null)

        {

            if (ChargeBoxId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxId),        "The given charge box identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given charging station vendor must not be null or empty!");

            if (Model.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),   "The given charging station model must not be null or empty!");


            this.ChargeBoxId              = ChargeBoxId;

            this.connectors               = new Dictionary<Connector_Id, ChargingStationConnector>();
            //for (var i = 1; i <= NumberOfConnectors; i++)
            //{
            //    this.connectors.Add(Connector_Id.Parse(i.ToString()),
            //                        new ChargePointConnector(Connector_Id.Parse(i.ToString()),
            //                                                 Availabilities.Inoperative));
            //}

            //this.Configuration = new Dictionary<String, ConfigurationData> {
            //    { "hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false) },
            //    { "changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false) },
            //    { "doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false) },
            //    { "password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false) }
            //};
            this.EnquedRequests           = new List<EnquedRequest>();

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
            this.SendHeartbeatTimer       = new Timer(DoSendHeartbeatSync,
                                                      null,
                                                      this.SendHeartbeatEvery,
                                                      this.SendHeartbeatEvery);

            this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
            this.MaintenanceEvery         = MaintenanceEvery      ?? DefaultMaintenanceEvery;
            this.MaintenanceTimer         = new Timer(DoMaintenanceSync,
                                                      null,
                                                      this.MaintenanceEvery,
                                                      this.MaintenanceEvery);

            this.HTTPBasicAuth            = HTTPBasicAuth;
            this.DNSClient                = DNSClient;

        }

        #endregion


        #region ConnectWebSocket(...)

        public async Task<HTTPResponse?> ConnectWebSocket(String                                From,
                                                          String                                To,

                                                          URL                                   RemoteURL,
                                                          HTTPHostname?                         VirtualHostname              = null,
                                                          String?                               Description                  = null,
                                                          RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                                          X509Certificate?                      ClientCert                   = null,
                                                          SslProtocols?                         TLSProtocol                  = null,
                                                          Boolean?                              PreferIPv4                   = null,
                                                          String?                               HTTPUserAgent                = null,
                                                          HTTPPath?                             URLPathPrefix                = null,
                                                          Tuple<String, String>?                HTTPBasicAuth                = null,
                                                          TimeSpan?                             RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                                          UInt16?                               MaxNumberOfRetries           = null,
                                                          Boolean                               UseHTTPPipelining            = false,

                                                          Boolean                               DisableMaintenanceTasks      = false,
                                                          TimeSpan?                             MaintenanceEvery             = null,
                                                          Boolean                               DisableWebSocketPings        = false,
                                                          TimeSpan?                             WebSocketPingEvery           = null,
                                                          TimeSpan?                             SlowNetworkSimulationDelay   = null,

                                                          String?                               LoggingPath                  = null,
                                                          String?                               LoggingContext               = null,
                                                          LogfileCreatorDelegate?               LogfileCreator               = null,
                                                          HTTPClientLogger?                     HTTPLogger                   = null,
                                                          DNSClient?                            DNSClient                    = null)

        {

            var WSClient   = new ChargingStationWSClient(
                                 ChargeBoxId,
                                 From,
                                 To,

                                 RemoteURL,
                                 VirtualHostname,
                                 Description,
                                 RemoteCertificateValidator,
                                 ClientCertificateSelector,
                                 ClientCert,
                                 HTTPUserAgent,
                                 URLPathPrefix,
                                 TLSProtocol,
                                 PreferIPv4,
                                 HTTPBasicAuth ?? this.HTTPBasicAuth,
                                 RequestTimeout,
                                 TransmissionRetryDelay,
                                 MaxNumberOfRetries,
                                 UseHTTPPipelining,

                                 DisableMaintenanceTasks,
                                 MaintenanceEvery,
                                 DisableWebSocketPings,
                                 WebSocketPingEvery,
                                 SlowNetworkSimulationDelay,

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
                    response = new ResetResponse(Request,
                                                 ResetStatus.Rejected);
                }
                else
                {
                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming '", Request.ResetType, "' reset request accepted."));
                    response = new ResetResponse(Request,
                                                 ResetStatus.Accepted);
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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid UpdateFirmware request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new UpdateFirmwareResponse(Request, UpdateFirmwareStatus.Rejected);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming UpdateFirmware request for '" + Request.Firmware.FirmwareURL + "'.");

                    response = new UpdateFirmwareResponse(Request, UpdateFirmwareStatus.Accepted);

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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid PublishFirmware request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new PublishFirmwareResponse(Request, GenericStatus.Rejected);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming PublishFirmware request for '" + Request.DownloadLocation + "'.");

                    response = new PublishFirmwareResponse(Request, GenericStatus.Accepted);

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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid UnpublishFirmware request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new UnpublishFirmwareResponse(Request, Result.GenericError());

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming UnpublishFirmware request for '" + Request.MD5Checksum + "'.");

                    response = new UnpublishFirmwareResponse(Request, UnpublishFirmwareStatus.Unpublished);

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

            // OnGetBaseReport
            // OnGetReport
            // OnGetLog
            // OnSetVariables
            // OnGetVariables
            // OnSetMonitoringBase
            // OnGetMonitoringReport
            // OnSetMonitoringLevel
            // OnSetVariableMonitoring
            // OnClearVariableMonitoring
            // OnSetNetworkProfile

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

                    //if (connectors.ContainsKey(Request.ConnectorId))
                    //{

                    //    connectors[Request.RequestId .ConnectorId].Availability = Request.Availability;

                    //    response = new ChangeAvailabilityResponse(Request,
                    //                                              ChangeAvailabilityStatus.Accepted);

                    //}
                    //else
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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid TriggerMessage request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new TriggerMessageResponse(Request,
                                                          TriggerMessageStatus.Rejected);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at EVSE '" + Request.EVSEId + "'.");

                    response = new TriggerMessageResponse(Request,
                                                          TriggerMessageStatus.Rejected);

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

                    if (Request.VendorId == "GraphDefined OEM")
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


            // OnCertificateSigned
            // OnInstallCertificate
            // OnGetInstalledCertificateIds
            // OnDeleteCertificate


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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid GetLocalListVersion request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new GetLocalListVersionResponse(Request,
                                                               0);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming GetLocalListVersion request.");

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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid SendLocalList request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new SendLocalListResponse(Request,
                                                         UpdateStatus.NotSupported);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.VersionNumber + "'.");

                    response = new SendLocalListResponse(Request,
                                                         UpdateStatus.NotSupported);

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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid ClearCache request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new ClearCacheResponse(Request,
                                                      ClearCacheStatus.Rejected);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming ClearCache request.");

                    response = new ClearCacheResponse(Request,
                                                      ClearCacheStatus.Rejected);

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

                //transactionId1 = Request.ChargingProfile?.TransactionId;

                var response = new ReserveNowResponse(Request,
                                                      ReservationStatus.Accepted);

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

                //transactionId1 = Request.ChargingProfile?.TransactionId;

                var response = new CancelReservationResponse(Request,
                                                             CancelReservationStatus.Accepted);

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

            // OnRequestStartTransaction
            // OnRequestStopTransaction
            // OnGetTransactionStatus

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


                await Task.Delay(10);


                SetChargingProfileResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid SetChargingProfile request for charge box '" + Request.ChargeBoxId + "'!");

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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming SetChargingProfile for '" + Request.EVSEId + "'.");

                    // ToDo: lock(connectors)

                    if (Request.EVSEId.ToString() == "0")
                    {
                        foreach (var conn in connectors.Values)
                        {

                            if (!Request.ChargingProfile.TransactionId.HasValue)
                                conn.ChargingProfile = Request.ChargingProfile;

                            else if (conn.TransactionId == Request.ChargingProfile.TransactionId.Value)
                                conn.ChargingProfile = Request.ChargingProfile;

                        }
                    }
                    //else if (connectors.ContainsKey(Request.EVSEId))
                    //{

                    //    connectors[Request.EVSEId].ChargingProfile = Request.ChargingProfile;

                    //    response = new SetChargingProfileResponse(Request,
                    //                                              ChargingProfileStatus.Accepted);

                    //}
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

            // OnGetChargingProfiles

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


                ClearChargingProfileResponse? response = null;



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


                GetCompositeScheduleResponse? response = null;



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


                await Task.Delay(10);


                UnlockConnectorResponse? response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid UnlockConnector request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new UnlockConnectorResponse(Request,
                                                           UnlockStatus.UnlockFailed);

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

                    // ToDo: lock(connectors)

                    if (connectors.ContainsKey(Request.ConnectorId))
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

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Invalid SetDisplayMessage request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new SetDisplayMessageResponse(Request,
                                                             Result.GenericError(""));

                }
                else
                {

                    DebugX.Log("ChargeBox[" + ChargeBoxId + "] Incoming SetDisplayMessage request.");

                    response = new SetDisplayMessageResponse(Request,
                                                             DisplayMessageStatus.Accepted);

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

            // OnGetDisplayMessages
            // OnClearDisplayMessage
            // OnCostUpdated
            // OnCustomerInformation


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

            foreach (var enquedRequest in EnquedRequests.ToArray())
            {
                if (CSClient is ChargingStationWSClient wsClient)
                {

                    var response = await wsClient.SendRequest(
                                             enquedRequest.Command,
                                             enquedRequest.Request.RequestId,
                                             enquedRequest.RequestJSON
                                         );

                    enquedRequest.ResponseAction(response);

                    EnquedRequests.Remove(enquedRequest);

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


        #region SendBootNotification             (...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.BootNotificationResponse>

            SendBootNotification(BootReasons         BootReason,
                                 CustomData?         CustomData          = null,

                                 DateTime?           RequestTimestamp    = null,
                                 TimeSpan?           RequestTimeout      = null,
                                 EventTracking_Id?   EventTrackingId     = null,
                                 CancellationToken?  CancellationToken   = null)

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

                                 NextRequestId,
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

        #region SendFirmwareStatusNotification   (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatus      Status,
                                           CustomData?         CustomData          = null,

                                           DateTime?           RequestTimestamp    = null,
                                           TimeSpan?           RequestTimeout      = null,
                                           EventTracking_Id?   EventTrackingId     = null,
                                           CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new FirmwareStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,
                                 0,
                                 CustomData,

                                 NextRequestId,
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

        // PublishFirmwareStatusNotification

        #region SendHeartbeat                    (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.HeartbeatResponse>

            SendHeartbeat(CustomData?         CustomData          = null,

                          DateTime?           RequestTimestamp    = null,
                          TimeSpan?           RequestTimeout      = null,
                          EventTracking_Id?   EventTrackingId     = null,
                          CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new HeartbeatRequest(
                                 ChargeBoxId,
                                 CustomData,

                                 NextRequestId,
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

        // NotifyEvent
        // SendSecurityEventNotification
        // NotifyReport
        // NotifyMonitoringReport
        // SendLogStatusNotification

        #region TransferData                     (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.DataTransferResponse>

            TransferData(String              VendorId,
                         String?             MessageId           = null,
                         JToken?             Data                = null,
                         CustomData?         CustomData          = null,

                         DateTime?           RequestTimestamp    = null,
                         TimeSpan?           RequestTimeout      = null,
                         EventTracking_Id?   EventTrackingId     = null,
                         CancellationToken?  CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new DataTransferRequest(
                                 ChargeBoxId,
                                 VendorId,
                                 MessageId,
                                 Data,
                                 CustomData,

                                 NextRequestId,
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

        // SignCertificate
        // Get15118EVCertificate
        // GetCertificateStatus

        // ReservationStatusUpdate

        #region Authorize                        (IdToken, Certificate = null, ISO15118CertificateHashData = null, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.AuthorizeResponse>

            Authorize(IdToken                        IdToken,
                      Certificate?                   Certificate                   = null,
                      IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,
                      CustomData?                    CustomData                    = null,

                      DateTime?                      RequestTimestamp              = null,
                      TimeSpan?                      RequestTimeout                = null,
                      EventTracking_Id?              EventTrackingId               = null,
                      CancellationToken?             CancellationToken             = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new AuthorizeRequest(
                                 ChargeBoxId,
                                 IdToken,
                                 Certificate,
                                 ISO15118CertificateHashData,
                                 CustomData,

                                 NextRequestId,
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

        // NotifyEVChargingNeeds

        #region SendTransactionEvent             (EventType, Timestamp, TriggerReason, SequenceNumber, TransactionInfo, ...)

        /// <summary>
        /// TransactionEvent the given token.
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
        /// 
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

                                 DateTime?                 RequestTimestamp     = null,
                                 TimeSpan?                 RequestTimeout       = null,
                                 EventTracking_Id?         EventTrackingId      = null,
                                 CancellationToken?        CancellationToken    = null)

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

                                 NextRequestId,
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

        #region SendStatusNotification           (EVSEId, ConnectorId, Timestamp, Status, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="Status">The current status of the connector.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.StatusNotificationResponse>

            SendStatusNotification(EVSE_Id             EVSEId,
                                   Connector_Id        ConnectorId,
                                   DateTime            Timestamp,
                                   ConnectorStatus     Status,
                                   CustomData?         CustomData          = null,

                                   DateTime?           RequestTimestamp    = null,
                                   TimeSpan?           RequestTimeout      = null,
                                   EventTracking_Id?   EventTrackingId     = null,
                                   CancellationToken?  CancellationToken   = null)

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

                                 NextRequestId,
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

        #region SendMeterValues                  (EVSEId, MeterValues, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="EVSEId">The EVSE identification at the charging station.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CSMS.MeterValuesResponse>

            SendMeterValues(EVSE_Id                  EVSEId, // 0 => main power meter; 1 => first EVSE
                            IEnumerable<MeterValue>  MeterValues,
                            CustomData?              CustomData          = null,

                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken?       CancellationToken   = null)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new MeterValuesRequest(
                                 ChargeBoxId,
                                 EVSEId,
                                 MeterValues,
                                 CustomData,

                                 NextRequestId,
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

        // NotifyChargingLimit
        // SendClearedChargingLimit
        // ReportChargingProfiles

        // NotifyDisplayMessages
        // NotifyCustomerInformation


    }

}
