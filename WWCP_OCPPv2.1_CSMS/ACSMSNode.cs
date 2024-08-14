/*
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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using BCx509 = Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using System.Net.Sockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Properties

        public Connector_Id    Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region ChargingStationConnector(Id, ConnectorType)

        public ChargingStationConnector(Connector_Id    Id,
                                        ConnectorType  ConnectorType)
        {

            this.Id             = Id;
            this.ConnectorType  = ConnectorType;

        }

        #endregion

    }

    #endregion

    #region (class) ChargingStationEVSE

    /// <summary>
    /// An EVSE at a charging station.
    /// </summary>
    public class ChargingStationEVSE
    {

        #region Properties

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


        public ChargingTariff?    DefaultChargingTariff    { get; set; }

        #endregion

        #region ChargingStationEVSE(Id, AdminStatus, ...)

        public ChargingStationEVSE(EVSE_Id                                 Id,
                                   OperationalStatus                       AdminStatus,
                                   String?                                 MeterType           = null,
                                   String?                                 MeterSerialNumber   = null,
                                   String?                                 MeterPublicKey      = null,
                                   IEnumerable<ChargingStationConnector>?  Connectors          = null)
        {

            this.Id                 = Id;
            this.AdminStatus        = AdminStatus;
            this.MeterType          = MeterType;
            this.MeterSerialNumber  = MeterSerialNumber;
            this.MeterPublicKey     = MeterPublicKey;
            this.connectors         = Connectors is not null && Connectors.Any()
                                          ? new HashSet<ChargingStationConnector>(Connectors)
                                          : new HashSet<ChargingStationConnector>();

        }

        #endregion


        #region Connectors

        private readonly HashSet<ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }

        #endregion


    }

    #endregion


    /// <summary>
    /// An abstract Charging Station Management System node.
    /// </summary>
    public abstract class ACSMSNode : ANetworkingNode
    {

        #region Data

        private          readonly  HashSet<SignaturePolicy>                                                      signaturePolicies            = [];

        //private          readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<CSMS.ICSMSChannel, DateTime>>   connectedNetworkingNodes     = [];

        protected static readonly  SemaphoreSlim                                                                 ChargingStationSemaphore     = new (1, 1);

        private          readonly  TimeSpan                                                                      defaultRequestTimeout        = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The Charging Station Management System vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                        { get; }      = "";

        /// <summary>
        ///  The Charging Station Management System model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                             { get; }      = "";

        /// <summary>
        /// The optional serial number of the Charging Station Management System.
        /// </summary>
        [Optional]
        public String?                     SerialNumber                      { get; }

        /// <summary>
        /// The optional firmware version of the Charging Station Management System.
        /// </summary>
        [Optional]
        public String?                     SoftwareVersion                   { get; }


        public AsymmetricCipherKeyPair?    ClientCAKeyPair                   { get; }
        public BCx509.X509Certificate?     ClientCACertificate               { get; }



        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                   CSMSTime                          { get; set; } = Timestamp.Now;


        public HTTPAPI?                    HTTPAPI                           { get; }

        public DownloadAPI?                HTTPDownloadAPI                   { get; }
        public HTTPPath?                   HTTPDownloadAPI_Path              { get; }
        public String?                     HTTPDownloadAPI_FileSystemPath    { get; }

        public UploadAPI?                  HTTPUploadAPI                     { get; }
        public HTTPPath?                   HTTPUploadAPI_Path                { get; }
        public String?                     HTTPUploadAPI_FileSystemPath      { get; }

        public QRCodeAPI?                  QRCodeAPI                         { get; }
        public HTTPPath?                   QRCodeAPI_Path                    { get; }

        public WebAPI?                     WebAPI                            { get; }
        public HTTPPath?                   WebAPI_Path                       { get; }

        #endregion

        #region Events

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
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

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
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract Charging Station Management System (CSMS).
        /// </summary>
        /// <param name="Id">The unique identification of this Charging Station Management System (CSMS).</param>
        /// <param name="Description">An optional multi-language description of the Charging Station Management System (CSMS).</param>
        public ACSMSNode(NetworkingNode_Id         Id,
                         String                    VendorName,
                         String                    Model,
                         String?                   SerialNumber                     = null,
                         String?                   SoftwareVersion                  = null,
                         I18NString?               Description                      = null,
                         CustomData?               CustomData                       = null,

                         AsymmetricCipherKeyPair?  ClientCAKeyPair                  = null,
                         BCx509.X509Certificate?   ClientCACertificate              = null,

                         SignaturePolicy?          SignaturePolicy                  = null,
                         SignaturePolicy?          ForwardingSignaturePolicy        = null,

                         HTTPAPI?                  HTTPAPI                          = null,
                         Boolean                   HTTPAPI_Disabled                 = false,
                         IPPort?                   HTTPAPI_Port                     = null,
                         String?                   HTTPAPI_ServerName               = null,
                         String?                   HTTPAPI_ServiceName              = null,
                         EMailAddress?             HTTPAPI_RobotEMailAddress        = null,
                         String?                   HTTPAPI_RobotGPGPassphrase       = null,
                         Boolean                   HTTPAPI_EventLoggingDisabled     = false,

                         DownloadAPI?              HTTPDownloadAPI                  = null,
                         Boolean                   HTTPDownloadAPI_Disabled         = false,
                         HTTPPath?                 HTTPDownloadAPI_Path             = null,
                         String?                   HTTPDownloadAPI_FileSystemPath   = null,

                         UploadAPI?                HTTPUploadAPI                    = null,
                         Boolean                   HTTPUploadAPI_Disabled           = false,
                         HTTPPath?                 HTTPUploadAPI_Path               = null,
                         String?                   HTTPUploadAPI_FileSystemPath     = null,

                         //HTTPPath?                 FirmwareDownloadAPIPath          = null,
                         //HTTPPath?                 LogfilesUploadAPIPath            = null,
                         //HTTPPath?                 DiagnosticsUploadAPIPath         = null,

                         QRCodeAPI?                QRCodeAPI                        = null,
                         Boolean                   QRCodeAPI_Disabled               = false,
                         HTTPPath?                 QRCodeAPI_Path                   = null,

                         WebAPI?                   WebAPI                           = null,
                         Boolean                   WebAPI_Disabled                  = false,
                         HTTPPath?                 WebAPI_Path                      = null,

                         TimeSpan?                 DefaultRequestTimeout            = null,

                         Boolean                   DisableSendHeartbeats            = false,
                         TimeSpan?                 SendHeartbeatsEvery              = null,

                         Boolean                   DisableMaintenanceTasks          = false,
                         TimeSpan?                 MaintenanceEvery                 = null,

                         ISMTPClient?              SMTPClient                       = null,
                         DNSClient?                DNSClient                        = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   !HTTPAPI_Disabled
                       ? new HTTPExtAPI(
                             HTTPServerPort:         HTTPAPI_Port               ?? IPPort.Auto,
                             HTTPServerName:         HTTPAPI_ServerName         ?? "GraphDefined OCPP Test Central System",
                             HTTPServiceName:        HTTPAPI_ServiceName        ?? "GraphDefined OCPP Test Central System Service",
                             APIRobotEMailAddress:   HTTPAPI_RobotEMailAddress  ?? EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                             APIRobotGPGPassphrase:  HTTPAPI_RobotGPGPassphrase ?? "test123",
                             SMTPClient:             SMTPClient                 ?? new NullMailer(),
                             DNSClient:              DNSClient,
                             AutoStart:              true
                         )
                       : null,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DefaultRequestTimeout ?? TimeSpan.FromMinutes(1),

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   DNSClient)

        {

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given vendor name must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given model must not be null or empty!");

            this.VendorName                      = VendorName;
            this.Model                           = Model;
            this.SerialNumber                    = SerialNumber;
            this.SoftwareVersion                 = SoftwareVersion;

            this.ClientCAKeyPair                 = ClientCAKeyPair;
            this.ClientCACertificate             = ClientCACertificate;

            OCPP.IN.AnycastIds.Add(NetworkingNode_Id.CSMS);

            #region Setup Web-/Upload-/DownloadAPIs

            this.HTTPUploadAPI_Path              = HTTPUploadAPI_Path             ?? HTTPPath.Parse("uploads");
            this.HTTPDownloadAPI_Path            = HTTPDownloadAPI_Path           ?? HTTPPath.Parse("downloads");
            this.QRCodeAPI_Path                  = QRCodeAPI_Path                 ?? HTTPPath.Parse("qr");
            this.WebAPI_Path                     = WebAPI_Path                    ?? HTTPPath.Parse("webapi");

            this.HTTPUploadAPI_FileSystemPath    = HTTPUploadAPI_FileSystemPath   ?? Path.Combine(AppContext.BaseDirectory, "UploadAPI");
            this.HTTPDownloadAPI_FileSystemPath  = HTTPDownloadAPI_FileSystemPath ?? Path.Combine(AppContext.BaseDirectory, "DownloadAPI");

            if (HTTPExtAPI is not null)
            {

                this.HTTPAPI                     = HTTPAPI ?? new HTTPAPI(
                                                                  this,
                                                                  HTTPExtAPI,
                                                                  EventLoggingDisabled: HTTPAPI_EventLoggingDisabled
                                                              );

                #region HTTP API Security Settings

                this.HTTPExtAPI.HTTPServer.AddAuth(request => {

                    // Allow some URLs for anonymous access...
                    if (request.Path.StartsWith(HTTPExtAPI.URLPathPrefix + this.HTTPUploadAPI_Path)   ||
                        request.Path.StartsWith(HTTPExtAPI.URLPathPrefix + this.HTTPDownloadAPI_Path) ||
                        request.Path.StartsWith(HTTPExtAPI.URLPathPrefix + this.WebAPI_Path))
                    {
                        return HTTPExtAPI.Anonymous;
                    }

                    return null;

                });

                #endregion


                if (!HTTPUploadAPI_Disabled)
                {

                    Directory.CreateDirectory(this.HTTPUploadAPI_FileSystemPath);
                    this.HTTPUploadAPI              = HTTPUploadAPI   ?? new UploadAPI(
                                                                             this,
                                                                             HTTPExtAPI.HTTPServer,
                                                                             URLPathPrefix:   this.HTTPUploadAPI_Path,
                                                                             FileSystemPath:  this.HTTPUploadAPI_FileSystemPath
                                                                         );

                }

                if (!HTTPDownloadAPI_Disabled)
                {

                    Directory.CreateDirectory(this.HTTPDownloadAPI_FileSystemPath);
                    this.HTTPDownloadAPI            = HTTPDownloadAPI ?? new DownloadAPI(
                                                                             this,
                                                                             HTTPExtAPI.HTTPServer,
                                                                             URLPathPrefix:   this.HTTPDownloadAPI_Path,
                                                                             FileSystemPath:  this.HTTPDownloadAPI_FileSystemPath
                                                                         );

                }

                if (!QRCodeAPI_Disabled)
                {

                    this.QRCodeAPI                  = QRCodeAPI       ?? new QRCodeAPI(
                                                                             this,
                                                                             HTTPExtAPI.HTTPServer,
                                                                             URLPathPrefix:   this.QRCodeAPI_Path
                                                                         );

                }

                if (!WebAPI_Disabled)
                {

                    this.WebAPI                     = WebAPI          ?? new WebAPI(
                                                                             this,
                                                                             HTTPExtAPI.HTTPServer,
                                                                             URLPathPrefix:   this.WebAPI_Path
                                                                         );

                }

            }

            #endregion

        }

        #endregion



        #region ChargingStations

        #region Data

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        protected internal readonly ConcurrentDictionary<ChargingStation_Id, ChargingStation> chargingStations = new();

        /// <summary>
        /// An enumeration of all charging stationes.
        /// </summary>
        public IEnumerable<ChargingStation> ChargingStations
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
                                                            ChargingStation    ChargingStation,
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

            _AddChargingStation(ChargingStation                             ChargingStation,
                                Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                EventTracking_Id?                           EventTrackingId   = null,
                                User_Id?                                    CurrentUserId     = null)

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

            AddChargingStation(ChargingStation                             ChargingStation,
                               Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                               EventTracking_Id?                           EventTrackingId   = null,
                               User_Id?                                    CurrentUserId     = null)

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

            _AddChargingStationIfNotExists(ChargingStation                             ChargingStation,
                                           Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                           EventTracking_Id?                           EventTrackingId   = null,
                                           User_Id?                                    CurrentUserId     = null)

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

            AddChargingStationIfNotExists(ChargingStation                             ChargingStation,
                                          Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                          EventTracking_Id?                           EventTrackingId   = null,
                                          User_Id?                                    CurrentUserId     = null)

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

            _AddOrUpdateChargingStation(ChargingStation                             ChargingStation,
                                        Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                        Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                        EventTracking_Id?                           EventTrackingId   = null,
                                        User_Id?                                    CurrentUserId     = null)

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

            AddOrUpdateChargingStation(ChargingStation                             ChargingStation,
                                       Action<ChargingStation, EventTracking_Id>?  OnAdded           = null,
                                       Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                       EventTracking_Id?                           EventTrackingId   = null,
                                       User_Id?                                    CurrentUserId     = null)

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
                                                              ChargingStation    ChargingStation,
                                                              ChargingStation    OldChargingStation,
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

            _UpdateChargingStation(ChargingStation                             ChargingStation,
                                   Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                   EventTracking_Id?                           EventTrackingId   = null,
                                   User_Id?                                    CurrentUserId     = null)

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

            UpdateChargingStation(ChargingStation                             ChargingStation,
                                  Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                           EventTrackingId   = null,
                                  User_Id?                                    CurrentUserId     = null)

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

            _UpdateChargingStation(ChargingStation                             ChargingStation,
                                   Action<ChargingStation.Builder>             UpdateDelegate,
                                   Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                   EventTracking_Id?                           EventTrackingId   = null,
                                   User_Id?                                    CurrentUserId     = null)

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

            UpdateChargingStation(ChargingStation                             ChargingStation,
                                  Action<ChargingStation.Builder>             UpdateDelegate,
                                  Action<ChargingStation, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                           EventTrackingId   = null,
                                  User_Id?                                    CurrentUserId     = null)

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
        public delegate Task OnChargingStationDeletedDelegate(DateTime           Timestamp,
                                                              ChargingStation    ChargingStation,
                                                              EventTracking_Id?  EventTrackingId   = null,
                                                              User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charging station was deleted.
        /// </summary>
        public event OnChargingStationDeletedDelegate? OnChargingStationDeleted;



        #region (protected internal virtual) _CanDeleteChargingStation(ChargingStation)

        /// <summary>
        /// Determines whether the chargingStation can safely be deleted from the API.
        /// </summary>
        /// <param name="ChargingStation">The chargingStation to be deleted.</param>
        protected internal virtual I18NString? _CanDeleteChargingStation(ChargingStation ChargingStation)
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

            _DeleteChargingStation(ChargingStation                             ChargingStation,
                                   Action<ChargingStation, EventTracking_Id>?  OnDeleted         = null,
                                   EventTracking_Id?                           EventTrackingId   = null,
                                   User_Id?                                    CurrentUserId     = null)

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

            DeleteChargingStation(ChargingStation                             ChargingStation,
                                  Action<ChargingStation, EventTracking_Id>?  OnDeleted         = null,
                                  EventTracking_Id?                           EventTrackingId   = null,
                                  User_Id?                                    CurrentUserId     = null)

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
        protected internal ChargingStation? _GetChargingStation(ChargingStation_Id ChargingStationId)
        {

            if (ChargingStationId.IsNotNullOrEmpty && chargingStations.TryGetValue(ChargingStationId, out var chargingStation))
                return chargingStation;

            return default;

        }

        /// <summary>
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        protected internal ChargingStation? _GetChargingStation(ChargingStation_Id? ChargingStationId)
        {

            if (ChargingStationId is not null && chargingStations.TryGetValue(ChargingStationId.Value, out var chargingStation))
                return chargingStation;

            return default;

        }


        /// <summary>
        /// Get the chargingStation having the given unique identification.
        /// </summary>
        /// <param name="ChargingStationId">The unique identification of an charging station.</param>
        public ChargingStation? GetChargingStation(ChargingStation_Id ChargingStationId)
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
        public ChargingStation? GetChargingStation(ChargingStation_Id? ChargingStationId)
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
        protected internal Boolean _TryGetChargingStation(ChargingStation_Id                        ChargingStationId,
                                                          [NotNullWhen(true)] out ChargingStation?  ChargingStation)
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
        protected internal Boolean _TryGetChargingStation(ChargingStation_Id?                       ChargingStationId,
                                                          [NotNullWhen(true)] out ChargingStation?  ChargingStation)
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
        public Boolean TryGetChargingStation(ChargingStation_Id                        ChargingStationId,
                                             [NotNullWhen(true)] out ChargingStation?  ChargingStation)
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
        public Boolean TryGetChargingStation(ChargingStation_Id?                       ChargingStationId,
                                             [NotNullWhen(true)] out ChargingStation?  ChargingStation)
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



        #region (override) HandleErrors(Module, Caller, ErrorResponse)

        public override Task HandleErrors(String  Module,
                                          String  Caller,
                                          String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ErrorResponse
                   );

        }

        #endregion

        #region (override) HandleErrors(Module, Caller, ExceptionOccured)

        public override Task HandleErrors(String     Module,
                                          String     Caller,
                                          Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ExceptionOccured
                   );

        }

        #endregion


    }

}
