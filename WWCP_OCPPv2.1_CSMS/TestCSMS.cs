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

using System.Reflection;
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCSMS : ICSMSService,
                            IEventSender
    {

        #region Data

        private          readonly  HashSet<SignaturePolicy>                                                  signaturePolicies           = new();

        private          readonly  HashSet<ICSMSChannel>                                                     centralSystemServers        = new();

        private          readonly  ConcurrentDictionary<ChargingStation_Id, Tuple<ICSMSChannel, DateTime>>   reachableChargingStations   = new();

        private          readonly  HTTPExtAPI                                                                TestAPI;

        private          readonly  CSMSWebAPI                                                                WebAPI;

        protected static readonly  SemaphoreSlim                                                             ChargingStationSemaphore    = new (1, 1);

        protected static readonly  TimeSpan                                                                  SemaphoreSlimTimeout        = TimeSpan.FromSeconds(5);

        public    static readonly  IPPort                                                                    DefaultHTTPUploadPort       = IPPort.Parse(9901);

        private                    Int64                                                                     internalRequestId           = 900000;

        private                    TimeSpan                                                                  defaultRequestTimeout       = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public CSMS_Id    Id                        { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => this.Id.ToString();


        public UploadAPI  HTTPUploadAPI             { get; }

        public IPPort     HTTPUploadPort            { get; }

        public DNSClient  DNSClient                 { get; }

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean    RequireAuthentication     { get; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan   DefaultRequestTimeout     { get; }


        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<ICSMSServer> CSMSServers
            => centralSystemServers;


        public IEnumerable<ICSMSChannel> CSMSChannels
            => centralSystemServers;

        /// <summary>
        /// The unique identifications of all connected or reachable charging stations.
        /// </summary>
        public IEnumerable<ChargingStation_Id> ChargingStationIds
            => reachableChargingStations.Values.SelectMany(csmsChannel => csmsChannel.Item1.ChargingStationIds);


        public Dictionary<String, Transaction_Id> TransactionIds = new ();

        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               SignaturePolicy
            => SignaturePolicies.First();

        #endregion

        #region Events

        #region WebSocket connections

        /// <summary>
        /// An event sent whenever the HTTP web socket server started.
        /// </summary>
        public event OnServerStartedDelegate?                 OnServerStarted;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnValidateTCPConnectionDelegate?         OnValidateTCPConnection;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnNewTCPConnectionDelegate?              OnNewTCPConnection;

        /// <summary>
        /// An event sent whenever a HTTP request was received.
        /// </summary>
        public event HTTPRequestLogDelegate?                  OnHTTPRequest;

        /// <summary>
        /// An event sent whenever the HTTP headers of a new web socket connection
        /// need to be validated or filtered by an upper layer application logic.
        /// </summary>
        public event OnValidateWebSocketConnectionDelegate?   OnValidateWebSocketConnection;

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnNewWebSocketConnectionDelegate?        OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCloseMessageDelegate?                  OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnTCPConnectionClosedDelegate?           OnTCPConnectionClosed;

        #endregion


        #region Generic Text Messages

        /// <summary>
        /// An event sent whenever a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a text message was sent.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a text message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion


        #region CSMS <- Charging Station Messages

        #region OnBootNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request was sent from a charging station.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat (-Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request was sent from a charging station.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion

        #region OnNotifyEvent (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEvent request was sent from a charging station.
        /// </summary>
        public event OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request was sent from a charging station.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReport (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyReport request was sent from a charging station.
        /// </summary>
        public event OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReport (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request was sent from a charging station.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region OnIncomingDataTransfer (-Request/-Response)

        /// <summary>
        /// An event sent whenever an IncomingDataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingDataTransfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion


        #region OnSignCertificate (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request was sent from a charging station.
        /// </summary>
        public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificate (-Request/-Response)

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request was sent from a charging station.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request was sent from a charging station.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCRL request was sent from a charging station.
        /// </summary>
        public event OnGetCRLRequestDelegate?   OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OnGetCRLResponseDelegate?  OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdate (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request was sent from a charging station.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorize (-Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request was sent from a charging station.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeeds (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request was sent from a charging station.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEvent (-Request/-Response)

        /// <summary>
        /// An event fired whenever a TransactionEvent was sent from a charging station.
        /// </summary>
        public event OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification (-Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request was sent from a charging station.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues (-Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request was sent from a charging station.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request was sent from a charging station.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimit (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request was sent from a charging station.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request was sent from a charging station.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request was sent from a charging station.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request was sent from a charging station.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate (-Request/-Response)

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request was sent from a charging station.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessages (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request was sent from a charging station.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformation (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request was sent from a charging station.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer (-Request/-Response)

        /// <summary>
        /// An event sent whenever an IncomingBinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferRequestDelegate?   OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        /// </summary>
        public event OnIncomingBinaryDataTransferResponseDelegate?  OnIncomingBinaryDataTransferResponse;

        #endregion


        #endregion

        #region CSMS -> Charging Station Messages

        #region OnReset                       (-Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the charging station.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnUpdateFirmware              (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion

        #region OnPublishFirmware             (-Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        #endregion

        #region OnUnpublishFirmware           (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        #endregion

        #region OnGetBaseReport               (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the charging station.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        #endregion

        #region OnGetReport                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the charging station.
        /// </summary>
        public event OnGetReportRequestDelegate?   OnGetReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OnGetReportResponseDelegate?  OnGetReportResponse;

        #endregion

        #region OnGetLog                      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the charging station.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region OnSetVariables                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the charging station.
        /// </summary>
        public event OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        #endregion

        #region OnGetVariables                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the charging station.
        /// </summary>
        public event OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        #endregion

        #region OnSetMonitoringBase           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        #endregion

        #region OnGetMonitoringReport         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        #endregion

        #region OnSetMonitoringLevel          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        #endregion

        #region SetVariableMonitoring         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        #endregion

        #region OnClearVariableMonitoring     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        #endregion

        #region OnSetNetworkProfile           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        #endregion

        #region OnChangeAvailability          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnTriggerMessage              (-Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the charging station.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnDataTransfer                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DataTransfer request will be sent to the charging station.
        /// </summary>
        public event CSMS.OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was received.
        /// </summary>
        public event CSMS.OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        #region OnCertificateSigned           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the charging station.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region OnInstallCertificate          (-Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the charging station.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnDeleteCertificate           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region OnNotifyCRL                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the charging station.
        /// </summary>
        public event OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        #endregion


        #region OnGetLocalListVersion         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList               (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the charging station.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region OnClearCache                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the charging station.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        #region OnReserveNow                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the charging station.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservation           (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the charging station.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnRequestStartTransaction     (-Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        #endregion

        #region OnRequestStopTransaction      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        #endregion

        #region OnGetTransactionStatus        (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        #endregion

        #region OnSetChargingProfile          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnGetChargingProfiles         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        #endregion

        #region OnClearChargingProfile        (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule        (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnUpdateDynamicSchedule       (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to the charging station.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer (-Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region OnUsePriorityCharging         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        #endregion

        #region OnUnlockConnector             (-Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the charging station.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region OnAFRRSignal                  (-Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRRSignal request will be sent to the charging station.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        /// <summary>
        /// An event fired whenever a response to an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        #endregion


        #region SetDisplayMessage/-Response   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        #endregion

        #region OnGetDisplayMessages          (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        #endregion

        #region OnClearDisplayMessage         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        #endregion

        #region OnCostUpdated                 (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the charging station.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        #endregion

        #region OnCustomerInformation         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the charging station.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        #endregion


        // Binary Data Streams Extensions

        #region OnBinaryDataTransfer          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request will be sent to the charging station.
        /// </summary>
        public event CSMS.OnBinaryDataTransferRequestDelegate?   OnBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event CSMS.OnBinaryDataTransferResponseDelegate?  OnBinaryDataTransferResponse;

        #endregion

        #region OnGetFile                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetFile request will be sent to the charging station.
        /// </summary>
        public event CSMS.OnGetFileRequestDelegate?   OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was received.
        /// </summary>
        public event CSMS.OnGetFileResponseDelegate?  OnGetFileResponse;

        #endregion

        #region OnSendFile                    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SendFile request will be sent to the charging station.
        /// </summary>
        public event CSMS.OnSendFileRequestDelegate?   OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was received.
        /// </summary>
        public event CSMS.OnSendFileResponseDelegate?  OnSendFileResponse;

        #endregion

        #region OnDeleteFile                    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteFile request will be sent to the charging station.
        /// </summary>
        public event CSMS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was received.
        /// </summary>
        public event CSMS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy            (-Request/-Response)

        /// <summary>
        /// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a AddSignaturePolicy request was received.
        /// </summary>
        public event OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        #endregion

        #region UpdateSignaturePolicy         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        #endregion

        #region DeleteSignaturePolicy         (-Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        /// </summary>
        public event OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        #endregion

        #region AddUserRole                   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a AddUserRole request will be sent to the charging station.
        /// </summary>
        public event OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a AddUserRole request was received.
        /// </summary>
        public event OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        #endregion

        #region UpdateUserRole                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateUserRole request will be sent to the charging station.
        /// </summary>
        public event OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateUserRole request was received.
        /// </summary>
        public event OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        #endregion

        #region DeleteUserRole                (-Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteUserRole request will be sent to the charging station.
        /// </summary>
        public event OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        #endregion


        // E2E Charging Tariff Extensions

        #region SetDefaultChargingTariff      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

        #endregion

        #region GetDefaultChargingTariff      (-Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDefaultChargingTariff request was received.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

        #endregion

        #region RemoveDefaultChargingTariff   (-Request/-Response)

        /// <summary>
        /// An event fired whenever a RemoveDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a RemoveDefaultChargingTariff request was received.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

        #endregion

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region CSMS Request  Messages

        public CustomJObjectSerializerDelegate<ResetRequest>?                                        CustomResetRequestSerializer                                 { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                               CustomUpdateFirmwareRequestSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<PublishFirmwareRequest>?                              CustomPublishFirmwareRequestSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<UnpublishFirmwareRequest>?                            CustomUnpublishFirmwareRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<GetBaseReportRequest>?                                CustomGetBaseReportRequestSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<GetReportRequest>?                                    CustomGetReportRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<GetLogRequest>?                                       CustomGetLogRequestSerializer                                { get; set; }

        public CustomJObjectSerializerDelegate<SetVariablesRequest>?                                 CustomSetVariablesRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<GetVariablesRequest>?                                 CustomGetVariablesRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<SetMonitoringBaseRequest>?                            CustomSetMonitoringBaseRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?                          CustomGetMonitoringReportRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?                           CustomSetMonitoringLevelRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?                        CustomSetVariableMonitoringRequestSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<ClearVariableMonitoringRequest>?                      CustomClearVariableMonitoringRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?                            CustomSetNetworkProfileRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?                           CustomChangeAvailabilityRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                               CustomTriggerMessageRequestSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }


        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?                            CustomCertificateSignedRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?                           CustomInstallCertificateRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?                   CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?                            CustomDeleteCertificateRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<NotifyCRLRequest>?                                    CustomNotifyCRLRequestSerializer                             { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?                          CustomGetLocalListVersionRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                                CustomSendLocalListRequestSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                                   CustomClearCacheRequestSerializer                            { get; set; }


        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                                   CustomReserveNowRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>?                            CustomCancelReservationRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?                      CustomRequestStartTransactionRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<RequestStopTransactionRequest>?                       CustomRequestStopTransactionRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?                         CustomGetTransactionStatusRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?                           CustomSetChargingProfileRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?                          CustomGetChargingProfilesRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?                         CustomClearChargingProfileRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?                         CustomGetCompositeScheduleRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?                        CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?                  CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?                          CustomUsePriorityChargingRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?                              CustomUnlockConnectorRequestSerializer                       { get; set; }


        public CustomJObjectSerializerDelegate<AFRRSignalRequest>?                                   CustomAFRRSignalRequestSerializer                            { get; set; }


        public CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?                            CustomSetDisplayMessageRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?                           CustomGetDisplayMessagesRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?                          CustomClearDisplayMessageRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<CostUpdatedRequest>?                                  CustomCostUpdatedRequestSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CustomerInformationRequest>?                          CustomCustomerInformationRequestSerializer                   { get; set; }


        // Binary Data Streams Extensions

        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetFileRequest>?                                      CustomGetFileRequestSerializer                               { get; set; }
        public CustomBinarySerializerDelegate <SendFileRequest>?                                     CustomSendFileRequestSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileRequest>?                                   CustomDeleteFileRequestSerializer                            { get; set; }


        // E2E Security Extensions

        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }


        // E2E Charging Tariffs
        public CustomJObjectSerializerDelegate<SetDefaultChargingTariffRequest>?                     CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<GetDefaultChargingTariffRequest>?                     CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffRequest>?                  CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

        #endregion

        #region CSMS Response Messages

        public CustomJObjectSerializerDelegate<BootNotificationResponse>?                            CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationResponse>?                  CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationResponse>?           CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<HeartbeatResponse>?                                   CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEventResponse>?                                 CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SecurityEventNotificationResponse>?                   CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyReportResponse>?                                CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<NotifyMonitoringReportResponse>?                      CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<LogStatusNotificationResponse>?                       CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomIncomingDataTransferResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<SignCertificateResponse>?                             CustomSignCertificateResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?                       CustomGet15118EVCertificateResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<GetCertificateStatusResponse>?                        CustomGetCertificateStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<GetCRLResponse>?                                      CustomGetCRLResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateResponse>?                     CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizeResponse>?                                   CustomAuthorizeResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsResponse>?                       CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TransactionEventResponse>?                            CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<StatusNotificationResponse>?                          CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<MeterValuesResponse>?                                 CustomMeterValuesResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?                         CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearedChargingLimitResponse>?                        CustomClearedChargingLimitResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<ReportChargingProfilesResponse>?                      CustomReportChargingProfilesResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?                    CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?                      CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateResponse>?                   CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesResponse>?                       CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCustomerInformationResponse>?                   CustomNotifyCustomerInformationResponseSerializer            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?                           CustomIncomingBinaryDataTransferResponseSerializer           { get; set; }

        #endregion


        #region Charging Station Request  Messages

        public CustomJObjectSerializerDelegate<CS.BootNotificationRequest>?                          CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.FirmwareStatusNotificationRequest>?                CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CS.PublishFirmwareStatusNotificationRequest>?         CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<CS.HeartbeatRequest>?                                 CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEventRequest>?                               CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.SecurityEventNotificationRequest>?                 CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyReportRequest>?                              CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyMonitoringReportRequest>?                    CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.LogStatusNotificationRequest>?                     CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.DataTransferRequest>?                              CustomIncomingDataTransferRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<CS.SignCertificateRequest>?                           CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.Get15118EVCertificateRequest>?                     CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCertificateStatusRequest>?                      CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCRLRequest>?                                    CustomGetCRLRequestSerializer                                { get; set; }

        public CustomJObjectSerializerDelegate<CS.ReservationStatusUpdateRequest>?                   CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.AuthorizeRequest>?                                 CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEVChargingNeedsRequest>?                     CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.TransactionEventRequest>?                          CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.StatusNotificationRequest>?                        CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.MeterValuesRequest>?                               CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyChargingLimitRequest>?                       CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearedChargingLimitRequest>?                      CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ReportChargingProfilesRequest>?                    CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyEVChargingScheduleRequest>?                  CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyPriorityChargingRequest>?                    CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.PullDynamicScheduleUpdateRequest>?                 CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<CS.NotifyDisplayMessagesRequest>?                     CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyCustomerInformationRequest>?                 CustomNotifyCustomerInformationRequestSerializer             { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <CS.BinaryDataTransferRequest>?                        CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }

        #endregion

        #region Charging Station Response Messages

        public CustomJObjectSerializerDelegate<CS.ResetResponse>?                                    CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateFirmwareResponse>?                           CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.PublishFirmwareResponse>?                          CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnpublishFirmwareResponse>?                        CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetBaseReportResponse>?                            CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetReportResponse>?                                CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetLogResponse>?                                   CustomGetLogResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<CS.SetVariablesResponse>?                             CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetVariablesResponse>?                             CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringBaseResponse>?                        CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetMonitoringReportResponse>?                      CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetMonitoringLevelResponse>?                       CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetVariableMonitoringResponse>?                    CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearVariableMonitoringResponse>?                  CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetNetworkProfileResponse>?                        CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.ChangeAvailabilityResponse>?                       CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.TriggerMessageResponse>?                           CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CS.DataTransferResponse>?                             CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<CS.CertificateSignedResponse>?                        CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.InstallCertificateResponse>?                       CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetInstalledCertificateIdsResponse>?               CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CS.DeleteCertificateResponse>?                        CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyCRLResponse>?                                CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<CS.GetLocalListVersionResponse>?                      CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.SendLocalListResponse>?                            CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearCacheResponse>?                               CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.ReserveNowResponse>?                               CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CS.CancelReservationResponse>?                        CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStartTransactionResponse>?                  CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CS.RequestStopTransactionResponse>?                   CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetTransactionStatusResponse>?                     CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.SetChargingProfileResponse>?                       CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetChargingProfilesResponse>?                      CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearChargingProfileResponse>?                     CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetCompositeScheduleResponse>?                     CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CS.UpdateDynamicScheduleResponse>?                    CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CS.NotifyAllowedEnergyTransferResponse>?              CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<CS.UsePriorityChargingResponse>?                      CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.UnlockConnectorResponse>?                          CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CS.AFRRSignalResponse>?                               CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<CS.SetDisplayMessageResponse>?                        CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDisplayMessagesResponse>?                       CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CS.ClearDisplayMessageResponse>?                      CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CS.CostUpdatedResponse>?                              CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.CustomerInformationResponse>?                      CustomCustomerInformationResponseSerializer                  { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <CS.BinaryDataTransferResponse>?                       CustomBinaryDataTransferResponseSerializer                   { get; set; }
        public CustomBinarySerializerDelegate <CS.GetFileResponse>?                                  CustomGetFileResponseSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CS.SendFileResponse>?                                 CustomSendFileResponseSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CS.DeleteFileResponse>?                               CustomDeleteFileResponseSerializer                           { get; set; }


        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<CS.SetDefaultChargingTariffResponse>?                 CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDefaultChargingTariffResponse>?                 CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.RemoveDefaultChargingTariffResponse>?              CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region Data Structures

        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                   { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                { get; set; }

        public CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                            { get; set; }


        public CustomJObjectSerializerDelegate<ChargingStation>?                                     CustomChargingStationSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                        { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                         { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<Signature>?                                            CustomBinarySignatureSerializer                        { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                    { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        public TestCSMS(CSMS_Id           Id,
                        Boolean           RequireAuthentication   = true,
                        TimeSpan?         DefaultRequestTimeout   = null,
                        IPPort?           HTTPUploadPort          = null,
                        DNSClient?        DNSClient               = null,

                        SignaturePolicy?  SignaturePolicy         = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id), "The given central system identification must not be null or empty!");

            this.Id                      = Id;
            this.RequireAuthentication   = RequireAuthentication;
            this.DefaultRequestTimeout   = DefaultRequestTimeout ?? defaultRequestTimeout;
            this.HTTPUploadPort          = HTTPUploadPort        ?? DefaultHTTPUploadPort;

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            this.TestAPI                 = new HTTPExtAPI(
                                               HTTPServerPort:         IPPort.Parse(3502),
                                               HTTPServerName:         "GraphDefined OCPP Test Central System",
                                               HTTPServiceName:        "GraphDefined OCPP Test Central System Service",
                                               APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                               APIRobotGPGPassphrase:  "test123",
                                               SMTPClient:             new NullMailer(),
                                               DNSClient:              DNSClient,
                                               AutoStart:              true
                                           );

            this.TestAPI.HTTPServer.AddAuth(request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(TestAPI.URLPathPrefix + "/webapi"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            this.HTTPUploadAPI           = new UploadAPI(
                                               this,
                                               new HTTPServer(
                                                   this.HTTPUploadPort,
                                                   "Open Charging Cloud OCPP Upload Server",
                                                   "Open Charging Cloud OCPP Upload Service"
                                               )
                                           );

            this.WebAPI                  = new CSMSWebAPI(
                                               TestAPI
                                           );
            this.WebAPI.AttachCSMS(this);

            this.DNSClient               = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

        }

        #endregion


        public static void ShowAllRequests()
        {

            var interfaceType      = typeof(IRequest);
            var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                              GetTypes().
                                              Where(t => interfaceType.IsAssignableFrom(t) &&
                                                         !t.IsInterface &&
                                                          t.FullName is not null &&
                                                          t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CSMS.")).
                                              ToArray() ?? [];

            foreach (var type in implementingTypes)
            {

                var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

            }

        }

        public static void ShowAllResponses()
        {

            var interfaceType      = typeof(IResponse);
            var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                              GetTypes().
                                              Where(t => interfaceType.IsAssignableFrom(t) &&
                                                         !t.IsInterface &&
                                                          t.FullName is not null &&
                                                          t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CS.")).
                                              ToArray() ?? [];

            foreach (var type in implementingTypes)
            {

                var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

            }

        }


        #region CreateWebSocketService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSMSWSServer CreateWebSocketService(String       HTTPServerName               = CSMSWSServer.DefaultHTTPServiceName,
                                                   IIPAddress?  IPAddress                    = null,
                                                   IPPort?      TCPPort                      = null,

                                                   Boolean      DisableWebSocketPings        = false,
                                                   TimeSpan?    WebSocketPingEvery           = null,
                                                   TimeSpan?    SlowNetworkSimulationDelay   = null,

                                                   DNSClient?   DNSClient                    = null,
                                                   Boolean      AutoStart                    = false)
        {

            var centralSystemServer = new CSMSWSServer(
                                          HTTPServerName,
                                          IPAddress,
                                          TCPPort,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DNSClient: DNSClient ?? this.DNSClient,
                                          AutoStart: false
                                      );

            AttachCSMSChannel(centralSystemServer);

            if (AutoStart)
                centralSystemServer.Start();

            return centralSystemServer;

        }

        #endregion

        #region (private) AttachCSMSChannel(CSMSChannel)

        private void AttachCSMSChannel(ICSMSChannel CSMSChannel)
        {


            centralSystemServers.Add(CSMSChannel);


            if (CSMSChannel is CSMSWSServer centralSystemWSServer)
            {
                centralSystemWSServer.OnNewCSMSWSConnection += async (LogTimestamp,
                                                                      CSMS,
                                                                      Connection,
                                                                      EventTrackingId,
                                                                      CancellationToken) =>
                {

                    if (Connection.TryGetCustomDataAs(CSMSWSServer.chargingStationId_WebSocketKey, out ChargingStation_Id chargingStationId))
                    {
                        if (!reachableChargingStations.ContainsKey(chargingStationId))
                            reachableChargingStations.TryAdd(chargingStationId, new Tuple<ICSMSChannel, DateTime>(CSMS, Timestamp.Now));
                        else
                            reachableChargingStations[chargingStationId] = new Tuple<ICSMSChannel, DateTime>(CSMS, Timestamp.Now);
                    }

                };
            }


            #region WebSocket related

            #region OnServerStarted

            CSMSChannel.OnServerStarted += (Timestamp,
                                            server,
                                            eventTrackingId) => {

                DebugX.Log($"OCPP {Version.String} web socket server has started on {server.IPSocket}!");
                return Task.CompletedTask;

            };

            #endregion

            #region OnNewTCPConnection

            CSMSChannel.OnNewTCPConnection += async (timestamp,
                                                     webSocketServer,
                                                     newWebSocketConnection,
                                                     eventTrackingId,
                                                     cancellationToken) => {

                var logger = OnNewTCPConnection;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnNewTCPConnectionDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               newWebSocketConnection,
                                                                                               eventTrackingId,
                                                                                               cancellationToken)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNewTCPConnection),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnNewWebSocketConnection

            CSMSChannel.OnNewWebSocketConnection += async (timestamp,
                                                           webSocketServer,
                                                           newWebSocketConnection,
                                                           eventTrackingId,
                                                           cancellationToken) => {

                var logger = OnNewWebSocketConnection;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnNewWebSocketConnectionDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               newWebSocketConnection,
                                                                                               eventTrackingId,
                                                                                               cancellationToken)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNewWebSocketConnection),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnCloseMessageReceived

            CSMSChannel.OnCloseMessageReceived += async (timestamp,
                                                         server,
                                                         connection,
                                                         eventTrackingId,
                                                         statusCode,
                                                         reason) => {

                var logger = OnCloseMessageReceived;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnCloseMessageDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               server,
                                                                                               connection,
                                                                                               eventTrackingId,
                                                                                               statusCode,
                                                                                               reason)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnCloseMessageReceived),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnTCPConnectionClosed

            CSMSChannel.OnTCPConnectionClosed += async (timestamp,
                                                        server,
                                                        connection,
                                                        reason,
                                                        eventTrackingId) => {

                var logger = OnTCPConnectionClosed;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnTCPConnectionClosedDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               server,
                                                                                               connection,
                                                                                               reason,
                                                                                               eventTrackingId)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnTCPConnectionClosed),
                                  e
                              );
                    }

                }

            };

            #endregion

            // Failed (Charging Station) Authentication

            // (Generic) Error Handling

            #endregion


            #region OnTextMessageRequestReceived

            CSMSChannel.OnJSONMessageRequestReceived += async (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               eventTrackingId,
                                                               requestTimestamp,
                                                               requestMessage) => {

                var logger = OnJSONMessageRequestReceived;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketJSONMessageRequestDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestTimestamp,
                                                                                               requestMessage)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONMessageRequestReceived),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnTextMessageResponseSent

            CSMSChannel.OnJSONMessageResponseSent += async (timestamp,
                                                            webSocketServer,
                                                            webSocketConnection,
                                                            eventTrackingId,
                                                            requestTimestamp,
                                                            jsonRequestMessage,
                                                            binaryRequestMessage,
                                                            responseTimestamp,
                                                            responseMessage) => {


                var logger = OnJSONMessageResponseSent;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketJSONMessageResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestTimestamp,
                                                                                               jsonRequestMessage,
                                                                                               binaryRequestMessage,
                                                                                               responseTimestamp,
                                                                                               responseMessage)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONMessageResponseSent),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnTextErrorResponseSent

            CSMSChannel.OnJSONErrorResponseSent += async (timestamp,
                                                          webSocketServer,
                                                          webSocketConnection,
                                                          eventTrackingId,
                                                          requestTimestamp,
                                                          jsonRequestMessage,
                                                          binaryRequestMessage,
                                                          responseTimestamp,
                                                          responseMessage) => {

                var logger = OnJSONErrorResponseSent;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketTextErrorResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestTimestamp,
                                                                                               jsonRequestMessage,
                                                                                               binaryRequestMessage,
                                                                                               responseTimestamp,
                                                                                               responseMessage)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONErrorResponseSent),
                                  e
                              );
                    }

                }

            };

            #endregion


            #region OnTextMessageRequestSent

            CSMSChannel.OnJSONMessageRequestSent += async (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           requestMessage) => {


                var logger = OnJSONMessageRequestSent;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketTextMessageDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestMessage.ToString())).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONMessageRequestSent),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnTextMessageResponseReceived

            CSMSChannel.OnJSONMessageResponseReceived += async (timestamp,
                                                                webSocketServer,
                                                                webSocketConnection,
                                                                eventTrackingId,
                                                                requestTimestamp,
                                                                jsonRequestMessage,
                                                                binaryRequestMessage,
                                                                responseTimestamp,
                                                                responseMessage) => {


                var logger = OnJSONMessageResponseReceived;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketJSONMessageResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestTimestamp,
                                                                                               jsonRequestMessage,
                                                                                               binaryRequestMessage,
                                                                                               responseTimestamp,
                                                                                               responseMessage)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONMessageResponseReceived),
                                  e
                              );
                    }

                }

            };

            #endregion

            #region OnTextErrorResponseReceived

            CSMSChannel.OnJSONErrorResponseReceived += async (timestamp,
                                                              webSocketServer,
                                                              webSocketConnection,
                                                              eventTrackingId,
                                                              requestTimestamp,
                                                              jsonRequestMessage,
                                                              binaryRequestMessage,
                                                              responseTimestamp,
                                                              responseMessage) => {

                var logger = OnJSONErrorResponseReceived;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketTextErrorResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                               webSocketServer,
                                                                                               webSocketConnection,
                                                                                               eventTrackingId,
                                                                                               requestTimestamp,
                                                                                               jsonRequestMessage,
                                                                                               binaryRequestMessage,
                                                                                               responseTimestamp,
                                                                                               responseMessage)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnJSONErrorResponseReceived),
                                  e
                              );
                    }

                }

            };

            #endregion


            #region Incoming OCPP messages and their responses...

            #region OnBootNotification

            CSMSChannel.OnBootNotification += async (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                #region Send OnBootNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnBootNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnBootNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             connection,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnBootNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingStation
                // Reason

                DebugX.Log($"OnBootNotification: {request.ChargingStation?.SerialNumber ?? "-"} ({request.ChargingStationId})");


                //await AddChargeBoxIfNotExists(new ChargeBox(Request.ChargeBoxId,
                //                                            1,
                //                                            Request.ChargePointVendor,
                //                                            Request.ChargePointModel,
                //                                            null,
                //                                            Request.ChargePointSerialNumber,
                //                                            Request.ChargeBoxSerialNumber,
                //                                            Request.FirmwareVersion,
                //                                            Request.Iccid,
                //                                            Request.IMSI,
                //                                            Request.MeterType,
                //                                            Request.MeterSerialNumber));

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomBootNotificationRequestSerializer,
                                       CustomChargingStationSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new BootNotificationResponse(
                                         Request:       request,
                                         Result:        Result.SignatureError(
                                                            $"Invalid signature(s): {errorResponse}"
                                                        )
                                     )

                                   : new BootNotificationResponse(
                                         Request:       request,
                                         Status:        RegistrationStatus.Accepted,
                                         CurrentTime:   Timestamp.Now,
                                         Interval:      TimeSpan.FromMinutes(5),
                                         StatusInfo:    null,
                                         CustomData:    null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomBootNotificationResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnBootNotificationResponse event

                var responseLogger = OnBootNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnBootNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                connection,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnBootNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnFirmwareStatusNotification

            CSMSChannel.OnFirmwareStatusNotification += async (timestamp,
                                                               sender,
                                                               request,
                                                               cancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnFirmwareStatusNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnFirmwareStatusNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnFirmwareStatusNotificationRequest),
                                  e
                              );
                    }

                }

                                                                  #endregion

                // Status
                // UpdateFirmwareRequestId

                DebugX.Log("OnFirmwareStatus: " + request.Status);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomFirmwareStatusNotificationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new FirmwareStatusNotificationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new FirmwareStatusNotificationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomFirmwareStatusNotificationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnFirmwareStatusNotificationResponse event

                var responseLogger = OnFirmwareStatusNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnFirmwareStatusNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnFirmwareStatusNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnPublishFirmwareStatusNotification

            CSMSChannel.OnPublishFirmwareStatusNotification += async (timestamp,
                                                                      sender,
                                                                      request,
                                                                      cancellationToken) => {

                #region Send OnPublishFirmwareStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPublishFirmwareStatusNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPublishFirmwareStatusNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnPublishFirmwareStatusNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Status
                // PublishFirmwareStatusNotificationRequestId
                // DownloadLocations

                DebugX.Log("OnPublishFirmwareStatusNotification: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomPublishFirmwareStatusNotificationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new PublishFirmwareStatusNotificationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new PublishFirmwareStatusNotificationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomPublishFirmwareStatusNotificationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnPublishFirmwareStatusNotificationResponse event

                var responseLogger = OnPublishFirmwareStatusNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPublishFirmwareStatusNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnPublishFirmwareStatusNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnHeartbeat

            CSMSChannel.OnHeartbeat += async (timestamp,
                                              sender,
                                              request,
                                              cancellationToken) => {

                #region Send OnHeartbeatRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnHeartbeatRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnHeartbeatRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnHeartbeatRequest),
                                  e
                              );
                    }

                }

                #endregion


                DebugX.Log("OnHeartbeat: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomHeartbeatRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new HeartbeatResponse(
                                         Request:       request,
                                         Result:        Result.SignatureError(
                                                            $"Invalid signature(s): {errorResponse}"
                                                        )
                                     )

                                   : new HeartbeatResponse(
                                         Request:       request,
                                         CurrentTime:   Timestamp.Now,
                                         CustomData:    null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomHeartbeatResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnHeartbeatResponse event

                var responseLogger = OnHeartbeatResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnHeartbeatResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnHeartbeatResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyEvent

            CSMSChannel.OnNotifyEvent += async (timestamp,
                                                sender,
                                                request,
                                                cancellationToken) => {

                #region Send OnNotifyEventRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEventRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEventRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEventRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GeneratedAt
                // SequenceNumber
                // EventData
                // ToBeContinued

                DebugX.Log("OnNotifyEvent: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyEventRequestSerializer,
                                       CustomEventDataSerializer,
                                       CustomComponentSerializer,
                                       CustomEVSESerializer,
                                       CustomVariableSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyEventResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyEventResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyEventResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyEventResponse event

                var responseLogger = OnNotifyEventResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEventResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEventResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnSecurityEventNotification

            CSMSChannel.OnSecurityEventNotification += async (timestamp,
                                                              sender,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnSecurityEventNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSecurityEventNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSecurityEventNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSecurityEventNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Type
                // Timestamp
                // TechInfo

                DebugX.Log("OnSecurityEventNotification: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomSecurityEventNotificationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new SecurityEventNotificationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new SecurityEventNotificationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomSecurityEventNotificationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnSecurityEventNotificationResponse event

                var responseLogger = OnSecurityEventNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSecurityEventNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnSecurityEventNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyReport

            CSMSChannel.OnNotifyReport += async (timestamp,
                                                 sender,
                                                 request,
                                                 cancellationToken) => {

                #region Send OnNotifyReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyReportRequest),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyReportRequestId
                // SequenceNumber
                // GeneratedAt
                // ReportData

                DebugX.Log("OnNotifyReport: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyReportRequestSerializer,
                                       CustomReportDataSerializer,
                                       CustomComponentSerializer,
                                       CustomEVSESerializer,
                                       CustomVariableSerializer,
                                       CustomVariableAttributeSerializer,
                                       CustomVariableCharacteristicsSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyReportResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyReportResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyReportResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyReportResponse event

                var responseLogger = OnNotifyReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyMonitoringReport

            CSMSChannel.OnNotifyMonitoringReport += async (timestamp,
                                                           sender,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnNotifyMonitoringReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyMonitoringReportRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyMonitoringReportRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyMonitoringReportRequest),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyMonitoringReportRequestId
                // SequenceNumber
                // GeneratedAt
                // MonitoringData
                // ToBeContinued

                DebugX.Log("OnNotifyMonitoringReport: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyMonitoringReportRequestSerializer,
                                       CustomMonitoringDataSerializer,
                                       CustomComponentSerializer,
                                       CustomEVSESerializer,
                                       CustomVariableSerializer,
                                       CustomVariableMonitoringSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyMonitoringReportResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyMonitoringReportResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyMonitoringReportResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyMonitoringReportResponse event

                var responseLogger = OnNotifyMonitoringReportResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyMonitoringReportResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyMonitoringReportResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnLogStatusNotification

            CSMSChannel.OnLogStatusNotification += async (timestamp,
                                                          sender,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnLogStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnLogStatusNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnLogStatusNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnLogStatusNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Status
                // LogRquestId

                DebugX.Log("OnLogStatusNotification: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomLogStatusNotificationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new LogStatusNotificationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new LogStatusNotificationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomLogStatusNotificationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnLogStatusNotificationResponse event

                var responseLogger = OnLogStatusNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnLogStatusNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnLogStatusNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnIncomingDataTransfer

            CSMSChannel.OnIncomingDataTransfer += async (timestamp,
                                                         sender,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnIncomingDataTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnIncomingDataTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnIncomingDataTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSetDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VendorId
                // MessageId
                // Data

                DebugX.Log("OnIncomingDataTransfer: " + request.VendorId  + ", " +
                                                        request.MessageId + ", " +
                                                        request.Data);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var responseData = request.Data;

                if (request.Data is not null)
                {

                    if      (request.Data.Type == JTokenType.String)
                        responseData = request.Data.ToString().Reverse();

                    else if (request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                   property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomIncomingDataTransferRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new DataTransferResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : request.VendorId == Vendor_Id.GraphDefined

                                         ? new (
                                               Request:      request,
                                               Status:       DataTransferStatus.Accepted,
                                               Data:         responseData,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           )

                                         : new DataTransferResponse(
                                               Request:      request,
                                               Status:       DataTransferStatus.Rejected,
                                               Data:         null,
                                               StatusInfo:   null,
                                               CustomData:   null
                                         );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomIncomingDataTransferResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnIncomingDataTransferResponse event

                var responseLogger = OnIncomingDataTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnIncomingDataTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnIncomingDataTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnSignCertificate

            CSMSChannel.OnSignCertificate += async (timestamp,
                                                    sender,
                                                    request,
                                                    cancellationToken) => {

                #region Send OnSignCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSignCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSignCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSignCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // CSR
                // CertificateType

                DebugX.Log("OnSignCertificate: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomSignCertificateRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new SignCertificateResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new SignCertificateResponse(
                                         Request:      request,
                                         Status:       GenericStatus.Accepted,
                                         StatusInfo:   null,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomSignCertificateResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnSignCertificateResponse event

                var responseLogger = OnSignCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSignCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnSignCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGet15118EVCertificate

            CSMSChannel.OnGet15118EVCertificate += async (timestamp,
                                                          sender,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnGet15118EVCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGet15118EVCertificateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGet15118EVCertificateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnGet15118EVCertificateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ISO15118SchemaVersion
                // CertificateAction
                // EXIRequest
                // MaximumContractCertificateChains
                // PrioritizedEMAIds

                DebugX.Log("OnGet15118EVCertificate: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomGet15118EVCertificateRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new Get15118EVCertificateResponse(
                                         Request:              request,
                                         Result:               Result.SignatureError(
                                                                   $"Invalid signature(s): {errorResponse}"
                                                               )
                                     )

                                   : new Get15118EVCertificateResponse(
                                         Request:              request,
                                         Status:               ISO15118EVCertificateStatus.Accepted,
                                         EXIResponse:          EXIData.Parse("0x1234"),
                                         RemainingContracts:   null,
                                         StatusInfo:           null,
                                         CustomData:           null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomGet15118EVCertificateResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnGet15118EVCertificateResponse event

                var responseLogger = OnGet15118EVCertificateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGet15118EVCertificateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnGet15118EVCertificateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCertificateStatus

            CSMSChannel.OnGetCertificateStatus += async (timestamp,
                                                         sender,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnGetCertificateStatusRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCertificateStatusRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCertificateStatusRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnGetCertificateStatusRequest),
                                  e
                              );
                    }

                }

                #endregion

                // OCSPRequestData

                DebugX.Log("OnGetCertificateStatus: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomGetCertificateStatusRequestSerializer,
                                       CustomOCSPRequestDataSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new GetCertificateStatusResponse(
                                         Request:              request,
                                         Result:               Result.SignatureError(
                                                                   $"Invalid signature(s): {errorResponse}"
                                                               )
                                     )

                                   : new GetCertificateStatusResponse(
                                         Request:      request,
                                         Status:       GetCertificateStatus.Accepted,
                                         OCSPResult:   OCSPResult.Parse("ok!"),
                                         StatusInfo:   null,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomGetCertificateStatusResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnGetCertificateStatusResponse event

                var responseLogger = OnGetCertificateStatusResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCertificateStatusResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnGetCertificateStatusResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCRL

            CSMSChannel.OnGetCRL += async (timestamp,
                                           sender,
                                           request,
                                           cancellationToken) => {

                #region Send OnGetCRLRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCRLRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCRLRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnGetCRLRequest),
                                  e
                              );
                    }

                }

                #endregion

                // GetCRLRequestId
                // CertificateHashData

                DebugX.Log("OnGetCRL: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomGetCRLRequestSerializer,
                                       CustomCertificateHashDataSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new GetCRLResponse(
                                         Request:           request,
                                         Result:            Result.SignatureError(
                                                                $"Invalid signature(s): {errorResponse}"
                                                            )
                                     )

                                   : new GetCRLResponse(
                                         Request:           request,
                                         GetCRLRequestId:   request.GetCRLRequestId,
                                         Status:            GenericStatus.Accepted,
                                         StatusInfo:        null,
                                         CustomData:        null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomGetCRLResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnGetCRLResponse event

                var responseLogger = OnGetCRLResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCRLResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnGetCRLResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnReservationStatusUpdate

            CSMSChannel.OnReservationStatusUpdate += async (timestamp,
                                                            sender,
                                                            request,
                                                            cancellationToken) => {

                #region Send OnReservationStatusUpdateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReservationStatusUpdateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReservationStatusUpdateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnReservationStatusUpdateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ReservationId
                // ReservationUpdateStatus

                DebugX.Log("OnReservationStatusUpdate: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomReservationStatusUpdateRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new ReservationStatusUpdateResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new ReservationStatusUpdateResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomReservationStatusUpdateResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnReservationStatusUpdateResponse event

                var responseLogger = OnReservationStatusUpdateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReservationStatusUpdateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnReservationStatusUpdateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnAuthorize

            CSMSChannel.OnAuthorize += async (timestamp,
                                              sender,
                                              request,
                                              cancellationToken) => {

                #region Send OnAuthorizeRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnAuthorizeRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnAuthorizeRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnAuthorizeRequest),
                                  e
                              );
                    }

                }

                #endregion

                // IdToken
                // Certificate
                // ISO15118CertificateHashData

                DebugX.Log("OnAuthorize: " + request.ChargingStationId + ", " +
                                                    request.IdToken);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomAuthorizeRequestSerializer,
                                       CustomIdTokenSerializer,
                                       CustomAdditionalInfoSerializer,
                                       CustomOCSPRequestDataSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new AuthorizeResponse(
                                         Request:             request,
                                         Result:              Result.SignatureError(
                                                                  $"Invalid signature(s): {errorResponse}"
                                                              )
                                     )

                                   : new AuthorizeResponse(
                                         Request:             request,
                                         IdTokenInfo:         new IdTokenInfo(
                                                                  Status:                AuthorizationStatus.Accepted,
                                                                  CacheExpiryDateTime:   Timestamp.Now.AddDays(3)
                                                              ),
                                         CertificateStatus:   AuthorizeCertificateStatus.Accepted,
                                         CustomData:          null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomAuthorizeResponseSerializer,
                        CustomIdTokenInfoSerializer,
                        CustomIdTokenSerializer,
                        CustomAdditionalInfoSerializer,
                        CustomMessageContentSerializer,
                        CustomTransactionLimitsSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnAuthorizeResponse event

                var responseLogger = OnAuthorizeResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnAuthorizeResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnAuthorizeResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyEVChargingNeeds

            CSMSChannel.OnNotifyEVChargingNeeds += async (timestamp,
                                                          sender,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnNotifyEVChargingNeedsRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEVChargingNeedsRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEVChargingNeedsRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEVChargingNeedsRequest),
                                  e
                              );
                    }

                }

                #endregion

                // EVSEId
                // ChargingNeeds
                // MaxScheduleTuples

                DebugX.Log("OnNotifyEVChargingNeeds: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyEVChargingNeedsRequestSerializer,
                                       CustomChargingNeedsSerializer,
                                       CustomACChargingParametersSerializer,
                                       CustomDCChargingParametersSerializer,
                                       CustomV2XChargingParametersSerializer,
                                       CustomEVEnergyOfferSerializer,
                                       CustomEVPowerScheduleSerializer,
                                       CustomEVPowerScheduleEntrySerializer,
                                       CustomEVAbsolutePriceScheduleSerializer,
                                       CustomEVAbsolutePriceScheduleEntrySerializer,
                                       CustomEVPriceRuleSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyEVChargingNeedsResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyEVChargingNeedsResponse(
                                         Request:      request,
                                         Status:       NotifyEVChargingNeedsStatus.Accepted,
                                         StatusInfo:   null,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyEVChargingNeedsResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyEVChargingNeedsResponse event

                var responseLogger = OnNotifyEVChargingNeedsResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEVChargingNeedsResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEVChargingNeedsResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnTransactionEvent

            CSMSChannel.OnTransactionEvent += async (timestamp,
                                                     sender,
                                                     request,
                                                     cancellationToken) => {

                #region Send OnTransactionEventRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnTransactionEventRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnTransactionEventRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnTransactionEventRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargeBoxId
                // EventType
                // Timestamp
                // TriggerReason
                // SequenceNumber
                // TransactionInfo
                // 
                // Offline
                // NumberOfPhasesUsed
                // CableMaxCurrent
                // ReservationId
                // IdToken
                // EVSE
                // MeterValues
                // PreconditioningStatus

                DebugX.Log("OnTransactionEvent: " + request.ChargingStationId + ", " +
                                                    request.IdToken);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CSMSSOAPServer centralSystemSOAPServer)

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomTransactionEventRequestSerializer,
                                       CustomTransactionSerializer,
                                       CustomIdTokenSerializer,
                                       CustomAdditionalInfoSerializer,
                                       CustomEVSESerializer,
                                       CustomMeterValueSerializer,
                                       CustomSampledValueSerializer,
                                       CustomSignedMeterValueSerializer,
                                       CustomUnitsOfMeasureSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new TransactionEventResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new TransactionEventResponse(
                                         Request:                  request,
                                         TotalCost:                null,
                                         ChargingPriority:         null,
                                         IdTokenInfo:              null,
                                         UpdatedPersonalMessage:   null,
                                         CustomData:               null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomTransactionEventResponseSerializer,
                        CustomIdTokenInfoSerializer,
                        CustomIdTokenSerializer,
                        CustomAdditionalInfoSerializer,
                        CustomMessageContentSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnTransactionEventResponse event

                var responseLogger = OnTransactionEventResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnTransactionEventResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnTransactionEventResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnStatusNotification

            CSMSChannel.OnStatusNotification += async (timestamp,
                                                       sender,
                                                       request,
                                                       cancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnStatusNotificationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnStatusNotificationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnStatusNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // Timestamp
                // ConnectorStatus
                // EVSEId
                // ConnectorId

                DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomStatusNotificationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new StatusNotificationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new StatusNotificationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomStatusNotificationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnStatusNotificationResponse event

                var responseLogger = OnStatusNotificationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnStatusNotificationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnStatusNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnMeterValues

            CSMSChannel.OnMeterValues += async (timestamp,
                                                sender,
                                                request,
                                                cancellationToken) => {

                #region Send OnMeterValuesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnMeterValuesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnMeterValuesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnMeterValuesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // EVSEId
                // MeterValues

                DebugX.Log("OnMeterValues: " + request.EVSEId);

                DebugX.Log(request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                                                        meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomMeterValuesRequestSerializer,
                                       CustomMeterValueSerializer,
                                       CustomSampledValueSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new MeterValuesResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new MeterValuesResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomMeterValuesResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnMeterValuesResponse event

                var responseLogger = OnMeterValuesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnMeterValuesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnMeterValuesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyChargingLimit

            CSMSChannel.OnNotifyChargingLimit += async (timestamp,
                                                        sender,
                                                        request,
                                                        cancellationToken) => {

                #region Send OnNotifyChargingLimitRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyChargingLimitRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyChargingLimitRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyChargingLimitRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingLimit
                // ChargingSchedules
                // EVSEId

                DebugX.Log("OnNotifyChargingLimit: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(

                                       CustomNotifyChargingLimitRequestSerializer,
                                       CustomChargingScheduleSerializer,
                                       CustomLimitBeyondSoCSerializer,
                                       CustomChargingSchedulePeriodSerializer,
                                       CustomV2XFreqWattEntrySerializer,
                                       CustomV2XSignalWattEntrySerializer,
                                       CustomSalesTariffSerializer,
                                       CustomSalesTariffEntrySerializer,
                                       CustomRelativeTimeIntervalSerializer,
                                       CustomConsumptionCostSerializer,
                                       CustomCostSerializer,

                                       CustomAbsolutePriceScheduleSerializer,
                                       CustomPriceRuleStackSerializer,
                                       CustomPriceRuleSerializer,
                                       CustomTaxRuleSerializer,
                                       CustomOverstayRuleListSerializer,
                                       CustomOverstayRuleSerializer,
                                       CustomAdditionalServiceSerializer,

                                       CustomPriceLevelScheduleSerializer,
                                       CustomPriceLevelScheduleEntrySerializer,

                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer

                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyChargingLimitResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyChargingLimitResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyChargingLimitResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyChargingLimitResponse event

                var responseLogger = OnNotifyChargingLimitResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyChargingLimitResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyChargingLimitResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearedChargingLimit

            CSMSChannel.OnClearedChargingLimit += async (timestamp,
                                                         sender,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnClearedChargingLimitRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearedChargingLimitRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearedChargingLimitRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnClearedChargingLimitRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingLimitSource
                // EVSEId

                DebugX.Log("OnClearedChargingLimit: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomClearedChargingLimitRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new ClearedChargingLimitResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new ClearedChargingLimitResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomClearedChargingLimitResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnClearedChargingLimitResponse event

                var responseLogger = OnClearedChargingLimitResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearedChargingLimitResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnClearedChargingLimitResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnReportChargingProfiles

            CSMSChannel.OnReportChargingProfiles += async (timestamp,
                                                           sender,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnReportChargingProfilesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReportChargingProfilesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReportChargingProfilesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnReportChargingProfilesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ReportChargingProfilesRequestId
                // ChargingLimitSource
                // EVSEId
                // ChargingProfiles
                // ToBeContinued

                DebugX.Log("OnReportChargingProfiles: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(

                                       CustomReportChargingProfilesRequestSerializer,
                                       CustomChargingProfileSerializer,
                                       CustomLimitBeyondSoCSerializer,
                                       CustomChargingScheduleSerializer,
                                       CustomChargingSchedulePeriodSerializer,
                                       CustomV2XFreqWattEntrySerializer,
                                       CustomV2XSignalWattEntrySerializer,
                                       CustomSalesTariffSerializer,
                                       CustomSalesTariffEntrySerializer,
                                       CustomRelativeTimeIntervalSerializer,
                                       CustomConsumptionCostSerializer,
                                       CustomCostSerializer,

                                       CustomAbsolutePriceScheduleSerializer,
                                       CustomPriceRuleStackSerializer,
                                       CustomPriceRuleSerializer,
                                       CustomTaxRuleSerializer,
                                       CustomOverstayRuleListSerializer,
                                       CustomOverstayRuleSerializer,
                                       CustomAdditionalServiceSerializer,

                                       CustomPriceLevelScheduleSerializer,
                                       CustomPriceLevelScheduleEntrySerializer,

                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer

                                   ),
                                   out var errorResponse
                               )

                                   ? new ReportChargingProfilesResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new ReportChargingProfilesResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomReportChargingProfilesResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnReportChargingProfilesResponse event

                var responseLogger = OnReportChargingProfilesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReportChargingProfilesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnReportChargingProfilesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyEVChargingSchedule

            CSMSChannel.OnNotifyEVChargingSchedule += async (timestamp,
                                                             sender,
                                                             request,
                                                             cancellationToken) => {

                #region Send OnNotifyEVChargingScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEVChargingScheduleRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEVChargingScheduleRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEVChargingScheduleRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TimeBase
                // EVSEId
                // ChargingSchedule
                // SelectedScheduleTupleId
                // PowerToleranceAcceptance

                DebugX.Log("OnNotifyEVChargingSchedule: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(

                                       CustomNotifyEVChargingScheduleRequestSerializer,
                                       CustomChargingScheduleSerializer,
                                       CustomLimitBeyondSoCSerializer,
                                       CustomChargingSchedulePeriodSerializer,
                                       CustomV2XFreqWattEntrySerializer,
                                       CustomV2XSignalWattEntrySerializer,
                                       CustomSalesTariffSerializer,
                                       CustomSalesTariffEntrySerializer,
                                       CustomRelativeTimeIntervalSerializer,
                                       CustomConsumptionCostSerializer,
                                       CustomCostSerializer,

                                       CustomAbsolutePriceScheduleSerializer,
                                       CustomPriceRuleStackSerializer,
                                       CustomPriceRuleSerializer,
                                       CustomTaxRuleSerializer,
                                       CustomOverstayRuleListSerializer,
                                       CustomOverstayRuleSerializer,
                                       CustomAdditionalServiceSerializer,

                                       CustomPriceLevelScheduleSerializer,
                                       CustomPriceLevelScheduleEntrySerializer,

                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer

                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyEVChargingScheduleResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyEVChargingScheduleResponse(
                                         Request:      request,
                                         Status:       GenericStatus.Accepted,
                                         StatusInfo:   null,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyEVChargingScheduleResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyEVChargingScheduleResponse event

                var responseLogger = OnNotifyEVChargingScheduleResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEVChargingScheduleResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyEVChargingScheduleResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyPriorityCharging

            CSMSChannel.OnNotifyPriorityCharging += async (timestamp,
                                                           sender,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnNotifyPriorityChargingRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyPriorityChargingRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyPriorityChargingRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyPriorityChargingRequest),
                                  e
                              );
                    }

                }

                #endregion

                // TransactionId
                // Activated

                DebugX.Log("OnNotifyPriorityCharging: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyPriorityChargingRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyPriorityChargingResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyPriorityChargingResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyPriorityChargingResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyPriorityChargingResponse event

                var responseLogger = OnNotifyPriorityChargingResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyPriorityChargingResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyPriorityChargingResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnPullDynamicScheduleUpdate

            CSMSChannel.OnPullDynamicScheduleUpdate += async (timestamp,
                                                              sender,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnPullDynamicScheduleUpdateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPullDynamicScheduleUpdateRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPullDynamicScheduleUpdateRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnPullDynamicScheduleUpdateRequest),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingProfileId

                DebugX.Log("OnPullDynamicScheduleUpdate: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomPullDynamicScheduleUpdateRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new PullDynamicScheduleUpdateResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new PullDynamicScheduleUpdateResponse(

                                         Request:               request,

                                         Limit:                 ChargingRateValue.Parse( 1, ChargingRateUnits.Watts),
                                         Limit_L2:              ChargingRateValue.Parse( 2, ChargingRateUnits.Watts),
                                         Limit_L3:              ChargingRateValue.Parse( 3, ChargingRateUnits.Watts),

                                         DischargeLimit:        ChargingRateValue.Parse(-4, ChargingRateUnits.Watts),
                                         DischargeLimit_L2:     ChargingRateValue.Parse(-5, ChargingRateUnits.Watts),
                                         DischargeLimit_L3:     ChargingRateValue.Parse(-6, ChargingRateUnits.Watts),

                                         Setpoint:              ChargingRateValue.Parse( 7, ChargingRateUnits.Watts),
                                         Setpoint_L2:           ChargingRateValue.Parse( 8, ChargingRateUnits.Watts),
                                         Setpoint_L3:           ChargingRateValue.Parse( 9, ChargingRateUnits.Watts),

                                         SetpointReactive:      ChargingRateValue.Parse(10, ChargingRateUnits.Watts),
                                         SetpointReactive_L2:   ChargingRateValue.Parse(11, ChargingRateUnits.Watts),
                                         SetpointReactive_L3:   ChargingRateValue.Parse(12, ChargingRateUnits.Watts),

                                         CustomData:            null

                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomPullDynamicScheduleUpdateResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnPullDynamicScheduleUpdateResponse event

                var responseLogger = OnPullDynamicScheduleUpdateResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPullDynamicScheduleUpdateResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnPullDynamicScheduleUpdateResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            #region OnNotifyDisplayMessages

            CSMSChannel.OnNotifyDisplayMessages += async (timestamp,
                                                          sender,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnNotifyDisplayMessagesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyDisplayMessagesRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyDisplayMessagesRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyDisplayMessagesRequest),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyDisplayMessagesRequestId
                // MessageInfos
                // ToBeContinued

                //DebugX.Log("OnNotifyDisplayMessages: " + Request.EVSEId);

                //DebugX.Log(Request.NotifyDisplayMessages.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                //                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyDisplayMessagesRequestSerializer,
                                       CustomMessageInfoSerializer,
                                       CustomMessageContentSerializer,
                                       CustomComponentSerializer,
                                       CustomEVSESerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyDisplayMessagesResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyDisplayMessagesResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyDisplayMessagesResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyDisplayMessagesResponse event

                var responseLogger = OnNotifyDisplayMessagesResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyDisplayMessagesResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyDisplayMessagesResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnNotifyCustomerInformation

            CSMSChannel.OnNotifyCustomerInformation += async (timestamp,
                                                              sender,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnNotifyCustomerInformationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyCustomerInformationRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyCustomerInformationRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnNotifyCustomerInformationRequest),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyCustomerInformationRequestId
                // Data
                // SequenceNumber
                // GeneratedAt
                // ToBeContinued

                DebugX.Log("OnNotifyCustomerInformation: " + request.ChargingStationId);

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));

                }
                else
                {

                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);

                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomNotifyCustomerInformationRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new NotifyCustomerInformationResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new NotifyCustomerInformationResponse(
                                         Request:      request,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomNotifyCustomerInformationResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnNotifyCustomerInformationResponse event

                var responseLogger = OnNotifyCustomerInformationResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyCustomerInformationResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnNotifyCustomerInformationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            // Binary Data Streams Extensions

            #region OnIncomingBinaryDataTransfer

            CSMSChannel.OnIncomingBinaryDataTransfer += async (timestamp,
                                                               sender,
                                                               request,
                                                               cancellationToken) => {

                #region Send OnIncomingBinaryDataTransferRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnIncomingBinaryDataTransferRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnIncomingBinaryDataTransferRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                             this,
                                                                                                             request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSetDisplayMessageRequest),
                                  e
                              );
                    }

                }

                #endregion

                // VendorId
                // MessageId
                // BinaryData

                DebugX.Log("OnIncomingBinaryDataTransfer: " + request.VendorId  + ", " +
                                                              request.MessageId + ", " +
                                                              request.Data?.ToUTF8String() ?? "-");

                if (!reachableChargingStations.ContainsKey(request.ChargingStationId))
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations.TryAdd(request.ChargingStationId, new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (sender is CSMSWSServer centralSystemWSServer)
                        reachableChargingStations[request.ChargingStationId] = new Tuple<ICSMSChannel, DateTime>(centralSystemWSServer, Timestamp.Now);
                }


                var responseBinaryData = new Byte[0];

                if (request.Data is not null)
                {
                    responseBinaryData = ((Byte[]) request.Data.Clone()).Reverse();
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToBinary(
                                       CustomIncomingBinaryDataTransferRequestSerializer,
                                       CustomBinarySignatureSerializer,
                                       IncludeSignatures: false
                                   ),
                                   out var errorResponse
                               )

                                   ? new BinaryDataTransferResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : request.VendorId == Vendor_Id.GraphDefined

                                         ? new (
                                               Request:                request,
                                               Status:                 BinaryDataTransferStatus.Accepted,
                                               AdditionalStatusInfo:   null,
                                               Data:                   responseBinaryData
                                           )

                                         : new BinaryDataTransferResponse(
                                               Request:                request,
                                               Status:                 BinaryDataTransferStatus.Rejected,
                                               AdditionalStatusInfo:   null,
                                               Data:                   responseBinaryData
                                           );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToBinary(
                        CustomIncomingBinaryDataTransferResponseSerializer,
                        null, //CustomStatusInfoSerializer,
                        CustomBinarySignatureSerializer,
                        IncludeSignatures: false
                    ),
                    out var errorResponse2);


                #region Send OnIncomingBinaryDataTransferResponse event

                var responseLogger = OnIncomingBinaryDataTransferResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnIncomingBinaryDataTransferResponseDelegate>().
                                                              Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
                                                                                                                this,
                                                                                                                request,
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
                                  nameof(TestCSMS),
                                  nameof(OnIncomingBinaryDataTransferResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages

        }

        #endregion


        #region CSMS -> Charging Station Messages

        #region NextRequestId

        public Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion


        #region Reset                       (Request)

        /// <summary>
        /// Reset the given charging station.
        /// </summary>
        /// <param name="Request">A Reset request.</param>
        public async Task<CS.ResetResponse>
            Reset(ResetRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnResetRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomResetRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.Reset(Request)

                                      : new CS.ResetResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ResetResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomResetResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware              (Request)

        /// <summary>
        /// Initiate a firmware update of the given charging station.
        /// </summary>
        /// <param name="Request">An UpdateFirmware request.</param>
        public async Task<CS.UpdateFirmwareResponse>
            UpdateFirmware(UpdateFirmwareRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUpdateFirmwareRequestSerializer,
                                          CustomFirmwareSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateFirmware(Request)

                                      : new CS.UpdateFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateFirmwareResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUpdateFirmwareResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PublishFirmware             (Request)

        /// <summary>
        /// Publish a firmware onto a local controller.
        /// </summary>
        /// <param name="Request">A PublishFirmware request.</param>
        public async Task<CS.PublishFirmwareResponse>
            PublishFirmware(PublishFirmwareRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnPublishFirmwareRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomPublishFirmwareRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.PublishFirmware(Request)

                                      : new CS.PublishFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.PublishFirmwareResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomPublishFirmwareResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnPublishFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnPublishFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnpublishFirmware           (Request)

        /// <summary>
        /// Unpublish a firmware from a local controller.
        /// </summary>
        /// <param name="Request">An UnpublishFirmware request.</param>
        public async Task<CS.UnpublishFirmwareResponse>
            UnpublishFirmware(UnpublishFirmwareRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnpublishFirmwareRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUnpublishFirmwareRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UnpublishFirmware(Request)

                                      : new CS.UnpublishFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UnpublishFirmwareResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUnpublishFirmwareResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUnpublishFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnpublishFirmwareResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnpublishFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetBaseReport               (Request)

        /// <summary>
        /// Retrieve the base report from the charging station.
        /// </summary>
        /// <param name="Request">A GetBaseReport request.</param>
        public async Task<CS.GetBaseReportResponse>
            GetBaseReport(GetBaseReportRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetBaseReportRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetBaseReportRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetBaseReport(Request)

                                      : new CS.GetBaseReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetBaseReportResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetBaseReportResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetBaseReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetBaseReportResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetBaseReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetReport                   (Request)

        /// <summary>
        /// Retrieve reports from the charging station.
        /// </summary>
        /// <param name="Request">A GetReport request.</param>
        public async Task<CS.GetReportResponse>
            GetReport(GetReportRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetReportRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetReportRequestSerializer,
                                          CustomComponentVariableSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomVariableSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetReport(Request)

                                      : new CS.GetReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetReportResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetReportResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetReportResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                      (Request)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A GetLog request.</param>
        public async Task<CS.GetLogResponse>
            GetLog(GetLogRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetLogRequestSerializer,
                                          CustomLogParametersSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetLog(Request)

                                      : new CS.GetLogResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetLogResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetLogResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetLogResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLogResponse?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SetVariables                (Request)

        /// <summary>
        /// Set variable data on a charging station.
        /// </summary>
        /// <param name="Request">A SetVariables request.</param>
        public async Task<CS.SetVariablesResponse>
            SetVariables(SetVariablesRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariablesRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetVariablesRequestSerializer,
                                          CustomSetVariableDataSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomVariableSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetVariables(Request)

                                      : new CS.SetVariablesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetVariablesResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetVariablesResponseSerializer,
                    CustomSetVariableResultSerializer,
                    CustomComponentSerializer,
                    CustomEVSESerializer,
                    CustomVariableSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetVariablesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetVariablesResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetVariables                (Request)

        /// <summary>
        /// Get variable data from a charging station.
        /// </summary>
        /// <param name="Request">A GetVariables request.</param>
        public async Task<CS.GetVariablesResponse>
            GetVariables(GetVariablesRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetVariablesRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetVariablesRequestSerializer,
                                          CustomGetVariableDataSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomVariableSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetVariables(Request)

                                      : new CS.GetVariablesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetVariablesResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetVariablesResponseSerializer,
                    CustomGetVariableResultSerializer,
                    CustomComponentSerializer,
                    CustomEVSESerializer,
                    CustomVariableSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetVariablesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetVariablesResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetMonitoringBase           (Request)

        /// <summary>
        /// Set the monitoring base of a charging station.
        /// </summary>
        /// <param name="Request">A SetMonitoringBase request.</param>
        public async Task<CS.SetMonitoringBaseResponse>
            SetMonitoringBase(SetMonitoringBaseRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringBaseRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetMonitoringBaseRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetMonitoringBase(Request)

                                      : new CS.SetMonitoringBaseResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetMonitoringBaseResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetMonitoringBaseResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetMonitoringBaseResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetMonitoringBaseResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringBaseResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetMonitoringReport         (Request)

        /// <summary>
        /// Get monitoring report from a charging station.
        /// </summary>
        /// <param name="Request">A GetMonitoringReport request.</param>
        public async Task<CS.GetMonitoringReportResponse>
            GetMonitoringReport(GetMonitoringReportRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetMonitoringReportRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetMonitoringReportRequestSerializer,
                                          CustomComponentVariableSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomVariableSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetMonitoringReport(Request)

                                      : new CS.GetMonitoringReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetMonitoringReportResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetMonitoringReportResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetMonitoringReportResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetMonitoringLevel          (Request)

        /// <summary>
        /// Set the monitoring level on a charging station.
        /// </summary>
        /// <param name="Request">A SetMonitoringLevel request.</param>
        public async Task<CS.SetMonitoringLevelResponse>
            SetMonitoringLevel(SetMonitoringLevelRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringLevelRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetMonitoringLevelRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetMonitoringLevel(Request)

                                      : new CS.SetMonitoringLevelResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetMonitoringLevelResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetMonitoringLevelResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetMonitoringLevelResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetMonitoringLevelResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringLevelResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetVariableMonitoring       (Request)

        /// <summary>
        /// Set a variable monitoring on a charging station.
        /// </summary>
        /// <param name="Request">A SetVariableMonitoring request.</param>
        public async Task<CS.SetVariableMonitoringResponse>
            SetVariableMonitoring(SetVariableMonitoringRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariableMonitoringRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetVariableMonitoringRequestSerializer,
                                          CustomSetMonitoringDataSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomVariableSerializer,
                                          CustomPeriodicEventStreamParametersSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetVariableMonitoring(Request)

                                      : new CS.SetVariableMonitoringResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetVariableMonitoringResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetVariableMonitoringResponseSerializer,
                    CustomSetMonitoringResultSerializer,
                    CustomComponentSerializer,
                    CustomEVSESerializer,
                    CustomVariableSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetVariableMonitoringResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetVariableMonitoringResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariableMonitoringResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearVariableMonitoring     (Request)

        /// <summary>
        /// Delete a variable monitoring on a charging station.
        /// </summary>
        /// <param name="Request">A ClearVariableMonitoring request.</param>
        public async Task<CS.ClearVariableMonitoringResponse>
            ClearVariableMonitoring(ClearVariableMonitoringRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearVariableMonitoringRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearVariableMonitoringRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearVariableMonitoring(Request)

                                      : new CS.ClearVariableMonitoringResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearVariableMonitoringResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearVariableMonitoringResponseSerializer,
                    CustomClearMonitoringResultSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearVariableMonitoringResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearVariableMonitoringResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearVariableMonitoringResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetNetworkProfile           (Request)

        /// <summary>
        /// Set the network profile of a charging station.
        /// </summary>
        /// <param name="Request">A SetNetworkProfile request.</param>
        public async Task<CS.SetNetworkProfileResponse>
            SetNetworkProfile(SetNetworkProfileRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetNetworkProfileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetNetworkProfileRequestSerializer,
                                          CustomNetworkConnectionProfileSerializer,
                                          CustomVPNConfigurationSerializer,
                                          CustomAPNConfigurationSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetNetworkProfile(Request)

                                      : new CS.SetNetworkProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetNetworkProfileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetNetworkProfileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetNetworkProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetNetworkProfileResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetNetworkProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability          (Request)

        /// <summary>
        /// Change the availability of the given charging station.
        /// </summary>
        /// <param name="Request">A ChangeAvailability request.</param>
        public async Task<CS.ChangeAvailabilityResponse>
            ChangeAvailability(ChangeAvailabilityRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomChangeAvailabilityRequestSerializer,
                                          CustomEVSESerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ChangeAvailability(Request)

                                      : new CS.ChangeAvailabilityResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ChangeAvailabilityResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomChangeAvailabilityResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage              (Request)

        /// <summary>
        /// Create a trigger for the given message at the given charging station connector.
        /// </summary>
        /// <param name="Request">A TriggerMessage request.</param>
        public async Task<CS.TriggerMessageResponse>
            TriggerMessage(TriggerMessageRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomTriggerMessageRequestSerializer,
                                          CustomEVSESerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.TriggerMessage(Request)

                                      : new CS.TriggerMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.TriggerMessageResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomTriggerMessageResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData                (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<CS.DataTransferResponse>
            TransferData(DataTransferRequest Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDataTransferRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.TransferData(Request)

                                      : new CS.DataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DataTransferResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDataTransferResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendSignedCertificate       (Request)

        /// <summary>
        /// Send the signed certificate to the given charging station.
        /// </summary>
        /// <param name="Request">A CertificateSigned request.</param>
        public async Task<CS.CertificateSignedResponse>
            SendSignedCertificate(CertificateSignedRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCertificateSignedRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendSignedCertificate(Request)

                                      : new CS.CertificateSignedResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CertificateSignedResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCertificateSignedResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate          (Request)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">A InstallCertificate request.</param>
        public async Task<CS.InstallCertificateResponse>
            InstallCertificate(InstallCertificateRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomInstallCertificateRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.InstallCertificate(Request)

                                      : new CS.InstallCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.InstallCertificateResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomInstallCertificateResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds  (Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A GetInstalledCertificateIds request.</param>
        public async Task<CS.GetInstalledCertificateIdsResponse>
            GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetInstalledCertificateIdsRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetInstalledCertificateIds(Request)

                                      : new CS.GetInstalledCertificateIdsResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetInstalledCertificateIdsResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetInstalledCertificateIdsResponseSerializer,
                    CustomCertificateHashDataSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate           (Request)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="Request">A DeleteCertificate request.</param>
        public async Task<CS.DeleteCertificateResponse>
            DeleteCertificate(DeleteCertificateRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDeleteCertificateRequestSerializer,
                                          CustomCertificateHashDataSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteCertificate(Request)

                                      : new CS.DeleteCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DeleteCertificateResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDeleteCertificateResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCRLAvailability       (Request)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="Request">A NotifyCRLAvailability request.</param>
        public async Task<CS.NotifyCRLResponse>
            NotifyCRLAvailability(NotifyCRLRequest Request)

        {

            #region Send OnNotifyCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCRLRequest?.Invoke(startTime,
                                           this,
                                           Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyCRLRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomNotifyCRLRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.NotifyCRLAvailability(Request)

                                      : new CS.NotifyCRLResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.NotifyCRLResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyCRLResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCRLResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion         (Request)

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="Request">A GetLocalListVersion request.</param>
        public async Task<CS.GetLocalListVersionResponse>
            GetLocalListVersion(GetLocalListVersionRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetLocalListVersionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetLocalListVersion(Request)

                                      : new CS.GetLocalListVersionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetLocalListVersionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetLocalListVersionResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList               (Request)

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="Request">A SendLocalList request.</param>
        public async Task<CS.SendLocalListResponse>
            SendLocalList(SendLocalListRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSendLocalListRequestSerializer,
                                          CustomAuthorizationDataSerializer,
                                          CustomIdTokenSerializer,
                                          CustomAdditionalInfoSerializer,
                                          CustomIdTokenInfoSerializer,
                                          CustomMessageContentSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendLocalList(Request)

                                      : new CS.SendLocalListResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SendLocalListResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSendLocalListResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache                  (Request)

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="Request">A ClearCache request.</param>
        public async Task<CS.ClearCacheResponse>
            ClearCache(ClearCacheRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearCacheRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearCache(Request)

                                      : new CS.ClearCacheResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearCacheResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearCacheResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow                  (Request)

        /// <summary>
        /// Create a charging reservation of the given charging station connector.
        /// </summary>
        /// <param name="Request">A ReserveNow request.</param>
        public async Task<CS.ReserveNowResponse>
            ReserveNow(ReserveNowRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomReserveNowRequestSerializer,
                                          CustomIdTokenSerializer,
                                          CustomAdditionalInfoSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ReserveNow(Request)

                                      : new CS.ReserveNowResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ReserveNowResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomReserveNowResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation           (Request)

        /// <summary>
        /// Cancel the given charging reservation at the given charging station.
        /// </summary>
        /// <param name="Request">A CancelReservation request.</param>
        public async Task<CS.CancelReservationResponse>
            CancelReservation(CancelReservationRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCancelReservationRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.CancelReservation(Request)

                                      : new CS.CancelReservationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CancelReservationResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCancelReservationResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartCharging               (Request)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="Request">A StartCharging request.</param>
        public async Task<CS.RequestStartTransactionResponse>
            StartCharging(RequestStartTransactionRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStartTransactionRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRequestStartTransactionRequestSerializer,
                                          CustomIdTokenSerializer,
                                          CustomAdditionalInfoSerializer,
                                          CustomChargingProfileSerializer,
                                          CustomLimitBeyondSoCSerializer,
                                          CustomChargingScheduleSerializer,
                                          CustomChargingSchedulePeriodSerializer,
                                          CustomV2XFreqWattEntrySerializer,
                                          CustomV2XSignalWattEntrySerializer,
                                          CustomSalesTariffSerializer,
                                          CustomSalesTariffEntrySerializer,
                                          CustomRelativeTimeIntervalSerializer,
                                          CustomConsumptionCostSerializer,
                                          CustomCostSerializer,

                                          CustomAbsolutePriceScheduleSerializer,
                                          CustomPriceRuleStackSerializer,
                                          CustomPriceRuleSerializer,
                                          CustomTaxRuleSerializer,
                                          CustomOverstayRuleListSerializer,
                                          CustomOverstayRuleSerializer,
                                          CustomAdditionalServiceSerializer,

                                          CustomPriceLevelScheduleSerializer,
                                          CustomPriceLevelScheduleEntrySerializer,

                                          CustomTransactionLimitsSerializer,

                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.StartCharging(Request)

                                      : new CS.RequestStartTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RequestStartTransactionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomRequestStartTransactionResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnRequestStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopCharging                (Request)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="Request">A StopCharging request.</param>
        public async Task<CS.RequestStopTransactionResponse>
            StopCharging(RequestStopTransactionRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStopTransactionRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRequestStopTransactionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.StopCharging(Request)

                                      : new CS.RequestStopTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RequestStopTransactionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomRequestStopTransactionResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnRequestStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRequestStopTransactionResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetTransactionStatus        (Request)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="Request">A GetTransactionStatus request.</param>
        public async Task<CS.GetTransactionStatusResponse>
            GetTransactionStatus(GetTransactionStatusRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetTransactionStatusRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetTransactionStatusRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetTransactionStatus(Request)

                                      : new CS.GetTransactionStatusResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetTransactionStatusResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetTransactionStatusResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetTransactionStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetTransactionStatusResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetTransactionStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile          (Request)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="Request">A SetChargingProfile request.</param>
        public async Task<CS.SetChargingProfileResponse>
            SetChargingProfile(SetChargingProfileRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetChargingProfileRequestSerializer,
                                          CustomChargingProfileSerializer,
                                          CustomLimitBeyondSoCSerializer,
                                          CustomChargingScheduleSerializer,
                                          CustomChargingSchedulePeriodSerializer,
                                          CustomV2XFreqWattEntrySerializer,
                                          CustomV2XSignalWattEntrySerializer,
                                          CustomSalesTariffSerializer,
                                          CustomSalesTariffEntrySerializer,
                                          CustomRelativeTimeIntervalSerializer,
                                          CustomConsumptionCostSerializer,
                                          CustomCostSerializer,

                                          CustomAbsolutePriceScheduleSerializer,
                                          CustomPriceRuleStackSerializer,
                                          CustomPriceRuleSerializer,
                                          CustomTaxRuleSerializer,
                                          CustomOverstayRuleListSerializer,
                                          CustomOverstayRuleSerializer,
                                          CustomAdditionalServiceSerializer,

                                          CustomPriceLevelScheduleSerializer,
                                          CustomPriceLevelScheduleEntrySerializer,

                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetChargingProfile(Request)

                                      : new CS.SetChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetChargingProfileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetChargingProfileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetChargingProfiles         (Request)

        /// <summary>
        /// Set the charging profile of the given charging station connector.
        /// </summary>
        /// <param name="Request">A GetChargingProfiles request.</param>
        public async Task<CS.GetChargingProfilesResponse>
            GetChargingProfiles(GetChargingProfilesRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetChargingProfilesRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetChargingProfilesRequestSerializer,
                                          CustomChargingProfileCriterionSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetChargingProfiles(Request)

                                      : new CS.GetChargingProfilesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetChargingProfilesResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetChargingProfilesResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetChargingProfilesResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile        (Request)

        /// <summary>
        /// Remove the charging profile at the given charging station connector.
        /// </summary>
        /// <param name="Request">A ClearChargingProfile request.</param>
        public async Task<CS.ClearChargingProfileResponse>
            ClearChargingProfile(ClearChargingProfileRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearChargingProfileRequestSerializer,
                                          CustomClearChargingProfileSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearChargingProfile(Request)

                                      : new CS.ClearChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearChargingProfileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearChargingProfileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule        (Request)

        /// <summary>
        /// Return the charging schedule of the given charging station connector.
        /// </summary>
        /// <param name="Request">A GetCompositeSchedule request.</param>
        public async Task<CS.GetCompositeScheduleResponse>
            GetCompositeSchedule(GetCompositeScheduleRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetCompositeScheduleRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetCompositeSchedule(Request)

                                      : new CS.GetCompositeScheduleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetCompositeScheduleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetCompositeScheduleResponseSerializer,
                    CustomCompositeScheduleSerializer,
                    CustomChargingSchedulePeriodSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateDynamicSchedule       (Request)

        /// <summary>
        /// Update the dynamic charging schedule for the given charging profile.
        /// </summary>
        /// <param name="Request">A UpdateDynamicSchedule request.</param>
        public async Task<CS.UpdateDynamicScheduleResponse>
            UpdateDynamicSchedule(UpdateDynamicScheduleRequest Request)

        {

            #region Send OnUpdateDynamicScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateDynamicScheduleRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateDynamicScheduleRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUpdateDynamicScheduleRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateDynamicSchedule(Request)

                                      : new CS.UpdateDynamicScheduleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateDynamicScheduleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUpdateDynamicScheduleResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateDynamicScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateDynamicScheduleResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateDynamicScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyAllowedEnergyTransfer (Request)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="Request">A NotifyAllowedEnergyTransfer request.</param>
        public async Task<CS.NotifyAllowedEnergyTransferResponse>
            NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request)

        {

            #region Send OnNotifyAllowedEnergyTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyAllowedEnergyTransferRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyAllowedEnergyTransferRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomNotifyAllowedEnergyTransferRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.NotifyAllowedEnergyTransfer(Request)

                                      : new CS.NotifyAllowedEnergyTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.NotifyAllowedEnergyTransferResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomNotifyAllowedEnergyTransferResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyAllowedEnergyTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyAllowedEnergyTransferResponse?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyAllowedEnergyTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UsePriorityCharging         (Request)

        /// <summary>
        /// Switch to the priority charging profile.
        /// </summary>
        /// <param name="Request">A UsePriorityCharging request.</param>
        public async Task<CS.UsePriorityChargingResponse>
            UsePriorityCharging(UsePriorityChargingRequest Request)

        {

            #region Send OnUsePriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUsePriorityChargingRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUsePriorityChargingRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUsePriorityChargingRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UsePriorityCharging(Request)

                                      : new CS.UsePriorityChargingResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UsePriorityChargingResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUsePriorityChargingResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUsePriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUsePriorityChargingResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUsePriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector             (Request)

        /// <summary>
        /// Unlock the given charging station connector.
        /// </summary>
        /// <param name="Request">A UnlockConnector request.</param>
        public async Task<CS.UnlockConnectorResponse>
            UnlockConnector(UnlockConnectorRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUnlockConnectorRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UnlockConnector(Request)

                                      : new CS.UnlockConnectorResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UnlockConnectorResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUnlockConnectorResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendAFRRSignal              (Request)

        /// <summary>
        /// Send an aFRR signal to the charging station.
        /// The charging station uses the value of signal to select a matching power value
        /// from the v2xSignalWattCurve in the charging schedule period.
        /// </summary>
        /// <param name="Request">A AFRRSignal request.</param>
        public async Task<CS.AFRRSignalResponse>
            SendAFRRSignal(AFRRSignalRequest Request)

        {

            #region Send OnAFRRSignalRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAFRRSignalRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAFRRSignalRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomAFRRSignalRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendAFRRSignal(Request)

                                      : new CS.AFRRSignalResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.AFRRSignalResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomAFRRSignalResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAFRRSignalResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAFRRSignalResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAFRRSignalResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SetDisplayMessage           (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A SetDisplayMessage request.</param>
        public async Task<CS.SetDisplayMessageResponse>
            SetDisplayMessage(SetDisplayMessageRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDisplayMessageRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetDisplayMessageRequestSerializer,
                                          CustomMessageInfoSerializer,
                                          CustomMessageContentSerializer,
                                          CustomComponentSerializer,
                                          CustomEVSESerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetDisplayMessage(Request)

                                      : new CS.SetDisplayMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetDisplayMessageResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetDisplayMessageResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetDisplayMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetDisplayMessageResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDisplayMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDisplayMessages          (Request)

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="Request">A GetDisplayMessages request.</param>
        public async Task<CS.GetDisplayMessagesResponse>
            GetDisplayMessages(GetDisplayMessagesRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDisplayMessagesRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetDisplayMessagesRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetDisplayMessages(Request)

                                      : new CS.GetDisplayMessagesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetDisplayMessagesResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetDisplayMessagesResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearDisplayMessage         (Request)

        /// <summary>
        /// Remove a display message.
        /// </summary>
        /// <param name="Request">A ClearDisplayMessage request.</param>
        public async Task<CS.ClearDisplayMessageResponse>
            ClearDisplayMessage(ClearDisplayMessageRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearDisplayMessageRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearDisplayMessageRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearDisplayMessage(Request)

                                      : new CS.ClearDisplayMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearDisplayMessageResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearDisplayMessageResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearDisplayMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearDisplayMessageResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearDisplayMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendCostUpdated             (Request)

        /// <summary>
        /// Send updated total costs.
        /// </summary>
        /// <param name="Request">A CostUpdated request.</param>
        public async Task<CS.CostUpdatedResponse>
            SendCostUpdated(CostUpdatedRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCostUpdatedRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCostUpdatedRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendCostUpdated(Request)

                                      : new CS.CostUpdatedResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CostUpdatedResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCostUpdatedResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCostUpdatedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCostUpdatedResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCostUpdatedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RequestCustomerInformation  (Request)

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="Request">A CostUpdated request.</param>
        public async Task<CS.CustomerInformationResponse>
            RequestCustomerInformation(CustomerInformationRequest Request)

        {

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
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCustomerInformationRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCustomerInformationRequestSerializer,
                                          CustomIdTokenSerializer,
                                          CustomAdditionalInfoSerializer,
                                          CustomCertificateHashDataSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.RequestCustomerInformation(Request)

                                      : new CS.CustomerInformationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CustomerInformationResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCustomerInformationResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCustomerInformationResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData          (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<CS.BinaryDataTransferResponse>
            TransferBinaryData(BinaryDataTransferRequest Request)

        {

            #region Send OnBinaryDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBinaryDataTransferRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomBinaryDataTransferRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.TransferBinaryData(Request)

                                      : new CS.BinaryDataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.BinaryDataTransferResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomBinaryDataTransferResponseSerializer,
                    null, // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnBinaryDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBinaryDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetFile                     (Request)

        /// <summary>
        /// Request the given file from the charging station.
        /// </summary>
        /// <param name="Request">A GetFile request.</param>
        public async Task<CS.GetFileResponse>
            GetFile(GetFileRequest Request)

        {

            #region Send OnGetFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetFileRequest?.Invoke(startTime,
                                         this,
                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetFileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetFile(Request)

                                      : new CS.GetFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetFileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomGetFileResponseSerializer,
                    null, // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnGetFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetFileResponse?.Invoke(endTime,
                                          this,
                                          Request,
                                          response,
                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetFileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFile                    (Request)

        /// <summary>
        /// Request the given file from the charging station.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        public async Task<CS.SendFileResponse>
            SendFile(SendFileRequest Request)

        {

            #region Send OnSendFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileRequest?.Invoke(startTime,
                                          this,
                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendFileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomSendFileRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendFile(Request)

                                      : new CS.SendFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SendFileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSendFileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnSendFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendFileResponse?.Invoke(endTime,
                                           this,
                                           Request,
                                           response,
                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendFileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteFile                  (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        public async Task<CS.DeleteFileResponse>
            DeleteFile(DeleteFileRequest Request)

        {

            #region Send OnDeleteFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteFileRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteFileRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDeleteFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteFile(Request)

                                      : new CS.DeleteFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DeleteFileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDeleteFileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteFileResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteFileResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy          (Request)

        /// <summary>
        /// Add a signature policy.
        /// </summary>
        /// <param name="Request">An AddSignaturePolicy request.</param>
        public async Task<CS.AddSignaturePolicyResponse>
            AddSignaturePolicy(AddSignaturePolicyRequest Request)

        {

            #region Send OnAddSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAddSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomAddSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.AddSignaturePolicy(Request)

                                      : new CS.AddSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.AddSignaturePolicyResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomAddSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAddSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAddSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateSignaturePolicy       (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A UpdateSignaturePolicy request.</param>
        public async Task<CS.UpdateSignaturePolicyResponse>
            UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)

        {

            #region Send OnUpdateSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomUpdateSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateSignaturePolicy(Request)

                                      : new CS.UpdateSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateSignaturePolicyResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomUpdateSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteSignaturePolicy       (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A DeleteSignaturePolicy request.</param>
        public async Task<CS.DeleteSignaturePolicyResponse>
            DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)

        {

            #region Send OnDeleteSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomDeleteSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteSignaturePolicy(Request)

                                      : new CS.DeleteSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DeleteSignaturePolicyResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomDeleteSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region AddUserRole                 (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A AddUserRole request.</param>
        public async Task<CS.AddUserRoleResponse>
            AddUserRole(AddUserRoleRequest Request)

        {

            #region Send OnAddUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAddUserRoleRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomAddUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.AddUserRole(Request)

                                      : new CS.AddUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.AddUserRoleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomAddUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAddUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAddUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAddUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateUserRole              (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A UpdateUserRole request.</param>
        public async Task<CS.UpdateUserRoleResponse>
            UpdateUserRole(UpdateUserRoleRequest Request)

        {

            #region Send OnUpdateUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateUserRoleRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomUpdateUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateUserRole(Request)

                                      : new CS.UpdateUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateUserRoleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomUpdateUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteUserRole              (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A DeleteUserRole request.</param>
        public async Task<CS.DeleteUserRoleResponse>
            DeleteUserRole(DeleteUserRoleRequest Request)

        {

            #region Send OnDeleteUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteUserRoleRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomDeleteUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteUserRole(Request)

                                      : new CS.DeleteUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DeleteUserRoleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomDeleteUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff    (Request)

        /// <summary>
        /// Set a default charging tariff for the charging station,
        /// or for a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="Request">An SetDefaultChargingTariff request.</param>
        public async Task<CS.SetDefaultChargingTariffResponse>
            SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)

        {

            #region Send OnSetDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDefaultChargingTariffRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDefaultChargingTariffRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetDefaultChargingTariffRequestSerializer,
                                          CustomChargingTariffSerializer,
                                          CustomPriceSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTaxRateSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomIdTokenSerializer,
                                          CustomAdditionalInfoSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetDefaultChargingTariff(Request)

                                      : new CS.SetDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetDefaultChargingTariffResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetDefaultChargingTariffResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomEVSEStatusInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetDefaultChargingTariffResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDefaultChargingTariff    (Request)

        /// <summary>
        /// Get the default charging tariff(s) for the charging station and its EVSEs.
        /// </summary>
        /// <param name="Request">An GetDefaultChargingTariff request.</param>
        public async Task<CS.GetDefaultChargingTariffResponse>
            GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)

        {

            #region Send OnGetDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDefaultChargingTariffRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetDefaultChargingTariffRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetDefaultChargingTariff(Request)

                                      : new CS.GetDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetDefaultChargingTariffResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetDefaultChargingTariffResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomChargingTariffSerializer,
                    CustomPriceSerializer,
                    CustomTariffElementSerializer,
                    CustomPriceComponentSerializer,
                    CustomTaxRateSerializer,
                    CustomTariffRestrictionsSerializer,
                    CustomEnergyMixSerializer,
                    CustomEnergySourceSerializer,
                    CustomEnvironmentalImpactSerializer,
                    CustomIdTokenSerializer,
                    CustomAdditionalInfoSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoveDefaultChargingTariff (Request)

        /// <summary>
        /// Remove the default charging tariff of the charging station,
        /// or of a subset of EVSEs of the charging station.
        /// </summary>
        /// <param name="Request">An RemoveDefaultChargingTariff request.</param>
        public async Task<CS.RemoveDefaultChargingTariffResponse>
            RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)

        {

            #region Send OnRemoveDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRemoveDefaultChargingTariffRequest));
            }

            #endregion


            var response  = reachableChargingStations.TryGetValue(Request.ChargingStationId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRemoveDefaultChargingTariffRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.RemoveDefaultChargingTariff(Request)

                                      : new CS.RemoveDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RemoveDefaultChargingTariffResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charging station!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomRemoveDefaultChargingTariffResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomEVSEStatusInfoSerializer2,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnRemoveDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffResponse?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRemoveDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #endregion


        #region AddOrUpdateHTTPBasicAuth(ChargeBoxId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of the charging station.</param>
        /// <param name="Password">The password of the charging station.</param>
        public void AddOrUpdateHTTPBasicAuth(ChargingStation_Id  ChargeBoxId,
                                             String        Password)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CSMSWSServer centralSystemWSServer)
                {
                    centralSystemWSServer.AddOrUpdateHTTPBasicAuth(ChargeBoxId, Password);
                }
            }

        }

        #endregion

        #region RemoveHTTPBasicAuth(ChargeBoxId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given charging station.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of the charging station.</param>
        public void RemoveHTTPBasicAuth(ChargingStation_Id ChargeBoxId)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CSMSWSServer centralSystemWSServer)
                {
                    centralSystemWSServer.RemoveHTTPBasicAuth(ChargeBoxId);
                }
            }

        }

        #endregion

        #region ChargeBoxes

        #region Data

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        protected internal readonly ConcurrentDictionary<ChargingStation_Id, ChargeBox> chargeBoxes = new();

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        public IEnumerable<ChargeBox> ChargeBoxes
            => chargeBoxes.Values;

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(ChargeBox,                      MessageType,    OldChargeBox = null, ...)

        ///// <summary>
        ///// Write the given chargeBox to the database and send out notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charging station.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async Task WriteToDatabaseFileAndNotify(ChargeBox             ChargeBox,
        //                                                           NotificationMessageType  MessageType,
        //                                                           ChargeBox             OldChargeBox   = null,
        //                                                           EventTracking_Id         EventTrackingId   = null,
        //                                                           User_Id?                 CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    if (MessageType.IsNullOrEmpty)
        //        throw new ArgumentNullException(nameof(MessageType),   "The given message type must not be null or empty!");


        //    var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

        //    await WriteToDatabaseFile(MessageType,
        //                              ChargeBox.ToJSON(false, true),
        //                              eventTrackingId,
        //                              CurrentUserId);

        //    await SendNotifications(ChargeBox,
        //                            MessageType,
        //                            OldChargeBox,
        //                            eventTrackingId,
        //                            CurrentUserId);

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox,                      MessageType(s), OldChargeBox = null, ...)

        //protected virtual String ChargeBoxHTMLInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Name.FirstText(), "</a> ",
        //                                        "(<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>)")
        //                         : String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>"));

        //protected virtual String ChargeBoxTextInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("'", ChargeBox.Name.FirstText(), "' (", ChargeBox.Id, ")")
        //                         : String.Concat("'", ChargeBox.Id.ToString(), "'"));


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charging station.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox             ChargeBox,
        //                                                  NotificationMessageType  MessageType,
        //                                                  ChargeBox             OldChargeBox   = null,
        //                                                  EventTracking_Id         EventTrackingId   = null,
        //                                                  User_Id?                 CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         new NotificationMessageType[] { MessageType },
        //                         OldChargeBox,
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charging station.</param>
        ///// <param name="MessageTypes">The chargeBox notifications.</param>
        ///// <param name="OldChargeBox">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        ChargeBox                          OldChargeBox   = null,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

        //    if (messageTypesHash.Contains(addChargeBoxIfNotExists_MessageType))
        //        messageTypesHash.Add(addChargeBox_MessageType);

        //    if (messageTypesHash.Contains(addOrUpdateChargeBox_MessageType))
        //        messageTypesHash.Add(OldChargeBox == null
        //                               ? addChargeBox_MessageType
        //                               : updateChargeBox_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    ComparizionResult? comparizionResult = null;

        //    if (messageTypes.Contains(updateChargeBox_MessageType))
        //        comparizionResult = ChargeBox.CompareWith(OldChargeBox);


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ChargeBox.GetNotificationsOf<TelegramNotification>(messageTypes).
        //                                                     ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.\n" + comparizionResult?.ToTelegram(),
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications  = ChargeBox.GetNotificationsOf<SMSNotification>(messageTypes).
        //                                                    ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' was successfully created. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' information had been successfully updated. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                                          // + {Updated information}
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications  = ChargeBox.GetNotificationsOf<HTTPSNotification>(messageTypes).
        //                                                      ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxCreated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxUpdated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications  = ChargeBox.GetNotificationsOf<EMailNotification>(messageTypes).
        //                                                          ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " was successfully created",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " was successfully created.\r\n",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.<br /><br />",
        //                                                                    comparizionResult?.ToHTML() ?? "",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated.\r\r\r\r",
        //                                                                    comparizionResult?.ToText() ?? "",
        //                                                                    "\r\r\r\r",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox, ParentChargeBoxes, MessageType(s), ...)

        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charging station.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charging stationes.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox               ChargeBox,
        //                                                  IEnumerable<ChargeBox>  ParentChargeBoxes,
        //                                                  NotificationMessageType    MessageType,
        //                                                  EventTracking_Id           EventTrackingId   = null,
        //                                                  User_Id?                   CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         ParentChargeBoxes,
        //                         new NotificationMessageType[] { MessageType },
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charging station.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charging stationes.</param>
        ///// <param name="MessageTypes">The user notifications.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<ChargeBox>             ParentChargeBoxes,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),         "The given chargeBox must not be null or empty!");

        //    if (ParentChargeBoxes is null)
        //        ParentChargeBoxes = new ChargeBox[0];

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),         "The given enumeration of message types must not be null or empty!");

        //    //if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
        //    //    messageTypesHash.Add(addUser_MessageType);

        //    //if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
        //    //    messageTypesHash.Add(OldChargeBox == null
        //    //                           ? addUser_MessageType
        //    //                           : updateUser_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ParentChargeBoxes.
        //                                                    SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                    SelectMany(edge   => edge.Source.GetNotificationsOf<TelegramNotification>(deleteChargeBox_MessageType)).
        //                                                    ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications = ParentChargeBoxes.
        //                                          SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                          SelectMany(edge   => edge.Source.GetNotificationsOf<SMSNotification>(deleteChargeBox_MessageType)).
        //                                          ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' has been deleted."),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications = ParentChargeBoxes.
        //                                            SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                            SelectMany(edge   => edge.Source.GetNotificationsOf<HTTPSNotification>(deleteChargeBox_MessageType)).
        //                                            ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxDeleted",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications = ParentChargeBoxes.
        //                                                SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                SelectMany(edge   => edge.Source.GetNotificationsOf<EMailNotification>(deleteChargeBox_MessageType)).
        //                                                ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                             new HTMLEMailBuilder() {

        //                                 From           = Robot.EMail,
        //                                 To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                 Passphrase     = APIPassphrase,
        //                                 Subject        = ChargeBoxTextInfo(ChargeBox) + " has been deleted",

        //                                 HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.<br />",
        //                                                                HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxTextInfo(ChargeBox) + " has been deleted.\r\n",
        //                                                                TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 SecurityLevel  = EMailSecurity.autosign

        //                             });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) GetChargeBoxSerializator (Request, ChargeBox)

        protected internal ChargeBoxToJSONDelegate GetChargeBoxSerializator(HTTPRequest  Request,
                                                                            User         User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (chargeBox,
                            embedded,
                            expandTags,
                            includeCryptoHash)

                            => chargeBox.ToJSON(embedded,
                                                expandTags,
                                                includeCryptoHash);

            }

        }

        #endregion


        #region AddChargeBox           (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was added.</param>
        /// <param name="ChargeBox">The added charging station.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxAddedDelegate(DateTime           Timestamp,
                                                      ChargeBox          ChargeBox,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                                      User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was added.
        /// </summary>
        public event OnChargeBoxAddedDelegate? OnChargeBoxAdded;


        #region (protected internal) _AddChargeBox(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxResult>

            _AddChargeBox(ChargeBox                             ChargeBox,
                          Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                          EventTracking_Id?                     EventTrackingId   = null,
                          User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API is not null && ChargeBox.API != this)
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"ChargeBox identification '{ChargeBox.Id}' already exists!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                       nameof(ChargeBox),
            //                                       "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.TryAdd(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal is not null)
                await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                   ChargeBox,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBox_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddChargeBox                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charging station.
        /// </summary>
        /// <param name="ChargeBox">A new charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxResult>

            AddChargeBox(ChargeBox                             ChargeBox,
                         Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                         EventTracking_Id?                     EventTrackingId   = null,
                         User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBox(ChargeBox,
                                               OnAdded,
                                               eventTrackingId,
                                               CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        #region (protected internal) _AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxResult>

            _AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                     Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                     EventTracking_Id?                     EventTrackingId   = null,
                                     User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.NoOperation(
                           chargeBoxes[ChargeBox.Id],
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                  nameof(ChargeBox),
            //                                                  "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBoxIfNotExists_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.TryAdd(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal != null)
                await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                   ChargeBox,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBoxIfNotExists_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddChargeBoxIfNotExists                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charging station.
        /// </summary>
        /// <param name="ChargeBox">A new charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxResult>

            AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                    Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                    EventTracking_Id?                     EventTrackingId   = null,
                                    User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBoxIfNotExists(ChargeBox,
                                                          OnAdded,
                                                          eventTrackingId,
                                                          CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddOrUpdateChargeBox   (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateChargeBox(ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddOrUpdateChargeBoxResult>

            _AddOrUpdateChargeBox(ChargeBox                             ChargeBox,
                                  Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                  Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                     EventTrackingId   = null,
                                  User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddOrUpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargeBox.Id.Length < MinChargeBoxIdLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addOrUpdateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            if (chargeBoxes.TryGetValue(ChargeBox.Id, out var OldChargeBox))
            {
                chargeBoxes.TryRemove(OldChargeBox.Id, out _);
                ChargeBox.CopyAllLinkedDataFromBase(OldChargeBox);
            }

            chargeBoxes.TryAdd(ChargeBox.Id, ChargeBox);

            if (OldChargeBox is null)
            {

                OnAdded?.Invoke(ChargeBox,
                                eventTrackingId);

                var OnChargeBoxAddedLocal = OnChargeBoxAdded;
                if (OnChargeBoxAddedLocal != null)
                    await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                       ChargeBox,
                                                       eventTrackingId,
                                                       CurrentUserId);

                //await SendNotifications(ChargeBox,
                //                        addChargeBox_MessageType,
                //                        null,
                //                        eventTrackingId,
                //                        CurrentUserId);

                return AddOrUpdateChargeBoxResult.Added(
                           ChargeBox,
                           eventTrackingId,
                           Id,
                           this
                       );

            }

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal != null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                        ChargeBox,
                                                        OldChargeBox,
                                                        eventTrackingId,
                                                        CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        updateChargeBox_MessageType,
            //                        OldChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddOrUpdateChargeBoxResult.Updated(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddOrUpdateChargeBox                      (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddOrUpdateChargeBoxResult>

            AddOrUpdateChargeBox(ChargeBox                             ChargeBox,
                                 Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                 Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                 EventTracking_Id?                     EventTrackingId   = null,
                                 User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddOrUpdateChargeBox(ChargeBox,
                                                       OnAdded,
                                                       OnUpdated,
                                                       eventTrackingId,
                                                       CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddOrUpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return AddOrUpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region UpdateChargeBox        (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was updated.</param>
        /// <param name="ChargeBox">The updated charging station.</param>
        /// <param name="OldChargeBox">The old charging station.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxUpdatedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        ChargeBox          OldChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was updated.
        /// </summary>
        public event OnChargeBoxUpdatedDelegate? OnChargeBoxUpdated;


        #region (protected internal) _UpdateChargeBox(ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult>

            _UpdateChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_TryGetChargeBox(ChargeBox.Id, out var OldChargeBox))
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"The given chargeBox '{ChargeBox.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (ChargeBox.API != null && ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            ChargeBox.API = this;


            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.TryRemove(OldChargeBox.Id, out _);
            ChargeBox.CopyAllLinkedDataFromBase(OldChargeBox);
            chargeBoxes.TryAdd(ChargeBox.Id, ChargeBox);

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     OldChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        updateChargeBox_MessageType,
            //                        OldChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult>

            UpdateChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion


        #region (protected internal) _UpdateChargeBox(ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charging station.
        /// </summary>
        /// <param name="ChargeBox">An charging station.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult>

            _UpdateChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox.Builder>             UpdateDelegate,
                             Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_ChargeBoxExists(ChargeBox.Id))
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"The given chargeBox '{ChargeBox.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (UpdateDelegate is null)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given update delegate must not be null!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var builder = ChargeBox.ToBuilder();
            UpdateDelegate(builder);
            var updatedChargeBox = builder.ToImmutable;

            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          updatedChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.TryRemove(ChargeBox.Id, out _);
            updatedChargeBox.CopyAllLinkedDataFromBase(ChargeBox);
            chargeBoxes.TryAdd(updatedChargeBox.Id, updatedChargeBox);

            OnUpdated?.Invoke(updatedChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     updatedChargeBox,
                                                     ChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(updatedChargeBox,
            //                        updateChargeBox_MessageType,
            //                        ChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charging station.
        /// </summary>
        /// <param name="ChargeBox">An charging station.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult>

            UpdateChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox.Builder>             UpdateDelegate,
                            Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  UpdateDelegate,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region DeleteChargeBox        (ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was deleted.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was deleted.</param>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnChargeBoxDeletedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was deleted.
        /// </summary>
        public event OnChargeBoxDeletedDelegate? OnChargeBoxDeleted;


        #region (protected internal virtual) _CanDeleteChargeBox(ChargeBox)

        /// <summary>
        /// Determines whether the chargeBox can safely be deleted from the API.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        protected internal virtual I18NString? _CanDeleteChargeBox(ChargeBox ChargeBox)
        {

            //if (ChargeBox.Users.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has members!");

            //if (ChargeBox.SubChargeBoxes.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has sub chargeBoxs!");

            return null;

        }

        #endregion

        #region (protected internal) _DeleteChargeBox(ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charging station.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<DeleteChargeBoxResult>

            _DeleteChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != this)
                return DeleteChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (!chargeBoxes.TryGetValue(ChargeBox.Id, out var ChargeBoxToBeDeleted))
                return DeleteChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var veto = _CanDeleteChargeBox(ChargeBox);

            if (veto is not null)
                return DeleteChargeBoxResult.CanNotBeRemoved(
                           ChargeBox,
                           eventTrackingId,
                           Id,
                           this,
                           veto
                       );


            //// Get all parent chargeBoxs now, because later
            //// the --isChildOf--> edge will no longer be available!
            //var parentChargeBoxes = ChargeBox.GetAllParents(parent => parent != NoOwner).
            //                                       ToArray();


            //// Remove all: this --edge--> other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxOutEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Source,
            //                               edge.EdgeLabel,
            //                               edge.Target,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);

            //// Remove all: this <--edge-- other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxInEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Target,
            //                               edge.EdgeLabel,
            //                               edge.Source,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);


            //await WriteToDatabaseFile(deleteChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.TryRemove(ChargeBox.Id, out _);

            OnDeleted?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxDeletedLocal = OnChargeBoxDeleted;
            if (OnChargeBoxDeletedLocal is not null)
                await OnChargeBoxDeletedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        parentChargeBoxes,
            //                        deleteChargeBox_MessageType,
            //                        eventTrackingId,
            //                        CurrentUserId);


            return DeleteChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region DeleteChargeBox                      (ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charging station.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<DeleteChargeBoxResult>

            DeleteChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _DeleteChargeBox(ChargeBox,
                                                  OnDeleted,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return DeleteChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               Id,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return DeleteChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion


        #region ChargeBoxExists(ChargeBoxId)

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        protected internal Boolean _ChargeBoxExists(ChargingStation_Id ChargeBoxId)

            => ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.ContainsKey(ChargeBoxId);

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        protected internal Boolean _ChargeBoxExists(ChargingStation_Id? ChargeBoxId)

            => ChargeBoxId.IsNotNullOrEmpty() && chargeBoxes.ContainsKey(ChargeBoxId.Value);


        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        public Boolean ChargeBoxExists(ChargingStation_Id ChargeBoxId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        public Boolean ChargeBoxExists(ChargingStation_Id? ChargeBoxId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        #endregion

        #region GetChargeBox   (ChargeBoxId)

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        protected internal ChargeBox? _GetChargeBox(ChargingStation_Id ChargeBoxId)
        {

            if (ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(ChargeBoxId, out var chargeBox))
                return chargeBox;

            return default;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        protected internal ChargeBox? _GetChargeBox(ChargingStation_Id? ChargeBoxId)
        {

            if (ChargeBoxId is not null && chargeBoxes.TryGetValue(ChargeBoxId.Value, out var chargeBox))
                return chargeBox;

            return default;

        }


        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        public ChargeBox? GetChargeBox(ChargingStation_Id ChargeBoxId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        public ChargeBox? GetChargeBox(ChargingStation_Id? ChargeBoxId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(ChargeBoxId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        #endregion

        #region TryGetChargeBox(ChargeBoxId, out ChargeBox)

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        /// <param name="ChargeBox">The charging station.</param>
        protected internal Boolean _TryGetChargeBox(ChargingStation_Id    ChargeBoxId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(ChargeBoxId, out var chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        /// <param name="ChargeBox">The charging station.</param>
        protected internal Boolean _TryGetChargeBox(ChargingStation_Id?   ChargeBoxId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxId is not null && chargeBoxes.TryGetValue(ChargeBoxId.Value, out var chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }


        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        /// <param name="ChargeBox">The charging station.</param>
        public Boolean TryGetChargeBox(ChargingStation_Id    ChargeBoxId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(ChargeBoxId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="ChargeBoxId">The unique identification of an charging station.</param>
        /// <param name="ChargeBox">The charging station.</param>
        public Boolean TryGetChargeBox(ChargingStation_Id?   ChargeBoxId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(ChargeBoxId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargingStationSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        #endregion

        #endregion


        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }


    }

}
