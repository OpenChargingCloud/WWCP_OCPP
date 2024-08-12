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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.EnergyMeter
{

    /// <summary>
    /// An abstract energy meter node.
    /// </summary>
    public abstract class AEnergyMeterNode : ANetworkingNode,
                                             EM.IEnergyMeterNode
    {

        #region Data

        private readonly HTTPExtAPI?  HTTPAPI;

        #endregion

        #region Properties

        /// <summary>
        /// The energy meter vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                 { get; }      = "";

        /// <summary>
        ///  The energy meter model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                      { get; }      = "";

        /// <summary>
        /// The optional serial number of the energy meter.
        /// </summary>
        [Optional]
        public String?                     SerialNumber               { get; }

        /// <summary>
        /// The optional firmware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                     FirmwareVersion            { get; }

        /// <summary>
        /// The modem of the energy meter.
        /// </summary>
        [Optional]
        public Modem?                      Modem                      { get; }


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
        /// Create a new abstract energy meter node.
        /// </summary>
        /// <param name="Id">The unique identification of this energy meter node.</param>
        public AEnergyMeterNode(NetworkingNode_Id  Id,
                                String             VendorName,
                                String             Model,
                                String?            SerialNumber                = null,
                                String?            FirmwareVersion             = null,
                                Modem?             Modem                       = null,
                                I18NString?        Description                 = null,
                                CustomData?        CustomData                  = null,

                                SignaturePolicy?   SignaturePolicy             = null,
                                SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                Boolean            DisableHTTPAPI              = false,
                                IPPort?            HTTPAPIPort                 = null,

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
            this.FirmwareVersion          = FirmwareVersion;
            this.Modem                    = Modem;

            #region Setup generic HTTP API

            this.HTTPAPI  = !DisableHTTPAPI

                                ? new HTTPExtAPI(
                                      HTTPServerPort:         HTTPAPIPort ?? IPPort.Auto,
                                      HTTPServerName:         "GraphDefined OCPP Test Energy Meter",
                                      HTTPServiceName:        "GraphDefined OCPP Test Energy Meter Service",
                                      APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Energy Meter Robot <robot@charging.cloud>"),
                                      APIRobotGPGPassphrase:  "test123",
                                      SMTPClient:             new NullMailer(),
                                      DNSClient:              DNSClient,
                                      AutoStart:              true
                                  )

                                : null;

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
