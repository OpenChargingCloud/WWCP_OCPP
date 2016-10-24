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

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler          OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate OnStopTransactionRequest;

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
                                                         _OCPPHeader.To,   // Fake it!
                                                         _OCPPHeader.From, // Fake it!
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


        }

        #endregion

    }

}
