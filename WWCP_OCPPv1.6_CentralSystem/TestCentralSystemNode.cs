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

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Utilities;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.CentralSystem;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A Central System for testing.
    /// </summary>
    public class TestCentralSystemNode : ACentralSystemNode,
                                         ICentralSystemService,
                                         IEventSender
    {

        #region Data

        private          readonly  HashSet<WWCP.SignaturePolicy>                                                                 signaturePolicies        = [];

        //private          readonly  HashSet<OCPPv1_6.CS.ICentralSystemChannel>                                                    centralSystemServers     = [];

        //private          readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<OCPPv1_6.CS.ICentralSystemChannel, DateTime>>   reachableChargeBoxes     = [];

        private          readonly  HTTPExtAPI                                                                                    TestAPI;

        private          readonly  OCPPWebAPI                                                                                    WebAPI;

        protected static readonly  SemaphoreSlim                                                                                 ChargeBoxesSemaphore     = new (1, 1);

        public    static readonly  IPPort                                                                                        DefaultHTTPUploadPort    = IPPort.Parse(9901);

        private                    Int64                                                                                         internalRequestId        = 900000;

        private                    TimeSpan                                                                                      defaultRequestTimeout    = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        public UploadAPI  HTTPUploadAPI             { get; }

        public IPPort     HTTPUploadPort            { get; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan   DefaultRequestTimeout     { get; }


        ///// <summary>
        ///// An enumeration of central system servers.
        ///// </summary>
        //public IEnumerable<OCPPv1_6.CS.ICentralSystemChannel> CentralSystemServers
        //    => centralSystemServers;

        ///// <summary>
        ///// The unique identifications of all connected or reachable charge boxes.
        ///// </summary>
        //public IEnumerable<NetworkingNode_Id> ConnectedNetworkingNodeIds
        //    => reachableChargeBoxes.Values.SelectMany(tuple => tuple.Item1.ConnectedNetworkingNodeIds);


        public Dictionary<String, Transaction_Id> TransactionIds = [];


        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<WWCP.SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public WWCP.SignaturePolicy               SignaturePolicy
            => SignaturePolicies.First();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this central system.</param>
        public TestCentralSystemNode(NetworkingNode_Id      Id,
                                     String                 VendorName,
                                     String                 Model,
                                     String?                SerialNumber                       = null,
                                     String?                SoftwareVersion                    = null,
                                     I18NString?            Description                        = null,
                                     CustomData?            CustomData                         = null,

                                     IPPort?                HTTPUploadPort                     = null,

                                     WebSocketServer?       ControlWebSocketServer             = null,

                                     SignaturePolicy?       SignaturePolicy                    = null,
                                     SignaturePolicy?       ForwardingSignaturePolicy          = null,

                                     ChargeBoxAccessTypes?  AutoCreatedChargeBoxesAccessType   = null,

                                     TimeSpan?              DefaultRequestTimeout              = null,

                                     Boolean                DisableSendHeartbeats              = false,
                                     TimeSpan?              SendHeartbeatsEvery                = null,

                                     Boolean                DisableMaintenanceTasks            = false,
                                     TimeSpan?              MaintenanceEvery                   = null,

                                     ISMTPClient?           SMTPClient                         = null,
                                     DNSClient?             DNSClient                          = null)

            : base(Id,
                   VendorName,
                   Model,
                   SerialNumber,
                   SoftwareVersion,
                   Description,
                   CustomData,

                   null,
                   null,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   null,
                   false,
                   null,
                   null,

                   ControlWebSocketServer,

                   AutoCreatedChargeBoxesAccessType,

                   DefaultRequestTimeout,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   SMTPClient,
                   DNSClient)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id), "The given central system identification must not be null or empty!");

            this.DefaultRequestTimeout  = DefaultRequestTimeout ?? defaultRequestTimeout;
            this.HTTPUploadPort         = HTTPUploadPort        ?? DefaultHTTPUploadPort;

            Directory.CreateDirectory("HTTPSSEs");

            this.TestAPI                = new HTTPExtAPI(
                                              HTTPServerPort:         IPPort.Parse(3500),
                                              HTTPServerName:         "GraphDefined OCPP v1.6 Test Central System",
                                              HTTPServiceName:        "GraphDefined OCPP v1.6 Test Central System Service",
                                              APIRobotEMailAddress:   EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                              SMTPClient:             new NullMailer(),
                                              DNSClient:              DNSClient,
                                              AutoStart:              true
                                          );

            this.TestAPI.HTTPServer.AddAuth(request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(TestAPI.URLPathPrefix + "/webapi"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            this.HTTPUploadAPI           = new UploadAPI(
                                               this,
                                               new HTTPServer(
                                                   this.HTTPUploadPort,
                                                   "Open Charging Cloud OCPP Upload Server",
                                                   "Open Charging Cloud OCPP Upload Service"
                                               )
                                           );

            this.WebAPI                  = new OCPPWebAPI(
                                               this,
                                               TestAPI.HTTPServer
                                           );

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());


            #region Certificates

            #region OnSignCertificate

            OCPP.IN.OnSignCertificate += async (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnSignCertificate: " + request.DestinationId);

                // CSR
                // CertificateType

                Pkcs10CertificationRequest?  parsedCSR       = null;
                String?                      errorResponse   = null;

                //if (!request.CSR.HasValue ||
                //     request.CertificateType.Value == CertificateSigningUse.ChargingStationCertificate)
                //{

                    try
                    {

                        using (var reader = new StringReader(request.CSR))
                        {
                            var pemReader = new PemReader(reader);
                            parsedCSR     = (Pkcs10CertificationRequest) pemReader.ReadObject();
                        }

                    } catch (Exception e)
                    {
                        errorResponse = "The certificate signing request could not be parsed: " + e.Message;
                    }

                    if (parsedCSR is null)
                        errorResponse = "The certificate signing request could not be parsed!";

                    else
                    {

                        if (!parsedCSR.Verify())
                            errorResponse = "The certificate signing request could not be verified!";

                        else if (ClientCAKeyPair     is null)
                            errorResponse = "No ClientCA key pair available!";

                        else if (ClientCACertificate is null)
                            errorResponse = "No ClientCA certificate available!";

                        else
                        {

                            #region Sign the client certificate (background process!)

                            //ToDo: Find better ways to do this!
                            var delayedResponse = Task.Run(async () => {

                                try
                                {

                                    await Task.Delay(100);

                                    var secureRandom  = new SecureRandom();
                                    var now           = Timestamp.Now;

                                    var certificateGenerator = new X509V3CertificateGenerator();
                                    certificateGenerator.SetIssuerDN    (ClientCACertificate.SubjectDN);
                                    certificateGenerator.SetSubjectDN   (parsedCSR.GetCertificationRequestInfo().Subject);
                                    certificateGenerator.SetPublicKey   (parsedCSR.GetPublicKey());
                                    certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), secureRandom));
                                    certificateGenerator.SetNotBefore   (now.AddDays(-1). DateTime);
                                    certificateGenerator.SetNotAfter    (now.AddMonths(3).DateTime);

                                    certificateGenerator.AddExtension   (X509Extensions.KeyUsage,
                                                                         critical: true,
                                                                         new KeyUsage(
                                                                             KeyUsage.NonRepudiation |
                                                                             KeyUsage.DigitalSignature |
                                                                             KeyUsage.KeyEncipherment
                                                                         ));

                                    certificateGenerator.AddExtension   (X509Extensions.ExtendedKeyUsage,
                                                                         critical: true,
                                                                         new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth));

                                    var newClientCertificate = certificateGenerator.Generate(
                                                                   new Asn1SignatureFactory(
                                                                       "SHA256WithRSAEncryption",
                                                                       ClientCAKeyPair.Private,
                                                                       secureRandom
                                                                   )
                                                               );


                                    await Task.Delay(3000);


                                    await OCPP.OUT.CertificateSigned(
                                              new CertificateSignedRequest(
                                                  SourceRouting.To(request.NetworkPath.Source),
                                                  protocols.OCPP.CertificateChain.From(ClientCACertificate, newClientCertificate)
                                             //     request.CertificateType ?? CertificateSigningUse.ChargingStationCertificate
                                              )
                                          );

                                } catch (Exception e)
                                {
                                    DebugX.LogException(e, "The client certificate could not be signed!");
                                }

                            },
                            cancellationToken);

                            #endregion

                        }

                    }

                //}
                //else
                //    errorResponse = $"Invalid CertificateSigningUse: '{request.CertificateType?.ToString() ?? "-"}'!";

                return errorResponse is not null

                           ? new SignCertificateResponse(
                                 Request:      request,
                                 Status:       GenericStatus.Rejected,
                                 Result:       Result.GenericError(errorResponse),
                                 CustomData:   null
                             )

                           : new SignCertificateResponse(
                                 Request:      request,
                                 Status:       GenericStatus.Accepted,
                                 CustomData:   null
                             );

            };

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            this.OCPP.IN.OnAuthorize += (timestamp,
                                         sender,
                                         connection,
                                         request,
                                         cancellationToken) => {

                // IdTag

                return Task.FromResult(
                           idTags.TryGetValue(request.IdTag, out var idTagInfo)

                               ? new AuthorizeResponse(
                                     Request:      request,
                                     IdTagInfo:    idTagInfo,
                                     CustomData:   null
                                 )

                               : new AuthorizeResponse(
                                     Request:      request,
                                     IdTagInfo:    new IdTagInfo(
                                                       Status:      AuthorizationStatus.Invalid,
                                                       ExpiryDate:  Timestamp.Now.AddMinutes(15)
                                                   ),
                                     CustomData:   null
                                 )

                       );

            };

            #endregion

            #region OnMeterValues

            OCPP.IN.OnMeterValues += (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      cancellationToken) => {

                // ConnectorId
                // MeterValues
                // TransactionId

                // We can not say 'NO!" anyway!

                return Task.FromResult(
                           new MeterValuesResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnStartTransaction

            OCPP.IN.OnStartTransaction += (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           cancellationToken) => {

                // ConnectorId
                // IdTag
                // StartTimestamp
                // MeterStart
                // ReservationId

                return Task.FromResult(
                           new StartTransactionResponse(
                               Request:         request,
                               TransactionId:   Transaction_Id.NewRandom,
                               IdTagInfo:       new IdTagInfo(
                                                    AuthorizationStatus.Accepted
                                                ),
                               CustomData:      null
                           )
                       );

            };

            #endregion

            #region OnStatusNotification

            OCPP.IN.OnStatusNotification += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                //DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                // Timestamp
                // ConnectorStatus
                // EVSEId
                // ConnectorId

                return Task.FromResult(
                           new StatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnStopTransaction

            OCPP.IN.OnStopTransaction += (timestamp,
                                          sender,
                                          connection,
                                          request,
                                          cancellationToken) => {

                // TransactionId
                // StopTimestamp
                // MeterStop
                // IdTag
                // Reason
                // TransactionData

                return Task.FromResult(
                           new StopTransactionResponse(
                               Request:      request,
                               IdTagInfo:    null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Custom

            #region OnDataTransfer

            OCPP.IN.OnDataTransfer += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       cancellationToken) => {

                var responseData = request.Data;

                if (request.Data is not null)
                {

                    if      (request.Data.Type == JTokenType.String)
                        responseData = request.Data.ToString().Reverse();

                    else if (request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                   property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }


                var response =  request.VendorId == Vendor_Id.GraphDefined

                                    ? new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Accepted,
                                          Data:          responseData,
                                          CustomData:    null
                                      )

                                    : new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Rejected,
                                          Data:          null,
                                          CustomData:    null
                                      );


                return Task.FromResult(response);

            };

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            this.OCPP.IN.OnBootNotification += async (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      cancellationToken) => {

                // IdTag

                return new BootNotificationResponse(
                    Request:             request,
                    Status:              RegistrationStatus.Accepted,
                    CurrentTime:         Timestamp.Now,
                    HeartbeatInterval:   TimeSpan.FromSeconds(30),
                    CustomData:          null
                );

                if (request.ChargeBoxSerialNumber.IsNullOrEmpty() ||
                    !ChargeBox_Id.TryParse(request.ChargeBoxSerialNumber, out var chargeBoxSerialNumber))
                {
                    return new BootNotificationResponse(
                               request,
                               RegistrationStatus.Rejected,
                               Timestamp.Now,
                               TimeSpan.FromSeconds(30)
                           );
                }

                if (chargeBoxes.TryGetValue(chargeBoxSerialNumber, out var chargeBoxAccess))
                {

                    if (chargeBoxAccess.ChargeBoxAccessType == ChargeBoxAccessTypes.Denied)
                        return new BootNotificationResponse(
                                   Request:             request,
                                   Status:              RegistrationStatus.Rejected,
                                   CurrentTime:         Timestamp.Now,
                                   HeartbeatInterval:   TimeSpan.FromMinutes(5),
                                   CustomData:          null
                               );

                    if (chargeBoxAccess.ChargeBoxAccessType == ChargeBoxAccessTypes.Pending ||
                        chargeBoxAccess.ChargeBoxAccessType == ChargeBoxAccessTypes.Allowed)
                    {

                        if (request.ChargePointVendor != chargeBoxAccess.ChargeBox?.ChargePointVendor ||
                            request.ChargePointModel  != chargeBoxAccess.ChargeBox?.ChargePointModel)
                        {

                            var chargeBoxBuilder = chargeBoxAccess.ChargeBox?.ToBuilder()
                                                       ?? new ChargeBox.Builder(chargeBoxSerialNumber);

                            chargeBoxBuilder.ChargePointVendor        = request.ChargePointVendor;
                            chargeBoxBuilder.ChargePointModel         = request.ChargePointModel;
                            chargeBoxBuilder.ChargePointSerialNumber  = request.ChargePointSerialNumber;
                            chargeBoxBuilder.ChargeBoxSerialNumber    = request.ChargeBoxSerialNumber;
                            chargeBoxBuilder.FirmwareVersion          = request.FirmwareVersion;
                            chargeBoxBuilder.Iccid                    = request.Iccid;
                            chargeBoxBuilder.IMSI                     = request.IMSI;

                            // MeterType
                            // MeterSerialNumber

                            await AddOrUpdateChargeBox(
                                      chargeBoxBuilder,
                                      chargeBoxAccess.ChargeBoxAccessType
                                  );

                        }

                    }

                    if (chargeBoxAccess.ChargeBoxAccessType == ChargeBoxAccessTypes.Pending)
                        return new BootNotificationResponse(
                                   Request:             request,
                                   Status:              RegistrationStatus.Pending,
                                   CurrentTime:         Timestamp.Now,
                                   HeartbeatInterval:   TimeSpan.FromSeconds(30),
                                   CustomData:          null
                               );

                    if (chargeBoxAccess.ChargeBoxAccessType == ChargeBoxAccessTypes.Allowed)
                        return new BootNotificationResponse(
                                   Request:             request,
                                   Status:              RegistrationStatus.Accepted,
                                   CurrentTime:         Timestamp.Now,
                                   HeartbeatInterval:   TimeSpan.FromSeconds(30),
                                   CustomData:          null
                               );

                }

                #region Auto-Create unknown ChargeBoxes?

                if (AutoCreatedChargeBoxesAccessType.HasValue)
                {

                    var chargeBoxBuilder = new ChargeBox.Builder(chargeBoxSerialNumber) {

                                               ChargePointVendor        = request.ChargePointVendor,
                                               ChargePointModel         = request.ChargePointModel,
                                               ChargePointSerialNumber  = request.ChargePointSerialNumber,
                                               ChargeBoxSerialNumber    = request.ChargeBoxSerialNumber,
                                               FirmwareVersion          = request.FirmwareVersion,
                                               Iccid                    = request.Iccid,
                                               IMSI                     = request.IMSI

                                               // MeterType
                                               // MeterSerialNumber

                                           };

                    var addChargeBoxResult = await AddChargeBox(
                                                       chargeBoxBuilder,
                                                       AutoCreatedChargeBoxesAccessType.Value
                                                   );

                    if (addChargeBoxResult.Result == CommandResult.Success)
                        return new BootNotificationResponse(
                                   Request:             request,
                                   Status:              AutoCreatedChargeBoxesAccessType.Value switch {
                                                            ChargeBoxAccessTypes.Allowed  => RegistrationStatus.Accepted,
                                                            ChargeBoxAccessTypes.Pending  => RegistrationStatus.Pending,
                                                            _                             => RegistrationStatus.Rejected
                                                        },
                                   CurrentTime:         Timestamp.Now,
                                   HeartbeatInterval:   TimeSpan.FromSeconds(30),
                                   CustomData:          null
                               );

                }

                #endregion

                return new BootNotificationResponse(
                           Request:             request,
                           Status:              RegistrationStatus.Rejected,
                           CurrentTime:         Timestamp.Now,
                           HeartbeatInterval:   TimeSpan.FromSeconds(30),
                           CustomData:          null
                       );

            };

            #endregion

            #region OnFirmwareStatusNotification

            OCPP.IN.OnFirmwareStatusNotification += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                // Status
                // UpdateFirmwareRequestId

                return Task.FromResult(
                           new FirmwareStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnHeartbeat

            OCPP.IN.OnHeartbeat += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    cancellationToken) => {

                return Task.FromResult(
                           new HeartbeatResponse(
                               Request:       request,
                               CurrentTime:   Timestamp.Now,
                               CustomData:    null
                           )
                       );

            };

            #endregion

            #region OnSignedFirmwareStatusNotification

            OCPP.IN.OnSignedFirmwareStatusNotification += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                // Status

                return Task.FromResult(
                           new SignedFirmwareStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Monitoring

            #region OnDiagnosticsStatusNotification

            this.OCPP.IN.OnDiagnosticsStatusNotification += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {


                DebugX.Log("OnDiagnosticsStatusNotification: " + request.DestinationId);

                // Status

                return Task.FromResult(
                           new DiagnosticsStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnLogStatusNotification

            this.OCPP.IN.OnLogStatusNotification += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {


                DebugX.Log("OnLogStatusNotification: " + request.DestinationId);

                // Status
                // LogRquestId

                return Task.FromResult(
                           new LogStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSecurityEventNotification

            OCPP.IN.OnSecurityEventNotification += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                                                        DebugX.Log("OnSecurityEventNotification: " + request.DestinationId);

                                                        // Type
                                                        // Timestamp
                                                        // TechInfo

                                                        return Task.FromResult(
                                                                   new SecurityEventNotificationResponse(
                                                                       Request: request,
                                                                       CustomData: null
                                                                   )
                                                               );

                                                    };

            #endregion

            #endregion

        }

        #endregion


        #region AttachSOAPService(...)

        ///// <summary>
        ///// Create a new central system for testing using HTTP/SOAP.
        ///// </summary>
        ///// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        ///// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        ///// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        ///// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        ///// <param name="ContentType">An optional HTTP content type to use.</param>
        ///// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        ///// <param name="DNSClient">An optional DNS client to use.</param>
        ///// <param name="AutoStart">Start the server immediately.</param>
        //public CentralSystemSOAPServer AttachSOAPService(String            HTTPServerName            = CentralSystemSOAPServer.DefaultHTTPServerName,
        //                                                 IPPort?           TCPPort                   = null,
        //                                                 String?           ServiceName               = null,
        //                                                 HTTPPath?         URLPrefix                 = null,
        //                                                 HTTPContentType?  ContentType               = null,
        //                                                 Boolean           RegisterHTTPRootService   = true,
        //                                                 DNSClient?        DNSClient                 = null,
        //                                                 Boolean           AutoStart                 = false)
        //{

        //    var centralSystemServer = new CentralSystemSOAPServer(
        //                                  HTTPServerName,
        //                                  TCPPort,
        //                                  ServiceName,
        //                                  URLPrefix,
        //                                  ContentType,
        //                                  RegisterHTTPRootService,
        //                                  DNSClient ?? this.DNSClient,
        //                                  false
        //                              );

        //    //Attach(centralSystemServer);

        //    if (AutoStart)
        //        centralSystemServer.Start();

        //    return centralSystemServer;

        //}

        #endregion

        #region AttachWebSocketService(...)

        ///// <summary>
        ///// Create a new central system for testing using HTTP/WebSocket.
        ///// </summary>
        ///// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        ///// <param name="IPAddress">An IP address to listen on.</param>
        ///// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        ///// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        ///// 
        ///// <param name="DNSClient">An optional DNS client to use.</param>
        ///// <param name="AutoStart">Start the server immediately.</param>
        //public CentralSystemWSServer AttachWebSocketService(String       HTTPServerName               = CentralSystemWSServer.DefaultHTTPServiceName,
        //                                                    IIPAddress?  IPAddress                    = null,
        //                                                    IPPort?      TCPPort                      = null,
        //                                                    I18NString?  Description                  = null,

        //                                                    Boolean      DisableWebSocketPings        = false,
        //                                                    TimeSpan?    WebSocketPingEvery           = null,
        //                                                    TimeSpan?    SlowNetworkSimulationDelay   = null,

        //                                                    DNSClient?   DNSClient                    = null,
        //                                                    Boolean      AutoStart                    = false)
        //{

        //    var centralSystemServer = new CentralSystemWSServer(

        //                                  NetworkingNode_Id.Parse(Id.ToString()),

        //                                  HTTPServerName,
        //                                  IPAddress,
        //                                  TCPPort,
        //                                  Description,

        //                                  RequireAuthentication,
        //                                  DisableWebSocketPings,
        //                                  WebSocketPingEvery,
        //                                  SlowNetworkSimulationDelay,

        //                                  DNSClient: DNSClient ?? this.DNSClient,
        //                                  AutoStart: false

        //                              );

        //    #region WebSocket related

        //    #region OnServerStarted

        //    centralSystemServer.OnServerStarted += async (timestamp,
        //                                                  webSocketServer,
        //                                                  eventTrackingId,
        //                                                  cancellationToken) => {

        //        var onServerStarted = OnServerStarted;
        //        if (onServerStarted is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onServerStarted.GetInvocationList().
        //                                       OfType <OnServerStartedDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      eventTrackingId,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnServerStarted),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnNewTCPConnection

        //    centralSystemServer.OnNewTCPConnection += async (timestamp,
        //                                                     webSocketServer,
        //                                                     newTCPConnection,
        //                                                     eventTrackingId,
        //                                                     cancellationToken) => {

        //        var onNewTCPConnection = OnNewTCPConnection;
        //        if (onNewTCPConnection is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onNewTCPConnection.GetInvocationList().
        //                                       OfType <OnNewTCPConnectionDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      newTCPConnection,
        //                                                                      eventTrackingId,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnNewTCPConnection),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    // Failed (Charging Station) Authentication

        //    #region OnNewWebSocketConnection

        //    centralSystemServer.OnCSMSNewWebSocketConnection += async (timestamp,
        //                                                               csmsChannel,
        //                                                               newConnection,
        //                                                               networkingNodeId,
        //                                                               eventTrackingId,
        //                                                               sharedSubprotocols,
        //                                                               cancellationToken) => {

        //        // A new connection from the same networking node/charge box will replace the older one!
        //        if (!reachableChargeBoxes.TryAdd(networkingNodeId, new Tuple<ICentralSystemChannel, DateTime>(csmsChannel as OCPPv1_6.CS.ICentralSystemChannel, timestamp)))
        //             reachableChargeBoxes[networkingNodeId]      = new Tuple<ICentralSystemChannel, DateTime>(csmsChannel as OCPPv1_6.CS.ICentralSystemChannel, timestamp);


        //        var onNewWebSocketConnection = OnNewWebSocketConnection;
        //        if (onNewWebSocketConnection is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onNewWebSocketConnection.GetInvocationList().
        //                                       OfType <OnCSMSNewWebSocketConnectionDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      csmsChannel,
        //                                                                      newConnection,
        //                                                                      networkingNodeId,
        //                                                                      eventTrackingId,
        //                                                                      sharedSubprotocols,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnNewWebSocketConnection),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnCloseMessageReceived

        //    centralSystemServer.OnCSMSCloseMessageReceived += async (timestamp,
        //                                                             csmsChannel,
        //                                                             connection,
        //                                                             networkingNodeId,
        //                                                             eventTrackingId,
        //                                                             statusCode,
        //                                                             reason,
        //                                                             cancellationToken) => {

        //        var onCloseMessageReceived = OnCloseMessageReceived;
        //        if (onCloseMessageReceived is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onCloseMessageReceived.GetInvocationList().
        //                                       OfType <OnCSMSCloseMessageReceivedDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      csmsChannel,
        //                                                                      connection,
        //                                                                      networkingNodeId,
        //                                                                      eventTrackingId,
        //                                                                      statusCode,
        //                                                                      reason,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnCloseMessageReceived),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnTCPConnectionClosed

        //    centralSystemServer.OnCSMSTCPConnectionClosed += async (timestamp,
        //                                                            csmsChannel,
        //                                                            connection,
        //                                                            networkingNodeId,
        //                                                            eventTrackingId,
        //                                                            reason,
        //                                                            cancellationToken) => {

        //        var onTCPConnectionClosed = OnTCPConnectionClosed;
        //        if (onTCPConnectionClosed is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onTCPConnectionClosed.GetInvocationList().
        //                                       OfType <OnCSMSTCPConnectionClosedDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      centralSystemServer,
        //                                                                      connection,
        //                                                                      networkingNodeId,
        //                                                                      eventTrackingId,
        //                                                                      reason,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnTCPConnectionClosed),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnServerStopped

        //    centralSystemServer.OnServerStopped += async (timestamp,
        //                                                  server,
        //                                                  eventTrackingId,
        //                                                  reason,
        //                                                  cancellationToken) => {

        //        var onServerStopped = OnServerStopped;
        //        if (onServerStopped is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onServerStopped.GetInvocationList().
        //                                         OfType <OnServerStoppedDelegate>().
        //                                         Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                        timestamp,
        //                                                                        server,
        //                                                                        eventTrackingId,
        //                                                                        reason,
        //                                                                        cancellationToken
        //                                                                    )).
        //                                         ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnServerStopped),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    // (Generic) Error Handling

        //    #endregion


        //    #region OnJSONMessageRequestReceived

        //    centralSystemServer.OnJSONMessageRequestReceived += async (timestamp,
        //                                                               webSocketServer,
        //                                                               webSocketConnection,
        //                                                               networkingNodeId,
        //                                                               networkPath,
        //                                                               eventTrackingId,
        //                                                               requestTimestamp,
        //                                                               requestMessage,
        //                                                               cancellationToken) => {

        //        var onJSONMessageRequestReceived = OnJSONMessageRequestReceived;
        //        if (onJSONMessageRequestReceived is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONMessageRequestReceived.GetInvocationList().
        //                                       OfType <OnWebSocketJSONMessageRequestDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      networkingNodeId,
        //                                                                      networkPath,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      requestMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONMessageRequestReceived),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnJSONMessageResponseSent

        //    centralSystemServer.OnJSONMessageResponseSent += async (timestamp,
        //                                                            webSocketServer,
        //                                                            webSocketConnection,
        //                                                            networkingNodeId,
        //                                                            networkPath,
        //                                                            eventTrackingId,
        //                                                            requestTimestamp,
        //                                                            jsonRequestMessage,
        //                                                            binaryRequestMessage,
        //                                                            responseTimestamp,
        //                                                            responseMessage,
        //                                                            cancellationToken) => {

        //        var onJSONMessageResponseSent = OnJSONMessageResponseSent;
        //        if (onJSONMessageResponseSent is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
        //                                       OfType <OnWebSocketJSONMessageResponseDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      networkingNodeId,
        //                                                                      networkPath,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      jsonRequestMessage,
        //                                                                      binaryRequestMessage,
        //                                                                      responseTimestamp,
        //                                                                      responseMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONMessageResponseSent),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnJSONErrorResponseSent

        //    centralSystemServer.OnJSONErrorResponseSent += async (timestamp,
        //                                                          webSocketServer,
        //                                                          webSocketConnection,
        //                                                          eventTrackingId,
        //                                                          requestTimestamp,
        //                                                          jsonRequestMessage,
        //                                                          binaryRequestMessage,
        //                                                          responseTimestamp,
        //                                                          responseMessage,
        //                                                          cancellationToken) => {

        //        var onJSONErrorResponseSent = OnJSONErrorResponseSent;
        //        if (onJSONErrorResponseSent is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONErrorResponseSent.GetInvocationList().
        //                                       OfType <OnWebSocketTextErrorResponseDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      jsonRequestMessage,
        //                                                                      binaryRequestMessage,
        //                                                                      responseTimestamp,
        //                                                                      responseMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONErrorResponseSent),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion


        //    #region OnJSONMessageRequestSent

        //    centralSystemServer.OnJSONMessageRequestSent += async (timestamp,
        //                                                           webSocketServer,
        //                                                           webSocketConnection,
        //                                                           networkingNodeId,
        //                                                           networkPath,
        //                                                           eventTrackingId,
        //                                                           requestTimestamp,
        //                                                           requestMessage,
        //                                                           cancellationToken) => {

        //        var onJSONMessageRequestSent = OnJSONMessageRequestSent;
        //        if (onJSONMessageRequestSent is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONMessageRequestSent.GetInvocationList().
        //                                       OfType <OnWebSocketJSONMessageRequestDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      networkingNodeId,
        //                                                                      networkPath,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      requestMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONMessageRequestSent),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnJSONMessageResponseReceived

        //    centralSystemServer.OnJSONMessageResponseReceived += async (timestamp,
        //                                                                webSocketServer,
        //                                                                webSocketConnection,
        //                                                                networkingNodeId,
        //                                                                networkPath,
        //                                                                eventTrackingId,
        //                                                                requestTimestamp,
        //                                                                jsonRequestMessage,
        //                                                                binaryRequestMessage,
        //                                                                responseTimestamp,
        //                                                                responseMessage,
        //                                                                cancellationToken) => {

        //        var onJSONMessageResponseReceived = OnJSONMessageResponseReceived;
        //        if (onJSONMessageResponseReceived is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONMessageResponseReceived.GetInvocationList().
        //                                       OfType <OnWebSocketJSONMessageResponseDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      networkingNodeId,
        //                                                                      networkPath,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      jsonRequestMessage,
        //                                                                      binaryRequestMessage,
        //                                                                      responseTimestamp,
        //                                                                      responseMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONMessageResponseReceived),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion

        //    #region OnJSONErrorResponseReceived

        //    centralSystemServer.OnJSONErrorResponseReceived += async (timestamp,
        //                                                              webSocketServer,
        //                                                              webSocketConnection,
        //                                                              eventTrackingId,
        //                                                              requestTimestamp,
        //                                                              jsonRequestMessage,
        //                                                              binaryRequestMessage,
        //                                                              responseTimestamp,
        //                                                              responseMessage,
        //                                                              cancellationToken) => {

        //        var onJSONErrorResponseReceived = OnJSONErrorResponseReceived;
        //        if (onJSONErrorResponseReceived is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onJSONErrorResponseReceived.GetInvocationList().
        //                                       OfType <OnWebSocketTextErrorResponseDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      timestamp,
        //                                                                      webSocketServer,
        //                                                                      webSocketConnection,
        //                                                                      eventTrackingId,
        //                                                                      requestTimestamp,
        //                                                                      jsonRequestMessage,
        //                                                                      binaryRequestMessage,
        //                                                                      responseTimestamp,
        //                                                                      responseMessage,
        //                                                                      cancellationToken
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnJSONErrorResponseReceived),
        //                          e
        //                      );
        //            }
        //        }

        //    };

        //    #endregion


        //    Attach(centralSystemServer);


        //    if (AutoStart)
        //        centralSystemServer.Start();

        //    return centralSystemServer;

        //}

        #endregion

        #region Attach(CentralSystemServer)

        //public void Attach(ICentralSystemChannel CentralSystemServer)
        //{

        //    centralSystemServers.Add(CentralSystemServer);


        //    // Wire events...

        //    #region OnBootNotification

        //    CentralSystemServer.OnBootNotification += async (LogTimestamp,
        //                                                     sender,
        //                                                     connection,
        //                                                     request,
        //                                                     cancellationToken) => {

        //        #region Send OnBootNotificationRequest event

        //        var startTime      = Timestamp.Now;

        //        var requestLogger  = OnBootNotificationRequest;
        //        if (requestLogger is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(requestLogger.GetInvocationList().
        //                                       OfType <OnBootNotificationRequestDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      startTime,
        //                                                                      this,
        //                                                                      connection,
        //                                                                      request
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnBootNotificationRequest),
        //                          e
        //                      );
        //            }

        //        }

        //        #endregion


        //        Console.WriteLine("OnBootNotification: " + request.DestinationId             + ", " +
        //                                                   request.ChargePointVendor       + ", " +
        //                                                   request.ChargePointModel        + ", " +
        //                                                   request.ChargePointSerialNumber + ", " +
        //                                                   request.ChargeBoxSerialNumber);


        //        //await AddChargeBoxIfNotExists(new ChargeBox(request.NetworkingNodeId,
        //        //                                            1,
        //        //                                            request.ChargePointVendor,
        //        //                                            request.ChargePointModel,
        //        //                                            null,
        //        //                                            request.ChargePointSerialNumber,
        //        //                                            request.ChargeBoxSerialNumber,
        //        //                                            request.FirmwareVersion,
        //        //                                            request.Iccid,
        //        //                                            request.IMSI,
        //        //                                            request.MeterType,
        //        //                                            request.MeterSerialNumber));


        //        await Task.Delay(100, cancellationToken);


        //        var response = new BootNotificationResponse(Request:            request,
        //                                                    Status:             RegistrationStatus.Accepted,
        //                                                    CurrentTime:        Timestamp.Now,
        //                                                    HeartbeatInterval:  TimeSpan.FromMinutes(5));


        //        #region Send OnBootNotificationResponse event

        //        var responseLogger = OnBootNotificationResponse;
        //        if (responseLogger is not null)
        //        {
        //            try
        //            {

        //                var responseTime = Timestamp.Now;

        //                await Task.WhenAll(responseLogger.GetInvocationList().
        //                                       OfType <OnBootNotificationResponseDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(
        //                                                                      responseTime,
        //                                                                      this,
        //                                                                      connection,
        //                                                                      request,
        //                                                                      response,
        //                                                                      responseTime - startTime
        //                                                                  )).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestCentralSystem),
        //                          nameof(OnBootNotificationResponse),
        //                          e
        //                      );
        //            }

        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnHeartbeat

        //    CentralSystemServer.OnHeartbeat += async (LogTimestamp,
        //                                              Sender,
        //                                              connection,
        //                                              Request,
        //                                              CancellationToken) => {

        //        #region Send OnHeartbeatRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnHeartbeatRequest?.Invoke(startTime,
        //                                       this,
        //                                       connection,
        //                                       Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnHeartbeat: " + Request.DestinationId);


        //        await Task.Delay(100, CancellationToken);


        //        var response = new HeartbeatResponse(Request:      Request,
        //                                             CurrentTime:  Timestamp.Now);


        //        #region Send OnHeartbeatResponse event

        //        try
        //        {

        //            OnHeartbeatResponse?.Invoke(Timestamp.Now,
        //                                        this,
        //                                        connection,
        //                                        Request,
        //                                        response,
        //                                        Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnDiagnosticsStatusNotification

        //    CentralSystemServer.OnDiagnosticsStatusNotification += async (LogTimestamp,
        //                                                                  Sender,
        //                                                                  connection,
        //                                                                  Request,
        //                                                                  CancellationToken) => {

        //        #region Send OnDiagnosticsStatusNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
        //                                                           this,
        //                                                           connection,
        //                                                           Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnDiagnosticsStatusNotification: " + Request.Status);


        //        await Task.Delay(100, CancellationToken);

        //        var response = new DiagnosticsStatusNotificationResponse(Request);


        //        #region Send OnDiagnosticsStatusResponse event

        //        try
        //        {

        //            OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
        //                                                            this,
        //                                                            connection,
        //                                                            Request,
        //                                                            response,
        //                                                            Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnFirmwareStatusNotification

        //    CentralSystemServer.OnFirmwareStatusNotification += async (LogTimestamp,
        //                                                               Sender,
        //                                                               connection,
        //                                                               Request,
        //                                                               CancellationToken) => {

        //        #region Send OnFirmwareStatusNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnFirmwareStatusNotificationRequest?.Invoke(startTime,
        //                                                        this,
        //                                                        connection,
        //                                                        Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnFirmwareStatus: " + Request.Status);

        //        await Task.Delay(100, CancellationToken);

        //        var response = new FirmwareStatusNotificationResponse(Request);


        //        #region Send OnFirmwareStatusResponse event

        //        try
        //        {

        //            OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
        //                                                         this,
        //                                                         connection,
        //                                                         Request,
        //                                                         response,
        //                                                         Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        #region OnAuthorize

        //     CentralSystemServer.OnAuthorize

        #endregion

        //    #region OnStartTransaction

        //    CentralSystemServer.OnStartTransaction += async (LogTimestamp,
        //                                                     Sender,
        //                                                     connection,
        //                                                     Request,
        //                                                     CancellationToken) => {

        //        #region Send OnStartTransactionRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnStartTransactionRequest?.Invoke(startTime,
        //                                              this,
        //                                              connection,
        //                                              Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnStartTransaction: " + Request.DestinationId + ", " +
        //                                                   Request.ConnectorId + ", " +
        //                                                   Request.IdTag + ", " +
        //                                                   Request.StartTimestamp + ", " +
        //                                                   Request.MeterStart + ", " +
        //                                                   Request.ReservationId ?? "-");

        //        await Task.Delay(100, CancellationToken);

        //        var response = new StartTransactionResponse(Request:        Request,
        //                                                    TransactionId:  Transaction_Id.NewRandom,
        //                                                    IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
        //                                                                                  ExpiryDate:  Timestamp.Now.AddDays(3)));

        //        var key = Request.DestinationId + "*" + Request.ConnectorId;

        //        if (TransactionIds.ContainsKey(key))
        //            TransactionIds[key] = response.TransactionId;
        //        else
        //            TransactionIds.Add(key, response.TransactionId);


        //        #region Send OnStartTransactionResponse event

        //        try
        //        {

        //            OnStartTransactionResponse?.Invoke(Timestamp.Now,
        //                                               this,
        //                                               connection,
        //                                               Request,
        //                                               response,
        //                                               Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnStatusNotification

        //    CentralSystemServer.OnStatusNotification += async (LogTimestamp,
        //                                                       Sender,
        //                                                       connection,
        //                                                       Request,
        //                                                       CancellationToken) => {

        //        #region Send OnStatusNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnStatusNotificationRequest?.Invoke(startTime,
        //                                                this,
        //                                                connection,
        //                                                Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnStatusNotification: " + Request.ConnectorId     + ", " +
        //                                                     Request.Status          + ", " +
        //                                                     Request.ErrorCode       + ", " +
        //                                                     Request.Info            + ", " +
        //                                                     Request.StatusTimestamp + ", " +
        //                                                     Request.VendorId        + ", " +
        //                                                     Request.VendorErrorCode);


        //        await Task.Delay(100, CancellationToken);

        //        var response = new StatusNotificationResponse(Request);


        //        #region Send OnStatusNotificationResponse event

        //        try
        //        {

        //            OnStatusNotificationResponse?.Invoke(Timestamp.Now,
        //                                                 this,
        //                                                 connection,
        //                                                 Request,
        //                                                 response,
        //                                                 Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnMeterValues

        //    CentralSystemServer.OnMeterValues += async (LogTimestamp,
        //                                                Sender,
        //                                                connection,
        //                                                Request,
        //                                                CancellationToken) => {

        //        #region Send OnMeterValuesRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnMeterValuesRequest?.Invoke(startTime,
        //                                         this,
        //                                         connection,
        //                                         Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesRequest));
        //        }

        //                                                    #endregion


        //        Console.WriteLine("OnMeterValues: " + Request.ConnectorId + ", " +
        //                                              Request.TransactionId);

        //        Console.WriteLine(Request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToISO8601() +
        //                                  meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));


        //        await Task.Delay(100, CancellationToken);

        //        var response = new MeterValuesResponse(Request);


        //        #region Send OnMeterValuesResponse event

        //        try
        //        {

        //            OnMeterValuesResponse?.Invoke(Timestamp.Now,
        //                                          this,
        //                                          connection,
        //                                          Request,
        //                                          response,
        //                                          Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnStopTransaction

        //    CentralSystemServer.OnStopTransaction += async (LogTimestamp,
        //                                                    Sender,
        //                                                    connection,
        //                                                    Request,
        //                                                    CancellationToken) => {

        //        #region Send OnStopTransactionRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnStopTransactionRequest?.Invoke(startTime,
        //                                             this,
        //                                             connection,
        //                                             Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnStopTransaction: " + Request.TransactionId + ", " +
        //                                                  Request.IdTag + ", " +
        //                                                  Request.StopTimestamp.ToISO8601() + ", " +
        //                                                  Request.MeterStop + ", " +
        //                                                  Request.Reason);

        //        Console.WriteLine(Request.TransactionData.SafeSelect(transactionData => transactionData.Timestamp.ToISO8601() +
        //                                  transactionData.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

        //        await Task.Delay(100, CancellationToken);

        //        var response = new StopTransactionResponse(Request:    Request,
        //                                                   IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
        //                                                                             ExpiryDate:  Timestamp.Now.AddDays(3)));

        //        var kvp = TransactionIds.Where(trid => trid.Value == Request.TransactionId).ToArray();
        //        if (kvp.SafeAny())
        //            TransactionIds.Remove(kvp.First().Key);


        //        #region Send OnStopTransactionResponse event

        //        try
        //        {

        //            OnStopTransactionResponse?.Invoke(Timestamp.Now,
        //                                              this,
        //                                              connection,
        //                                              Request,
        //                                              response,
        //                                              Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    #region OnIncomingDataTransfer

        //    CentralSystemServer.OnIncomingDataTransfer += async (LogTimestamp,
        //                                                         Sender,
        //                                                         connection,
        //                                                         request,
        //                                                         CancellationToken) => {

        //        #region Send OnIncomingDataRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnIncomingDataTransferRequest?.Invoke(startTime,
        //                                                  this,
        //                                                  connection,
        //                                                  request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferRequest));
        //        }

        //        #endregion


        //        // VendorId
        //        // MessageId
        //        // Data

        //        DebugX.Log("OnIncomingDataTransfer: " + request.VendorId  + ", " +
        //                                                request.MessageId + ", " +
        //                                                request.Data);


        //        var responseData = request.Data;

        //        if (request.Data is not null)
        //        {

        //            if      (request.Data.Type == JTokenType.String)
        //                responseData = request.Data.ToString().Reverse();

        //            else if (request.Data.Type == JTokenType.Object) {

        //                var responseObject = new JObject();

        //                foreach (var property in (request.Data as JObject)!)
        //                {
        //                    if (property.Value?.Type == JTokenType.String)
        //                        responseObject.Add(property.Key,
        //                                           property.Value.ToString().Reverse());
        //                }

        //                responseData = responseObject;

        //            }

        //            else if (request.Data.Type == JTokenType.Array) {

        //                var responseArray = new JArray();

        //                foreach (var element in (request.Data as JArray)!)
        //                {
        //                    if (element?.Type == JTokenType.String)
        //                        responseArray.Add(element.ToString().Reverse());
        //                }

        //                responseData = responseArray;

        //            }

        //        }


        //        var response = !SignaturePolicy.VerifyRequestMessage(
        //                           request,
        //                           request.ToJSON(
        //                               CustomIncomingDataTransferRequestSerializer,
        //                               CustomSignatureSerializer,
        //                               CustomCustomDataSerializer
        //                           ),
        //                           out var errorResponse
        //                       )

        //                           ? new DataTransferResponse(
        //                                 Request:      request,
        //                                 Result:       Result.SignatureError(
        //                                                   $"Invalid signature(s): {errorResponse}"
        //                                               )
        //                             )

        //                           : request.VendorId == Vendor_Id.GraphDefined

        //                                 ? new (
        //                                       Request:      request,
        //                                       Status:       DataTransferStatus.Accepted,
        //                                       Data:         responseData,
        //                                       StatusInfo:   null,
        //                                       CustomData:   null
        //                                   )

        //                                 : new DataTransferResponse(
        //                                       Request:      request,
        //                                       Status:       DataTransferStatus.Rejected,
        //                                       Data:         null,
        //                                       StatusInfo:   null,
        //                                       CustomData:   null
        //                                 );

        //        SignaturePolicy.SignResponseMessage(
        //            response,
        //            response.ToJSON(
        //                CustomIncomingDataTransferResponseSerializer,
        //                null,
        //                CustomSignatureSerializer,
        //                CustomCustomDataSerializer
        //            ),
        //            out var errorResponse2);


        //        #region Send OnIncomingDataResponse event

        //        try
        //        {

        //            OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
        //                                                   this,
        //                                                   connection,
        //                                                   request,
        //                                                   response,
        //                                                   Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    // Security extensions

        //    #region OnSecurityEventNotification

        //    CentralSystemServer.OnSecurityEventNotification += async (LogTimestamp,
        //                                                               Sender,
        //                                                               connection,
        //                                                               Request,
        //                                                               CancellationToken) => {

        //        #region Send OnSecurityEventNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnSecurityEventNotificationRequest?.Invoke(startTime,
        //                                                        this,
        //                                                        connection,
        //                                                        Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecurityEventNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnSecurityEventNotification: " );

        //        await Task.Delay(100, CancellationToken);

        //        var response = new SecurityEventNotificationResponse(Request);


        //        #region Send OnFirmwareStatusResponse event

        //        try
        //        {

        //            OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
        //                                                         this,
        //                                                         connection,
        //                                                         Request,
        //                                                         response,
        //                                                         Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecurityEventNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnLogStatusNotification

        //    CentralSystemServer.OnLogStatusNotification += async (LogTimestamp,
        //                                                               Sender,
        //                                                               connection,
        //                                                               Request,
        //                                                               CancellationToken) => {

        //        #region Send OnLogStatusNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnLogStatusNotificationRequest?.Invoke(startTime,
        //                                                        this,
        //                                                        connection,
        //                                                        Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnLogStatusNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnLogStatusNotification: " + Request.Status);

        //        await Task.Delay(100, CancellationToken);

        //        var response = new LogStatusNotificationResponse(Request);


        //        #region Send OnFirmwareStatusResponse event

        //        try
        //        {

        //            OnLogStatusNotificationResponse?.Invoke(Timestamp.Now,
        //                                                         this,
        //                                                         connection,
        //                                                         Request,
        //                                                         response,
        //                                                         Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnLogStatusNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnSignCertificate

        //    CentralSystemServer.OnSignCertificate += async (LogTimestamp,
        //                                                               Sender,
        //                                                               connection,
        //                                                               Request,
        //                                                               CancellationToken) => {

        //        #region Send OnSignCertificateRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnSignCertificateRequest?.Invoke(startTime,
        //                                                        this,
        //                                                        connection,
        //                                                        Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignCertificateRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnSignCertificate: " );

        //        await Task.Delay(100, CancellationToken);

        //        var response = new SignCertificateResponse(Request, GenericStatus.Accepted);


        //        #region Send OnFirmwareStatusResponse event

        //        try
        //        {

        //            OnSignCertificateResponse?.Invoke(Timestamp.Now,
        //                                                         this,
        //                                                         connection,
        //                                                         Request,
        //                                                         response,
        //                                                         Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignCertificateResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnSignedFirmwareStatusNotification

        //    CentralSystemServer.OnSignedFirmwareStatusNotification += async (LogTimestamp,
        //                                                               Sender,
        //                                                               connection,
        //                                                               Request,
        //                                                               CancellationToken) => {

        //        #region Send OnSignedFirmwareStatusNotificationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnSignedFirmwareStatusNotificationRequest?.Invoke(startTime,
        //                                                        this,
        //                                                        connection,
        //                                                        Request);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedFirmwareStatusNotificationRequest));
        //        }

        //        #endregion


        //        Console.WriteLine("OnSignedFirmwareStatusNotification: " + Request.Status);

        //        await Task.Delay(100, CancellationToken);

        //        var response = new SignedFirmwareStatusNotificationResponse(Request);


        //        #region Send OnFirmwareStatusResponse event

        //        try
        //        {

        //            OnSignedFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
        //                                                         this,
        //                                                         connection,
        //                                                         Request,
        //                                                         response,
        //                                                         Timestamp.Now - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedFirmwareStatusNotificationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    // Binary Data Streams Extensions

        //    #region OnIncomingBinaryDataTransfer

        ////    CentralSystemServer.OnIncomingBinaryDataTransfer += async (timestamp,
        ////                                                               sender,
        ////                                                               connection,
        ////                                                               request,
        ////                                                               cancellationToken) => {

        ////        #region Send OnIncomingBinaryDataTransfer event

        ////        var startTime = Timestamp.Now;

        ////        try
        ////        {

        ////            OnBinaryDataTransferRequestReceived?.Invoke(startTime,
        ////                                                        this,
        ////                                                        connection,
        ////                                                        request);

        ////        }
        ////        catch (Exception e)
        ////        {
        ////            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferRequestReceived));
        ////        }

        ////        #endregion


        ////        // VendorId
        ////        // MessageId
        ////        // BinaryData

        ////        DebugX.Log("OnIncomingBinaryDataTransfer: " + request.VendorId  + ", " +
        ////                                                      request.MessageId + ", " +
        ////                                                      request.Data?.ToUTF8String() ?? "-");


        ////        var responseBinaryData = Array.Empty<byte>();

        ////        if (request.Data is not null)
        ////        {
        ////            responseBinaryData = ((Byte[]) request.Data.Clone()).Reverse();
        ////        }


        ////        var response = !SignaturePolicy.VerifyRequestMessage(
        ////                           request,
        ////                           request.ToBinary(
        ////                               CustomIncomingBinaryDataTransferRequestSerializer,
        ////                               CustomBinarySignatureSerializer,
        ////                               IncludeSignatures: false
        ////                           ),
        ////                           out var errorResponse
        ////                       )

        ////                           ? new BinaryDataTransferResponse(
        ////                                 Request:      request,
        ////                                 Result:       Result.SignatureError(
        ////                                                   $"Invalid signature(s): {errorResponse}"
        ////                                               )
        ////                             )

        ////                           : request.VendorId == Vendor_Id.GraphDefined

        ////                                 ? new (
        ////                                       Request:                request,
        ////                                       Status:                 BinaryDataTransferStatus.Accepted,
        ////                                       AdditionalStatusInfo:   null,
        ////                                       Data:                   responseBinaryData
        ////                                   )

        ////                                 : new BinaryDataTransferResponse(
        ////                                       Request:                request,
        ////                                       Status:                 BinaryDataTransferStatus.Rejected,
        ////                                       AdditionalStatusInfo:   null,
        ////                                       Data:                   responseBinaryData
        ////                                   );

        ////        SignaturePolicy.SignResponseMessage(
        ////            response,
        ////            response.ToBinary(
        ////                CustomIncomingBinaryDataTransferResponseSerializer,
        ////                null, //CustomStatusInfoSerializer,
        ////                CustomBinarySignatureSerializer,
        ////                IncludeSignatures: false
        ////            ),
        ////            out var errorResponse2);


        ////        #region Send OnIncomingBinaryDataTransferResponse event

        ////        try
        ////        {

        ////            OnBinaryDataTransferResponseSent?.Invoke(Timestamp.Now,
        ////                                                         this,
        ////                                                         connection,
        ////                                                         request,
        ////                                                         response,
        ////                                                         Timestamp.Now - startTime);

        ////        }
        ////        catch (Exception e)
        ////        {
        ////            DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferResponseSent));
        ////        }

        ////        #endregion

        ////        return response;

        ////    };

        //    #endregion

        //}

        #endregion


        #region AddHTTPBasicAuth(NetworkingNodeId, Password)

        ///// <summary>
        ///// Add the given HTTP Basic Authentication password for the given charge box.
        ///// </summary>
        ///// <param name="NetworkingNodeId">The unique identification of the charge box.</param>
        ///// <param name="Password">The password of the charge box.</param>
        //public void AddHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
        //                             String             Password)
        //{

        //    foreach (var centralSystemServer in centralSystemServers)
        //    {
        //        if (centralSystemServer is CentralSystemWSServer centralSystemWSServer)
        //        {

        //            //centralSystemWSServer.AddHTTPBasicAuth(NetworkingNodeId,
        //            //                                       Password);

        //        }
        //    }

        //}

        #endregion




        public void EnableLogging()
        {

            #region Charging

            #region OnAuthorizeResponseSent

            OCPP.OUT.OnAuthorizeResponseSent += static (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime,
                                                        sentMessageResult,
                                                        cancellationToken) => {
                                                            DebugX.Log($"OnAuthorize: {request.DestinationId}, {request.IdTag} => {response.IdTagInfo.Status}");
                                                            return Task.CompletedTask;
                                                        };

            #endregion

            #region OnMeterValues

            // ConnectorId
            // MeterValues
            // TransactionId

            OCPP.OUT.OnMeterValuesResponseSent += static (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          sentMessageResult,
                                                          cancellationToken) => {
                                                              var meterValue = request.MeterValues.FirstOrDefault();
                                                              DebugX.Log($"OnMeterValues: {request.NetworkPath.Source} {request.ConnectorId}{(request.TransactionId.HasValue ? request.TransactionId : "")} {request.MeterValues.Count()} meter value(s), {(meterValue is not null ? $"{meterValue.SampledValues.FirstOrDefault()?.Value ?? ""} kWh @{meterValue.Timestamp.ToISO8601()}" : "-")} => {response.Result}");

                                                              //DebugX.Log(request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToISO8601() +
                                                              //                                                        meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                                                              return Task.CompletedTask;
                                                          };

            #endregion

            #region OnStartTransaction

            OCPP.OUT.OnStartTransactionResponseSent += static (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               cancellationToken) => {

                                                                   // ConnectorId
                                                                   // IdTag
                                                                   // StartTimestamp
                                                                   // MeterStart
                                                                   // ReservationId

                                                                   //DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                                                                   return Task.CompletedTask;
                                                               };

            #endregion

            #region OnStatusNotification

            OCPP.OUT.OnStatusNotificationResponseSent += static (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 cancellationToken) => {

                                                                     // Timestamp
                                                                     // ConnectorStatus
                                                                     // EVSEId
                                                                     // ConnectorId

                                                                     //DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                                                                     return Task.CompletedTask;
                                                                 };

            #endregion

            #region OnStopTransaction

            OCPP.OUT.OnStopTransactionResponseSent += static (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              cancellationToken) => {

                                                                  // TransactionId
                                                                  // StopTimestamp
                                                                  // MeterStop
                                                                  // IdTag
                                                                  // Reason
                                                                  // TransactionData

                                                                  //DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                                                                  return Task.CompletedTask;
                                                              };

            #endregion

            #endregion

            #region Common

            #region OnDataTransfer

            OCPP.OUT.OnDataTransferResponseSent += static (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           sentMessageResult,
                                                           cancellationToken) => {
                                                               DebugX.Log($"'{request.NetworkPath.Source}': Incoming DataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");
                                                               return Task.CompletedTask;
                                                           };

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            OCPP.OUT.OnBootNotificationResponseSent += static (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               cancellationToken) => {
                                                                   DebugX.Log($"OnBootNotification: {request.DestinationId}, '{request.ChargePointVendor}'/'{request.ChargePointModel}' => {response.Status}");
                                                                   return Task.CompletedTask;
                                                               };

            #endregion

            #region OnFirmwareStatusNotification

            OCPP.OUT.OnFirmwareStatusNotificationResponseSent += static (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         cancellationToken) => {

                                                                             DebugX.Log("OnFirmwareStatus: " + request.Status);

                                                                             // Status
                                                                             // UpdateFirmwareRequestId

                                                                             return Task.CompletedTask;
                                                                         };

            #endregion

            #region OnHeartbeat

            OCPP.OUT.OnHeartbeatResponseSent += static (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime,
                                                        sentMessageResult,
                                                        cancellationToken) => {
                                                            DebugX.Log("OnHeartbeat: " + request.DestinationId);
                                                            return Task.CompletedTask;
                                                        };

            #endregion

            #region OnSignedFirmwareStatusNotification

            OCPP.OUT.OnSignedFirmwareStatusNotificationResponseSent += static (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               sentMessageResult,
                                                                               cancellationToken) => {

                                                                                   DebugX.Log("OnSignedFirmwareStatusNotification: " + request.Status);

                                                                                   // Status
                                                                                   // UpdateFirmwareRequestId

                                                                                   return Task.CompletedTask;
                                                                               };

            #endregion

            #endregion

            #region Monitoring

            #region OnDiagnosticsStatusNotification

            OCPP.OUT.OnDiagnosticsStatusNotificationResponseSent += static (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    cancellationToken) => {
                                                                        // Status
                                                                        DebugX.Log("OnDiagnosticsStatusNotification: " + request.DestinationId);
                                                                        return Task.CompletedTask;
                                                                    };

            #endregion

            #region OnLogStatusNotification

            OCPP.OUT.OnLogStatusNotificationResponseSent += static (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    cancellationToken) => {
                                                                        // Status
                                                                        // LogRquestId
                                                                        DebugX.Log("OnLogStatusNotification: " + request.DestinationId);
                                                                        return Task.CompletedTask;
                                                                    };

            #endregion

            #region OnSecurityEventNotification

            OCPP.IN.OnSecurityEventNotification += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                DebugX.Log("OnSecurityEventNotification: " + request.DestinationId);

                // Type
                // Timestamp
                // TechInfo

                return Task.FromResult(
                           new SecurityEventNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

        }



        #region CSMS -> Charging Station Messages

        #region (private) NextRequestId

        //private Request_Id NextRequestId
        //{
        //    get
        //    {

        //        Interlocked.Increment(ref internalRequestId);

        //        return Request_Id.Parse(internalRequestId.ToString());

        //    }
        //}

        //    Request_Id ICentralSystem.NextRequestId => throw new NotImplementedException();

        public IEnumerable<ICSMSChannel> CSMSChannels => throw new NotImplementedException();

   //     public NetworkingNode_Id ChargeBoxIdentity => throw new NotImplementedException();

        public string From => throw new NotImplementedException();

        public string To => throw new NotImplementedException();

        #endregion


        //#region Reset                       (Request)

        ///// <summary>
        ///// Reset the given charge box.
        ///// </summary>
        ///// <param name="Request">A Reset request.</param>
        //public async Task<CP.ResetResponse> Reset(ResetRequest Request)
        //{

        //    #region Send OnResetRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnResetRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnResetRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomResetRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.Reset(Request)

        //                              : new CP.ResetResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ResetResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomResetResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnResetResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnResetResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnResetResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region ChangeAvailability          (Request)

        ///// <summary>
        ///// ChangeAvailability the given charge box.
        ///// </summary>
        ///// <param name="Request">A ChangeAvailability request.</param>
        //public async Task<CP.ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        //{

        //    #region Send OnChangeAvailabilityRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnChangeAvailabilityRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeAvailabilityRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomChangeAvailabilityRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ChangeAvailability(Request)

        //                              : new CP.ChangeAvailabilityResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ChangeAvailabilityResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomChangeAvailabilityResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnChangeAvailabilityResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnChangeAvailabilityResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeAvailabilityResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region GetConfiguration            (Request)

        ///// <summary>
        ///// GetConfiguration the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetConfiguration request.</param>
        //public async Task<CP.GetConfigurationResponse> GetConfiguration(GetConfigurationRequest Request)
        //{

        //    #region Send OnGetConfigurationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetConfigurationRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetConfigurationRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetConfigurationRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetConfiguration(Request)

        //                              : new CP.GetConfigurationResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetConfigurationResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetConfigurationResponseSerializer,
        //            CustomConfigurationKeySerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetConfigurationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetConfigurationResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetConfigurationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region ChangeConfiguration         (Request)

        ///// <summary>
        ///// ChangeConfiguration the given charge box.
        ///// </summary>
        ///// <param name="Request">A ChangeConfiguration request.</param>
        //public async Task<CP.ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request)
        //{

        //    #region Send OnChangeConfigurationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnChangeConfigurationRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeConfigurationRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomChangeConfigurationRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ChangeConfiguration(Request)

        //                              : new CP.ChangeConfigurationResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ChangeConfigurationResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomChangeConfigurationResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnChangeConfigurationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnChangeConfigurationResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeConfigurationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region DataTransfer                (Request)

        ///// <summary>
        ///// DataTransfer the given charge box.
        ///// </summary>
        ///// <param name="Request">A DataTransfer request.</param>
        //public async Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest Request)
        //{

        //    #region Send OnDataTransferRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnDataTransferRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDataTransferRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomDataTransferRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.DataTransfer(Request)

        //                              : new CP.DataTransferResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.DataTransferResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomDataTransferResponseSerializer,
        //            null,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnDataTransferResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnDataTransferResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDataTransferResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region GetDiagnostics              (Request)

        ///// <summary>
        ///// GetDiagnostics the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetDiagnostics request.</param>
        //public async Task<CP.GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest Request)
        //{

        //    #region Send OnGetDiagnosticsRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetDiagnosticsRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetDiagnosticsRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetDiagnosticsRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetDiagnostics(Request)

        //                              : new CP.GetDiagnosticsResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetDiagnosticsResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetDiagnosticsResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetDiagnosticsResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetDiagnosticsResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetDiagnosticsResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region TriggerMessage              (Request)

        ///// <summary>
        ///// TriggerMessage the given charge box.
        ///// </summary>
        ///// <param name="Request">A TriggerMessage request.</param>
        //public async Task<CP.TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        //{

        //    #region Send OnTriggerMessageRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnTriggerMessageRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnTriggerMessageRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomTriggerMessageRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.TriggerMessage(Request)

        //                              : new CP.TriggerMessageResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.TriggerMessageResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomTriggerMessageResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnTriggerMessageResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnTriggerMessageResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnTriggerMessageResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region UpdateFirmware              (Request)

        ///// <summary>
        ///// UpdateFirmware the given charge box.
        ///// </summary>
        ///// <param name="Request">A UpdateFirmware request.</param>
        //public async Task<CP.UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        //{

        //    #region Send OnUpdateFirmwareRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnUpdateFirmwareRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateFirmwareRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomUpdateFirmwareRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.UpdateFirmware(Request)

        //                              : new CP.UpdateFirmwareResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.UpdateFirmwareResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomUpdateFirmwareResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnUpdateFirmwareResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnUpdateFirmwareResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateFirmwareResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //#region ReserveNow                  (Request)

        ///// <summary>
        ///// ReserveNow the given charge box.
        ///// </summary>
        ///// <param name="Request">A ReserveNow request.</param>
        //public async Task<CP.ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        //{

        //    #region Send OnReserveNowRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnReserveNowRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnReserveNowRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomReserveNowRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ReserveNow(Request)

        //                              : new CP.ReserveNowResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ReserveNowResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomReserveNowResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnReserveNowResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnReserveNowResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnReserveNowResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region CancelReservation           (Request)

        ///// <summary>
        ///// CancelReservation the given charge box.
        ///// </summary>
        ///// <param name="Request">A CancelReservation request.</param>
        //public async Task<CP.CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        //{

        //    #region Send OnCancelReservationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnCancelReservationRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCancelReservationRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomCancelReservationRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.CancelReservation(Request)

        //                              : new CP.CancelReservationResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.CancelReservationResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomCancelReservationResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnCancelReservationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnCancelReservationResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCancelReservationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region RemoteStartTransaction      (Request)

        ///// <summary>
        ///// RemoteStartTransaction the given charge box.
        ///// </summary>
        ///// <param name="Request">A RemoteStartTransaction request.</param>
        //public async Task<CP.RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request)
        //{

        //    #region Send OnRemoteStartTransactionRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnRemoteStartTransactionRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStartTransactionRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomRemoteStartTransactionRequestSerializer,
        //                                  CustomChargingProfileSerializer,
        //                                  CustomChargingScheduleSerializer,
        //                                  CustomChargingSchedulePeriodSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.RemoteStartTransaction(Request)

        //                              : new CP.RemoteStartTransactionResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.RemoteStartTransactionResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomRemoteStartTransactionResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnRemoteStartTransactionResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnRemoteStartTransactionResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStartTransactionResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region RemoteStopTransaction       (Request)

        ///// <summary>
        ///// RemoteStopTransaction the given charge box.
        ///// </summary>
        ///// <param name="Request">A RemoteStopTransaction request.</param>
        //public async Task<CP.RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest Request)
        //{

        //    #region Send OnRemoteStopTransactionRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnRemoteStopTransactionRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStopTransactionRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomRemoteStopTransactionRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.RemoteStopTransaction(Request)

        //                              : new CP.RemoteStopTransactionResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.RemoteStopTransactionResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomRemoteStopTransactionResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnRemoteStopTransactionResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnRemoteStopTransactionResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStopTransactionResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SetChargingProfile          (Request)

        ///// <summary>
        ///// SetChargingProfile the given charge box.
        ///// </summary>
        ///// <param name="Request">A SetChargingProfile request.</param>
        //public async Task<CP.SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        //{

        //    #region Send OnSetChargingProfileRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSetChargingProfileRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSetChargingProfileRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomSetChargingProfileRequestSerializer,
        //                                  CustomChargingProfileSerializer,
        //                                  CustomChargingScheduleSerializer,
        //                                  CustomChargingSchedulePeriodSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.SetChargingProfile(Request)

        //                              : new CP.SetChargingProfileResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.SetChargingProfileResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomSetChargingProfileResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnSetChargingProfileResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSetChargingProfileResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSetChargingProfileResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region ClearChargingProfile        (Request)

        ///// <summary>
        ///// ClearChargingProfile the given charge box.
        ///// </summary>
        ///// <param name="Request">A ClearChargingProfile request.</param>
        //public async Task<CP.ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        //{

        //    #region Send OnClearChargingProfileRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnClearChargingProfileRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearChargingProfileRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomClearChargingProfileRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ClearChargingProfile(Request)

        //                              : new CP.ClearChargingProfileResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ClearChargingProfileResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomClearChargingProfileResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnClearChargingProfileResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnClearChargingProfileResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearChargingProfileResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region GetCompositeSchedule        (Request)

        ///// <summary>
        ///// GetCompositeSchedule the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetCompositeSchedule request.</param>
        //public async Task<CP.GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        //{

        //    #region Send OnGetCompositeScheduleRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetCompositeScheduleRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetCompositeScheduleRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetCompositeScheduleRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetCompositeSchedule(Request)

        //                              : new CP.GetCompositeScheduleResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetCompositeScheduleResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetCompositeScheduleResponseSerializer,
        //            CustomChargingScheduleSerializer,
        //            CustomChargingSchedulePeriodSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetCompositeScheduleResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetCompositeScheduleResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetCompositeScheduleResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region UnlockConnector             (Request)

        ///// <summary>
        ///// UnlockConnector the given charge box.
        ///// </summary>
        ///// <param name="Request">A UnlockConnector request.</param>
        //public async Task<CP.UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        //{

        //    #region Send OnUnlockConnectorRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnUnlockConnectorRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUnlockConnectorRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomUnlockConnectorRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.UnlockConnector(Request)

        //                              : new CP.UnlockConnectorResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.UnlockConnectorResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomUnlockConnectorResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnUnlockConnectorResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnUnlockConnectorResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUnlockConnectorResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //#region GetLocalListVersion         (Request)

        ///// <summary>
        ///// GetLocalListVersion the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetLocalListVersion request.</param>
        //public async Task<CP.GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        //{

        //    #region Send OnGetLocalListVersionRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetLocalListVersionRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLocalListVersionRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetLocalListVersionRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetLocalListVersion(Request)

        //                              : new CP.GetLocalListVersionResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetLocalListVersionResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetLocalListVersionResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetLocalListVersionResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetLocalListVersionResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLocalListVersionResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SendLocalList               (Request)

        ///// <summary>
        ///// SendLocalList the given charge box.
        ///// </summary>
        ///// <param name="Request">A SendLocalList request.</param>
        //public async Task<CP.SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        //{

        //    #region Send OnSendLocalListRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSendLocalListRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendLocalListRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomSendLocalListRequestSerializer,
        //                                  CustomAuthorizationDataSerializer,
        //                                  CustomIdTagInfoSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.SendLocalList(Request)

        //                              : new CP.SendLocalListResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.SendLocalListResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomSendLocalListResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnSendLocalListResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSendLocalListResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendLocalListResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region ClearCache                  (Request)

        ///// <summary>
        ///// ClearCache the given charge box.
        ///// </summary>
        ///// <param name="Request">A ClearCache request.</param>
        //public async Task<CP.ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        //{

        //    #region Send OnClearCacheRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnClearCacheRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearCacheRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomClearCacheRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ClearCache(Request)

        //                              : new CP.ClearCacheResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ClearCacheResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomClearCacheResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnClearCacheResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnClearCacheResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearCacheResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //// Security extensions

        //#region CertificateSigned           (Request)

        ///// <summary>
        ///// CertificateSigned the given charge box.
        ///// </summary>
        ///// <param name="Request">A CertificateSigned request.</param>
        //public async Task<CP.CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        //{

        //    #region Send OnCertificateSignedRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnCertificateSignedRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCertificateSignedRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomCertificateSignedRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.CertificateSigned(Request)

        //                              : new CP.CertificateSignedResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.CertificateSignedResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomCertificateSignedResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnCertificateSignedResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnCertificateSignedResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCertificateSignedResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region DeleteCertificate           (Request)

        ///// <summary>
        ///// DeleteCertificate the given charge box.
        ///// </summary>
        ///// <param name="Request">A DeleteCertificate request.</param>
        //public async Task<CP.DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        //{

        //    #region Send OnDeleteCertificateRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnDeleteCertificateRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteCertificateRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomDeleteCertificateRequestSerializer,
        //                                  CustomCertificateHashDataSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.DeleteCertificate(Request)

        //                              : new CP.DeleteCertificateResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.DeleteCertificateResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomDeleteCertificateResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnDeleteCertificateResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnDeleteCertificateResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteCertificateResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region ExtendedTriggerMessage      (Request)

        ///// <summary>
        ///// ExtendedTriggerMessage the given charge box.
        ///// </summary>
        ///// <param name="Request">A ExtendedTriggerMessage request.</param>
        //public async Task<CP.ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request)
        //{

        //    #region Send OnExtendedTriggerMessageRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnExtendedTriggerMessageRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnExtendedTriggerMessageRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomExtendedTriggerMessageRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.ExtendedTriggerMessage(Request)

        //                              : new CP.ExtendedTriggerMessageResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.ExtendedTriggerMessageResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomExtendedTriggerMessageResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnExtendedTriggerMessageResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnExtendedTriggerMessageResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnExtendedTriggerMessageResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region GetInstalledCertificateIds  (Request)

        ///// <summary>
        ///// GetInstalledCertificateIds the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetInstalledCertificateIds request.</param>
        //public async Task<CP.GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        //{

        //    #region Send OnGetInstalledCertificateIdsRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetInstalledCertificateIdsRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetInstalledCertificateIdsRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetInstalledCertificateIds(Request)

        //                              : new CP.GetInstalledCertificateIdsResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetInstalledCertificateIdsResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetInstalledCertificateIdsResponseSerializer,
        //            CustomCertificateHashDataSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetInstalledCertificateIdsResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetInstalledCertificateIdsResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region GetLog                      (Request)

        ///// <summary>
        ///// GetLog the given charge box.
        ///// </summary>
        ///// <param name="Request">A GetLog request.</param>
        //public async Task<CP.GetLogResponse> GetLog(GetLogRequest Request)
        //{

        //    #region Send OnGetLogRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetLogRequest?.Invoke(startTime,
        //                                this,
        //                                Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLogRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomGetLogRequestSerializer,
        //                                  CustomLogParametersSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.GetLog(Request)

        //                              : new CP.GetLogResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.GetLogResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomGetLogResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnGetLogResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnGetLogResponse?.Invoke(endTime,
        //                                 this,
        //                                 Request,
        //                                 response,
        //                                 endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLogResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region InstallCertificate          (Request)

        ///// <summary>
        ///// InstallCertificate the given charge box.
        ///// </summary>
        ///// <param name="Request">A InstallCertificate request.</param>
        //public async Task<CP.InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        //{

        //    #region Send OnInstallCertificateRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnInstallCertificateRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnInstallCertificateRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomInstallCertificateRequestSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.InstallCertificate(Request)

        //                              : new CP.InstallCertificateResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.InstallCertificateResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomInstallCertificateResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnInstallCertificateResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnInstallCertificateResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnInstallCertificateResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SignedUpdateFirmware        (Request)

        ///// <summary>
        ///// SignedUpdateFirmware the given charge box.
        ///// </summary>
        ///// <param name="Request">A SignedUpdateFirmware request.</param>
        //public async Task<CP.SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request)
        //{

        //    #region Send OnSignedUpdateFirmwareRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignedUpdateFirmwareRequest?.Invoke(startTime,
        //                               this,
        //                               Request);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedUpdateFirmwareRequest));
        //    }

        //    #endregion


        //    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        //                        centralSystem is not null

        //                        ? SignaturePolicy.SignRequestMessage(
        //                              Request,
        //                              Request.ToJSON(
        //                                  CustomSignedUpdateFirmwareRequestSerializer,
        //                                  CustomFirmwareImageSerializer,
        //                                  CustomSignatureSerializer,
        //                                  CustomCustomDataSerializer
        //                              ),
        //                              out var errorResponse
        //                          )

        //                              ? await centralSystem.Item1.SignedUpdateFirmware(Request)

        //                              : new CP.SignedUpdateFirmwareResponse(
        //                                    Request,
        //                                    Result.SignatureError(errorResponse)
        //                                )

        //                        : new CP.SignedUpdateFirmwareResponse(
        //                              Request,
        //                              Result.Server("Unknown or unreachable charge box!")
        //                          );


        //    SignaturePolicy.VerifyResponseMessage(
        //        response,
        //        response.ToJSON(
        //            CustomSignedUpdateFirmwareResponseSerializer,
        //            CustomSignatureSerializer,
        //            CustomCustomDataSerializer
        //        ),
        //        out errorResponse
        //    );


        //    #region Send OnSignedUpdateFirmwareResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignedUpdateFirmwareResponse?.Invoke(endTime,
        //                                this,
        //                                Request,
        //                                response,
        //                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedUpdateFirmwareResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //// Binary Data Streams Extensions

        //#region BinaryDataTransfer          (Request)

        /////// <summary>
        /////// Transfer the given data to the given charging station.
        /////// </summary>
        /////// <param name="Request">A BinaryDataTransfer request.</param>
        ////public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        ////{

        ////    #region Send OnBinaryDataTransferRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnBinaryDataTransferRequest?.Invoke(startTime,
        ////                                            this,
        ////                                            Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToBinary(
        ////                                  CustomBinaryDataTransferRequestSerializer,
        ////                                  CustomBinarySignatureSerializer,
        ////                                  IncludeSignatures: false
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.BinaryDataTransfer(Request)

        ////                              : new BinaryDataTransferResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new BinaryDataTransferResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToBinary(
        ////            CustomBinaryDataTransferResponseSerializer,
        ////            null, // CustomStatusInfoSerializer
        ////            CustomBinarySignatureSerializer,
        ////            IncludeSignatures: false
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnBinaryDataTransferResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnBinaryDataTransferResponse?.Invoke(endTime,
        ////                                       this,
        ////                                       Request,
        ////                                       response,
        ////                                       endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region GetFile                     (Request)

        /////// <summary>
        /////// Request the given file from the charging station.
        /////// </summary>
        /////// <param name="Request">A GetFile request.</param>
        ////public async Task<OCPP.CS.GetFileResponse> GetFile(GetFileRequest Request)
        ////{

        ////    #region Send OnGetFileRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnGetFileRequest?.Invoke(startTime,
        ////                                 this,
        ////                                 Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetFileRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  CustomGetFileRequestSerializer,
        ////                                  CustomSignatureSerializer,
        ////                                  CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.GetFile(Request)

        ////                              : new OCPP.CS.GetFileResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.GetFileResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToBinary(
        ////            CustomGetFileResponseSerializer,
        ////            null, // CustomStatusInfoSerializer
        ////            CustomBinarySignatureSerializer,
        ////            IncludeSignatures: false
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnGetFileResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnGetFileResponse?.Invoke(endTime,
        ////                                  this,
        ////                                  Request,
        ////                                  response,
        ////                                  endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetFileResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region SendFile                    (Request)

        /////// <summary>
        /////// Request the given file from the charging station.
        /////// </summary>
        /////// <param name="Request">A SendFile request.</param>
        ////public async Task<OCPP.CS.SendFileResponse> SendFile(SendFileRequest Request)
        ////{

        ////    #region Send OnSendFileRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnSendFileRequest?.Invoke(startTime,
        ////                                  this,
        ////                                  Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendFileRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToBinary(
        ////                                  CustomSendFileRequestSerializer,
        ////                                  CustomBinarySignatureSerializer,
        ////                                  IncludeSignatures: false
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.SendFile(Request)

        ////                              : new OCPP.CS.SendFileResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.SendFileResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            CustomSendFileResponseSerializer,
        ////            CustomStatusInfoSerializer,
        ////            CustomSignatureSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnSendFileResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnSendFileResponse?.Invoke(endTime,
        ////                                   this,
        ////                                   Request,
        ////                                   response,
        ////                                   endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendFileResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region DeleteFile                  (Request)

        /////// <summary>
        /////// Delete the given file from the charging station.
        /////// </summary>
        /////// <param name="Request">A DeleteFile request.</param>
        ////public async Task<OCPP.CS.DeleteFileResponse> DeleteFile(DeleteFileRequest Request)
        ////{

        ////    #region Send OnDeleteFileRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteFileRequest?.Invoke(startTime,
        ////                                    this,
        ////                                    Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteFileRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  CustomDeleteFileRequestSerializer,
        ////                                  CustomSignatureSerializer,
        ////                                  CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.DeleteFile(Request)

        ////                              : new OCPP.CS.DeleteFileResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.DeleteFileResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            CustomDeleteFileResponseSerializer,
        ////            CustomStatusInfoSerializer,
        ////            CustomSignatureSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnDeleteFileResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteFileResponse?.Invoke(endTime,
        ////                                     this,
        ////                                     Request,
        ////                                     response,
        ////                                     endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteFileResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region ListDirectory               (Request)

        /////// <summary>
        /////// Delete the given file from the charging station.
        /////// </summary>
        /////// <param name="Request">A ListDirectory request.</param>
        ////public async Task<OCPP.CS.ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        ////{

        ////    #region Send OnListDirectoryRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnListDirectoryRequest?.Invoke(startTime,
        ////                                       this,
        ////                                       Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnListDirectoryRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  CustomListDirectoryRequestSerializer,
        ////                                  CustomSignatureSerializer,
        ////                                  CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.ListDirectory(Request)

        ////                              : new OCPP.CS.ListDirectoryResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.ListDirectoryResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            CustomListDirectoryResponseSerializer,
        ////            CustomStatusInfoSerializer,
        ////            CustomSignatureSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnListDirectoryResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnListDirectoryResponse?.Invoke(endTime,
        ////                                        this,
        ////                                        Request,
        ////                                        response,
        ////                                        endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnListDirectoryResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion



        //// E2E Security Extensions

        //#region AddSignaturePolicy          (Request)

        /////// <summary>
        /////// Add a signature policy.
        /////// </summary>
        /////// <param name="Request">An AddSignaturePolicy request.</param>
        ////public async Task<OCPP.CS.AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
        ////{

        ////    #region Send OnAddSignaturePolicyRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnAddSignaturePolicyRequest?.Invoke(startTime,
        ////                                            this,
        ////                                            Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddSignaturePolicyRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomAddSignaturePolicyRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.AddSignaturePolicy(Request)

        ////                              : new OCPP.CS.AddSignaturePolicyResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.AddSignaturePolicyResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomAddSignaturePolicyResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnAddSignaturePolicyResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnAddSignaturePolicyResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddSignaturePolicyResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region UpdateSignaturePolicy       (Request)

        /////// <summary>
        /////// Set a display message.
        /////// </summary>
        /////// <param name="Request">A UpdateSignaturePolicy request.</param>
        ////public async Task<OCPP.CS.UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
        ////{

        ////    #region Send OnUpdateSignaturePolicyRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnUpdateSignaturePolicyRequest?.Invoke(startTime,
        ////                                           this,
        ////                                           Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateSignaturePolicyRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomUpdateSignaturePolicyRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.UpdateSignaturePolicy(Request)

        ////                              : new OCPP.CS.UpdateSignaturePolicyResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.UpdateSignaturePolicyResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomUpdateSignaturePolicyResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnUpdateSignaturePolicyResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnUpdateSignaturePolicyResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateSignaturePolicyResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region DeleteSignaturePolicy       (Request)

        /////// <summary>
        /////// Set a display message.
        /////// </summary>
        /////// <param name="Request">A DeleteSignaturePolicy request.</param>
        ////public async Task<OCPP.CS.DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)
        ////{

        ////    #region Send OnDeleteSignaturePolicyRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteSignaturePolicyRequest?.Invoke(startTime,
        ////                                           this,
        ////                                           Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteSignaturePolicyRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomDeleteSignaturePolicyRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.DeleteSignaturePolicy(Request)

        ////                              : new OCPP.CS.DeleteSignaturePolicyResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.DeleteSignaturePolicyResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomDeleteSignaturePolicyResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnDeleteSignaturePolicyResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteSignaturePolicyResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteSignaturePolicyResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region AddUserRole                 (Request)

        /////// <summary>
        /////// Set a display message.
        /////// </summary>
        /////// <param name="Request">A AddUserRole request.</param>
        ////public async Task<OCPP.CS.AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request)
        ////{

        ////    #region Send OnAddUserRoleRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnAddUserRoleRequest?.Invoke(startTime,
        ////                                           this,
        ////                                           Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddUserRoleRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomAddUserRoleRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.AddUserRole(Request)

        ////                              : new OCPP.CS.AddUserRoleResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.AddUserRoleResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomAddUserRoleResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnAddUserRoleResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnAddUserRoleResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddUserRoleResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region UpdateUserRole              (Request)

        /////// <summary>
        /////// Set a display message.
        /////// </summary>
        /////// <param name="Request">A UpdateUserRole request.</param>
        ////public async Task<OCPP.CS.UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
        ////{

        ////    #region Send OnUpdateUserRoleRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnUpdateUserRoleRequest?.Invoke(startTime,
        ////                                           this,
        ////                                           Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateUserRoleRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomUpdateUserRoleRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.UpdateUserRole(Request)

        ////                              : new OCPP.CS.UpdateUserRoleResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.UpdateUserRoleResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomUpdateUserRoleResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnUpdateUserRoleResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnUpdateUserRoleResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateUserRoleResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        //#region DeleteUserRole              (Request)

        /////// <summary>
        /////// Set a display message.
        /////// </summary>
        /////// <param name="Request">A DeleteUserRole request.</param>
        ////public async Task<OCPP.CS.DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request)
        ////{

        ////    #region Send OnDeleteUserRoleRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteUserRoleRequest?.Invoke(startTime,
        ////                                           this,
        ////                                           Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteUserRoleRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToJSON(
        ////                                  //CustomDeleteUserRoleRequestSerializer,
        ////                                  //CustomMessageInfoSerializer,
        ////                                  //CustomMessageContentSerializer,
        ////                                  //CustomComponentSerializer,
        ////                                  //CustomEVSESerializer,
        ////                                  //CustomSignatureSerializer,
        ////                                  //CustomCustomDataSerializer
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.DeleteUserRole(Request)

        ////                              : new OCPP.CS.DeleteUserRoleResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new OCPP.CS.DeleteUserRoleResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToJSON(
        ////            //CustomDeleteUserRoleResponseSerializer,
        ////            //CustomStatusInfoSerializer,
        ////            //CustomSignatureSerializer,
        ////            //CustomCustomDataSerializer
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnDeleteUserRoleResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnDeleteUserRoleResponse?.Invoke(endTime,
        ////                                            this,
        ////                                            Request,
        ////                                            response,
        ////                                            endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteUserRoleResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion


        //#region SecureDataTransfer          (Request)

        /////// <summary>
        /////// Transfer the given data to the given charging station.
        /////// </summary>
        /////// <param name="Request">A SecureDataTransfer request.</param>
        ////public async Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request)
        ////{

        ////    #region Send OnSecureDataTransferRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnSecureDataTransferRequest?.Invoke(startTime,
        ////                                            this,
        ////                                            Request);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecureDataTransferRequest));
        ////    }

        ////    #endregion


        ////    var response  = reachableChargeBoxes.TryGetValue(Request.DestinationId, out var centralSystem) &&
        ////                        centralSystem is not null

        ////                        ? SignaturePolicy.SignRequestMessage(
        ////                              Request,
        ////                              Request.ToBinary(
        ////                                  CustomSecureDataTransferRequestSerializer,
        ////                                  CustomBinarySignatureSerializer,
        ////                                  IncludeSignatures: false
        ////                              ),
        ////                              out var errorResponse
        ////                          )

        ////                              ? await centralSystem.Item1.SecureDataTransfer(Request)

        ////                              : new SecureDataTransferResponse(
        ////                                    Request,
        ////                                    Result.SignatureError(errorResponse)
        ////                                )

        ////                        : new SecureDataTransferResponse(
        ////                              Request,
        ////                              Result.UnknownOrUnreachable(Request.DestinationId)
        ////                          );


        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToBinary(
        ////            CustomSecureDataTransferResponseSerializer,
        ////            // CustomStatusInfoSerializer
        ////            CustomBinarySignatureSerializer,
        ////            IncludeSignatures: false
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnSecureDataTransferResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnSecureDataTransferResponse?.Invoke(endTime,
        ////                                       this,
        ////                                       Request,
        ////                                       response,
        ////                                       endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecureDataTransferResponse));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        #endregion



        #region Shutdown(Message, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            //var centralSystemServersCopy = centralSystemServers.ToArray();
            //if (centralSystemServersCopy.Length > 0)
            //{
            //    try
            //    {

            //        await Task.WhenAll(centralSystemServers.
            //                               Select(centralSystemServer => centralSystemServer.Shutdown(
            //                                                                 Message,
            //                                                                 Wait
            //                                                             )).
            //                               ToArray());

            //    }
            //    catch (Exception e)
            //    {
            //        await HandleErrors(
            //                  nameof(TestCentralSystem),
            //                  nameof(Shutdown),
            //                  e
            //              );
            //    }
            //}

        }

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccurred)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccurred)
        {

            DebugX.LogException(ExceptionOccurred, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
