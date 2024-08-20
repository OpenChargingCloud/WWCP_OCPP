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
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

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
    /// An abstract OCPP Local Controller node.
    /// </summary>
    public abstract class ALocalControllerNode : ANetworkingNode,
                                                 LC.ILocalControllerNode
    {

        #region Properties

        /// <summary>
        /// The local controller vendor identification.
        /// </summary>
        [Mandatory]
        public String        VendorName                        { get; }      = "";

        /// <summary>
        ///  The local controller model identification.
        /// </summary>
        [Mandatory]
        public String        Model                             { get; }      = "";

        /// <summary>
        /// The optional serial number of the local controller.
        /// </summary>
        [Optional]
        public String?       SerialNumber                      { get; }

        /// <summary>
        /// The optional firmware version of the local controller.
        /// </summary>
        [Optional]
        public String?       SoftwareVersion                   { get; }

        /// <summary>
        /// The modem of the local controller.
        /// </summary>
        [Optional]
        public Modem?        Modem                             { get; }


        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?     CSMSTime                          { get; set; } = Timestamp.Now;


        public HTTPAPI?      HTTPAPI                           { get; }

        public WebAPI?       WebAPI                            { get; }
        public HTTPPath?     WebAPI_Path                       { get; }

        public DownloadAPI?  HTTPDownloadAPI                   { get; }
        public HTTPPath?     HTTPDownloadAPI_Path              { get; }
        public String?       HTTPDownloadAPI_FileSystemPath    { get; }

        public UploadAPI?    HTTPUploadAPI                     { get; }
        public HTTPPath?     HTTPUploadAPI_Path                { get; }
        public String?       HTTPUploadAPI_FileSystemPath      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new local controller node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this local controller node.</param>
        public ALocalControllerNode(NetworkingNode_Id  Id,
                                    String             VendorName,
                                    String             Model,
                                    String?            SerialNumber                     = null,
                                    String?            SoftwareVersion                  = null,
                                    Modem?             Modem                            = null,
                                    I18NString?        Description                      = null,
                                    CustomData?        CustomData                       = null,

                                    SignaturePolicy?   SignaturePolicy                  = null,
                                    SignaturePolicy?   ForwardingSignaturePolicy        = null,

                                    Boolean            HTTPAPI_Disabled                 = false,
                                    IPPort?            HTTPAPI_Port                     = null,
                                    String?            HTTPAPI_ServerName               = null,
                                    String?            HTTPAPI_ServiceName              = null,
                                    EMailAddress?      HTTPAPI_RobotEMailAddress        = null,
                                    String?            HTTPAPI_RobotGPGPassphrase       = null,
                                    Boolean            HTTPAPI_EventLoggingDisabled     = false,

                                    DownloadAPI?       HTTPDownloadAPI                  = null,
                                    Boolean            HTTPDownloadAPI_Disabled         = false,
                                    HTTPPath?          HTTPDownloadAPI_Path             = null,
                                    String?            HTTPDownloadAPI_FileSystemPath   = null,

                                    UploadAPI?         HTTPUploadAPI                    = null,
                                    Boolean            HTTPUploadAPI_Disabled           = false,
                                    HTTPPath?          HTTPUploadAPI_Path               = null,
                                    String?            HTTPUploadAPI_FileSystemPath     = null,

                                    WebAPI?            WebAPI                           = null,
                                    Boolean            WebAPI_Disabled                  = false,
                                    HTTPPath?          WebAPI_Path                      = null,

                                    TimeSpan?          DefaultRequestTimeout            = null,

                                    Boolean            DisableSendHeartbeats            = false,
                                    TimeSpan?          SendHeartbeatsEvery              = null,

                                    Boolean            DisableMaintenanceTasks          = false,
                                    TimeSpan?          MaintenanceEvery                 = null,

                                    DNSClient?         DNSClient                        = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   !HTTPAPI_Disabled
                       ? new HTTPExtAPI(
                             HTTPServerPort:          HTTPAPI_Port ?? IPPort.Auto,
                             HTTPServerName:          "GraphDefined OCPP Test Local Controller",
                             HTTPServiceName:         "GraphDefined OCPP Test Local Controller Service",
                             APIRobotEMailAddress:    EMailAddress.Parse("GraphDefined OCPP Test Local Controller Robot <robot@charging.cloud>"),
                             APIRobotGPGPassphrase:   "test123",
                             SMTPClient:              new NullMailer(),
                             DNSClient:               DNSClient,
                             AutoStart:               true
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
            this.Modem                           = Modem;

            this.HTTPUploadAPI_Path              = HTTPUploadAPI_Path             ?? HTTPPath.Parse("uploads");
            this.HTTPDownloadAPI_Path            = HTTPDownloadAPI_Path           ?? HTTPPath.Parse("downloads");
            this.WebAPI_Path                     = WebAPI_Path                    ?? HTTPPath.Parse("webapi");

            this.HTTPUploadAPI_FileSystemPath    = HTTPUploadAPI_FileSystemPath   ?? Path.Combine(AppContext.BaseDirectory, "UploadAPI");
            this.HTTPDownloadAPI_FileSystemPath  = HTTPDownloadAPI_FileSystemPath ?? Path.Combine(AppContext.BaseDirectory, "DownloadAPI");

            #region Setup Web-/Upload-/DownloadAPIs

            if (HTTPExtAPI is not null)
            {

                this.HTTPAPI                     = HTTPAPI         ?? new HTTPAPI(
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
                    this.HTTPUploadAPI           = HTTPUploadAPI   ?? new UploadAPI(
                                                                          this,
                                                                          HTTPExtAPI.HTTPServer,
                                                                          URLPathPrefix:   this.HTTPUploadAPI_Path,
                                                                          FileSystemPath:  this.HTTPUploadAPI_FileSystemPath
                                                                      );

                }

                if (!HTTPDownloadAPI_Disabled)
                {

                    Directory.CreateDirectory(this.HTTPDownloadAPI_FileSystemPath);
                    this.HTTPDownloadAPI         = HTTPDownloadAPI ?? new DownloadAPI(
                                                                          this,
                                                                          HTTPExtAPI.HTTPServer,
                                                                          URLPathPrefix:   this.HTTPDownloadAPI_Path,
                                                                          FileSystemPath:  this.HTTPDownloadAPI_FileSystemPath
                                                                      );

                }

                if (!WebAPI_Disabled)
                {

                    this.WebAPI                  = WebAPI          ?? new WebAPI(
                                                                          this,
                                                                          HTTPExtAPI.HTTPServer,
                                                                          URLPathPrefix:   this.WebAPI_Path
                                                                      );

                }

            }

            #endregion

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
