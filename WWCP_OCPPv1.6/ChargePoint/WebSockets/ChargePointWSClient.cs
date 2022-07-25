/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using cloud.charging.open.protocols.OCPPv1_6.CS;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using org.GraphDefined.Vanaheimr.Hermod.Websocket;
//using org.GraphDefined.Vanaheimr.Hermod.Websocket.v1_2;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Authentication;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The delegate for the WebSocket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketClient">The sending WebSocket client.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WSClientRequestLogHandler(DateTime             Timestamp,
                                                   ChargePointWSClient  WebSocketClient,
                                                   JArray               Request);

    /// <summary>
    /// The delegate for the WebSocket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketClient">The sending WebSocket client.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    public delegate Task WSClientResponseLogHandler(DateTime             Timestamp,
                                                    ChargePointWSClient  WebSocketClient,
                                                    JArray               Request,
                                                    JArray               Response);


    /// <summary>
    /// The charge point WebSockets client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : IHTTPClient,
                                               ICPWSClient,
                                               IChargePointServerEvents
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public const           String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.Number + " CP WebSocket Client";

        /// <summary>
        /// The default remote TCP port to connect to.
        /// </summary>
        public static readonly IPPort  DefaultRemotePort     = IPPort.Parse(443);


        private Socket           TCPSocket;
        private MyNetworkStream  TCPStream;
        private SslStream        TLSStream;
        private Stream           HTTPStream;

        private Int64            requestId = 100000;

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public readonly TimeSpan DefaultMaintenanceEvery = TimeSpan.FromSeconds(1);
        private static readonly SemaphoreSlim MaintenanceSemaphore = new SemaphoreSlim(1, 1);
        private readonly Timer MaintenanceTimer;

        public readonly TimeSpan DefaultWebSocketPingEvery = TimeSpan.FromSeconds(30);
        //private static readonly SemaphoreSlim WebSocketPingSemaphore = new SemaphoreSlim(1, 1);
        private readonly Timer WebSocketPingTimer;

        protected static readonly TimeSpan SemaphoreSlimTimeout = TimeSpan.FromSeconds(5);

        private const String LogfileName = "ChargePointWSClient.log";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id                         ChargeBoxIdentity               { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From                            { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                              { get; }


        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        public ChargePointWSClient.CPClientLogger    Logger                          { get; }



        /// <summary>
        /// The remote URL of the HTTP endpoint to connect to.
        /// </summary>
        public URL                                   RemoteURL                       { get; }

        /// <summary>
        /// The virtual HTTP hostname to connect to.
        /// </summary>
        public HTTPHostname?                         VirtualHostname                 { get; }

        /// <summary>
        /// An optional description of this HTTP client.
        /// </summary>
        public String?                               Description                     { get; set; }

        /// <summary>
        /// The remote SSL/TLS certificate validator.
        /// </summary>
        public RemoteCertificateValidationCallback?  RemoteCertificateValidator      { get; private set; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionCallback?    ClientCertificateSelector       { get; }

        /// <summary>
        /// The SSL/TLS client certificate to use of HTTP authentication.
        /// </summary>
        public X509Certificate?                      ClientCert                      { get; }

        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        public SslProtocols                          TLSProtocol                     { get; }

        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        public Boolean                               PreferIPv4                      { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String                                HTTPUserAgent                   { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan                              RequestTimeout                  { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate        TransmissionRetryDelay          { get; }

        /// <summary>
        /// The maximum number of retries when communicationg with the remote OICP service.
        /// </summary>
        public UInt16                                MaxNumberOfRetries              { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean                               UseHTTPPipelining               { get; }

        /// <summary>
        /// The CPO client (HTTP client) logger.
        /// </summary>
        public HTTPClientLogger                      HTTPLogger                      { get; set; }




        /// <summary>
        /// The DNS client defines which DNS servers to use.
        /// </summary>
        public DNSClient                             DNSClient                       { get; }



        /// <summary>
        /// Our local IP port.
        /// </summary>
        public IPPort                                LocalPort                       { get; private set; }

        /// <summary>
        /// The IP Address to connect to.
        /// </summary>
        public IIPAddress                            RemoteIPAddress                 { get; protected set; }


        public Int32 Available
                    => TCPSocket.Available;

        public Boolean Connected
            => TCPSocket.Connected;

        public LingerOption? LingerState
        {
            get
            {
                return TCPSocket.LingerState;
            }
            set
            {
                TCPSocket.LingerState = value;
            }
        }

        public Boolean NoDelay
        {
            get
            {
                return TCPSocket.NoDelay;
            }
            set
            {
                TCPSocket.NoDelay = value;
            }
        }

        public Byte TTL
        {
            get
            {
                return (Byte) TCPSocket.Ttl;
            }
            set
            {
                TCPSocket.Ttl = value;
            }
        }


        public Tuple<String, String>?               HTTPBasicAuth                   { get; }


        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean                              DisableMaintenanceTasks         { get; set; }

        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan                             MaintenanceEvery                { get; }


        /// <summary>
        /// Disable web socket pings.
        /// </summary>
        public Boolean                              DisableWebSocketPingTasks       { get; set; }

        /// <summary>
        /// The web socket ping interval.
        /// </summary>
        public TimeSpan                             WebSocketPingEvery              { get; }


        #region CustomRequestParsers

        public CustomJObjectParserDelegate<ResetRequest>                   CustomResetRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>      CustomChangeAvailabilityRequestParser        { get; set; }
        public CustomJObjectParserDelegate<GetConfigurationRequest>        CustomGetConfigurationRequestParser          { get; set; }
        public CustomJObjectParserDelegate<ChangeConfigurationRequest>     CustomChangeConfigurationRequestParser       { get; set; }
        public CustomJObjectParserDelegate<CS.DataTransferRequest>         CustomIncomingDataTransferRequestParser      { get; set; }
        public CustomJObjectParserDelegate<GetDiagnosticsRequest>          CustomGetDiagnosticsRequestParser            { get; set; }
        public CustomJObjectParserDelegate<TriggerMessageRequest>          CustomTriggerMessageRequestParser            { get; set; }
        public CustomJObjectParserDelegate<UpdateFirmwareRequest>          CustomUpdateFirmwareRequestParser            { get; set; }

        public CustomJObjectParserDelegate<ReserveNowRequest>              CustomReserveNowRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CancelReservationRequest>       CustomCancelReservationRequestParser         { get; set; }
        public CustomJObjectParserDelegate<RemoteStartTransactionRequest>  CustomRemoteStartTransactionRequestParser    { get; set; }
        public CustomJObjectParserDelegate<RemoteStopTransactionRequest>   CustomRemoteStopTransactionRequestParser     { get; set; }
        public CustomJObjectParserDelegate<SetChargingProfileRequest>      CustomSetChargingProfileRequestParser        { get; set; }
        public CustomJObjectParserDelegate<ClearChargingProfileRequest>    CustomClearChargingProfileRequestParser      { get; set; }
        public CustomJObjectParserDelegate<GetCompositeScheduleRequest>    CustomGetCompositeScheduleRequestParser      { get; set; }
        public CustomJObjectParserDelegate<UnlockConnectorRequest>         CustomUnlockConnectorRequestParser           { get; set; }

        public CustomJObjectParserDelegate<GetLocalListVersionRequest>     CustomGetLocalListVersionRequestParser       { get; set; }
        public CustomJObjectParserDelegate<SendLocalListRequest>           CustomSendLocalListRequestParser             { get; set; }
        public CustomJObjectParserDelegate<ClearCacheRequest>              CustomClearCacheRequestParser                { get; set; }

        #endregion

        #endregion

        #region Events

        #region HTTPRequest-/ResponseLog

        /// <summary>
        /// A delegate for logging the HTTP request.
        /// </summary>
        public event ClientRequestLogHandler?   RequestLogDelegate;

        /// <summary>
        /// A delegate for logging the HTTP request/response.
        /// </summary>
        public event ClientResponseLogHandler?  ResponseLogDelegate;

        #endregion


        // Outgoing messages (to central system)

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification Request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?             OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?            OnBootNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification Request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat Request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?      OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?     OnHeartbeatWSResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat Request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize Request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?      OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?     OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize Request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction Request will be send to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate?   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?             OnStartTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?            OnStartTransactionWSResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction Request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate?  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification Request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?               OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification Request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values Request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?        OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnMeterValuesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values Request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction Request will be send to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate?   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?            OnStopTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?           OnStopTransactionWSResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction Request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate?  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer Request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?         OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?        OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer Request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification Request will be send to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                          OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?                         OnDiagnosticsStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification Request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification Request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification websocket Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                       OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification websocket Request was received.
        /// </summary>
        public event ClientResponseLogHandler?                      OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification Request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion


        // Incoming messages (from central system)

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event ResetRequestDelegate?          OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate?               OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event ResetResponseDelegate?         OnResetResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnResetWSResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?             OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event ChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate?          OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event ChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?            OnChangeAvailabilityWSResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnGetConfigurationWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event GetConfigurationRequestDelegate?     OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationDelegate?          OnGetConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event GetConfigurationResponseDelegate?    OnGetConfigurationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnGetConfigurationWSResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnChangeConfigurationWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event ChangeConfigurationRequestDelegate?     OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationDelegate?          OnChangeConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event ChangeConfigurationResponseDelegate?    OnChangeConfigurationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnChangeConfigurationWSResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event IncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?          OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event IncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnIncomingDataTransferWSResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnGetDiagnosticsWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event GetDiagnosticsRequestDelegate?     OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsDelegate?          OnGetDiagnostics;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event GetDiagnosticsResponseDelegate?    OnGetDiagnosticsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnGetDiagnosticsWSResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event TriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageDelegate?          OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event TriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnTriggerMessageWSResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event UpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?          OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event UpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnUpdateFirmwareWSResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnReserveNowWSRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event ReserveNowRequestDelegate?     OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate?          OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event ReserveNowResponseDelegate?    OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reserve now request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnReserveNowWSResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnCancelReservationWSRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event CancelReservationRequestDelegate?     OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate?          OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event CancelReservationResponseDelegate?    OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cancel reservation request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnCancelReservationWSResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a remote start transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnRemoteStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction request was received.
        /// </summary>
        public event RemoteStartTransactionRequestDelegate?     OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction was received.
        /// </summary>
        public event OnRemoteStartTransactionDelegate?          OnRemoteStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event RemoteStartTransactionResponseDelegate?    OnRemoteStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a remote start transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnRemoteStartTransactionWSResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a remote stop transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnRemoteStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction request was received.
        /// </summary>
        public event RemoteStopTransactionRequestDelegate?     OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction was received.
        /// </summary>
        public event OnRemoteStopTransactionDelegate?          OnRemoteStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event RemoteStopTransactionResponseDelegate?    OnRemoteStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a remote stop transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnRemoteStopTransactionWSResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?             OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event SetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate?          OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event SetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?            OnSetChargingProfileWSResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnClearChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event ClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate?          OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event ClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnClearChargingProfileWSResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event GetCompositeScheduleRequestDelegate?     OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?          OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event GetCompositeScheduleResponseDelegate?    OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnGetCompositeScheduleWSResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnUnlockConnectorWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event UnlockConnectorRequestDelegate?     OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate?          OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event UnlockConnectorResponseDelegate?    OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnUnlockConnectorWSResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event GetLocalListVersionRequestDelegate?     OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate?          OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event GetLocalListVersionResponseDelegate?    OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnGetLocalListVersionWSResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?        OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event SendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate?          OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event SendLocalListResponseDelegate?    OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?       OnSendLocalListWSResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event ClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate?          OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event ClearCacheResponseDelegate?    OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnClearCacheWSResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region ChargePointWSClient(Request.ChargeBoxId, Hostname, ..., LoggingContext = CPClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new charge point websocket client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPBasicAuth">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargePointWSClient(ChargeBox_Id                          ChargeBoxIdentity,
                                   String                                From,
                                   String                                To,

                                   URL                                   RemoteURL,
                                   HTTPHostname?                         VirtualHostname              = null,
                                   String?                               Description                  = null,
                                   RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                   X509Certificate?                      ClientCert                   = null,
                                   String                                HTTPUserAgent                = DefaultHTTPUserAgent,
                                   HTTPPath?                             URLPathPrefix                = null,
                                   SslProtocols?                         TLSProtocol                  = null,
                                   Boolean?                              PreferIPv4                   = null,
                                   Tuple<String, String>?                HTTPBasicAuth                = null,
                                   TimeSpan?                             RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                   UInt16?                               MaxNumberOfRetries           = 3,
                                   Boolean                               UseHTTPPipelining            = false,

                                   TimeSpan?                             MaintenanceEvery             = null,
                                   TimeSpan?                             WebSocketPingEvery           = null,

                                   String?                               LoggingPath                  = null,
                                   String                                LoggingContext               = ChargePointWSClient.CPClientLogger.DefaultContext,
                                   LogfileCreatorDelegate?               LogFileCreator               = null,
                                   HTTPClientLogger?                     HTTPLogger                   = null,
                                   DNSClient?                            DNSClient                    = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

            #endregion

            this.RemoteURL                   = RemoteURL;
            this.VirtualHostname             = VirtualHostname;
            this.Description                 = Description;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;
            this.ClientCertificateSelector   = ClientCertificateSelector;
            this.ClientCert                  = ClientCert;
            this.HTTPUserAgent               = HTTPUserAgent;
            //this.URLPathPrefix               = URLPathPrefix;
            this.TLSProtocol                 = TLSProtocol        ?? SslProtocols.Tls12 | SslProtocols.Tls13;
            this.PreferIPv4                  = PreferIPv4         ?? false;
            this.HTTPBasicAuth               = HTTPBasicAuth;
            this.RequestTimeout              = RequestTimeout     ?? TimeSpan.FromMinutes(10);
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries ?? 3;
            this.UseHTTPPipelining           = UseHTTPPipelining;
            this.HTTPLogger                  = HTTPLogger;
            this.DNSClient                   = DNSClient;

            this.ChargeBoxIdentity           = ChargeBoxIdentity;
            this.From                        = From;
            this.To                          = To;

            //this.Logger                      = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                            LoggingPath,
            //                                                                            LoggingContext,
            //                                                                            LogFileCreator);

            this.MaintenanceEvery            = MaintenanceEvery   ?? DefaultMaintenanceEvery;
            this.MaintenanceTimer            = new Timer(DoMaintenanceSync,
                                                         null,
                                                         this.MaintenanceEvery,
                                                         this.MaintenanceEvery);

            this.WebSocketPingEvery          = WebSocketPingEvery ?? DefaultWebSocketPingEvery;
            this.WebSocketPingTimer          = new Timer(DoWebSocketPingSync,
                                                         null,
                                                         this.WebSocketPingEvery,
                                                         this.WebSocketPingEvery);

        }

        #endregion

        #region ChargePointWSClient(Request.ChargeBoxId, Logger, Hostname, ...)

        /// <summary>
        /// Create a new charge point websocket client.
        /// </summary>
        /// <param name="ChargeBoxIdentity">A unqiue identification of this client.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="Hostname">The OCPP hostname to connect to.</param>
        /// <param name="RemotePort">An optional OCPP TCP port to connect to.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URLPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        //public ChargePointWSClient(String                               ChargeBoxIdentity,
        //                             String                               From,
        //                             String                               To,

        //                             CPClientLogger                       Logger,
        //                             HTTPHostname                         Hostname,
        //                             IPPort?                              RemotePort                   = null,
        //                             RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
        //                             LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
        //                             HTTPHostname?                        HTTPVirtualHost              = null,
        //                             HTTPPath?                            URLPrefix                    = null,
        //                             String                               HTTPUserAgent                = DefaultHTTPUserAgent,
        //                             TimeSpan?                            RequestTimeout               = null,
        //                             Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
        //                             DNSClient                            DNSClient                    = null)

        //    : base(Request.ChargeBoxId,
        //           Hostname,
        //           RemotePort ?? DefaultRemotePort,
        //           RemoteCertificateValidator,
        //           ClientCertificateSelector,
        //           HTTPVirtualHost,
        //           URLPrefix ?? DefaultURLPrefix,
        //           null,
        //           HTTPUserAgent,
        //           RequestTimeout,
        //           null,
        //           MaxNumberOfRetries,
        //           DNSClient)

        //{

        //    #region Initial checks

        //    if (ChargeBoxIdentity.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

        //    if (From.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

        //    if (To.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

        //    #endregion

        //    this.From    = From;
        //    this.To      = To;

        //    this.Logger  = Logger ?? throw new ArgumentNullException(nameof(Hostname), "The given hostname must not be null or empty!");

        //}

        #endregion

        #endregion



        #region Connect(Request, RequestLogDelegate = null, ResponseLogDelegate = null, Timeout = null, CancellationToken = null)

        /// <summary>
        /// Execute the given HTTP request and return its result.
        /// </summary>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <param name="EventTrackingId"></param>
        /// <param name="RequestTimeout">An optional timeout.</param>
        /// <param name="NumberOfRetry">The number of retransmissions of this request.</param>
        public async Task<HTTPResponse> Connect(CancellationToken?        CancellationToken     = null,
                                                EventTracking_Id          EventTrackingId       = null,
                                                TimeSpan?                 RequestTimeout        = null,
                                                Byte                      NumberOfRetry         = 0)
        {

            HTTPRequest  request   = null;
            HTTPResponse response  = null;

            if (!RequestTimeout.HasValue)
                RequestTimeout = TimeSpan.FromMinutes(10);

            try
            {

                //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                #region Data

                var HTTPHeaderBytes   = new Byte[0];
                var HTTPBodyBytes     = new Byte[0];
                var sw                = new Stopwatch();

                #endregion

                #region Create TCP connection (possibly also do DNS lookups)

                Boolean restart;

                do
                {

                    restart = false;

                    if (TCPSocket == null)
                    {

                        System.Net.IPEndPoint _FinalIPEndPoint = null;

                        if (RemoteIPAddress == null)
                        {

                            if      (IPAddress.IsIPv4Localhost(RemoteURL.Hostname))
                                RemoteIPAddress = IPv4Address.Localhost;

                            else if (IPAddress.IsIPv6Localhost(RemoteURL.Hostname))
                                RemoteIPAddress = IPv6Address.Localhost;

                            else if (IPAddress.IsIPv4(RemoteURL.Hostname.Name))
                                RemoteIPAddress = IPv4Address.Parse(RemoteURL.Hostname.Name);

                            else if (IPAddress.IsIPv6(RemoteURL.Hostname.Name))
                                RemoteIPAddress = IPv6Address.Parse(RemoteURL.Hostname.Name);

                            #region DNS lookup...

                            if (RemoteIPAddress == null)
                            {

                                var IPv4AddressLookupTask  = DNSClient.
                                                                 Query<A>(RemoteURL.Hostname.Name).
                                                                 ContinueWith(query => query.Result.Select(ARecord    => ARecord.IPv4Address));

                                var IPv6AddressLookupTask  = DNSClient.
                                                                 Query<AAAA>(RemoteURL.Hostname.Name).
                                                                 ContinueWith(query => query.Result.Select(AAAARecord => AAAARecord.IPv6Address));

                                await Task.WhenAll(IPv4AddressLookupTask,
                                                   IPv6AddressLookupTask).
                                           ConfigureAwait(false);


                                if (IPv4AddressLookupTask.Result.Any())
                                    RemoteIPAddress = IPv4AddressLookupTask.Result.First();

                                else if (IPv6AddressLookupTask.Result.Any())
                                    RemoteIPAddress = IPv6AddressLookupTask.Result.First();


                                if (RemoteIPAddress == null || RemoteIPAddress.GetBytes() == null)
                                    throw new Exception("DNS lookup failed!");

                            }

                            #endregion

                        }

                        _FinalIPEndPoint = new System.Net.IPEndPoint(new System.Net.IPAddress(RemoteIPAddress.GetBytes()),
                                                                     RemoteURL.Port.Value.ToInt32());

                        sw.Start();

                        //TCPClient = new TcpClient();
                        //TCPClient.Connect(_FinalIPEndPoint);
                        //TCPClient.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;


                        if (RemoteIPAddress.IsIPv4)
                            TCPSocket = new Socket(AddressFamily.InterNetwork,
                                                   SocketType.Stream,
                                                   ProtocolType.Tcp);

                        else if (RemoteIPAddress.IsIPv6)
                            TCPSocket = new Socket(AddressFamily.InterNetworkV6,
                                                   SocketType.Stream,
                                                   ProtocolType.Tcp);

                        TCPSocket.SendTimeout    = (Int32) RequestTimeout.Value.TotalMilliseconds;
                        TCPSocket.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;
                        TCPSocket.Connect(_FinalIPEndPoint);
                        TCPSocket.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;

                    }

                    TCPStream = new MyNetworkStream(TCPSocket, true) {
                                    ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds
                                };

                #endregion

                #region Create (Crypto-)Stream

                    if (RemoteCertificateValidator == null &&
                       (RemoteURL.Protocol == URLProtocols.wss || RemoteURL.Protocol == URLProtocols.https))
                    {
                        RemoteCertificateValidator = (sender, certificate, chain, sslPolicyErrors) => {
                            return true;
                        };
                    }

                    if (RemoteCertificateValidator != null)
                    {

                        if (TLSStream == null)
                        {

                            TLSStream = new SslStream(TCPStream,
                                                      false,
                                                      RemoteCertificateValidator,
                                                      ClientCertificateSelector,
                                                      EncryptionPolicy.RequireEncryption)
                            {

                                ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds

                            };

                            HTTPStream = TLSStream;

                            try
                            {

                                await TLSStream.AuthenticateAsClientAsync(RemoteURL.Hostname.Name,
                                                                          null,
                                                                          SslProtocols.Tls12,
                                                                          false);//, new X509CertificateCollection(new X509Certificate[] { ClientCert }), SslProtocols.Default, true);

                            }
                            catch (Exception e)
                            {
                                TCPSocket = null;
                                restart = true;
                            }

                        }

                    }

                    else
                    {
                        TLSStream   = null;
                        HTTPStream  = TCPStream;
                    }

                    HTTPStream.ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;

                }
                while (restart);

                this.LocalPort = IPSocket.FromIPEndPoint(TCPStream.Socket.LocalEndPoint).Port;

                #endregion

                #region Send Request

                // GET /webServices/ocpp/CP3211 HTTP/1.1
                // Host:                    some.server.com:33033
                // Connection:              Upgrade
                // Upgrade:                 websocket
                // Sec-WebSocket-Key:       x3JJHMbDL1EzLkh9GBhXDw==
                // Sec-WebSocket-Protocol:  ocpp1.6, ocpp1.5
                // Sec-WebSocket-Version:   13

                request = new HTTPRequest.Builder {
                    Path                  = RemoteURL.Path,
                    Host                  = HTTPHostname.Parse(String.Concat(RemoteURL.Hostname, ":", RemoteURL.Port)),
                    Connection            = "Upgrade",
                    Upgrade               = "websocket",
                    SecWebSocketKey       = "x3JJHMbDL1EzLkh9GBhXDw==",
                    SecWebSocketProtocol  = "ocpp1.6",
                    SecWebSocketVersion   = "13",
                    Authorization         = HTTPBasicAuth?.Item1.IsNotNullOrEmpty() == true && HTTPBasicAuth?.Item2.IsNotNullOrEmpty() == true
                                                ? new HTTPBasicAuthentication(HTTPBasicAuth.Item1, HTTPBasicAuth.Item2)
                                                : null
                }.AsImmutable;

                #region Call the optional HTTP request log delegate

                try
                {

                    if (RequestLogDelegate != null)
                        await Task.WhenAll(RequestLogDelegate.GetInvocationList().
                                           Cast<ClientRequestLogHandler>().
                                           Select(e => e(Timestamp.Now,
                                                         this,
                                                         request))).
                                           ConfigureAwait(false);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(HTTPClient) + "." + nameof(RequestLogDelegate));
                }

                #endregion

                HTTPStream.Write((request.EntirePDU + "\r\n\r\n").ToUTF8Bytes());

                File.AppendAllText(LogfileName,
                                   String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                                Environment.NewLine,
                                                 "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                             Environment.NewLine,
                                                 "HTTP request: ", Environment.NewLine, Environment.NewLine,
                                                 request.EntirePDU, Environment.NewLine,
                                                 "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                #endregion

                #region Wait for HTTP response

                var buffer  = new Byte[16 * 1024];
                var pos     = 0;

                do
                {

                    pos += HTTPStream.Read(buffer, pos, 2048);

                    if (sw.ElapsedMilliseconds >= RequestTimeout.Value.TotalMilliseconds)
                        throw new HTTPTimeoutException(sw.Elapsed);

                    Thread.Sleep(1);

                } while (TCPStream.DataAvailable && pos < buffer.Length - 2048);

                var responseData  = buffer.ToUTF8String(pos);
                var lines         = responseData.Split('\n').Select(line => line?.Trim()).TakeWhile(line => line.IsNotNullOrEmpty()).ToArray();
                response          = HTTPResponse.Parse(lines.AggregateWith(Environment.NewLine),
                                                       new Byte[0],
                                                       request);

                // HTTP/1.1 101 Switching Protocols
                // Upgrade:                 websocket
                // Connection:              Upgrade
                // Sec-WebSocket-Accept:    s3pPLMBiTxaQ9kYGzzhZRbK+xOo=
                // Sec-WebSocket-Protocol:  ocpp1.6

                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                //var swk             = WSConnection.GetHTTPHeader("Sec-WebSocket-Key");
                //var swka            = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                //var swkaSha1        = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                //var swkaSha1Base64  = Convert.ToBase64String(swkaSha1);

                File.AppendAllText(LogfileName,
                                   String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                                Environment.NewLine,
                                                 "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                             Environment.NewLine,
                                                 "HTTP response: ", Environment.NewLine, Environment.NewLine,
                                                 response.EntirePDU, Environment.NewLine,
                                                 "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                if (response.HTTPStatusCode.Code != 101)
                    DebugX.Log("response.HTTPStatusCode.Code != 101");

                #endregion

                #region Close connection if requested!

                if (response.Connection          ==  null   ||
                    response.Connection          == "close" ||
                    response.HTTPStatusCode.Code != 101)
                {

                    if (TLSStream != null)
                    {
                        TLSStream.Close();
                        TLSStream = null;
                    }

                    if (TCPSocket != null)
                    {
                        TCPSocket.Close();
                        //TCPClient.Dispose();
                        TCPSocket = null;
                    }

                    HTTPStream = null;

                }

                #endregion

            }

            #region Catch...

            catch (HTTPTimeoutException e)
            {

                #region Create a HTTP response for the exception...

                response = new HTTPResponse.Builder(request,
                                                    HTTPStatusCode.RequestTimeout)
                {

                    ContentType  = HTTPContentType.JSON_UTF8,
                    Content      = JSONObject.Create(new JProperty("timeout",     (Int32) e.Timeout.TotalMilliseconds),
                                                     new JProperty("message",     e.Message),
                                                     new JProperty("stackTrace",  e.StackTrace)).
                                              ToUTF8Bytes()

                };

                #endregion

                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream = null;
                }

                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                    TCPSocket = null;
                }

            }
            catch (Exception e)
            {

                #region Create a HTTP response for the exception...

                while (e.InnerException != null)
                    e = e.InnerException;

                response = new HTTPResponse.Builder(request,
                                                    HTTPStatusCode.BadRequest)
                {

                    ContentType  = HTTPContentType.JSON_UTF8,
                    Content      = JSONObject.Create(new JProperty("message",     e.Message),
                                                     new JProperty("stackTrace",  e.StackTrace)).
                                              ToUTF8Bytes()

                };

                #endregion

                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream = null;
                }

                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                    TCPSocket = null;
                }

            }

            #endregion

            #region Call the optional HTTP response log delegate

            try
            {

                if (ResponseLogDelegate != null)
                    await Task.WhenAll(ResponseLogDelegate.GetInvocationList().
                                       Cast<ClientResponseLogHandler>().
                                       Select(e => e(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response))).
                                       ConfigureAwait(false);

            }
            catch (Exception e2)
            {
                DebugX.Log(e2, nameof(HTTPClient) + "." + nameof(ResponseLogDelegate));
            }

            #endregion

            return response;

        }

        #endregion


        #region (Timer) DoWebSocketPing(State)

        private void DoWebSocketPingSync(Object State)
        {
            if (!DisableWebSocketPingTasks)
                DoWebSocketPing(State).Wait();
        }

        private async Task DoWebSocketPing(Object State)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {
                    if (HTTPStream != null)
                    {

                        HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                            WebSocketFrame.MaskStatus.On,
                                                            new Byte[] { 0xaa, 0xbb, 0xcc, 0xdd },
                                                            WebSocketFrame.Opcodes.Ping,
                                                            Guid.NewGuid().ToByteArray(),
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off).ToByteArray());

                        HTTPStream.Flush();

                        File.AppendAllText(LogfileName,
                                           String.Concat("Timestamp: ",   Timestamp.Now.ToIso8601(),    Environment.NewLine,
                                                         "ChargeBoxId: ", ChargeBoxIdentity.ToString(), Environment.NewLine,
                                                         "Ping sent: ",   Environment.NewLine,
                                                         "--------------------------------------------------------------------------------------------", Environment.NewLine));

                        //var buffer   = new Byte[64 * 1024];
                        //var pos      = 0;
                        //var sw       = Stopwatch.StartNew();
                        //var timeout  = (Int64) TimeSpan.FromSeconds(5).TotalMilliseconds;

                        //do
                        //{

                        //    pos += HTTPStream.Read(buffer, pos, 2048);

                        //    if (sw.ElapsedMilliseconds >= timeout)
                        //        break;

                        //    Thread.Sleep(5);

                        //} while (TCPStream.DataAvailable);

                        //Array.Resize(ref buffer, pos);

                        //if (WebSocketFrame.TryParse(buffer, out WebSocketFrame frame, out UInt64 frameLength))
                        //{


                        //        File.AppendAllText(LogfileName,
                        //                           String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                                Environment.NewLine,
                        //                                         "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                             Environment.NewLine,
                        //                                         "Message received: ",  wsResponseMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                        //                                         "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                        //        return wsResponseMessage;

                        //    }
                        //    else
                        //        DebugX.Log(nameof(ChargePointWSClient), " error: " + frame.Payload.ToUTF8String());
                        //}

                    }
                }
                catch (ObjectDisposedException ode)
                {
                    WebSocketPingTimer.Dispose();
                }
                catch (Exception e)
                {

                    while (e.InnerException != null)
                        e = e.InnerException;

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }
            else
                DebugX.LogT("Could not aquire the web socket ping task lock!");

        }

        #endregion


        #region (Timer) DoMaintenance(State)

        private void DoMaintenanceSync(Object State)
        {
            if (!DisableMaintenanceTasks)
                DoMaintenance(State).Wait();
        }

        protected internal virtual async Task _DoMaintenance(Object State)
        {

            if (TCPStream?.DataAvailable == true)
            {

                var buffer = new Byte[64 * 1024];
                var pos    = 0;

                do
                {

                    pos += HTTPStream.Read(buffer, pos, 2048);

                    //if (sw.ElapsedMilliseconds >= RequestTimeout.Value.TotalMilliseconds)
                    //    throw new HTTPTimeoutException(sw.Elapsed);

                    Thread.Sleep(1);

                } while (TCPStream.DataAvailable);

                Array.Resize(ref buffer, pos);

                do
                {

                    if (WebSocketFrame.TryParse(buffer,
                                                out WebSocketFrame?  frame,
                                                out UInt64           frameLength,
                                                out String?          errorResponse))
                    {

                        if (frame is not null)
                        {

                            switch (frame.Opcode)
                            {

                                case WebSocketFrame.Opcodes.Text: {

                                    var textPayload = frame.Payload.ToUTF8String();

                                    if (textPayload == "[]")
                                        DebugX.Log(nameof(ChargePointWSClient), " [] received!");

                                    else if (OCPP_WebSocket_RequestMessage. TryParse(textPayload, out OCPP_WebSocket_RequestMessage  wsRequestMessage))
                                    {

                                        File.AppendAllText(LogfileName,
                                                           String.Concat("timestamp: ",         Timestamp.Now.ToIso8601(),                                               Environment.NewLine,
                                                                         "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                            Environment.NewLine,
                                                                         "Message received: ",  wsRequestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                                                                         "--------------------------------------------------------------------------------------------", Environment.NewLine));


                                        var requestJSON              = JArray.Parse(textPayload);
                                        var cancellationTokenSource  = new CancellationTokenSource();

                                        JObject?                     OCPPResponseJSON   = null;
                                        OCPP_WebSocket_ErrorMessage? ErrorMessage       = null;

                                        switch (wsRequestMessage.Action)
                                        {

                                            case "Reset":
                                                {

                                                    #region Send OnResetWSRequest event

                                                    try
                                                    {

                                                        OnResetWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnResetWSRequest));
                                                    }

                                                    #endregion

                                                    ResetResponse response = null;

                                                    try
                                                    {

                                                        if (ResetRequest.TryParse(wsRequestMessage.Message,
                                                                                  wsRequestMessage.RequestId,
                                                                                  ChargeBoxIdentity,
                                                                                  out ResetRequest request,
                                                                                  out String ErrorResponse,
                                                                                  CustomResetRequestParser))
                                                        {

                                                            #region Send OnResetRequest event

                                                            try
                                                            {

                                                                OnResetRequest?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnResetRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnReset?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnResetDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ResetResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnResetResponse event

                                                            try
                                                            {

                                                                OnResetResponse?.Invoke(Timestamp.Now,
                                                                                        this,
                                                                                        request,
                                                                                        response,
                                                                                        response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnResetResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'Reset' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'Reset' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnResetWSResponse event

                                                    try
                                                    {

                                                        OnResetWSResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  requestJSON,
                                                                                  new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                        OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnResetWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "ChangeAvailability":
                                                {

                                                    #region Send OnChangeAvailabilityWSRequest event

                                                    try
                                                    {

                                                        OnChangeAvailabilityWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeAvailabilityWSRequest));
                                                    }

                                                    #endregion

                                                    ChangeAvailabilityResponse response = null;

                                                    try
                                                    {

                                                        if (ChangeAvailabilityRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out ChangeAvailabilityRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomChangeAvailabilityRequestParser))
                                                        {

                                                            #region Send OnChangeAvailabilityRequest event

                                                            try
                                                            {

                                                                OnChangeAvailabilityRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeAvailabilityRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnChangeAvailability?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ChangeAvailabilityResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnChangeAvailabilityResponse event

                                                            try
                                                            {

                                                                OnChangeAvailabilityResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeAvailabilityResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'ChangeAvailability' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'ChangeAvailability' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnChangeAvailabilityWSResponse event

                                                    try
                                                    {

                                                        OnChangeAvailabilityWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "GetConfiguration":
                                                {

                                                    #region Send OnGetConfigurationWSRequest event

                                                    try
                                                    {

                                                        OnGetConfigurationWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetConfigurationWSRequest));
                                                    }

                                                    #endregion

                                                    GetConfigurationResponse response = null;

                                                    try
                                                    {

                                                        if (GetConfigurationRequest.TryParse(wsRequestMessage.Message,
                                                                                             wsRequestMessage.RequestId,
                                                                                             ChargeBoxIdentity,
                                                                                             out GetConfigurationRequest request,
                                                                                             out String ErrorResponse,
                                                                                             CustomGetConfigurationRequestParser))
                                                        {

                                                            #region Send OnGetConfigurationRequest event

                                                            try
                                                            {

                                                                OnGetConfigurationRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetConfigurationRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnGetConfiguration?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnGetConfigurationDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = GetConfigurationResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnGetConfigurationResponse event

                                                            try
                                                            {

                                                                OnGetConfigurationResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetConfigurationResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                              OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                              "The given 'GetConfiguration' request could not be parsed!",
                                                                                              new JObject(
                                                                                                  new JProperty("request", requestJSON)
                                                                                              ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                          OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                          "Processing the given 'GetConfiguration' request led to an exception!",
                                                                                          new JObject(
                                                                                              new JProperty("request", requestJSON),
                                                                                              new JProperty("exception", e.Message),
                                                                                              new JProperty("stacktrace", e.StackTrace)
                                                                                          ));

                                                    }


                                                    #region Send OnGetConfigurationWSResponse event

                                                    try
                                                    {

                                                        OnGetConfigurationWSResponse?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             requestJSON,
                                                                                             new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                   OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetConfigurationWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "ChangeConfiguration":
                                                {

                                                    #region Send OnChangeConfigurationWSRequest event

                                                    try
                                                    {

                                                        OnChangeConfigurationWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationWSRequest));
                                                    }

                                                    #endregion

                                                    ChangeConfigurationResponse response = null;

                                                    try
                                                    {

                                                        if (ChangeConfigurationRequest.TryParse(wsRequestMessage.Message,
                                                                                                wsRequestMessage.RequestId,
                                                                                                ChargeBoxIdentity,
                                                                                                out ChangeConfigurationRequest request,
                                                                                                out String ErrorResponse,
                                                                                                CustomChangeConfigurationRequestParser))
                                                        {

                                                            #region Send OnChangeConfigurationRequest event

                                                            try
                                                            {

                                                                OnChangeConfigurationRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnChangeConfiguration?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnChangeConfigurationDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ChangeConfigurationResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnChangeConfigurationResponse event

                                                            try
                                                            {

                                                                OnChangeConfigurationResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'ChangeConfiguration' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'ChangeConfiguration' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnChangeConfigurationWSResponse event

                                                    try
                                                    {

                                                        OnChangeConfigurationWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "DataTransfer":
                                                {

                                                    #region Send OnIncomingDataTransferWSRequest event

                                                    try
                                                    {

                                                        OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                                                this,
                                                                                                requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
                                                    }

                                                    #endregion

                                                    CP.DataTransferResponse response = null;

                                                    try
                                                    {

                                                        if (CS.DataTransferRequest.TryParse(wsRequestMessage.Message,
                                                                                            wsRequestMessage.RequestId,
                                                                                            ChargeBoxIdentity,
                                                                                            out CS.DataTransferRequest  request,
                                                                                            out String                  ErrorResponse,
                                                                                            CustomIncomingDataTransferRequestParser))
                                                        {

                                                            #region Send OnIncomingDataTransferRequest event

                                                            try
                                                            {

                                                                OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                                                      this,
                                                                                                      request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDataTransferRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnIncomingDataTransfer?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = DataTransferResponse.Failed(request);

                                                            }

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
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage =  new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                               OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                               "The given 'DataTransfer' request could not be parsed!",
                                                                                               new JObject(
                                                                                                   new JProperty("request", requestJSON)
                                                                                               ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                          OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                          "Processing the given 'DataTransfer' request led to an exception!",
                                                                                          new JObject(
                                                                                              new JProperty("request",     requestJSON),
                                                                                              new JProperty("exception",   e.Message),
                                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                                          ));

                                                    }


                                                    #region Send OnIncomingDataTransferWSResponse event

                                                    try
                                                    {

                                                        OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                                   this,
                                                                                                   requestJSON,
                                                                                                   new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                                      OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "GetDiagnostics":
                                                {

                                                    #region Send OnGetDiagnosticsWSRequest event

                                                    try
                                                    {

                                                        OnGetDiagnosticsWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsWSRequest));
                                                    }

                                                    #endregion

                                                    GetDiagnosticsResponse response = null;

                                                    try
                                                    {

                                                        if (GetDiagnosticsRequest.TryParse(wsRequestMessage.Message,
                                                                                           wsRequestMessage.RequestId,
                                                                                           ChargeBoxIdentity,
                                                                                           out GetDiagnosticsRequest request,
                                                                                           out String ErrorResponse,
                                                                                           CustomGetDiagnosticsRequestParser))
                                                        {

                                                            #region Send OnGetDiagnosticsRequest event

                                                            try
                                                            {

                                                                OnGetDiagnosticsRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnGetDiagnostics?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnGetDiagnosticsDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = GetDiagnosticsResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnGetDiagnosticsResponse event

                                                            try
                                                            {

                                                                OnGetDiagnosticsResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'GetDiagnostics' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'GetDiagnostics' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnGetDiagnosticsWSResponse event

                                                    try
                                                    {

                                                        OnGetDiagnosticsWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "TriggerMessage":
                                                {

                                                    #region Send OnTriggerMessageWSRequest event

                                                    try
                                                    {

                                                        OnTriggerMessageWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnTriggerMessageWSRequest));
                                                    }

                                                    #endregion

                                                    TriggerMessageResponse response = null;

                                                    try
                                                    {

                                                        if (TriggerMessageRequest.TryParse(wsRequestMessage.Message,
                                                                                           wsRequestMessage.RequestId,
                                                                                           ChargeBoxIdentity,
                                                                                           out TriggerMessageRequest request,
                                                                                           out String ErrorResponse,
                                                                                           CustomTriggerMessageRequestParser))
                                                        {

                                                            #region Send OnTriggerMessageRequest event

                                                            try
                                                            {

                                                                OnTriggerMessageRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnTriggerMessageRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnTriggerMessage?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnTriggerMessageDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = TriggerMessageResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnTriggerMessageResponse event

                                                            try
                                                            {

                                                                OnTriggerMessageResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnTriggerMessageResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'TriggerMessage' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'TriggerMessage' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnTriggerMessageWSResponse event

                                                    try
                                                    {

                                                        OnTriggerMessageWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnTriggerMessageWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "UpdateFirmware":
                                                {

                                                    #region Send OnUpdateFirmwareWSRequest event

                                                    try
                                                    {

                                                        OnUpdateFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
                                                    }

                                                    #endregion

                                                    UpdateFirmwareResponse response = null;

                                                    try
                                                    {

                                                        if (UpdateFirmwareRequest.TryParse(wsRequestMessage.Message,
                                                                                           wsRequestMessage.RequestId,
                                                                                           ChargeBoxIdentity,
                                                                                           out UpdateFirmwareRequest request,
                                                                                           out String ErrorResponse,
                                                                                           CustomUpdateFirmwareRequestParser))
                                                        {

                                                            #region Send OnUpdateFirmwareRequest event

                                                            try
                                                            {

                                                                OnUpdateFirmwareRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnUpdateFirmware?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnUpdateFirmwareDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = UpdateFirmwareResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnUpdateFirmwareResponse event

                                                            try
                                                            {

                                                                OnUpdateFirmwareResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'UpdateFirmware' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'UpdateFirmware' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnUpdateFirmwareWSResponse event

                                                    try
                                                    {

                                                        OnUpdateFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;


                                            case "ReserveNow":
                                                {

                                                    #region Send OnReserveNowWSRequest event

                                                    try
                                                    {

                                                        OnReserveNowWSRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowWSRequest));
                                                    }

                                                    #endregion

                                                    ReserveNowResponse response = null;

                                                    try
                                                    {

                                                        if (ReserveNowRequest.TryParse(wsRequestMessage.Message,
                                                                                       wsRequestMessage.RequestId,
                                                                                       ChargeBoxIdentity,
                                                                                       out ReserveNowRequest request,
                                                                                       out String ErrorResponse,
                                                                                       CustomReserveNowRequestParser))
                                                        {

                                                            #region Send OnReserveNowRequest event

                                                            try
                                                            {

                                                                OnReserveNowRequest?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnReserveNow?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ReserveNowResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnReserveNowResponse event

                                                            try
                                                            {

                                                                OnReserveNowResponse?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             request,
                                                                                             response,
                                                                                             response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'ReserveNow' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'ReserveNow' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnReserveNowWSResponse event

                                                    try
                                                    {

                                                        OnReserveNowWSResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       requestJSON,
                                                                                       new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                             OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "CancelReservation":
                                                {

                                                    #region Send OnCancelReservationWSRequest event

                                                    try
                                                    {

                                                        OnCancelReservationWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationWSRequest));
                                                    }

                                                    #endregion

                                                    CancelReservationResponse response = null;

                                                    try
                                                    {

                                                        if (CancelReservationRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out CancelReservationRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomCancelReservationRequestParser))
                                                        {

                                                            #region Send OnCancelReservationRequest event

                                                            try
                                                            {

                                                                OnCancelReservationRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnCancelReservation?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = CancelReservationResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnCancelReservationResponse event

                                                            try
                                                            {

                                                                OnCancelReservationResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'CancelReservation' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'CancelReservation' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnCancelReservationWSResponse event

                                                    try
                                                    {

                                                        OnCancelReservationWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "RemoteStartTransaction":
                                                {

                                                    #region Send OnRemoteStartTransactionWSRequest event

                                                    try
                                                    {

                                                        OnRemoteStartTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStartTransactionWSRequest));
                                                    }

                                                    #endregion

                                                    RemoteStartTransactionResponse response = null;

                                                    try
                                                    {

                                                        if (RemoteStartTransactionRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out RemoteStartTransactionRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomRemoteStartTransactionRequestParser))
                                                        {

                                                            #region Send OnRemoteStartTransactionRequest event

                                                            try
                                                            {

                                                                OnRemoteStartTransactionRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStartTransactionRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnRemoteStartTransaction?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnRemoteStartTransactionDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = RemoteStartTransactionResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnRemoteStartTransactionResponse event

                                                            try
                                                            {

                                                                OnRemoteStartTransactionResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStartTransactionResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'RemoteStartTransaction' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'RemoteStartTransaction' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnRemoteStartTransactionWSResponse event

                                                    try
                                                    {

                                                        OnRemoteStartTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStartTransactionWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "RemoteStopTransaction":
                                                {

                                                    #region Send OnRemoteStopTransactionWSRequest event

                                                    try
                                                    {

                                                        OnRemoteStopTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionWSRequest));
                                                    }

                                                    #endregion

                                                    RemoteStopTransactionResponse response = null;

                                                    try
                                                    {

                                                        if (RemoteStopTransactionRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out RemoteStopTransactionRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomRemoteStopTransactionRequestParser))
                                                        {

                                                            #region Send OnRemoteStopTransactionRequest event

                                                            try
                                                            {

                                                                OnRemoteStopTransactionRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnRemoteStopTransaction?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnRemoteStopTransactionDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = RemoteStopTransactionResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnRemoteStopTransactionResponse event

                                                            try
                                                            {

                                                                OnRemoteStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'RemoteStopTransaction' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'RemoteStopTransaction' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnRemoteStopTransactionWSResponse event

                                                    try
                                                    {

                                                        OnRemoteStopTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "SetChargingProfile":
                                                {

                                                    #region Send OnSetChargingProfileWSRequest event

                                                    try
                                                    {

                                                        OnSetChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSetChargingProfileWSRequest));
                                                    }

                                                    #endregion

                                                    SetChargingProfileResponse response = null;

                                                    try
                                                    {

                                                        if (SetChargingProfileRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out SetChargingProfileRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomSetChargingProfileRequestParser))
                                                        {

                                                            #region Send OnSetChargingProfileRequest event

                                                            try
                                                            {

                                                                OnSetChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSetChargingProfileRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnSetChargingProfile?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnSetChargingProfileDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = SetChargingProfileResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnSetChargingProfileResponse event

                                                            try
                                                            {

                                                                OnSetChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSetChargingProfileResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'SetChargingProfile' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'SetChargingProfile' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnSetChargingProfileWSResponse event

                                                    try
                                                    {

                                                        OnSetChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSetChargingProfileWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "ClearChargingProfile":
                                                {

                                                    #region Send OnClearChargingProfileWSRequest event

                                                    try
                                                    {

                                                        OnClearChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearChargingProfileWSRequest));
                                                    }

                                                    #endregion

                                                    ClearChargingProfileResponse response = null;

                                                    try
                                                    {

                                                        if (ClearChargingProfileRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out ClearChargingProfileRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomClearChargingProfileRequestParser))
                                                        {

                                                            #region Send OnClearChargingProfileRequest event

                                                            try
                                                            {

                                                                OnClearChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearChargingProfileRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnClearChargingProfile?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnClearChargingProfileDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ClearChargingProfileResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnClearChargingProfileResponse event

                                                            try
                                                            {

                                                                OnClearChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearChargingProfileResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'ClearChargingProfile' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'ClearChargingProfile' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnClearChargingProfileWSResponse event

                                                    try
                                                    {

                                                        OnClearChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearChargingProfileWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "GetCompositeSchedule":
                                                {

                                                    #region Send OnGetCompositeScheduleWSRequest event

                                                    try
                                                    {

                                                        OnGetCompositeScheduleWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleWSRequest));
                                                    }

                                                    #endregion

                                                    GetCompositeScheduleResponse response = null;

                                                    try
                                                    {

                                                        if (GetCompositeScheduleRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out GetCompositeScheduleRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomGetCompositeScheduleRequestParser))
                                                        {

                                                            #region Send OnGetCompositeScheduleRequest event

                                                            try
                                                            {

                                                                OnGetCompositeScheduleRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnGetCompositeSchedule?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnGetCompositeScheduleDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = GetCompositeScheduleResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnGetCompositeScheduleResponse event

                                                            try
                                                            {

                                                                OnGetCompositeScheduleResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'GetCompositeSchedule' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'GetCompositeSchedule' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnGetCompositeScheduleWSResponse event

                                                    try
                                                    {

                                                        OnGetCompositeScheduleWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "UnlockConnector":
                                                {

                                                    #region Send OnUnlockConnectorWSRequest event

                                                    try
                                                    {

                                                        OnUnlockConnectorWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUnlockConnectorWSRequest));
                                                    }

                                                    #endregion

                                                    UnlockConnectorResponse response = null;

                                                    try
                                                    {

                                                        if (UnlockConnectorRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out UnlockConnectorRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomUnlockConnectorRequestParser))
                                                        {

                                                            #region Send OnUnlockConnectorRequest event

                                                            try
                                                            {

                                                                OnUnlockConnectorRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUnlockConnectorRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnUnlockConnector?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnUnlockConnectorDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = UnlockConnectorResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnUnlockConnectorResponse event

                                                            try
                                                            {

                                                                OnUnlockConnectorResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUnlockConnectorResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'UnlockConnector' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'UnlockConnector' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnUnlockConnectorWSResponse event

                                                    try
                                                    {

                                                        OnUnlockConnectorWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUnlockConnectorWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;


                                            case "GetLocalListVersion":
                                                {

                                                    #region Send OnGetLocalListVersionWSRequest event

                                                    try
                                                    {

                                                        OnGetLocalListVersionWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionWSRequest));
                                                    }

                                                    #endregion

                                                    GetLocalListVersionResponse response = null;

                                                    try
                                                    {

                                                        if (GetLocalListVersionRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out GetLocalListVersionRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomGetLocalListVersionRequestParser))
                                                        {

                                                            #region Send OnGetLocalListVersionRequest event

                                                            try
                                                            {

                                                                OnGetLocalListVersionRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnGetLocalListVersion?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnGetLocalListVersionDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = GetLocalListVersionResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnGetLocalListVersionResponse event

                                                            try
                                                            {

                                                                OnGetLocalListVersionResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'GetLocalListVersion' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'GetLocalListVersion' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnGetLocalListVersionWSResponse event

                                                    try
                                                    {

                                                        OnGetLocalListVersionWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "SendLocalList":
                                                {

                                                    #region Send OnSendLocalListWSRequest event

                                                    try
                                                    {

                                                        OnSendLocalListWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListWSRequest));
                                                    }

                                                    #endregion

                                                    SendLocalListResponse response = null;

                                                    try
                                                    {

                                                        if (SendLocalListRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out SendLocalListRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomSendLocalListRequestParser))
                                                        {

                                                            #region Send OnSendLocalListRequest event

                                                            try
                                                            {

                                                                OnSendLocalListRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnSendLocalList?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnSendLocalListDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = SendLocalListResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnSendLocalListResponse event

                                                            try
                                                            {

                                                                OnSendLocalListResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'SendLocalList' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'SendLocalList' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnSendLocalListWSResponse event

                                                    try
                                                    {

                                                        OnSendLocalListWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                            case "ClearCache":
                                                {

                                                    #region Send OnClearCacheWSRequest event

                                                    try
                                                    {

                                                        OnClearCacheWSRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 requestJSON);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheWSRequest));
                                                    }

                                                    #endregion

                                                    ClearCacheResponse response = null;

                                                    try
                                                    {

                                                        if (ClearCacheRequest.TryParse(wsRequestMessage.Message,
                                                                                               wsRequestMessage.RequestId,
                                                                                               ChargeBoxIdentity,
                                                                                               out ClearCacheRequest request,
                                                                                               out String ErrorResponse,
                                                                                               CustomClearCacheRequestParser))
                                                        {

                                                            #region Send OnClearCacheRequest event

                                                            try
                                                            {

                                                                OnClearCacheRequest?.Invoke(Timestamp.Now,
                                                                                                    this,
                                                                                                    request);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheRequest));
                                                            }

                                                            #endregion

                                                            #region Call async subscribers

                                                            if (response == null)
                                                            {

                                                                var results = OnClearCache?.
                                                                                    GetInvocationList()?.
                                                                                    SafeSelect(subscriber => (subscriber as OnClearCacheDelegate)
                                                                                        (Timestamp.Now,
                                                                                         this,
                                                                                         request,
                                                                                         cancellationTokenSource.Token)).
                                                                                    ToArray();

                                                                if (results?.Length > 0)
                                                                {

                                                                    await Task.WhenAll(results);

                                                                    response = results.FirstOrDefault()?.Result;

                                                                }

                                                                if (results == null || response == null)
                                                                    response = ClearCacheResponse.Failed(request);

                                                            }

                                                            #endregion

                                                            #region Send OnClearCacheResponse event

                                                            try
                                                            {

                                                                OnClearCacheResponse?.Invoke(Timestamp.Now,
                                                                                                     this,
                                                                                                     request,
                                                                                                     response,
                                                                                                     response.Runtime);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheResponse));
                                                            }

                                                            #endregion

                                                            OCPPResponseJSON = response.ToJSON();

                                                        }

                                                        else
                                                            ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                                "The given 'ClearCache' request could not be parsed!",
                                                                                                new JObject(
                                                                                                    new JProperty("request", requestJSON)
                                                                                                ));

                                                    }
                                                    catch (Exception e)
                                                    {

                                                        ErrorMessage = new OCPP_WebSocket_ErrorMessage(wsRequestMessage.RequestId,
                                                                                            OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                                            "Processing the given 'ClearCache' request led to an exception!",
                                                                                            new JObject(
                                                                                                new JProperty("request", requestJSON),
                                                                                                new JProperty("exception", e.Message),
                                                                                                new JProperty("stacktrace", e.StackTrace)
                                                                                            ));

                                                    }


                                                    #region Send OnClearCacheWSResponse event

                                                    try
                                                    {

                                                        OnClearCacheWSResponse?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               requestJSON,
                                                                                               new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId,
                                                                                                                     OCPPResponseJSON).ToJSON());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheWSResponse));
                                                    }

                                                    #endregion

                                                }
                                                break;

                                        }

                                        if (OCPPResponseJSON is not null)
                                        {

                                            var wsResponseMessage = new OCPP_WebSocket_ResponseMessage(wsRequestMessage.RequestId, OCPPResponseJSON);

                                            HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                                                WebSocketFrame.MaskStatus.On,
                                                                                new Byte[] { 0xaa, 0xaa, 0xaa, 0xaa },
                                                                                WebSocketFrame.Opcodes.Text,
                                                                                wsResponseMessage.ToByteArray(),
                                                                                WebSocketFrame.Rsv.Off,
                                                                                WebSocketFrame.Rsv.Off,
                                                                                WebSocketFrame.Rsv.Off).ToByteArray());

                                            File.AppendAllText(LogfileName,
                                                               String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                                Environment.NewLine,
                                                                             "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                             Environment.NewLine,
                                                                             "Message sent: ",      wsResponseMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                                                                             "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                                            HTTPStream.Flush();

                                        }

                                    }

                                    else if (OCPP_WebSocket_ResponseMessage.TryParse(textPayload, out OCPP_WebSocket_ResponseMessage wsResponseMessage))
                                    {
                                        DebugX.Log(nameof(ChargePointWSClient), " Received unknown OCPP response message: " + textPayload);
                                    }

                                    else if (OCPP_WebSocket_ErrorMessage.   TryParse(textPayload, out OCPP_WebSocket_ErrorMessage    wsErrorMessage))
                                    {
                                        DebugX.Log(nameof(ChargePointWSClient), " Received unknown OCPP error message: " + textPayload);
                                    }

                                    else
                                        DebugX.Log(nameof(ChargePointWSClient), " Received unknown OCPP request/response message: " + textPayload);

                                }
                                break;

                                case WebSocketFrame.Opcodes.Ping: {

                                    DebugX.Log(nameof(ChargePointWSClient) + ": Ping received!");

                                    HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                                        WebSocketFrame.MaskStatus.On,
                                                                        new Byte[] { 0xaa, 0xaa, 0xaa, 0xaa },
                                                                        WebSocketFrame.Opcodes.Pong,
                                                                        frame.Payload,
                                                                        WebSocketFrame.Rsv.Off,
                                                                        WebSocketFrame.Rsv.Off,
                                                                        WebSocketFrame.Rsv.Off).ToByteArray());

                                    HTTPStream.Flush();

                                }
                                break;

                                case WebSocketFrame.Opcodes.Pong: {
                                    DebugX.Log(nameof(ChargePointWSClient) + ": Pong received!");
                                }
                                break;

                                default:
                                    DebugX.Log(nameof(ChargePointWSClient), " Received unknown " + frame.Opcode + " frame!");
                                    //DebugX.Log(nameof(ChargePointWSClient), " Received unknown OCPP request/response message: " + textPayload);
                                    break;

                            }

                        }

                    }

                    if ((UInt64) buffer.Length > frameLength)
                    {
                        var newBuffer = new Byte[(UInt64) buffer.Length - frameLength];
                        Array.Copy(buffer, (UInt32) frameLength, newBuffer, 0, newBuffer.Length);
                        buffer = newBuffer;
                    }
                    else
                        buffer = null;

                } while (buffer != null);

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
                catch (ObjectDisposedException ode)
                {
                    MaintenanceTimer.Dispose();
                    TCPStream   = null;
                    HTTPStream  = null;
                }
                catch (Exception e)
                {

                    while (e.InnerException != null)
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


        public async Task<OCPP_WebSocket_ResponseMessage> SendRequest(String   Action,
                                                                      JObject  Message)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream != null)
                    {

                        Interlocked.Increment(ref requestId);

                        var wsRequestMessage = new OCPP_WebSocket_RequestMessage(Request_Id.Parse(requestId.ToString()), Action, Message);

                        HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                            WebSocketFrame.MaskStatus.On,
                                                            new Byte[] { 0xaa, 0xbb, 0xcc, 0xdd },
                                                            WebSocketFrame.Opcodes.Text,
                                                            wsRequestMessage.ToByteArray(),
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off).ToByteArray());

                        HTTPStream.Flush();

                        File.AppendAllText(LogfileName,
                                           String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                               Environment.NewLine,
                                                         "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                            Environment.NewLine,
                                                         "Message sent: ",      wsRequestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                                                         "--------------------------------------------------------------------------------------------", Environment.NewLine));

                        var buffer = new Byte[64 * 1024];
                        var pos    = 0;

                        do
                        {

                            pos += HTTPStream.Read(buffer, pos, 2048);

                            //if (sw.ElapsedMilliseconds >= RequestTimeout.Value.TotalMilliseconds)
                            //    throw new HTTPTimeoutException(sw.Elapsed);

                            Thread.Sleep(1);

                        } while (TCPStream.DataAvailable);

                        Array.Resize(ref buffer, pos);
                        //var frame = WebSocketFrame.Parse(buffer);

                        if (WebSocketFrame.TryParse(buffer,
                                                    out WebSocketFrame?  frame,
                                                    out UInt64           frameLength,
                                                    out String?          errorResponse))
                        {
                            if (frame is not null)
                            {
                                if (OCPP_WebSocket_ResponseMessage.TryParse(frame.Payload.ToUTF8String(), out OCPP_WebSocket_ResponseMessage wsResponseMessage))
                                {

                                    File.AppendAllText(LogfileName,
                                                       String.Concat("Timestamp: ",         Timestamp.Now.ToIso8601(),                                                Environment.NewLine,
                                                                     "ChargeBoxId: ", ChargeBoxIdentity.ToString(),                                             Environment.NewLine,
                                                                     "Message received: ",  wsResponseMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented), Environment.NewLine,
                                                                     "--------------------------------------------------------------------------------------------",  Environment.NewLine));

                                    return wsResponseMessage;

                                }
                                else if (OCPP_WebSocket_RequestMessage.TryParse(frame.Payload.ToUTF8String(), out OCPP_WebSocket_RequestMessage wsRequestMessage2))
                                {
                                    DebugX.Log(nameof(ChargePointWSClient), " received a WSRequestMessage when we expected a response message: " + frame.Payload.ToUTF8String());
                                }
                                else if (OCPP_WebSocket_ErrorMessage.TryParse(frame.Payload.ToUTF8String(), out OCPP_WebSocket_ErrorMessage wsErrorMessage))
                                {
                                    DebugX.Log(nameof(ChargePointWSClient), " received a WSErrorMessage when we expected a response message: " + frame.Payload.ToUTF8String());
                                }
                                else
                                    DebugX.Log(nameof(ChargePointWSClient), " error: " + frame.Payload.ToUTF8String());
                            }
                        }

                    }
                    else
                    {

                        DebugX.Log("Invalid web socket connection! Will try to reconnect!");

                        await Connect();

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException != null)
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

            return null;

        }



        #region SendBootNotification             (Request, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<BootNotificationResponse>

            SendBootNotification(BootNotificationRequest  Request,

                                 DateTime?                Timestamp           = null,
                                 CancellationToken?       CancellationToken   = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 TimeSpan?                RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given boot notification request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var responseFrame = await SendRequest("BootNotification",
                                                  Request.ToJSON());


            if (!BootNotificationResponse.TryParse(Request,
                                                   responseFrame?.Message,
                                                   out BootNotificationResponse  response,
                                                   out String                    ErrorResponse))
            {

                response = new BootNotificationResponse(Request,
                                                        Result.Format(ErrorResponse));

            }

            //RequestLogDelegate:   OnBootNotificationwebsocketRequest,
            //ResponseLogDelegate:  OnBootNotificationwebsocketResponse,
            //CancellationToken:    CancellationToken,
            //EventTrackingId:      EventTrackingId,
            //RequestTimeout:       RequestTimeout,


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   this,
                                                   Request,
                                                   response,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                    (Request, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HeartbeatResponse>

            SendHeartbeat(HeartbeatRequest    Request,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id?   EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given heartbeat Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var responseFrame = await SendRequest("Heartbeat",
                                                  Request.ToJSON());


            if (!HeartbeatResponse.TryParse(Request,
                                           responseFrame?.Message,
                                           out HeartbeatResponse  response,
                                           out String             ErrorResponse))
            {

                response = new HeartbeatResponse(Request,
                                                 Result.Format(ErrorResponse));

            }


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            this,
                                            Request,
                                            response,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Authorize                        (Request, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<AuthorizeResponse>

            Authorize(AuthorizeRequest    Request,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given authorize Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var responseFrame = await SendRequest("Authorize",
                                                  Request.ToJSON());


            if (!AuthorizeResponse.TryParse(Request,
                                            responseFrame?.Message,
                                            out AuthorizeResponse  response,
                                            out String             ErrorResponse))
            {

                response = new AuthorizeResponse(Request,
                                                 Result.Format(ErrorResponse));

            }


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            this,
                                            Request,
                                            response,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartTransaction                 (Request, ...)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A start transaction request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<StartTransactionResponse>

            StartTransaction(StartTransactionRequest  Request,

                             DateTime?                Timestamp          = null,
                             CancellationToken?       CancellationToken  = null,
                             EventTracking_Id?        EventTrackingId    = null,
                             TimeSpan?                RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given start transaction Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            var responseFrame = await SendRequest("StartTransaction",
                                                  Request.ToJSON());


            if (!StartTransactionResponse.TryParse(Request,
                                                   responseFrame?.Message,
                                                   out StartTransactionResponse  response,
                                                   out String                    ErrorResponse))
            {

                response = new StartTransactionResponse(Request,
                                                        Result.Format(ErrorResponse));

            }


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   this,
                                                   Request,
                                                   response,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification           (Request, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<StatusNotificationResponse>

            SendStatusNotification(StatusNotificationRequest  Request,

                                   DateTime?                  Timestamp          = null,
                                   CancellationToken?         CancellationToken  = null,
                                   EventTracking_Id?          EventTrackingId    = null,
                                   TimeSpan?                  RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var responseFrame = await SendRequest("StatusNotification",
                                                  Request.ToJSON());


            if (!StatusNotificationResponse.TryParse(Request,
                                                     responseFrame?.Message,
                                                     out StatusNotificationResponse  response,
                                                     out String                      ErrorResponse))
            {

                response = new StatusNotificationResponse(Request,
                                                          Result.Format(ErrorResponse));

            }


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     this,
                                                     Request,
                                                     response,
                                                     org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendMeterValues                  (Request, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<MeterValuesResponse>

            SendMeterValues(MeterValuesRequest  Request,

                            DateTime?           Timestamp          = null,
                            CancellationToken?  CancellationToken  = null,
                            EventTracking_Id?   EventTrackingId    = null,
                            TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given meter values Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var responseFrame = await SendRequest("MeterValues",
                                                  Request.ToJSON());


            if (!MeterValuesResponse.TryParse(Request,
                                              responseFrame?.Message,
                                              out MeterValuesResponse  response,
                                              out String               ErrorResponse))
            {

                response = new MeterValuesResponse(Request,
                                                   Result.Format(ErrorResponse));

            }


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              this,
                                              Request,
                                              response,
                                              org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopTransaction                  (Request, ...)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<StopTransactionResponse>

            StopTransaction(StopTransactionRequest  Request,

                            DateTime?               Timestamp          = null,
                            CancellationToken?      CancellationToken  = null,
                            EventTracking_Id?       EventTrackingId    = null,
                            TimeSpan?               RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given stop transaction Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            var responseFrame = await SendRequest("StopTransaction",
                                                  Request.ToJSON());


            if (!StopTransactionResponse.TryParse(Request,
                                                  responseFrame?.Message,
                                                  out StopTransactionResponse  response,
                                                  out String                   ErrorResponse))
            {

                response = new StopTransactionResponse(Request,
                                                       Result.Format(ErrorResponse));

            }


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request,
                                                  response,
                                                  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region TransferData                     (Request, ...)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.DataTransferResponse>

            TransferData(DataTransferRequest  Request,

                         DateTime?            Timestamp          = null,
                         CancellationToken?   CancellationToken  = null,
                         EventTracking_Id?    EventTrackingId    = null,
                         TimeSpan?            RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given data transfer Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var responseFrame = await SendRequest("DataTransfer",
                                                  Request.ToJSON());


            if (!CS.DataTransferResponse.TryParse(Request,
                                                  responseFrame?.Message,
                                                  out CS.DataTransferResponse  response,
                                                  out String                   ErrorResponse))
            {

                response = new CS.DataTransferResponse(Request,
                                                       Result.Format(ErrorResponse));

            }


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                               this,
                                               Request,
                                               response,
                                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendDiagnosticsStatusNotification(Request, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Request">A diagnostics status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<DiagnosticsStatusNotificationResponse>

            SendDiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest  Request,

                                              DateTime?                             Timestamp          = null,
                                              CancellationToken?                    CancellationToken  = null,
                                              EventTracking_Id?                     EventTrackingId    = null,
                                              TimeSpan?                             RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given diagnostics status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                               this,
                                                               Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            var responseFrame = await SendRequest("DiagnosticsStatusNotification",
                                                  Request.ToJSON());


            if (!DiagnosticsStatusNotificationResponse.TryParse(Request,
                                                                responseFrame?.Message,
                                                                out DiagnosticsStatusNotificationResponse  response,
                                                                out String                                 ErrorResponse))
            {

                response = new DiagnosticsStatusNotificationResponse(Request,
                                                                     Result.Format(ErrorResponse));

            }


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                this,
                                                                Request,
                                                                response,
                                                                org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFirmwareStatusNotification   (Request, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest  Request,

                                           DateTime?                          Timestamp          = null,
                                           CancellationToken?                 CancellationToken  = null,
                                           EventTracking_Id?                  EventTrackingId    = null,
                                           TimeSpan?                          RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given firmware status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                            this,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var responseFrame = await SendRequest("FirmwareStatusNotification",
                                                  Request.ToJSON());


            if (!FirmwareStatusNotificationResponse.TryParse(Request,
                                                             responseFrame?.Message,
                                                             out FirmwareStatusNotificationResponse  response,
                                                             out String                              ErrorResponse))
            {

                response = new FirmwareStatusNotificationResponse(Request,
                                                                  Result.Format(ErrorResponse));

            }


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                             this,
                                                             Request,
                                                             response,
                                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion



        #region Close()

        public void Close()
        {

            try
            {
                if (HTTPStream != null)
                {
                    HTTPStream.Close();
                    HTTPStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TCPStream != null)
                {
                    TCPStream.Close();
                    TCPStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                }
            }
            catch (Exception)
            { }

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

    }

}
