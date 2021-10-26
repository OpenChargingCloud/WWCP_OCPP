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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charge point for testing.
    /// </summary>
    public class TestChargePoint : IEventSender
    {

        /// <summary>
        /// A charge point connector.
        /// </summary>
        public class ChargePointConnector
        {

            public Connector_Id     Id                       { get; }

            public Availabilities   Availability             { get; set; }


            public Boolean          IsReserved               { get; set; }

            public Boolean          IsCharging               { get; set; }

            public IdToken          IdToken                  { get; set; }

            public IdTagInfo        IdTagInfo                { get; set; }

            public Transaction_Id   TransactionId            { get; set; }

            public ChargingProfile  ChargingProfile          { get; set; }


            public DateTime         StartTimestamp           { get; set; }

            public UInt64           MeterStartValue          { get; set; }

            public String           SignedStartMeterValue    { get; set; }


            public DateTime         StopTimestamp            { get; set; }

            public UInt64           MeterStopValue           { get; set; }

            public String           SignedStopMeterValue     { get; set; }


            public ChargePointConnector(Connector_Id    Id,
                                        Availabilities  Availability)
            {

                this.Id            = Id;
                this.Availability  = Availability;

            }


        }


        /// <summary>
        /// A configuration value.
        /// </summary>
        public class ConfigurationData
        {

            /// <summary>
            /// The configuration value.
            /// </summary>
            public String   Value              { get; set; }

            /// <summary>
            /// This configuration value can not be changed.
            /// </summary>
            public Boolean  IsReadOnly         { get; }

            /// <summary>
            /// Changing this configuration value requires a reboot of the charge box to take effect.
            /// </summary>
            public Boolean  RebootRequired     { get; }

            /// <summary>
            /// Create a new configuration value.
            /// </summary>
            /// <param name="Value">The configuration value.</param>
            /// <param name="IsReadOnly">This configuration value can not be changed.</param>
            /// <param name="RebootRequired">Changing this configuration value requires a reboot of the charge box to take effect.</param>
            public ConfigurationData(String   Value,
                                     Boolean  IsReadOnly,
                                     Boolean  RebootRequired = false)
            {

                this.Value           = Value;
                this.IsReadOnly      = IsReadOnly;
                this.RebootRequired  = RebootRequired;

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

            public IRequest        Request           { get; }

            public DateTime        EnqueTimestamp    { get; }

            public EnquedStatus    Status            { get; set; }

            public Action<Object>  ResponseAction    { get; }

            public EnquedRequest(IRequest        Request,
                                 DateTime        EnqueTimestamp,
                                 EnquedStatus    Status,
                                 Action<Object>  ResponseAction)
            {

                this.Request         = Request;
                this.EnqueTimestamp  = EnqueTimestamp;
                this.Status          = Status;
                this.ResponseAction  = ResponseAction;

            }

        }



        #region Data

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly TimeSpan DefaultSendHeartbeatEvery = TimeSpan.FromMinutes(5);

        protected static readonly TimeSpan SemaphoreSlimTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public readonly TimeSpan DefaultMaintenanceEvery = TimeSpan.FromMinutes(1);
        private static readonly SemaphoreSlim MaintenanceSemaphore = new SemaphoreSlim(1, 1);
        private readonly Timer MaintenanceTimer;

        private readonly Timer SendHeartbeatTimer;


        private readonly List<EnquedRequest> EnquedRequests;


        public DNSClient DNSClient { get; }

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a central system.
        /// </summary>
        public ICPClient                CPClient                    { get; private set; }


        public ChargePointSOAPServer    CPServer                    { get; private set; }


        /// <summary>
        /// The charge box identification.
        /// </summary>
        public ChargeBox_Id             ChargeBoxId                 { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxId.ToString();

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        public String                   ChargePointModel            { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        public I18NString               Description                 { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargeBoxSerialNumber       { get; }

        /// <summary>
        /// The optional firmware version of the charge point.
        /// </summary>
        public String                   FirmwareVersion             { get; }

        /// <summary>
        /// The optional ICCID of the charge point's SIM card.
        /// </summary>
        public String                   Iccid                       { get; }

        /// <summary>
        /// The optional IMSI of the charge point’s SIM card.
        /// </summary>
        public String                   IMSI                        { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charge point.
        /// </summary>
        public String                   MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charge point.
        /// </summary>
        public String                   MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charge point.
        /// </summary>
        public String                   MeterPublicKey              { get; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; set; }

        /// <summary>
        /// The time at the central system.
        /// </summary>
        public DateTime?                CentralSystemTime           { get; private set; }

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






        // Controlled by the central system!

        private readonly Dictionary<Connector_Id, ChargePointConnector> connectors;

        public IEnumerable<ChargePointConnector> Connectors
            => connectors.Values;


        public readonly Dictionary<String, ConfigurationData> Configuration;

        #endregion

        #region Events

        // Outgoing messages (to central system)

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction request will be send to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction request will be send to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification request will be send to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion



        // Incoming messages (from central system)

        #region OnResetRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate  OnResetResponse;

        #endregion

        #region ChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate  OnChangeAvailabilityResponse;

        #endregion

        #region GetConfigurationRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationRequestDelegate   OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetConfigurationResponseDelegate  OnGetConfigurationResponse;

        #endregion

        #region ChangeConfigurationRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate   OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate  OnChangeConfigurationResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

        #endregion

        #region GetDiagnosticsRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate  OnGetDiagnosticsResponse;

        #endregion

        #region TriggerMessageRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate  OnTriggerMessageResponse;

        #endregion

        #region UpdateFirmwareRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransactionRequest/-Response

        /// <summary>
        /// An event sent whenever a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransactionRequest/-Response

        /// <summary>
        /// An event sent whenever a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate  OnRemoteStopTransactionResponse;

        #endregion

        #region SetChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate  OnSetChargingProfileResponse;

        #endregion

        #region ClearChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate  OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnectorRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate  OnUnlockConnectorResponse;

        #endregion


        #region GetLocalListVersionRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalListRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate  OnSendLocalListResponse;

        #endregion

        #region ClearCacheRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate  OnClearCacheResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NumberOfConnectors">Number of available connectors.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charge box description.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
        public TestChargePoint(ChargeBox_Id  ChargeBoxId,
                               Byte          NumberOfConnectors,
                               String        ChargePointVendor,
                               String        ChargePointModel,

                               I18NString    Description               = null,
                               String        ChargePointSerialNumber   = null,
                               String        ChargeBoxSerialNumber     = null,
                               String        FirmwareVersion           = null,
                               String        Iccid                     = null,
                               String        IMSI                      = null,
                               String        MeterType                 = null,
                               String        MeterSerialNumber         = null,
                               String        MeterPublicKey            = null,

                               TimeSpan?     SendHeartbeatEvery        = null,

                               TimeSpan?     DefaultRequestTimeout     = null,
                               DNSClient     DNSClient                 = null)

        {

            if (ChargeBoxId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxId),        "The given charge box identification must not be null or empty!");

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model must not be null or empty!");


            this.ChargeBoxId              = ChargeBoxId;

            this.connectors               = new Dictionary<Connector_Id, ChargePointConnector>();
            for (var i = 1; i <= NumberOfConnectors; i++)
            {
                this.connectors.Add(Connector_Id.Parse(i.ToString()),
                                    new ChargePointConnector(Connector_Id.Parse(i.ToString()),
                                                             Availabilities.Inoperative));
            }

            this.Configuration            = new Dictionary<String, ConfigurationData>();
            this.EnquedRequests           = new List<EnquedRequest>();

            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;

            this.Description              = Description;
            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;

            this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

            this.DisableSendHeartbeats    = true;
            this.SendHeartbeatTimer       = new Timer(DoSendHeartbeatSync,
                                                      null,
                                                      this.SendHeartbeatEvery,
                                                      this.SendHeartbeatEvery);

            this.DNSClient                = DNSClient;

        }

        #endregion


        #region InitSOAP(...)

        public async Task InitSOAP(String                               From,
                                   String                               To,

                                   URL                                  RemoteURL,
                                   HTTPHostname?                        VirtualHostname              = null,
                                   String                               Description                  = null,
                                   RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                                   X509Certificate                      ClientCert                   = null,
                                   String                               HTTPUserAgent                = null,
                                   HTTPPath?                            URLPathPrefix                = null,
                                   Tuple<String, String>                WSSLoginPassword             = null,
                                   HTTPContentType                      HTTPContentType              = null,
                                   TimeSpan?                            RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate       TransmissionRetryDelay       = null,
                                   UInt16?                              MaxNumberOfRetries           = null,
                                   Boolean                              UseHTTPPipelining            = false,
                                   String                               LoggingPath                  = null,
                                   String                               LoggingContext               = null,
                                   LogfileCreatorDelegate               LogFileCreator               = null,
                                   HTTPClientLogger                     HTTPLogger                   = null,

                                   String                               HTTPServerName               = null,
                                   IPPort?                              TCPPort                      = null,
                                   String                               ServiceName                  = null,
                                   HTTPPath?                            URLPrefix                    = null,
                                   HTTPContentType                      ContentType                  = null,
                                   Boolean                              RegisterHTTPRootService      = true,
                                   DNSClient                            DNSClient                    = null,
                                   Boolean                              AutoStart                    = false)

        {

            this.CPClient = new ChargePointSOAPClient(ChargeBoxId,
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
                                                      WSSLoginPassword,
                                                      HTTPContentType,
                                                      RequestTimeout,
                                                      TransmissionRetryDelay,
                                                      MaxNumberOfRetries,
                                                      UseHTTPPipelining,
                                                      LoggingPath,
                                                      LoggingContext,
                                                      LogFileCreator,
                                                      HTTPLogger,
                                                      DNSClient ?? this.DNSClient);

            this.CPServer = new ChargePointSOAPServer(HTTPServerName,
                                                      TCPPort,
                                                      ServiceName,
                                                      URLPrefix,
                                                      ContentType,
                                                      RegisterHTTPRootService,
                                                      DNSClient ?? this.DNSClient,
                                                      AutoStart);

            WireEvents(CPServer);

        }

        #endregion

        #region ConnectWebSocket(...)

        public async Task ConnectWebSocket(String                               From,
                                           String                               To,

                                           URL                                  RemoteURL,
                                           HTTPHostname?                        VirtualHostname              = null,
                                           String                               Description                  = null,
                                           RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                                           LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                                           X509Certificate                      ClientCert                   = null,
                                           String                               HTTPUserAgent                = null,
                                           HTTPPath?                            URLPathPrefix                = null,
                                           Tuple<String, String>                HTTPBasicAuth                = null,
                                           TimeSpan?                            RequestTimeout               = null,
                                           TransmissionRetryDelayDelegate       TransmissionRetryDelay       = null,
                                           UInt16?                              MaxNumberOfRetries           = null,
                                           Boolean                              UseHTTPPipelining            = false,
                                           String                               LoggingPath                  = null,
                                           String                               LoggingContext               = null,
                                           LogfileCreatorDelegate               LogFileCreator               = null,
                                           HTTPClientLogger                     HTTPLogger                   = null,
                                           DNSClient                            DNSClient                    = null)

        {

            var WSClient   = new ChargePointWSClient(ChargeBoxId,
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
                                                     HTTPBasicAuth,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining,
                                                     null,
                                                     LoggingPath,
                                                     LoggingContext,
                                                     LogFileCreator,
                                                     HTTPLogger,
                                                     DNSClient ?? this.DNSClient);

            this.CPClient  = WSClient;

            WireEvents(WSClient);

            await WSClient.Connect();

        }

        #endregion

        #region WireEvents(CPServer)

        public void WireEvents(IChargePointServerEvents CPServer)
        {

            #region OnReset

            CPServer.OnReset += async (LogTimestamp,
                                       Sender,
                                       Request,
                                       CancellationToken) => {

                #region Send OnResetRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnResetRequest?.Invoke(requestTimestamp,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetRequest));
                }

                #endregion


                await Task.Delay(10);


                ResetResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {
                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid reset request for charge box '" + Request.ChargeBoxId + "'!");
                    response = new ResetResponse(Request,
                                                 ResetStatus.Rejected);
                }
                else
                {
                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming '" + Request.ResetType + "' reset request.");
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
                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnChangeAvailabilityRequest?.Invoke(requestTimestamp,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityRequest));
                }

                #endregion


                await Task.Delay(10);


                ChangeAvailabilityResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid ChangeAvailability request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new ChangeAvailabilityResponse(Request,
                                                              AvailabilityStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming ChangeAvailability '" + Request.Availability + "' request for connector '" + Request.ConnectorId + "'.");

                    if (connectors.ContainsKey(Request.ConnectorId))
                    {

                        connectors[Request.ConnectorId].Availability = Request.Availability;

                        response = new ChangeAvailabilityResponse(Request,
                                                                  AvailabilityStatus.Accepted);

                    }
                    else
                        response = new ChangeAvailabilityResponse(Request,
                                                                  AvailabilityStatus.Rejected);
                }


                #region Send OnChangeAvailabilityResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnChangeAvailabilityResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetConfiguration

            CPServer.OnGetConfiguration += async (LogTimestamp,
                                                  Sender,
                                                  Request,
                                                  CancellationToken) => {

                #region Send OnGetConfigurationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnGetConfigurationRequest?.Invoke(requestTimestamp,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationRequest));
                }

                #endregion


                await Task.Delay(10);


                GetConfigurationResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid GetConfiguration request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new GetConfigurationResponse(Request,
                                                            new ConfigurationKey[0],
                                                            Request.Keys);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming GetConfiguration request.");

                    var _configurationKeys  = new List<ConfigurationKey>();
                    var _unkownKeys         = new List<String>();

                    foreach (var key in Request.Keys)
                    {

                        if (Configuration.TryGetValue(key, out ConfigurationData Data))
                            _configurationKeys.Add(new ConfigurationKey(key,
                                                                        Data.IsReadOnly,
                                                                        Data.Value));

                        else
                            _unkownKeys.Add(key);

                    }

                    response = new GetConfigurationResponse(Request,
                                                            _configurationKeys,
                                                            _unkownKeys);

                }


                #region Send OnGetConfigurationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetConfigurationResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnChangeConfiguration

            CPServer.OnChangeConfiguration += async (LogTimestamp,
                                                     Sender,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnChangeConfigurationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnChangeConfigurationRequest?.Invoke(requestTimestamp,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationRequest));
                }

                #endregion


                await Task.Delay(10);


                ChangeConfigurationResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid ChangeConfiguration request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new ChangeConfigurationResponse(Request,
                                                               ConfigurationStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming ChangeConfiguration for '" + Request.Key + "' with value '" + Request.Value + "'.");

                    if (Configuration.TryGetValue(Request.Key, out ConfigurationData Data) &&
                        !Data.IsReadOnly)
                    {

                        Data.Value  = Request.Value;
                        response    = new ChangeConfigurationResponse(Request,
                                                                      Data.RebootRequired
                                                                          ? ConfigurationStatus.RebootRequired
                                                                          : ConfigurationStatus.Accepted);

                    }
                    else
                        response = new ChangeConfigurationResponse(Request,
                                                                   ConfigurationStatus.Rejected);

                }


                #region Send OnChangeConfigurationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnChangeConfigurationResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(requestTimestamp,
                                                          this,
                                                          Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
                }

                #endregion


                await Task.Delay(10);


                DataTransferResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid DataTransfer request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new DataTransferResponse(Request,
                                                        DataTransferStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming DataTransfer request: " + Request.VendorId + "/" + Request.MessageId);

                    response = new DataTransferResponse(Request,
                                                        DataTransferStatus.Rejected);

                }


                #region Send OnDataTransferResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnIncomingDataTransferResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           Request,
                                                           response,
                                                           responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetDiagnostics

            CPServer.OnGetDiagnostics += async (LogTimestamp,
                                                Sender,
                                                Request,
                                                CancellationToken) => {

                #region Send OnGetDiagnosticsRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnGetDiagnosticsRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsRequest));
                }

                #endregion


                await Task.Delay(10);


                GetDiagnosticsResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid GetDiagnostics request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new GetDiagnosticsResponse(Request);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming GetDiagnostics request");

                    response = new GetDiagnosticsResponse(Request);

                }


                #region Send OnGetDiagnosticsResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetDiagnosticsResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     Request,
                                                     response,
                                                     responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnTriggerMessageRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageRequest));
                }

                #endregion


                await Task.Delay(10);


                TriggerMessageResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid TriggerMessage request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new TriggerMessageResponse(Request,
                                                          TriggerMessageStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at connector '" + Request.ConnectorId + "'.");

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
                                                     responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnUpdateFirmwareRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareRequest));
                }

                #endregion


                await Task.Delay(10);


                UpdateFirmwareResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid UpdateFirmware request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new UpdateFirmwareResponse(Request);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming UpdateFirmware request for '" + Request.Location + "'.");

                    response = new UpdateFirmwareResponse(Request);

                }


                #region Send OnUpdateFirmwareResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnUpdateFirmwareResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     Request,
                                                     response,
                                                     responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnReserveNowRequest?.Invoke(requestTimestamp,
                                                this,
                                                Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowRequest));
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
                                                 responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnCancelReservationRequest?.Invoke(requestTimestamp,
                                                       this,
                                                       Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationRequest));
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
                                                        responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnRemoteStartTransaction

            CPServer.OnRemoteStartTransaction += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnRemoteStartTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnRemoteStartTransactionRequest?.Invoke(requestTimestamp,
                                                            this,
                                                            Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionRequest));
                }

                #endregion


                await Task.Delay(10);


                RemoteStartTransactionResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid RemoteStartTransaction request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new RemoteStartTransactionResponse(Request,
                                                                  RemoteStartStopStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming RemoteStartTransaction for '" + Request.ConnectorId + "' with IdTag '" + Request.IdTag + "'.");

                    // ToDo: lock(connectors)

                    ChargePointConnector connector = null;

                    if (!Request.ConnectorId.HasValue && connectors.Count == 1)
                        connector = connectors.First().Value;

                    else
                        connectors.TryGetValue(Request.ConnectorId.Value, out connector);

                    if (connector != null && connector.IsCharging == false)
                    {

                        connector.IsCharging      = true;
                        connector.StartTimestamp  = Timestamp.Now;

                        EnquedRequests.Add(new EnquedRequest(new StartTransactionRequest(ChargeBoxId,
                                                                                         Request.ConnectorId ?? Connector_Id.Parse(0),
                                                                                         Request.IdTag,
                                                                                         Timestamp.Now,
                                                                                         connector.MeterStartValue,
                                                                                         null), // ReservationId
                                                             Timestamp.Now,
                                                             EnquedRequest.EnquedStatus.New,
                                                             response => {
                                                                 if (response is CS.StartTransactionResponse startTransactionResponse)
                                                                 {
                                                                     connector.IdToken          = Request.IdTag;
                                                                     connector.ChargingProfile  = Request.ChargingProfile;
                                                                     connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
                                                                     connector.TransactionId    = startTransactionResponse.TransactionId;
                                                                 }
                                                             }));

                        response = new RemoteStartTransactionResponse(Request,
                                                                      RemoteStartStopStatus.Accepted);

                    }
                    else
                        response = new RemoteStartTransactionResponse(Request,
                                                                      RemoteStartStopStatus.Rejected);

                }


                #region Send OnRemoteStartTransactionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnRemoteStartTransactionResponse?.Invoke(responseTimestamp,
                                                             this,
                                                             Request,
                                                             response,
                                                             responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnRemoteStopTransaction

            CPServer.OnRemoteStopTransaction += async (LogTimestamp,
                                                       Sender,
                                                       Request,
                                                       CancellationToken) => {

                #region Send OnRemoteStopTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnRemoteStopTransactionRequest?.Invoke(requestTimestamp,
                                                           this,
                                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionRequest));
                }

                #endregion


                await Task.Delay(10);


                RemoteStopTransactionResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid RemoteStopTransaction request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new RemoteStopTransactionResponse(Request,
                                                                 RemoteStartStopStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming RemoteStopTransaction for '" + Request.TransactionId + "'.");

                    // ToDo: lock(connectors)

                    var connector = connectors.Values.Where(conn => conn.TransactionId == Request.TransactionId).FirstOrDefault();

                    if (connector != null && connector.IsCharging == true)
                    {

                        connector.StopTimestamp  = Timestamp.Now;

                        EnquedRequests.Add(new EnquedRequest(new StopTransactionRequest(ChargeBoxId,
                                                                                        Request.TransactionId,
                                                                                        Timestamp.Now,
                                                                                        connector.MeterStopValue,
                                                                                        null,  // IdTag
                                                                                        Reasons.SoftReset,
                                                                                        null), // TransactionData
                                                             Timestamp.Now,
                                                             EnquedRequest.EnquedStatus.New,
                                                             response => {
                                                                 if (response is CS.StopTransactionResponse stopTransactionResponse)
                                                                 {
                                                                     connector.IsCharging = false;
                                                                 }
                                                             }));

                        response = new RemoteStopTransactionResponse(Request,
                                                                     RemoteStartStopStatus.Accepted);

                    }
                    else
                        response = new RemoteStopTransactionResponse(Request,
                                                                     RemoteStartStopStatus.Rejected);

                }


                #region Send OnRemoteStopTransactionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnRemoteStopTransactionResponse?.Invoke(responseTimestamp,
                                                            this,
                                                            Request,
                                                            response,
                                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnSetChargingProfileRequest?.Invoke(requestTimestamp,
                                                        this,
                                                        Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileRequest));
                }

                #endregion


                await Task.Delay(10);


                SetChargingProfileResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid SetChargingProfile request for charge box '" + Request.ChargeBoxId + "'!");

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

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming SetChargingProfile for '" + Request.ConnectorId + "'.");

                    // ToDo: lock(connectors)

                    if (Request.ConnectorId.ToString() == "0")
                    {
                        foreach (var conn in connectors.Values)
                        {

                            if (!Request.ChargingProfile.TransactionId.HasValue)
                                conn.ChargingProfile = Request.ChargingProfile;

                            else if (conn.TransactionId == Request.ChargingProfile.TransactionId.Value)
                                conn.ChargingProfile = Request.ChargingProfile;

                        }
                    }
                    else if (connectors.ContainsKey(Request.ConnectorId))
                    {

                        connectors[Request.ConnectorId].ChargingProfile = Request.ChargingProfile;

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
                                                         responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnClearChargingProfileRequest?.Invoke(requestTimestamp,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileRequest));
                }

                #endregion


                ClearChargingProfileResponse response = null;



                #region Send OnClearChargingProfileResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnClearChargingProfileResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnGetCompositeScheduleRequest?.Invoke(requestTimestamp,
                                           this,
                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleRequest));
                }

                #endregion


                GetCompositeScheduleResponse response = null;



                #region Send OnGetCompositeScheduleResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetCompositeScheduleResponse?.Invoke(responseTimestamp,
                                            this,
                                            Request,
                                            response,
                                            responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnUnlockConnectorRequest?.Invoke(requestTimestamp,
                                                     this,
                                                     Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorRequest));
                }

                #endregion


                await Task.Delay(10);


                UnlockConnectorResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid UnlockConnector request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new UnlockConnectorResponse(Request,
                                                           UnlockStatus.UnlockFailed);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

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
                                                      responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnGetLocalListVersionRequest?.Invoke(requestTimestamp,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionRequest));
                }

                #endregion


                await Task.Delay(10);


                GetLocalListVersionResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid GetLocalListVersion request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new GetLocalListVersionResponse(Request,
                                                               0);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming GetLocalListVersion request.");

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
                                                          responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnSendLocalListRequest?.Invoke(requestTimestamp,
                                                   this,
                                                   Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListRequest));
                }

                #endregion


                await Task.Delay(10);


                SendLocalListResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid SendLocalList request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new SendLocalListResponse(Request,
                                                         UpdateStatus.NotSupported);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.ListVersion + "'.");

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
                                                    responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListResponse));
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

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnClearCacheRequest?.Invoke(requestTimestamp,
                                                this,
                                                Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheRequest));
                }

                #endregion


                await Task.Delay(10);


                ClearCacheResponse response = null;

                if (Request.ChargeBoxId != ChargeBoxId)
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Invalid ClearCache request for charge box '" + Request.ChargeBoxId + "'!");

                    response = new ClearCacheResponse(Request,
                                                      ClearCacheStatus.Rejected);

                }
                else
                {

                    Console.WriteLine("ChargeBox: " + ChargeBoxId + ": Incoming ClearCache request.");

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
                                                 responseTimestamp - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheResponse));
                }

                #endregion

                return response;

            };

            #endregion

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

        #region (Timer) DoSendHeartbeatSync(State)

        private void DoSendHeartbeatSync(Object State)
        {
            if (!DisableSendHeartbeats)
                SendHeartbeat().Wait();
        }

        #endregion


        #region SendBootNotification             (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.BootNotificationResponse>

            SendBootNotification(CancellationToken?  CancellationToken   = null,
                                 EventTracking_Id    EventTrackingId     = null,
                                 TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new BootNotificationRequest(ChargeBoxId,
                                                                ChargePointVendor,
                                                                ChargePointModel,

                                                                ChargePointSerialNumber,
                                                                ChargeBoxSerialNumber,
                                                                FirmwareVersion,
                                                                Iccid,
                                                                IMSI,
                                                                MeterType,
                                                                MeterSerialNumber,

                                                                Request_Id.Random(),
                                                                requestTimestamp,
                                                                EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var response  = await CPClient.SendBootNotification(request,

                                                                requestTimestamp,
                                                                CancellationToken,
                                                                EventTrackingId,
                                                                RequestTimeout ?? DefaultRequestTimeout);

            if (response != null)
            {
                switch (response.Status)
                {

                    case RegistrationStatus.Accepted:
                        this.CentralSystemTime      = response.CurrentTime;
                        this.SendHeartbeatEvery     = response.HeartbeatInterval >= TimeSpan.FromSeconds(5) ? response.HeartbeatInterval : TimeSpan.FromSeconds(5);
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


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response,
                                                   Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                    (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.HeartbeatResponse>

            SendHeartbeat(CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new HeartbeatRequest(ChargeBoxId,

                                                         Request_Id.Random(),
                                                         requestTimestamp,
                                                         EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(requestTimestamp,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var response = await CPClient.SendHeartbeat(request,

                                                        requestTimestamp,
                                                        CancellationToken,
                                                        EventTrackingId,
                                                        RequestTimeout ?? DefaultRequestTimeout);

            if (response != null)
            {
                this.CentralSystemTime = response.CurrentTime;
            }


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                            this,
                                            request,
                                            response,
                                            Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Authorize                        (IdTag, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.AuthorizeResponse>

            Authorize(IdToken             IdTag,

                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new AuthorizeRequest(ChargeBoxId,
                                                         IdTag,

                                                         Request_Id.Random(),
                                                         requestTimestamp,
                                                         EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(requestTimestamp,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var response = await CPClient.Authorize(request,

                                                    requestTimestamp,
                                                    CancellationToken,
                                                    EventTrackingId,
                                                    RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                            this,
                                            request,
                                            response,
                                            Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartTransaction                 (ConnectorId, IdTag, TransactionTimestamp, MeterStart, ReservationId = null, ...)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="TransactionTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StartTransactionResponse>

            StartTransaction(Connector_Id        ConnectorId,
                             IdToken             IdTag,
                             DateTime            TransactionTimestamp,
                             UInt64              MeterStart,
                             Reservation_Id?     ReservationId       = null,

                             CancellationToken?  CancellationToken   = null,
                             EventTracking_Id    EventTrackingId     = null,
                             TimeSpan?           RequestTimeout      = null)

            {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StartTransactionRequest(ChargeBoxId,
                                                                ConnectorId,
                                                                IdTag,
                                                                TransactionTimestamp,
                                                                MeterStart,
                                                                ReservationId,

                                                                Request_Id.Random(),
                                                                requestTimestamp,
                                                                EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            var response = await CPClient.StartTransaction(request,

                                                           requestTimestamp,
                                                           CancellationToken,
                                                           EventTrackingId,
                                                           RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response,
                                                   Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification           (ConnectorId, Status, ErrorCode, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
        /// <param name="VendorErrorCode">A vendor-specific error code.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StatusNotificationResponse>

            SendStatusNotification(Connector_Id           ConnectorId,
                                   ChargePointStatus      Status,
                                   ChargePointErrorCodes  ErrorCode,
                                   String                 Info                = null,
                                   DateTime?              StatusTimestamp     = null,
                                   String                 VendorId            = null,
                                   String                 VendorErrorCode     = null,

                                   CancellationToken?     CancellationToken   = null,
                                   EventTracking_Id       EventTrackingId     = null,
                                   TimeSpan?              RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StatusNotificationRequest(ChargeBoxId,
                                                                  ConnectorId,
                                                                  Status,
                                                                  ErrorCode,
                                                                  Info,
                                                                  StatusTimestamp,
                                                                  VendorId,
                                                                  VendorErrorCode,

                                                                  Request_Id.Random(),
                                                                  requestTimestamp,
                                                                  EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.SendStatusNotification(request,

                                                                 requestTimestamp,
                                                                 CancellationToken,
                                                                 EventTrackingId,
                                                                 RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response,
                                                     Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendMeterValues                  (ConnectorId, TransactionId = null, MeterValues = null, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.MeterValuesResponse>

            SendMeterValues(Connector_Id             ConnectorId,
                            IEnumerable<MeterValue>  MeterValues,
                            Transaction_Id?          TransactionId       = null,

                            CancellationToken?       CancellationToken   = null,
                            EventTracking_Id         EventTrackingId     = null,
                            TimeSpan?                RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new MeterValuesRequest(ChargeBoxId,
                                                           ConnectorId,
                                                           MeterValues,
                                                           TransactionId,

                                                           Request_Id.Random(),
                                                           requestTimestamp,
                                                           EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(requestTimestamp,
                                             this,
                                             request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var response = await CPClient.SendMeterValues(request,

                                                          requestTimestamp,
                                                          CancellationToken,
                                                          EventTrackingId,
                                                          RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                              this,
                                              request,
                                              response,
                                              Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopTransaction                  (TransactionId, TransactionTimestamp, MeterStop, ...)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="TransactionTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StopTransactionResponse>

            StopTransaction(Transaction_Id           TransactionId,
                            DateTime                 TransactionTimestamp,
                            UInt64                   MeterStop,
                            IdToken?                 IdTag               = null,
                            Reasons?                 Reason              = null,
                            IEnumerable<MeterValue>  TransactionData     = null,

                            CancellationToken?       CancellationToken   = null,
                            EventTracking_Id         EventTrackingId     = null,
                            TimeSpan?                RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StopTransactionRequest(ChargeBoxId,
                                                               TransactionId,
                                                               TransactionTimestamp,
                                                               MeterStop,
                                                               IdTag,
                                                               Reason,
                                                               TransactionData,

                                                               Request_Id.Random(),
                                                               requestTimestamp,
                                                               EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(requestTimestamp,
                                                 this,
                                                 request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            var response = await CPClient.StopTransaction(request,

                                                          requestTimestamp,
                                                          CancellationToken,
                                                          EventTrackingId,
                                                          RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                  this,
                                                  request,
                                                  response,
                                                  Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region TransferData                     (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">The charge point model identification.</param>
        /// <param name="Data">The serial number of the charge point.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.DataTransferResponse>

            TransferData(String              VendorId,
                         String              MessageId           = null,
                         String              Data                = null,

                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new DataTransferRequest(ChargeBoxId,
                                                            VendorId,
                                                            MessageId,
                                                            Data,

                                                            Request_Id.Random(),
                                                            requestTimestamp,
                                                            EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(requestTimestamp,
                                              this,
                                              request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response = await CPClient.TransferData(request,

                                                       requestTimestamp,
                                                       CancellationToken,
                                                       EventTrackingId,
                                                       RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(Timestamp.Now,
                                               this,
                                               request,
                                               response,
                                               Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendDiagnosticsStatusNotification(Status, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.DiagnosticsStatusNotificationResponse>

            SendDiagnosticsStatusNotification(DiagnosticsStatus   Status,

                                              CancellationToken?  CancellationToken  = null,
                                              EventTracking_Id    EventTrackingId    = null,
                                              TimeSpan?           RequestTimeout     = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new DiagnosticsStatusNotificationRequest(ChargeBoxId,
                                                                             Status,

                                                                             Request_Id.Random(),
                                                                             requestTimestamp,
                                                                             EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(requestTimestamp,
                                                               this,
                                                               request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.SendDiagnosticsStatusNotification(request,

                                                                            requestTimestamp,
                                                                            CancellationToken,
                                                                            EventTrackingId,
                                                                            RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFirmwareStatusNotification   (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatus      Status,

                                           CancellationToken?  CancellationToken  = null,
                                           EventTracking_Id    EventTrackingId    = null,
                                           TimeSpan?           RequestTimeout     = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new FirmwareStatusNotificationRequest(ChargeBoxId,
                                                                          Status,

                                                                          Request_Id.Random(),
                                                                          requestTimestamp,
                                                                          EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(requestTimestamp,
                                                            this,
                                                            request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.SendFirmwareStatusNotification(request,

                                                                         requestTimestamp,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response,
                                                             Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

    }

}
