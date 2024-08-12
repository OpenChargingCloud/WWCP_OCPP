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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.LocalController.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LocalController
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
    /// An abstract local controller node.
    /// </summary>
    public abstract class ALocalControllerNode : ANetworkingNode,
                                                 LC.ILocalControllerNode
    {

        #region Data

        private readonly HTTPExtAPI  HTTPAPI;

        #endregion

        #region Properties

        /// <summary>
        /// The local controller vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                 { get; }      = "";

        /// <summary>
        ///  The local controller model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                      { get; }      = "";

        /// <summary>
        /// The optional serial number of the local controller.
        /// </summary>
        [Optional]
        public String?                     SerialNumber               { get; }

        /// <summary>
        /// The optional firmware version of the local controller.
        /// </summary>
        [Optional]
        public String?                     SoftwareVersion            { get; }

        /// <summary>
        /// The modem of the local controller.
        /// </summary>
        [Optional]
        public Modem?                      Modem                      { get; }


        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                   CSMSTime                   { get; set; } = Timestamp.Now;





        public WebAPI                      WebAPI                     { get; }

        public UploadAPI                   HTTPUploadAPI              { get; }

        public DownloadAPI                 HTTPDownloadAPI            { get; }



        private readonly HashSet<WebAPI> webAPIs = [];

        /// <summary>
        /// An enumeration of all WebAPIs.
        /// </summary>
        public IEnumerable<WebAPI> WebAPIs
            => webAPIs;


        private readonly HashSet<UploadAPI> uploadAPIs = [];

        /// <summary>
        /// An enumeration of all UploadAPIs.
        /// </summary>
        public IEnumerable<UploadAPI> UploadAPIs
            => uploadAPIs;


        private readonly HashSet<DownloadAPI> downloadAPIs = [];

        /// <summary>
        /// An enumeration of all DownloadAPIs.
        /// </summary>
        public IEnumerable<DownloadAPI> DownloadAPIs
            => downloadAPIs;

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
        /// Create a new local controller node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this local controller node.</param>
        public ALocalControllerNode(NetworkingNode_Id  Id,
                                    String             VendorName,
                                    String             Model,
                                    String?            SerialNumber                = null,
                                    String?            SoftwareVersion             = null,
                                    Modem?             Modem                       = null,
                                    I18NString?        Description                 = null,
                                    CustomData?        CustomData                  = null,

                                    SignaturePolicy?   SignaturePolicy             = null,
                                    SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                    Boolean            DisableSendHeartbeats       = false,
                                    TimeSpan?          SendHeartbeatsEvery         = null,
                                    TimeSpan?          DefaultRequestTimeout       = null,

                                    IPPort?            HTTPUploadPort              = null,
                                    IPPort?            HTTPDownloadPort            = null,

                                    Boolean            DisableMaintenanceTasks     = false,
                                    TimeSpan?          MaintenanceEvery            = null,
                                    DNSClient?         DNSClient                   = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

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

            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.SerialNumber             = SerialNumber;
            this.SoftwareVersion          = SoftwareVersion;
            this.Modem                    = Modem;

            #region Setup generic HTTP API

            this.HTTPAPI  = new HTTPExtAPI(
                                HTTPServerPort:         IPPort.Auto,
                                HTTPServerName:         "GraphDefined OCPP Test Local Controller",
                                HTTPServiceName:        "GraphDefined OCPP Test Local Controller Service",
                                APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Local Controller Robot <robot@charging.cloud>"),
                                APIRobotGPGPassphrase:  "test123",
                                SMTPClient:             new NullMailer(),
                                DNSClient:              DNSClient,
                                AutoStart:              true
                            );

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            #endregion

            #region HTTP API Security Settings

            var webAPIPrefix        = "webapi";
            var uploadAPIPrefix     = "uploads";
            var downloadAPIPrefix   = "downloads";

            this.HTTPAPI.HTTPServer.AddAuth(request => {

                // Allow some URLs for anonymous access...
                if (request.Path.StartsWith(HTTPAPI.URLPathPrefix + webAPIPrefix)    ||
                    request.Path.StartsWith(HTTPAPI.URLPathPrefix + uploadAPIPrefix) ||
                    request.Path.StartsWith(HTTPAPI.URLPathPrefix + downloadAPIPrefix))
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });

            #endregion

            #region Setup Web-/Upload-/DownloadAPIs

            this.WebAPI             = new WebAPI(
                                          this,
                                          HTTPAPI,

                                          URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

                                      );

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "UploadAPI"));

            this.HTTPUploadAPI      = new UploadAPI(
                                          this,
                                          HTTPUploadPort.HasValue
                                              ? new HTTPServer(
                                                    HTTPUploadPort.Value,
                                                    UploadAPI.DefaultHTTPServerName,
                                                    UploadAPI.DefaultHTTPServiceName
                                                )
                                              : HTTPAPI.HTTPServer,

                                          URLPathPrefix:   HTTPPath.Parse(uploadAPIPrefix),
                                          FileSystemPath:  Path.Combine(AppContext.BaseDirectory, "UploadAPI")

                                      );

            this.HTTPDownloadAPI    = new DownloadAPI(
                                          this,
                                          HTTPDownloadPort.HasValue
                                              ? new HTTPServer(
                                                    HTTPDownloadPort.Value,
                                                    DownloadAPI.DefaultHTTPServerName,
                                                    DownloadAPI.DefaultHTTPServiceName
                                                )
                                              : HTTPAPI.HTTPServer,

                                          URLPathPrefix:   HTTPPath.Parse(downloadAPIPrefix),
                                          FileSystemPath:  Path.Combine(AppContext.BaseDirectory, "DownloadAPI")

                                      );

            #endregion

        }

        #endregion


        #region AttachWebAPI(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public WebAPI AttachWebAPI(HTTPHostname?                               HTTPHostname            = null,
                                                 String?                                     ExternalDNSName         = null,
                                                 IPPort?                                     HTTPServerPort          = null,
                                                 HTTPPath?                                   BasePath                = null,
                                                 String?                                     HTTPServerName          = null,

                                                 HTTPPath?                                   URLPathPrefix           = null,
                                                 String?                                     HTTPServiceName         = null,

                                                 String                                      HTTPRealm               = "...",
                                                 Boolean                                     RequireAuthentication   = true,
                                                 IEnumerable<KeyValuePair<String, String>>?  HTTPLogins              = null,

                                                 String?                                     HTMLTemplate            = null,
                                                 Boolean                                     AutoStart               = false)
        {

            var httpAPI  = new HTTPExtAPI(

                               HTTPHostname,
                               ExternalDNSName,
                               HTTPServerPort,
                               BasePath,
                               HTTPServerName,

                               URLPathPrefix,
                               HTTPServiceName,

                               DNSClient: DNSClient,
                               AutoStart: false

                           );

            var webAPI   = new WebAPI(

                               this,
                               httpAPI,

                               HTTPServerName,
                               URLPathPrefix,
                               BasePath,
                               HTTPRealm,
                               HTTPLogins,
                               HTMLTemplate

                           );

            if (AutoStart)
                httpAPI.Start();

            return webAPI;

        }

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
