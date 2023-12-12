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

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
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

            public Connector_Id      Id                       { get; }

            public Availabilities    Availability             { get; set; }


            public Boolean           IsReserved               { get; set; }

            public Boolean           IsCharging               { get; set; }

            public IdToken           IdToken                  { get; set; }

            public IdTagInfo         IdTagInfo                { get; set; }

            public Transaction_Id    TransactionId            { get; set; }

            public ChargingProfile?  ChargingProfile          { get; set; }


            public DateTime          StartTimestamp           { get; set; }

            public UInt64            MeterStartValue          { get; set; }

            public String?           SignedStartMeterValue    { get; set; }


            public DateTime          StopTimestamp            { get; set; }

            public UInt64            MeterStopValue           { get; set; }

            public String?           SignedStopMeterValue     { get; set; }


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
            public String        Value             { get; set; }

            /// <summary>
            /// This configuration value can not be changed.
            /// </summary>
            public AccessRights  AccessRights      { get; }

            /// <summary>
            /// Changing this configuration value requires a reboot of the charge box to take effect.
            /// </summary>
            public Boolean       RebootRequired    { get; }

            /// <summary>
            /// Create a new configuration value.
            /// </summary>
            /// <param name="Value">The configuration value.</param>
            /// <param name="AccessRights">This configuration value is: read/write, read-only, write-only.</param>
            /// <param name="RebootRequired">Changing this configuration value requires a reboot of the charge box to take effect.</param>
            public ConfigurationData(String        Value,
                                     AccessRights  AccessRights,
                                     Boolean       RebootRequired   = false)
            {

                this.Value           = Value;
                this.AccessRights    = AccessRights;
                this.RebootRequired  = RebootRequired;

            }

        }



        public class EnqueuedRequest
        {

            public enum EnqueuedStatus
            {
                New,
                Processing,
                Finished
            }

            public String          Command           { get; }

            public OCPP.IRequest   Request           { get; }

            public JObject         RequestJSON       { get; }

            public DateTime        EnqueTimestamp    { get; }

            public EnqueuedStatus  Status            { get; set; }

            public Action<Object>  ResponseAction    { get; }

            public EnqueuedRequest(String          Command,
                                   OCPP.IRequest   Request,
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
        /// The client connected to a central system.
        /// </summary>
        public IChargePointClient       CPClient                    { get; private set; }


        public ChargePointSOAPServer    CPServer                    { get; private set; }


        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxId.ToString();



        /// <summary>
        /// The charge box identification.
        /// </summary>
        public OCPP.NetworkingNode_Id   ChargeBoxId            { get; }

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        [Mandatory]
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        [Mandatory]
        public String                   ChargePointModel            { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        [Optional]
        public I18NString?              Description                 { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        [Optional]
        public String?                  ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        [Optional]
        public String?                  ChargeBoxSerialNumber       { get; }

        /// <summary>
        /// The optional firmware version of the charge point.
        /// </summary>
        [Optional]
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The optional ICCID of the charge point's SIM card.
        /// </summary>
        [Optional]
        public String?                  Iccid                       { get; }

        /// <summary>
        /// The optional IMSI of the charge point’s SIM card.
        /// </summary>
        [Optional]
        public String?                  IMSI                        { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String?                  MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String?                  MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String?                  MeterPublicKey              { get; }


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
        /// An event fired whenever a boot notification request will be sent to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction request will be sent to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate?   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate?  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction request will be sent to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate?   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate?  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the central system.
        /// </summary>
        public event OCPP.CS.OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OCPP.CS.OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification request will be sent to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion



        // Incoming messages (from central system)

        #region OnResetRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region ChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region GetConfigurationRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationRequestDelegate?   OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetConfigurationResponseDelegate?  OnGetConfigurationResponse;

        #endregion

        #region ChangeConfigurationRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate?   OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate?  OnChangeConfigurationResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OCPP.CS.OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OCPP.CS.OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion

        #region GetDiagnosticsRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        #endregion

        #region TriggerMessageRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region UpdateFirmwareRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransactionRequest/-Response

        /// <summary>
        /// An event sent whenever a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate?   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate?  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransactionRequest/-Response

        /// <summary>
        /// An event sent whenever a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        #endregion

        #region SetChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region ClearChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnectorRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region GetLocalListVersionRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalListRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCacheRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

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
        public TestChargePoint(NetworkingNode_Id     ChargeBoxId,
                               Byte                  NumberOfConnectors,
                               String                ChargePointVendor,
                               String                ChargePointModel,

                               I18NString?           Description               = null,
                               String?               ChargePointSerialNumber   = null,
                               String?               ChargeBoxSerialNumber     = null,
                               String?               FirmwareVersion           = null,
                               String?               Iccid                     = null,
                               String?               IMSI                      = null,
                               String?               MeterType                 = null,
                               String?               MeterSerialNumber         = null,
                               String?               MeterPublicKey            = null,

                               Boolean               DisableSendHeartbeats     = false,
                               TimeSpan?             SendHeartbeatEvery        = null,

                               Boolean               DisableMaintenanceTasks   = false,
                               TimeSpan?             MaintenanceEvery          = null,

                               TimeSpan?             DefaultRequestTimeout     = null,
                               IHTTPAuthentication?  HTTPAuthentication        = null,
                               DNSClient?            DNSClient                 = null)

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

            this.Configuration = new Dictionary<String, ConfigurationData> {
                { "hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false) },
                { "changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false) },
                { "doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false) },
                { "password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false) }
            };
            this.EnqueuedRequests           = new List<EnqueuedRequest>();

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


        #region InitSOAP(...)

        public async Task InitSOAP(String                               From,
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
                                   HTTPPath?                            URLPathPrefix                = null,
                                   Tuple<String, String>?               WSSLoginPassword             = null,
                                   HTTPContentType?                     HTTPContentType              = null,
                                   TimeSpan?                            RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                   UInt16?                              MaxNumberOfRetries           = null,
                                   UInt32?                              InternalBufferSize           = null,
                                   Boolean                              UseHTTPPipelining            = false,
                                   String?                              LoggingPath                  = null,
                                   String?                              LoggingContext               = null,
                                   LogfileCreatorDelegate?              LogfileCreator               = null,
                                   Boolean?                             DisableLogging               = false,
                                   HTTPClientLogger?                    HTTPLogger                   = null,

                                   String?                              HTTPServerName               = null,
                                   IPPort?                              TCPPort                      = null,
                                   String?                              ServiceName                  = null,
                                   HTTPPath?                            URLPrefix                    = null,
                                   HTTPContentType?                     ContentType                  = null,
                                   Boolean                              RegisterHTTPRootService      = true,
                                   DNSClient?                           DNSClient                    = null,
                                   Boolean                              AutoStart                    = false)

        {

            this.CPClient = new ChargePointSOAPClient(
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
                                URLPathPrefix,
                                WSSLoginPassword,
                                HTTPContentType,
                                RequestTimeout,
                                TransmissionRetryDelay,
                                MaxNumberOfRetries,
                                InternalBufferSize,
                                UseHTTPPipelining,
                                LoggingPath,
                                LoggingContext,
                                LogfileCreator,
                                DisableLogging,
                                HTTPLogger,
                                DNSClient ?? this.DNSClient
                            );

            this.CPServer = new ChargePointSOAPServer(
                                HTTPServerName,
                                TCPPort,
                                ServiceName,
                                URLPrefix,
                                ContentType,
                                RegisterHTTPRootService,
                                DNSClient ?? this.DNSClient,
                                AutoStart
                            );

            WireEvents(CPServer);

        }

        #endregion

        #region ConnectWebSocket(...)

        public async Task<HTTPResponse?> ConnectWebSocket(String                               From,
                                                          String                               To,

                                                          URL                                  RemoteURL,
                                                          HTTPHostname?                        VirtualHostname              = null,
                                                          String?                              Description                  = null,
                                                          Boolean?                             PreferIPv4                   = null,
                                                          RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                          X509Certificate?                     ClientCert                   = null,
                                                          SslProtocols?                        TLSProtocol                  = null,
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

            var WSClient   = new ChargePointWSClient(
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

            this.CPClient  = WSClient;

            WireEvents(WSClient);

            var response = await WSClient.Connect();

            return response;

        }

        #endregion

        #region WireEvents(CPServer)

        public void WireEvents(IChargePointServer CPServer)
        {

            #region OnReset

            CPServer.OnReset += async (LogTimestamp,
                                       Sender,
                                       connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetRequest));
                }

                #endregion


                await Task.Delay(10);


                ResetResponse? response = null;


                DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming '", Request.ResetType, "' reset request accepted."));
                response = new ResetResponse(Request,
                                             ResetStatus.Accepted);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnChangeAvailability

            CPServer.OnChangeAvailability += async (LogTimestamp,
                                                    Sender,
                                                    connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityRequest));
                }

                #endregion


                await Task.Delay(10);


                ChangeAvailabilityResponse? response = null;


                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming ChangeAvailability '", Request.Availability, "' request for connector '", Request.ConnectorId, "'."));

                    if (connectors.ContainsKey(Request.ConnectorId))
                    {

                        connectors[Request.ConnectorId].Availability = Request.Availability;

                        response = new ChangeAvailabilityResponse(Request,
                                                                  AvailabilityStatus.Accepted);

                    }
                    else
                        response = new ChangeAvailabilityResponse(Request,
                                                                  AvailabilityStatus.Rejected);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetConfiguration

            CPServer.OnGetConfiguration += async (LogTimestamp,
                                                  Sender,
                                                  connection,
                                                  Request,
                                                  CancellationToken) => {

                #region Send OnGetConfigurationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetConfigurationRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationRequest));
                }

                #endregion


                await Task.Delay(10);


                GetConfigurationResponse? response = null;

                

                DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming get configuration request."));

                var configurationKeys  = new List<ConfigurationKey>();
                var unkownKeys         = new List<String>();

                if (Request.Keys.Any())
                {
                    foreach (var key in Request.Keys)
                    {

                        if (Configuration.TryGetValue(key, out var configurationData))
                            configurationKeys.Add(new ConfigurationKey(key,
                                                                       configurationData.AccessRights,
                                                                       configurationData.Value));

                        else
                            unkownKeys.Add(key);

                    }
                }
                else
                {
                    foreach (var configuration in Configuration)
                    {
                        configurationKeys.Add(new ConfigurationKey(configuration.Key,
                                                                   configuration.Value.AccessRights,
                                                                   configuration.Value.Value));
                    }
                }


                response = new GetConfigurationResponse(Request,
                                                        configurationKeys,
                                                        unkownKeys);

                


                #region Send OnGetConfigurationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetConfigurationResponse?.Invoke(responseTimestamp,
                                                       this,
                                                       Request,
                                                       response,
                                                       responseTimestamp - startTime);

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
                                                     connection,
                                                     Request,
                                                     CancellationToken) => {

                #region Send OnChangeConfigurationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnChangeConfigurationRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationRequest));
                }

                #endregion


                await Task.Delay(10);


                ChangeConfigurationResponse? response = null;



                DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming change configuration for '", Request.Key, "' with value '", Request.Value, "'."));

                if (Configuration.TryGetValue(Request.Key, out var configurationData))
                {
                    if (configurationData.AccessRights == AccessRights.ReadOnly)
                    {

                        response                 = new ChangeConfigurationResponse(Request,
                                                                                   ConfigurationStatus.Rejected);

                    }
                    else
                    {

                        configurationData.Value  = Request.Value;

                        response                 = new ChangeConfigurationResponse(Request,
                                                                                   configurationData.RebootRequired
                                                                                       ? ConfigurationStatus.RebootRequired
                                                                                       : ConfigurationStatus.Accepted);

                    }
                }
                else
                {

                    Configuration.Add(Request.Key,
                                      new ConfigurationData(Request.Value,
                                                            AccessRights.ReadWrite,
                                                            false));

                    response  = new ChangeConfigurationResponse(Request,
                                                                ConfigurationStatus.Accepted);

                }



                #region Send OnChangeConfigurationResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnChangeConfigurationResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          Request,
                                                          response,
                                                          responseTimestamp - startTime);

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
                                                      connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
                }

                #endregion


                await Task.Delay(10);


                OCPP.CS.DataTransferResponse? response = null;


                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming data transfer request: ", Request.VendorId, ".", Request.MessageId?.ToString() ?? "-", ": ", Request.Data ?? "-"));

                    if (Request.VendorId.  ToString().ToLower() == "graphdefined" &&
                        Request.MessageId?.ToString().ToLower() == "hello"        &&
                        Request.Data?.ToString()?.    ToLower() == "world!")
                    {
                        response = new OCPP.CS.DataTransferResponse(Request,
                                                            OCPP.DataTransferStatus.Accepted,
                                                            "Hello World!");
                    }
                    else
                        response = new OCPP.CS.DataTransferResponse(Request,
                                                            OCPP.DataTransferStatus.Rejected);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetDiagnostics

            CPServer.OnGetDiagnostics += async (LogTimestamp,
                                                Sender,
                                                connection,
                                                Request,
                                                CancellationToken) => {

                #region Send OnGetDiagnosticsRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetDiagnosticsRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsRequest));
                }

                #endregion


                await Task.Delay(10);


                GetDiagnosticsResponse? response = null;


                    DebugX.Log(String.Concat("ChargeBox[", ChargeBoxId, "] Incoming get diagnostics request"));

                    response = new GetDiagnosticsResponse(Request);


                #region Send OnGetDiagnosticsResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnGetDiagnosticsResponse?.Invoke(responseTimestamp,
                                                     this,
                                                     Request,
                                                     response,
                                                     responseTimestamp - startTime);

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
                                                connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageRequest));
                }

                #endregion


                await Task.Delay(10);


                TriggerMessageResponse? response = null;

                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at connector '" + Request.ConnectorId + "'.");

                response = new TriggerMessageResponse(Request,
                                                      TriggerMessageStatus.Rejected);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnUpdateFirmware

            CPServer.OnUpdateFirmware += async (LogTimestamp,
                                                Sender,
                                                connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareRequest));
                }

                #endregion


                await Task.Delay(10);


                UpdateFirmwareResponse? response = null;


                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UpdateFirmware request for '" + Request.FirmwareURL + "'.");

                response = new UpdateFirmwareResponse(Request);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnReserveNow

            CPServer.OnReserveNow += async (LogTimestamp,
                                            Sender,
                                            connection,
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
                                                 responseTimestamp - startTime);

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
                                                   connection,
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
                                                        responseTimestamp - startTime);

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
                                                        connection,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnRemoteStartTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnRemoteStartTransactionRequest?.Invoke(startTime,
                                                            this,
                                                            Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionRequest));
                }

                #endregion


                await Task.Delay(10);


                RemoteStartTransactionResponse? response = null;


                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming RemoteStartTransaction for '" + Request.ConnectorId + "' with IdTag '" + Request.IdTag + "'.");

                    // ToDo: lock(connectors)

                    ChargePointConnector connector = null;

                    if (!Request.ConnectorId.HasValue && connectors.Count == 1)
                        connector = connectors.First().Value;

                    else
                        connectors.TryGetValue(Request.ConnectorId.Value, out connector);

                    if (connector != null && connector.IsCharging == false)
                    {

                        connector.IsCharging         = true;
                        connector.StartTimestamp     = Timestamp.Now;

                        var startTransactionRequest  = new StartTransactionRequest(ChargeBoxId,
                                                                                   Request.ConnectorId ?? Connector_Id.Parse(0),
                                                                                   Request.IdTag,
                                                                                   Timestamp.Now,
                                                                                   connector.MeterStartValue,
                                                                                   null); // ReservationId

                        EnqueuedRequests.Add(new EnqueuedRequest("StartTransaction",
                                                             startTransactionRequest,
                                                             startTransactionRequest.ToJSON(),
                                                             Timestamp.Now,
                                                             EnqueuedRequest.EnqueuedStatus.New,
                                                             response => {
                                                                 if (response is OCPP.WebSockets.OCPP_JSONResponseMessage wsResponseMessage &&
                                                                     CS.StartTransactionResponse.TryParse(startTransactionRequest,
                                                                                                          wsResponseMessage.Payload,
                                                                                                          out var startTransactionResponse,
                                                                                                          out var ErrorResponse))
                                                                 {


                                                                     connector.IdToken          = Request.IdTag;
                                                                     connector.ChargingProfile  = Request.ChargingProfile;
                                                                     connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
                                                                     connector.TransactionId    = startTransactionResponse.TransactionId;

                                                                     DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging... " + startTransactionResponse.TransactionId);

                                                                 }
                                                             }));

                        // ToDo: StartTransaction request might fail!
                        var statusNotificationRequest  = new StatusNotificationRequest(ChargeBoxId,
                                                                                       Request.ConnectorId ?? Connector_Id.Parse(0),
                                                                                       ChargePointStatus.Charging,
                                                                                       ChargePointErrorCodes.NoError);

                        EnqueuedRequests.Add(new EnqueuedRequest("StatusNotification",
                                                             statusNotificationRequest,
                                                             statusNotificationRequest.ToJSON(),
                                                             Timestamp.Now,
                                                             EnqueuedRequest.EnqueuedStatus.New,
                                                             response => {
                                                                 //if (response is WebSockets.WSResponseMessage wsResponseMessage &&
                                                                 //    CS.StartTransactionResponse.TryParse(startTransactionRequest,
                                                                 //                                         wsResponseMessage.Message,
                                                                 //                                         out CS.StartTransactionResponse  startTransactionResponse,
                                                                 //                                         out String                       ErrorResponse))
                                                                 //{
                                                                 //    connector.IdToken          = Request.IdTag;
                                                                 //    connector.ChargingProfile  = Request.ChargingProfile;
                                                                 //    connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
                                                                 //    connector.TransactionId    = startTransactionResponse.TransactionId;
                                                                 //    DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging...");
                                                                 //}
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
                                                             responseTimestamp - startTime);

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
                                                       connection,
                                                       Request,
                                                       CancellationToken) => {

                #region Send OnRemoteStopTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnRemoteStopTransactionRequest?.Invoke(startTime,
                                                           this,
                                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionRequest));
                }

                #endregion


                await Task.Delay(10);


                RemoteStopTransactionResponse? response = null;


                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming RemoteStopTransaction for '" + Request.TransactionId + "'.");

                // ToDo: lock(connectors)

                var connector = connectors.Values.Where(conn => conn.IsCharging && conn.TransactionId == Request.TransactionId).FirstOrDefault();

                if (connector != null)
                {

                    connector.StopTimestamp  = Timestamp.Now;
                    connector.IsCharging     = false;

                    var stopTransactionRequest = new StopTransactionRequest(ChargeBoxId,
                                                                            Request.TransactionId,
                                                                            Timestamp.Now,
                                                                            connector.MeterStopValue,
                                                                            null,  // IdTag
                                                                            Reasons.Remote,
                                                                            null);

                    EnqueuedRequests.Add(new EnqueuedRequest("StopTransaction",
                                                         stopTransactionRequest, // TransactionData
                                                         stopTransactionRequest.ToJSON(),
                                                         Timestamp.Now,
                                                         EnqueuedRequest.EnqueuedStatus.New,
                                                         response => {
                                                             if (response is OCPP.WebSockets.OCPP_JSONResponseMessage wsResponseMessage &&
                                                                 CS.StopTransactionResponse.TryParse(stopTransactionRequest,
                                                                                                     wsResponseMessage.Payload,
                                                                                                     out var stopTransactionResponse,
                                                                                                     out var ErrorResponse))
                                                             {
                                                                 DebugX.Log(nameof(TestChargePoint), "Connector " + connector.Id + " stopped charging...");
                                                             }
                                                         }));


                    // ToDo: StopTransaction request might fail!
                    var statusNotificationRequest  = new StatusNotificationRequest(ChargeBoxId,
                                                                                   connector.Id,
                                                                                   ChargePointStatus.Available,
                                                                                   ChargePointErrorCodes.NoError);

                    EnqueuedRequests.Add(new EnqueuedRequest("StatusNotification",
                                                         statusNotificationRequest,
                                                         statusNotificationRequest.ToJSON(),
                                                         Timestamp.Now,
                                                         EnqueuedRequest.EnqueuedStatus.New,
                                                         response => {
                                                             //if (response is WebSockets.WSResponseMessage wsResponseMessage &&
                                                             //    CS.StartTransactionResponse.TryParse(startTransactionRequest,
                                                             //                                         wsResponseMessage.Message,
                                                             //                                         out CS.StartTransactionResponse  startTransactionResponse,
                                                             //                                         out String                       ErrorResponse))
                                                             //{
                                                             //    connector.IdToken          = Request.IdTag;
                                                             //    connector.ChargingProfile  = Request.ChargingProfile;
                                                             //    connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
                                                             //    connector.TransactionId    = startTransactionResponse.TransactionId;
                                                             //    DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging...");
                                                             //}
                                                         }));


                    response = new RemoteStopTransactionResponse(Request,
                                                                 RemoteStartStopStatus.Accepted);

                }
                else
                    response = new RemoteStopTransactionResponse(Request,
                                                                 RemoteStartStopStatus.Rejected);


                #region Send OnRemoteStopTransactionResponse event

                try
                {

                    var responseTimestamp = Timestamp.Now;

                    OnRemoteStopTransactionResponse?.Invoke(responseTimestamp,
                                                            this,
                                                            Request,
                                                            response,
                                                            responseTimestamp - startTime);

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
                                                    connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileRequest));
                }

                #endregion


                await Task.Delay(10);


                SetChargingProfileResponse? response = null;

                if (Request.ChargingProfile is null)
                {

                    response = new SetChargingProfileResponse(Request,
                                                              ChargingProfileStatus.Rejected);

                }
                else
                {

                    DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SetChargingProfile for '" + Request.ConnectorId + "'.");

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
                                                         responseTimestamp - startTime);

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
                                                      connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileRequest));
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnGetCompositeSchedule

            CPServer.OnGetCompositeSchedule += async (LogTimestamp,
                                                      Sender,
                                                      connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleRequest));
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnUnlockConnector

            CPServer.OnUnlockConnector += async (LogTimestamp,
                                                 Sender,
                                                 connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorRequest));
                }

                #endregion


                await Task.Delay(10);


                UnlockConnectorResponse? response = null;


                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnGetLocalListVersion

            CPServer.OnGetLocalListVersion += async (LogTimestamp,
                                                     Sender,
                                                     connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionRequest));
                }

                #endregion


                await Task.Delay(10);


                GetLocalListVersionResponse? response = null;



                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming GetLocalListVersion request.");

                response = new GetLocalListVersionResponse(Request,
                                                           0);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSendLocalList

            CPServer.OnSendLocalList += async (LogTimestamp,
                                               Sender,
                                               connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListRequest));
                }

                #endregion


                await Task.Delay(10);


                SendLocalListResponse? response = null;


                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.ListVersion + "'.");

                response = new SendLocalListResponse(Request,
                                                     UpdateStatus.NotSupported);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnClearCache

            CPServer.OnClearCache += async (LogTimestamp,
                                            Sender,
                                            connection,
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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheRequest));
                }

                #endregion


                await Task.Delay(10);


                ClearCacheResponse? response = null;


                DebugX.Log($"ChargeBox[{ChargeBoxId}] Incoming ClearCache request.");

                response = new ClearCacheResponse(Request,
                                                  ClearCacheStatus.Rejected);


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
                    DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheResponse));
                }

                #endregion

                return response;

            };

            #endregion


            //ToDo: Add security extensions

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
                if (CPClient is ChargePointWSClient wsClient)
                {

                    //var response = await wsClient.SendRequest(
                    //                         enquedRequest.Command,
                    //                         enquedRequest.Request.RequestId,
                    //                         enquedRequest.RequestJSON
                    //                     );

                    //enquedRequest.ResponseAction(response);

                    //EnqueuedRequests.Remove(enquedRequest);

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

        private OCPP.Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return OCPP.Request_Id.Parse(internalRequestId.ToString());

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
        public async Task<CS.BootNotificationResponse>

            SendBootNotification(DateTime?          RequestTimestamp    = null,
                                 TimeSpan?          RequestTimeout      = null,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new BootNotificationRequest(
                                 ChargeBoxId,
                                 ChargePointVendor,
                                 ChargePointModel,

                                 ChargePointSerialNumber,
                                 ChargeBoxSerialNumber,
                                 FirmwareVersion,
                                 Iccid,
                                 IMSI,
                                 MeterType,
                                 MeterSerialNumber,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            CS.BootNotificationResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.BootNotification(request);

            if (response is not null)
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

            response ??= new CS.BootNotificationResponse(request,
                                                         OCPP.Result.Server("Response is null!"));


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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                    (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.HeartbeatResponse>

            SendHeartbeat(DateTime?          RequestTimestamp    = null,
                          TimeSpan?          RequestTimeout      = null,
                          EventTracking_Id?  EventTrackingId     = null,
                          CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new HeartbeatRequest(
                                 ChargeBoxId,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            CS.HeartbeatResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.Heartbeat(request);

            if (response is not null)
            {
                this.CentralSystemTime = response.CurrentTime;
            }

            response ??= new CS.HeartbeatResponse(request,
                                                  OCPP.Result.Server("Response is null!"));


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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.AuthorizeResponse>

            Authorize(IdToken            IdTag,

                      DateTime?          RequestTimestamp    = null,
                      TimeSpan?          RequestTimeout      = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new AuthorizeRequest(
                                 ChargeBoxId,
                                 IdTag,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            CS.AuthorizeResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.Authorize(request);

            response ??= new CS.AuthorizeResponse(request,
                                                  OCPP.Result.Server("Response is null!"));


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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartTransaction                 (ConnectorId, IdTag, StartTimestamp, MeterStart, ReservationId = null, ...)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.StartTransactionResponse>

            StartTransaction(Connector_Id       ConnectorId,
                             IdToken            IdTag,
                             DateTime           StartTimestamp,
                             UInt64             MeterStart,
                             Reservation_Id?    ReservationId       = null,

                             DateTime?          RequestTimestamp    = null,
                             TimeSpan?          RequestTimeout      = null,
                             EventTracking_Id?  EventTrackingId     = null,
                             CancellationToken  CancellationToken   = default)

            {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new StartTransactionRequest(
                                 ChargeBoxId,
                                 ConnectorId,
                                 IdTag,
                                 StartTimestamp,
                                 MeterStart,
                                 ReservationId,

                                 RequestId: NextRequestId
                             );

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(startTime,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            CS.StartTransactionResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.StartTransaction(request);

            if (response is not null)
            {

                if (connectors.TryGetValue(ConnectorId, out var connector))
                {

                    connector.IsCharging     = true;
                    connector.IdToken        = IdTag;
                    connector.IdTagInfo      = response.IdTagInfo;
                    connector.TransactionId  = response.TransactionId;

                    DebugX.Log(nameof(TestChargePoint), "Connector " + ConnectorId + " started (local) charging with transaction identification " + response.TransactionId + "...");

                }
                else
                    DebugX.Log(nameof(TestChargePoint), "Unkown connector " + ConnectorId + "!");

            }

            response ??= new CS.StartTransactionResponse(request,
                                                         OCPP.Result.Server("Response is null!"));


            #region Send OnStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStartTransactionResponse?.Invoke(endTime,
                                                   this,
                                                   request,
                                                   response,
                                                   endTime - startTime);

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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.StatusNotificationResponse>

            SendStatusNotification(Connector_Id           ConnectorId,
                                   ChargePointStatus      Status,
                                   ChargePointErrorCodes  ErrorCode,
                                   String?                Info                = null,
                                   DateTime?              StatusTimestamp     = null,
                                   String?                VendorId            = null,
                                   String?                VendorErrorCode     = null,

                                   DateTime?              RequestTimestamp    = null,
                                   TimeSpan?              RequestTimeout      = null,
                                   EventTracking_Id?      EventTrackingId     = null,
                                   CancellationToken      CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new StatusNotificationRequest(
                                 ChargeBoxId,
                                 ConnectorId,
                                 Status,
                                 ErrorCode,
                                 Info,
                                 StatusTimestamp,
                                 VendorId,
                                 VendorErrorCode,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            CS.StatusNotificationResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.StatusNotification(request);

            response ??= new CS.StatusNotificationResponse(request,
                                                           OCPP.Result.Server("Response is null!"));


            #region Send OnStatusNotificationResponse event

            var endTime = Timestamp.Now;

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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.MeterValuesResponse>

            SendMeterValues(Connector_Id             ConnectorId,
                            IEnumerable<MeterValue>  MeterValues,
                            Transaction_Id?          TransactionId       = null,

                            DateTime?                RequestTimestamp    = null,
                            TimeSpan?                RequestTimeout      = null,
                            EventTracking_Id?        EventTrackingId     = null,
                            CancellationToken        CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new MeterValuesRequest(
                                 ChargeBoxId,
                                 ConnectorId,
                                 MeterValues,
                                 TransactionId,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            CS.MeterValuesResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.MeterValues(request);

            response ??= new CS.MeterValuesResponse(request,
                                                    OCPP.Result.Server("Response is null!"));


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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopTransaction                  (TransactionId, StopTimestamp, MeterStop, ...)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// 
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.StopTransactionResponse>

            StopTransaction(Transaction_Id            TransactionId,
                            DateTime                  StopTimestamp,
                            UInt64                    MeterStop,
                            IdToken?                  IdTag               = null,
                            Reasons?                  Reason              = null,
                            IEnumerable<MeterValue>?  TransactionData     = null,

                            DateTime?                 RequestTimestamp    = null,
                            TimeSpan?                 RequestTimeout      = null,
                            EventTracking_Id?         EventTrackingId     = null,
                            CancellationToken         CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new StopTransactionRequest(
                                 ChargeBoxId,
                                 TransactionId,
                                 StopTimestamp,
                                 MeterStop,
                                 IdTag,
                                 Reason,
                                 TransactionData,

                                 RequestId: NextRequestId
                             );

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(startTime,
                                                 this,
                                                 request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            CS.StopTransactionResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.StopTransaction(request);

            response ??= new CS.StopTransactionResponse(request,
                                                        OCPP.Result.Server("Response is null!"));


            #region Send OnStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStopTransactionResponse?.Invoke(endTime,
                                                  this,
                                                  request,
                                                  response,
                                                  endTime - startTime);

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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPP.CSMS.DataTransferResponse>

            TransferData(Vendor_Id          VendorId,
                         Message_Id?        MessageId           = null,
                         String?            Data                = null,

                         DateTime?          RequestTimestamp    = null,
                         TimeSpan?          RequestTimeout      = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new OCPP.CS.DataTransferRequest(
                                 ChargeBoxId,
                                 VendorId,
                                 MessageId,
                                 Data,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            OCPP.CSMS.DataTransferResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.DataTransfer(request);

            response ??= new OCPP.CSMS.DataTransferResponse(request,
                                                     OCPP.Result.Server("Response is null!"));


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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.DiagnosticsStatusNotificationResponse>

            SendDiagnosticsStatusNotification(DiagnosticsStatus  Status,

                                              DateTime?          RequestTimestamp    = null,
                                              TimeSpan?          RequestTimeout      = null,
                                              EventTracking_Id?  EventTrackingId     = null,
                                              CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new DiagnosticsStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,

                                 RequestId: NextRequestId
                             );

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
                                                               this,
                                                               request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            CS.DiagnosticsStatusNotificationResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.DiagnosticsStatusNotification(request);

            response ??= new CS.DiagnosticsStatusNotificationResponse(request,
                                                                      OCPP.Result.Server("Response is null!"));


            #region Send OnDiagnosticsStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(endTime,
                                                                this,
                                                                request,
                                                                response,
                                                                endTime - startTime);

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
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<CS.FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatus     Status,

                                           DateTime?          RequestTimestamp    = null,
                                           TimeSpan?          RequestTimeout      = null,
                                           EventTracking_Id?  EventTrackingId     = null,
                                           CancellationToken  CancellationToken   = default)

        {

            #region Create request

            var startTime  = Timestamp.Now;

            var request    = new FirmwareStatusNotificationRequest(
                                 ChargeBoxId,
                                 Status,

                                 RequestId: NextRequestId
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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            CS.FirmwareStatusNotificationResponse? response = null;

            if (CPClient is not null)
                response = await CPClient.FirmwareStatusNotification(request);

            response ??= new CS.FirmwareStatusNotificationResponse(request,
                                                                   OCPP.Result.Server("Response is null!"));


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
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        //ToDo: Add security extensions


    }

}
