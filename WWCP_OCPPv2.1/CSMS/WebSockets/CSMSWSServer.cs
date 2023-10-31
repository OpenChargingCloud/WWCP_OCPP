﻿/*
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public class CSMSWSServer : WebSocketServer,
                                ICSMSChannel
    {

        #region (enum)  SendJSONResults

        public enum SendJSONResults
        {
            Success,
            UnknownClient,
            TransmissionFailed
        }

        #endregion

        #region (class) SendRequestState

        public class SendRequestState
        {

            public DateTime                       Timestamp            { get; }
            public ChargingStation_Id             ChargingStationId    { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage     { get; }
            public DateTime                       Timeout              { get; }

            public DateTime?                      ResponseTimestamp    { get; set; }
            public JObject?                       Response             { get; set; }

            public ResultCodes?                   ErrorCode            { get; set; }
            public String?                        ErrorDescription     { get; set; }
            public JObject?                       ErrorDetails         { get; set; }


            public Boolean                        NoErrors
                 => !ErrorCode.HasValue;

            public Boolean                        HasErrors
                 =>  ErrorCode.HasValue;


            public SendRequestState(DateTime                       Timestamp,
                                    ChargingStation_Id             ChargingStationId,
                                    OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                    DateTime                       Timeout,

                                    DateTime?                      ResponseTimestamp   = null,
                                    JObject?                       Response            = null,

                                    ResultCodes?                   ErrorCode           = null,
                                    String?                        ErrorDescription    = null,
                                    JObject?                       ErrorDetails        = null)
            {

                this.Timestamp          = Timestamp;
                this.ChargingStationId  = ChargingStationId;
                this.WSRequestMessage   = WSRequestMessage;
                this.Timeout            = Timeout;

                this.ResponseTimestamp  = ResponseTimestamp;
                this.Response           = Response;

                this.ErrorCode          = ErrorCode;
                this.ErrorDescription   = ErrorDescription;
                this.ErrorDetails       = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const            String                                                                                 DefaultHTTPServiceName           = $"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly  IPPort                                                                                 DefaultHTTPServerPort            = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public static readonly  HTTPPath                                                                               DefaultURLPrefix                 = HTTPPath.Parse("/" + Version.String);

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly  TimeSpan                                                                               DefaultRequestTimeout            = TimeSpan.FromSeconds(30);


        public  const           String                                                                                 chargingStationId_WebSocketKey   = "chargingStationId";

        private readonly        ConcurrentDictionary<ChargingStation_Id, Tuple<WebSocketServerConnection, DateTime>>   connectedChargingStations        = new();

        private readonly        ConcurrentDictionary<Request_Id, SendRequestState>                                     requests                         = new();


        private const           String                                                                                 LogfileName                      = "CSMSWSServer.log";

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => HTTPServiceName;

        /// <summary>
        /// The enumeration of all connected charging stations.
        /// </summary>
        public IEnumerable<ChargingStation_Id> ChargingStationIds
            => connectedChargingStations.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean                                      RequireAuthentication    { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public ConcurrentDictionary<ChargingStation_Id, String?>  ChargingBoxLogins        { get; }
            = new();

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                                   JSONFormatting           { get; set; }
            = Formatting.None;

        /// <summary>
        /// The request timeout for messages sent by this HTTP WebSocket server.
        /// </summary>
        public TimeSpan?                                    RequestTimeout           { get; set; }

        #endregion

        #region Events

        public event OnNewCSMSWSConnectionDelegate?  OnNewCSMSWSConnection;


        #region Generic Text Messages

        /// <summary>
        /// An event sent whenever a text message request was received.
        /// </summary>
        public event OnWebSocketTextMessageRequestDelegate?     OnTextMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextMessageResponseDelegate?    OnTextMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnTextErrorResponseSent;


        /// <summary>
        /// An event sent whenever a text message request was sent.
        /// </summary>
        public event OnWebSocketTextMessageRequestDelegate?     OnTextMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextMessageResponseDelegate?    OnTextMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnTextErrorResponseReceived;

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
        public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


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
        public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion


        #region CSMS <- Charging Station Messages

        #region OnBootNotification                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a BootNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a BootNotification was received.
        /// </summary>
        public event OnBootNotificationDelegate?            OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a BootNotification was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a BootNotification was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnBootNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotification           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                      OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?            OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                     OnFirmwareStatusNotificationWSResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                             OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?     OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationDelegate?            OnPublishFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?    OnPublishFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                            OnPublishFirmwareStatusNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a Heartbeat WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a Heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?            OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a Heartbeat was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnHeartbeatWSResponse;

        #endregion

        #region OnNotifyEvent

        /// <summary>
        /// An event sent whenever a NotifyEvent WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?       OnNotifyEventWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEvent request was received.
        /// </summary>
        public event OnNotifyEventRequestDelegate?     OnNotifyEventRequest;

        /// <summary>
        /// An event sent whenever a NotifyEvent was received.
        /// </summary>
        public event OnNotifyEventDelegate?            OnNotifyEvent;

        /// <summary>
        /// An event sent whenever a response to a NotifyEvent was sent.
        /// </summary>
        public event OnNotifyEventResponseDelegate?    OnNotifyEventResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEvent was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?      OnNotifyEventWSResponse;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a SecurityEventNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?     OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?            OnSecurityEventNotification;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?    OnSecurityEventNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a SecurityEventNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnSecurityEventNotificationWSResponse;

        #endregion

        #region OnNotifyReport

        /// <summary>
        /// An event sent whenever a NotifyReport WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?        OnNotifyReportWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyReport request was received.
        /// </summary>
        public event OnNotifyReportRequestDelegate?     OnNotifyReportRequest;

        /// <summary>
        /// An event sent whenever a NotifyReport was received.
        /// </summary>
        public event OnNotifyReportDelegate?            OnNotifyReport;

        /// <summary>
        /// An event sent whenever a response to a NotifyReport was sent.
        /// </summary>
        public event OnNotifyReportResponseDelegate?    OnNotifyReportResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyReport was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?       OnNotifyReportWSResponse;

        #endregion

        #region OnNotifyMonitoringReport

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?     OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport was received.
        /// </summary>
        public event OnNotifyMonitoringReportDelegate?            OnNotifyMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a NotifyMonitoringReport was sent.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?    OnNotifyMonitoringReportResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyMonitoringReport was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnNotifyMonitoringReportWSResponse;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a LogStatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?     OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationDelegate?            OnLogStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?    OnLogStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a LogStatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnLogStatusNotificationWSResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a DataTransfer WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a DataTransfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnIncomingDataTransferWSResponse;

        #endregion


        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a SignCertificate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?           OnSignCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateRequestDelegate?     OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateDelegate?            OnSignCertificate;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        public event OnSignCertificateResponseDelegate?    OnSignCertificateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a SignCertificate request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?          OnSignCertificateWSResponse;

        #endregion

        #region OnGet15118EVCertificate

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?     OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate was received.
        /// </summary>
        public event OnGet15118EVCertificateDelegate?            OnGet15118EVCertificate;

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate was sent.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?    OnGet15118EVCertificateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a Get15118EVCertificate was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnGet15118EVCertificateWSResponse;

        #endregion

        #region OnGetCertificateStatus

        /// <summary>
        /// An event sent whenever a GetCertificateStatus WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?     OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus was received.
        /// </summary>
        public event OnGetCertificateStatusDelegate?            OnGetCertificateStatus;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus was sent.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?    OnGetCertificateStatusResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a GetCertificateStatus was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnGetCertificateStatusWSResponse;

        #endregion

        #region OnGetCRL

        /// <summary>
        /// An event sent whenever a GetCRL WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnGetCRLWSRequest;

        /// <summary>
        /// An event sent whenever a GetCRL request was received.
        /// </summary>
        public event OnGetCRLRequestDelegate?        OnGetCRLRequest;

        /// <summary>
        /// An event sent whenever a GetCRL was received.
        /// </summary>
        public event OnGetCRLDelegate?               OnGetCRL;

        /// <summary>
        /// An event sent whenever a response to a GetCRL was sent.
        /// </summary>
        public event OnGetCRLResponseDelegate?       OnGetCRLResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a GetCRL was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnGetCRLWSResponse;

        #endregion


        #region OnReservationStatusUpdate

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                   OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate was received.
        /// </summary>
        public event OnReservationStatusUpdateDelegate?            OnReservationStatusUpdate;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                  OnReservationStatusUpdateWSResponse;

        #endregion

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an Authorize WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?            OnAuthorize;

        /// <summary>
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an Authorize WebSocket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnAuthorizeWSResponse;

        #endregion

        #region OnNotifyEVChargingNeeds

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?     OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsDelegate?            OnNotifyEVChargingNeeds;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingNeeds was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?    OnNotifyEVChargingNeedsResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEVChargingNeeds was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnNotifyEVChargingNeedsWSResponse;

        #endregion

        #region OnTransactionEvent

        /// <summary>
        /// An event sent whenever a TransactionEvent WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnTransactionEventWSRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventRequestDelegate?     OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventDelegate?            OnTransactionEvent;

        /// <summary>
        /// An event sent whenever a TransactionEvent response was sent.
        /// </summary>
        public event OnTransactionEventResponseDelegate?    OnTransactionEventResponse;

        /// <summary>
        /// An event sent whenever a TransactionEvent WebSocket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnTransactionEventWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a StatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?              OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?     OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?            OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?    OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a StatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?             OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a MeterValues WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?       OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?     OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesDelegate?            OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?    OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a MeterValues request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?      OnMeterValuesWSResponse;

        #endregion

        #region OnNotifyChargingLimit

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?               OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?     OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit was received.
        /// </summary>
        public event OnNotifyChargingLimitDelegate?            OnNotifyChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit was sent.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?    OnNotifyChargingLimitResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyChargingLimit was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?              OnNotifyChargingLimitWSResponse;

        #endregion

        #region OnClearedChargingLimit

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?     OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit was received.
        /// </summary>
        public event OnClearedChargingLimitDelegate?            OnClearedChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a ClearedChargingLimit was sent.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?    OnClearedChargingLimitResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ClearedChargingLimit was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnClearedChargingLimitWSResponse;

        #endregion

        #region OnReportChargingProfiles

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?     OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        public event OnReportChargingProfilesDelegate?            OnReportChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?    OnReportChargingProfilesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReportChargingProfiles was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnReportChargingProfilesWSResponse;

        #endregion

        #region OnNotifyEVChargingSchedule

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                    OnNotifyEVChargingScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?     OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleDelegate?            OnNotifyEVChargingSchedule;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?    OnNotifyEVChargingScheduleResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                   OnNotifyEVChargingScheduleWSResponse;

        #endregion

        #region OnNotifyPriorityCharging

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnNotifyPriorityChargingWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?     OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging was received.
        /// </summary>
        public event OnNotifyPriorityChargingDelegate?            OnNotifyPriorityCharging;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging was sent.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?    OnNotifyPriorityChargingResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyPriorityCharging was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnNotifyPriorityChargingWSResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnPullDynamicScheduleUpdateWSRequest;

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?     OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateDelegate?            OnPullDynamicScheduleUpdate;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?    OnPullDynamicScheduleUpdateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnPullDynamicScheduleUpdateWSResponse;

        #endregion


        #region OnNotifyDisplayMessages

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?     OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages was received.
        /// </summary>
        public event OnNotifyDisplayMessagesDelegate?            OnNotifyDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a NotifyDisplayMessages was sent.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?    OnNotifyDisplayMessagesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyDisplayMessages was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnNotifyDisplayMessagesWSResponse;

        #endregion

        #region OnNotifyCustomerInformation

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?     OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation was received.
        /// </summary>
        public event OnNotifyCustomerInformationDelegate?            OnNotifyCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a NotifyCustomerInformation was sent.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?    OnNotifyCustomerInformationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyCustomerInformation was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnNotifyCustomerInformationWSResponse;

        #endregion

        #endregion

        #region CSMS -> Charging Station Messages

        #region OnReset                          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a Reset request was sent.
        /// </summary>
        public event OnResetRequestDelegate?     OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?    OnResetResponse;

        #endregion

        #region OnUpdateFirmware                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        #endregion

        #region OnPublishFirmware                (-Request/-Response)

        /// <summary>
        /// An event sent whenever a PublishFirmware request was sent.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?     OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?    OnPublishFirmwareResponse;

        #endregion

        #region OnUnpublishFirmware              (-Request/-Response)

        /// <summary>
        /// An event sent whenever an UnpublishFirmware request was sent.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?     OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UnpublishFirmware request was sent.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?    OnUnpublishFirmwareResponse;

        #endregion

        #region OnGetBaseReport                  (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetBaseReport request was sent.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport request was sent.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        #endregion

        #region OnGetReport                      (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetReport request was sent.
        /// </summary>
        public event OnGetReportRequestDelegate?     OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        public event OnGetReportResponseDelegate?    OnGetReportResponse;

        #endregion

        #region OnGetLog                         (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetLog request was sent.
        /// </summary>
        public event OnGetLogRequestDelegate?     OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLog request was sent.
        /// </summary>
        public event OnGetLogResponseDelegate?    OnGetLogResponse;

        #endregion

        #region OnSetVariables                   (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetVariables request was sent.
        /// </summary>
        public event OnSetVariablesRequestDelegate?     OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        public event OnSetVariablesResponseDelegate?    OnSetVariablesResponse;

        #endregion

        #region OnGetVariables                   (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetVariables request was sent.
        /// </summary>
        public event OnGetVariablesRequestDelegate?     OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetVariables request was sent.
        /// </summary>
        public event OnGetVariablesResponseDelegate?    OnGetVariablesResponse;

        #endregion

        #region OnSetMonitoringBase              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetMonitoringBase request was sent.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?     OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringBase request was sent.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?    OnSetMonitoringBaseResponse;

        #endregion

        #region OnGetMonitoringReport            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was sent.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?     OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetMonitoringReport request was sent.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?    OnGetMonitoringReportResponse;

        #endregion

        #region OnSetMonitoringLevel             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetMonitoringLevel request was sent.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?     OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a response to a SetMonitoringLevel request was sent.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?    OnSetMonitoringLevelResponse;

        #endregion

        #region OnSetVariableMonitoring          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetVariableMonitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?     OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a SetVariableMonitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?    OnSetVariableMonitoringResponse;

        #endregion

        #region OnClearVariableMonitoring        (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was sent.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?     OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearVariableMonitoring request was sent.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?    OnClearVariableMonitoringResponse;

        #endregion

        #region OnSetNetworkProfile              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetNetworkProfile request was sent.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?     OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetNetworkProfile request was sent.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?    OnSetNetworkProfileResponse;

        #endregion

        #region OnChangeAvailability             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was sent.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a ChangeAvailability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        #endregion

        #region OnTriggerMessage                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever a TriggerMessage request was sent.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a TriggerMessage request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        #endregion

        #region OnDataTransfer                   (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DataTransfer request was sent.
        /// </summary>
        public event OnDataTransferRequestDelegate?     OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was sent.
        /// </summary>
        public event OnDataTransferResponseDelegate?    OnDataTransferResponse;

        #endregion


        #region OnCertificateSigned              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a CertificateSigned request was sent.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?     OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?    OnCertificateSignedResponse;

        #endregion

        #region OnInstallCertificate             (-Request/-Response)

        /// <summary>
        /// An event sent whenever an InstallCertificate request was sent.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?     OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an InstallCertificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?    OnInstallCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?    OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnDeleteCertificate              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteCertificate request was sent.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?     OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteCertificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?    OnDeleteCertificateResponse;

        #endregion

        #region OnNotifyCRL                      (-Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyCRL request was sent.
        /// </summary>
        public event OnNotifyCRLRequestDelegate?     OnNotifyCRLRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        public event OnNotifyCRLResponseDelegate?    OnNotifyCRLResponse;

        #endregion


        #region OnGetLocalListVersion            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was sent.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?     OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?    OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList                  (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?    OnSendLocalListResponse;

        #endregion

        #region OnClearCache                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearCache request was sent.
        /// </summary>
        public event OnClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?    OnClearCacheResponse;

        #endregion


        #region OnReserveNow                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ReserveNow request was sent.
        /// </summary>
        public event OnReserveNowRequestDelegate?     OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a ReserveNow request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?    OnReserveNowResponse;

        #endregion

        #region OnCancelReservation              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a CancelReservation request was sent.
        /// </summary>
        public event OnCancelReservationRequestDelegate?     OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a CancelReservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?    OnCancelReservationResponse;

        #endregion

        #region OnRequestStartTransaction        (-Request/-Response)

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        #endregion

        #region OnRequestStopTransaction         (-Request/-Response)

        /// <summary>
        /// An event sent whenever a RequestStopTransaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?     OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStopTransaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?    OnRequestStopTransactionResponse;

        #endregion

        #region OnGetTransactionStatus           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?     OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?    OnGetTransactionStatusResponse;

        #endregion

        #region OnSetChargingProfile             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetChargingProfile request was sent.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        #endregion

        #region OnGetChargingProfiles            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetChargingProfiles request was sent.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?     OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetChargingProfiles request was sent.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?    OnGetChargingProfilesResponse;

        #endregion

        #region OnClearChargingProfile           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was sent.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule           (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?     OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?    OnGetCompositeScheduleResponse;

        #endregion

        #region OnUpdateDynamicSchedule          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a UpdateDynamicSchedule request was sent.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?     OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a UpdateDynamicSchedule request was sent.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?    OnUpdateDynamicScheduleResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?    OnNotifyAllowedEnergyTransferResponse;

        #endregion

        #region OnUsePriorityCharging            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a UsePriorityCharging request was sent.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?     OnUsePriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a UsePriorityCharging request was sent.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?    OnUsePriorityChargingResponse;

        #endregion

        #region OnUnlockConnector                (-Request/-Response)

        /// <summary>
        /// An event sent whenever an UnlockConnector request was sent.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?     OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?    OnUnlockConnectorResponse;

        #endregion


        #region OnAFRRSignal                     (-Request/-Response)

        /// <summary>
        /// An event sent whenever an AFRR signal request was sent.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?     OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever a response to an AFRR signal request was sent.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?    OnAFRRSignalResponse;

        #endregion


        #region OnSetDisplayMessage              (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetDisplayMessage request was sent.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?     OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDisplayMessage request was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?    OnSetDisplayMessageResponse;

        #endregion

        #region OnGetDisplayMessages             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?     OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?    OnGetDisplayMessagesResponse;

        #endregion

        #region OnClearDisplayMessage            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a ClearDisplayMessage request was sent.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?     OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearDisplayMessage request was sent.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?    OnClearDisplayMessageResponse;

        #endregion

        #region OnCostUpdated                    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a CostUpdated request was sent.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?     OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a response to a CostUpdated request was sent.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?    OnCostUpdatedResponse;

        #endregion

        #region OnCustomerInformation            (-Request/-Response)

        /// <summary>
        /// An event sent whenever a CustomerInformation request was sent.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?     OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a response to a CustomerInformation request was sent.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?    OnCustomerInformationResponse;

        #endregion


        // E2E Security Extensions

        #region OnAddSignaturePolicy             (-Request/-Response)

        /// <summary>
        /// An event sent whenever a AddSignaturePolicy request was sent.
        /// </summary>
        public event OnAddSignaturePolicyRequestDelegate?     OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a AddSignaturePolicy request was sent.
        /// </summary>
        public event OnAddSignaturePolicyResponseDelegate?    OnAddSignaturePolicyResponse;

        #endregion

        #region OnUpdateSignaturePolicy          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?     OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?    OnUpdateSignaturePolicyResponse;

        #endregion

        #region OnDeleteSignaturePolicy          (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was sent.
        /// </summary>
        public event OnDeleteSignaturePolicyRequestDelegate?     OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteSignaturePolicy request was sent.
        /// </summary>
        public event OnDeleteSignaturePolicyResponseDelegate?    OnDeleteSignaturePolicyResponse;

        #endregion

        #region OnAddUserRole                    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a AddUserRole request was sent.
        /// </summary>
        public event OnAddUserRoleRequestDelegate?     OnAddUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to a AddUserRole request was sent.
        /// </summary>
        public event OnAddUserRoleResponseDelegate?    OnAddUserRoleResponse;

        #endregion

        #region OnUpdateUserRole                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever a UpdateUserRole request was sent.
        /// </summary>
        public event OnUpdateUserRoleRequestDelegate?     OnUpdateUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to a UpdateUserRole request was sent.
        /// </summary>
        public event OnUpdateUserRoleResponseDelegate?    OnUpdateUserRoleResponse;

        #endregion

        #region OnDeleteUserRole                 (-Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was sent.
        /// </summary>
        public event OnDeleteUserRoleRequestDelegate?     OnDeleteUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteUserRole request was sent.
        /// </summary>
        public event OnDeleteUserRoleResponseDelegate?    OnDeleteUserRoleResponse;

        #endregion


        // E2E Charging Tariffs Extensions

        #region OnSetDefaultChargingTariff       (-Request/-Response)

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestDelegate?        OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseDelegate?       OnSetDefaultChargingTariffResponse;

        #endregion

        #region OnGetDefaultChargingTariff       (-Request/-Response)

        /// <summary>
        /// An event sent whenever a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestDelegate?        OnGetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseDelegate?       OnGetDefaultChargingTariffResponse;

        #endregion

        #region OnRemoveDefaultChargingTariff    (-Request/-Response)

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestDelegate?     OnRemoveDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseDelegate?    OnRemoveDefaultChargingTariffResponse;

        #endregion

        #endregion

        #endregion

        #region Custom JSON parser delegates

        #region Charging Station Messages

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>?                   CustomBootNotificationRequestParser                     { get; set; }

        /// <summary>
        /// A delegate to parse custom FirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?         CustomFirmwareStatusNotificationRequestParser           { get; set; }

        /// <summary>
        /// A delegate to parse custom PublishFirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestParser    { get; set; }

        /// <summary>
        /// A delegate to parse custom Heartbeat requests.
        /// </summary>
        public CustomJObjectParserDelegate<HeartbeatRequest>?                          CustomHeartbeatRequestParser                            { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyEvent requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyEventRequest>?                        CustomNotifyEventRequestParser                          { get; set; }

        /// <summary>
        /// A delegate to parse custom SecurityEventNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<SecurityEventNotificationRequest>?          CustomSecurityEventNotificationRequestParser            { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyReport requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyReportRequest>?                       CustomNotifyReportRequestParser                         { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyMonitoringReport requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyMonitoringReportRequest>?             CustomNotifyMonitoringReportRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom LogStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<LogStatusNotificationRequest>?              CustomLogStatusNotificationRequestParser                { get; set; }

        /// <summary>
        /// A delegate to parse custom DataTransfer requests.
        /// </summary>
        public CustomJObjectParserDelegate<CS.DataTransferRequest>?                    CustomDataTransferRequestParser                         { get; set; }


        /// <summary>
        /// A delegate to parse custom SignCertificate requests.
        /// </summary>
        public CustomJObjectParserDelegate<SignCertificateRequest>?                    CustomSignCertificateRequestParser                      { get; set; }

        /// <summary>
        /// A delegate to parse custom Get15118EVCertificate requests.
        /// </summary>
        public CustomJObjectParserDelegate<Get15118EVCertificateRequest>?              CustomGet15118EVCertificateRequestParser                { get; set; }

        /// <summary>
        /// A delegate to parse custom GetCertificateStatus requests.
        /// </summary>
        public CustomJObjectParserDelegate<GetCertificateStatusRequest>?               CustomGetCertificateStatusRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom GetCRL requests.
        /// </summary>
        public CustomJObjectParserDelegate<GetCRLRequest>?                             CustomGetCRLRequestParser                               { get; set; }

        /// <summary>
        /// A delegate to parse custom ReservationStatusUpdate requests.
        /// </summary>
        public CustomJObjectParserDelegate<ReservationStatusUpdateRequest>?            CustomReservationStatusUpdateRequestParser              { get; set; }

        /// <summary>
        /// A delegate to parse custom Authorize requests.
        /// </summary>
        public CustomJObjectParserDelegate<AuthorizeRequest>?                          CustomAuthorizeRequestParser                            { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyEVChargingNeeds requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?              CustomNotifyEVChargingNeedsRequestParser                { get; set; }


        /// <summary>
        /// A delegate to parse custom TransactionEvent requests.
        /// </summary>
        public CustomJObjectParserDelegate<TransactionEventRequest>?                   CustomTransactionEventRequestParser                     { get; set; }

        /// <summary>
        /// A delegate to parse custom StatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<StatusNotificationRequest>?                 CustomStatusNotificationRequestParser                   { get; set; }

        /// <summary>
        /// A delegate to parse custom MeterValues requests.
        /// </summary>
        public CustomJObjectParserDelegate<MeterValuesRequest>?                        CustomMeterValuesRequestParser                          { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyChargingLimit requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyChargingLimitRequest>?                CustomNotifyChargingLimitRequestParser                  { get; set; }

        /// <summary>
        /// A delegate to parse custom ClearedChargingLimit requests.
        /// </summary>
        public CustomJObjectParserDelegate<ClearedChargingLimitRequest>?               CustomClearedChargingLimitRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom ReportChargingProfiles requests.
        /// </summary>
        public CustomJObjectParserDelegate<ReportChargingProfilesRequest>?             CustomReportChargingProfilesRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyEVChargingSchedule requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?           CustomNotifyEVChargingScheduleRequestParser             { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyPriorityCharging requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?             CustomNotifyPriorityChargingRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom PullDynamicScheduleUpdate requests.
        /// </summary>
        public CustomJObjectParserDelegate<PullDynamicScheduleUpdateRequest>?          CustomPullDynamicScheduleUpdateRequestParser            { get; set; }



        /// <summary>
        /// A delegate to parse custom NotifyDisplayMessages requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyDisplayMessagesRequest>?              CustomNotifyDisplayMessagesRequestParser                { get; set; }

        /// <summary>
        /// A delegate to parse custom NotifyCustomerInformation requests.
        /// </summary>
        public CustomJObjectParserDelegate<NotifyCustomerInformationRequest>?          CustomNotifyCustomerInformationRequestParser            { get; set; }

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region CSMS Request Messages

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


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?                           CustomAddSignaturePolicyRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?                        CustomUpdateSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?                        CustomDeleteSignaturePolicyRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                                  CustomAddUserRoleRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                               CustomUpdateUserRoleRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                               CustomDeleteUserRoleRequestSerializer                        { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<SetDefaultChargingTariffRequest>?                     CustomSetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<GetDefaultChargingTariffRequest>?                     CustomGetDefaultChargingTariffRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffRequest>?                  CustomRemoveDefaultChargingTariffRequestSerializer           { get; set; }

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

        #endregion

        #region Data Structures
        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }
        
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }
        
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }


        // E2E Charging Tariffs Extensions
        public CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize a new HTTP server for the CSMS HTTP/WebSocket/JSON API.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSMSWSServer(String                               HTTPServiceName              = DefaultHTTPServiceName,
                            IIPAddress?                          IPAddress                    = null,
                            IPPort?                              TCPPort                      = null,

                            Boolean                              RequireAuthentication        = true,
                            Boolean                              DisableWebSocketPings        = false,
                            TimeSpan?                            WebSocketPingEvery           = null,
                            TimeSpan?                            SlowNetworkSimulationDelay   = null,

                            ServerCertificateSelectorDelegate?   ServerCertificateSelector    = null,
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

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   HTTPServiceName,

                   new[] {
                      "ocpp2.0.1",
                       Version.WebSocketSubProtocolId
                   },
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

                   DNSClient,
                   false)

        {

            this.RequireAuthentication           = RequireAuthentication;

            base.OnValidateTCPConnection        += ValidateTCPConnection;
            base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            base.OnCloseMessageReceived         += ProcessCloseMessage;

            if (AutoStart)
                Start();

        }

        #endregion


        #region AddOrUpdateHTTPBasicAuth(ChargingStationId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given charging station.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of the charging station.</param>
        /// <param name="Password">The password of the charging station.</param>
        public void AddOrUpdateHTTPBasicAuth(ChargingStation_Id  ChargingStationId,
                                             String              Password)
        {

            ChargingBoxLogins.AddOrUpdate(ChargingStationId,
                                          Password,
                                          (chargingStationId, password) => Password);

        }

        #endregion

        #region RemoveHTTPBasicAuth     (ChargingStationId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given charging station.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of the charging station.</param>
        public Boolean RemoveHTTPBasicAuth(ChargingStation_Id ChargingStationId)
        {

            if (ChargingBoxLogins.ContainsKey(ChargingStationId))
                return ChargingBoxLogins.TryRemove(ChargingStationId, out _);

            return true;

        }

        #endregion


        #region (protected) ValidateTCPConnection        (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<ConnectionFilterResponse> ValidateTCPConnection(DateTime                      LogTimestamp,
                                                                     AWebSocketServer              Server,
                                                                     System.Net.Sockets.TcpClient  Connection,
                                                                     EventTracking_Id              EventTrackingId,
                                                                     CancellationToken             CancellationToken)
        {

            return Task.FromResult(ConnectionFilterResponse.Accepted());

        }

        #endregion

        #region (protected) ValidateWebSocketConnection  (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<HTTPResponse?> ValidateWebSocketConnection(DateTime                   LogTimestamp,
                                                                AWebSocketServer           Server,
                                                                WebSocketServerConnection  Connection,
                                                                EventTracking_Id           EventTrackingId,
                                                                CancellationToken          CancellationToken)
        {

            #region Verify 'Sec-WebSocket-Protocol'...

            if (Connection.HTTPRequest?.SecWebSocketProtocol is null ||
                Connection.HTTPRequest?.SecWebSocketProtocol.Any() == false)
            {

                DebugX.Log("Missing 'Sec-WebSocket-Protocol' HTTP header!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                     JSONObject.Create(
                                                         new JProperty("en", "Missing 'Sec-WebSocket-Protocol' HTTP header!")
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }
            else if (!SecWebSocketProtocols.Overlaps(Connection.HTTPRequest?.SecWebSocketProtocol ?? Array.Empty<String>()))
            {

                var error = $"This WebSocket service only supports {(SecWebSocketProtocols.Select(id => $"'{id}'").AggregateWith(", "))}!";

                DebugX.Log(error);

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                         JSONObject.Create(
                                                             new JProperty("en", error)
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            #region Verify HTTP Authentication

            if (RequireAuthentication)
            {

                if (Connection.HTTPRequest?.Authorization is HTTPBasicAuthentication basicAuthentication)
                {

                    if (ChargingBoxLogins.TryGetValue(ChargingStation_Id.Parse(basicAuthentication.Username), out var password) &&
                        basicAuthentication.Password == password)
                    {
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " using authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(CSMSWSServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            return Task.FromResult<HTTPResponse?>(null);

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        protected Task ProcessNewWebSocketConnection(DateTime                   LogTimestamp,
                                                     AWebSocketServer           Server,
                                                     WebSocketServerConnection  Connection,
                                                     EventTracking_Id           EventTrackingId,
                                                     CancellationToken          CancellationToken)
        {

            if (!Connection.HasCustomData(chargingStationId_WebSocketKey) &&
                Connection.HTTPRequest is not null &&
                ChargingStation_Id.TryParse(Connection.HTTPRequest.Path.ToString()[(Connection.HTTPRequest.Path.ToString().LastIndexOf("/") + 1)..], out var chargingStationId))
            {

                // Add the charging station identification to the WebSocket connection
                Connection.TryAddCustomData(chargingStationId_WebSocketKey, chargingStationId);

                if (!connectedChargingStations.ContainsKey(chargingStationId))
                     connectedChargingStations.TryAdd(chargingStationId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                else
                {

                    DebugX.Log($"{nameof(CSMSWSServer)} Duplicate charging station '{chargingStationId}' detected!");

                    var oldChargingStation_WebSocketConnection = connectedChargingStations[chargingStationId].Item1;

                    connectedChargingStations.TryRemove(chargingStationId, out _);
                    connectedChargingStations.TryAdd   (chargingStationId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                    try
                    {
                        oldChargingStation_WebSocketConnection.Close();
                    }
                    catch (Exception e)
                    {
                        DebugX.Log($"{nameof(CSMSWSServer)} Closing old HTTP WebSocket connection failed: {e.Message}");
                    }

                }

            }

            #region Send OnNewCSMSWSConnection event

            var OnNewCSMSWSConnectionLocal = OnNewCSMSWSConnection;
            if (OnNewCSMSWSConnectionLocal is not null)
            {

                OnNewCSMSWSConnection?.Invoke(LogTimestamp,
                                              this,
                                              Connection,
                                              EventTrackingId,
                                              CancellationToken);

            }

            #endregion

            return Task.CompletedTask;

        }

        #endregion

        #region (protected) ProcessCloseMessage          (LogTimestamp, Server, Connection, EventTrackingId, StatusCode, Reason)

        protected Task ProcessCloseMessage(DateTime                          LogTimestamp,
                                           AWebSocketServer                  Server,
                                           WebSocketServerConnection         Connection,
                                           EventTracking_Id                  EventTrackingId,
                                           WebSocketFrame.ClosingStatusCode  StatusCode,
                                           String?                           Reason)
        {

            if (Connection.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey, out var chargingStationId))
            {
                //DebugX.Log(nameof(CSMSWSServer), " Charging station " + chargingStationId + " disconnected!");
                connectedChargingStations.TryRemove(chargingStationId, out _);
            }

            return Task.CompletedTask;

        }

        #endregion


        #region (protected) ProcessTextMessages          (RequestTimestamp, Connection, OCPPTextMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="OCPPTextMessage">The received OCPP message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime                   RequestTimestamp,
                                                                                    WebSocketServerConnection  Connection,
                                                                                    String                     OCPPTextMessage,
                                                                                    EventTracking_Id           EventTrackingId,
                                                                                    CancellationToken          CancellationToken)
        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            var eventTrackingId = EventTracking_Id.New;

            try
            {

                var json = JArray.Parse(OCPPTextMessage);

                #region MessageType 2: CALL        (A request  from a charging station)

                // [
                //     2,                  // MessageType: CALL
                //    "19223201",          // A unique request identification
                //    "BootNotification",  // The OCPP action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (json.Count             == 4                   &&
                    json[0].Type           == JTokenType.Integer  &&
                    json[0].Value<Byte>()  == 2                   &&
                    json[1].Type == JTokenType.String             &&
                    json[2].Type == JTokenType.String             &&
                    json[3].Type == JTokenType.Object)
                {

                    #region OnTextMessageRequestReceived

                    var requestLogger = OnTextMessageRequestReceived;
                    if (requestLogger is not null)
                    {

                        var loggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnWebSocketTextMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                          this,
                                                                                                          Connection,
                                                                                                          eventTrackingId,
                                                                                                          Timestamp.Now,
                                                                                                          OCPPTextMessage)).
                                                        ToArray();

                        try
                        {
                            await Task.WhenAll(loggerTasks);
                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageRequestReceived));
                        }

                    }

                    #endregion

                    #region Initial checks

                    var chargingStationId  = Connection.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey);
                    var requestId          = Request_Id.TryParse(json[1]?.Value<String>() ?? "");
                    var action             = json[2]?.Value<String>()?.Trim();
                    var requestData        = json[3]?.Value<JObject>();

                    if (!chargingStationId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId ?? Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'charging station identity' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (!requestId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 Request_Id.Parse("0"),
                                                 ResultCodes.ProtocolError,
                                                 "The given 'request identification' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (action is null || action.IsNullOrEmpty())
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given 'action' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    else if (requestData is null)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 ResultCodes.ProtocolError,
                                                 "The given request JSON payload must not be null!",
                                                 new JObject(
                                                     new JProperty("request", OCPPTextMessage)
                                                 )
                                             );

                    #endregion

                    else
                        switch (action)
                        {

                            #region BootNotification

                            case "BootNotification":
                                {

                                    #region Send OnBootNotificationWSRequest event

                                    try
                                    {

                                        OnBootNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (BootNotificationRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargingStationId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomBootNotificationRequestParser) && request is not null) {

                                            #region Send OnBootNotificationRequest event

                                            try
                                            {

                                                OnBootNotificationRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  Connection,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            BootNotificationResponse? response = null;

                                            var responseTasks = OnBootNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                this,
                                                                                                                                                Connection,
                                                                                                                                                request,
                                                                                                                                                CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= BootNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnBootNotificationResponse event

                                            try
                                            {

                                                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   Connection,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'BootNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'BootNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnBootNotificationWSResponse event

                                    try
                                    {

                                        OnBootNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             json,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region FirmwareStatusNotification

                            case "FirmwareStatusNotification":
                                {

                                    #region Send OnFirmwareStatusNotificationWSRequest event

                                    try
                                    {

                                        OnFirmwareStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (FirmwareStatusNotificationRequest.TryParse(requestData,
                                                                                       requestId.Value,
                                                                                       chargingStationId.Value,
                                                                                       out var request,
                                                                                       out var errorResponse,
                                                                                       CustomFirmwareStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnFirmwareStatusNotificationRequest event

                                            try
                                            {

                                                OnFirmwareStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            FirmwareStatusNotificationResponse? response = null;

                                            var responseTasks = OnFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                          this,
                                                                                                                                                          request,
                                                                                                                                                          CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= FirmwareStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnFirmwareStatusNotificationResponse event

                                            try
                                            {

                                                OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             request,
                                                                                             response,
                                                                                             response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'FirmwareStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'FirmwareStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnFirmwareStatusNotificationWSResponse event

                                    try
                                    {

                                        OnFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       json,
                                                                                       OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region PublishFirmwareStatusNotification

                            case "PublishFirmwareStatusNotification":
                                {

                                    #region Send OnPublishFirmwareStatusNotificationWSRequest event

                                    try
                                    {

                                        OnPublishFirmwareStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (PublishFirmwareStatusNotificationRequest.TryParse(requestData,
                                                                                              requestId.Value,
                                                                                              chargingStationId.Value,
                                                                                              out var request,
                                                                                              out var errorResponse,
                                                                                              CustomPublishFirmwareStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnPublishFirmwareStatusNotificationRequest event

                                            try
                                            {

                                                OnPublishFirmwareStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                                   this,
                                                                                                   request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            PublishFirmwareStatusNotificationResponse? response = null;

                                            var responseTasks = OnPublishFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnPublishFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                                 this,
                                                                                                                                                                 request,
                                                                                                                                                                 CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= PublishFirmwareStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnPublishFirmwareStatusNotificationResponse event

                                            try
                                            {

                                                OnPublishFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request,
                                                                                                    response,
                                                                                                    response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'PublishFirmwareStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'PublishFirmwareStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnPublishFirmwareStatusNotificationWSResponse event

                                    try
                                    {

                                        OnPublishFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                              this,
                                                                                              json,
                                                                                              OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region Heartbeat

                            case "Heartbeat":
                                {

                                    #region Send OnHeartbeatWSRequest event

                                    try
                                    {

                                        OnHeartbeatWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (HeartbeatRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargingStationId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomHeartbeatRequestParser) && request is not null) {

                                            #region Send OnHeartbeatRequest event

                                            try
                                            {

                                                OnHeartbeatRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            HeartbeatResponse? response = null;

                                            var responseTasks = OnHeartbeat?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= HeartbeatResponse.Failed(request);

                                            #endregion

                                            #region Send OnHeartbeatResponse event

                                            try
                                            {

                                                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'Heartbeat' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'Heartbeat' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnHeartbeatWSResponse event

                                    try
                                    {

                                        OnHeartbeatWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      json,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyEvent

                            case "NotifyEvent":
                                {

                                    #region Send OnNotifyEventWSRequest event

                                    try
                                    {

                                        OnNotifyEventWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEventWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyEventRequest.TryParse(requestData,
                                                                        requestId.Value,
                                                                        chargingStationId.Value,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomNotifyEventRequestParser) && request is not null) {

                                            #region Send OnNotifyEventRequest event

                                            try
                                            {

                                                OnNotifyEventRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEventRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyEventResponse? response = null;

                                            var responseTasks = OnNotifyEvent?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyEventDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                           this,
                                                                                                                                           request,
                                                                                                                                           CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyEventResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyEventResponse event

                                            try
                                            {

                                                OnNotifyEventResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEventResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyEvent' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyEvent' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyEventWSResponse event

                                    try
                                    {

                                        OnNotifyEventWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        json,
                                                                        OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEventWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region SecurityEventNotification

                            case "SecurityEventNotification":
                                {

                                    #region Send OnSecurityEventNotificationWSRequest event

                                    try
                                    {

                                        OnSecurityEventNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (SecurityEventNotificationRequest.TryParse(requestData,
                                                                                      requestId.Value,
                                                                                      chargingStationId.Value,
                                                                                      out var request,
                                                                                      out var errorResponse,
                                                                                      CustomSecurityEventNotificationRequestParser) && request is not null) {

                                            #region Send OnSecurityEventNotificationRequest event

                                            try
                                            {

                                                OnSecurityEventNotificationRequest?.Invoke(Timestamp.Now,
                                                                                           this,
                                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            SecurityEventNotificationResponse? response = null;

                                            var responseTasks = OnSecurityEventNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnSecurityEventNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                         this,
                                                                                                                                                         request,
                                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= SecurityEventNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnSecurityEventNotificationResponse event

                                            try
                                            {

                                                OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request,
                                                                                            response,
                                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'SecurityEventNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'SecurityEventNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnSecurityEventNotificationWSResponse event

                                    try
                                    {

                                        OnSecurityEventNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      json,
                                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyReport

                            case "NotifyReport":
                                {

                                    #region Send OnNotifyReportWSRequest event

                                    try
                                    {

                                        OnNotifyReportWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyReportWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyReportRequest.TryParse(requestData,
                                                                         requestId.Value,
                                                                         chargingStationId.Value,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomNotifyReportRequestParser) && request is not null) {

                                            #region Send OnNotifyReportRequest event

                                            try
                                            {

                                                OnNotifyReportRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyReportRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyReportResponse? response = null;

                                            var responseTasks = OnNotifyReport?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                            this,
                                                                                                                                            request,
                                                                                                                                            CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyReportResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyReportResponse event

                                            try
                                            {

                                                OnNotifyReportResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyReportResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyReport' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyReport' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyReportWSResponse event

                                    try
                                    {

                                        OnNotifyReportWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         json,
                                                                         OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyReportWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyMonitoringReport

                            case "NotifyMonitoringReport":
                                {

                                    #region Send OnNotifyMonitoringReportWSRequest event

                                    try
                                    {

                                        OnNotifyMonitoringReportWSRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyMonitoringReportRequest.TryParse(requestData,
                                                                                   requestId.Value,
                                                                                   chargingStationId.Value,
                                                                                   out var request,
                                                                                   out var errorResponse,
                                                                                   CustomNotifyMonitoringReportRequestParser) && request is not null) {

                                            #region Send OnNotifyMonitoringReportRequest event

                                            try
                                            {

                                                OnNotifyMonitoringReportRequest?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyMonitoringReportResponse? response = null;

                                            var responseTasks = OnNotifyMonitoringReport?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyMonitoringReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                      this,
                                                                                                                                                      request,
                                                                                                                                                      CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyMonitoringReportResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyMonitoringReportResponse event

                                            try
                                            {

                                                OnNotifyMonitoringReportResponse?.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         response,
                                                                                         response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyMonitoringReport' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyMonitoringReport' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyMonitoringReportWSResponse event

                                    try
                                    {

                                        OnNotifyMonitoringReportWSResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   json,
                                                                                   OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region LogStatusNotification

                            case "LogStatusNotification":
                                {

                                    #region Send OnLogStatusNotificationWSRequest event

                                    try
                                    {

                                        OnLogStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (LogStatusNotificationRequest.TryParse(requestData,
                                                                                  requestId.Value,
                                                                                  chargingStationId.Value,
                                                                                  out var request,
                                                                                  out var errorResponse,
                                                                                  CustomLogStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnLogStatusNotificationRequest event

                                            try
                                            {

                                                OnLogStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            LogStatusNotificationResponse? response = null;

                                            var responseTasks = OnLogStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnLogStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                     this,
                                                                                                                                                     request,
                                                                                                                                                     CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= LogStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnLogStatusNotificationResponse event

                                            try
                                            {

                                                OnLogStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'LogStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'LogStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnLogStatusNotificationWSResponse event

                                    try
                                    {

                                        OnLogStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json,
                                                                                  OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region DataTransfer

                            case "DataTransfer":
                                {

                                    #region Send OnIncomingDataTransferWSRequest event

                                    try
                                    {

                                        OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (CS.DataTransferRequest.TryParse(requestData,
                                                                            requestId.Value,
                                                                            chargingStationId.Value,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomDataTransferRequestParser) && request is not null) {

                                            #region Send OnIncomingDataTransferRequest event

                                            try
                                            {

                                                OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            DataTransferResponse? response = null;

                                            var responseTasks = OnIncomingDataTransfer?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                    this,
                                                                                                                                                    request,
                                                                                                                                                    CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= DataTransferResponse.Failed(request);

                                            #endregion

                                            #region Send OnIncomingDataTransferResponse event

                                            try
                                            {

                                                OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request,
                                                                                       response,
                                                                                       response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'IncomingDataTransfer' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'IncomingDataTransfer' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnIncomingDataTransferWSResponse event

                                    try
                                    {

                                        OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json,
                                                                                 OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region SignCertificate

                            case "SignCertificate":
                                {

                                    #region Send OnSignCertificateWSRequest event

                                    try
                                    {

                                        OnSignCertificateWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (SignCertificateRequest.TryParse(requestData,
                                                                            requestId.Value,
                                                                            chargingStationId.Value,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomSignCertificateRequestParser) && request is not null) {

                                            #region Send OnSignCertificateRequest event

                                            try
                                            {

                                                OnSignCertificateRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            SignCertificateResponse? response = null;

                                            var responseTasks = OnSignCertificate?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnSignCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                               this,
                                                                                                                                               request,
                                                                                                                                               CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= SignCertificateResponse.Failed(request);

                                            #endregion

                                            #region Send OnSignCertificateResponse event

                                            try
                                            {

                                                OnSignCertificateResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'SignCertificate' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'SignCertificate' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnSignCertificateWSResponse event

                                    try
                                    {

                                        OnSignCertificateWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json,
                                                                            OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region Get15118EVCertificate

                            case "Get15118EVCertificate":
                                {

                                    #region Send OnGet15118EVCertificateWSRequest event

                                    try
                                    {

                                        OnGet15118EVCertificateWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (Get15118EVCertificateRequest.TryParse(requestData,
                                                                                  requestId.Value,
                                                                                  chargingStationId.Value,
                                                                                  out var request,
                                                                                  out var errorResponse,
                                                                                  CustomGet15118EVCertificateRequestParser) && request is not null) {

                                            #region Send OnGet15118EVCertificateRequest event

                                            try
                                            {

                                                OnGet15118EVCertificateRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            Get15118EVCertificateResponse? response = null;

                                            var responseTasks = OnGet15118EVCertificate?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnGet15118EVCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                     this,
                                                                                                                                                     request,
                                                                                                                                                     CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= Get15118EVCertificateResponse.Failed(request);

                                            #endregion

                                            #region Send OnGet15118EVCertificateResponse event

                                            try
                                            {

                                                OnGet15118EVCertificateResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'Get15118EVCertificate' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'Get15118EVCertificate' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnGet15118EVCertificateWSResponse event

                                    try
                                    {

                                        OnGet15118EVCertificateWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json,
                                                                                  OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region GetCertificateStatus

                            case "GetCertificateStatus":
                                {

                                    #region Send OnGetCertificateStatusWSRequest event

                                    try
                                    {

                                        OnGetCertificateStatusWSRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (GetCertificateStatusRequest.TryParse(requestData,
                                                                                 requestId.Value,
                                                                                 chargingStationId.Value,
                                                                                 out var request,
                                                                                 out var errorResponse,
                                                                                 CustomGetCertificateStatusRequestParser) && request is not null) {

                                            #region Send OnGetCertificateStatusRequest event

                                            try
                                            {

                                                OnGetCertificateStatusRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            GetCertificateStatusResponse? response = null;

                                            var responseTasks = OnGetCertificateStatus?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnGetCertificateStatusDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                    this,
                                                                                                                                                    request,
                                                                                                                                                    CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= GetCertificateStatusResponse.Failed(request);

                                            #endregion

                                            #region Send OnGetCertificateStatusResponse event

                                            try
                                            {

                                                OnGetCertificateStatusResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request,
                                                                                       response,
                                                                                       response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'GetCertificateStatus' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'GetCertificateStatus' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnGetCertificateStatusWSResponse event

                                    try
                                    {

                                        OnGetCertificateStatusWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json,
                                                                                 OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region GetCRL

                            case "GetCRL":
                                {

                                    #region Send OnGetCRLWSRequest event

                                    try
                                    {

                                        OnGetCRLWSRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (GetCRLRequest.TryParse(requestData,
                                                                   requestId.Value,
                                                                   chargingStationId.Value,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomGetCRLRequestParser) && request is not null) {

                                            #region Send OnGetCRLRequest event

                                            try
                                            {

                                                OnGetCRLRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            GetCRLResponse? response = null;

                                            var responseTasks = OnGetCRL?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnGetCRLDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= GetCRLResponse.Failed(request);

                                            #endregion

                                            #region Send OnGetCRLResponse event

                                            try
                                            {

                                                OnGetCRLResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         response,
                                                                         response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'GetCRL' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'GetCRL' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnGetCRLWSResponse event

                                    try
                                    {

                                        OnGetCRLWSResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   json,
                                                                   OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region ReservationStatusUpdate

                            case "ReservationStatusUpdate":
                                {

                                    #region Send OnReservationStatusUpdateWSRequest event

                                    try
                                    {

                                        OnReservationStatusUpdateWSRequest?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (ReservationStatusUpdateRequest.TryParse(requestData,
                                                                                    requestId.Value,
                                                                                    chargingStationId.Value,
                                                                                    out var request,
                                                                                    out var errorResponse,
                                                                                    CustomReservationStatusUpdateRequestParser) && request is not null) {

                                            #region Send OnReservationStatusUpdateRequest event

                                            try
                                            {

                                                OnReservationStatusUpdateRequest?.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            ReservationStatusUpdateResponse? response = null;

                                            var responseTasks = OnReservationStatusUpdate?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnReservationStatusUpdateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                       this,
                                                                                                                                                       request,
                                                                                                                                                       CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= ReservationStatusUpdateResponse.Failed(request);

                                            #endregion

                                            #region Send OnReservationStatusUpdateResponse event

                                            try
                                            {

                                                OnReservationStatusUpdateResponse?.Invoke(Timestamp.Now,
                                                                                          this,
                                                                                          request,
                                                                                          response,
                                                                                          response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'ReservationStatusUpdate' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'ReservationStatusUpdate' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnReservationStatusUpdateWSResponse event

                                    try
                                    {

                                        OnReservationStatusUpdateWSResponse?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    json,
                                                                                    OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region Authorize

                            case "Authorize":
                                {

                                    #region Send OnAuthorizeWSRequest event

                                    try
                                    {

                                        OnAuthorizeWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (AuthorizeRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargingStationId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomAuthorizeRequestParser) && request is not null) {

                                            #region Send OnAuthorizeRequest event

                                            try
                                            {

                                                OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            AuthorizeResponse? response = null;

                                            var responseTasks = OnAuthorize?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= AuthorizeResponse.Failed(request);

                                            #endregion

                                            #region Send OnAuthorizeResponse event

                                            try
                                            {

                                                OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'Authorize' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'Authorize' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnAuthorizeWSResponse event

                                    try
                                    {

                                        OnAuthorizeWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      json,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyEVChargingNeeds

                            case "NotifyEVChargingNeeds":
                                {

                                    #region Send OnNotifyEVChargingNeedsWSRequest event

                                    try
                                    {

                                        OnNotifyEVChargingNeedsWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyEVChargingNeedsRequest.TryParse(requestData,
                                                                                  requestId.Value,
                                                                                  chargingStationId.Value,
                                                                                  out var request,
                                                                                  out var errorResponse,
                                                                                  CustomNotifyEVChargingNeedsRequestParser) && request is not null) {

                                            #region Send OnNotifyEVChargingNeedsRequest event

                                            try
                                            {

                                                OnNotifyEVChargingNeedsRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyEVChargingNeedsResponse? response = null;

                                            var responseTasks = OnNotifyEVChargingNeeds?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyEVChargingNeedsDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                     this,
                                                                                                                                                     request,
                                                                                                                                                     CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyEVChargingNeedsResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyEVChargingNeedsResponse event

                                            try
                                            {

                                                OnNotifyEVChargingNeedsResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyEVChargingNeeds' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyEVChargingNeeds' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyEVChargingNeedsWSResponse event

                                    try
                                    {

                                        OnNotifyEVChargingNeedsWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json,
                                                                                  OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region TransactionEvent

                            case "TransactionEvent":
                                {

                                    #region Send OnTransactionEventWSRequest event

                                    try
                                    {

                                        OnTransactionEventWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (TransactionEventRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargingStationId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomTransactionEventRequestParser) && request is not null) {

                                            #region Send OnTransactionEventRequest event

                                            try
                                            {

                                                OnTransactionEventRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            TransactionEventResponse? response = null;

                                            var responseTasks = OnTransactionEvent?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnTransactionEventDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                this,
                                                                                                                                                request,
                                                                                                                                                CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= TransactionEventResponse.Failed(request);

                                            #endregion

                                            #region Send OnTransactionEventResponse event

                                            try
                                            {

                                                OnTransactionEventResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'TransactionEvent' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'TransactionEvent' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnTransactionEventWSResponse event

                                    try
                                    {

                                        OnTransactionEventWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             json,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region StatusNotification

                            case "StatusNotification":
                                {

                                    #region Send OnStatusNotificationWSRequest event

                                    try
                                    {

                                        OnStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (StatusNotificationRequest.TryParse(requestData,
                                                                               requestId.Value,
                                                                               chargingStationId.Value,
                                                                               out var request,
                                                                               out var errorResponse,
                                                                               CustomStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnStatusNotificationRequest event

                                            try
                                            {

                                                OnStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            StatusNotificationResponse? response = null;

                                            var responseTasks = OnStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                  this,
                                                                                                                                                  request,
                                                                                                                                                  CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= StatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnStatusNotificationResponse event

                                            try
                                            {

                                                OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request,
                                                                                     response,
                                                                                     response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'StatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'StatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnStatusNotificationWSResponse event

                                    try
                                    {

                                        OnStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               json,
                                                                               OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region MeterValues

                            case "MeterValues":
                                {

                                    #region Send OnMeterValuesWSRequest event

                                    try
                                    {

                                        OnMeterValuesWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (MeterValuesRequest.TryParse(requestData,
                                                                        requestId.Value,
                                                                        chargingStationId.Value,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomMeterValuesRequestParser) && request is not null) {

                                            #region Send OnMeterValuesRequest event

                                            try
                                            {

                                                OnMeterValuesRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            MeterValuesResponse? response = null;

                                            var responseTasks = OnMeterValues?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                           this,
                                                                                                                                           request,
                                                                                                                                           CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= MeterValuesResponse.Failed(request);

                                            #endregion

                                            #region Send OnMeterValuesResponse event

                                            try
                                            {

                                                OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'MeterValues' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'MeterValues' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",     OCPPTextMessage),
                                                                    new JProperty("exception",   e.Message),
                                                                    new JProperty("stacktrace",  e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnMeterValuesWSResponse event

                                    try
                                    {

                                        OnMeterValuesWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        json,
                                                                        OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyChargingLimit

                            case "NotifyChargingLimit":
                                {

                                    #region Send OnNotifyChargingLimitWSRequest event

                                    try
                                    {

                                        OnNotifyChargingLimitWSRequest?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyChargingLimitRequest.TryParse(requestData,
                                                                                requestId.Value,
                                                                                chargingStationId.Value,
                                                                                out var request,
                                                                                out var errorResponse,
                                                                                CustomNotifyChargingLimitRequestParser) && request is not null) {

                                            #region Send OnNotifyChargingLimitRequest event

                                            try
                                            {

                                                OnNotifyChargingLimitRequest?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyChargingLimitResponse? response = null;

                                            var responseTasks = OnNotifyChargingLimit?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyChargingLimitDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                   this,
                                                                                                                                                   request,
                                                                                                                                                   CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyChargingLimitResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyChargingLimitResponse event

                                            try
                                            {

                                                OnNotifyChargingLimitResponse?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request,
                                                                                      response,
                                                                                      response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyChargingLimit' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyChargingLimit' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyChargingLimitWSResponse event

                                    try
                                    {

                                        OnNotifyChargingLimitWSResponse?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                json,
                                                                                OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region ClearedChargingLimit

                            case "ClearedChargingLimit":
                                {

                                    #region Send OnClearedChargingLimitWSRequest event

                                    try
                                    {

                                        OnClearedChargingLimitWSRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (ClearedChargingLimitRequest.TryParse(requestData,
                                                                                 requestId.Value,
                                                                                 chargingStationId.Value,
                                                                                 out var request,
                                                                                 out var errorResponse,
                                                                                 CustomClearedChargingLimitRequestParser) && request is not null) {

                                            #region Send OnClearedChargingLimitRequest event

                                            try
                                            {

                                                OnClearedChargingLimitRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            ClearedChargingLimitResponse? response = null;

                                            var responseTasks = OnClearedChargingLimit?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnClearedChargingLimitDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                    this,
                                                                                                                                                    request,
                                                                                                                                                    CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= ClearedChargingLimitResponse.Failed(request);

                                            #endregion

                                            #region Send OnClearedChargingLimitResponse event

                                            try
                                            {

                                                OnClearedChargingLimitResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request,
                                                                                       response,
                                                                                       response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'ClearedChargingLimit' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'ClearedChargingLimit' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnClearedChargingLimitWSResponse event

                                    try
                                    {

                                        OnClearedChargingLimitWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json,
                                                                                 OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region ReportChargingProfiles

                            case "ReportChargingProfiles":
                                {

                                    #region Send OnReportChargingProfilesWSRequest event

                                    try
                                    {

                                        OnReportChargingProfilesWSRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (ReportChargingProfilesRequest.TryParse(requestData,
                                                                                   requestId.Value,
                                                                                   chargingStationId.Value,
                                                                                   out var request,
                                                                                   out var errorResponse,
                                                                                   CustomReportChargingProfilesRequestParser) && request is not null) {

                                            #region Send OnReportChargingProfilesRequest event

                                            try
                                            {

                                                OnReportChargingProfilesRequest?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            ReportChargingProfilesResponse? response = null;

                                            var responseTasks = OnReportChargingProfiles?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnReportChargingProfilesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                      this,
                                                                                                                                                      request,
                                                                                                                                                      CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= ReportChargingProfilesResponse.Failed(request);

                                            #endregion

                                            #region Send OnReportChargingProfilesResponse event

                                            try
                                            {

                                                OnReportChargingProfilesResponse?.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         response,
                                                                                         response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'ReportChargingProfiles' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'ReportChargingProfiles' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnReportChargingProfilesWSResponse event

                                    try
                                    {

                                        OnReportChargingProfilesWSResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   json,
                                                                                   OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyEVChargingSchedule

                            case "NotifyEVChargingSchedule":
                                {

                                    #region Send OnNotifyEVChargingScheduleWSRequest event

                                    try
                                    {

                                        OnNotifyEVChargingScheduleWSRequest?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyEVChargingScheduleRequest.TryParse(requestData,
                                                                                     requestId.Value,
                                                                                     chargingStationId.Value,
                                                                                     out var request,
                                                                                     out var errorResponse,
                                                                                     CustomNotifyEVChargingScheduleRequestParser) && request is not null) {

                                            #region Send OnNotifyEVChargingScheduleRequest event

                                            try
                                            {

                                                OnNotifyEVChargingScheduleRequest?.Invoke(Timestamp.Now,
                                                                                          this,
                                                                                          request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyEVChargingScheduleResponse? response = null;

                                            var responseTasks = OnNotifyEVChargingSchedule?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyEVChargingScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                        this,
                                                                                                                                                        request,
                                                                                                                                                        CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyEVChargingScheduleResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyEVChargingScheduleResponse event

                                            try
                                            {

                                                OnNotifyEVChargingScheduleResponse?.Invoke(Timestamp.Now,
                                                                                           this,
                                                                                           request,
                                                                                           response,
                                                                                           response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyEVChargingSchedule' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyEVChargingSchedule' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyEVChargingScheduleWSResponse event

                                    try
                                    {

                                        OnNotifyEVChargingScheduleWSResponse?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     json,
                                                                                     OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyPriorityCharging

                            case "NotifyPriorityCharging":
                                {

                                    #region Send OnNotifyPriorityChargingWSRequest event

                                    try
                                    {

                                        OnNotifyPriorityChargingWSRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyPriorityChargingRequest.TryParse(requestData,
                                                                                   requestId.Value,
                                                                                   chargingStationId.Value,
                                                                                   out var request,
                                                                                   out var errorResponse,
                                                                                   CustomNotifyPriorityChargingRequestParser) && request is not null) {

                                            #region Send OnNotifyPriorityChargingRequest event

                                            try
                                            {

                                                OnNotifyPriorityChargingRequest?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyPriorityChargingResponse? response = null;

                                            var responseTasks = OnNotifyPriorityCharging?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyPriorityChargingDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                      this,
                                                                                                                                                      request,
                                                                                                                                                      CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyPriorityChargingResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyPriorityChargingResponse event

                                            try
                                            {

                                                OnNotifyPriorityChargingResponse?.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         response,
                                                                                         response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyPriorityCharging' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyPriorityCharging' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyPriorityChargingWSResponse event

                                    try
                                    {

                                        OnNotifyPriorityChargingWSResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   json,
                                                                                   OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region PullDynamicScheduleUpdate

                            case "PullDynamicScheduleUpdate":
                                {

                                    #region Send OnPullDynamicScheduleUpdateWSRequest event

                                    try
                                    {

                                        OnPullDynamicScheduleUpdateWSRequest?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (PullDynamicScheduleUpdateRequest.TryParse(requestData,
                                                                                      requestId.Value,
                                                                                      chargingStationId.Value,
                                                                                      out var request,
                                                                                      out var errorResponse,
                                                                                      CustomPullDynamicScheduleUpdateRequestParser) && request is not null) {

                                            #region Send OnPullDynamicScheduleUpdateRequest event

                                            try
                                            {

                                                OnPullDynamicScheduleUpdateRequest?.Invoke(Timestamp.Now,
                                                                                           this,
                                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            PullDynamicScheduleUpdateResponse? response = null;

                                            var responseTasks = OnPullDynamicScheduleUpdate?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnPullDynamicScheduleUpdateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                         this,
                                                                                                                                                         request,
                                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= PullDynamicScheduleUpdateResponse.Failed(request);

                                            #endregion

                                            #region Send OnPullDynamicScheduleUpdateResponse event

                                            try
                                            {

                                                OnPullDynamicScheduleUpdateResponse?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request,
                                                                                            response,
                                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'PullDynamicScheduleUpdate' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'PullDynamicScheduleUpdate' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnPullDynamicScheduleUpdateWSResponse event

                                    try
                                    {

                                        OnPullDynamicScheduleUpdateWSResponse?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      json,
                                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region NotifyDisplayMessages

                            case "NotifyDisplayMessages":
                                {

                                    #region Send OnNotifyDisplayMessagesWSRequest event

                                    try
                                    {

                                        OnNotifyDisplayMessagesWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyDisplayMessagesWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyDisplayMessagesRequest.TryParse(requestData,
                                                                                  requestId.Value,
                                                                                  chargingStationId.Value,
                                                                                  out var request,
                                                                                  out var errorResponse,
                                                                                  CustomNotifyDisplayMessagesRequestParser) && request is not null) {

                                            #region Send OnNotifyDisplayMessagesRequest event

                                            try
                                            {

                                                OnNotifyDisplayMessagesRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyDisplayMessagesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyDisplayMessagesResponse? response = null;

                                            var responseTasks = OnNotifyDisplayMessages?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyDisplayMessagesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                     this,
                                                                                                                                                     request,
                                                                                                                                                     CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyDisplayMessagesResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyDisplayMessagesResponse event

                                            try
                                            {

                                                OnNotifyDisplayMessagesResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyDisplayMessagesResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyDisplayMessages' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyDisplayMessages' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyDisplayMessagesWSResponse event

                                    try
                                    {

                                        OnNotifyDisplayMessagesWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  json,
                                                                                  OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyDisplayMessagesWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region NotifyCustomerInformation

                            case "NotifyCustomerInformation":
                                {

                                    #region Send OnNotifyCustomerInformationWSRequest event

                                    try
                                    {

                                        OnNotifyCustomerInformationWSRequest?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     json);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (NotifyCustomerInformationRequest.TryParse(requestData,
                                                                                      requestId.Value,
                                                                                      chargingStationId.Value,
                                                                                      out var request,
                                                                                      out var errorResponse,
                                                                                      CustomNotifyCustomerInformationRequestParser) && request is not null) {

                                            #region Send OnNotifyCustomerInformationRequest event

                                            try
                                            {

                                                OnNotifyCustomerInformationRequest?.Invoke(Timestamp.Now,
                                                                                           this,
                                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            NotifyCustomerInformationResponse? response = null;

                                            var responseTasks = OnNotifyCustomerInformation?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnNotifyCustomerInformationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                                         this,
                                                                                                                                                         request,
                                                                                                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks!);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            response ??= NotifyCustomerInformationResponse.Failed(request);

                                            #endregion

                                            #region Send OnNotifyCustomerInformationResponse event

                                            try
                                            {

                                                OnNotifyCustomerInformationResponse?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request,
                                                                                            response,
                                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    ResultCodes.FormationViolation,
                                                                    "The given 'NotifyCustomerInformation' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPTextMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                ResultCodes.FormationViolation,
                                                                "Processing the given 'NotifyCustomerInformation' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPTextMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnNotifyCustomerInformationWSResponse event

                                    try
                                    {

                                        OnNotifyCustomerInformationWSResponse?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      json,
                                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            default:

                                DebugX.Log($"{nameof(CSMSWSServer)}: The OCPP message '{action}' is unkown!");

                                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                         requestId.Value,
                                                         ResultCodes.ProtocolError,
                                                         $"The OCPP message '{action}' is unkown!",
                                                         new JObject(
                                                             new JProperty("request", OCPPTextMessage)
                                                         )
                                                     );

                                break;

                        }

                    #region OnTextMessageResponseSent

                    var now = Timestamp.Now;

                    if (OCPPResponse is not null || OCPPErrorResponse is not null)
                    {

                        var logger = OnTextMessageResponseSent;
                        if (logger is not null)
                        {

                            var loggerTasks = logger.GetInvocationList().
                                                     OfType <OnWebSocketTextMessageResponseDelegate>().
                                                     Select (loggingDelegate => loggingDelegate.Invoke(now,
                                                                                                       this,
                                                                                                       Connection,
                                                                                                       eventTrackingId,
                                                                                                       RequestTimestamp,
                                                                                                       json.ToString(JSONFormatting),
                                                                                                       now,
                                                                                                       (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting))).

                                                                                                       //ToDo: For some use cases returning an error to a charging station might be useless!

                                                                                                       //OCPPResponse?.ToJSON()?.ToString(JSONFormatting));
                                                     ToArray();

                            try
                            {
                                await Task.WhenAll(loggerTasks);
                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageResponseSent));
                            }

                        }

                    }

                    #endregion

                }

                #endregion

                #region MessageType 3: CALLRESULT  (A response from a charging station)

                // [
                //     3,                         // MessageType: CALLRESULT
                //    "19223201",                 // The request identification copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                else if (json.Count             == 3         &&
                         json[0].Type == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 3         &&
                         json[1].Type == JTokenType.String   &&
                         json[2].Type == JTokenType.Object)
                {

                    if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                        requests.TryGetValue(requestId, out var request))
                    {

                        request.ResponseTimestamp  = Timestamp.Now;
                        request.Response           = json[2] as JObject;

                        #region OnTextMessageResponseReceived

                        try
                        {

                            OnTextMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  Connection,
                                                                  EventTracking_Id.New,
                                                                  request.Timestamp,
                                                                  request.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                  Timestamp.Now,
                                                                  request.Response?.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageResponseReceived));
                        }

                        #endregion

                    }

                    // No response to the charging station!

                }

                #endregion

                #region MessageType 4: CALLERROR   (A charging station reports an error for a received request)

                // [
                //     4,                         // MessageType: CALLERROR
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                // Error Code                    Description
                // -----------------------------------------------------------------------------------------------
                // NotImplemented                Requested Action is not known by receiver
                // NotSupported                  Requested Action is recognized but not supported by the receiver
                // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
                // ProtocolError                 Payload for Action is incomplete
                // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
                // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
                // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
                // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
                // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
                // GenericError                  Any other error not covered by the previous ones

                else if (json.Count             == 5                   &&
                         json[0].Type           == JTokenType.Integer  &&
                         json[0].Value<Byte>()  == 4                   &&
                         json[1].Type == JTokenType.String             &&
                         json[2].Type == JTokenType.String             &&
                         json[3].Type == JTokenType.String             &&
                         json[4].Type == JTokenType.Object)
                {

                    if (Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId) &&
                        requests.TryGetValue(requestId, out var request))
                    {

                        // ToDo: Refactor 
                        if (ResultCodes.TryParse(json[2]?.Value<String>() ?? "", out var errorCode))
                            request.ErrorCode = errorCode;
                        else
                            request.ErrorCode = ResultCodes.GenericError;

                        request.Response          = null;
                        request.ErrorDescription  = json[3]?.Value<String>();
                        request.ErrorDetails      = json[4] as JObject;

                        #region OnTextErrorResponseReceived

                        try
                        {

                            OnTextErrorResponseReceived?.Invoke(Timestamp.Now,
                                                                this,
                                                                Connection,
                                                                EventTracking_Id.New,
                                                                request.Timestamp,
                                                                request.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                Timestamp.Now,
                                                                request.Response?.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextErrorResponseReceived));
                        }

                        #endregion

                    }

                    // No response to the charging station!

                }

                #endregion

                #region Unknown message structure

                else
                {

                    OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                            Request_Id.Zero,
                                            ResultCodes.InternalError,
                                            $"{nameof(CSMSWSServer)} The OCPP message '{OCPPTextMessage}' is invalid!",
                                            new JObject(
                                                new JProperty("request", OCPPTextMessage)
                                            )
                                        );

                    DebugX.Log(OCPPErrorResponse.ErrorDescription);

                }

                #endregion

            }
            catch (Exception e)
            {
                #region Handle exception

                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                        Request_Id.Zero,
                                        ResultCodes.InternalError,
                                        $"The OCPP message '{OCPPTextMessage}' received in " + nameof(CSMSWSServer) + " led to an exception!",
                                        new JObject(
                                            new JProperty("request",      OCPPTextMessage),
                                            new JProperty("exception",    e.Message),
                                            new JProperty("stacktrace",   e.StackTrace)
                                        )
                                    );

                DebugX.LogException(e, OCPPErrorResponse.ErrorDescription);

                #endregion
            }


            #region OnTextErrorResponseSent

            if (OCPPErrorResponse is not null)
            {

                var now    = Timestamp.Now;
                var logger = OnTextErrorResponseSent;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnWebSocketTextErrorResponseDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(now,
                                                                                               this,
                                                                                               Connection,
                                                                                               eventTrackingId,
                                                                                               RequestTimestamp,
                                                                                               OCPPTextMessage,
                                                                                               now,
                                                                                               (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting))).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextErrorResponseSent));
                    }

                }

            }

            #endregion


            // The response to the charging station... might be empty!
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       OCPPTextMessage,
                       Timestamp.Now,
                       (OCPPResponse?.     ToJSON() ??
                        OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting) ?? String.Empty,
                       EventTrackingId
                   );

        }

        #endregion


        #region SendRequest(EventTrackingId, RequestId, ChargingStationId, OCPPAction, JSONPayload, RequestTimeout = null)

        public async Task<SendRequestState> SendRequest(EventTracking_Id  EventTrackingId,
                                                        Request_Id        RequestId,
                                                        ChargingStation_Id      ChargingStationId,
                                                        String            OCPPAction,
                                                        JObject           JSONPayload,
                                                        TimeSpan?         RequestTimeout   = null)
        {

            var endTime         = Timestamp.Now + (RequestTimeout ?? this.RequestTimeout ?? DefaultRequestTimeout);

            var sendJSONResult  = await SendJSON(
                                      EventTrackingId,
                                      RequestId,
                                      ChargingStationId,
                                      OCPPAction,
                                      JSONPayload,
                                      endTime
                                  );

            if (sendJSONResult == SendJSONResults.Success) {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25);

                        if (requests.TryGetValue(RequestId, out var sendRequestState1) &&
                            sendRequestState1?.Response is not null ||
                            sendRequestState1?.ErrorCode.HasValue == true)
                        {

                            requests.TryRemove(RequestId, out _);

                            return sendRequestState1;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(CSMSWSServer), ".", nameof(SendRequest), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                if (requests.TryGetValue(RequestId, out var sendRequestState2) && sendRequestState2 is not null)
                {
                    sendRequestState2.ErrorCode = ResultCodes.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                if (requests.TryGetValue(RequestId, out var sendRequestState3) && sendRequestState3 is not null)
                {
                    sendRequestState3.ErrorCode = ResultCodes.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState3;
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return new SendRequestState(
                       Timestamp:           now,
                       ChargingStationId:         ChargingStationId,
                       WSRequestMessage:    new OCPP_WebSocket_RequestMessage(
                                                RequestId,
                                                OCPPAction,
                                                JSONPayload
                                            ),
                       Timeout:             now,
                       ResponseTimestamp:   now,
                       Response:            new JObject(),
                       ErrorCode:           ResultCodes.InternalError,
                       ErrorDescription:    null,
                       ErrorDetails:        null
                   );

        }

        #endregion

        #region SendJSON   (EventTrackingId, RequestId, ChargingStationId, Action, JSON, RequestTimeout)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="ChargingStationId">A charging station identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="JSON">The JSON payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public async Task<SendJSONResults> SendJSON(EventTracking_Id  EventTrackingId,
                                                    Request_Id        RequestId,
                                                    ChargingStation_Id      ChargingStationId,
                                                    String            Action,
                                                    JObject           JSON,
                                                    DateTime          RequestTimeout)
        {

            var wsRequestMessage  = new OCPP_WebSocket_RequestMessage(
                                        RequestId,
                                        Action,
                                        JSON
                                    );

            var ocppTextMessage   = wsRequestMessage.ToJSON().ToString(Formatting.None);

            try
            {

                var webSocketConnections  = WebSocketConnections.Where  (ws => ws.TryGetCustomDataAs<ChargingStation_Id>(chargingStationId_WebSocketKey) == ChargingStationId).
                                                                 ToArray();

                if (webSocketConnections.Any())
                {

                    requests.TryAdd(RequestId,
                                    new SendRequestState(
                                        Timestamp.Now,
                                        ChargingStationId,
                                        wsRequestMessage,
                                        RequestTimeout
                                    ));

                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        #region OnTextMessageRequestSent

                        var requestLogger = OnTextMessageRequestSent;
                        if (requestLogger is not null)
                        {

                            var loggerTasks = requestLogger.GetInvocationList().
                                                            OfType <OnWebSocketTextMessageDelegate>().
                                                            Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                              this,
                                                                                                              webSocketConnection,
                                                                                                              EventTrackingId,
                                                                                                              ocppTextMessage)).
                                                            ToArray();

                            try
                            {
                                await Task.WhenAll(loggerTasks);
                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTextMessageRequestSent));
                            }

                        }

                        #endregion

                        var success = await SendTextMessage(webSocketConnection,
                                                            ocppTextMessage,
                                                            EventTrackingId);

                        if (success == SendStatus.Success)
                            break;

                        else
                            RemoveConnection(webSocketConnection);

                    }

                    return SendJSONResults.Success;

                }
                else
                    return SendJSONResults.UnknownClient;

            }
            catch (Exception)
            {
                return SendJSONResults.TransmissionFailed;
            }

        }

        #endregion


        #region CSMS -> Charging Station Messages

        #region Reset                      (Request)

        public async Task<ResetResponse> Reset(ResetRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetRequest));
            }

            #endregion


            ResetResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomResetRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ResetResponse.TryParse(Request,
                                           sendRequestState.Response,
                                           out var resetResponse,
                                           out var errorResponse) &&
                    resetResponse is not null)
                {
                    response = resetResponse;
                }

                response ??= new ResetResponse(Request,
                                               Result.Format(errorResponse));

            }

            response ??= new ResetResponse(Request,
                                           Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware             (Request)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUpdateFirmwareRequestSerializer,
                                                         CustomFirmwareSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var updateFirmwareResponse,
                                                    out var errorResponse) &&
                    updateFirmwareResponse is not null)
                {
                    response = updateFirmwareResponse;
                }

                response ??= new UpdateFirmwareResponse(Request,
                                                        Result.Format(errorResponse));

            }

            response ??= new UpdateFirmwareResponse(Request,
                                                    Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PublishFirmware            (Request)

        public async Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareRequest));
            }

            #endregion


            PublishFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomPublishFirmwareRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (PublishFirmwareResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var publishFirmwareResponse,
                                                     out var errorResponse) &&
                    publishFirmwareResponse is not null)
                {
                    response = publishFirmwareResponse;
                }

                response ??= new PublishFirmwareResponse(Request,
                                                         Result.Format(errorResponse));

            }

            response ??= new PublishFirmwareResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnpublishFirmware          (Request)

        public async Task<UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnpublishFirmwareRequest));
            }

            #endregion


            UnpublishFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUnpublishFirmwareRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UnpublishFirmwareResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var unpublishFirmwareResponse,
                                                       out var errorResponse) &&
                    unpublishFirmwareResponse is not null)
                {
                    response = unpublishFirmwareResponse;
                }

                response ??= new UnpublishFirmwareResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new UnpublishFirmwareResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnpublishFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetBaseReport              (Request)

        public async Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetBaseReportRequest));
            }

            #endregion


            GetBaseReportResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetBaseReportRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetBaseReportResponse.TryParse(Request,
                                                   sendRequestState.Response,
                                                   out var getBaseReportResponse,
                                                   out var errorResponse) &&
                    getBaseReportResponse is not null)
                {
                    response = getBaseReportResponse;
                }

                response ??= new GetBaseReportResponse(Request,
                                                       Result.Format(errorResponse));

            }

            response ??= new GetBaseReportResponse(Request,
                                                   Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetBaseReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetReport                  (Request)

        public async Task<GetReportResponse> GetReport(GetReportRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetReportRequest));
            }

            #endregion


            GetReportResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetReportRequestSerializer,
                                                         CustomComponentVariableSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomVariableSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetReportResponse.TryParse(Request,
                                               sendRequestState.Response,
                                               out var getReport,
                                               out var errorResponse) &&
                    getReport is not null)
                {
                    response = getReport;
                }

                response ??= new GetReportResponse(Request,
                                                   Result.Format(errorResponse));

            }

            response ??= new GetReportResponse(Request,
                                               Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                     (Request)

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        public async Task<GetLogResponse> GetLog(GetLogRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            GetLogResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetLogRequestSerializer,
                                                         CustomLogParametersSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetLogResponse.TryParse(Request,
                                            sendRequestState.Response,
                                            out var getLogResponse,
                                            out var errorResponse) &&
                    getLogResponse is not null)
                {
                    response = getLogResponse;
                }

                response ??= new GetLogResponse(Request,
                                                Result.Format(errorResponse));

            }

            response ??= new GetLogResponse(Request,
                                            Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetVariables               (Request)

        public async Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariablesRequest));
            }

            #endregion


            SetVariablesResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetVariablesRequestSerializer,
                                                         CustomSetVariableDataSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomVariableSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetVariablesResponse.TryParse(Request,
                                                  sendRequestState.Response,
                                                  out var setVariablesResponse,
                                                  out var errorResponse) &&
                    setVariablesResponse is not null)
                {
                    response = setVariablesResponse;
                }

                response ??= new SetVariablesResponse(Request,
                                                      Result.Format(errorResponse));

            }

            response ??= new SetVariablesResponse(Request,
                                                  Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetVariables               (Request)

        public async Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetVariablesRequest));
            }

            #endregion


            GetVariablesResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetVariablesRequestSerializer,
                                                         CustomGetVariableDataSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomVariableSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetVariablesResponse.TryParse(Request,
                                                  sendRequestState.Response,
                                                  out var getVariablesResponse,
                                                  out var errorResponse) &&
                    getVariablesResponse is not null)
                {
                    response = getVariablesResponse;
                }

                response ??= new GetVariablesResponse(Request,
                                                      Result.Format(errorResponse));

            }

            response ??= new GetVariablesResponse(Request,
                                                  Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetMonitoringBase          (Request)

        public async Task<SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetMonitoringBaseRequest));
            }

            #endregion


            SetMonitoringBaseResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetMonitoringBaseRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetMonitoringBaseResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setMonitoringBaseResponse,
                                                       out var errorResponse) &&
                    setMonitoringBaseResponse is not null)
                {
                    response = setMonitoringBaseResponse;
                }

                response ??= new SetMonitoringBaseResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new SetMonitoringBaseResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetMonitoringBaseResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetMonitoringReport        (Request)

        public async Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetMonitoringReportRequest));
            }

            #endregion


            GetMonitoringReportResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetMonitoringReportRequestSerializer,
                                                         CustomComponentVariableSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomVariableSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetMonitoringReportResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var getMonitoringReportResponse,
                                                         out var errorResponse) &&
                    getMonitoringReportResponse is not null)
                {
                    response = getMonitoringReportResponse;
                }

                response ??= new GetMonitoringReportResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new GetMonitoringReportResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetMonitoringLevel         (Request)

        public async Task<SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetMonitoringLevelRequest));
            }

            #endregion


            SetMonitoringLevelResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetMonitoringLevelRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetMonitoringLevelResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var setMonitoringLevelResponse,
                                                        out var errorResponse) &&
                    setMonitoringLevelResponse is not null)
                {
                    response = setMonitoringLevelResponse;
                }

                response ??= new SetMonitoringLevelResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new SetMonitoringLevelResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetMonitoringLevelResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetVariableMonitoring      (Request)

        public async Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariableMonitoringRequest));
            }

            #endregion


            SetVariableMonitoringResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetVariableMonitoringRequestSerializer,
                                                         CustomSetMonitoringDataSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomVariableSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetVariableMonitoringResponse.TryParse(Request,
                                                           sendRequestState.Response,
                                                           out var setVariableMonitoringResponse,
                                                           out var errorResponse) &&
                    setVariableMonitoringResponse is not null)
                {
                    response = setVariableMonitoringResponse;
                }

                response ??= new SetVariableMonitoringResponse(Request,
                                                               Result.Format(errorResponse));

            }

            response ??= new SetVariableMonitoringResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariableMonitoringResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearVariableMonitoring    (Request)

        public async Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearVariableMonitoringRequest));
            }

            #endregion


            ClearVariableMonitoringResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomClearVariableMonitoringRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ClearVariableMonitoringResponse.TryParse(Request,
                                                             sendRequestState.Response,
                                                             out var clearVariableMonitoringResponse,
                                                             out var errorResponse) &&
                    clearVariableMonitoringResponse is not null)
                {
                    response = clearVariableMonitoringResponse;
                }

                response ??= new ClearVariableMonitoringResponse(Request,
                                                                 Result.Format(errorResponse));

            }

            response ??= new ClearVariableMonitoringResponse(Request,
                                                             Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearVariableMonitoringResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetNetworkProfile          (Request)

        public async Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetNetworkProfileRequest));
            }

            #endregion


            SetNetworkProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetNetworkProfileRequestSerializer,
                                                         CustomNetworkConnectionProfileSerializer,
                                                         CustomVPNConfigurationSerializer,
                                                         CustomAPNConfigurationSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetNetworkProfileResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setNetworkProfileResponse,
                                                       out var errorResponse) &&
                    setNetworkProfileResponse is not null)
                {
                    response = setNetworkProfileResponse;
                }

                response ??= new SetNetworkProfileResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new SetNetworkProfileResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetNetworkProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability         (Request)

        public async Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            ChangeAvailabilityResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomChangeAvailabilityRequestSerializer,
                                                         CustomEVSESerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var changeAvailabilityResponse,
                                                        out var errorResponse) &&
                    changeAvailabilityResponse is not null)
                {
                    response = changeAvailabilityResponse;
                }

                response ??= new ChangeAvailabilityResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new ChangeAvailabilityResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage             (Request)

        public async Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            TriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomTriggerMessageRequestSerializer,
                                                         CustomEVSESerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (TriggerMessageResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var triggerMessageResponse,
                                                    out var errorResponse) &&
                    triggerMessageResponse is not null)
                {
                    response = triggerMessageResponse;
                }

                response ??= new TriggerMessageResponse(Request,
                                                        Result.Format(errorResponse));

            }

            response ??= new TriggerMessageResponse(Request,
                                                    Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData               (Request)

        public async Task<CS.DataTransferResponse> TransferData(DataTransferRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CS.DataTransferResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomDataTransferRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CS.DataTransferResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var dataTransferResponse,
                                                     out var errorResponse) &&
                    dataTransferResponse is not null)
                {
                    response = dataTransferResponse;
                }

                response ??= new CS.DataTransferResponse(Request,
                                                         Result.Format(errorResponse));

            }

            response ??= new CS.DataTransferResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region CertificateSigned          (Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        public async Task<CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            CertificateSignedResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomCertificateSignedRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CertificateSignedResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var certificateSignedResponse,
                                                       out var errorResponse) &&
                    certificateSignedResponse is not null)
                {
                    response = certificateSignedResponse;
                }

                response ??= new CertificateSignedResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new CertificateSignedResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate         (Request)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        public async Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            InstallCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomInstallCertificateRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (InstallCertificateResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var installCertificateResponse,
                                                        out var errorResponse) &&
                    installCertificateResponse is not null)
                {
                    response = installCertificateResponse;
                }

                response ??= new InstallCertificateResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new InstallCertificateResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds (Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        public async Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            GetInstalledCertificateIdsResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetInstalledCertificateIdsRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetInstalledCertificateIdsResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var getInstalledCertificateIdsResponse,
                                                                out var errorResponse) &&
                    getInstalledCertificateIdsResponse is not null)
                {
                    response = getInstalledCertificateIdsResponse;
                }

                response ??= new GetInstalledCertificateIdsResponse(Request,
                                                                    Result.Format(errorResponse));

            }

            response ??= new GetInstalledCertificateIdsResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate          (Request)

        /// <summary>
        /// Delete the given certificate on the charging station.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        public async Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            DeleteCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomDeleteCertificateRequestSerializer,
                                                         CustomCertificateHashDataSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (DeleteCertificateResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var deleteCertificateResponse,
                                                       out var errorResponse) &&
                    deleteCertificateResponse is not null)
                {
                    response = deleteCertificateResponse;
                }

                response ??= new DeleteCertificateResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new DeleteCertificateResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCRL                  (Request)

        /// <summary>
        /// Notify the charging station about the status of a certificate revocation list.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        public async Task<NotifyCRLResponse> NotifyCRLAvailability(NotifyCRLRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCRLRequest));
            }

            #endregion


            NotifyCRLResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomNotifyCRLRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (NotifyCRLResponse.TryParse(Request,
                                               sendRequestState.Response,
                                               out var deleteCertificateResponse,
                                               out var errorResponse) &&
                    deleteCertificateResponse is not null)
                {
                    response = deleteCertificateResponse;
                }

                response ??= new NotifyCRLResponse(
                                 Request,
                                 Result.Format(errorResponse)
                             );

            }

            response ??= new NotifyCRLResponse(
                             Request,
                             Result.FromSendRequestState(sendRequestState)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion        (Request)

        public async Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            GetLocalListVersionResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetLocalListVersionRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetLocalListVersionResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var getLocalListVersionResponse,
                                                         out var errorResponse) &&
                    getLocalListVersionResponse is not null)
                {
                    response = getLocalListVersionResponse;
                }

                response ??= new GetLocalListVersionResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new GetLocalListVersionResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList              (Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            SendLocalListResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
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
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SendLocalListResponse.TryParse(Request,
                                                   sendRequestState.Response,
                                                   out var sendLocalListResponse,
                                                   out var errorResponse) &&
                    sendLocalListResponse is not null)
                {
                    response = sendLocalListResponse;
                }

                response ??= new SendLocalListResponse(Request,
                                                       Result.Format(errorResponse));

            }

            response ??= new SendLocalListResponse(Request,
                                                   Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache                 (Request)

        public async Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            ClearCacheResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomClearCacheRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ClearCacheResponse.TryParse(Request,
                                                sendRequestState.Response,
                                                out var clearCacheResponse,
                                                out var errorResponse) &&
                    clearCacheResponse is not null)
                {
                    response = clearCacheResponse;
                }

                response ??= new ClearCacheResponse(Request,
                                                    Result.Format(errorResponse));

            }

            response ??= new ClearCacheResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow                 (Request)

        public async Task<ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            ReserveNowResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomReserveNowRequestSerializer,
                                                         CustomIdTokenSerializer,
                                                         CustomAdditionalInfoSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ReserveNowResponse.TryParse(Request,
                                                sendRequestState.Response,
                                                out var reserveNowResponse,
                                                out var errorResponse) &&
                    reserveNowResponse is not null)
                {
                    response = reserveNowResponse;
                }

                response ??= new ReserveNowResponse(Request,
                                                    Result.Format(errorResponse));

            }

            response ??= new ReserveNowResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation          (Request)

        public async Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            CancelReservationResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomCancelReservationRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CancelReservationResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var cancelReservationResponse,
                                                       out var errorResponse) &&
                    cancelReservationResponse is not null)
                {
                    response = cancelReservationResponse;
                }

                response ??= new CancelReservationResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new CancelReservationResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartCharging              (Request)

        public async Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionRequest));
            }

            #endregion


            RequestStartTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
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

                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer

                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (RequestStartTransactionResponse.TryParse(Request,
                                                             sendRequestState.Response,
                                                             out var requestStartTransactionResponse,
                                                             out var errorResponse) &&
                    requestStartTransactionResponse is not null)
                {
                    response = requestStartTransactionResponse;
                }

                response ??= new RequestStartTransactionResponse(Request,
                                                                 Result.Format(errorResponse));

            }

            response ??= new RequestStartTransactionResponse(Request,
                                                             Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopCharging               (Request)

        public async Task<RequestStopTransactionResponse> StopCharging(RequestStopTransactionRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStopTransactionRequest));
            }

            #endregion


            RequestStopTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomRequestStopTransactionRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (RequestStopTransactionResponse.TryParse(Request,
                                                            sendRequestState.Response,
                                                            out var requestStopTransactionResponse,
                                                            out var errorResponse) &&
                    requestStopTransactionResponse is not null)
                {
                    response = requestStopTransactionResponse;
                }

                response ??= new RequestStopTransactionResponse(Request,
                                                                Result.Format(errorResponse));

            }

            response ??= new RequestStopTransactionResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetTransactionStatus       (Request)

        public async Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetTransactionStatusRequest));
            }

            #endregion


            GetTransactionStatusResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetTransactionStatusRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetTransactionStatusResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var getTransactionStatusResponse,
                                                          out var errorResponse) &&
                    getTransactionStatusResponse is not null)
                {
                    response = getTransactionStatusResponse;
                }

                response ??= new GetTransactionStatusResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new GetTransactionStatusResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetTransactionStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile         (Request)

        public async Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            SetChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
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
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetChargingProfileResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var setChargingProfileResponse,
                                                        out var errorResponse) &&
                    setChargingProfileResponse is not null)
                {
                    response = setChargingProfileResponse;
                }

                response ??= new SetChargingProfileResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new SetChargingProfileResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetChargingProfiles        (Request)

        public async Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetChargingProfilesRequest));
            }

            #endregion


            GetChargingProfilesResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetChargingProfilesRequestSerializer,
                                                         CustomChargingProfileCriterionSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetChargingProfilesResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var getChargingProfilesResponse,
                                                         out var errorResponse) &&
                    getChargingProfilesResponse is not null)
                {
                    response = getChargingProfilesResponse;
                }

                response ??= new GetChargingProfilesResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new GetChargingProfilesResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile       (Request)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomClearChargingProfileRequestSerializer,
                                                         CustomClearChargingProfileSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse) &&
                    clearChargingProfileResponse is not null)
                {
                    response = clearChargingProfileResponse;
                }

                response ??= new ClearChargingProfileResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new ClearChargingProfileResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule       (Request)


        public async Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            GetCompositeScheduleResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetCompositeScheduleRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetCompositeScheduleResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var getCompositeScheduleResponse,
                                                          out var errorResponse) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new GetCompositeScheduleResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new GetCompositeScheduleResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateDynamicSchedule      (Request)


        public async Task<UpdateDynamicScheduleResponse> UpdateDynamicSchedule(UpdateDynamicScheduleRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateDynamicScheduleRequest));
            }

            #endregion


            UpdateDynamicScheduleResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUpdateDynamicScheduleRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UpdateDynamicScheduleResponse.TryParse(Request,
                                                           sendRequestState.Response,
                                                           out var getCompositeScheduleResponse,
                                                           out var errorResponse) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new UpdateDynamicScheduleResponse(Request,
                                                               Result.Format(errorResponse));

            }

            response ??= new UpdateDynamicScheduleResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateDynamicScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyAllowedEnergyTransfer(Request)


        public async Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyAllowedEnergyTransferRequest));
            }

            #endregion


            NotifyAllowedEnergyTransferResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (NotifyAllowedEnergyTransferResponse.TryParse(Request,
                                                                 sendRequestState.Response,
                                                                 out var getCompositeScheduleResponse,
                                                                 out var errorResponse) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new NotifyAllowedEnergyTransferResponse(Request,
                                                                     Result.Format(errorResponse));

            }

            response ??= new NotifyAllowedEnergyTransferResponse(Request,
                                                                 Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyAllowedEnergyTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UsePriorityCharging        (Request)


        public async Task<UsePriorityChargingResponse> UsePriorityCharging(UsePriorityChargingRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUsePriorityChargingRequest));
            }

            #endregion


            UsePriorityChargingResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUsePriorityChargingRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UsePriorityChargingResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var getCompositeScheduleResponse,
                                                         out var errorResponse) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new UsePriorityChargingResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new UsePriorityChargingResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUsePriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector            (Request)

        public async Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            UnlockConnectorResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUnlockConnectorRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UnlockConnectorResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var unlockConnectorResponse,
                                                     out var errorResponse) &&
                    unlockConnectorResponse is not null)
                {
                    response = unlockConnectorResponse;
                }

                response ??= new UnlockConnectorResponse(Request,
                                                         Result.Format(errorResponse));

            }

            response ??= new UnlockConnectorResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region AFRRSignal                 (Request)

        public async Task<AFRRSignalResponse> SendAFRRSignal(AFRRSignalRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAFRRSignalRequest));
            }

            #endregion


            AFRRSignalResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomAFRRSignalRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (AFRRSignalResponse.TryParse(Request,
                                                sendRequestState.Response,
                                                out var unlockConnectorResponse,
                                                out var errorResponse) &&
                    unlockConnectorResponse is not null)
                {
                    response = unlockConnectorResponse;
                }

                response ??= new AFRRSignalResponse(Request,
                                                    Result.Format(errorResponse));

            }

            response ??= new AFRRSignalResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAFRRSignalResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SetDisplayMessage          (Request)

        public async Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDisplayMessageRequest));
            }

            #endregion


            SetDisplayMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetDisplayMessageRequestSerializer,
                                                         CustomMessageInfoSerializer,
                                                         CustomMessageContentSerializer,
                                                         CustomComponentSerializer,
                                                         CustomEVSESerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetDisplayMessageResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new SetDisplayMessageResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new SetDisplayMessageResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDisplayMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDisplayMessages         (Request)

        public async Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDisplayMessagesRequest));
            }

            #endregion


            GetDisplayMessagesResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetDisplayMessagesRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetDisplayMessagesResponse.TryParse(Request,
                                                        sendRequestState.Response,
                                                        out var getDisplayMessagesResponse,
                                                        out var errorResponse) &&
                    getDisplayMessagesResponse is not null)
                {
                    response = getDisplayMessagesResponse;
                }

                response ??= new GetDisplayMessagesResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new GetDisplayMessagesResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearDisplayMessage        (Request)

        public async Task<ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearDisplayMessageRequest));
            }

            #endregion


            ClearDisplayMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomClearDisplayMessageRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ClearDisplayMessageResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var clearDisplayMessageResponse,
                                                         out var errorResponse) &&
                    clearDisplayMessageResponse is not null)
                {
                    response = clearDisplayMessageResponse;
                }

                response ??= new ClearDisplayMessageResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new ClearDisplayMessageResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearDisplayMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendCostUpdated            (Request)

        public async Task<CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCostUpdatedRequest));
            }

            #endregion


            CostUpdatedResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomCostUpdatedRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CostUpdatedResponse.TryParse(Request,
                                                 sendRequestState.Response,
                                                 out var costUpdatedResponse,
                                                 out var errorResponse) &&
                    costUpdatedResponse is not null)
                {
                    response = costUpdatedResponse;
                }

                response ??= new CostUpdatedResponse(Request,
                                                     Result.Format(errorResponse));

            }

            response ??= new CostUpdatedResponse(Request,
                                                 Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCostUpdatedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RequestCustomerInformation (Request)

        public async Task<CustomerInformationResponse> RequestCustomerInformation(CustomerInformationRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCustomerInformationRequest));
            }

            #endregion


            CustomerInformationResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomCustomerInformationRequestSerializer,
                                                         CustomIdTokenSerializer,
                                                         CustomAdditionalInfoSerializer,
                                                         CustomCertificateHashDataSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CustomerInformationResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var customerInformationResponse,
                                                         out var errorResponse) &&
                    customerInformationResponse is not null)
                {
                    response = customerInformationResponse;
                }

                response ??= new CustomerInformationResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new CustomerInformationResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy          (Request)

        public async Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAddSignaturePolicyRequest));
            }

            #endregion


            AddSignaturePolicyResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomAddSignaturePolicyRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (AddSignaturePolicyResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new AddSignaturePolicyResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new AddSignaturePolicyResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAddSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateSignaturePolicy       (Request)

        public async Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateSignaturePolicyRequest));
            }

            #endregion


            UpdateSignaturePolicyResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomUpdateSignaturePolicyRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UpdateSignaturePolicyResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new UpdateSignaturePolicyResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new UpdateSignaturePolicyResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteSignaturePolicy       (Request)

        public async Task<DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteSignaturePolicyRequest));
            }

            #endregion


            DeleteSignaturePolicyResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomDeleteSignaturePolicyRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (DeleteSignaturePolicyResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new DeleteSignaturePolicyResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new DeleteSignaturePolicyResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region AddUserRole                 (Request)

        public async Task<AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAddUserRoleRequest));
            }

            #endregion


            AddUserRoleResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomAddUserRoleRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (AddUserRoleResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new AddUserRoleResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new AddUserRoleResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAddUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateUserRole              (Request)

        public async Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateUserRoleRequest));
            }

            #endregion


            UpdateUserRoleResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomUpdateUserRoleRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UpdateUserRoleResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new UpdateUserRoleResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new UpdateUserRoleResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteUserRole              (Request)

        public async Task<DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteUserRoleRequest));
            }

            #endregion


            DeleteUserRoleResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         //CustomDeleteUserRoleRequestSerializer,
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (DeleteUserRoleResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var setDisplayMessageResponse,
                                                       out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new DeleteUserRoleResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new DeleteUserRoleResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnDeleteUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff    (Request)

        public async Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDefaultChargingTariffRequest));
            }

            #endregion


            SetDefaultChargingTariffResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomSetDefaultChargingTariffRequestSerializer,
                                                         CustomChargingTariffSerializer,
                                                         CustomPriceSerializer,
                                                         CustomTariffElementSerializer,
                                                         CustomPriceComponentSerializer,
                                                         CustomTariffRestrictionsSerializer,
                                                         CustomEnergyMixSerializer,
                                                         CustomEnergySourceSerializer,
                                                         CustomEnvironmentalImpactSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (SetDefaultChargingTariffResponse.TryParse(Request,
                                                              sendRequestState.Response,
                                                              out var setDisplayMessageResponse,
                                                              out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new SetDefaultChargingTariffResponse(Request,
                                                                  Result.Format(errorResponse));

            }

            response ??= new SetDefaultChargingTariffResponse(Request,
                                                              Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDefaultChargingTariff    (Request)

        public async Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDefaultChargingTariffRequest));
            }

            #endregion


            GetDefaultChargingTariffResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomGetDefaultChargingTariffRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (GetDefaultChargingTariffResponse.TryParse(Request,
                                                              sendRequestState.Response,
                                                              out var setDisplayMessageResponse,
                                                              out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new GetDefaultChargingTariffResponse(Request,
                                                                  Result.Format(errorResponse));

            }

            response ??= new GetDefaultChargingTariffResponse(Request,
                                                              Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoveDefaultChargingTariff (Request)

        public async Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRemoveDefaultChargingTariffRequest));
            }

            #endregion


            RemoveDefaultChargingTariffResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomRemoveDefaultChargingTariffRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (RemoveDefaultChargingTariffResponse.TryParse(Request,
                                                                 sendRequestState.Response,
                                                                 out var setDisplayMessageResponse,
                                                                 out var errorResponse) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new RemoveDefaultChargingTariffResponse(Request,
                                                                     Result.Format(errorResponse));

            }

            response ??= new RemoveDefaultChargingTariffResponse(Request,
                                                                 Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRemoveDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #endregion


    }

}
