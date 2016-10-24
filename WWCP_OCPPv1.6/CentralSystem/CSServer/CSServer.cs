/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/GraphDefined/WWCP_OCPP>
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

using System;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;
using System.Threading.Tasks;

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
        public new const           String    DefaultHTTPServerName  = "GraphDefined OCPP v1.6 HTTP/SOAP/XML Central System Server API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort  = new IPPort(2010);

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new const           String    DefaultURIPrefix       = "v1.6";

        /// <summary>
        /// The default query timeout.
        /// </summary>
        public new static readonly TimeSpan  DefaultQueryTimeout    = TimeSpan.FromMinutes(1);

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                  OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler                   OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate  OnBootNotificationRequest;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler            OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate  OnHeartbeatRequest;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler            OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate  OnAuthorizeRequest;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler            OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate  OnStartTransactionRequest;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler            OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate  OnStatusNotificationRequest;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler            OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate  OnMeterValuesRequest;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                 OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler                  OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate  OnStopTransactionRequest;

        #endregion

        #endregion

        #region Constructor(s)

        #region CSServer(HTTPServerName, TCPPort = null, URIPrefix = DefaultURIPrefix, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the OCPP HTTP/SOAP/XML Central System Server API using IPAddress.Any.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Whether to start the server immediately or not.</param>
        public CSServer(String    HTTPServerName  = DefaultHTTPServerName,
                        IPPort    TCPPort         = null,
                        String    URIPrefix       = DefaultURIPrefix,
                        DNSClient DNSClient       = null,
                        Boolean   AutoStart       = false)

            : base(HTTPServerName.IsNotNullOrEmpty() ? HTTPServerName : DefaultHTTPServerName,
                   TCPPort ?? DefaultHTTPServerPort,
                   URIPrefix.     IsNotNullOrEmpty() ? URIPrefix      : DefaultURIPrefix,
                   HTTPContentType.SOAPXML_UTF8,
                   DNSClient,
                   AutoStart: false)

        {

            if (AutoStart)
                Start();

        }

        #endregion

        #region CSServer(SOAPServer, URIPrefix = DefaultURIPrefix)

        /// <summary>
        /// Use the given HTTP server for the OCPP HTTP/SOAP/XML Central System Server API using IPAddress.Any.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        public CSServer(SOAPServer  SOAPServer,
                        String      URIPrefix  = DefaultURIPrefix)

            : base(SOAPServer,
                   URIPrefix.IsNotNullOrEmpty() ? URIPrefix : DefaultURIPrefix)

        { }

        #endregion

        #endregion


        #region (override) RegisterURITemplates()

        protected override void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            SOAPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         new String[] { "/", URIPrefix + "/" },
                                         HTTPContentType.TEXT_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponseBuilder(Request) {

                                                 HTTPStatusCode  = HTTPStatusCode.BadGateway,
                                                 ContentType     = HTTPContentType.TEXT_UTF8,
                                                 Content         = ("Welcome at " + DefaultHTTPServerName + Environment.NewLine +
                                                                    "This is a HTTP/SOAP/XML endpoint!" + Environment.NewLine + Environment.NewLine +
                                                                    "Defined endpoints: " + Environment.NewLine + Environment.NewLine +
                                                                    SOAPServer.
                                                                        SOAPDispatchers.
                                                                        Select(group => " - " + group.Key + Environment.NewLine +
                                                                                        "   " + group.SelectMany(dispatcher => dispatcher.SOAPDispatches).
                                                                                                      Select    (dispatch   => dispatch.  Description).
                                                                                                      AggregateWith(", ")
                                                                              ).AggregateWith(Environment.NewLine + Environment.NewLine)
                                                                   ).ToUTF8Bytes(),
                                                 Connection      = "close"

                                             };

                                         },
                                         AllowReplacement: URIReplacement.Allow);

            #endregion


            #region / - BootNotification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "BootNotification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, BootNotificationXML) => {

                #region Send OnBootNotificationSOAPRequest event

                try
                {

                    OnBootNotificationSOAPRequest?.Invoke(DateTime.Now,
                                                          this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnBootNotificationRequestDelegate)
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
                                           DefaultQueryTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = BootNotificationResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                           this.SOAPServer,
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

                    OnHeartbeatSOAPRequest?.Invoke(DateTime.Now,
                                                   this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnHeartbeatRequestDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           DefaultQueryTimeout)).
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

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                    this.SOAPServer,
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

                    OnAuthorizeSOAPRequest?.Invoke(DateTime.Now,
                                                   this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnAuthorizeRequestDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _AuthorizeRequest.IdTag,
                                           DefaultQueryTimeout)).
                                      ToArray();

                    if (results.Length > 0)
                    {

                        await Task.WhenAll(results);

                        response = results.FirstOrDefault()?.Result;

                    }

                    if (results.Length == 0 || response == null)
                        response = AuthorizeResponse.Failed;

                }

                #endregion



                #region Create SOAPResponse

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                    this.SOAPServer,
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

                    OnStartTransactionSOAPRequest?.Invoke(DateTime.Now,
                                                          this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnStartTransactionRequestDelegate)
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
                                           DefaultQueryTimeout)).
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

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                           this.SOAPServer,
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

                    OnStatusNotificationSOAPRequest?.Invoke(DateTime.Now,
                                                            this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnStatusNotificationRequestDelegate)
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
                                           DefaultQueryTimeout)).
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

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                             this.SOAPServer,
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

                    OnMeterValuesSOAPRequest?.Invoke(DateTime.Now,
                                                     this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnMeterValuesRequestDelegate)
                                          (DateTime.Now,
                                           this,
                                           Request.CancellationToken,
                                           Request.EventTrackingId,
                                           _OCPPHeader.ChargeBoxIdentity,
                                           _MeterValuesRequest.ConnectorId,
                                           _MeterValuesRequest.TransactionId,
                                           _MeterValuesRequest.MeterValues,
                                           DefaultQueryTimeout)).
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

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                      this.SOAPServer,
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

                    OnStopTransactionSOAPRequest?.Invoke(DateTime.Now,
                                                         this.SOAPServer,
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
                                      SafeSelect(subscriber => (subscriber as OnStopTransactionRequestDelegate)
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
                                           DefaultQueryTimeout)).
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

                var HTTPResponse = new HTTPResponseBuilder(Request) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
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
                                                          this.SOAPServer,
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


        }

        #endregion

    }

}
