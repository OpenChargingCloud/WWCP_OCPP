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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system HTTP/SOAP/XML server.
    /// </summary>
    public class CentralSystemSOAPServer : ASOAPServer,
                                           ICentralSystemServer
    {

        #region Data

        /// <summary>
        /// The default HTTP/SOAP/XML server name.
        /// </summary>
        public new const           String           DefaultHTTPServerName  = $"GraphDefined OCPP {Version.String} HTTP/SOAP/XML Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort           DefaultHTTPServerPort  = IPPort.Parse(2010);

        /// <summary>
        /// The default TCP service name shown e.g. on service startup.
        /// </summary>
        public     const           String           DefaultServiceName     = $"OCPP {Version.String} Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new static readonly HTTPPath         DefaultURLPrefix       = HTTPPath.Parse("/" + Version.String);

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public new static readonly HTTPContentType  DefaultContentType     = HTTPContentType.Text.XML_UTF8;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public new static readonly TimeSpan         DefaultRequestTimeout  = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => SOAPServer.HTTPServer.DefaultServerName;

        /// <summary>
        /// The unique identifications of all reachable charge boxes.
        /// </summary>
        public IEnumerable<NetworkingNode_Id>  NetworkingNodeIds
            => Array.Empty<NetworkingNode_Id>();

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                    OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?    OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate?           OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?   OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler?                     OnBootNotificationSOAPResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler?             OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?    OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?           OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?   OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler?              OnHeartbeatSOAPResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler?             OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?    OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?           OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?   OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler?              OnAuthorizeSOAPResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                    OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionRequestDelegate?    OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate?           OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate?   OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler?                     OnStartTransactionSOAPResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                      OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?           OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?   OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler?                       OnStatusNotificationSOAPResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler?               OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate?           OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?   OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler?                OnMeterValuesSOAPResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                   OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate?    OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate?           OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate?   OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler?                    OnStopTransactionSOAPResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                                  OnIncomingDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferRequestDelegate?    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferDelegate?           OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferResponseDelegate?   OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler?                                   OnIncomingDataTransferSOAPResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                                 OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?    OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate?           OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?   OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler?                                  OnDiagnosticsStatusNotificationSOAPResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler?                              OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?           OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?   OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler?                               OnFirmwareStatusNotificationSOAPResponse;

        #endregion



        public event OnSecurityEventNotificationRequestDelegate OnSecurityEventNotificationRequest;
        public event OnSecurityEventNotificationDelegate OnSecurityEventNotification;
        public event OnSecurityEventNotificationResponseDelegate OnSecurityEventNotificationResponse;

        public event OnLogStatusNotificationRequestDelegate OnLogStatusNotificationRequest;
        public event OnLogStatusNotificationDelegate OnLogStatusNotification;
        public event OnLogStatusNotificationResponseDelegate OnLogStatusNotificationResponse;

        public event OnSignCertificateRequestDelegate OnSignCertificateRequest;
        public event OnSignCertificateDelegate OnSignCertificate;
        public event OnSignCertificateResponseDelegate OnSignCertificateResponse;

        public event OnSignedFirmwareStatusNotificationRequestDelegate OnSignedFirmwareStatusNotificationRequest;
        public event OnSignedFirmwareStatusNotificationDelegate OnSignedFirmwareStatusNotification;
        public event OnSignedFirmwareStatusNotificationResponseDelegate OnSignedFirmwareStatusNotificationResponse;

        public event OnIncomingBinaryDataTransferRequestDelegate OnIncomingBinaryDataTransferRequest;
        public event OnIncomingBinaryDataTransferDelegate OnIncomingBinaryDataTransfer;
        public event OnIncomingBinaryDataTransferResponseDelegate OnIncomingBinaryDataTransferResponse;

        #endregion

        #region Constructor(s)

        #region CentralSystemSOAPServer(HTTPServerName, TCPPort = default, URLPrefix = default, ContentType = default, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the central system HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemSOAPServer(String            HTTPServerName            = DefaultHTTPServerName,
                                       IPPort?           TCPPort                   = null,
                                       String?           ServiceName               = null,
                                       HTTPPath?         URLPrefix                 = null,
                                       HTTPContentType?  ContentType               = null,
                                       Boolean           RegisterHTTPRootService   = true,
                                       DNSClient?        DNSClient                 = null,
                                       Boolean           AutoStart                 = false)

            : base(HTTPServerName.IsNotNullOrEmpty()
                       ? HTTPServerName
                       : DefaultHTTPServerName,
                   TCPPort     ?? DefaultHTTPServerPort,
                   ServiceName ?? DefaultServiceName,
                   URLPrefix   ?? DefaultURLPrefix,
                   ContentType ?? DefaultContentType,
                   RegisterHTTPRootService,
                   DNSClient,
                   AutoStart: false)

        {

            RegisterURLTemplates();

            if (AutoStart)
                Start();

        }

        #endregion

        #region CentralSystemSOAPServer(SOAPServer, URLPrefix = DefaultURLPrefix)

        /// <summary>
        /// Use the given HTTP server for the central system HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        public CentralSystemSOAPServer(SOAPServer  SOAPServer,
                                       HTTPPath?   URLPrefix = null)

            : base(SOAPServer,
                   URLPrefix ?? DefaultURLPrefix)

        {

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region RegisterURLTemplates()

        /// <summary>
        /// Register all URL templates for this SOAP API.
        /// </summary>
        protected void RegisterURLTemplates()
        {

            #region / - BootNotification

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "BootNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, BootNotificationXML) => {

                #region Send OnBootNotificationSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnBootNotificationSOAPRequest?.Invoke(requestTimestamp,
                                                          SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPRequest));
                }

                #endregion

                BootNotificationResponse? response      = null;
                HTTPResponse?             HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = BootNotificationRequest.Parse(BootNotificationXML,
                                                                    Request_Id.Parse(OCPPHeader.MessageId),
                                                                    NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnBootNotificationRequest event

                    try
                    {

                        OnBootNotificationRequest?.Invoke(request.RequestTimestamp,
                                                          this,
                                                          null,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnBootNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                      this,
                                                                                                                      null,
                                                                                                                      request,
                                                                                                                      Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = BootNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnBootNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnBootNotificationResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           null,
                                                           request,
                                                           response,
                                                           responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/BootNotificationResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/BootNotification");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnBootNotificationSOAPResponse event

                try
                {

                    OnBootNotificationSOAPResponse?.Invoke(Timestamp.Now,
                                                           SOAPServer.HTTPServer,
                                                           Request,
                                                           HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - Heartbeat

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "Heartbeat",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "heartbeatRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, HeartbeatXML) => {

                #region Send OnHeartbeatSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnHeartbeatSOAPRequest?.Invoke(requestTimestamp,
                                                   SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatSOAPRequest));
                }

                #endregion

                HeartbeatResponse? response      = null;
                HTTPResponse?      HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = HeartbeatRequest.Parse(HeartbeatXML,
                                                             Request_Id.Parse(OCPPHeader.MessageId),
                                                             NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnHeartbeatRequest event

                    try
                    {

                        OnHeartbeatRequest?.Invoke(request.RequestTimestamp,
                                                   this,
                                                   null,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnHeartbeat?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)?.Invoke(Timestamp.Now,
                                                                                                               this,
                                                                                                               null,
                                                                                                               request,
                                                                                                               Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = HeartbeatResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnHeartbeatResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnHeartbeatResponse?.Invoke(responseTimestamp,
                                                    this,
                                                    null,
                                                    request,
                                                    response,
                                                    responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/HeartbeatResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/Heartbeat");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnHeartbeatSOAPResponse event

                try
                {

                    OnHeartbeatSOAPResponse?.Invoke(Timestamp.Now,
                                                    SOAPServer.HTTPServer,
                                                    Request,
                                                    HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion


            #region / - Authorize

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "Authorize",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "authorizeRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, AuthorizeXML) => {

                #region Send OnAuthorizeSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnAuthorizeSOAPRequest?.Invoke(requestTimestamp,
                                                   SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeSOAPRequest));
                }

                #endregion

                AuthorizeResponse? response      = null;
                HTTPResponse?      HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = AuthorizeRequest.Parse(AuthorizeXML,
                                                             Request_Id.Parse(OCPPHeader.MessageId),
                                                             NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(request.RequestTimestamp,
                                                   this,
                                                   null,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnAuthorize?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
                                                                                                               this,
                                                                                                               null,
                                                                                                               request,
                                                                                                               Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = AuthorizeResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnAuthorizeResponse?.Invoke(responseTimestamp,
                                                    this,
                                                    null,
                                                    request,
                                                    response,
                                                    responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/AuthorizeResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "./Authorize");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnAuthorizeSOAPResponse event

                try
                {

                    OnAuthorizeSOAPResponse?.Invoke(Timestamp.Now,
                                                    SOAPServer.HTTPServer,
                                                    Request,
                                                    HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StartTransaction

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "StartTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, StartTransactionXML) => {

                #region Send OnStartTransactionSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStartTransactionSOAPRequest?.Invoke(requestTimestamp,
                                                          SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionSOAPRequest));
                }

                #endregion

                StartTransactionResponse? response      = null;
                HTTPResponse?             HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StartTransactionRequest.Parse(StartTransactionXML,
                                                                    Request_Id.Parse(OCPPHeader.MessageId),
                                                                    NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnStartTransactionRequest event

                    try
                    {

                        OnStartTransactionRequest?.Invoke(request.RequestTimestamp,
                                                          this,
                                                          null,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnStartTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                      this,
                                                                                                                      null,
                                                                                                                      request,
                                                                                                                      Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = StartTransactionResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStartTransactionResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStartTransactionResponse?.Invoke(responseTimestamp,
                                                           this,
                                                           null,
                                                           request,
                                                           response,
                                                           responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StartTransactionResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/StartTransaction");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnStartTransactionSOAPResponse event

                try
                {

                    OnStartTransactionSOAPResponse?.Invoke(Timestamp.Now,
                                                           SOAPServer.HTTPServer,
                                                           Request,
                                                           HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StatusNotification

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "StatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "statusNotificationRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, StatusNotificationXML) => {

                #region Send OnStatusNotificationSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStatusNotificationSOAPRequest?.Invoke(requestTimestamp,
                                                            SOAPServer.HTTPServer,
                                                            Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationSOAPRequest));
                }

                #endregion

                StatusNotificationResponse? response      = null;
                HTTPResponse?               HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StatusNotificationRequest.Parse(StatusNotificationXML,
                                                                      Request_Id.Parse(OCPPHeader.MessageId),
                                                                      NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnStatusNotificationRequest event

                    try
                    {

                        OnStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                            this,
                                                            null,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                        this,
                                                                                                                        null,
                                                                                                                        request,
                                                                                                                        Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = StatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStatusNotificationResponse?.Invoke(responseTimestamp,
                                                             this,
                                                             null,
                                                             request,
                                                             response,
                                                             responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StatusNotificationResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/StatusNotification");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnStatusNotificationSOAPResponse event

                try
                {

                    OnStatusNotificationSOAPResponse?.Invoke(Timestamp.Now,
                                                             SOAPServer.HTTPServer,
                                                             Request,
                                                             HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - MeterValues

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "MeterValues",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "meterValuesRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, MeterValuesXML) => {

                #region Send OnMeterValuesSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnMeterValuesSOAPRequest?.Invoke(requestTimestamp,
                                                     SOAPServer.HTTPServer,
                                                     Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesSOAPRequest));
                }

                #endregion

                MeterValuesResponse? response      = null;
                HTTPResponse?        HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = MeterValuesRequest.Parse(MeterValuesXML,
                                                               Request_Id.Parse(OCPPHeader.MessageId),
                                                               NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnMeterValuesRequest event

                    try
                    {

                        OnMeterValuesRequest?.Invoke(request.RequestTimestamp,
                                                     this,
                                                     null,
                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnMeterValues?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                 this,
                                                                                                                 null,
                                                                                                                 request,
                                                                                                                 Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = MeterValuesResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnMeterValuesResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnMeterValuesResponse?.Invoke(responseTimestamp,
                                                      this,
                                                      null,
                                                      request,
                                                      response,
                                                      responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/MeterValuesResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/MeterValues");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnMeterValuesSOAPResponse event

                try
                {

                    OnMeterValuesSOAPResponse?.Invoke(Timestamp.Now,
                                                      SOAPServer.HTTPServer,
                                                      Request,
                                                      HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StopTransaction

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "StopTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, StopTransactionXML) => {

                #region Send OnStopTransactionSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStopTransactionSOAPRequest?.Invoke(requestTimestamp,
                                                         SOAPServer.HTTPServer,
                                                         Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionSOAPRequest));
                }

                #endregion

                StopTransactionResponse? response      = null;
                HTTPResponse?            HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StopTransactionRequest.Parse(StopTransactionXML,
                                                                   Request_Id.Parse(OCPPHeader.MessageId),
                                                                   NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnStopTransactionRequest event

                    try
                    {

                        OnStopTransactionRequest?.Invoke(request.RequestTimestamp,
                                                         this,
                                                         null,
                                                         request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnStopTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                     this,
                                                                                                                     null,
                                                                                                                     request,
                                                                                                                     Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = StopTransactionResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStopTransactionResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStopTransactionResponse?.Invoke(responseTimestamp,
                                                          this,
                                                          null,
                                                          request,
                                                          response,
                                                          responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StopTransactionResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/StopTransaction");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnStopTransactionSOAPResponse event

                try
                {

                    OnStopTransactionSOAPResponse?.Invoke(Timestamp.Now,
                                                          SOAPServer.HTTPServer,
                                                          Request,
                                                          HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion


            #region / - IncomingDataTransfer

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "DataTransfer",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "dataTransferRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, DataTransferXML) => {

                #region Send OnIncomingDataTransferSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferSOAPRequest?.Invoke(requestTimestamp,
                                                              SOAPServer.HTTPServer,
                                                              Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPRequest));
                }

                #endregion


                OCPP.CSMS.DataTransferResponse? response      = null;
                HTTPResponse?                   HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = OCPP.CS.DataTransferRequest.Parse(DataTransferXML,
                                                                        OCPPNS.OCPPv1_6_CS,
                                                                        Request_Id.Parse(OCPPHeader.MessageId),
                                                                        NetworkPath.Empty,
                                                                        NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(request.RequestTimestamp,
                                                              this,
                                                              null,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnIncomingDataTransfer?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OCPP.CSMS.OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    null,
                                                                                                                                    request,
                                                                                                                                    Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = OCPP.CSMS.DataTransferResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnIncomingDataTransferResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnIncomingDataTransferResponse?.Invoke(responseTimestamp,
                                                               this,
                                                               null,
                                                               request,
                                                               response,
                                                               responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DataTransferResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML(OCPPNS.OCPPv1_6_CS)).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/IncomingDataTransfer");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnIncomingDataTransferSOAPResponse event

                try
                {

                    OnIncomingDataTransferSOAPResponse?.Invoke(Timestamp.Now,
                                                               SOAPServer.HTTPServer,
                                                               Request,
                                                               HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - DiagnosticsStatusNotification

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "DiagnosticsStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, DiagnosticsStatusNotificationXML) => {

                #region Send OnDiagnosticsStatusNotificationSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnDiagnosticsStatusNotificationSOAPRequest?.Invoke(requestTimestamp,
                                                                       SOAPServer.HTTPServer,
                                                                       Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPRequest));
                }

                #endregion


                DiagnosticsStatusNotificationResponse? response      = null;
                HTTPResponse?                          HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = DiagnosticsStatusNotificationRequest.Parse(DiagnosticsStatusNotificationXML,
                                                                                 Request_Id.Parse(OCPPHeader.MessageId),
                                                                                 NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnDiagnosticsStatusNotificationRequest event

                    try
                    {

                        OnDiagnosticsStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                                       this,
                                                                       null,
                                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnDiagnosticsStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   null,
                                                                                                                                   request,
                                                                                                                                   Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = DiagnosticsStatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnDiagnosticsStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnDiagnosticsStatusNotificationResponse?.Invoke(responseTimestamp,
                                                                        this,
                                                                        null,
                                                                        request,
                                                                        response,
                                                                        responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DiagnosticsStatusNotificationResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/DiagnosticsStatusNotification");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnDiagnosticsStatusNotificationSOAPResponse event

                try
                {

                    OnDiagnosticsStatusNotificationSOAPResponse?.Invoke(Timestamp.Now,
                                                                        SOAPServer.HTTPServer,
                                                                        Request,
                                                                        HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - FirmwareStatusNotification

            SOAPServer.RegisterSOAPDelegate(null,
                                            HTTPHostname.Any,
                                            URLPrefix,
                                            "FirmwareStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationRequest").FirstOrDefault()!,
                                            async (Request, HeaderXML, FirmwareStatusNotificationXML) => {

                #region Send OnFirmwareStatusNotificationSOAPRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationSOAPRequest?.Invoke(requestTimestamp,
                                                                    SOAPServer.HTTPServer,
                                                                    Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationSOAPRequest));
                }

                #endregion


                FirmwareStatusNotificationResponse? response      = null;
                HTTPResponse?                       HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = FirmwareStatusNotificationRequest.Parse(FirmwareStatusNotificationXML,
                                                                              Request_Id.Parse(OCPPHeader.MessageId),
                                                                              NetworkingNode_Id.Parse(OCPPHeader.ChargeBoxIdentity.ToString()));

                    #region Send OnFirmwareStatusNotificationRequest event

                    try
                    {

                        OnFirmwareStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                                    this,
                                                                    null,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response is null)
                    {

                        var results = OnFirmwareStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                null,
                                                                                                                                request,
                                                                                                                                Request.CancellationToken)).
                                          ToArray();

                        if (results?.Length > 0)
                        {

                            await Task.WhenAll(results!);

                            response = results.First()?.Result!;

                        }

                        if (results?.Length == 0 || response == null)
                            response = FirmwareStatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnFirmwareStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnFirmwareStatusNotificationResponse?.Invoke(responseTimestamp,
                                                                     this,
                                                                     null,
                                                                     request,
                                                                     response,
                                                                     responseTimestamp - requestTimestamp);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = Timestamp.Now,
                        ContentType     = HTTPContentType.Text.XML_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/FirmwareStatusNotificationResponse",
                                                             Request_Id.NewRandom().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/FirmwareStatusNotification");
                }

                HTTPResponse ??= new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                     Server          = SOAPServer.HTTPServer.DefaultServerName,
                                     Date            = Timestamp.Now
                                 };


                #region Send OnFirmwareStatusNotificationSOAPResponse event

                try
                {

                    OnFirmwareStatusNotificationSOAPResponse?.Invoke(Timestamp.Now,
                                                                     SOAPServer.HTTPServer,
                                                                     Request,
                                                                     HTTPResponse);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

        }

        #endregion


    }

}
