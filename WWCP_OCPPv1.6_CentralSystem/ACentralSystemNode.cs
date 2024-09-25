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
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CentralSystem
{

    /// <summary>
    /// An abstract Charging Station Management System node.
    /// </summary>
    public abstract class ACentralSystemNode : AOCPPNetworkingNode
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


        //public HTTPAPI?                    HTTPAPI                           { get; }

        //public DownloadAPI?                HTTPDownloadAPI                   { get; }
        //public HTTPPath?                   HTTPDownloadAPI_Path              { get; }
        //public String?                     HTTPDownloadAPI_FileSystemPath    { get; }

        //public UploadAPI?                  HTTPUploadAPI                     { get; }
        //public HTTPPath?                   HTTPUploadAPI_Path                { get; }
        //public String?                     HTTPUploadAPI_FileSystemPath      { get; }

        //public QRCodeAPI?                  QRCodeAPI                         { get; }
        //public HTTPPath?                   QRCodeAPI_Path                    { get; }

        //public WebAPI?                     WebAPI                            { get; }
        //public HTTPPath?                   WebAPI_Path                       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract Charging Station Management System (CSMS).
        /// </summary>
        /// <param name="Id">The unique identification of this Charging Station Management System (CSMS).</param>
        /// <param name="Description">An optional multi-language description of the Charging Station Management System (CSMS).</param>
        public ACentralSystemNode(NetworkingNode_Id              Id,
                         String                         VendorName,
                         String                         Model,
                         String?                        SerialNumber                     = null,
                         String?                        SoftwareVersion                  = null,
                         I18NString?                    Description                      = null,
                         CustomData?                    CustomData                       = null,

                         AsymmetricCipherKeyPair?       ClientCAKeyPair                  = null,
                         BCx509.X509Certificate?        ClientCACertificate              = null,

                         SignaturePolicy?               SignaturePolicy                  = null,
                         SignaturePolicy?               ForwardingSignaturePolicy        = null,

                         //Func<ACentralSystemNode, HTTPAPI>?      HTTPAPI                          = null,
                         //Boolean                        HTTPAPI_Disabled                 = false,
                         //IPPort?                        HTTPAPI_Port                     = null,
                         //String?                        HTTPAPI_ServerName               = null,
                         //String?                        HTTPAPI_ServiceName              = null,
                         //EMailAddress?                  HTTPAPI_RobotEMailAddress        = null,
                         //String?                        HTTPAPI_RobotGPGPassphrase       = null,
                         //Boolean                        HTTPAPI_EventLoggingDisabled     = false,

                         //Func<ACentralSystemNode, DownloadAPI>?  HTTPDownloadAPI                  = null,
                         //Boolean                        HTTPDownloadAPI_Disabled         = false,
                         //HTTPPath?                      HTTPDownloadAPI_Path             = null,
                         //String?                        HTTPDownloadAPI_FileSystemPath   = null,

                         Func<ACentralSystemNode, UploadAPI>?    HTTPUploadAPI                    = null,
                         Boolean                        HTTPUploadAPI_Disabled           = false,
                         HTTPPath?                      HTTPUploadAPI_Path               = null,
                         String?                        HTTPUploadAPI_FileSystemPath     = null,

                         //HTTPPath?                      FirmwareDownloadAPIPath          = null,
                         //HTTPPath?                      LogfilesUploadAPIPath            = null,
                         //HTTPPath?                      DiagnosticsUploadAPIPath         = null,

                         //Func<ACentralSystemNode, QRCodeAPI>?    QRCodeAPI                        = null,
                         //Boolean                        QRCodeAPI_Disabled               = false,
                         //HTTPPath?                      QRCodeAPI_Path                   = null,

                         //Func<ACentralSystemNode, WebAPI>?       WebAPI                           = null,
                         //Boolean                        WebAPI_Disabled                  = false,
                         //HTTPPath?                      WebAPI_Path                      = null,

                         TimeSpan?                      DefaultRequestTimeout            = null,

                         Boolean                        DisableSendHeartbeats            = false,
                         TimeSpan?                      SendHeartbeatsEvery              = null,

                         Boolean                        DisableMaintenanceTasks          = false,
                         TimeSpan?                      MaintenanceEvery                 = null,

                         ISMTPClient?                   SMTPClient                       = null,
                         DNSClient?                     DNSClient                        = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   null,
                   //!HTTPAPI_Disabled
                   //    ? new HTTPExtAPI(
                   //          HTTPServerPort:         HTTPAPI_Port               ?? IPPort.Auto,
                   //          HTTPServerName:         HTTPAPI_ServerName         ?? "GraphDefined OCPP Test Central System",
                   //          HTTPServiceName:        HTTPAPI_ServiceName        ?? "GraphDefined OCPP Test Central System Service",
                   //          APIRobotEMailAddress:   HTTPAPI_RobotEMailAddress  ?? EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                   //          APIRobotGPGPassphrase:  HTTPAPI_RobotGPGPassphrase ?? "test123",
                   //          SMTPClient:             SMTPClient                 ?? new NullMailer(),
                   //          DNSClient:              DNSClient,
                   //          AutoStart:              true
                   //      )
                   //    : null,

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

            //this.HTTPUploadAPI_Path              = HTTPUploadAPI_Path             ?? HTTPPath.Parse("uploads");
            //this.HTTPDownloadAPI_Path            = HTTPDownloadAPI_Path           ?? HTTPPath.Parse("downloads");
            //this.QRCodeAPI_Path                  = QRCodeAPI_Path                 ?? HTTPPath.Parse("qr");
            //this.WebAPI_Path                     = WebAPI_Path                    ?? HTTPPath.Parse("webapi");

            //this.HTTPUploadAPI_FileSystemPath    = HTTPUploadAPI_FileSystemPath   ?? Path.Combine(AppContext.BaseDirectory, "UploadAPI");
            //this.HTTPDownloadAPI_FileSystemPath  = HTTPDownloadAPI_FileSystemPath ?? Path.Combine(AppContext.BaseDirectory, "DownloadAPI");

            //if (this.HTTPExtAPI is not null)
            //    this.HTTPAPI                     = !HTTPAPI_Disabled
            //                                           ? HTTPAPI?.Invoke(this)      ?? new HTTPAPI(
            //                                                                               this,
            //                                                                               HTTPExtAPI,
            //                                                                               EventLoggingDisabled: HTTPAPI_EventLoggingDisabled
            //                                                                           )
            //                                           : null;

            //if (this.HTTPAPI is not null)
            //{

            //    #region HTTP API Security Settings

            //    this.HTTPAPI.HTTPBaseAPI.HTTPServer.AddAuth(request => {

            //        // Allow some URLs for anonymous access...
            //        if (request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.HTTPUploadAPI_Path)   ||
            //            request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.HTTPDownloadAPI_Path) ||
            //            request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.WebAPI_Path))
            //        {
            //            return HTTPExtAPI.Anonymous;
            //        }

            //        return null;

            //    });

            //    #endregion


            //    if (!HTTPUploadAPI_Disabled)
            //    {

            //        Directory.CreateDirectory(this.HTTPUploadAPI_FileSystemPath);
            //        this.HTTPUploadAPI              = HTTPUploadAPI?.Invoke(this)   ?? new UploadAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.HTTPUploadAPI_Path,
            //                                                                               FileSystemPath:  this.HTTPUploadAPI_FileSystemPath
            //                                                                           );

            //    }

            //    if (!HTTPDownloadAPI_Disabled)
            //    {

            //        Directory.CreateDirectory(this.HTTPDownloadAPI_FileSystemPath);
            //        this.HTTPDownloadAPI            = HTTPDownloadAPI?.Invoke(this) ?? new DownloadAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.HTTPDownloadAPI_Path,
            //                                                                               FileSystemPath:  this.HTTPDownloadAPI_FileSystemPath
            //                                                                           );

            //    }

            //    if (!QRCodeAPI_Disabled)
            //    {

            //        this.QRCodeAPI                  = QRCodeAPI?.Invoke(this)       ?? new QRCodeAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.QRCodeAPI_Path
            //                                                                           );

            //    }

            //    if (!WebAPI_Disabled)
            //    {

            //        this.WebAPI                     = WebAPI?.Invoke(this)          ?? new WebAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.WebAPI_Path
            //                                                                           );

            //    }

            //}

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
