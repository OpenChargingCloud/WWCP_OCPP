/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.WWCP.OCPPv1_6.CP;
using System;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// The central system HTTP/SOAP/XML server.
    /// </summary>
    public class CentralSystemSOAPServer : ASOAPServer
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
        public event BootNotificationDelegate          OnBootNotification;

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
        public event HeartbeatDelegate          OnHeartbeat;

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
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate          OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

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
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate          OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

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
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate          OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

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
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate          OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

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
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate          OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

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
        public event OnIncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate          OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

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
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate          OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

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
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate          OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

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
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemSOAPServer(String           HTTPServerName            = DefaultHTTPServerName,
                                       IPPort?          TCPPort                   = null,
                                       HTTPPath?        URLPrefix                 = null,
                                       HTTPContentType  ContentType               = null,
                                       Boolean          RegisterHTTPRootService   = true,
                                       DNSClient        DNSClient                 = null,
                                       Boolean          AutoStart                 = false)

            : base(HTTPServerName.IsNotNullOrEmpty()
                       ? HTTPServerName
                       : DefaultHTTPServerName,
                   TCPPort     ?? DefaultHTTPServerPort,
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

                try
                {

                    OnBootNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                          SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPRequest));
                }

                #endregion

                BootNotificationResponse response      = null;
                HTTPResponse             HTTPResponse  = null;

                try
                {

                    var OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                    var bootNotificationRequest  = BootNotificationRequest.Parse(BootNotificationXML);

                    #region Send OnBootNotificationRequest event

                    try
                    {

                        OnBootNotificationRequest?.Invoke(bootNotificationRequest.RequestTimestamp,
                                                          this,
                                                          Request.EventTrackingId,

                                                          OCPPHeader.ChargeBoxIdentity,

                                                          bootNotificationRequest.ChargePointVendor,
                                                          bootNotificationRequest.ChargePointModel,
                                                          bootNotificationRequest.ChargePointSerialNumber,
                                                          bootNotificationRequest.ChargeBoxSerialNumber,
                                                          bootNotificationRequest.FirmwareVersion,
                                                          bootNotificationRequest.Iccid,
                                                          bootNotificationRequest.IMSI,
                                                          bootNotificationRequest.MeterType,
                                                          bootNotificationRequest.MeterSerialNumber);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnBootNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as BootNotificationDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               bootNotificationRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = BootNotificationResponse.Failed(bootNotificationRequest);

                    }

                    #endregion

                    #region Send OnBootNotificationResponse event

                    try
                    {

                        OnBootNotificationResponse?.Invoke(response.ResponseTimestamp,
                                                           this,
                                                           Request.EventTrackingId,

                                                           OCPPHeader.ChargeBoxIdentity,

                                                           bootNotificationRequest.ChargePointVendor,
                                                           bootNotificationRequest.ChargePointModel,
                                                           bootNotificationRequest.ChargePointSerialNumber,
                                                           bootNotificationRequest.ChargeBoxSerialNumber,
                                                           bootNotificationRequest.FirmwareVersion,
                                                           bootNotificationRequest.Iccid,
                                                           bootNotificationRequest.IMSI,
                                                           bootNotificationRequest.MeterType,
                                                           bootNotificationRequest.MeterSerialNumber,

                                                           response.Result,
                                                           response.Status,
                                                           response.CurrentTime,
                                                           response.Interval,
                                                           response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/BootNotificationResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPResponse));
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

                try
                {

                    OnHeartbeatSOAPRequest?.Invoke(DateTime.UtcNow,
                                                   SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatSOAPRequest));
                }

                #endregion

                HeartbeatResponse response      = null;
                HTTPResponse      HTTPResponse  = null;

                try
                {

                    var OCPPHeader        = SOAPHeader.Parse(HeaderXML);
                    var heartbeatRequest  = HeartbeatRequest.Parse(HeartbeatXML);

                    #region Send OnHeartbeatRequest event

                    try
                    {

                        OnHeartbeatRequest?.Invoke(heartbeatRequest.RequestTimestamp,
                                                   this,
                                                   Request.EventTrackingId,

                                                   OCPPHeader.ChargeBoxIdentity);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnHeartbeat?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as HeartbeatDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               heartbeatRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = HeartbeatResponse.Failed(heartbeatRequest);

                    }

                    #endregion

                    #region Send OnHeartbeatResponse event

                    try
                    {

                        OnHeartbeatResponse?.Invoke(response.ResponseTimestamp,
                                                    this,
                                                    Request.EventTrackingId,

                                                    OCPPHeader.ChargeBoxIdentity,

                                                    response.Result,
                                                    response.CurrentTime,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/HeartbeatResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnHeartbeatSOAPResponse));
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

                try
                {

                    OnAuthorizeSOAPRequest?.Invoke(DateTime.UtcNow,
                                                   SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeSOAPRequest));
                }

                #endregion

                AuthorizeResponse response      = null;
                HTTPResponse      HTTPResponse  = null;

                try
                {

                    var OCPPHeader        = SOAPHeader.Parse(HeaderXML);
                    var authorizeRequest  = AuthorizeRequest.Parse(AuthorizeXML);

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(authorizeRequest.RequestTimestamp,
                                                   this,
                                                   Request.EventTrackingId,

                                                   OCPPHeader.ChargeBoxIdentity,

                                                   authorizeRequest.IdTag);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnAuthorize?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               authorizeRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = AuthorizeResponse.Failed(authorizeRequest);

                    }

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        OnAuthorizeResponse?.Invoke(response.ResponseTimestamp,
                                                    this,
                                                    Request.EventTrackingId,

                                                    OCPPHeader.ChargeBoxIdentity,

                                                    authorizeRequest.IdTag,

                                                    response.Result,
                                                    response.IdTagInfo,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/AuthorizeResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnAuthorizeSOAPResponse));
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

                try
                {

                    OnStartTransactionSOAPRequest?.Invoke(DateTime.UtcNow,
                                                          SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionSOAPRequest));
                }

                #endregion

                StartTransactionResponse response      = null;
                HTTPResponse             HTTPResponse  = null;

                try
                {

                    var OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                    var startTransactionRequest  = StartTransactionRequest.Parse(StartTransactionXML);

                    #region Send OnStartTransactionRequest event

                    try
                    {

                        OnStartTransactionRequest?.Invoke(startTransactionRequest.RequestTimestamp,
                                                          this,
                                                          Request.EventTrackingId,

                                                          OCPPHeader.ChargeBoxIdentity,

                                                          startTransactionRequest.ConnectorId,
                                                          startTransactionRequest.IdTag,
                                                          startTransactionRequest.StartTimestamp,
                                                          startTransactionRequest.MeterStart,
                                                          startTransactionRequest.ReservationId);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStartTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               startTransactionRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StartTransactionResponse.Failed(startTransactionRequest);

                    }

                    #endregion

                    #region Send OnStartTransactionResponse event

                    try
                    {

                        OnStartTransactionResponse?.Invoke(response.ResponseTimestamp,
                                                           this,
                                                           Request.EventTrackingId,

                                                           OCPPHeader.ChargeBoxIdentity,

                                                           startTransactionRequest.ConnectorId,
                                                           startTransactionRequest.IdTag,
                                                           startTransactionRequest.StartTimestamp,
                                                           startTransactionRequest.MeterStart,
                                                           startTransactionRequest.ReservationId,

                                                           response.Result,
                                                           response.TransactionId,
                                                           response.IdTagInfo,
                                                           response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StartTransactionResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStartTransactionSOAPResponse));
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

                try
                {

                    OnStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                            SOAPServer.HTTPServer,
                                                            Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationSOAPRequest));
                }

                #endregion

                StatusNotificationResponse response      = null;
                HTTPResponse               HTTPResponse  = null;

                try
                {

                    var OCPPHeader                 = SOAPHeader.Parse(HeaderXML);
                    var statusNotificationRequest  = StatusNotificationRequest.Parse(StatusNotificationXML);

                    #region Send OnStatusNotificationRequest event

                    try
                    {

                        OnStatusNotificationRequest?.Invoke(statusNotificationRequest.RequestTimestamp,
                                                            this,
                                                            Request.EventTrackingId,

                                                            OCPPHeader.ChargeBoxIdentity,

                                                            statusNotificationRequest.ConnectorId,
                                                            statusNotificationRequest.Status,
                                                            statusNotificationRequest.ErrorCode,
                                                            statusNotificationRequest.Info,
                                                            statusNotificationRequest.StatusTimestamp,
                                                            statusNotificationRequest.VendorId,
                                                            statusNotificationRequest.VendorErrorCode);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               statusNotificationRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StatusNotificationResponse.Failed(statusNotificationRequest);

                    }

                    #endregion

                    #region Send OnStatusNotificationResponse event

                    try
                    {

                        OnStatusNotificationResponse?.Invoke(response.ResponseTimestamp,
                                                             this,
                                                             Request.EventTrackingId,

                                                             OCPPHeader.ChargeBoxIdentity,

                                                             statusNotificationRequest.ConnectorId,
                                                             statusNotificationRequest.Status,
                                                             statusNotificationRequest.ErrorCode,
                                                             statusNotificationRequest.Info,
                                                             statusNotificationRequest.StatusTimestamp,
                                                             statusNotificationRequest.VendorId,
                                                             statusNotificationRequest.VendorErrorCode,

                                                             response.Result,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StatusNotificationResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStatusNotificationSOAPResponse));
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

                try
                {

                    OnMeterValuesSOAPRequest?.Invoke(DateTime.UtcNow,
                                                     SOAPServer.HTTPServer,
                                                     Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesSOAPRequest));
                }

                #endregion

                MeterValuesResponse response      = null;
                HTTPResponse        HTTPResponse  = null;

                try
                {

                    var OCPPHeader          = SOAPHeader.Parse(HeaderXML);
                    var meterValuesRequest  = MeterValuesRequest.Parse(MeterValuesXML);

                    #region Send OnMeterValuesRequest event

                    try
                    {

                        OnMeterValuesRequest?.Invoke(meterValuesRequest.RequestTimestamp,
                                                     this,
                                                     Request.EventTrackingId,

                                                     OCPPHeader.ChargeBoxIdentity,

                                                     meterValuesRequest.ConnectorId,
                                                     meterValuesRequest.TransactionId,
                                                     meterValuesRequest.MeterValues);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnMeterValues?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               meterValuesRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = MeterValuesResponse.Failed(meterValuesRequest);

                    }

                    #endregion

                    #region Send OnMeterValuesResponse event

                    try
                    {

                        OnMeterValuesResponse?.Invoke(response.ResponseTimestamp,
                                                      this,
                                                      Request.EventTrackingId,

                                                      OCPPHeader.ChargeBoxIdentity,

                                                      meterValuesRequest.ConnectorId,
                                                      meterValuesRequest.TransactionId,
                                                      meterValuesRequest.MeterValues,

                                                      response.Result,
                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/MeterValuesResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnMeterValuesSOAPResponse));
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

                try
                {

                    OnStopTransactionSOAPRequest?.Invoke(DateTime.UtcNow,
                                                         SOAPServer.HTTPServer,
                                                         Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionSOAPRequest));
                }

                #endregion

                StopTransactionResponse response      = null;
                HTTPResponse            HTTPResponse  = null;

                try
                {

                    var OCPPHeader              = SOAPHeader.Parse(HeaderXML);
                    var stopTransactionRequest  = StopTransactionRequest.Parse(StopTransactionXML);

                    #region Send OnStopTransactionRequest event

                    try
                    {

                        OnStopTransactionRequest?.Invoke(stopTransactionRequest.RequestTimestamp,
                                                         this,
                                                         Request.EventTrackingId,

                                                         OCPPHeader.ChargeBoxIdentity,

                                                         stopTransactionRequest.TransactionId,
                                                         stopTransactionRequest.Timestamp,
                                                         stopTransactionRequest.MeterStop,
                                                         stopTransactionRequest.IdTag,
                                                         stopTransactionRequest.Reason,
                                                         stopTransactionRequest.TransactionData);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnStopTransaction?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               stopTransactionRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = StopTransactionResponse.Failed(stopTransactionRequest);

                    }

                    #endregion

                    #region Send OnStopTransactionResponse event

                    try
                    {

                        OnStopTransactionResponse?.Invoke(response.ResponseTimestamp,
                                                          this,
                                                          Request.EventTrackingId,

                                                          OCPPHeader.ChargeBoxIdentity,

                                                          stopTransactionRequest.TransactionId,
                                                          stopTransactionRequest.Timestamp,
                                                          stopTransactionRequest.MeterStop,
                                                          stopTransactionRequest.IdTag,
                                                          stopTransactionRequest.Reason,
                                                          stopTransactionRequest.TransactionData,

                                                          response.Result,
                                                          response.IdTagInfo,
                                                          response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/StopTransactionResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnStopTransactionSOAPResponse));
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

                try
                {

                    OnIncomingDataTransferSOAPRequest?.Invoke(DateTime.UtcNow,
                                                              SOAPServer.HTTPServer,
                                                              Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPRequest));
                }

                #endregion


                DataTransferResponse response      = null;
                HTTPResponse                 HTTPResponse  = null;

                try
                {

                    var OCPPHeader           = SOAPHeader.Parse(HeaderXML);
                    var dataTransferRequest  = CP.DataTransferRequest.Parse(DataTransferXML);

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(dataTransferRequest.RequestTimestamp,
                                                              this,
                                                              Request.EventTrackingId,

                                                              OCPPHeader.ChargeBoxIdentity,

                                                              dataTransferRequest.VendorId,
                                                              dataTransferRequest.MessageId,
                                                              dataTransferRequest.Data);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnIncomingDataTransfer?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               dataTransferRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = DataTransferResponse.Failed(dataTransferRequest);

                    }

                    #endregion

                    #region Send OnIncomingDataTransferResponse event

                    try
                    {

                        OnIncomingDataTransferResponse?.Invoke(response.ResponseTimestamp,
                                                               this,
                                                               Request.EventTrackingId,

                                                               OCPPHeader.ChargeBoxIdentity,

                                                               dataTransferRequest.VendorId,
                                                               dataTransferRequest.Data,
                                                               dataTransferRequest.MessageId,

                                                               response.Result,
                                                               response.Status,
                                                               response.Data,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DataTransferResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPResponse));
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

                try
                {

                    OnDiagnosticsStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                                       SOAPServer.HTTPServer,
                                                                       Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPRequest));
                }

                #endregion


                DiagnosticsStatusNotificationResponse response      = null;
                HTTPResponse                          HTTPResponse  = null;

                try
                {

                    var OCPPHeader                            = SOAPHeader.Parse(HeaderXML);
                    var diagnosticsStatusNotificationRequest  = DiagnosticsStatusNotificationRequest.Parse(DiagnosticsStatusNotificationXML);

                    #region Send OnDiagnosticsStatusNotificationRequest event

                    try
                    {

                        OnDiagnosticsStatusNotificationRequest?.Invoke(diagnosticsStatusNotificationRequest.RequestTimestamp,
                                                                       this,
                                                                       Request.EventTrackingId,

                                                                       OCPPHeader.ChargeBoxIdentity,

                                                                       diagnosticsStatusNotificationRequest.Status);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnDiagnosticsStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               diagnosticsStatusNotificationRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = DiagnosticsStatusNotificationResponse.Failed(diagnosticsStatusNotificationRequest);

                    }

                    #endregion

                    #region Send OnDiagnosticsStatusNotificationResponse event

                    try
                    {

                        OnDiagnosticsStatusNotificationResponse?.Invoke(response.ResponseTimestamp,
                                                                        this,
                                                                        Request.EventTrackingId,

                                                                        OCPPHeader.ChargeBoxIdentity,

                                                                        diagnosticsStatusNotificationRequest.Status,

                                                                        response.Result,
                                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/DiagnosticsStatusNotificationResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPResponse));
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

                try
                {

                    OnFirmwareStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                                    SOAPServer.HTTPServer,
                                                                    Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationSOAPRequest));
                }

                #endregion


                FirmwareStatusNotificationResponse response      = null;
                HTTPResponse                       HTTPResponse  = null;

                try
                {

                    var OCPPHeader                         = SOAPHeader.Parse(HeaderXML);
                    var firmwareStatusNotificationRequest  = FirmwareStatusNotificationRequest.Parse(FirmwareStatusNotificationXML);

                    #region Send OnFirmwareStatusNotificationRequest event

                    try
                    {

                        OnFirmwareStatusNotificationRequest?.Invoke(firmwareStatusNotificationRequest.RequestTimestamp,
                                                                    this,
                                                                    Request.EventTrackingId,

                                                                    OCPPHeader.ChargeBoxIdentity,

                                                                    firmwareStatusNotificationRequest.Status);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    if (response == null)
                    {

                        var results = OnFirmwareStatusNotification?.
                                          GetInvocationList()?.
                                          SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                              (DateTime.UtcNow,
                                               this,
                                               Request.CancellationToken,
                                               Request.EventTrackingId,
                                               OCPPHeader.ChargeBoxIdentity,
                                               firmwareStatusNotificationRequest)).
                                          ToArray();

                        if (results.Length > 0)
                        {

                            await Task.WhenAll(results);

                            response = results.FirstOrDefault()?.Result;

                        }

                        if (results.Length == 0 || response == null)
                            response = FirmwareStatusNotificationResponse.Failed(firmwareStatusNotificationRequest);

                    }

                    #endregion

                    #region Send OnFirmwareStatusNotificationResponse event

                    try
                    {

                        OnFirmwareStatusNotificationResponse?.Invoke(response.ResponseTimestamp,
                                                                     this,
                                                                     Request.EventTrackingId,

                                                                     OCPPHeader.ChargeBoxIdentity,

                                                                     firmwareStatusNotificationRequest.Status,

                                                                     response.Result,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                    }

                    #endregion


                    #region Create HTTP Response

                    HTTPResponse = new HTTPResponse.Builder(Request) {
                        HTTPStatusCode  = HTTPStatusCode.OK,
                        Server          = SOAPServer.HTTPServer.DefaultServerName,
                        Date            = DateTime.UtcNow,
                        ContentType     = HTTPContentType.XMLTEXT_UTF8,
                        Content         = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                             "/FirmwareStatusNotificationResponse",
                                                             null,
                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                             OCPPHeader.To,         // Fake it!
                                                             OCPPHeader.From,       // Fake it!
                                                             response.ToXML()).ToUTF8Bytes()
                    };

                    #endregion

                }
                catch (Exception e)
                {

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
                    e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnFirmwareStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

        }

        #endregion

    }

}
