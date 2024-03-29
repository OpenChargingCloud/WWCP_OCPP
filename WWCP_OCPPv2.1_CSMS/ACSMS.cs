﻿/*
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

using System.Reflection;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using BCx509 = Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Utilities;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An abstract Charging Station Management System (CSMS).
    /// </summary>
    public abstract class ACSMS : ICSMS,
                                  ICSMSWebSocket,
                                  IEventSender
    {

        #region Data

        private          readonly  HashSet<SignaturePolicy>                                                      signaturePolicies            = [];

        private          readonly  HashSet<CSMS.ICSMSChannel>                                                    csmsChannelServers           = [];

        private          readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<CSMS.ICSMSChannel, DateTime>>   connectedNetworkingNodes     = [];
        private          readonly  ConcurrentDictionary<NetworkingNode_Id, NetworkingNode_Id>                    reachableViaNetworkingHubs   = [];

        private          readonly  HTTPExtAPI                                                                    CSMSAPI;


        protected static readonly  SemaphoreSlim                                                                 ChargingStationSemaphore     = new (1, 1);

        protected static readonly  TimeSpan                                                                      SemaphoreSlimTimeout         = TimeSpan.FromSeconds(5);

        private                    Int64                                                                         internalRequestId            = 900000;

        private          readonly  TimeSpan                                                                      defaultRequestTimeout        = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public NetworkingNode_Id  Id        { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => this.Id.ToString();



        public WebAPI       WebAPI                    { get; }

        public UploadAPI    HTTPUploadAPI             { get; }

        public DownloadAPI  HTTPDownloadAPI           { get; }





        public DNSClient  DNSClient                 { get; }

        /// <summary>
        /// Require a HTTP Basic Authentication of all connecting networking nodes/charging stations.
        /// </summary>
        public Boolean    RequireAuthentication     { get; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan   DefaultRequestTimeout     { get; }

        /// <summary>
        /// The default charging station registration status.
        /// </summary>
        public RegistrationStatus  DefaultRegistrationStatus    { get; set; } = OCPPv2_1.RegistrationStatus.Rejected;



        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<CSMS.ICSMSIncomingMessages> CSMSServers
            => csmsChannelServers;


        public IEnumerable<CSMS.ICSMSChannel> CSMSChannels
            => csmsChannelServers;

        /// <summary>
        /// The unique identifications of all connected or reachable networking nodes.
        /// </summary>
        public IEnumerable<NetworkingNode_Id> ConnectedNetworkingNodeIds
            => connectedNetworkingNodes.Values.SelectMany(csmsChannel => csmsChannel.Item1.ConnectedNetworkingNodeIds);


        public Dictionary<String, Transaction_Id> TransactionIds = [];



        public AsymmetricCipherKeyPair?  ClientCAKeyPair        { get; }
        public BCx509.X509Certificate?   ClientCACertificate    { get; }


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

        public String?                       ClientCloseMessage
            => "Bye!";

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
        public event OnCSMSNewWebSocketConnectionDelegate?    OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        public event HTTPResponseLogDelegate?                 OnHTTPResponse;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCSMSCloseMessageReceivedDelegate?      OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnCSMSTCPConnectionClosedDelegate?       OnTCPConnectionClosed;

        /// <summary>
        /// An event sent whenever the HTTP web socket server stopped.
        /// </summary>
        public event OnServerStoppedDelegate?                 OnServerStopped;

        #endregion


        public event OnWebSocketFrameDelegate?          OnWebSocketFrameSent;
        public event OnWebSocketFrameDelegate?          OnWebSocketFrameReceived;

        public event OnWebSocketTextMessageDelegate?    OnTextMessageSent;
        public event OnWebSocketTextMessageDelegate?    OnTextMessageReceived;

        public event OnWebSocketBinaryMessageDelegate?  OnBinaryMessageSent;
        public event OnWebSocketBinaryMessageDelegate?  OnBinaryMessageReceived;

        public event OnWebSocketFrameDelegate?          OnPingMessageReceived;
        public event OnWebSocketFrameDelegate?          OnPingMessageSent;
        public event OnWebSocketFrameDelegate?          OnPongMessageReceived;


        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a JSON message was sent.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a JSON message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a JSON message request was received.
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

        #region OnBootNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request was sent from a charging station.
        /// </summary>
        public event OnBootNotificationRequestReceivedDelegate?   OnBootNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationResponseSentDelegate?  OnBootNotificationResponseSent;

        #endregion

        #region OnFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestReceivedDelegate?   OnFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseSentDelegate?  OnFirmwareStatusNotificationResponseSent;

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestReceivedDelegate?   OnPublishFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseSentDelegate?  OnPublishFirmwareStatusNotificationResponseSent;

        #endregion

        #region OnHeartbeat (Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request was sent from a charging station.
        /// </summary>
        public event OnHeartbeatRequestReceivedDelegate?   OnHeartbeatRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseSentDelegate?  OnHeartbeatResponseSent;

        #endregion

        #region OnNotifyEvent (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEvent request was sent from a charging station.
        /// </summary>
        public event OnNotifyEventRequestReceivedDelegate?   OnNotifyEventRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OnNotifyEventResponseSentDelegate?  OnNotifyEventResponseSent;

        #endregion

        #region OnSecurityEventNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request was sent from a charging station.
        /// </summary>
        public event OnSecurityEventNotificationRequestReceivedDelegate?   OnSecurityEventNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseSentDelegate?  OnSecurityEventNotificationResponseSent;

        #endregion

        #region OnNotifyReport (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyReport request was sent from a charging station.
        /// </summary>
        public event OnNotifyReportRequestReceivedDelegate?   OnNotifyReportRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OnNotifyReportResponseSentDelegate?  OnNotifyReportResponseSent;

        #endregion

        #region OnNotifyMonitoringReport (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request was sent from a charging station.
        /// </summary>
        public event OnNotifyMonitoringReportRequestReceivedDelegate?    OnNotifyMonitoringReportRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseSentDelegate?       OnNotifyMonitoringReportResponseSent;

        #endregion

        #region OnLogStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request was sent from a charging station.
        /// </summary>
        public event OnLogStatusNotificationRequestReceivedDelegate?    OnLogStatusNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseSentDelegate?       OnLogStatusNotificationResponseSent;

        #endregion

        #region OnIncomingDataTransfer (Request/-Response)

        /// <summary>
        /// An event sent whenever an IncomingDataTransfer request was received.
        /// </summary>
        public event OnDataTransferRequestReceivedDelegate?             OnDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an IncomingDataTransfer request was sent.
        /// </summary>
        public event OnDataTransferResponseSentDelegate?                OnDataTransferResponseSent;

        #endregion


        #region OnSignCertificate (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request was sent from a charging station.
        /// </summary>
        public event OnSignCertificateRequestReceivedDelegate?   OnSignCertificateRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateResponseSentDelegate?  OnSignCertificateResponseSent;

        #endregion

        #region OnGet15118EVCertificate (Request/-Response)

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request was sent from a charging station.
        /// </summary>
        public event OnGet15118EVCertificateRequestReceivedDelegate?   OnGet15118EVCertificateRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseSentDelegate?  OnGet15118EVCertificateResponseSent;

        #endregion

        #region OnGetCertificateStatus (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request was sent from a charging station.
        /// </summary>
        public event OnGetCertificateStatusRequestReceivedDelegate?   OnGetCertificateStatusRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseSentDelegate?  OnGetCertificateStatusResponseSent;

        #endregion

        #region OnGetCRL (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCRL request was sent from a charging station.
        /// </summary>
        public event OnGetCRLRequestReceivedDelegate?   OnGetCRLRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OnGetCRLResponseSentDelegate?  OnGetCRLResponseSent;

        #endregion


        #region OnReservationStatusUpdate (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request was sent from a charging station.
        /// </summary>
        public event OnReservationStatusUpdateRequestReceivedDelegate?   OnReservationStatusUpdateRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseSentDelegate?  OnReservationStatusUpdateResponseSent;

        #endregion

        #region OnAuthorize (Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request was sent from a charging station.
        /// </summary>
        public event OnAuthorizeRequestReceivedDelegate?   OnAuthorizeRequestReceived;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseSentDelegate?  OnAuthorizeResponseSent;

        #endregion

        #region OnNotifyEVChargingNeeds (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request was sent from a charging station.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestReceivedDelegate?   OnNotifyEVChargingNeedsRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseSentDelegate?  OnNotifyEVChargingNeedsResponseSent;

        #endregion

        #region OnTransactionEvent (Request/-Response)

        /// <summary>
        /// An event fired whenever a TransactionEvent was sent from a charging station.
        /// </summary>
        public event OnTransactionEventRequestReceivedDelegate?   OnTransactionEventRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventResponseSentDelegate?  OnTransactionEventResponseSent;

        #endregion

        #region OnStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request was sent from a charging station.
        /// </summary>
        public event OnStatusNotificationRequestReceivedDelegate?   OnStatusNotificationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationResponseSentDelegate?  OnStatusNotificationResponseSent;

        #endregion

        #region OnMeterValues (Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request was sent from a charging station.
        /// </summary>
        public event OnMeterValuesRequestReceivedDelegate?   OnMeterValuesRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesResponseSentDelegate?  OnMeterValuesResponseSent;

        #endregion

        #region OnNotifyChargingLimit (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request was sent from a charging station.
        /// </summary>
        public event OnNotifyChargingLimitRequestReceivedDelegate?   OnNotifyChargingLimitRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseSentDelegate?  OnNotifyChargingLimitResponseSent;

        #endregion

        #region OnClearedChargingLimit (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request was sent from a charging station.
        /// </summary>
        public event OnClearedChargingLimitRequestReceivedDelegate?   OnClearedChargingLimitRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseSentDelegate?  OnClearedChargingLimitResponseSent;

        #endregion

        #region OnReportChargingProfiles (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request was sent from a charging station.
        /// </summary>
        public event OnReportChargingProfilesRequestReceivedDelegate?   OnReportChargingProfilesRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseSentDelegate?  OnReportChargingProfilesResponseSent;

        #endregion

        #region OnNotifyEVChargingSchedule (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request was sent from a charging station.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestReceivedDelegate?   OnNotifyEVChargingScheduleRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseSentDelegate?  OnNotifyEVChargingScheduleResponseSent;

        #endregion

        #region OnNotifyPriorityCharging (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request was sent from a charging station.
        /// </summary>
        public event OnNotifyPriorityChargingRequestReceivedDelegate?   OnNotifyPriorityChargingRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseSentDelegate?  OnNotifyPriorityChargingResponseSent;

        #endregion

        #region OnNotifySettlement (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifySettlement request was sent from a charging station.
        /// </summary>
        public event OnNotifySettlementRequestReceivedDelegate?   OnNotifySettlementRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifySettlement request was received.
        /// </summary>
        public event OnNotifySettlementResponseSentDelegate?  OnNotifySettlementResponseSent;

        #endregion

        #region OnPullDynamicScheduleUpdate (Request/-Response)

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request was sent from a charging station.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestReceivedDelegate?   OnPullDynamicScheduleUpdateRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseSentDelegate?  OnPullDynamicScheduleUpdateResponseSent;

        #endregion


        #region OnNotifyDisplayMessages (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request was sent from a charging station.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestReceivedDelegate?   OnNotifyDisplayMessagesRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseSentDelegate?  OnNotifyDisplayMessagesResponseSent;

        #endregion

        #region OnNotifyCustomerInformation (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request was sent from a charging station.
        /// </summary>
        public event OnNotifyCustomerInformationRequestReceivedDelegate?   OnNotifyCustomerInformationRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseSentDelegate?  OnNotifyCustomerInformationResponseSent;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer (Request/-Response)

        /// <summary>
        /// An event sent whenever an IncomingBinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferRequestReceivedDelegate?   OnBinaryDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseSentDelegate?  OnBinaryDataTransferResponseSent;

        #endregion


        // Overlay Networking Extensions

        #region OnIncomingBinaryDataTransfer (Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnIncomingNotifyNetworkTopologyRequestDelegate?   OnIncomingNotifyNetworkTopologyRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnIncomingNotifyNetworkTopologyResponseDelegate?  OnIncomingNotifyNetworkTopologyResponse;

        #endregion

        #endregion

        #region CSMS -> Charging Station Messages

        #region OnReset                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the charging station.
        /// </summary>
        public event OnResetRequestSentDelegate?   OnResetRequestSent;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        public event OnResetResponseReceivedDelegate?  OnResetResponseReceived;

        #endregion

        #region OnUpdateFirmware              (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUpdateFirmwareRequestSentDelegate?   OnUpdateFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseReceivedDelegate?  OnUpdateFirmwareResponseReceived;

        #endregion

        #region OnPublishFirmware             (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnPublishFirmwareRequestSentDelegate?   OnPublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareResponseReceivedDelegate?  OnPublishFirmwareResponseReceived;

        #endregion

        #region OnUnpublishFirmware           (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OnUnpublishFirmwareRequestSentDelegate?   OnUnpublishFirmwareRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseReceivedDelegate?  OnUnpublishFirmwareResponseReceived;

        #endregion

        #region OnGetBaseReport               (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the charging station.
        /// </summary>
        public event OnGetBaseReportRequestSentDelegate?   OnGetBaseReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportResponseReceivedDelegate?  OnGetBaseReportResponseReceived;

        #endregion

        #region OnGetReport                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the charging station.
        /// </summary>
        public event OnGetReportRequestSentDelegate?   OnGetReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OnGetReportResponseReceivedDelegate?  OnGetReportResponseReceived;

        #endregion

        #region OnGetLog                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the charging station.
        /// </summary>
        public event OnGetLogRequestSentDelegate?   OnGetLogRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OnGetLogResponseReceivedDelegate?  OnGetLogResponseReceived;

        #endregion

        #region OnSetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the charging station.
        /// </summary>
        public event OnSetVariablesRequestSentDelegate?   OnSetVariablesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesResponseReceivedDelegate?  OnSetVariablesResponseReceived;

        #endregion

        #region OnGetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the charging station.
        /// </summary>
        public event OnGetVariablesRequestSentDelegate?   OnGetVariablesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OnGetVariablesResponseReceivedDelegate?  OnGetVariablesResponseReceived;

        #endregion

        #region OnSetMonitoringBase           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringBaseRequestSentDelegate?   OnSetMonitoringBaseRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OnSetMonitoringBaseResponseReceivedDelegate?  OnSetMonitoringBaseResponseReceived;

        #endregion

        #region OnGetMonitoringReport         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
        /// </summary>
        public event OnGetMonitoringReportRequestSentDelegate?   OnGetMonitoringReportRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportResponseReceivedDelegate?  OnGetMonitoringReportResponseReceived;

        #endregion

        #region OnSetMonitoringLevel          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
        /// </summary>
        public event OnSetMonitoringLevelRequestSentDelegate?   OnSetMonitoringLevelRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OnSetMonitoringLevelResponseReceivedDelegate?  OnSetMonitoringLevelResponseReceived;

        #endregion

        #region SetVariableMonitoring         (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnSetVariableMonitoringRequestSentDelegate?   OnSetVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringResponseReceivedDelegate?  OnSetVariableMonitoringResponseReceived;

        #endregion

        #region OnClearVariableMonitoring     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OnClearVariableMonitoringRequestSentDelegate?   OnClearVariableMonitoringRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseReceivedDelegate?  OnClearVariableMonitoringResponseReceived;

        #endregion

        #region OnSetNetworkProfile           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetNetworkProfileRequestSentDelegate?   OnSetNetworkProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OnSetNetworkProfileResponseReceivedDelegate?  OnSetNetworkProfileResponseReceived;

        #endregion

        #region OnChangeAvailability          (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
        /// </summary>
        public event OnChangeAvailabilityRequestSentDelegate?   OnChangeAvailabilityRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseReceivedDelegate?  OnChangeAvailabilityResponseReceived;

        #endregion

        #region OnTriggerMessage              (Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the charging station.
        /// </summary>
        public event OnTriggerMessageRequestSentDelegate?   OnTriggerMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        public event OnTriggerMessageResponseReceivedDelegate?  OnTriggerMessageResponseReceived;

        #endregion

        #region OnDataTransfer                (Request/-Response)

        /// <summary>
        /// An event sent whenever a DataTransfer request will be sent to the charging station.
        /// </summary>
        public event OnDataTransferRequestSentDelegate?   OnDataTransferRequestSent;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferResponseReceivedDelegate?  OnDataTransferResponseReceived;

        #endregion


        #region OnCertificateSigned           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the charging station.
        /// </summary>
        public event OnCertificateSignedRequestSentDelegate?   OnCertificateSignedRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OnCertificateSignedResponseReceivedDelegate?  OnCertificateSignedResponseReceived;

        #endregion

        #region OnInstallCertificate          (Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the charging station.
        /// </summary>
        public event OnInstallCertificateRequestSentDelegate?   OnInstallCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OnInstallCertificateResponseReceivedDelegate?  OnInstallCertificateResponseReceived;

        #endregion

        #region OnGetInstalledCertificateIds  (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestSentDelegate?   OnGetInstalledCertificateIdsRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseReceivedDelegate?  OnGetInstalledCertificateIdsResponseReceived;

        #endregion

        #region OnDeleteCertificate           (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
        /// </summary>
        public event OnDeleteCertificateRequestSentDelegate?   OnDeleteCertificateRequestSent;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OnDeleteCertificateResponseReceivedDelegate?  OnDeleteCertificateResponseReceived;

        #endregion

        #region OnNotifyCRL                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the charging station.
        /// </summary>
        public event OnNotifyCRLRequestSentDelegate?   OnNotifyCRLRequestSent;

        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLResponseReceivedDelegate?  OnNotifyCRLResponseReceived;

        #endregion


        #region OnGetLocalListVersion         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
        /// </summary>
        public event OnGetLocalListVersionRequestSentDelegate?   OnGetLocalListVersionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseReceivedDelegate?  OnGetLocalListVersionResponseReceived;

        #endregion

        #region OnSendLocalList               (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the charging station.
        /// </summary>
        public event OnSendLocalListRequestSentDelegate?   OnSendLocalListRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseReceivedDelegate?  OnSendLocalListResponseReceived;

        #endregion

        #region OnClearCache                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the charging station.
        /// </summary>
        public event OnClearCacheRequestSentDelegate?   OnClearCacheRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OnClearCacheResponseReceivedDelegate?  OnClearCacheResponseReceived;

        #endregion


        #region OnReserveNow                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the charging station.
        /// </summary>
        public event OnReserveNowRequestSentDelegate?   OnReserveNowRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowResponseReceivedDelegate?  OnReserveNowResponseReceived;

        #endregion

        #region OnCancelReservation           (Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the charging station.
        /// </summary>
        public event OnCancelReservationRequestSentDelegate?   OnCancelReservationRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationResponseReceivedDelegate?  OnCancelReservationResponseReceived;

        #endregion

        #region OnRequestStartTransaction     (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStartTransactionRequestSentDelegate?   OnRequestStartTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionResponseReceivedDelegate?  OnRequestStartTransactionResponseReceived;

        #endregion

        #region OnRequestStopTransaction      (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
        /// </summary>
        public event OnRequestStopTransactionRequestSentDelegate?   OnRequestStopTransactionRequestSent;

        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionResponseReceivedDelegate?  OnRequestStopTransactionResponseReceived;

        #endregion

        #region OnGetTransactionStatus        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
        /// </summary>
        public event OnGetTransactionStatusRequestSentDelegate?   OnGetTransactionStatusRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusResponseReceivedDelegate?  OnGetTransactionStatusResponseReceived;

        #endregion

        #region OnSetChargingProfile          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnSetChargingProfileRequestSentDelegate?   OnSetChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseReceivedDelegate?  OnSetChargingProfileResponseReceived;

        #endregion

        #region OnGetChargingProfiles         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
        /// </summary>
        public event OnGetChargingProfilesRequestSentDelegate?   OnGetChargingProfilesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OnGetChargingProfilesResponseReceivedDelegate?  OnGetChargingProfilesResponseReceived;

        #endregion

        #region OnClearChargingProfile        (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OnClearChargingProfileRequestSentDelegate?   OnClearChargingProfileRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseReceivedDelegate?  OnClearChargingProfileResponseReceived;

        #endregion

        #region OnGetCompositeSchedule        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
        /// </summary>
        public event OnGetCompositeScheduleRequestSentDelegate?   OnGetCompositeScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseReceivedDelegate?  OnGetCompositeScheduleResponseReceived;

        #endregion

        #region OnUpdateDynamicSchedule       (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to the charging station.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestSentDelegate?   OnUpdateDynamicScheduleRequestSent;

        /// <summary>
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseReceivedDelegate?  OnUpdateDynamicScheduleResponseReceived;

        #endregion

        #region OnNotifyAllowedEnergyTransfer (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestSentDelegate?   OnNotifyAllowedEnergyTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseReceivedDelegate?  OnNotifyAllowedEnergyTransferResponseReceived;

        #endregion

        #region OnUsePriorityCharging         (Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
        /// </summary>
        public event OnUsePriorityChargingRequestSentDelegate?   OnUsePriorityChargingRequestSent;

        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingResponseReceivedDelegate?  OnUsePriorityChargingResponseReceived;

        #endregion

        #region OnUnlockConnector             (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the charging station.
        /// </summary>
        public event OnUnlockConnectorRequestSentDelegate?   OnUnlockConnectorRequestSent;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseReceivedDelegate?  OnUnlockConnectorResponseReceived;

        #endregion


        #region OnAFRRSignal                  (Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRRSignal request will be sent to the charging station.
        /// </summary>
        public event OnAFRRSignalRequestSentDelegate?   OnAFRRSignalRequestSent;

        /// <summary>
        /// An event fired whenever a response to an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalResponseReceivedDelegate?  OnAFRRSignalResponseReceived;

        #endregion


        #region SetDisplayMessage/-Response   (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnSetDisplayMessageRequestSentDelegate?   OnSetDisplayMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OnSetDisplayMessageResponseReceivedDelegate?  OnSetDisplayMessageResponseReceived;

        #endregion

        #region OnGetDisplayMessages          (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
        /// </summary>
        public event OnGetDisplayMessagesRequestSentDelegate?   OnGetDisplayMessagesRequestSent;

        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseReceivedDelegate?  OnGetDisplayMessagesResponseReceived;

        #endregion

        #region OnClearDisplayMessage         (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OnClearDisplayMessageRequestSentDelegate?   OnClearDisplayMessageRequestSent;

        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OnClearDisplayMessageResponseReceivedDelegate?  OnClearDisplayMessageResponseReceived;

        #endregion

        #region OnCostUpdated                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the charging station.
        /// </summary>
        public event OnCostUpdatedRequestSentDelegate?   OnCostUpdatedRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OnCostUpdatedResponseReceivedDelegate?  OnCostUpdatedResponseReceived;

        #endregion

        #region OnCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the charging station.
        /// </summary>
        public event OnCustomerInformationRequestSentDelegate?   OnCustomerInformationRequestSent;

        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        public event OnCustomerInformationResponseReceivedDelegate?  OnCustomerInformationResponseReceived;

        #endregion


        // Binary Data Streams Extensions

        #region OnBinaryDataTransfer          (Request/-Response)

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request will be sent to the charging station.
        /// </summary>
        public event OnBinaryDataTransferRequestSentDelegate?       OnBinaryDataTransferRequestSent;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate?  OnBinaryDataTransferResponseReceived;

        #endregion

        #region OnGetFile                     (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetFile request will be sent to the charging station.
        /// </summary>
        public event OnGetFileRequestDelegate?   OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was received.
        /// </summary>
        public event OnGetFileResponseDelegate?  OnGetFileResponse;

        #endregion

        #region OnSendFile                    (Request/-Response)

        /// <summary>
        /// An event sent whenever a SendFile request will be sent to the charging station.
        /// </summary>
        public event OnSendFileRequestDelegate?   OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was received.
        /// </summary>
        public event OnSendFileResponseDelegate?  OnSendFileResponse;

        #endregion

        #region OnDeleteFile                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteFile request will be sent to the charging station.
        /// </summary>
        public event OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was received.
        /// </summary>
        public event OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        #endregion

        #region OnListDirectory               (Request/-Response)

        /// <summary>
        /// An event sent whenever a ListDirectory request will be sent to the charging station.
        /// </summary>
        public event OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was received.
        /// </summary>
        public event OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy            (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a AddSignaturePolicy request was received.
        /// </summary>
        public event OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        #endregion

        #region UpdateSignaturePolicy         (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        #endregion

        #region DeleteSignaturePolicy         (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        /// </summary>
        public event OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        #endregion

        #region AddUserRole                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddUserRole request will be sent to the charging station.
        /// </summary>
        public event OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a AddUserRole request was received.
        /// </summary>
        public event OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        #endregion

        #region UpdateUserRole                (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateUserRole request will be sent to the charging station.
        /// </summary>
        public event OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateUserRole request was received.
        /// </summary>
        public event OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        #endregion

        #region DeleteUserRole                (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteUserRole request will be sent to the charging station.
        /// </summary>
        public event OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        #endregion


        #region OnSecureDataTransfer          (Request/-Response)

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request will be sent to the charging station.
        /// </summary>
        public event OnSecureDataTransferRequestSentDelegate?       OnSecureDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate?  OnSecureDataTransferResponse;

        #endregion


        // E2E Charging Tariff Extensions

        #region SetDefaultChargingTariff      (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

        #endregion

        #region GetDefaultChargingTariff      (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDefaultChargingTariff request was received.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

        #endregion

        #region RemoveDefaultChargingTariff   (Request/-Response)

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
        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                                CustomListDirectoryRequestSerializer                         { get; set; }


        // E2E Security Extensions

        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }

        public CustomBinarySerializerDelegate<SecureDataTransferRequest>?                            CustomSecureDataTransferRequestSerializer              { get; set; }


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


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<OCPP.NN.NotifyNetworkTopologyResponse>?               CustomIncomingNotifyNetworkTopologyResponseSerializer        { get; set; }

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
        public CustomJObjectSerializerDelegate<   DataTransferRequest>?                              CustomIncomingDataTransferRequestSerializer                  { get; set; }

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
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                          CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<OCPP.NN.NotifyNetworkTopologyRequest>?               CustomIncomingNotifyNetworkTopologyRequestSerializer          { get; set; }

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
        public CustomJObjectSerializerDelegate<   DataTransferResponse>?                             CustomDataTransferResponseSerializer                         { get; set; }

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
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }
        public CustomBinarySerializerDelegate <OCPP.CS.GetFileResponse>?                             CustomGetFileResponseSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.SendFileResponse>?                            CustomSendFileResponseSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.DeleteFileResponse>?                          CustomDeleteFileResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.ListDirectoryResponse>?                       CustomListDirectoryResponseSerializer                        { get; set; }


        // E2E Security Extensions
        public CustomBinarySerializerDelegate<SecureDataTransferResponse>?                           CustomSecureDataTransferResponseSerializer                   { get; set; }


        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<CS.SetDefaultChargingTariffResponse>?                 CustomSetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.GetDefaultChargingTariffResponse>?                 CustomGetDefaultChargingTariffResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.RemoveDefaultChargingTariffResponse>?              CustomRemoveDefaultChargingTariffResponseSerializer          { get; set; }

        #endregion


        #region Data Structures

        public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                              { get; set; }
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
        public CustomBinarySerializerDelegate<OCPP.Signature>?                                       CustomBinarySignatureSerializer                        { get; set; }


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


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer             { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all connecting networking nodes/charging stations.</param>
        public ACSMS(NetworkingNode_Id         Id,
                     Boolean                   RequireAuthentication   = true,

                     IPPort?                   HTTPUploadPort          = null,
                     IPPort?                   HTTPDownloadPort        = null,

                     AsymmetricCipherKeyPair?  ClientCAKeyPair         = null,
                     BCx509.X509Certificate?   ClientCACertificate     = null,

                     SignaturePolicy?          SignaturePolicy         = null,

                     TimeSpan?                 DefaultRequestTimeout   = null,
                     DNSClient?                DNSClient               = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id), "The given central system identification must not be null or empty!");

            this.Id                      = Id;
            this.RequireAuthentication   = RequireAuthentication;
            this.DefaultRequestTimeout   = DefaultRequestTimeout ?? defaultRequestTimeout;

            this.ClientCAKeyPair         = ClientCAKeyPair;
            this.ClientCACertificate     = ClientCACertificate;

            this.DNSClient               = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));


            #region Setup generic HTTP API

            this.CSMSAPI           = new HTTPExtAPI(
                                         HTTPServerPort:         IPPort.Parse(3502),
                                         HTTPServerName:         "GraphDefined OCPP Test CSMS",
                                         HTTPServiceName:        "GraphDefined OCPP Test CSMS Service",
                                         APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test CSMS Robot <robot@charging.cloud>"),
                                         APIRobotGPGPassphrase:  "test123",
                                         SMTPClient:             new NullMailer(),
                                         DNSClient:              DNSClient,
                                         AutoStart:              true
                                     );

            #endregion

            var webAPIPrefix       = "webapi";
            var uploadAPIPrefix    = "uploads";
            var downloadAPIPrefix  = "downloads";

            #region Setup Web-/Upload-/DownloadAPIs

            this.WebAPI            = new WebAPI(
                                         this,
                                         CSMSAPI,

                                         URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

                                     );

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "UploadAPI"));

            this.HTTPUploadAPI     = new UploadAPI(
                                         this,
                                         HTTPUploadPort.HasValue
                                             ? new HTTPServer(
                                                   HTTPUploadPort.Value,
                                                   UploadAPI.DefaultHTTPServerName,
                                                   UploadAPI.DefaultHTTPServiceName
                                               )
                                             : CSMSAPI.HTTPServer,

                                         URLPathPrefix:   HTTPPath.Parse(uploadAPIPrefix),
                                         FileSystemPath:  Path.Combine(AppContext.BaseDirectory, "UploadAPI")

                                     );

            this.HTTPDownloadAPI   = new DownloadAPI(
                                         this,
                                         HTTPDownloadPort.HasValue
                                             ? new HTTPServer(
                                                   HTTPDownloadPort.Value,
                                                   DownloadAPI.DefaultHTTPServerName,
                                                   DownloadAPI.DefaultHTTPServiceName
                                               )
                                             : CSMSAPI.HTTPServer,

                                         URLPathPrefix:   HTTPPath.Parse(downloadAPIPrefix),
                                         FileSystemPath:  Path.Combine(AppContext.BaseDirectory, "DownloadAPI")

                                     );

            #endregion

            #region HTTP API Security Settings

            this.CSMSAPI.HTTPServer.AddAuth(request => {

                var ss1 = CSMSAPI.URLPathPrefix;
                var ss2 = CSMSAPI.URLPathPrefix + webAPIPrefix;


                // Allow some URLs for anonymous access...
                if (request.Path.StartsWith(CSMSAPI.URLPathPrefix + webAPIPrefix)    ||
                    request.Path.StartsWith(CSMSAPI.URLPathPrefix + HTTPUploadAPI.  URLPathPrefix) ||
                    request.Path.StartsWith(CSMSAPI.URLPathPrefix + HTTPDownloadAPI.URLPathPrefix))
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });

            #endregion


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


        public void ClearNetworkingNodes()
        {
            reachableViaNetworkingHubs.Clear();
            connectedNetworkingNodes.  Clear();
        }


        public Boolean LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId, out CSMS.ICSMSChannel? CSMSChannel)
        {

            var lookUpNetworkingNodeId = NetworkingNodeId;

            if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
                lookUpNetworkingNodeId = networkingHubId;

            if (connectedNetworkingNodes.  TryGetValue(lookUpNetworkingNodeId, out var csmsChannel) &&
                csmsChannel?.Item1 is not null)
            {
                CSMSChannel = csmsChannel.Item1;
                return true;
            }

            CSMSChannel = null;
            return false;

        }

        public void AddStaticRouting(NetworkingNode_Id DestinationNodeId,
                                     NetworkingNode_Id NetworkingHubId)
        {

            if (connectedNetworkingNodes.TryGetValue(NetworkingHubId, out var csmsChannel) &&
                csmsChannel?.Item1 is not null)
            {

                csmsChannel.Item1.AddStaticRouting(DestinationNodeId,
                                                   NetworkingHubId);

                reachableViaNetworkingHubs.TryAdd(DestinationNodeId,
                                                  NetworkingHubId);

            }

        }

        public void RemoveStaticRouting(NetworkingNode_Id DestinationNodeId,
                                        NetworkingNode_Id NetworkingHubId)
        {

            if (connectedNetworkingNodes.TryGetValue(NetworkingHubId, out var csmsChannel) &&
                csmsChannel?.Item1 is not null)
            {

                csmsChannel.Item1.RemoveStaticRouting(DestinationNodeId,
                                                      NetworkingHubId);

                reachableViaNetworkingHubs.TryRemove(new KeyValuePair<NetworkingNode_Id, NetworkingNode_Id>(DestinationNodeId, NetworkingHubId));

            }

        }



        #region AttachWebSocketService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        /// 
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSMSWSServer AttachWebSocketService(String                               HTTPServerName               = CSMSWSServer.DefaultHTTPServiceName,
                                                   IIPAddress?                          IPAddress                    = null,
                                                   IPPort?                              TCPPort                      = null,
                                                   I18NString?                          Description                  = null,

                                                   Boolean                              DisableWebSocketPings        = false,
                                                   TimeSpan?                            WebSocketPingEvery           = null,
                                                   TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                   Func<X509Certificate2>?              ServerCertificateSelector    = null,
                                                   RemoteCertificateValidationHandler?  ClientCertificateValidator   = null,
                                                   LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                   SslProtocols?                        AllowedTLSProtocols          = null,
                                                   Boolean?                             ClientCertificateRequired    = null,
                                                   Boolean?                             CheckCertificateRevocation   = null,

                                                   ServerThreadNameCreatorDelegate?     ServerThreadNameCreator      = null,
                                                   ServerThreadPriorityDelegate?        ServerThreadPrioritySetter   = null,
                                                   Boolean?                             ServerThreadIsBackground     = null,
                                                   ConnectionIdBuilder?                 ConnectionIdBuilder          = null,
                                                   TimeSpan?                            ConnectionTimeout            = null,
                                                   UInt32?                              MaxClientConnections         = null,

                                                   DNSClient?                           DNSClient                    = null,
                                                   Boolean                              AutoStart                    = false)
        {

            var csmsChannelServer = new CSMSWSServer(

                                        this,

                                        HTTPServerName,
                                        IPAddress,
                                        TCPPort,
                                        Description,

                                        RequireAuthentication,
                                        DisableWebSocketPings,
                                        WebSocketPingEvery,
                                        SlowNetworkSimulationDelay,

                                        ServerCertificateSelector,
                                        ClientCertificateValidator,
                                        ClientCertificateSelector,
                                        AllowedTLSProtocols,
                                        ClientCertificateRequired,
                                        CheckCertificateRevocation,

                                        ServerThreadNameCreator,
                                        ServerThreadPrioritySetter,
                                        ServerThreadIsBackground,
                                        ConnectionIdBuilder,
                                        ConnectionTimeout,
                                        MaxClientConnections,

                                        DNSClient: DNSClient ?? this.DNSClient,
                                        AutoStart: false

                                    );

            AttachCSMSChannel(csmsChannelServer);

            if (AutoStart)
                csmsChannelServer.Start();

            return csmsChannelServer;

        }

        #endregion

        #region (private) AttachCSMSChannel(CSMSChannel)

        private void AttachCSMSChannel(ICSMSWebsocketsChannel CSMSChannel)
        {

            csmsChannelServers.Add(CSMSChannel);


            #region WebSocket related

            //var iWebSocketServerEvents = CSMSChannel as IWebSocketServerEvents;

            #region OnServerStarted

            //iWebSocketServerEvents.OnServerStarted += async (timestamp,
            //                                                 server,
            //                                                 eventTrackingId,
            //                                                 cancellationToken) => {

            //    var onServerStarted = OnServerStarted;
            //    if (onServerStarted is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onServerStarted.GetInvocationList().
            //                                   OfType <OnServerStartedDelegate>().
            //                                   Select (loggingDelegate => loggingDelegate.Invoke(
            //                                                                  timestamp,
            //                                                                  server,
            //                                                                  eventTrackingId,
            //                                                                  cancellationToken
            //                                                              )).
            //                                   ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            await HandleErrors(
            //                      nameof(TestCSMS),
            //                      nameof(OnServerStarted),
            //                      e
            //                  );
            //        }
            //    }

            //};

            #endregion

            #region OnNewTCPConnection

            //iWebSocketServerEvents.OnNewTCPConnection += async (timestamp,
            //                                                    webSocketServer,
            //                                                    newTCPConnection,
            //                                                    eventTrackingId,
            //                                                    cancellationToken) => {

            //    var onNewTCPConnection = OnNewTCPConnection;
            //    if (onNewTCPConnection is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onNewTCPConnection.GetInvocationList().
            //                                   OfType <OnNewTCPConnectionDelegate>().
            //                                   Select (loggingDelegate => loggingDelegate.Invoke(
            //                                                                  timestamp,
            //                                                                  webSocketServer,
            //                                                                  newTCPConnection,
            //                                                                  eventTrackingId,
            //                                                                  cancellationToken
            //                                                              )).
            //                                   ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            await HandleErrors(
            //                      nameof(TestCSMS),
            //                      nameof(OnNewTCPConnection),
            //                      e
            //                  );
            //        }
            //    }

            //};

            #endregion

            // Failed (Charging Station) Authentication

            #region OnCSMSNewWebSocketConnection

            CSMSChannel.OnCSMSNewWebSocketConnection += async (timestamp,
                                                               csmsChannel,
                                                               newConnection,
                                                               networkingNodeId,
                                                               eventTrackingId,
                                                               sharedSubprotocols,
                                                               cancellationToken) => {

                // A new connection from the same networking node/charging station will replace the older one!
                if (!connectedNetworkingNodes.TryAdd(networkingNodeId, new Tuple<CSMS.ICSMSChannel, DateTime>(csmsChannel as CSMS.ICSMSChannel, timestamp)))
                    connectedNetworkingNodes[networkingNodeId]       = new Tuple<CSMS.ICSMSChannel, DateTime>(csmsChannel as CSMS.ICSMSChannel, timestamp);


                var onNewWebSocketConnection = OnNewWebSocketConnection;
                if (onNewWebSocketConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNewWebSocketConnection.GetInvocationList().
                                               OfType <OnCSMSNewWebSocketConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              csmsChannel,
                                                                              newConnection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              sharedSubprotocols,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

            #region OnCSMSCloseMessageReceived

            //CSMSChannel.OnCSMSCloseMessageReceived += async (timestamp,
            //                                                 server,
            //                                                 connection,
            //                                                 networkingNodeId,
            //                                                 eventTrackingId,
            //                                                 statusCode,
            //                                                 reason,
            //                                                 cancellationToken) => {

            //    var onCloseMessageReceived = OnCloseMessageReceived;
            //    if (onCloseMessageReceived is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onCloseMessageReceived.GetInvocationList().
            //                                   OfType <OnCSMSCloseMessageReceivedDelegate>().
            //                                   Select (loggingDelegate => loggingDelegate.Invoke(
            //                                                                  timestamp,
            //                                                                  server,
            //                                                                  connection,
            //                                                                  networkingNodeId,
            //                                                                  eventTrackingId,
            //                                                                  statusCode,
            //                                                                  reason,
            //                                                                  cancellationToken
            //                                                              )).
            //                                   ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            await HandleErrors(
            //                      nameof(TestCSMS),
            //                      nameof(OnCloseMessageReceived),
            //                      e
            //                  );
            //        }
            //    }

            //};

            #endregion

            #region OnCSMSTCPConnectionClosed

            //CSMSChannel.OnCSMSTCPConnectionClosed += async (timestamp,
            //                                                server,
            //                                                connection,
            //                                                networkingNodeId,
            //                                                eventTrackingId,
            //                                                reason,
            //                                                cancellationToken) => {

            //    var onTCPConnectionClosed = OnTCPConnectionClosed;
            //    if (onTCPConnectionClosed is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onTCPConnectionClosed.GetInvocationList().
            //                                   OfType <OnCSMSTCPConnectionClosedDelegate>().
            //                                   Select (loggingDelegate => loggingDelegate.Invoke(
            //                                                                  timestamp,
            //                                                                  server,
            //                                                                  connection,
            //                                                                  networkingNodeId,
            //                                                                  eventTrackingId,
            //                                                                  reason,
            //                                                                  cancellationToken
            //                                                              )).
            //                                   ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            await HandleErrors(
            //                      nameof(TestCSMS),
            //                      nameof(OnTCPConnectionClosed),
            //                      e
            //                  );
            //        }
            //    }

            //};

            #endregion

            #region OnServerStopped

            //CSMSChannel.OnServerStopped += async (timestamp,
            //                                      server,
            //                                      eventTrackingId,
            //                                      reason,
            //                                      cancellationToken) => {

            //    var onServerStopped = OnServerStopped;
            //    if (onServerStopped is not null)
            //    {
            //        try
            //        {

            //            await Task.WhenAll(onServerStopped.GetInvocationList().
            //                                     OfType <OnServerStoppedDelegate>().
            //                                     Select (loggingDelegate => loggingDelegate.Invoke(
            //                                                                    timestamp,
            //                                                                    server,
            //                                                                    eventTrackingId,
            //                                                                    reason,
            //                                                                    cancellationToken
            //                                                                )).
            //                                     ToArray());

            //        }
            //        catch (Exception e)
            //        {
            //            await HandleErrors(
            //                      nameof(TestCSMS),
            //                      nameof(OnServerStopped),
            //                      e
            //                  );
            //        }
            //    }

            //};

            #endregion


            // (Generic) Error Handling

            #endregion


            #region OnJSONMessageRequestReceived

            CSMSChannel.OnJSONMessageRequestReceived += async (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               networkingNodeId,
                                                               networkPath,
                                                               eventTrackingId,
                                                               requestTimestamp,
                                                               requestMessage,
                                                               cancellationToken) => {

                var onJSONMessageRequestReceived = OnJSONMessageRequestReceived;
                if (onJSONMessageRequestReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageRequestReceived.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              requestMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

            #region OnJSONMessageResponseSent

            CSMSChannel.OnJSONMessageResponseSent += async (timestamp,
                                                            webSocketServer,
                                                            webSocketConnection,
                                                            networkingNodeId,
                                                            networkPath,
                                                            eventTrackingId,
                                                            requestTimestamp,
                                                            jsonRequestMessage,
                                                            binaryRequestMessage,
                                                            responseTimestamp,
                                                            responseMessage,
                                                            cancellationToken) => {

                var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                if (onJSONMessageResponseSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

            #region OnJSONErrorResponseSent

            CSMSChannel.OnJSONErrorResponseSent += async (timestamp,
                                                          webSocketServer,
                                                          webSocketConnection,
                                                          eventTrackingId,
                                                          requestTimestamp,
                                                          jsonRequestMessage,
                                                          binaryRequestMessage,
                                                          responseTimestamp,
                                                          responseMessage,
                                                          cancellationToken) => {

                var onJSONErrorResponseSent = OnJSONErrorResponseSent;
                if (onJSONErrorResponseSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONErrorResponseSent.GetInvocationList().
                                               OfType <OnWebSocketTextErrorResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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


            #region OnJSONMessageRequestSent

            CSMSChannel.OnJSONMessageRequestSent += async (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           networkingNodeId,
                                                           networkPath,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           requestMessage,
                                                           cancellationToken) => {

                var onJSONMessageRequestSent = OnJSONMessageRequestSent;
                if (onJSONMessageRequestSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageRequestSent.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              requestMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

            #region OnJSONMessageResponseReceived

            CSMSChannel.OnJSONMessageResponseReceived += async (timestamp,
                                                                webSocketServer,
                                                                webSocketConnection,
                                                                networkingNodeId,
                                                                networkPath,
                                                                eventTrackingId,
                                                                requestTimestamp,
                                                                jsonRequestMessage,
                                                                binaryRequestMessage,
                                                                responseTimestamp,
                                                                responseMessage,
                                                                cancellationToken) => {

                var onJSONMessageResponseReceived = OnJSONMessageResponseReceived;
                if (onJSONMessageResponseReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageResponseReceived.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

            #region OnJSONErrorResponseReceived

            CSMSChannel.OnJSONErrorResponseReceived += async (timestamp,
                                                              webSocketServer,
                                                              webSocketConnection,
                                                              eventTrackingId,
                                                              requestTimestamp,
                                                              jsonRequestMessage,
                                                              binaryRequestMessage,
                                                              responseTimestamp,
                                                              responseMessage,
                                                              cancellationToken) => {

                var onJSONErrorResponseReceived = OnJSONErrorResponseReceived;
                if (onJSONErrorResponseReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONErrorResponseReceived.GetInvocationList().
                                               OfType <OnWebSocketTextErrorResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

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

                var startTime = Timestamp.Now;

                var onBootNotificationRequest = OnBootNotificationRequestReceived;
                if (onBootNotificationRequest is not null)
                {
                    try
                    {

                        await Task.WhenAll(onBootNotificationRequest.GetInvocationList().
                                               OfType <OnBootNotificationRequestReceivedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              startTime,
                                                                              this,
                                                                              connection,
                                                                              request
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnBootNotificationRequestReceived),
                                  e
                              );
                    }

                }

                #endregion


                DebugX.Log($"OnBootNotification: {request.ChargingStation?.SerialNumber ?? "-"} ({request.DestinationNodeId})");

                #region Verify request signature(s)

                var response = BootNotificationResponse.Failed(request);

                if (!SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            CustomBootNotificationRequestSerializer,
                            CustomChargingStationSerializer,
                            CustomSignatureSerializer,
                            CustomCustomDataSerializer
                        ),
                        out var errorResponse)) {

                    response = new BootNotificationResponse(
                                   Request:  request,
                                   Result:   Result.SignatureError(
                                                 $"Invalid signature(s): {errorResponse}"
                                             )
                               );

                }

                #endregion

                else
                {

                    // ChargingStation
                    // Reason

                    //await AddChargingStationIfNotExists(new ChargingStation(
                    //                                        Request.ChargingStationId,
                    //                                        1,
                    //                                        Request.ChargePointVendor,
                    //                                        Request.ChargePointModel,
                    //                                        null,
                    //                                        Request.ChargePointSerialNumber,
                    //                                        Request.ChargingStationSerialNumber,
                    //                                        Request.FirmwareVersion,
                    //                                        Request.Iccid,
                    //                                        Request.IMSI,
                    //                                        Request.MeterType,
                    //                                        Request.MeterSerialNumber
                    //                                    ));

                    response  = new BootNotificationResponse(
                                    Request:      request,
                                    Status:       DefaultRegistrationStatus, //RegistrationStatus.Accepted,
                                    CurrentTime:  Timestamp.Now,
                                    Interval:     TimeSpan.FromSeconds(30),
                                    StatusInfo:   null,
                                    CustomData:   null
                                );

                }

                #region Sign response message

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomBootNotificationResponseSerializer,
                        CustomStatusInfoSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);

                #endregion


                #region Send OnBootNotificationResponse event

                var endTime = Timestamp.Now;

                var onBootNotificationResponse = OnBootNotificationResponseSent;
                if (onBootNotificationResponse is not null)
                {
                    try
                    {

                        await Task.WhenAll(onBootNotificationResponse.GetInvocationList().
                                               OfType <OnBootNotificationResponseSentDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              endTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              endTime - startTime
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnBootNotificationResponseSent),
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
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnFirmwareStatusNotificationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnFirmwareStatusNotificationRequestReceivedDelegate>().
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
                                  nameof(OnFirmwareStatusNotificationRequestReceived),
                                  e
                              );
                    }

                }

                                                                  #endregion

                // Status
                // UpdateFirmwareRequestId

                DebugX.Log("OnFirmwareStatus: " + request.Status);


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

                var responseLogger = OnFirmwareStatusNotificationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnFirmwareStatusNotificationResponseSentDelegate>().
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
                                  nameof(OnFirmwareStatusNotificationResponseSent),
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
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                #region Send OnPublishFirmwareStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPublishFirmwareStatusNotificationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPublishFirmwareStatusNotificationRequestReceivedDelegate>().
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
                                  nameof(OnPublishFirmwareStatusNotificationRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // Status
                // PublishFirmwareStatusNotificationRequestId
                // DownloadLocations

                DebugX.Log("OnPublishFirmwareStatusNotification: " + request.DestinationNodeId);


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

                var responseLogger = OnPublishFirmwareStatusNotificationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPublishFirmwareStatusNotificationResponseSentDelegate>().
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
                                  nameof(OnPublishFirmwareStatusNotificationResponseSent),
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
                                              connection,
                                              request,
                                              cancellationToken) => {

                #region Send OnHeartbeatRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnHeartbeatRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnHeartbeatRequestReceivedDelegate>().
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
                                  nameof(OnHeartbeatRequestReceived),
                                  e
                              );
                    }

                }

                #endregion


                DebugX.Log("OnHeartbeat: " + request.DestinationNodeId);


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

                var responseLogger = OnHeartbeatResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnHeartbeatResponseSentDelegate>().
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
                                  nameof(OnHeartbeatResponseSent),
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
                                                connection,
                                                request,
                                                cancellationToken) => {

                #region Send OnNotifyEventRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEventRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEventRequestReceivedDelegate>().
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
                                  nameof(OnNotifyEventRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // GeneratedAt
                // SequenceNumber
                // EventData
                // ToBeContinued

                DebugX.Log("OnNotifyEvent: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyEventResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEventResponseSentDelegate>().
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
                                  nameof(OnNotifyEventResponseSent),
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
                                                              connection,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnSecurityEventNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSecurityEventNotificationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSecurityEventNotificationRequestReceivedDelegate>().
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
                                  nameof(OnSecurityEventNotificationRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // Type
                // Timestamp
                // TechInfo

                DebugX.Log("OnSecurityEventNotification: " + request.DestinationNodeId);


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

                var responseLogger = OnSecurityEventNotificationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSecurityEventNotificationResponseSentDelegate>().
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
                                  nameof(OnSecurityEventNotificationResponseSent),
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
                                                 connection,
                                                 request,
                                                 cancellationToken) => {

                #region Send OnNotifyReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyReportRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyReportRequestReceivedDelegate>().
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
                                  nameof(OnNotifyReportRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // NotifyReportRequestId
                // SequenceNumber
                // GeneratedAt
                // ReportData

                DebugX.Log("OnNotifyReport: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyReportResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyReportResponseSentDelegate>().
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
                                  nameof(OnNotifyReportResponseSent),
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
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnNotifyMonitoringReportRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyMonitoringReportRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyMonitoringReportRequestReceivedDelegate>().
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
                                  nameof(OnNotifyMonitoringReportRequestReceived),
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

                DebugX.Log("OnNotifyMonitoringReport: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyMonitoringReportResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyMonitoringReportResponseSentDelegate>().
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
                                  nameof(OnNotifyMonitoringReportResponseSent),
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
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnLogStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnLogStatusNotificationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnLogStatusNotificationRequestReceivedDelegate>().
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
                                  nameof(OnLogStatusNotificationRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // Status
                // LogRquestId

                DebugX.Log("OnLogStatusNotification: " + request.DestinationNodeId);


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

                var responseLogger = OnLogStatusNotificationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnLogStatusNotificationResponseSentDelegate>().
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
                                  nameof(OnLogStatusNotificationResponseSent),
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
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnDataTransferRequestReceived event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnDataTransferRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnDataTransferRequestReceivedDelegate>().
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
                                  nameof(OnSetDisplayMessageRequestSent),
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


                #region Send OnDataTransferResponseSent event

                var responseLogger = OnDataTransferResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnDataTransferResponseSentDelegate>().
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
                                  nameof(OnDataTransferResponseSent),
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
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                #region Send OnSignCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnSignCertificateRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnSignCertificateRequestReceivedDelegate>().
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
                                  nameof(OnSignCertificateRequestReceived),
                                  e
                              );
                    }

                }

                                                        #endregion


                // CSR
                // CertificateType

                DebugX.Log("OnSignCertificate: " + request.DestinationNodeId);

                Pkcs10CertificationRequest?  parsedCSR       = null;
                String?                      errorResponse   = null;

                if (!request.CertificateType.HasValue ||
                     request.CertificateType.Value == CertificateSigningUse.ChargingStationCertificate)
                {

                    try
                    {

                        using (var reader = new StringReader(request.CSR))
                        {
                            var pemReader = new PemReader(reader);
                            parsedCSR     = (Pkcs10CertificationRequest) pemReader.ReadObject();
                        }

                    } catch (Exception e)
                    {
                        errorResponse = "The certificate signing request could not be parsed: " + e.Message;
                    }

                    if (parsedCSR is null)
                        errorResponse = "The certificate signing request could not be parsed!";

                    else
                    {

                        if (!parsedCSR.Verify())
                            errorResponse = "The certificate signing request could not be verified!";

                        else if (ClientCAKeyPair     is null)
                            errorResponse = "No ClientCA key pair available!";

                        else if (ClientCACertificate is null)
                            errorResponse = "No ClientCA certificcate available!";

                        else
                        {

                            #region Sign the client certificate (background process!)

                            //ToDo: Find better ways to do this!
                            var delayedResponse = Task.Run(async () => {

                                try
                                {

                                    await Task.Delay(100);

                                    var secureRandom  = new SecureRandom();
                                    var now           = Timestamp.Now;

                                    var certificateGenerator = new X509V3CertificateGenerator();
                                    certificateGenerator.SetIssuerDN    (ClientCACertificate.SubjectDN);
                                    certificateGenerator.SetSubjectDN   (parsedCSR.GetCertificationRequestInfo().Subject);
                                    certificateGenerator.SetPublicKey   (parsedCSR.GetPublicKey());
                                    certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), secureRandom));
                                    certificateGenerator.SetNotBefore   (now.AddDays(-1));
                                    certificateGenerator.SetNotAfter    (now.AddMonths(3));

                                    certificateGenerator.AddExtension   (X509Extensions.KeyUsage,
                                                                         critical: true,
                                                                         new KeyUsage(
                                                                             KeyUsage.NonRepudiation |
                                                                             KeyUsage.DigitalSignature |
                                                                             KeyUsage.KeyEncipherment
                                                                         ));

                                    certificateGenerator.AddExtension   (X509Extensions.ExtendedKeyUsage,
                                                                         critical: true,
                                                                         new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth));

                                    var newClientCertificate = certificateGenerator.Generate(
                                                                   new Asn1SignatureFactory(
                                                                       "SHA256WithRSAEncryption",
                                                                       ClientCAKeyPair.Private,
                                                                       secureRandom
                                                                   )
                                                               );


                                    await Task.Delay(3000);


                                    await SendSignedCertificate(
                                              new CertificateSignedRequest(
                                                  request.NetworkPath.Source,
                                                  CertificateChain.From(ClientCACertificate, newClientCertificate),
                                                  request.CertificateType ?? CertificateSigningUse.ChargingStationCertificate
                                              )
                                          );

                                } catch (Exception e)
                                {
                                    DebugX.LogException(e, "The client certificate could not be signed!");
                                }

                            },
                            cancellationToken);

                            #endregion

                        }

                    }

                }
                else
                    errorResponse = $"Invalid CertificateSigningUse: '{request.CertificateType?.ToString() ?? "-"}'!";

                SignCertificateResponse? response = null;

                if (errorResponse != null)
                     response = new SignCertificateResponse(
                                    Request:   request,
                                    Result:    Result.GenericError(
                                                   errorResponse
                                               )
                                );

                else
                    response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomSignCertificateRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out errorResponse
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

                var responseLogger = OnSignCertificateResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnSignCertificateResponseSentDelegate>().
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
                                  nameof(OnSignCertificateResponseSent),
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
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnGet15118EVCertificateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGet15118EVCertificateRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGet15118EVCertificateRequestReceivedDelegate>().
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
                                  nameof(OnGet15118EVCertificateRequestReceived),
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

                DebugX.Log("OnGet15118EVCertificate: " + request.DestinationNodeId);


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

                var responseLogger = OnGet15118EVCertificateResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGet15118EVCertificateResponseSentDelegate>().
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
                                  nameof(OnGet15118EVCertificateResponseSent),
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
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnGetCertificateStatusRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCertificateStatusRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCertificateStatusRequestReceivedDelegate>().
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
                                  nameof(OnGetCertificateStatusRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // OCSPRequestData

                DebugX.Log("OnGetCertificateStatus: " + request.DestinationNodeId);


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
                                         Request:      request,
                                         Result:       Result.SignatureError(
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

                var responseLogger = OnGetCertificateStatusResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCertificateStatusResponseSentDelegate>().
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
                                  nameof(OnGetCertificateStatusResponseSent),
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
                                           connection,
                                           request,
                                           cancellationToken) => {

                #region Send OnGetCRLRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnGetCRLRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnGetCRLRequestReceivedDelegate>().
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
                                  nameof(OnGetCRLRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // GetCRLRequestId
                // CertificateHashData

                DebugX.Log("OnGetCRL: " + request.DestinationNodeId);


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

                var responseLogger = OnGetCRLResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnGetCRLResponseSentDelegate>().
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
                                  nameof(OnGetCRLResponseSent),
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
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                #region Send OnReservationStatusUpdateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReservationStatusUpdateRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReservationStatusUpdateRequestReceivedDelegate>().
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
                                  nameof(OnReservationStatusUpdateRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // ReservationId
                // ReservationUpdateStatus

                DebugX.Log("OnReservationStatusUpdate: " + request.DestinationNodeId);


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

                var responseLogger = OnReservationStatusUpdateResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReservationStatusUpdateResponseSentDelegate>().
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
                                  nameof(OnReservationStatusUpdateResponseSent),
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
                                              connection,
                                              request,
                                              cancellationToken) => {

                #region Send OnAuthorizeRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnAuthorizeRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnAuthorizeRequestReceivedDelegate>().
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
                                  nameof(OnAuthorizeRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // IdToken
                // Certificate
                // ISO15118CertificateHashData

                DebugX.Log("OnAuthorize: " + request.DestinationNodeId + ", " +
                                             request.IdToken);


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

                var responseLogger = OnAuthorizeResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnAuthorizeResponseSentDelegate>().
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
                                  nameof(OnAuthorizeResponseSent),
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
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnNotifyEVChargingNeedsRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEVChargingNeedsRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEVChargingNeedsRequestReceivedDelegate>().
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
                                  nameof(OnNotifyEVChargingNeedsRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // EVSEId
                // ChargingNeeds
                // MaxScheduleTuples

                DebugX.Log("OnNotifyEVChargingNeeds: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyEVChargingNeedsResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEVChargingNeedsResponseSentDelegate>().
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
                                  nameof(OnNotifyEVChargingNeedsResponseSent),
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
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                #region Send OnTransactionEventRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnTransactionEventRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnTransactionEventRequestReceivedDelegate>().
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
                                  nameof(OnTransactionEventRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingStationId
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

                DebugX.Log("OnTransactionEvent: " + request.DestinationNodeId + ", " +
                                                    request.IdToken);


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

                var responseLogger = OnTransactionEventResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnTransactionEventResponseSentDelegate>().
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
                                  nameof(OnTransactionEventResponseSent),
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
                                                       connection,
                                                       request,
                                                       cancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnStatusNotificationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnStatusNotificationRequestReceivedDelegate>().
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
                                  nameof(OnStatusNotificationRequestReceived),
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

                var responseLogger = OnStatusNotificationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnStatusNotificationResponseSentDelegate>().
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
                                  nameof(OnStatusNotificationResponseSent),
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
                                                connection,
                                                request,
                                                cancellationToken) => {

                #region Send OnMeterValuesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnMeterValuesRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnMeterValuesRequestReceivedDelegate>().
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
                                  nameof(OnMeterValuesRequestReceived),
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

                var responseLogger = OnMeterValuesResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnMeterValuesResponseSentDelegate>().
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
                                  nameof(OnMeterValuesResponseSent),
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
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                #region Send OnNotifyChargingLimitRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyChargingLimitRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyChargingLimitRequestReceivedDelegate>().
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
                                  nameof(OnNotifyChargingLimitRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingLimit
                // ChargingSchedules
                // EVSEId

                DebugX.Log("OnNotifyChargingLimit: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyChargingLimitResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyChargingLimitResponseSentDelegate>().
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
                                  nameof(OnNotifyChargingLimitResponseSent),
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
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                #region Send OnClearedChargingLimitRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnClearedChargingLimitRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnClearedChargingLimitRequestReceivedDelegate>().
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
                                  nameof(OnClearedChargingLimitRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingLimitSource
                // EVSEId

                DebugX.Log("OnClearedChargingLimit: " + request.DestinationNodeId);


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

                var responseLogger = OnClearedChargingLimitResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnClearedChargingLimitResponseSentDelegate>().
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
                                  nameof(OnClearedChargingLimitResponseSent),
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
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnReportChargingProfilesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnReportChargingProfilesRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnReportChargingProfilesRequestReceivedDelegate>().
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
                                  nameof(OnReportChargingProfilesRequestReceived),
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

                DebugX.Log("OnReportChargingProfiles: " + request.DestinationNodeId);


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

                var responseLogger = OnReportChargingProfilesResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnReportChargingProfilesResponseSentDelegate>().
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
                                  nameof(OnReportChargingProfilesResponseSent),
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
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                #region Send OnNotifyEVChargingScheduleRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyEVChargingScheduleRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyEVChargingScheduleRequestReceivedDelegate>().
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
                                  nameof(OnNotifyEVChargingScheduleRequestReceived),
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

                DebugX.Log("OnNotifyEVChargingSchedule: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyEVChargingScheduleResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyEVChargingScheduleResponseSentDelegate>().
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
                                  nameof(OnNotifyEVChargingScheduleResponseSent),
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
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                #region Send OnNotifyPriorityChargingRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyPriorityChargingRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyPriorityChargingRequestReceivedDelegate>().
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
                                  nameof(OnNotifyPriorityChargingRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // TransactionId
                // Activated

                DebugX.Log("OnNotifyPriorityCharging: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyPriorityChargingResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyPriorityChargingResponseSentDelegate>().
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
                                  nameof(OnNotifyPriorityChargingResponseSent),
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
                                                              connection,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnPullDynamicScheduleUpdateRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnPullDynamicScheduleUpdateRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnPullDynamicScheduleUpdateRequestReceivedDelegate>().
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
                                  nameof(OnPullDynamicScheduleUpdateRequestReceived),
                                  e
                              );
                    }

                }

                #endregion

                // ChargingProfileId

                DebugX.Log("OnPullDynamicScheduleUpdate: " + request.DestinationNodeId);


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

                var responseLogger = OnPullDynamicScheduleUpdateResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnPullDynamicScheduleUpdateResponseSentDelegate>().
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
                                  nameof(OnPullDynamicScheduleUpdateResponseSent),
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
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                #region Send OnNotifyDisplayMessagesRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyDisplayMessagesRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyDisplayMessagesRequestReceivedDelegate>().
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
                                  nameof(OnNotifyDisplayMessagesRequestReceived),
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

                var responseLogger = OnNotifyDisplayMessagesResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyDisplayMessagesResponseSentDelegate>().
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
                                  nameof(OnNotifyDisplayMessagesResponseSent),
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
                                                              connection,
                                                              request,
                                                              cancellationToken) => {

                #region Send OnNotifyCustomerInformationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnNotifyCustomerInformationRequestReceived;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OnNotifyCustomerInformationRequestReceivedDelegate>().
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
                                  nameof(OnNotifyCustomerInformationRequestReceived),
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

                DebugX.Log("OnNotifyCustomerInformation: " + request.DestinationNodeId);


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

                var responseLogger = OnNotifyCustomerInformationResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnNotifyCustomerInformationResponseSentDelegate>().
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
                                  nameof(OnNotifyCustomerInformationResponseSent),
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
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                #region Send OnIncomingBinaryDataTransferRequest event

                var startTime = Timestamp.Now;

                var onIncomingBinaryDataTransferRequest = OnBinaryDataTransferRequestReceived;
                if (onIncomingBinaryDataTransferRequest is not null)
                {
                    try
                    {

                        await Task.WhenAll(onIncomingBinaryDataTransferRequest.GetInvocationList().
                                               OfType <OnBinaryDataTransferRequestReceivedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                 this,
                                                                                                 connection,
                                                                                                 request)).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSetDisplayMessageRequestSent),
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


                var responseBinaryData = Array.Empty<byte>();

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

                var responseLogger = OnBinaryDataTransferResponseSent;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnBinaryDataTransferResponseSentDelegate>().
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
                                  nameof(OnBinaryDataTransferResponseSent),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion


            // Overlay Networking Extensions

            #region OnIncomingNotifyNetworkTopology

            CSMSChannel.OnIncomingNotifyNetworkTopology += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                #region Send OnIncomingNotifyNetworkTopologyRequest event

                var startTime = Timestamp.Now;

                var onIncomingNotifyNetworkTopologyRequest = OnIncomingNotifyNetworkTopologyRequest;
                if (onIncomingNotifyNetworkTopologyRequest is not null)
                {
                    try
                    {

                        await Task.WhenAll(onIncomingNotifyNetworkTopologyRequest.GetInvocationList().
                                               OfType <OnIncomingNotifyNetworkTopologyRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(startTime,
                                                                                                 this,
                                                                                                 connection,
                                                                                                 request)).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCSMS),
                                  nameof(OnSetDisplayMessageRequestSent),
                                  e
                              );
                    }
                }

                #endregion

                // NetworkTopologyInformation

                DebugX.Log("OnIncomingNotifyNetworkTopology: " + request.NetworkTopologyInformation);


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomIncomingNotifyNetworkTopologyRequestSerializer,
                                       CustomNetworkTopologyInformationSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new OCPP.NN.NotifyNetworkTopologyResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : new (
                                         Request:      request,
                                         Status:       NetworkTopologyStatus.Accepted,
                                         CustomData:   null
                                     );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomIncomingNotifyNetworkTopologyResponseSerializer,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnIncomingNotifyNetworkTopologyResponse event

                var responseLogger = OnIncomingNotifyNetworkTopologyResponse;
                if (responseLogger is not null)
                {

                    var responseTime         = Timestamp.Now;

                    var responseLoggerTasks  = responseLogger.GetInvocationList().
                                                              OfType <OnIncomingNotifyNetworkTopologyResponseDelegate>().
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
                                  nameof(OnIncomingNotifyNetworkTopologyResponse),
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


        #region CSMS -> Charging Station Messages and Responses

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

        public Byte[] GetEncryptionKey(NetworkingNode_Id  DestinationNodeId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }

        public Byte[] GetDecryptionKey(NetworkingNode_Id  DestinationNodeId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }


        public UInt64 GetEncryptionNonce(NetworkingNode_Id  DestinationNodeId,
                                         UInt16?            KeyId   = null)
        {
            return 1;
        }

        public UInt64 GetEncryptionCounter(NetworkingNode_Id  DestinationNodeId,
                                           UInt16?            KeyId   = null)
        {
            return 1;
        }



        #region Reset                       (Request)

        /// <summary>
        /// Reset the given charging station.
        /// </summary>
        /// <param name="Request">A Reset request.</param>
        public async Task<CS.ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequestSent?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnResetRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomResetRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.Reset(Request)

                                      : new CS.ResetResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ResetResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnResetResponseReceived?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnResetResponseReceived));
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
        public async Task<CS.UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequestSent?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateFirmwareRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.UpdateFirmware(Request)

                                      : new CS.UpdateFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateFirmwareResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnUpdateFirmwareResponseReceived?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateFirmwareResponseReceived));
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
        public async Task<CS.PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request)
        {

            #region Send OnPublishFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareRequestSent?.Invoke(startTime,
                                                 this,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnPublishFirmwareRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomPublishFirmwareRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.PublishFirmware(Request)

                                      : new CS.PublishFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.PublishFirmwareResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnPublishFirmwareResponseReceived?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnPublishFirmwareResponseReceived));
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
        public async Task<CS.UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request)
        {

            #region Send OnUnpublishFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnpublishFirmwareRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnpublishFirmwareRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUnpublishFirmwareRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.UnpublishFirmware(Request)

                                      : new CS.UnpublishFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UnpublishFirmwareResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnUnpublishFirmwareResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnpublishFirmwareResponseReceived));
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
        public async Task<CS.GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request)
        {

            #region Send OnGetBaseReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetBaseReportRequestSent?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetBaseReportRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetBaseReportRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetBaseReport(Request)

                                      : new CS.GetBaseReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetBaseReportResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetBaseReportResponseReceived?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetBaseReportResponseReceived));
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
        public async Task<CS.GetReportResponse> GetReport(GetReportRequest Request)
        {

            #region Send OnGetReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetReportRequestSent?.Invoke(startTime,
                                           this,
                                           Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetReportRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.GetReport(Request)

                                      : new CS.GetReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetReportResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetReportResponseReceived?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetReportResponseReceived));
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
        public async Task<CS.GetLogResponse> GetLog(GetLogRequest Request)
        {

            #region Send OnGetLogRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLogRequestSent?.Invoke(startTime,
                                        this,
                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLogRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.GetLog(Request)

                                      : new CS.GetLogResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetLogResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetLogResponseReceived?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLogResponseReceived));
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
        public async Task<CS.SetVariablesResponse> SetVariables(SetVariablesRequest Request)
        {

            #region Send OnSetVariablesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariablesRequestSent?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariablesRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetVariables(Request)

                                      : new CS.SetVariablesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetVariablesResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetVariablesResponseReceived?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariablesResponseReceived));
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
        public async Task<CS.GetVariablesResponse> GetVariables(GetVariablesRequest Request)
        {

            #region Send OnGetVariablesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetVariablesRequestSent?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetVariablesRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.GetVariables(Request)

                                      : new CS.GetVariablesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetVariablesResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetVariablesResponseReceived?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetVariablesResponseReceived));
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
        public async Task<CS.SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request)
        {

            #region Send OnSetMonitoringBaseRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetMonitoringBaseRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringBaseRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetMonitoringBaseRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SetMonitoringBase(Request)

                                      : new CS.SetMonitoringBaseResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetMonitoringBaseResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetMonitoringBaseResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringBaseResponseReceived));
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
        public async Task<CS.GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request)
        {

            #region Send OnGetMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetMonitoringReportRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetMonitoringReportRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.GetMonitoringReport(Request)

                                      : new CS.GetMonitoringReportResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetMonitoringReportResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetMonitoringReportResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetMonitoringReportResponseReceived));
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
        public async Task<CS.SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request)
        {

            #region Send OnSetMonitoringLevelRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetMonitoringLevelRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringLevelRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetMonitoringLevelRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SetMonitoringLevel(Request)

                                      : new CS.SetMonitoringLevelResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetMonitoringLevelResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetMonitoringLevelResponseReceived?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetMonitoringLevelResponseReceived));
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
        public async Task<CS.SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request)
        {

            #region Send OnSetVariableMonitoringRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariableMonitoringRequestSent?.Invoke(startTime,
                                                       this,
                                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariableMonitoringRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetVariableMonitoring(Request)

                                      : new CS.SetVariableMonitoringResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetVariableMonitoringResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetVariableMonitoringResponseReceived?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetVariableMonitoringResponseReceived));
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
        public async Task<CS.ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request)
        {

            #region Send OnClearVariableMonitoringRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearVariableMonitoringRequestSent?.Invoke(startTime,
                                                         this,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearVariableMonitoringRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearVariableMonitoringRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.ClearVariableMonitoring(Request)

                                      : new CS.ClearVariableMonitoringResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearVariableMonitoringResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnClearVariableMonitoringResponseReceived?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearVariableMonitoringResponseReceived));
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
        public async Task<CS.SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request)
        {

            #region Send OnSetNetworkProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetNetworkProfileRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetNetworkProfileRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetNetworkProfile(Request)

                                      : new CS.SetNetworkProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetNetworkProfileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetNetworkProfileResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetNetworkProfileResponseReceived));
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
        public async Task<CS.ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        {

            #region Send OnChangeAvailabilityRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnChangeAvailabilityRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.ChangeAvailability(Request)

                                      : new CS.ChangeAvailabilityResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ChangeAvailabilityResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnChangeAvailabilityResponseReceived?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnChangeAvailabilityResponseReceived));
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
        public async Task<CS.TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        {

            #region Send OnTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageRequestSent?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTriggerMessageRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.TriggerMessage(Request)

                                      : new CS.TriggerMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.TriggerMessageResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnTriggerMessageResponseReceived?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnTriggerMessageResponseReceived));
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
        public async Task<DataTransferResponse> DataTransfer(DataTransferRequest Request)
        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequestSent?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDataTransferRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDataTransferRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.DataTransfer(Request)

                                      : new DataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new DataTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnDataTransferResponseReceived?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDataTransferResponseReceived));
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
        public async Task<CS.CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCertificateSignedRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCertificateSignedRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SendSignedCertificate(Request)

                                      : new CS.CertificateSignedResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CertificateSignedResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnCertificateSignedResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCertificateSignedResponseReceived));
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
        public async Task<CS.InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {

            #region Send OnInstallCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnInstallCertificateRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnInstallCertificateRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomInstallCertificateRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.InstallCertificate(Request)

                                      : new CS.InstallCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.InstallCertificateResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnInstallCertificateResponseReceived?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnInstallCertificateResponseReceived));
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
        public async Task<CS.GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {

            #region Send OnGetInstalledCertificateIdsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsRequestSent?.Invoke(startTime,
                                                            this,
                                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetInstalledCertificateIdsRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetInstalledCertificateIdsRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetInstalledCertificateIds(Request)

                                      : new CS.GetInstalledCertificateIdsResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetInstalledCertificateIdsResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetInstalledCertificateIdsResponseReceived?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetInstalledCertificateIdsResponseReceived));
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
        public async Task<CS.DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {

            #region Send OnDeleteCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteCertificateRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.DeleteCertificate(Request)

                                      : new CS.DeleteCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.DeleteCertificateResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnDeleteCertificateResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnDeleteCertificateResponseReceived));
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
        public async Task<CS.NotifyCRLResponse> NotifyCRLAvailability(NotifyCRLRequest Request)
        {

            #region Send OnNotifyCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCRLRequestSent?.Invoke(startTime,
                                           this,
                                           Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyCRLRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomNotifyCRLRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.NotifyCRLAvailability(Request)

                                      : new CS.NotifyCRLResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.NotifyCRLResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnNotifyCRLResponseReceived?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyCRLResponseReceived));
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
        public async Task<CS.GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        {

            #region Send OnGetLocalListVersionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLocalListVersionRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetLocalListVersionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetLocalListVersion(Request)

                                      : new CS.GetLocalListVersionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetLocalListVersionResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetLocalListVersionResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetLocalListVersionResponseReceived));
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
        public async Task<CS.SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequestSent?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendLocalListRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SendLocalList(Request)

                                      : new CS.SendLocalListResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SendLocalListResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSendLocalListResponseReceived?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSendLocalListResponseReceived));
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
        public async Task<CS.ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequestSent?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearCacheRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearCacheRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.ClearCache(Request)

                                      : new CS.ClearCacheResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearCacheResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnClearCacheResponseReceived?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearCacheResponseReceived));
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
        public async Task<CS.ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        {

            #region Send OnReserveNowRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowRequestSent?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnReserveNowRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.ReserveNow(Request)

                                      : new CS.ReserveNowResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ReserveNowResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnReserveNowResponseReceived?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnReserveNowResponseReceived));
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
        public async Task<CS.CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        {

            #region Send OnCancelReservationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCancelReservationRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCancelReservationRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.CancelReservation(Request)

                                      : new CS.CancelReservationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CancelReservationResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnCancelReservationResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCancelReservationResponseReceived));
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
        public async Task<CS.RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request)
        {

            #region Send OnRequestStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionRequestSent?.Invoke(startTime,
                                                         this,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStartTransactionRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.StartCharging(Request)

                                      : new CS.RequestStartTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RequestStartTransactionResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnRequestStartTransactionResponseReceived?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStartTransactionResponseReceived));
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
        public async Task<CS.RequestStopTransactionResponse> StopCharging(RequestStopTransactionRequest Request)
        {

            #region Send OnRequestStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStopTransactionRequestSent?.Invoke(startTime,
                                                        this,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStopTransactionRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRequestStopTransactionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.StopCharging(Request)

                                      : new CS.RequestStopTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RequestStopTransactionResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnRequestStopTransactionResponseReceived?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnRequestStopTransactionResponseReceived));
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
        public async Task<CS.GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request)
        {

            #region Send OnGetTransactionStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetTransactionStatusRequestSent?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetTransactionStatusRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetTransactionStatusRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetTransactionStatus(Request)

                                      : new CS.GetTransactionStatusResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetTransactionStatusResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetTransactionStatusResponseReceived?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetTransactionStatusResponseReceived));
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
        public async Task<CS.SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetChargingProfileRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetChargingProfile(Request)

                                      : new CS.SetChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetChargingProfileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetChargingProfileResponseReceived?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetChargingProfileResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetChargingProfiles         (Request)

        /// <summary>
        /// Get the charging profiles of the given charging station connector.
        /// </summary>
        /// <param name="Request">A GetChargingProfiles request.</param>
        public async Task<CS.GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request)
        {

            #region Send OnGetChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetChargingProfilesRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetChargingProfilesRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.GetChargingProfiles(Request)

                                      : new CS.GetChargingProfilesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetChargingProfilesResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetChargingProfilesResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetChargingProfilesResponseReceived));
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
        public async Task<CS.ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequestSent?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearChargingProfileRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.ClearChargingProfile(Request)

                                      : new CS.ClearChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearChargingProfileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnClearChargingProfileResponseReceived?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearChargingProfileResponseReceived));
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
        public async Task<CS.GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        {

            #region Send OnGetCompositeScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequestSent?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetCompositeScheduleRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetCompositeScheduleRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetCompositeSchedule(Request)

                                      : new CS.GetCompositeScheduleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetCompositeScheduleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetCompositeScheduleResponseReceived?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetCompositeScheduleResponseReceived));
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
        public async Task<CS.UpdateDynamicScheduleResponse> UpdateDynamicSchedule(UpdateDynamicScheduleRequest Request)
        {

            #region Send OnUpdateDynamicScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateDynamicScheduleRequestSent?.Invoke(startTime,
                                                       this,
                                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateDynamicScheduleRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUpdateDynamicScheduleRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.UpdateDynamicSchedule(Request)

                                      : new CS.UpdateDynamicScheduleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UpdateDynamicScheduleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnUpdateDynamicScheduleResponseReceived?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUpdateDynamicScheduleResponseReceived));
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
        public async Task<CS.NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request)
        {

            #region Send OnNotifyAllowedEnergyTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyAllowedEnergyTransferRequestSent?.Invoke(startTime,
                                                             this,
                                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyAllowedEnergyTransferRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomNotifyAllowedEnergyTransferRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.NotifyAllowedEnergyTransfer(Request)

                                      : new CS.NotifyAllowedEnergyTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.NotifyAllowedEnergyTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnNotifyAllowedEnergyTransferResponseReceived?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnNotifyAllowedEnergyTransferResponseReceived));
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
        public async Task<CS.UsePriorityChargingResponse> UsePriorityCharging(UsePriorityChargingRequest Request)
        {

            #region Send OnUsePriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUsePriorityChargingRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUsePriorityChargingRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUsePriorityChargingRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.UsePriorityCharging(Request)

                                      : new CS.UsePriorityChargingResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UsePriorityChargingResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnUsePriorityChargingResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUsePriorityChargingResponseReceived));
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
        public async Task<CS.UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        {

            #region Send OnUnlockConnectorRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequestSent?.Invoke(startTime,
                                                 this,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnlockConnectorRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUnlockConnectorRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.UnlockConnector(Request)

                                      : new CS.UnlockConnectorResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.UnlockConnectorResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnUnlockConnectorResponseReceived?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnUnlockConnectorResponseReceived));
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
        public async Task<CS.AFRRSignalResponse> SendAFRRSignal(AFRRSignalRequest Request)
        {

            #region Send OnAFRRSignalRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAFRRSignalRequestSent?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAFRRSignalRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomAFRRSignalRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SendAFRRSignal(Request)

                                      : new CS.AFRRSignalResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.AFRRSignalResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnAFRRSignalResponseReceived?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnAFRRSignalResponseReceived));
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
        public async Task<CS.SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request)
        {

            #region Send OnSetDisplayMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDisplayMessageRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDisplayMessageRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetDisplayMessage(Request)

                                      : new CS.SetDisplayMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetDisplayMessageResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnSetDisplayMessageResponseReceived?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSetDisplayMessageResponseReceived));
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
        public async Task<CS.GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
        {

            #region Send OnGetDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDisplayMessagesRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetDisplayMessagesRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetDisplayMessages(Request)

                                      : new CS.GetDisplayMessagesResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetDisplayMessagesResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnGetDisplayMessagesResponseReceived?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnGetDisplayMessagesResponseReceived));
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
        public async Task<CS.ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request)
        {

            #region Send OnClearDisplayMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearDisplayMessageRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearDisplayMessageRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearDisplayMessageRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.ClearDisplayMessage(Request)

                                      : new CS.ClearDisplayMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.ClearDisplayMessageResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnClearDisplayMessageResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnClearDisplayMessageResponseReceived));
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
        public async Task<CS.CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request)
        {

            #region Send OnCostUpdatedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCostUpdatedRequestSent?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCostUpdatedRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCostUpdatedRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SendCostUpdated(Request)

                                      : new CS.CostUpdatedResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CostUpdatedResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnCostUpdatedResponseReceived?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCostUpdatedResponseReceived));
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
        public async Task<CS.CustomerInformationResponse> RequestCustomerInformation(CustomerInformationRequest Request)
        {

            #region Send OnCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCustomerInformationRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCustomerInformationRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.RequestCustomerInformation(Request)

                                      : new CS.CustomerInformationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.CustomerInformationResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnCustomerInformationResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnCustomerInformationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


        // Binary Data Streams Extensions

        #region BinaryDataTransfer          (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        {

            #region Send OnBinaryDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferRequestSent?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBinaryDataTransferRequestSent));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomBinaryDataTransferRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.BinaryDataTransfer(Request)

                                      : new BinaryDataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new BinaryDataTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

                OnBinaryDataTransferResponseReceived?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnBinaryDataTransferResponseReceived));
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
        public async Task<OCPP.CS.GetFileResponse> GetFile(GetFileRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetFile(Request)

                                      : new OCPP.CS.GetFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.GetFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.SendFileResponse>
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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomSendFileRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SendFile(Request)

                                      : new OCPP.CS.SendFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.SendFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.DeleteFileResponse> DeleteFile(DeleteFileRequest Request)
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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDeleteFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.DeleteFile(Request)

                                      : new OCPP.CS.DeleteFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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

        #region ListDirectory               (Request)

        /// <summary>
        /// List the given directory of the charging station or networking node.
        /// </summary>
        /// <param name="Request">A ListDirectory request.</param>
        public async Task<OCPP.CS.ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        {

            #region Send OnListDirectoryRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnListDirectoryRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnListDirectoryRequest));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomListDirectoryRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.ListDirectory(Request)

                                      : new OCPP.CS.ListDirectoryResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.ListDirectoryResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomListDirectoryResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnListDirectoryResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnListDirectoryResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnListDirectoryResponse));
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
        public async Task<OCPP.CS.AddSignaturePolicyResponse>
            AddSignaturePolicy(OCPP.CSMS.AddSignaturePolicyRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.AddSignaturePolicy(Request)

                                      : new OCPP.CS.AddSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.AddSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.UpdateSignaturePolicyResponse>
            UpdateSignaturePolicy(OCPP.CSMS.UpdateSignaturePolicyRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.UpdateSignaturePolicy(Request)

                                      : new OCPP.CS.UpdateSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.UpdateSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.DeleteSignaturePolicyResponse>
            DeleteSignaturePolicy(OCPP.CSMS.DeleteSignaturePolicyRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.DeleteSignaturePolicy(Request)

                                      : new OCPP.CS.DeleteSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.AddUserRoleResponse>
            AddUserRole(OCPP.CSMS.AddUserRoleRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.AddUserRole(Request)

                                      : new OCPP.CS.AddUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.AddUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.UpdateUserRoleResponse>
            UpdateUserRole(OCPP.CSMS.UpdateUserRoleRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.UpdateUserRole(Request)

                                      : new OCPP.CS.UpdateUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.UpdateUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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
        public async Task<OCPP.CS.DeleteUserRoleResponse>
            DeleteUserRole(OCPP.CSMS.DeleteUserRoleRequest Request)

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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.DeleteUserRole(Request)

                                      : new OCPP.CS.DeleteUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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


        #region SecureDataTransfer          (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        public async Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request)
        {

            #region Send OnSecureDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSecureDataTransferRequest));
            }

            #endregion


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomSecureDataTransferRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.SecureDataTransfer(Request)

                                      : new SecureDataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new SecureDataTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomSecureDataTransferResponseSerializer,
                    // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnSecureDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCSMS) + "." + nameof(OnSecureDataTransferResponse));
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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

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

                                      ? await csmsChannel.SetDefaultChargingTariff(Request)

                                      : new CS.SetDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.SetDefaultChargingTariffResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetDefaultChargingTariffRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.GetDefaultChargingTariff(Request)

                                      : new CS.GetDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.GetDefaultChargingTariffResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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


            var response  = LookupNetworkingNode(Request.DestinationNodeId, out var csmsChannel) &&
                                csmsChannel is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRemoveDefaultChargingTariffRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await csmsChannel.RemoveDefaultChargingTariff(Request)

                                      : new CS.RemoveDefaultChargingTariffResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CS.RemoveDefaultChargingTariffResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
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


        #region AddOrUpdateHTTPBasicAuth(NetworkingNodeId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        /// <param name="Password">The password of the networking node.</param>
        public void AddOrUpdateHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
                                             String             Password)
        {

            foreach (var csmsChannelServer in csmsChannelServers)
            {
                if (csmsChannelServer is CSMSWSServer csmsChannelWSServer)
                {
                    csmsChannelWSServer.AddOrUpdateHTTPBasicAuth(NetworkingNodeId, Password);
                }
            }

        }

        #endregion

        #region RemoveHTTPBasicAuth(NetworkingNodeId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        public void RemoveHTTPBasicAuth(NetworkingNode_Id NetworkingNodeId)
        {

            foreach (var csmsChannelServer in csmsChannelServers)
            {
                if (csmsChannelServer is CSMSWSServer csmsChannelWSServer)
                {
                    csmsChannelWSServer.RemoveHTTPBasicAuth(NetworkingNodeId);
                }
            }

        }

        #endregion

        #region ChargingStations

        #region Data

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        protected internal readonly ConcurrentDictionary<ChargingStation_Id, CSMS.ChargingStation> chargingStations = new();

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        public IEnumerable<CSMS.ChargingStation> ChargingStations
            => chargingStations.Values;

        public bool DisableWebSocketPings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string HTTPServiceName => throw new NotImplementedException();

        public IIPAddress IPAddress => throw new NotImplementedException();

        public IPPort IPPort => throw new NotImplementedException();

        public IPSocket IPSocket => throw new NotImplementedException();

        public bool IsRunning => throw new NotImplementedException();

        public HashSet<string> SecWebSocketProtocols => throw new NotImplementedException();

        public bool ServerThreadIsBackground => throw new NotImplementedException();

        public ServerThreadNameCreatorDelegate ServerThreadNameCreator => throw new NotImplementedException();

        public ServerThreadPriorityDelegate ServerThreadPrioritySetter => throw new NotImplementedException();

        public TimeSpan? SlowNetworkSimulationDelay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<WebSocketServerConnection> WebSocketConnections => throw new NotImplementedException();

        public TimeSpan WebSocketPingEvery { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(ChargingStation,                      MessageType,    OldChargingStation = null, ...)

        ///// <summary>
        ///// Write the given chargingStation to the database and send out notifications.
        ///// </summary>
        ///// <param name="ChargingStation">The charging station.</param>
        ///// <param name="MessageType">The chargingStation notification.</param>
        ///// <param name="OldChargingStation">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async Task WriteToDatabaseFileAndNotify(ChargingStation             ChargingStation,
        //                                                           NotificationMessageType  MessageType,
        //                                                           ChargingStation             OldChargingStation   = null,
        //                                                           EventTracking_Id         EventTrackingId   = null,
        //                                                           User_Id?                 CurrentUserId     = null)
        //{

        //    if (ChargingStation is null)
        //        throw new ArgumentNullException(nameof(ChargingStation),  "The given chargingStation must not be null or empty!");

        //    if (MessageType.IsNullOrEmpty)
        //        throw new ArgumentNullException(nameof(MessageType),   "The given message type must not be null or empty!");


        //    var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

        //    await WriteToDatabaseFile(MessageType,
        //                              ChargingStation.ToJSON(false, true),
        //                              eventTrackingId,
        //                              CurrentUserId);

        //    await SendNotifications(ChargingStation,
        //                            MessageType,
        //                            OldChargingStation,
        //                            eventTrackingId,
        //                            CurrentUserId);

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargingStation,                      MessageType(s), OldChargingStation = null, ...)

        //protected virtual String ChargingStationHTMLInfo(ChargingStation ChargingStation)

        //    => String.Concat(ChargingStation.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id, "\">", ChargingStation.Name.FirstText(), "</a> ",
        //                                        "(<a href=\"https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id, "\">", ChargingStation.Id, "</a>)")
        //                         : String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id, "\">", ChargingStation.Id, "</a>"));

        //protected virtual String ChargingStationTextInfo(ChargingStation ChargingStation)

        //    => String.Concat(ChargingStation.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("'", ChargingStation.Name.FirstText(), "' (", ChargingStation.Id, ")")
        //                         : String.Concat("'", ChargingStation.Id.ToString(), "'"));


        ///// <summary>
        ///// Send chargingStation notifications.
        ///// </summary>
        ///// <param name="ChargingStation">The charging station.</param>
        ///// <param name="MessageType">The chargingStation notification.</param>
        ///// <param name="OldChargingStation">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargingStation identification</param>
        //protected internal virtual Task SendNotifications(ChargingStation             ChargingStation,
        //                                                  NotificationMessageType  MessageType,
        //                                                  ChargingStation             OldChargingStation   = null,
        //                                                  EventTracking_Id         EventTrackingId   = null,
        //                                                  User_Id?                 CurrentUserId     = null)

        //    => SendNotifications(ChargingStation,
        //                         new NotificationMessageType[] { MessageType },
        //                         OldChargingStation,
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargingStation notifications.
        ///// </summary>
        ///// <param name="ChargingStation">The charging station.</param>
        ///// <param name="MessageTypes">The chargingStation notifications.</param>
        ///// <param name="OldChargingStation">The old/updated charging station.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargingStation identification</param>
        //protected internal async virtual Task SendNotifications(ChargingStation                          ChargingStation,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        ChargingStation                          OldChargingStation   = null,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargingStation is null)
        //        throw new ArgumentNullException(nameof(ChargingStation),  "The given chargingStation must not be null or empty!");

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

        //    if (messageTypesHash.Contains(addChargingStationIfNotExists_MessageType))
        //        messageTypesHash.Add(addChargingStation_MessageType);

        //    if (messageTypesHash.Contains(addOrUpdateChargingStation_MessageType))
        //        messageTypesHash.Add(OldChargingStation == null
        //                               ? addChargingStation_MessageType
        //                               : updateChargingStation_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    ComparizionResult? comparizionResult = null;

        //    if (messageTypes.Contains(updateChargingStation_MessageType))
        //        comparizionResult = ChargingStation.CompareWith(OldChargingStation);


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ChargingStation.GetNotificationsOf<TelegramNotification>(messageTypes).
        //                                                     ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargingStation_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargingStationHTMLInfo(ChargingStation) + " was successfully created.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                    if (messageTypes.Contains(updateChargingStation_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargingStationHTMLInfo(ChargingStation) + " information had been successfully updated.\n" + comparizionResult?.ToTelegram(),
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

        //            var AllSMSNotifications  = ChargingStation.GetNotificationsOf<SMSNotification>(messageTypes).
        //                                                    ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargingStation_MessageType))
        //                    SendSMS(String.Concat("ChargingStation '", ChargingStation.Name.FirstText(), "' was successfully created. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //                if (messageTypes.Contains(updateChargingStation_MessageType))
        //                    SendSMS(String.Concat("ChargingStation '", ChargingStation.Name.FirstText(), "' information had been successfully updated. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id),
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

        //            var AllHTTPSNotifications  = ChargingStation.GetNotificationsOf<HTTPSNotification>(messageTypes).
        //                                                      ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargingStation_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargingStationCreated",
        //                                                         ChargingStation.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //                if (messageTypes.Contains(updateChargingStation_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargingStationUpdated",
        //                                                         ChargingStation.ToJSON()
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

        //                var AllEMailNotifications  = ChargingStation.GetNotificationsOf<EMailNotification>(messageTypes).
        //                                                          ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargingStation_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargingStationTextInfo(ChargingStation) + " was successfully created",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargingStationHTMLInfo(ChargingStation) + " was successfully created.",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargingStationTextInfo(ChargingStation) + " was successfully created.\r\n",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                    if (messageTypes.Contains(updateChargingStation_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargingStationTextInfo(ChargingStation) + " information had been successfully updated",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargingStationHTMLInfo(ChargingStation) + " information had been successfully updated.<br /><br />",
        //                                                                    comparizionResult?.ToHTML() ?? "",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargingStationTextInfo(ChargingStation) + " information had been successfully updated.\r\r\r\r",
        //                                                                    comparizionResult?.ToText() ?? "",
        //                                                                    "\r\r\r\r",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargingStations/", ChargingStation.Id, "\r\r\r\r",
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

        #region (protected internal) SendNotifications           (ChargingStation, ParentChargingStationes, MessageType(s), ...)

        ///// <summary>
        ///// Send chargingStation notifications.
        ///// </summary>
        ///// <param name="ChargingStation">The charging station.</param>
        ///// <param name="ParentChargingStationes">The enumeration of parent charging stationes.</param>
        ///// <param name="MessageType">The chargingStation notification.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargingStation identification</param>
        //protected internal virtual Task SendNotifications(ChargingStation               ChargingStation,
        //                                                  IEnumerable<ChargingStation>  ParentChargingStationes,
        //                                                  NotificationMessageType    MessageType,
        //                                                  EventTracking_Id           EventTrackingId   = null,
        //                                                  User_Id?                   CurrentUserId     = null)

        //    => SendNotifications(ChargingStation,
        //                         ParentChargingStationes,
        //                         new NotificationMessageType[] { MessageType },
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargingStation notifications.
        ///// </summary>
        ///// <param name="ChargingStation">The charging station.</param>
        ///// <param name="ParentChargingStationes">The enumeration of parent charging stationes.</param>
        ///// <param name="MessageTypes">The user notifications.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async virtual Task SendNotifications(ChargingStation                          ChargingStation,
        //                                                        IEnumerable<ChargingStation>             ParentChargingStationes,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargingStation is null)
        //        throw new ArgumentNullException(nameof(ChargingStation),         "The given chargingStation must not be null or empty!");

        //    if (ParentChargingStationes is null)
        //        ParentChargingStationes = new ChargingStation[0];

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),         "The given enumeration of message types must not be null or empty!");

        //    //if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
        //    //    messageTypesHash.Add(addUser_MessageType);

        //    //if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
        //    //    messageTypesHash.Add(OldChargingStation == null
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

        //                var AllTelegramNotifications  = ParentChargingStationes.
        //                                                    SelectMany(parent => parent.User2ChargingStationEdges).
        //                                                    SelectMany(edge   => edge.Source.GetNotificationsOf<TelegramNotification>(deleteChargingStation_MessageType)).
        //                                                    ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargingStation_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargingStationHTMLInfo(ChargingStation) + " has been deleted.",
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

        //            var AllSMSNotifications = ParentChargingStationes.
        //                                          SelectMany(parent => parent.User2ChargingStationEdges).
        //                                          SelectMany(edge   => edge.Source.GetNotificationsOf<SMSNotification>(deleteChargingStation_MessageType)).
        //                                          ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargingStation_MessageType))
        //                    SendSMS(String.Concat("ChargingStation '", ChargingStation.Name.FirstText(), "' has been deleted."),
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

        //            var AllHTTPSNotifications = ParentChargingStationes.
        //                                            SelectMany(parent => parent.User2ChargingStationEdges).
        //                                            SelectMany(edge   => edge.Source.GetNotificationsOf<HTTPSNotification>(deleteChargingStation_MessageType)).
        //                                            ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargingStation_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargingStationDeleted",
        //                                                         ChargingStation.ToJSON()
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

        //                var AllEMailNotifications = ParentChargingStationes.
        //                                                SelectMany(parent => parent.User2ChargingStationEdges).
        //                                                SelectMany(edge   => edge.Source.GetNotificationsOf<EMailNotification>(deleteChargingStation_MessageType)).
        //                                                ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargingStation_MessageType))
        //                        await SMTPClient.Send(
        //                             new HTMLEMailBuilder() {

        //                                 From           = Robot.EMail,
        //                                 To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                 Passphrase     = APIPassphrase,
        //                                 Subject        = ChargingStationTextInfo(ChargingStation) + " has been deleted",

        //                                 HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargingStationHTMLInfo(ChargingStation) + " has been deleted.<br />",
        //                                                                HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargingStationTextInfo(ChargingStation) + " has been deleted.\r\n",
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

        #region (protected internal) GetChargingStationSerializator (Request, ChargingStation)

        protected internal ChargingStationToJSONDelegate GetChargingStationSerializator(HTTPRequest  Request,
                                                                            User         User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (chargingStation,
                            embedded,
                            expandTags,
                            includeCryptoHash)

                            => chargingStation.ToJSON(embedded,
                                                expandTags,
                                                includeCryptoHash);

            }

        }

        #endregion


        #region AddChargingStation           (ChargingStation, OnAdded = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargingStation was added.</param>
        /// <param name="ChargingStation">The added charging station.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public delegate Task OnChargingStationAddedDelegate(DateTime           Timestamp,
                                                      CSMS.ChargingStation          ChargingStation,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                                      User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was added.
        /// </summary>
        public event OnChargingStationAddedDelegate? OnChargingStationAdded;


        #region (protected internal) _AddChargingStation(ChargingStation, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargingStation to the API.
        /// </summary>
        /// <param name="ChargingStation">A new chargingStation to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        protected internal async Task<AddChargingStationResult>

            _AddChargingStation(CSMS.ChargingStation                             ChargingStation,
                          Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                          EventTracking_Id?                     EventTrackingId   = null,
                          User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargingStation.API is not null && ChargingStation.API != this)
                return AddChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (chargingStations.ContainsKey(ChargingStation.Id))
                return AddChargingStationResult.ArgumentError(
                           ChargingStation,
                           $"ChargingStation identification '{ChargingStation.Id}' already exists!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargingStation.Id.Length < MinChargingStationIdLength)
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                               eventTrackingId,
            //                                               nameof(ChargingStation),
            //                                               "ChargingStation identification '" + ChargingStation.Id + "' is too short!");

            //if (ChargingStation.Name.IsNullOrEmpty() || ChargingStation.Name.IsNullOrEmpty())
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                               eventTrackingId,
            //                                               nameof(ChargingStation),
            //                                               "The given chargingStation name must not be null!");

            //if (ChargingStation.Name.Length < MinChargingStationNameLength)
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                       nameof(ChargingStation),
            //                                       "ChargingStation name '" + ChargingStation.Name + "' is too short!");

            ChargingStation.API = this;


            //await WriteToDatabaseFile(addChargingStation_MessageType,
            //                          ChargingStation.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargingStations.TryAdd(ChargingStation.Id, ChargingStation);

            OnAdded?.Invoke(ChargingStation,
                            eventTrackingId);

            var OnChargingStationAddedLocal = OnChargingStationAdded;
            if (OnChargingStationAddedLocal is not null)
                await OnChargingStationAddedLocal.Invoke(Timestamp.Now,
                                                   ChargingStation,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargingStation,
            //                        addChargingStation_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargingStationResult.Success(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddChargingStation                      (ChargingStation, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargingStation and add him/her to the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A new charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public async Task<AddChargingStationResult>

            AddChargingStation(CSMS.ChargingStation                             ChargingStation,
                         Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                         EventTracking_Id?                     EventTrackingId   = null,
                         User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargingStation(ChargingStation,
                                               OnAdded,
                                               eventTrackingId,
                                               CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargingStationResult.Error(
                               ChargingStation,
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

            return AddChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddChargingStationIfNotExists(ChargingStation, OnAdded = null, ...)

        #region (protected internal) _AddChargingStationIfNotExists(ChargingStation, OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given chargingStation to the API.
        /// </summary>
        /// <param name="ChargingStation">A new chargingStation to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        protected internal async Task<AddChargingStationResult>

            _AddChargingStationIfNotExists(CSMS.ChargingStation                             ChargingStation,
                                     Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                     EventTracking_Id?                     EventTrackingId   = null,
                                     User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargingStation.API != null && ChargingStation.API != this)
                return AddChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (chargingStations.ContainsKey(ChargingStation.Id))
                return AddChargingStationResult.NoOperation(
                           chargingStations[ChargingStation.Id],
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargingStation.Id.Length < MinChargingStationIdLength)
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargingStation),
            //                                                          "ChargingStation identification '" + ChargingStation.Id + "' is too short!");

            //if (ChargingStation.Name.IsNullOrEmpty() || ChargingStation.Name.IsNullOrEmpty())
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargingStation),
            //                                                          "The given chargingStation name must not be null!");

            //if (ChargingStation.Name.Length < MinChargingStationNameLength)
            //    return AddChargingStationResult.ArgumentError(ChargingStation,
            //                                                  nameof(ChargingStation),
            //                                                  "ChargingStation name '" + ChargingStation.Name + "' is too short!");

            ChargingStation.API = this;


            //await WriteToDatabaseFile(addChargingStationIfNotExists_MessageType,
            //                          ChargingStation.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargingStations.TryAdd(ChargingStation.Id, ChargingStation);

            OnAdded?.Invoke(ChargingStation,
                            eventTrackingId);

            var OnChargingStationAddedLocal = OnChargingStationAdded;
            if (OnChargingStationAddedLocal != null)
                await OnChargingStationAddedLocal.Invoke(Timestamp.Now,
                                                   ChargingStation,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargingStation,
            //                        addChargingStationIfNotExists_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargingStationResult.Success(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddChargingStationIfNotExists                      (ChargingStation, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargingStation and add him/her to the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A new charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public async Task<AddChargingStationResult>

            AddChargingStationIfNotExists(CSMS.ChargingStation                             ChargingStation,
                                    Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                    EventTracking_Id?                     EventTrackingId   = null,
                                    User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargingStationIfNotExists(ChargingStation,
                                                          OnAdded,
                                                          eventTrackingId,
                                                          CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargingStationResult.Error(
                               ChargingStation,
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

            return AddChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddOrUpdateChargingStation   (ChargingStation, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateChargingStation(ChargingStation, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargingStation to/within the API.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        protected internal async Task<AddOrUpdateChargingStationResult>

            _AddOrUpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                                  Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                  Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                     EventTrackingId   = null,
                                  User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargingStation.API != null && ChargingStation.API != this)
                return AddOrUpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargingStation.Id.Length < MinChargingStationIdLength)
            //    return AddOrUpdateChargingStationResult.ArgumentError(ChargingStation,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargingStation),
            //                                                       "The given chargingStation identification '" + ChargingStation.Id + "' is too short!");

            //if (ChargingStation.Name.IsNullOrEmpty() || ChargingStation.Name.IsNullOrEmpty())
            //    return AddOrUpdateChargingStationResult.ArgumentError(ChargingStation,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargingStation),
            //                                                       "The given chargingStation name must not be null!");

            //if (ChargingStation.Name.Length < MinChargingStationNameLength)
            //    return AddOrUpdateChargingStationResult.ArgumentError(ChargingStation,
            //                                               eventTrackingId,
            //                                               nameof(ChargingStation),
            //                                               "ChargingStation name '" + ChargingStation.Name + "' is too short!");

            ChargingStation.API = this;


            //await WriteToDatabaseFile(addOrUpdateChargingStation_MessageType,
            //                          ChargingStation.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            if (chargingStations.TryGetValue(ChargingStation.Id, out var OldChargingStation))
            {
                chargingStations.TryRemove(OldChargingStation.Id, out _);
                ChargingStation.CopyAllLinkedDataFromBase(OldChargingStation);
            }

            chargingStations.TryAdd(ChargingStation.Id, ChargingStation);

            if (OldChargingStation is null)
            {

                OnAdded?.Invoke(ChargingStation,
                                eventTrackingId);

                var OnChargingStationAddedLocal = OnChargingStationAdded;
                if (OnChargingStationAddedLocal != null)
                    await OnChargingStationAddedLocal.Invoke(Timestamp.Now,
                                                       ChargingStation,
                                                       eventTrackingId,
                                                       CurrentUserId);

                //await SendNotifications(ChargingStation,
                //                        addChargingStation_MessageType,
                //                        null,
                //                        eventTrackingId,
                //                        CurrentUserId);

                return AddOrUpdateChargingStationResult.Added(
                           ChargingStation,
                           eventTrackingId,
                           Id,
                           this
                       );

            }

            OnUpdated?.Invoke(ChargingStation,
                              eventTrackingId);

            var OnChargingStationUpdatedLocal = OnChargingStationUpdated;
            if (OnChargingStationUpdatedLocal != null)
                await OnChargingStationUpdatedLocal.Invoke(Timestamp.Now,
                                                        ChargingStation,
                                                        OldChargingStation,
                                                        eventTrackingId,
                                                        CurrentUserId);

            //await SendNotifications(ChargingStation,
            //                        updateChargingStation_MessageType,
            //                        OldChargingStation,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddOrUpdateChargingStationResult.Updated(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region AddOrUpdateChargingStation                      (ChargingStation, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargingStation to/within the API.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="OnAdded">A delegate run whenever the chargingStation has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public async Task<AddOrUpdateChargingStationResult>

            AddOrUpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                                 Action<CSMS.ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                 Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                 EventTracking_Id?                     EventTrackingId   = null,
                                 User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddOrUpdateChargingStation(ChargingStation,
                                                       OnAdded,
                                                       OnUpdated,
                                                       eventTrackingId,
                                                       CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddOrUpdateChargingStationResult.Error(
                               ChargingStation,
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

            return AddOrUpdateChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region UpdateChargingStation        (ChargingStation,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargingStation was updated.</param>
        /// <param name="ChargingStation">The updated charging station.</param>
        /// <param name="OldChargingStation">The old charging station.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public delegate Task OnChargingStationUpdatedDelegate(DateTime           Timestamp,
                                                        CSMS.ChargingStation          ChargingStation,
                                                        CSMS.ChargingStation          OldChargingStation,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was updated.
        /// </summary>
        public event OnChargingStationUpdatedDelegate? OnChargingStationUpdated;


        #region (protected internal) _UpdateChargingStation(ChargingStation,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargingStation to/within the API.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        protected internal async Task<UpdateChargingStationResult>

            _UpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                             Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_TryGetChargingStation(ChargingStation.Id, out var OldChargingStation))
                return UpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           $"The given chargingStation '{ChargingStation.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (ChargingStation.API != null && ChargingStation.API != this)
                return UpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            ChargingStation.API = this;


            //await WriteToDatabaseFile(updateChargingStation_MessageType,
            //                          ChargingStation.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargingStations.TryRemove(OldChargingStation.Id, out _);
            ChargingStation.CopyAllLinkedDataFromBase(OldChargingStation);
            chargingStations.TryAdd(ChargingStation.Id, ChargingStation);

            OnUpdated?.Invoke(ChargingStation,
                              eventTrackingId);

            var OnChargingStationUpdatedLocal = OnChargingStationUpdated;
            if (OnChargingStationUpdatedLocal is not null)
                await OnChargingStationUpdatedLocal.Invoke(Timestamp.Now,
                                                     ChargingStation,
                                                     OldChargingStation,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargingStation,
            //                        updateChargingStation_MessageType,
            //                        OldChargingStation,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargingStationResult.Success(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region UpdateChargingStation                      (ChargingStation,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargingStation to/within the API.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public async Task<UpdateChargingStationResult>

            UpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                            Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargingStation(ChargingStation,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargingStationResult.Error(
                               ChargingStation,
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

            return UpdateChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion


        #region (protected internal) _UpdateChargingStation(ChargingStation, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charging station.
        /// </summary>
        /// <param name="ChargingStation">An charging station.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        protected internal async Task<UpdateChargingStationResult>

            _UpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                             Action<CSMS.ChargingStation.Builder>             UpdateDelegate,
                             Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_ChargingStationExists(ChargingStation.Id))
                return UpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           $"The given chargingStation '{ChargingStation.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (ChargingStation.API != this)
                return UpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (UpdateDelegate is null)
                return UpdateChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given update delegate must not be null!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var builder = ChargingStation.ToBuilder();
            UpdateDelegate(builder);
            var updatedChargingStation = builder.ToImmutable;

            //await WriteToDatabaseFile(updateChargingStation_MessageType,
            //                          updatedChargingStation.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargingStations.TryRemove(ChargingStation.Id, out _);
            updatedChargingStation.CopyAllLinkedDataFromBase(ChargingStation);
            chargingStations.TryAdd(updatedChargingStation.Id, updatedChargingStation);

            OnUpdated?.Invoke(updatedChargingStation,
                              eventTrackingId);

            var OnChargingStationUpdatedLocal = OnChargingStationUpdated;
            if (OnChargingStationUpdatedLocal is not null)
                await OnChargingStationUpdatedLocal.Invoke(Timestamp.Now,
                                                     updatedChargingStation,
                                                     ChargingStation,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(updatedChargingStation,
            //                        updateChargingStation_MessageType,
            //                        ChargingStation,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargingStationResult.Success(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region UpdateChargingStation                      (ChargingStation, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charging station.
        /// </summary>
        /// <param name="ChargingStation">An charging station.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charging station.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargingStation has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargingStation identification initiating this command/request.</param>
        public async Task<UpdateChargingStationResult>

            UpdateChargingStation(CSMS.ChargingStation                             ChargingStation,
                            Action<CSMS.ChargingStation.Builder>             UpdateDelegate,
                            Action<CSMS.ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargingStation(ChargingStation,
                                                  UpdateDelegate,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargingStationResult.Error(
                               ChargingStation,
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

            return UpdateChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion

        #region DeleteChargingStation        (ChargingStation, OnDeleted = null, ...)

        /// <summary>
        /// A delegate called whenever a charging station was deleted.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargingStation was deleted.</param>
        /// <param name="ChargingStation">The chargingStation to be deleted.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnChargingStationDeletedDelegate(DateTime              Timestamp,
                                                              CSMS.ChargingStation  ChargingStation,
                                                              EventTracking_Id?     EventTrackingId   = null,
                                                              User_Id?              CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was deleted.
        /// </summary>
        public event OnChargingStationDeletedDelegate? OnChargingStationDeleted;



        #region (protected internal virtual) _CanDeleteChargingStation(ChargingStation)

        /// <summary>
        /// Determines whether the chargingStation can safely be deleted from the API.
        /// </summary>
        /// <param name="ChargingStation">The chargingStation to be deleted.</param>
        protected internal virtual I18NString? _CanDeleteChargingStation(CSMS.ChargingStation ChargingStation)
        {

            //if (ChargingStation.Users.Any())
            //    return new I18NString(Languages.en, "The chargingStation still has members!");

            //if (ChargingStation.SubChargingStationes.Any())
            //    return new I18NString(Languages.en, "The chargingStation still has sub chargingStations!");

            return null;

        }

        #endregion

        #region (protected internal) _DeleteChargingStation(ChargingStation, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charging station.
        /// </summary>
        /// <param name="ChargingStation">The chargingStation to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargingStation has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<DeleteChargingStationResult>

            _DeleteChargingStation(CSMS.ChargingStation                             ChargingStation,
                                   Action<CSMS.ChargingStation, EventTracking_Id>?  OnDeleted         = null,
                                   EventTracking_Id?                                EventTrackingId   = null,
                                   User_Id?                                         CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargingStation.API != this)
                return DeleteChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (!chargingStations.TryGetValue(ChargingStation.Id, out var ChargingStationToBeDeleted))
                return DeleteChargingStationResult.ArgumentError(
                           ChargingStation,
                           "The given chargingStation does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var veto = _CanDeleteChargingStation(ChargingStation);

            if (veto is not null)
                return DeleteChargingStationResult.CanNotBeRemoved(
                           ChargingStation,
                           eventTrackingId,
                           Id,
                           this,
                           veto
                       );


            //// Get all parent chargingStations now, because later
            //// the --isChildOf--> edge will no longer be available!
            //var parentChargingStationes = ChargingStation.GetAllParents(parent => parent != NoOwner).
            //                                       ToArray();


            //// Remove all: this --edge--> other_chargingStation
            //foreach (var edge in ChargingStation.ChargingStation2ChargingStationOutEdges.ToArray())
            //    await _UnlinkChargingStationes(edge.Source,
            //                               edge.EdgeLabel,
            //                               edge.Target,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);

            //// Remove all: this <--edge-- other_chargingStation
            //foreach (var edge in ChargingStation.ChargingStation2ChargingStationInEdges.ToArray())
            //    await _UnlinkChargingStationes(edge.Target,
            //                               edge.EdgeLabel,
            //                               edge.Source,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);


            //await WriteToDatabaseFile(deleteChargingStation_MessageType,
            //                          ChargingStation.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargingStations.TryRemove(ChargingStation.Id, out _);

            OnDeleted?.Invoke(ChargingStation,
                              eventTrackingId);

            var OnChargingStationDeletedLocal = OnChargingStationDeleted;
            if (OnChargingStationDeletedLocal is not null)
                await OnChargingStationDeletedLocal.Invoke(Timestamp.Now,
                                                     ChargingStation,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargingStation,
            //                        parentChargingStationes,
            //                        deleteChargingStation_MessageType,
            //                        eventTrackingId,
            //                        CurrentUserId);


            return DeleteChargingStationResult.Success(
                       ChargingStation,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #region DeleteChargingStation                      (ChargingStation, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charging station.
        /// </summary>
        /// <param name="ChargingStation">The chargingStation to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargingStation has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<DeleteChargingStationResult>

            DeleteChargingStation(CSMS.ChargingStation                             ChargingStation,
                                  Action<CSMS.ChargingStation, EventTracking_Id>?  OnDeleted         = null,
                                  EventTracking_Id?                                EventTrackingId   = null,
                                  User_Id?                                         CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargingStationSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _DeleteChargingStation(ChargingStation,
                                                  OnDeleted,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return DeleteChargingStationResult.Error(
                               ChargingStation,
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

            return DeleteChargingStationResult.LockTimeout(
                       ChargingStation,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion

        #endregion


        #region ChargingStationExists(ChargingStationId)

        /// <summary>
        /// Determines whether the given chargingStation identification exists within this API.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        protected internal Boolean _ChargingStationExists(ChargingStation_Id ChargingStationId)

            => ChargingStationId.IsNotNullOrEmpty && chargingStations.ContainsKey(ChargingStationId);

        /// <summary>
        /// Determines whether the given chargingStation identification exists within this API.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        protected internal Boolean _ChargingStationExists(ChargingStation_Id? ChargingStationId)

            => ChargingStationId.IsNotNullOrEmpty() && chargingStations.ContainsKey(ChargingStationId.Value);


        /// <summary>
        /// Determines whether the given chargingStation identification exists within this API.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        public Boolean ChargingStationExists(ChargingStation_Id ChargingStationId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargingStationExists(ChargingStationId);

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
        /// Determines whether the given chargingStation identification exists within this API.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        public Boolean ChargingStationExists(ChargingStation_Id? ChargingStationId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargingStationExists(ChargingStationId);

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

        #region GetChargingStation   (ChargingStationId)

        /// <summary>
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        protected internal CSMS.ChargingStation? _GetChargingStation(ChargingStation_Id ChargingStationId)
        {

            if (ChargingStationId.IsNotNullOrEmpty && chargingStations.TryGetValue(ChargingStationId, out var chargingStation))
                return chargingStation;

            return default;

        }

        /// <summary>
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        protected internal CSMS.ChargingStation? _GetChargingStation(ChargingStation_Id? ChargingStationId)
        {

            if (ChargingStationId is not null && chargingStations.TryGetValue(ChargingStationId.Value, out var chargingStation))
                return chargingStation;

            return default;

        }


        /// <summary>
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        public CSMS.ChargingStation? GetChargingStation(ChargingStation_Id ChargingStationId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargingStation(ChargingStationId);

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
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        public CSMS.ChargingStation? GetChargingStation(ChargingStation_Id? ChargingStationId)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargingStation(ChargingStationId);

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

        #region TryGetChargingStation(ChargingStationId, out ChargingStation)

        /// <summary>
        /// Try to get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        /// <param name="ChargingStation">The charging station.</param>
        protected internal Boolean _TryGetChargingStation(ChargingStation_Id                             ChargingStationId,
                                                          [NotNullWhen(true)] out CSMS.ChargingStation?  ChargingStation)
        {

            if (ChargingStationId.IsNotNullOrEmpty && chargingStations.TryGetValue(ChargingStationId, out var chargingStation))
            {
                ChargingStation = chargingStation;
                return true;
            }

            ChargingStation = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        /// <param name="ChargingStation">The charging station.</param>
        protected internal Boolean _TryGetChargingStation(ChargingStation_Id?                            ChargingStationId,
                                                          [NotNullWhen(true)] out CSMS.ChargingStation?  ChargingStation)
        {

            if (ChargingStationId is not null && chargingStations.TryGetValue(ChargingStationId.Value, out var chargingStation))
            {
                ChargingStation = chargingStation;
                return true;
            }

            ChargingStation = null;
            return false;

        }


        /// <summary>
        /// Try to get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        /// <param name="ChargingStation">The charging station.</param>
        public Boolean TryGetChargingStation(ChargingStation_Id                             ChargingStationId,
                                             [NotNullWhen(true)] out CSMS.ChargingStation?  ChargingStation)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargingStation(ChargingStationId, out ChargingStation);

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

            ChargingStation = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        /// <param name="ChargingStation">The charging station.</param>
        public Boolean TryGetChargingStation(ChargingStation_Id?                            ChargingStationId,
                                             [NotNullWhen(true)] out CSMS.ChargingStation?  ChargingStation)
        {

            if (ChargingStationSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargingStation(ChargingStationId, out ChargingStation);

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

            ChargingStation = null;
            return false;

        }

        #endregion

        #endregion


        #region Shutdown(Message, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            var csmsChannelServersCopy = csmsChannelServers.ToArray();
            if (csmsChannelServersCopy.Length > 0)
            {
                try
                {

                    await Task.WhenAll(csmsChannelServers.
                                           Select(csmsChannel => csmsChannel.Shutdown(
                                                                     Message,
                                                                     Wait
                                                                 )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(TestCSMS),
                              nameof(Shutdown),
                              e
                          );
                }
            }

        }

        #endregion


        #region (virtual) HandleErrors(Module, Caller, ErrorResponse)

        public virtual Task HandleErrors(String  Module,
                                         String  Caller,
                                         String  ErrorResponse)
        {

            return Task.CompletedTask;

        }

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccured)

        public virtual Task HandleErrors(String     Module,
                                         String     Caller,
                                         Exception  ExceptionOccured)
        {

            return Task.CompletedTask;

        }

        public Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime RequestTimestamp, WebSocketServerConnection Connection, string TextMessage, EventTracking_Id EventTrackingId, CancellationToken CancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime RequestTimestamp, WebSocketServerConnection Connection, byte[] BinaryMessage, EventTracking_Id EventTrackingId, CancellationToken CancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SendStatus> SendTextMessage(WebSocketServerConnection Connection, string TextMessage, EventTracking_Id? EventTrackingId = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SendStatus> SendBinaryMessage(WebSocketServerConnection Connection, byte[] BinaryMessage, EventTracking_Id? EventTrackingId = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SendStatus> SendWebSocketFrame(WebSocketServerConnection Connection, WebSocketFrame WebSocketFrame, EventTracking_Id? EventTrackingId = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool RemoveConnection(WebSocketServerConnection Connection)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}
