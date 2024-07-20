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

namespace cloud.charging.open.protocols.OCPPv2_1.Gateway
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
    /// An abstract gateway node.
    /// </summary>
    public abstract class AGatewayNode : ANetworkingNode,
                                         GW.IGatewayNode
    {

        #region Data

        private readonly HTTPExtAPI  HTTPAPI;

        #endregion

        #region Properties

        /// <summary>
        /// The gateway vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                 { get; }      = "";

        /// <summary>
        ///  The gateway model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                      { get; }      = "";

        /// <summary>
        /// The optional serial number of the gateway.
        /// </summary>
        [Optional]
        public String?                     SerialNumber               { get; }

        /// <summary>
        /// The optional firmware version of the gateway.
        /// </summary>
        [Optional]
        public String?                     SoftwareVersion            { get; }


        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                   CSMSTime                   { get; set; } = Timestamp.Now;



        public WebAPI                      WebAPI                     { get; }

        private readonly HashSet<WebAPI> webAPIs = [];

        /// <summary>
        /// An enumeration of all WebAPIs.
        /// </summary>
        public IEnumerable<WebAPI> WebAPIs
            => webAPIs;

        #endregion

        #region Events

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a JSON message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        ///// <summary>
        ///// An event sent whenever the error response to a JSON message was sent.
        ///// </summary>
        //public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        ///// <summary>
        ///// An event sent whenever an error response to a JSON message request was received.
        ///// </summary>
        //public event NetworkingNode.CSMS.OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event NetworkingNode.CSMS.OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract gateway node.
        /// </summary>
        /// <param name="Id">The unique identification of this gateway node.</param>
        public AGatewayNode(NetworkingNode_Id  Id,
                            String             VendorName,
                            String             Model,
                            String?            SerialNumber                = null,
                            String?            SoftwareVersion             = null,
                            I18NString?        Description                 = null,
                            CustomData?        CustomData                  = null,

                            SignaturePolicy?   SignaturePolicy             = null,
                            SignaturePolicy?   ForwardingSignaturePolicy   = null,

                            Boolean            DisableSendHeartbeats       = false,
                            TimeSpan?          SendHeartbeatsEvery         = null,
                            TimeSpan?          DefaultRequestTimeout       = null,

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

            #region Setup generic HTTP API

            this.HTTPAPI  = new HTTPExtAPI(
                                HTTPServerPort:         IPPort.Parse(3532),
                                HTTPServerName:         "GraphDefined OCPP Test Gateway",
                                HTTPServiceName:        "GraphDefined OCPP Test Gateway Service",
                                APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Gateway Robot <robot@charging.cloud>"),
                                APIRobotGPGPassphrase:  "test123",
                                SMTPClient:             new NullMailer(),
                                DNSClient:              DNSClient,
                                AutoStart:              true
                            );

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            #endregion

            #region HTTP API Security Settings

            var webAPIPrefix = "webapi";

            this.HTTPAPI.HTTPServer.AddAuth(request => {

                // Allow some URLs for anonymous access...
                if (request.Path.StartsWith(HTTPAPI.URLPathPrefix + webAPIPrefix))
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });

            #endregion

            #region Setup WebAPIs

            this.WebAPI             = new WebAPI(
                                          this,
                                          HTTPAPI,

                                          URLPathPrefix: HTTPPath.Parse(webAPIPrefix)

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
