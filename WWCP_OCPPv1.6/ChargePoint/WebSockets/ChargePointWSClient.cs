/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
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
                                               ICPClient,
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

        private Int64            requestId;

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public readonly TimeSpan DefaultMaintenanceEvery = TimeSpan.FromSeconds(1);
        private static readonly SemaphoreSlim MaintenanceSemaphore = new SemaphoreSlim(1, 1);
        private readonly Timer MaintenanceTimer;

        protected static readonly TimeSpan SemaphoreSlimTimeout = TimeSpan.FromSeconds(5);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id    ChargeBoxIdentity   { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                  { get; }


        /// <summary>
        /// The attached OCPP CP client (HTTP/SOAP client) logger.
        /// </summary>
        public ChargePointSOAPClient.CPClientLogger Logger              { get; }



        /// <summary>
        /// The remote URL of the HTTP endpoint to connect to.
        /// </summary>
        public URL                                  RemoteURL                     { get; }

        /// <summary>
        /// The virtual HTTP hostname to connect to.
        /// </summary>
        public HTTPHostname?                        VirtualHostname               { get; }

        /// <summary>
        /// An optional description of this HTTP client.
        /// </summary>
        public String                               Description                   { get; set; }

        /// <summary>
        /// The remote SSL/TLS certificate validator.
        /// </summary>
        public RemoteCertificateValidationCallback  RemoteCertificateValidator    { get; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionCallback    ClientCertificateSelector     { get; }

        /// <summary>
        /// The SSL/TLS client certificate to use of HTTP authentication.
        /// </summary>
        public X509Certificate                      ClientCert                    { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String                               HTTPUserAgent                 { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan                             RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate       TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicationg with the remote OICP service.
        /// </summary>
        public UInt16                               MaxNumberOfRetries            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean                              UseHTTPPipelining             { get; }

        /// <summary>
        /// The CPO client (HTTP client) logger.
        /// </summary>
        public HTTPClientLogger                     HTTPLogger                    { get; set; }




        /// <summary>
        /// The DNS client defines which DNS servers to use.
        /// </summary>
        public DNSClient                            DNSClient                     { get; }



        /// <summary>
        /// Our local IP port.
        /// </summary>
        public IPPort           LocalPort           { get; private set; }

        /// <summary>
        /// The IP Address to connect to.
        /// </summary>
        public IIPAddress       RemoteIPAddress     { get; protected set; }


        public Int32 Available
                    => TCPSocket.Available;

        public Boolean Connected
            => TCPSocket.Connected;

        public LingerOption LingerState
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



        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan  MaintenanceEvery            { get; }

        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean   DisableMaintenanceTasks     { get; set; }

        #endregion

        #region Events

        // Outgoing messages (to central system)

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification Request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification Request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat Request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat Request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize Request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize Request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction Request will be send to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction Request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification Request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler               OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler              OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification Request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values Request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler        OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler       OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values Request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction Request will be send to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler            OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction Request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer Request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler         OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer Request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification Request will be send to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                          OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification Request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification Request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                       OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification Request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion


        // Incoming messages (from central system)

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate      OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate             OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate     OnResetResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnResetWSResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate      OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate             OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate     OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnChangeAvailabilityWSResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnGetConfigurationWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationRequestDelegate      OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationDelegate             OnGetConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetConfigurationResponseDelegate     OnGetConfigurationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnGetConfigurationWSResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnChangeConfigurationWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate      OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationDelegate             OnChangeConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate     OnChangeConfigurationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnChangeConfigurationWSResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler                       OnIncomingDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate          OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler                        OnIncomingDataTransferSOAPResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnGetDiagnosticsWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate      OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsDelegate             OnGetDiagnostics;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate     OnGetDiagnosticsResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnGetDiagnosticsWSResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate      OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageDelegate             OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate     OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnTriggerMessageWSResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate      OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate             OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate     OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnUpdateFirmwareWSResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now SOAP request was received.
        /// </summary>
        public event RequestLogHandler             OnReserveNowSOAPRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate          OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate  OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reserve now request was sent.
        /// </summary>
        public event AccessLogHandler              OnReserveNowSOAPResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation SOAP request was received.
        /// </summary>
        public event RequestLogHandler                    OnCancelReservationSOAPRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate          OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a cancel reservation request was sent.
        /// </summary>
        public event AccessLogHandler                     OnCancelReservationSOAPResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a remote start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                         OnRemoteStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction was received.
        /// </summary>
        public event OnRemoteStartTransactionDelegate          OnRemoteStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a remote start transaction request was sent.
        /// </summary>
        public event AccessLogHandler                          OnRemoteStartTransactionSOAPResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a remote stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                        OnRemoteStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction was received.
        /// </summary>
        public event OnRemoteStopTransactionDelegate          OnRemoteStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate  OnRemoteStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a remote stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler                         OnRemoteStopTransactionSOAPResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate      OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate             OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate     OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnSetChargingProfileWSResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnClearChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate      OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate             OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate     OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnClearChargingProfileWSResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate      OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate             OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate     OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnGetCompositeScheduleWSResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnUnlockConnectorWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate      OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate             OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate     OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnUnlockConnectorWSResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate      OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate             OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate     OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnGetLocalListVersionWSResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate      OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate             OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate     OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnSendLocalListWSResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event WSClientRequestLogHandler   OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate      OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate             OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate     OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler  OnClearCacheWSResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region ChargePointWSClient(Request.ChargeBoxId, Hostname, ..., LoggingContext = CPClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new charge point SOAP client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/SOAP client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="WSSLoginPassword">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargePointWSClient(ChargeBox_Id                         ChargeBoxIdentity,
                                   String                               From,
                                   String                               To,

                                   URL                                  RemoteURL,
                                   HTTPHostname?                        VirtualHostname              = null,
                                   String                               Description                  = null,
                                   RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                                   X509Certificate                      ClientCert                   = null,
                                   String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                   HTTPPath?                            URLPathPrefix                = null,
                                   Tuple<String, String>                WSSLoginPassword             = null,
                                   TimeSpan?                            RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate       TransmissionRetryDelay       = null,
                                   UInt16?                              MaxNumberOfRetries           = 3,
                                   Boolean                              UseHTTPPipelining            = false,

                                   TimeSpan?                            MaintenanceEvery             = null,

                                   String                               LoggingPath                  = null,
                                   String                               LoggingContext               = ChargePointSOAPClient.CPClientLogger.DefaultContext,
                                   LogfileCreatorDelegate               LogFileCreator               = null,
                                   HTTPClientLogger                     HTTPLogger                   = null,
                                   DNSClient                            DNSClient                    = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

            #endregion

            this.RemoteURL                   = RemoteURL;
            this.VirtualHostname             = VirtualHostname;
            this.Description                 = Description;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;
            this.ClientCertificateSelector   = ClientCertificateSelector;
            this.ClientCert                  = ClientCert;
            this.HTTPUserAgent               = HTTPUserAgent;
            //this.URLPathPrefix               = URLPathPrefix;
            //this.WSSLoginPassword            = WSSLoginPassword;
            this.RequestTimeout              = RequestTimeout     ?? TimeSpan.FromMinutes(10);
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries ?? 3;
            this.UseHTTPPipelining           = UseHTTPPipelining;
            this.HTTPLogger                  = HTTPLogger;
            this.DNSClient                   = DNSClient;

            this.ChargeBoxIdentity           = ChargeBoxIdentity;
            this.From                        = From;
            this.To                          = To;

            //this.Logger                      = new ChargePointSOAPClient.CPClientLogger(this,
            //                                                                            LoggingPath,
            //                                                                            LoggingContext,
            //                                                                            LogFileCreator);

            this.MaintenanceEvery            = MaintenanceEvery ?? DefaultMaintenanceEvery;
            this.MaintenanceTimer            = new Timer(DoMaintenanceSync,
                                                         null,
                                                         this.MaintenanceEvery,
                                                         this.MaintenanceEvery);

        }

        #endregion

        #region ChargePointWSClient(Request.ChargeBoxId, Logger, Hostname, ...)

        /// <summary>
        /// Create a new charge point SOAP client.
        /// </summary>
        /// <param name="ChargeBoxIdentity">A unqiue identification of this client.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
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
        //        throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

        //    if (To.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

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

                #endregion

                this.LocalPort        = IPSocket.FromIPEndPoint(TCPStream.Socket.LocalEndPoint).Port;
                //Request.HTTPSource    = new HTTPSource(IPSocket.FromIPEndPoint(TCPStream.Socket.LocalEndPoint));
                //this.RemotePort       = IPSocket.FromIPEndPoint(TCPStream.Socket.RemoteEndPoint).Port;

                #region Call the optional HTTP request log delegate

                //try
                //{

                //    if (RequestLogDelegate != null)
                //        await Task.WhenAll(RequestLogDelegate.GetInvocationList().
                //                           Cast<ClientRequestLogHandler>().
                //                           Select(e => e(DateTime.UtcNow,
                //                                         this,
                //                                         Request))).
                //                           ConfigureAwait(false);

                //}
                //catch (Exception e)
                //{
                //    DebugX.Log(e, nameof(HTTPClient) + "." + nameof(RequestLogDelegate));
                //}

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
                    SecWebSocketVersion   = "13"
                }.AsImmutable;

                HTTPStream.Write((request.EntirePDU + "\r\n\r\n").ToUTF8Bytes());

                //HTTPStream.Write(String.Concat("GET ", RemoteURL, " ", "HTTP/1.1\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Host: ", RemoteURL.Hostname, ":", RemoteURL.Port, "\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Connection: Upgrade", "\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Upgrade: websocket", "\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Sec-WebSocket-Key: x3JJHMbDL1EzLkh9GBhXDw==", "\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Sec-WebSocket-Protocol: ocpp1.6", "\r\n").ToUTF8Bytes());
                //HTTPStream.Write(String.Concat("Sec-WebSocket-Version: 13", "\r\n").ToUTF8Bytes());


                // HTTP/1.1 101 Switching Protocols
                // Upgrade: websocket
                // Connection: Upgrade
                // Sec-WebSocket-Accept: s3pPLMBiTxaQ9kYGzzhZRbK+xOo=
                // Sec-WebSocket-Protocol: ocpp1.6

                var buffer  = new Byte[16 * 1024];
                var pos     = 0;

                do
                {

                    pos += HTTPStream.Read(buffer, pos, 2048);

                    if (sw.ElapsedMilliseconds >= RequestTimeout.Value.TotalMilliseconds)
                        throw new HTTPTimeoutException(sw.Elapsed);

                    Thread.Sleep(1);

                } while (TCPStream.DataAvailable);

                var responseData = buffer.ToUTF8String(pos);

                response = HTTPResponse.Parse(responseData,
                                              new Byte[0],
                                              request);

                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                //var swk             = WSConnection.GetHTTPHeader("Sec-WebSocket-Key");
                //var swka            = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                //var swkaSha1        = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                //var swkaSha1Base64  = Convert.ToBase64String(swkaSha1);

                var _InternalHTTPStream  = new MemoryStream();

                #endregion


                #region Close connection if requested!

                if (response.Connection == null ||
                    response.Connection == "close")
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


            #region Call the optional HTTP response log delegate

            //try
            //{

            //    if (ResponseLogDelegate != null)
            //        await Task.WhenAll(ResponseLogDelegate.GetInvocationList().
            //                           Cast<ClientResponseLogHandler>().
            //                           Select(e => e(DateTime.UtcNow,
            //                                         this,
            //                                         Request,
            //                                         Response))).
            //                           ConfigureAwait(false);

            //}
            //catch (Exception e2)
            //{
            //    DebugX.Log(e2, nameof(HTTPClient) + "." + nameof(ResponseLogDelegate));
            //}

            #endregion

            return response;

        }

        #endregion


        public CustomJObjectParserDelegate<ResetRequest>                CustomResetRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>   CustomChangeAvailabilityRequestParser       { get; set; }


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
                var frame = WebSocketFrame.Parse(buffer);

                if (WSRequestMessage.TryParse(frame.Payload.ToUTF8String(), out WSRequestMessage wsRequest))
                {

                    var requestJSON                  = JArray.Parse(frame.Payload.ToUTF8String());
                    var cancellationTokenSource      = new CancellationTokenSource();
                    JObject        OCPPResponseJSON  = null;
                    WSErrorMessage ErrorMessage      = null;


                    switch (wsRequest.Action)
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

                                    if (ResetRequest.TryParse(wsRequest.Message,
                                                              wsRequest.RequestId,
                                                              ChargeBoxIdentity,
                                                              out ResetRequest request,
                                                              out String       ErrorResponse,
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
                                        ErrorMessage =  new WSErrorMessage(wsRequest.RequestId,
                                                                            WSErrorCodes.FormationViolation,
                                                                            "The given 'Reset' request could not be parsed!",
                                                                            new JObject(
                                                                                new JProperty("request", requestJSON)
                                                                            ));

                                }
                                catch (Exception e)
                                {

                                    ErrorMessage = new WSErrorMessage(wsRequest.RequestId,
                                                                        WSErrorCodes.FormationViolation,
                                                                        "Processing the given 'Reset' request led to an exception!",
                                                                        new JObject(
                                                                            new JProperty("request",     requestJSON),
                                                                            new JProperty("exception",   e.Message),
                                                                            new JProperty("stacktrace",  e.StackTrace)
                                                                        ));

                                }


                                #region Send OnResetWSResponse event

                                try
                                {

                                    OnResetWSResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON,
                                                              new WSResponseMessage(wsRequest.RequestId,
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

                                    if (ChangeAvailabilityRequest.TryParse(wsRequest.Message,
                                                                           wsRequest.RequestId,
                                                                           ChargeBoxIdentity,
                                                                           out ChangeAvailabilityRequest request,
                                                                           out String                    ErrorResponse,
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
                                        ErrorMessage =  new WSErrorMessage(wsRequest.RequestId,
                                                                            WSErrorCodes.FormationViolation,
                                                                            "The given 'ChangeAvailability' request could not be parsed!",
                                                                            new JObject(
                                                                                new JProperty("request", requestJSON)
                                                                            ));

                                }
                                catch (Exception e)
                                {

                                    ErrorMessage = new WSErrorMessage(wsRequest.RequestId,
                                                                        WSErrorCodes.FormationViolation,
                                                                        "Processing the given 'ChangeAvailability' request led to an exception!",
                                                                        new JObject(
                                                                            new JProperty("request",     requestJSON),
                                                                            new JProperty("exception",   e.Message),
                                                                            new JProperty("stacktrace",  e.StackTrace)
                                                                        ));

                                }


                                #region Send OnChangeAvailabilityWSResponse event

                                try
                                {

                                    OnChangeAvailabilityWSResponse?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           requestJSON,
                                                                           new WSResponseMessage(wsRequest.RequestId,
                                                                                                 OCPPResponseJSON).ToJSON());

                                }
                                catch (Exception e)
                                {
                                    DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
                                }

                                #endregion

                            }
                            break;


























                            break;

                    }


                    if (OCPPResponseJSON != null)
                    {

                        HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                            WebSocketFrame.MaskStatus.On,
                                                            new Byte[] { 0xaa, 0xaa, 0xaa, 0xaa },
                                                            WebSocketFrame.Opcodes.Text,
                                                            new WSResponseMessage(wsRequest.RequestId, OCPPResponseJSON).ToByteArray(),
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off,
                                                            WebSocketFrame.Rsv.Off).ToByteArray());

                        HTTPStream.Flush();

                    }

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


        private async Task<WSResponseMessage> SendRequest(String   Action,
                                                          JObject  Message)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    Interlocked.Increment(ref requestId);

                    HTTPStream.Write(new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                        WebSocketFrame.MaskStatus.On,
                                                        new Byte[] { 0xaa, 0xaa, 0xaa, 0xaa },
                                                        WebSocketFrame.Opcodes.Text,
                                                        new WSRequestMessage(Request_Id.Parse(requestId.ToString()), Action, Message).ToByteArray(),
                                                        WebSocketFrame.Rsv.Off,
                                                        WebSocketFrame.Rsv.Off,
                                                        WebSocketFrame.Rsv.Off).ToByteArray());

                    HTTPStream.Flush();


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
                    var frame = WebSocketFrame.Parse(buffer);

                    if (WSResponseMessage.TryParse(frame.Payload.ToUTF8String(), out WSResponseMessage response))
                        return response;

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
                                 EventTracking_Id         EventTrackingId     = null,
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


            BootNotificationResponse response = null;

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
                                                   out response))
            {

                response = new BootNotificationResponse(Request,
                                                        Result.OK("Nothing to upload!"));

            }

            //RequestLogDelegate:   OnBootNotificationSOAPRequest,
            //ResponseLogDelegate:  OnBootNotificationSOAPResponse,
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
                          EventTracking_Id    EventTrackingId     = null,
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


            HeartbeatResponse response = null;

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


            if (HeartbeatResponse.TryParse(Request,
                                           responseFrame?.Message,
                                           out response))
            {

                response = new HeartbeatResponse(Request,
                                                 Result.OK("Nothing to upload!"));

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
                      EventTracking_Id    EventTrackingId     = null,
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


            AuthorizeResponse response = null;

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


            if (AuthorizeResponse.TryParse(Request,
                                           responseFrame?.Message,
                                           out response))
            {

                response = new AuthorizeResponse(Request,
                                                 Result.OK("Nothing to upload!"));

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
                             EventTracking_Id         EventTrackingId    = null,
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


            StartTransactionResponse response = null;

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


            if (StartTransactionResponse.TryParse(Request,
                                                  responseFrame?.Message,
                                                  out response))
            {

                response = new StartTransactionResponse(Request,
                                                        Result.OK("Nothing to upload!"));

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
                                   EventTracking_Id           EventTrackingId    = null,
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


            StatusNotificationResponse response = null;

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


            if (StatusNotificationResponse.TryParse(Request,
                                                    responseFrame?.Message,
                                                    out response))
            {

                response = new StatusNotificationResponse(Request,
                                                          Result.OK("Nothing to upload!"));

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
                            EventTracking_Id    EventTrackingId    = null,
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


            MeterValuesResponse response = null;

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


            if (MeterValuesResponse.TryParse(Request,
                                             responseFrame?.Message,
                                             out response))
            {

                response = new MeterValuesResponse(Request,
                                                   Result.OK("Nothing to upload!"));

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
                            EventTracking_Id        EventTrackingId    = null,
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


            StopTransactionResponse response = null;

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
                                                  out response))
            {

                response = new StopTransactionResponse(Request,
                                                       Result.OK("Nothing to upload!"));

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
                         EventTracking_Id     EventTrackingId    = null,
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


            CS.DataTransferResponse response = null;

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


            var responseFrame = await SendRequest("StopTransaction",
                                                  Request.ToJSON());


            if (!CS.DataTransferResponse.TryParse(Request,
                                                  responseFrame?.Message,
                                                  out response))
            {

                response = new CS.DataTransferResponse(Request,
                                                       Result.OK("Nothing to upload!"));

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
                                              EventTracking_Id                      EventTrackingId    = null,
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


            DiagnosticsStatusNotificationResponse response = null;

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
                                                                out response))
            {

                response = new DiagnosticsStatusNotificationResponse(Request,
                                                                     Result.OK("Nothing to upload!"));

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
                                           EventTracking_Id                   EventTrackingId    = null,
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


            FirmwareStatusNotificationResponse response = null;

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
                                                             out response))
            {

                response = new FirmwareStatusNotificationResponse(Request,
                                                                  Result.OK("Nothing to upload!"));

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
