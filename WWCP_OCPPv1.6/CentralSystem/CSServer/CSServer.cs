/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

using org.GraphDefined.WWCP.OCPPv1_6.CP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP HTTP/SOAP/XML Central System Server API.
    /// </summary>
    public class CSServer : ASOAPServer
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
        public new static readonly HTTPPath         DefaultURIPrefix       = HTTPPath.Parse("/");

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
        public event RequestLogHandler           OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate  OnBootNotificationRequest;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate  OnHeartbeatRequest;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler     OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate  OnAuthorizeRequest;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler     OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnAuthorizeDelegate  OnStartTransactionRequest;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler     OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnAuthorizeDelegate  OnStatusNotificationRequest;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler     OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnAuthorizeDelegate  OnMeterValuesRequest;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler          OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate  OnStopTransactionRequest;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler       OnDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnDataTransferDelegate  OnDataTransferRequest;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                        OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate  OnDiagnosticsStatusNotificationRequest;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                     OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate  OnFirmwareStatusNotificationRequest;

        #endregion

        #endregion

        #region Constructor(s)

        #region CSServer(HTTPServerName, TCPPort = default, URIPrefix = default, ContentType = default, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the OCPP HTTP/SOAP/XML Central System API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSServer(String          HTTPServerName           = DefaultHTTPServerName,
                        IPPort?         TCPPort                  = null,
                        HTTPPath?       URIPrefix                = null,
                        HTTPContentType ContentType              = null,
                        Boolean         RegisterHTTPRootService  = true,
                        DNSClient       DNSClient                = null,
                        Boolean         AutoStart                = false)

            : base(HTTPServerName.IsNotNullOrEmpty() ? HTTPServerName : DefaultHTTPServerName,
                   TCPPort     ?? DefaultHTTPServerPort,
                   URIPrefix   ?? DefaultURIPrefix,
                   ContentType ?? DefaultContentType,
                   RegisterHTTPRootService,
                   DNSClient,
                   AutoStart: false)

        {

            RegisterURITemplates();

            if (AutoStart)
                Start();

        }

        #endregion

        #region CSServer(SOAPServer, URIPrefix = DefaultURIPrefix)

        /// <summary>
        /// Use the given HTTP server for the OCPP HTTP/SOAP/XML Central System API.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        public CSServer(SOAPServer  SOAPServer,
                        HTTPPath?   URIPrefix = null)

            : base(SOAPServer,
                   URIPrefix ?? DefaultURIPrefix)

        {

            RegisterURITemplates();

        }

        #endregion

        #endregion


        #region RegisterURITemplates()

        /// <summary>
        /// Register all URI templates for this SOAP API.
        /// </summary>
        protected void RegisterURITemplates()
        {

            #region / - BootNotification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "BootNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, BootNotificationXML) => {

                #region Send OnBootNotificationSOAPRequest event

                try
                {

                    OnBootNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                          this.SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnBootNotificationSOAPRequest));
                }

                #endregion


                var _OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                var _BootNotificationRequest  = BootNotificationRequest.Parse(BootNotificationXML);

                BootNotificationResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnBootNotificationRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _BootNotificationRequest.ChargePointVendor,
                                           _BootNotificationRequest.ChargePointModel,
                                           _BootNotificationRequest.ChargePointSerialNumber,
                                           _BootNotificationRequest.FirmwareVersion,
                                           _BootNotificationRequest.Iccid,
                                           _BootNotificationRequest.IMSI,
                                           _BootNotificationRequest.MeterType,
                                           _BootNotificationRequest.MeterSerialNumber,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = BootNotificationResponse.Failed(_BootNotificationRequest);

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/BootNotificationResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnBootNotificationSOAPResponse event

                try
                {

                    OnBootNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                           this.SOAPServer.HTTPServer,
                                                           Request,
                                                           HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnBootNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - Heartbeat

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "Heartbeat",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "heartbeatRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, HeartbeatXML) => {

                #region Send OnHeartbeatSOAPRequest event

                try
                {

                    OnHeartbeatSOAPRequest?.Invoke(DateTime.UtcNow,
                                                   this.SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnHeartbeatSOAPRequest));
                }

                #endregion


                var _OCPPHeader        = SOAPHeader.Parse(HeaderXML);
                var _HeartbeatRequest  = HeartbeatRequest.Parse(HeartbeatXML);

                HeartbeatResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnHeartbeatRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = HeartbeatResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/HeartbeatResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id!
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnHeartbeatSOAPResponse event

                try
                {

                    OnHeartbeatSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                    this.SOAPServer.HTTPServer,
                                                    Request,
                                                    HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnHeartbeatSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion


            #region / - Authorize

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "Authorize",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "authorizeRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, AuthorizeXML) => {

                #region Send OnAuthorizeSOAPRequest event

                try
                {

                    OnAuthorizeSOAPRequest?.Invoke(DateTime.UtcNow,
                                                   this.SOAPServer.HTTPServer,
                                                   Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnAuthorizeSOAPRequest));
                }

                #endregion


                var _OCPPHeader        = SOAPHeader.Parse(HeaderXML);
                var _AuthorizeRequest  = AuthorizeRequest.Parse(AuthorizeXML);

                AuthorizeResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnAuthorizeRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _AuthorizeRequest.IdTag,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = AuthorizeResponse.Failed(_AuthorizeRequest);

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/AuthorizeResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnAuthorizeSOAPResponse event

                try
                {

                    OnAuthorizeSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                    this.SOAPServer.HTTPServer,
                                                    Request,
                                                    HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnAuthorizeSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StartTransaction

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "StartTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, StartTransactionXML) => {

                #region Send OnStartTransactionSOAPRequest event

                try
                {

                    OnStartTransactionSOAPRequest?.Invoke(DateTime.UtcNow,
                                                          this.SOAPServer.HTTPServer,
                                                          Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStartTransactionSOAPRequest));
                }

                #endregion


                var _OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                var _StartTransactionRequest  = StartTransactionRequest.Parse(StartTransactionXML);

                StartTransactionResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnStartTransactionRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _StartTransactionRequest.ConnectorId,
                                           _StartTransactionRequest.IdTag,
                                           _StartTransactionRequest.Timestamp,
                                           _StartTransactionRequest.MeterStart,
                                           _StartTransactionRequest.ReservationId,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = StartTransactionResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/StartTransactionResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnStartTransactionSOAPResponse event

                try
                {

                    OnStartTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                           this.SOAPServer.HTTPServer,
                                                           Request,
                                                           HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStartTransactionSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StatusNotification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "StatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "statusNotificationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, StatusNotificationXML) => {

                #region Send OnStatusNotificationSOAPRequest event

                try
                {

                    OnStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                            this.SOAPServer.HTTPServer,
                                                            Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStatusNotificationSOAPRequest));
                }

                #endregion


                var _OCPPHeader                 = SOAPHeader.Parse(HeaderXML);
                var _StatusNotificationRequest  = StatusNotificationRequest.Parse(StatusNotificationXML);

                StatusNotificationResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnStatusNotificationRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _StatusNotificationRequest.ConnectorId,
                                           _StatusNotificationRequest.Status,
                                           _StatusNotificationRequest.ErrorCode,
                                           _StatusNotificationRequest.Info,
                                           _StatusNotificationRequest.StatusTimestamp,
                                           _StatusNotificationRequest.VendorId,
                                           _StatusNotificationRequest.VendorErrorCode,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = StatusNotificationResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/StatusNotificationResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnStatusNotificationSOAPResponse event

                try
                {

                    OnStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                             this.SOAPServer.HTTPServer,
                                                             Request,
                                                             HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - MeterValues

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "MeterValues",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "meterValuesRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, MeterValuesXML) => {

                #region Send OnMeterValuesSOAPRequest event

                try
                {

                    OnMeterValuesSOAPRequest?.Invoke(DateTime.UtcNow,
                                                     this.SOAPServer.HTTPServer,
                                                     Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnMeterValuesSOAPRequest));
                }

                #endregion


                var _OCPPHeader          = SOAPHeader.Parse(HeaderXML);
                var _MeterValuesRequest  = MeterValuesRequest.Parse(MeterValuesXML);

                MeterValuesResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnMeterValuesRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _MeterValuesRequest.ConnectorId,
                                           _MeterValuesRequest.TransactionId,
                                           _MeterValuesRequest.MeterValues,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = MeterValuesResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/MeterValuesResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnMeterValuesSOAPResponse event

                try
                {

                    OnMeterValuesSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                      this.SOAPServer.HTTPServer,
                                                      Request,
                                                      HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnMeterValuesSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - StopTransaction

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "StopTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, StopTransactionXML) => {

                #region Send OnStopTransactionSOAPRequest event

                try
                {

                    OnStopTransactionSOAPRequest?.Invoke(DateTime.UtcNow,
                                                         this.SOAPServer.HTTPServer,
                                                         Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStopTransactionSOAPRequest));
                }

                #endregion


                var _OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                var _StopTransactionRequest  = StopTransactionRequest.Parse(StopTransactionXML);

                StopTransactionResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnStopTransactionRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _StopTransactionRequest.TransactionId,
                                           _StopTransactionRequest.Timestamp,
                                           _StopTransactionRequest.MeterStop,
                                           _StopTransactionRequest.IdTag,
                                           _StopTransactionRequest.Reason,
                                           _StopTransactionRequest.TransactionData,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = StopTransactionResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/StopTransactionResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnStopTransactionSOAPResponse event

                try
                {

                    OnStopTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                          this.SOAPServer.HTTPServer,
                                                          Request,
                                                          HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnStopTransactionSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion


            #region / - DataTransfer

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "DataTransfer",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "dataTransferRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, DataTransferXML) => {

                #region Send OnDataTransferSOAPRequest event

                try
                {

                    OnDataTransferSOAPRequest?.Invoke(DateTime.UtcNow,
                                                      this.SOAPServer.HTTPServer,
                                                      Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnDataTransferSOAPRequest));
                }

                #endregion


                var _OCPPHeader           = SOAPHeader.Parse(HeaderXML);
                var _DataTransferRequest  = DataTransferRequest.Parse(DataTransferXML);

                DataTransferResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnDataTransferRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDataTransferDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _DataTransferRequest.VendorId,
                                           _DataTransferRequest.MessageId,
                                           _DataTransferRequest.Data,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = DataTransferResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/DataTransferResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnDataTransferSOAPResponse event

                try
                {

                    OnDataTransferSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                       this.SOAPServer.HTTPServer,
                                                       Request,
                                                       HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnDataTransferSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - DiagnosticsStatusNotification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "DiagnosticsStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, DiagnosticsStatusNotificationXML) => {

                #region Send OnDiagnosticsStatusNotificationSOAPRequest event

                try
                {

                    OnDiagnosticsStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                                       this.SOAPServer.HTTPServer,
                                                                       Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPRequest));
                }

                #endregion


                var _OCPPHeader                            = SOAPHeader.Parse(HeaderXML);
                var _DiagnosticsStatusNotificationRequest  = DiagnosticsStatusNotificationRequest.Parse(DiagnosticsStatusNotificationXML);

                DiagnosticsStatusNotificationResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnDiagnosticsStatusNotificationRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _DiagnosticsStatusNotificationRequest.Status,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = DiagnosticsStatusNotificationResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/DiagnosticsStatusNotificationResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnDiagnosticsStatusNotificationSOAPResponse event

                try
                {

                    OnDiagnosticsStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                        this.SOAPServer.HTTPServer,
                                                                        Request,
                                                                        HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnDiagnosticsStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

            #region / - FirmwareStatusNotification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "FirmwareStatusNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, FirmwareStatusNotificationXML) => {

                #region Send OnFirmwareStatusNotificationSOAPRequest event

                try
                {

                    OnFirmwareStatusNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                                                    this.SOAPServer.HTTPServer,
                                                                    Request);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnFirmwareStatusNotificationSOAPRequest));
                }

                #endregion


                var _OCPPHeader               = SOAPHeader.Parse(HeaderXML);
                var _FirmwareStatusNotificationRequest  = FirmwareStatusNotificationRequest.Parse(FirmwareStatusNotificationXML);

                FirmwareStatusNotificationResponse response            = null;



                #region Call async subscribers

                if (response == null)
                {

                    var results = OnFirmwareStatusNotificationRequest?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _FirmwareStatusNotificationRequest.Status,
                                           DefaultRequestTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = FirmwareStatusNotificationResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponse.Builder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.HTTPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.XMLTEXT_UTF8,
                    Content         = SOAP.Encapsulation(_OCPPHeader.ChargeBoxIdentity,
                                                         "/FirmwareStatusNotificationResponse",
                                                         null,
                                                         _OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                         _OCPPHeader.To,         // Fake it!
                                                         _OCPPHeader.From,       // Fake it!
                                                         response.ToXML()).ToUTF8Bytes()
                };

                #endregion


                #region Send OnFirmwareStatusNotificationSOAPResponse event

                try
                {

                    OnFirmwareStatusNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                     this.SOAPServer.HTTPServer,
                                                                     Request,
                                                                     HTTPResponse);

                }
                catch (Exception e)
                {
                    e.Log(nameof(CSServer) + "." + nameof(OnFirmwareStatusNotificationSOAPResponse));
                }

                #endregion

                return HTTPResponse;

            });

            #endregion

        }

        #endregion

    }

}
