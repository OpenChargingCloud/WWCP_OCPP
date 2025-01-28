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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.LocalController
{

    #region (class) ChargePointConnector

    ///// <summary>
    ///// A connector at a charge point.
    ///// </summary>
    //public class ChargePointConnector
    //{

    //    #region Properties

    //    public Connector_Id    Id               { get; }
    //    public ConnectorType  ConnectorType    { get; }

    //    #endregion

    //    #region ChargePointConnector(Id, ConnectorType)

    //    public ChargePointConnector(Connector_Id    Id,
    //                                    ConnectorType  ConnectorType)
    //    {

    //        this.Id             = Id;
    //        this.ConnectorType  = ConnectorType;

    //    }

    //    #endregion

    //}

    #endregion


    /// <summary>
    /// An abstract OCPP Local Controller node.
    /// </summary>
    public abstract class ALocalControllerNode : AOCPPNetworkingNode,
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
     //   public Modem?        Modem                             { get; }


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
                             //       Modem?             Modem                            = null,
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

                                    WebSocketServer?   ControlWebSocketServer           = null,

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

            this.VendorName                      = VendorName;
            this.Model                           = Model;
            this.SerialNumber                    = SerialNumber;
            this.SoftwareVersion                 = SoftwareVersion;
          //  this.Modem                           = Modem;

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


    }

}
