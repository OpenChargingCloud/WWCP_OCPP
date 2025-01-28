/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.EnergyMeter
{

    /// <summary>
    /// An abstract energy meter node.
    /// </summary>
    public abstract class AEnergyMeterNode : AOCPPNetworkingNode,
                                             EM.IEnergyMeterNode
    {

        #region Data

        //private readonly HTTPExtAPI?  HTTPAPI;

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



        public HTTPAPI?                    HTTPAPI                           { get; }

        public WebAPI?                     WebAPI                            { get; }
        public HTTPPath?                   WebAPI_Path                       { get; }

        #endregion

        #region Events


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract energy meter node.
        /// </summary>
        /// <param name="Id">The unique identification of this energy meter node.</param>
        public AEnergyMeterNode(NetworkingNode_Id  Id,
                                String             VendorName,
                                String             Model,
                                String?            SerialNumber                   = null,
                                String?            FirmwareVersion                = null,
                                Modem?             Modem                          = null,
                                I18NString?        Description                    = null,
                                CustomData?        CustomData                     = null,

                                SignaturePolicy?   SignaturePolicy                = null,
                                SignaturePolicy?   ForwardingSignaturePolicy      = null,

                                HTTPAPI?           HTTPAPI                        = null,
                                Boolean            HTTPAPI_Disabled               = false,
                                IPPort?            HTTPAPI_Port                   = null,
                                String?            HTTPAPI_ServerName             = null,
                                String?            HTTPAPI_ServiceName            = null,
                                EMailAddress?      HTTPAPI_RobotEMailAddress      = null,
                                String?            HTTPAPI_RobotGPGPassphrase     = null,
                                Boolean            HTTPAPI_EventLoggingDisabled   = false,

                                WebAPI?            WebAPI                         = null,
                                Boolean            WebAPI_Disabled                = false,
                                HTTPPath?          WebAPI_Path                    = null,

                                WebSocketServer?   ControlWebSocketServer         = null,

                                Boolean            DisableSendHeartbeats          = false,
                                TimeSpan?          SendHeartbeatsEvery            = null,
                                TimeSpan?          DefaultRequestTimeout          = null,

                                Boolean            DisableMaintenanceTasks        = false,
                                TimeSpan?          MaintenanceEvery               = null,
                                DNSClient?         DNSClient                      = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   !HTTPAPI_Disabled
                       ? new HTTPExtAPI(
                             HTTPServerPort:          HTTPAPI_Port ?? IPPort.Auto,
                             HTTPServerName:          "GraphDefined OCPP Test Energy Meter",
                             HTTPServiceName:         "GraphDefined OCPP Test Energy Meter Service",
                             APIRobotEMailAddress:    EMailAddress.Parse("GraphDefined OCPP Test Energy Meter Robot <robot@charging.cloud>"),
                             APIRobotGPGPassphrase:   "test123",
                             SMTPClient:              new NullMailer(),
                             DNSClient:               DNSClient,
                             AutoStart:               true
                         )
                       : null,
                   ControlWebSocketServer,

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

            #region Setup Web-/Upload-/DownloadAPIs

            this.WebAPI_Path              = WebAPI_Path ?? HTTPPath.Parse("webapi");

            if (HTTPExtAPI is not null)
            {

                this.HTTPAPI              = HTTPAPI ?? new HTTPAPI(
                                                           this,
                                                           HTTPExtAPI,
                                                           EventLoggingDisabled: HTTPAPI_EventLoggingDisabled
                                                       );

                #region HTTP API Security Settings

                this.HTTPExtAPI.HTTPServer.AddAuth(request => {

                    // Allow some URLs for anonymous access...
                    if (request.Path.StartsWith(HTTPExtAPI.URLPathPrefix + this.WebAPI_Path))
                    {
                        return HTTPExtAPI.Anonymous;
                    }

                    return null;

                });

                #endregion

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
