/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

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
        public new const           String           DefaultHTTPServerName  = "GraphDefined OCPP " + Version.Number + " HTTP/SOAP/XML Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort           DefaultHTTPServerPort  = IPPort.Parse(2010);

        /// <summary>
        /// The default TCP service name shown e.g. on service startup.
        /// </summary>
        public     const           String           DefaultServiceName     = "OCPP " + Version.Number + " Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new static readonly HTTPPath         DefaultURLPrefix       = HTTPPath.Parse("/" + Version.Number);

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public new static readonly HTTPContentType  DefaultContentType     = HTTPContentType.XMLTEXT_UTF8;

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
        public IEnumerable<ChargeBox_Id>  ChargeBoxIds    { get; }

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                 OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event BootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate          OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event BootNotificationResponseDelegate  OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler                  OnBootNotificationSOAPResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler          OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event HeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate          OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event HeartbeatResponseDelegate  OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler           OnHeartbeatSOAPResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler            OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event AuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate          OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event AuthorizeResponseDelegate  OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler             OnAuthorizeSOAPResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                   OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event StartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate          OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event StartTransactionResponseDelegate  OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler                    OnStartTransactionSOAPResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                     OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event StatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate          OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event StatusNotificationResponseDelegate  OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler                      OnStatusNotificationSOAPResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler              OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event MeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate          OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event MeterValuesResponseDelegate  OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler               OnMeterValuesSOAPResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                  OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event StopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate          OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event StopTransactionResponseDelegate  OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler                   OnStopTransactionSOAPResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler                       OnIncomingDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event IncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate          OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event IncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler                        OnIncomingDataTransferSOAPResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                                OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event DiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate          OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event DiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler                                 OnDiagnosticsStatusNotificationSOAPResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                             OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event FirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate          OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event FirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler                              OnFirmwareStatusNotificationSOAPResponse;

        #endregion

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
        public CentralSystemSOAPServer(String           HTTPServerName            = DefaultHTTPServerName,
                                       IPPort?          TCPPort                   = null,
                                       String           ServiceName               = null,
                                       HTTPPath?        URLPrefix                 = null,
                                       HTTPContentType  ContentType               = null,
                                       Boolean          RegisterHTTPRootService   = true,
                                       DNSClient        DNSClient                 = null,
                                       Boolean          AutoStart                 = false)

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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "BootNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest").FirstOrDefault(),
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

                BootNotificationResponse response      = null;
                HTTPResponse             HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = BootNotificationRequest.Parse(BootNotificationXML,
                                                                    Request_Id.Parse(OCPPHeader.MessageId),
                                                                    OCPPHeader.ChargeBoxIdentity);

                    #region Send OnBootNotificationRequest event

                    try
                    {

                        OnBootNotificationRequest?.Invoke(request.RequestTimestamp,
                                                          this,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnBootNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = BootNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnBootNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnBootNotificationResponse?.Invoke(responseTimestamp,
                                                           this,
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
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/BootNotificationResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnBootNotificationSOAPResponse event

                try
                {

                    OnBootNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "Heartbeat",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "heartbeatRequest").FirstOrDefault(),
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

                HeartbeatResponse response      = null;
                HTTPResponse      HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = HeartbeatRequest.Parse(HeartbeatXML,
                                                             Request_Id.Parse(OCPPHeader.MessageId),
                                                             OCPPHeader.ChargeBoxIdentity);

                    #region Send OnHeartbeatRequest event

                    try
                    {

                        OnHeartbeatRequest?.Invoke(request.RequestTimestamp,
                                                   this,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnHeartbeat?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = HeartbeatResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnHeartbeatResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnHeartbeatResponse?.Invoke(responseTimestamp,
                                                    this,
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
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/HeartbeatResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnHeartbeatSOAPResponse event

                try
                {

                    OnHeartbeatSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "Authorize",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "authorizeRequest").FirstOrDefault(),
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

                AuthorizeResponse response      = null;
                HTTPResponse      HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = AuthorizeRequest.Parse(AuthorizeXML,
                                                             Request_Id.Parse(OCPPHeader.MessageId),
                                                             OCPPHeader.ChargeBoxIdentity);

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(request.RequestTimestamp,
                                                   this,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnAuthorize?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = AuthorizeResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnAuthorizeResponse?.Invoke(responseTimestamp,
                                                    this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/AuthorizeResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnAuthorizeSOAPResponse event

                try
                {

                    OnAuthorizeSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "StartTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault(),
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

                StartTransactionResponse response      = null;
                HTTPResponse             HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StartTransactionRequest.Parse(StartTransactionXML,
                                                                    Request_Id.Parse(OCPPHeader.MessageId),
                                                                    OCPPHeader.ChargeBoxIdentity);

                    #region Send OnStartTransactionRequest event

                    try
                    {

                        OnStartTransactionRequest?.Invoke(request.RequestTimestamp,
                                                          this,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStartTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StartTransactionResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStartTransactionResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStartTransactionResponse?.Invoke(responseTimestamp,
                                                           this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StartTransactionResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnStartTransactionSOAPResponse event

                try
                {

                    OnStartTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "StatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "statusNotificationRequest").FirstOrDefault(),
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

                StatusNotificationResponse response      = null;
                HTTPResponse               HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StatusNotificationRequest.Parse(StatusNotificationXML,
                                                                      Request_Id.Parse(OCPPHeader.MessageId),
                                                                      OCPPHeader.ChargeBoxIdentity);

                    #region Send OnStatusNotificationRequest event

                    try
                    {

                        OnStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                            this,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStatusNotificationResponse?.Invoke(responseTimestamp,
                                                             this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StatusNotificationResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnStatusNotificationSOAPResponse event

                try
                {

                    OnStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "MeterValues",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "meterValuesRequest").FirstOrDefault(),
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

                MeterValuesResponse response      = null;
                HTTPResponse        HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = MeterValuesRequest.Parse(MeterValuesXML,
                                                               Request_Id.Parse(OCPPHeader.MessageId),
                                                               OCPPHeader.ChargeBoxIdentity);

                    #region Send OnMeterValuesRequest event

                    try
                    {

                        OnMeterValuesRequest?.Invoke(request.RequestTimestamp,
                                                     this,
                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnMeterValues?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = MeterValuesResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnMeterValuesResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnMeterValuesResponse?.Invoke(responseTimestamp,
                                                      this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/MeterValuesResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnMeterValuesSOAPResponse event

                try
                {

                    OnMeterValuesSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "StopTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault(),
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

                StopTransactionResponse response      = null;
                HTTPResponse            HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = StopTransactionRequest.Parse(StopTransactionXML,
                                                                   Request_Id.Parse(OCPPHeader.MessageId),
                                                                   OCPPHeader.ChargeBoxIdentity);

                    #region Send OnStopTransactionRequest event

                    try
                    {

                        OnStopTransactionRequest?.Invoke(request.RequestTimestamp,
                                                         this,
                                                         request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStopTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StopTransactionResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnStopTransactionResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnStopTransactionResponse?.Invoke(responseTimestamp,
                                                          this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StopTransactionResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnStopTransactionSOAPResponse event

                try
                {

                    OnStopTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "DataTransfer",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "dataTransferRequest").FirstOrDefault(),
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


                DataTransferResponse response      = null;
                HTTPResponse         HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = CP.DataTransferRequest.Parse(DataTransferXML,
                                                                   Request_Id.Parse(OCPPHeader.MessageId),
                                                                   OCPPHeader.ChargeBoxIdentity);

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(request.RequestTimestamp,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnIncomingDataTransfer?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = DataTransferResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnIncomingDataTransferResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnIncomingDataTransferResponse?.Invoke(responseTimestamp,
                                                               this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DataTransferResponse",
                                                             Request_Id.Random().ToString(),
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.From,
                                                             OCPPHeader.To,
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/IncomingDataTransfer");
                }


                #region Send OnIncomingDataTransferSOAPResponse event

                try
                {

                    OnIncomingDataTransferSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "DiagnosticsStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest").FirstOrDefault(),
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


                DiagnosticsStatusNotificationResponse response      = null;
                HTTPResponse                          HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = DiagnosticsStatusNotificationRequest.Parse(DiagnosticsStatusNotificationXML,
                                                                                 Request_Id.Parse(OCPPHeader.MessageId),
                                                                                 OCPPHeader.ChargeBoxIdentity);

                    #region Send OnDiagnosticsStatusNotificationRequest event

                    try
                    {

                        OnDiagnosticsStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                                       this,
                                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnDiagnosticsStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = DiagnosticsStatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnDiagnosticsStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnDiagnosticsStatusNotificationResponse?.Invoke(responseTimestamp,
                                                                        this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DiagnosticsStatusNotificationResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnDiagnosticsStatusNotificationSOAPResponse event

                try
                {

                    OnDiagnosticsStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "FirmwareStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationRequest").FirstOrDefault(),
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


                FirmwareStatusNotificationResponse response      = null;
                HTTPResponse                       HTTPResponse  = null;

                try
                {

                    var OCPPHeader  = SOAPHeader.Parse(HeaderXML);
                    var request     = FirmwareStatusNotificationRequest.Parse(FirmwareStatusNotificationXML,
                                                                              Request_Id.Parse(OCPPHeader.MessageId),
                                                                              OCPPHeader.ChargeBoxIdentity);

                    #region Send OnFirmwareStatusNotificationRequest event

                    try
                    {

                        OnFirmwareStatusNotificationRequest?.Invoke(request.RequestTimestamp,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnFirmwareStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                              (Timestamp.Now,
                                               this,
                                               request,
                                               Request.CancellationToken)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = FirmwareStatusNotificationResponse.Failed(request);

                    }

                    #endregion

                    #region Send OnFirmwareStatusNotificationResponse event

                    try
                    {

                        var responseTimestamp = Timestamp.Now;

                        OnFirmwareStatusNotificationResponse?.Invoke(responseTimestamp,
                                                                     this,
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
                        Date            = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/FirmwareStatusNotificationResponse",
                                                             Request_Id.Random().ToString(),
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


                #region Send OnFirmwareStatusNotificationSOAPResponse event

                try
                {

                    OnFirmwareStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
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
