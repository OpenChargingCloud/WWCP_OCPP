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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP v1.5 HTTP/SOAP/XML Central System Server API.
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
        public     const           String    DefaultURIPrefix       = "v1.6";

        /// <summary>
        /// The default query timeout.
        /// </summary>
        public new static readonly TimeSpan  DefaultQueryTimeout    = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        #endregion

        #region Events

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to an authorize was sent.
        /// </summary>
        public event AccessLogHandler          OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeStartDelegate  OnAuthorize;

        #endregion

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler          OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnAuthorizeStartDelegate  OnBootNotification;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler          OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnAuthorizeStartDelegate  OnHeartbeat;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler          OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnAuthorizeStartDelegate  OnStartTransaction;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler          OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnAuthorizeStartDelegate  OnStopTransaction;

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
                   TCPPort        != null            ? TCPPort        : DefaultHTTPServerPort,
                   URIPrefix.     IsNotNullOrEmpty() ? URIPrefix      : DefaultURIPrefix,
                   HTTPContentType.SOAPXML_UTF8,
                   DNSClient,
                   AutoStart: false)

        {

            if (AutoStart)
                Start();

        }

        #endregion

        #region CSServer(SOAPServer, URIPrefix = "")

        /// <summary>
        /// Use the given HTTP server for the OCPP HTTP/SOAP/XML Central System Server API using IPAddress.Any.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        public CSServer(SOAPServer  SOAPServer,
                        String      URIPrefix  = "")

            : base(SOAPServer,
                   URIPrefix)

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
                                            async (SOAPRequest, HeaderXML, AuthorizeXML) => {

                #region Send OnAuthorizeRequest event

                var Runtime = Stopwatch.StartNew();

                try
                {

                    OnAuthorizeRequest?.Invoke(DateTime.Now,
                                               SOAPServer,
                                               SOAPRequest);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnAuthorizeRequest");
                }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <ns:chargeBoxIdentity>?</ns:chargeBoxIdentity>
                //       <wsa:Action soap:mustUnderstand="1">/BootNotification</wsa:Action>
                //       <wsa:ReplyTo soap:mustUnderstand="1">
                //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
                //       </wsa:ReplyTo>
                //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
                //       <wsa:To soap:mustUnderstand="1">http://127.0.0.1:2010/v1.6</wsa:To>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:authorizeRequest>
                //          <ocpp:idTag>?</ocpp:idTag>
                //       </ocpp:authorizeRequest>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Parse request parameters

                // SOAP header...
                var ChargeBoxIdentity  = HeaderXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity",
                                                                         "The given SOAP header did not provide a valid 'charge box identity' information!");

                var MessageId          = HeaderXML.ElementValueOrFail   (SOAPNS.SOAPAdressing + "MessageID");


                // <wsa5:From><wsa5:Address>http://62.133.94.210:12345</wsa5:Address></wsa5:From>


                // SOAP body...
                var IdTag              = AuthorizeXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                   "The given authorize XML did not provide a valid 'IdTag' information!");

                #endregion


                #region Call async subscribers

                //AuthStartEVSEResult result = null;

                //var OnAuthorizeStartLocal = OnAuthorizeStart;
                //if (OnAuthorizeStartLocal != null)
                //{

                //    var CTS = new CancellationTokenSource();

                //    var task = OnAuthorizeStartLocal(DateTime.Now,
                //                                     this,
                //                                     CTS.Token,
                //                                     EventTracking_Id.New,
                //                                     OperatorId,
                //                                     AuthToken,
                //                                     EVSEId,
                //                                     SessionId,
                //                                     ChargingProductId,
                //                                     PartnerSessionId,
                //                                     DefaultQueryTimeout);

                //    task.Wait();
                //    result = task.Result;

                //}

                #endregion


                var Status       = AuthorizationStatus.Accepted;
                var ExpiryDate   = DateTime.Now.Add(TimeSpan.FromDays(1));
                var ParentIdTag  = new IdToken("4567");

                #region Map result

                //var HubjectCode            = "";
                //var HubjectDescription     = "";
                //var HubjectAdditionalInfo  = "";

                //if (result != null)
                //    switch (result.Result)
                //    {

                //        case AuthStartEVSEResultType.Authorized:
                //            HubjectCode         = "000";
                //            HubjectDescription  = "Ready to charge!";
                //            break;

                //        case AuthStartEVSEResultType.NotAuthorized:
                //            HubjectCode         = "102";
                //            HubjectDescription  = "RFID Authentication failed - invalid UID";
                //            break;

                //        case AuthStartEVSEResultType.InvalidSessionId:
                //            HubjectCode         = "400";
                //            HubjectDescription  = "Session is invalid";
                //            break;

                //        case AuthStartEVSEResultType.EVSECommunicationTimeout:
                //            HubjectCode         = "501";
                //            HubjectDescription  = "Communication to EVSE failed!";
                //            break;

                //        case AuthStartEVSEResultType.StartChargingTimeout:
                //            HubjectCode         = "510";
                //            HubjectDescription  = "No EV connected to EVSE!";
                //            break;

                //        case AuthStartEVSEResultType.Reserved:
                //            HubjectCode         = "601";
                //            HubjectDescription  = "EVSE reserved!";
                //            break;

                //        //Note: Can not happen, or?
                //        //case AuthStartEVSEResultType.AlreadyInUse:
                //        //    HubjectCode         = "602";
                //        //    HubjectDescription  = "EVSE is already in use!";
                //        //    break;

                //        case AuthStartEVSEResultType.UnknownEVSE:
                //            HubjectCode         = "603";
                //            HubjectDescription  = "Unknown EVSE ID!";
                //            break;

                //        case AuthStartEVSEResultType.OutOfService:
                //            HubjectCode         = "700";
                //            HubjectDescription  = "EVSE out of service!";
                //            break;


                //        default:
                //            HubjectCode         = "320";
                //            HubjectDescription  = "Service not available!";
                //            break;

                //    }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <wsa:Action soap:mustUnderstand="true">/HeartbeatResponse</wsa:Action>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:authorizeResponse>
                //          <ocpp:idTagInfo>
                //
                //             <ocpp:status>?</ocpp:status>
                //
                //             <!--Optional:-->
                //             <ocpp:expiryDate>?</ocpp:expiryDate>
                //
                //             <!--Optional:-->
                //             <ocpp:parentIdTag>?</ocpp:parentIdTag>
                //
                //          </ocpp:idTagInfo>
                //       </ocpp:authorizeResponse>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Create HTTP/SOAP response

                var SOAPResponse = new HTTPResponseBuilder(SOAPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.SOAPXML_UTF8,
                    Content         = SOAP.Encapsulation(

                                          SOAPHeaders: new XElement[] {
                                                           new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Action",
                                                               new XAttribute(SOAPNS.SOAPEnvelope_v1_2 + "mustUnderstand", "true"),
                                                               "/AuthorizeResponse")
                                                           // RelatesTo: MessageID
                                                       },

                                          SOAPBody:    new XElement(OCPPNS.OCPPv1_6_CS + "authorizeResponse",
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                               new XElement(OCPPNS.OCPPv1_6_CS + "status",      Status.ToString()),
                                                               new XElement(OCPPNS.OCPPv1_6_CS + "expiryDate",  ExpiryDate.ToUniversalTime().ToIso8601()),
                                                               new XElement(OCPPNS.OCPPv1_6_CS + "parentIdTag", ParentIdTag.ToString())
                                                           )
                                                       )

                                         ).ToUTF8Bytes()
                };

                #endregion

                #region Send OnAuthorizeResponse event

                Runtime.Stop();

                try
                {

                    OnAuthorizeResponse?.Invoke(SOAPResponse.Timestamp,
                                                this.SOAPServer,
                                                SOAPRequest,
                                                SOAPResponse);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnAuthorizeResponse");
                }

                #endregion

                return SOAPResponse;

            });

            #endregion

            #region / - Boot Notification

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "Boot Notification",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest").FirstOrDefault(),
                                            async (SOAPRequest, HeaderXML, BootNotificationXML) => {

                #region Send OnBootNotificationRequest event

                var Runtime = Stopwatch.StartNew();

                try
                {

                    OnBootNotificationRequest?.Invoke(DateTime.Now,
                                                      SOAPServer,
                                                      SOAPRequest);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnBootNotificationRequest");
                }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <ns:chargeBoxIdentity>?</ns:chargeBoxIdentity>
                //       <wsa:Action soap:mustUnderstand="1">/BootNotification</wsa:Action>
                //       <wsa:ReplyTo soap:mustUnderstand="1">
                //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
                //       </wsa:ReplyTo>
                //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
                //       <wsa:To soap:mustUnderstand="1">http://127.0.0.1:2010/v1.6</wsa:To>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:bootNotificationRequest>
                //
                //          <ocpp:chargePointVendor>?</ocpp:chargePointVendor>
                //          <ocpp:chargePointModel>?</ocpp:chargePointModel>
                //
                //          <!--Optional:-->
                //          <ocpp:chargePointSerialNumber>?</ocpp:chargePointSerialNumber>
                //
                //          <!--Optional:-->
                //          <ocpp:chargeBoxSerialNumber>?</ocpp:chargeBoxSerialNumber>
                //
                //          <!--Optional:-->
                //          <ocpp:firmwareVersion>?</ocpp:firmwareVersion>
                //
                //          <!--Optional:-->
                //          <ocpp:iccid>?</ocpp:iccid>
                //
                //          <!--Optional:-->
                //          <ocpp:imsi>?</ocpp:imsi>
                //
                //          <!--Optional:-->
                //          <ocpp:meterType>?</ocpp:meterType>
                //
                //          <!--Optional:-->
                //          <ocpp:meterSerialNumber>?</ocpp:meterSerialNumber>
                //
                //       </ocpp:bootNotificationRequest>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Parse request parameters



                // SOAP header...
                var ChargeBoxIdentity        = HeaderXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity",
                                                                               "The given SOAP header did not provide a valid 'charge box identity' information!");

                var MessageId                = HeaderXML.ElementValueOrFail   (SOAPNS.SOAPAdressing + "MessageID");


                // <wsa5:From><wsa5:Address>http://62.133.94.210:12345</wsa5:Address></wsa5:From>





                // SOAP body...
                var ChargePointVendor        = BootNotificationXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointVendor",
                                                                                         "The given BootNotification did not provide a valid 'charge point vender' information!");

                var ChargePointModel         = BootNotificationXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointModel",
                                                                                         "The given BootNotification did not provide a valid 'charge point model' information!");

                var ChargePointSerialNumber  = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber", "");
                var ChargeBoxSerialNumber    = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargeBoxSerialNumber",   "");
                var FirmwareVersion          = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "firmwareVersion",         "");
                var ICCID                    = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "iccid",                   "");
                var IMSI                     = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "imsi",                    "");
                var MeterType                = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterType",               "");
                var MeterSerialNumer         = BootNotificationXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterSerialNumber",       "");

                #endregion


                #region Call async subscribers

                //AuthStartEVSEResult result = null;

                //var OnAuthorizeStartLocal = OnAuthorizeStart;
                //if (OnAuthorizeStartLocal != null)
                //{

                //    var CTS = new CancellationTokenSource();

                //    var task = OnAuthorizeStartLocal(DateTime.Now,
                //                                     this,
                //                                     CTS.Token,
                //                                     EventTracking_Id.New,
                //                                     OperatorId,
                //                                     AuthToken,
                //                                     EVSEId,
                //                                     SessionId,
                //                                     ChargingProductId,
                //                                     PartnerSessionId,
                //                                     DefaultQueryTimeout);

                //    task.Wait();
                //    result = task.Result;

                //}

                #endregion


                var Status    = RegistrationStatus.Accepted;
                var Interval  = 300;

                #region Map result

                //var HubjectCode            = "";
                //var HubjectDescription     = "";
                //var HubjectAdditionalInfo  = "";

                //if (result != null)
                //    switch (result.Result)
                //    {

                //        case AuthStartEVSEResultType.Authorized:
                //            HubjectCode         = "000";
                //            HubjectDescription  = "Ready to charge!";
                //            break;

                //        case AuthStartEVSEResultType.NotAuthorized:
                //            HubjectCode         = "102";
                //            HubjectDescription  = "RFID Authentication failed - invalid UID";
                //            break;

                //        case AuthStartEVSEResultType.InvalidSessionId:
                //            HubjectCode         = "400";
                //            HubjectDescription  = "Session is invalid";
                //            break;

                //        case AuthStartEVSEResultType.EVSECommunicationTimeout:
                //            HubjectCode         = "501";
                //            HubjectDescription  = "Communication to EVSE failed!";
                //            break;

                //        case AuthStartEVSEResultType.StartChargingTimeout:
                //            HubjectCode         = "510";
                //            HubjectDescription  = "No EV connected to EVSE!";
                //            break;

                //        case AuthStartEVSEResultType.Reserved:
                //            HubjectCode         = "601";
                //            HubjectDescription  = "EVSE reserved!";
                //            break;

                //        //Note: Can not happen, or?
                //        //case AuthStartEVSEResultType.AlreadyInUse:
                //        //    HubjectCode         = "602";
                //        //    HubjectDescription  = "EVSE is already in use!";
                //        //    break;

                //        case AuthStartEVSEResultType.UnknownEVSE:
                //            HubjectCode         = "603";
                //            HubjectDescription  = "Unknown EVSE ID!";
                //            break;

                //        case AuthStartEVSEResultType.OutOfService:
                //            HubjectCode         = "700";
                //            HubjectDescription  = "EVSE out of service!";
                //            break;


                //        default:
                //            HubjectCode         = "320";
                //            HubjectDescription  = "Service not available!";
                //            break;

                //    }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <wsa:Action soap:mustUnderstand="true">/HeartbeatResponse</wsa:Action>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:heartbeatResponse>
                //          <ocpp:currentTime>?</ocpp:currentTime>
                //       </ocpp:heartbeatResponse>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Create HTTP/SOAP response

                var SOAPResponse = new HTTPResponseBuilder(SOAPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.SOAPXML_UTF8,
                    Content         = SOAP.Encapsulation(

                                          SOAPHeaders: new XElement[] {
                                                           new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Action",
                                                               new XAttribute(SOAPNS.SOAPEnvelope_v1_2 + "mustUnderstand", "true"),
                                                               "/HeartbeatResponse")
                                                           // RelatesTo: MessageID
                                                       },

                                          SOAPBody:    new XElement(OCPPNS.OCPPv1_6_CS + "bootNotificationResponse",
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "status",       Status.ToString()),
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",  DateTime.Now.ToUniversalTime().ToIso8601()),
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "interval",     Interval)
                                                       )

                                         ).ToUTF8Bytes()
                };

                #endregion

                #region Send OnBootNotificationResponse event

                Runtime.Stop();

                try
                {

                    OnBootNotificationResponse?.Invoke(SOAPResponse.Timestamp,
                                                       SOAPServer,
                                                       SOAPRequest,
                                                       SOAPResponse);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnBootNotificationResponse");
                }

                #endregion

                return SOAPResponse;

            });

            #endregion

            #region / - Heartbeat

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "Heartbeat",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "heartbeatRequest").FirstOrDefault(),
                                            async (SOAPRequest, HeaderXML, HeartbeatXML) => {

                #region Send OnHeartbeatRequest event

                var Runtime = Stopwatch.StartNew();

                try
                {

                    OnHeartbeatRequest?.Invoke(DateTime.Now,
                                               SOAPServer,
                                               SOAPRequest);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnHeartbeatRequest");
                }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <ns:chargeBoxIdentity>?</ns:chargeBoxIdentity>
                //       <wsa:Action soap:mustUnderstand="1">/BootNotification</wsa:Action>
                //       <wsa:ReplyTo soap:mustUnderstand="1">
                //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
                //       </wsa:ReplyTo>
                //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
                //       <wsa:To soap:mustUnderstand="1">http://127.0.0.1:2010/v1.6</wsa:To>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:heartbeatRequest />
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion


                #region Call async subscribers

                //AuthStartEVSEResult result = null;

                //var OnAuthorizeStartLocal = OnAuthorizeStart;
                //if (OnAuthorizeStartLocal != null)
                //{

                //    var CTS = new CancellationTokenSource();

                //    var task = OnAuthorizeStartLocal(DateTime.Now,
                //                                     this,
                //                                     CTS.Token,
                //                                     EventTracking_Id.New,
                //                                     OperatorId,
                //                                     AuthToken,
                //                                     EVSEId,
                //                                     SessionId,
                //                                     ChargingProductId,
                //                                     PartnerSessionId,
                //                                     DefaultQueryTimeout);

                //    task.Wait();
                //    result = task.Result;

                //}

                #endregion


                #region Map result

                //var HubjectCode            = "";
                //var HubjectDescription     = "";
                //var HubjectAdditionalInfo  = "";

                //if (result != null)
                //    switch (result.Result)
                //    {

                //        case AuthStartEVSEResultType.Authorized:
                //            HubjectCode         = "000";
                //            HubjectDescription  = "Ready to charge!";
                //            break;

                //        case AuthStartEVSEResultType.NotAuthorized:
                //            HubjectCode         = "102";
                //            HubjectDescription  = "RFID Authentication failed - invalid UID";
                //            break;

                //        case AuthStartEVSEResultType.InvalidSessionId:
                //            HubjectCode         = "400";
                //            HubjectDescription  = "Session is invalid";
                //            break;

                //        case AuthStartEVSEResultType.EVSECommunicationTimeout:
                //            HubjectCode         = "501";
                //            HubjectDescription  = "Communication to EVSE failed!";
                //            break;

                //        case AuthStartEVSEResultType.StartChargingTimeout:
                //            HubjectCode         = "510";
                //            HubjectDescription  = "No EV connected to EVSE!";
                //            break;

                //        case AuthStartEVSEResultType.Reserved:
                //            HubjectCode         = "601";
                //            HubjectDescription  = "EVSE reserved!";
                //            break;

                //        //Note: Can not happen, or?
                //        //case AuthStartEVSEResultType.AlreadyInUse:
                //        //    HubjectCode         = "602";
                //        //    HubjectDescription  = "EVSE is already in use!";
                //        //    break;

                //        case AuthStartEVSEResultType.UnknownEVSE:
                //            HubjectCode         = "603";
                //            HubjectDescription  = "Unknown EVSE ID!";
                //            break;

                //        case AuthStartEVSEResultType.OutOfService:
                //            HubjectCode         = "700";
                //            HubjectDescription  = "EVSE out of service!";
                //            break;


                //        default:
                //            HubjectCode         = "320";
                //            HubjectDescription  = "Service not available!";
                //            break;

                //    }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <wsa:Action soap:mustUnderstand="true">/HeartbeatResponse</wsa:Action>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:heartbeatResponse>
                //          <ocpp:currentTime>?</ocpp:currentTime>
                //       </ocpp:heartbeatResponse>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Create HTTP/SOAP response

                var SOAPResponse = new HTTPResponseBuilder(SOAPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.SOAPXML_UTF8,
                    Content         = SOAP.Encapsulation(

                                          SOAPHeaders: new XElement[] {
                                                           new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Action",
                                                               new XAttribute(SOAPNS.SOAPEnvelope_v1_2 + "mustUnderstand", "true"),
                                                               "/HeartbeatResponse")
                                                           // RelatesTo: MessageID
                                                       },

                                          SOAPBody:    new XElement(OCPPNS.OCPPv1_6_CS + "heartbeatResponse",
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "currentTime", DateTime.Now.ToUniversalTime().ToIso8601())
                                                       )

                                         ).ToUTF8Bytes()
                };

                #endregion

                #region Send OnHeartbeatResponse event

                Runtime.Stop();

                try
                {

                    OnHeartbeatResponse?.Invoke(SOAPResponse.Timestamp,
                                                SOAPServer,
                                                SOAPRequest,
                                                SOAPResponse);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnHeartbeatResponse");
                }

                #endregion

                return SOAPResponse;

            });

            #endregion

            #region / - StartTransaction

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URIPrefix,
                                            "StartTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CS + "startTransactionRequest").FirstOrDefault(),
                                            async (SOAPRequest, HeaderXML, StartTransactionXML) => {

                #region Send OnStartTransactionRequest event

                var Runtime = Stopwatch.StartNew();

                try
                {

                    OnStartTransactionRequest?.Invoke(DateTime.Now,
                                               SOAPServer,
                                               SOAPRequest);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnStartTransactionRequest");
                }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <ns:chargeBoxIdentity>?</ns:chargeBoxIdentity>
                //       <wsa:Action soap:mustUnderstand="1">/BootNotification</wsa:Action>
                //       <wsa:ReplyTo soap:mustUnderstand="1">
                //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
                //       </wsa:ReplyTo>
                //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
                //       <wsa:To soap:mustUnderstand="1">http://127.0.0.1:2010/v1.6</wsa:To>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:startTransactionRequest>
                //
                //          <ocpp:connectorId>?</ocpp:connectorId>
                //          <ocpp:idTag>?</ocpp:idTag>
                //          <ocpp:timestamp>?</ocpp:timestamp>
                //          <ocpp:meterStart>?</ocpp:meterStart>
                //
                //          <!--Optional:-->
                //          <ocpp:reservationId>?</ocpp:reservationId>
                //
                //       </ocpp:startTransactionRequest>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion


                #region Parse request parameters

                // SOAP header...
                var ChargeBoxIdentity  = HeaderXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity",
                                                                         "The given SOAP header did not provide a valid 'charge box identity' information!");

                var MessageId          = HeaderXML.ElementValueOrFail   (SOAPNS.SOAPAdressing + "MessageID");


                // <wsa5:From><wsa5:Address>http://62.133.94.210:12345</wsa5:Address></wsa5:From>


                // SOAP body...
                var ConnectorId        = StartTransactionXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                   "The given start transaction request does not have valid 'connectorId' information!");

                var IdTag              = StartTransactionXML.MapValueOrFail       (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                   IdToken.Parse,
                                                                                   "The given start transaction request does not have valid 'idTag' information!");

                var Timestamp          = StartTransactionXML.MapValueOrFail       (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                                   DateTime.Parse,
                                                                                   "The given start transaction request does not have valid 'timestamp' information!");

                var MeterStart         = StartTransactionXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "meterStart",
                                                                                   "The given start transaction request does not have valid 'meterStart' information!");

                var ReservationId      = StartTransactionXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "reservationId","");

                #endregion


                #region Call async subscribers

                //AuthStartEVSEResult result = null;

                //var OnAuthorizeStartLocal = OnAuthorizeStart;
                //if (OnAuthorizeStartLocal != null)
                //{

                //    var CTS = new CancellationTokenSource();

                //    var task = OnAuthorizeStartLocal(DateTime.Now,
                //                                     this,
                //                                     CTS.Token,
                //                                     EventTracking_Id.New,
                //                                     OperatorId,
                //                                     AuthToken,
                //                                     EVSEId,
                //                                     SessionId,
                //                                     ChargingProductId,
                //                                     PartnerSessionId,
                //                                     DefaultQueryTimeout);

                //    task.Wait();
                //    result = task.Result;

                //}

                #endregion


                #region Map result

                //var HubjectCode            = "";
                //var HubjectDescription     = "";
                //var HubjectAdditionalInfo  = "";

                //if (result != null)
                //    switch (result.Result)
                //    {

                //        case AuthStartEVSEResultType.Authorized:
                //            HubjectCode         = "000";
                //            HubjectDescription  = "Ready to charge!";
                //            break;

                //        case AuthStartEVSEResultType.NotAuthorized:
                //            HubjectCode         = "102";
                //            HubjectDescription  = "RFID Authentication failed - invalid UID";
                //            break;

                //        case AuthStartEVSEResultType.InvalidSessionId:
                //            HubjectCode         = "400";
                //            HubjectDescription  = "Session is invalid";
                //            break;

                //        case AuthStartEVSEResultType.EVSECommunicationTimeout:
                //            HubjectCode         = "501";
                //            HubjectDescription  = "Communication to EVSE failed!";
                //            break;

                //        case AuthStartEVSEResultType.StartChargingTimeout:
                //            HubjectCode         = "510";
                //            HubjectDescription  = "No EV connected to EVSE!";
                //            break;

                //        case AuthStartEVSEResultType.Reserved:
                //            HubjectCode         = "601";
                //            HubjectDescription  = "EVSE reserved!";
                //            break;

                //        //Note: Can not happen, or?
                //        //case AuthStartEVSEResultType.AlreadyInUse:
                //        //    HubjectCode         = "602";
                //        //    HubjectDescription  = "EVSE is already in use!";
                //        //    break;

                //        case AuthStartEVSEResultType.UnknownEVSE:
                //            HubjectCode         = "603";
                //            HubjectDescription  = "Unknown EVSE ID!";
                //            break;

                //        case AuthStartEVSEResultType.OutOfService:
                //            HubjectCode         = "700";
                //            HubjectDescription  = "EVSE out of service!";
                //            break;


                //        default:
                //            HubjectCode         = "320";
                //            HubjectDescription  = "Service not available!";
                //            break;

                //    }

                #endregion

                #region Documentation

                // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
                //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
                //                xmlns:ocpp = "urn://Ocpp/Cs/2015/10/">
                //
                //    <soap:Header>
                //       <wsa:Action soap:mustUnderstand="true">/HeartbeatResponse</wsa:Action>
                //    </soap:Header>
                //
                //    <soap:Body>
                //       <ocpp:heartbeatResponse>
                //          <ocpp:currentTime>?</ocpp:currentTime>
                //       </ocpp:heartbeatResponse>
                //    </soap:Body>
                //
                // </soap:Envelope>

                #endregion

                #region Create HTTP/SOAP response

                var SOAPResponse = new HTTPResponseBuilder(SOAPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.OK,
                    Server          = SOAPServer.DefaultServerName,
                    Date            = DateTime.Now,
                    ContentType     = HTTPContentType.SOAPXML_UTF8,
                    Content         = SOAP.Encapsulation(

                                          SOAPHeaders: new XElement[] {
                                                           new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Action",
                                                               new XAttribute(SOAPNS.SOAPEnvelope_v1_2 + "mustUnderstand", "true"),
                                                               "/HeartbeatResponse")
                                                           // RelatesTo: MessageID
                                                       },

                                          SOAPBody:    new XElement(OCPPNS.OCPPv1_6_CS + "heartbeatResponse",
                                                           new XElement(OCPPNS.OCPPv1_6_CS + "currentTime", DateTime.Now.ToUniversalTime().ToIso8601())
                                                       )

                                         ).ToUTF8Bytes()
                };

                #endregion

                #region Send OnHeartbeatResponse event

                Runtime.Stop();

                try
                {

                    OnHeartbeatResponse?.Invoke(SOAPResponse.Timestamp,
                                                SOAPServer,
                                                SOAPRequest,
                                                SOAPResponse);

                }
                catch (Exception e)
                {
                    e.Log("OCPPv1.6.CentralSystemServer.OnHeartbeatResponse");
                }

                #endregion

                return SOAPResponse;

            });

            #endregion


        }

        #endregion


        //#region (internal) SendRemoteStart(...)

        //internal async Task<RemoteStartEVSEResult> SendRemoteStart(DateTime            Timestamp,
        //                                                           CSServer           Sender,
        //                                                           CancellationToken   CancellationToken,
        //                                                           EventTracking_Id    EventTrackingId,
        //                                                           EVSE_Id             EVSEId,
        //                                                           ChargingProduct_Id  ChargingProductId,
        //                                                           ChargingSession_Id  SessionId,
        //                                                           ChargingSession_Id  PartnerSessionId,
        //                                                           EVSP_Id             ProviderId,
        //                                                           eMA_Id              eMAId,
        //                                                           TimeSpan?           QueryTimeout  = null)
        //{

        //    var OnRemoteStartLocal = OnRemoteStart;
        //    if (OnRemoteStartLocal == null)
        //        return RemoteStartEVSEResult.Error();

        //    var results = await Task.WhenAll(OnRemoteStartLocal.
        //                                         GetInvocationList().
        //                                         Select(subscriber => (subscriber as OnRemoteStartDelegate)
        //                                             (Timestamp,
        //                                              this,
        //                                              CancellationToken,
        //                                              EventTrackingId,
        //                                              EVSEId,
        //                                              ChargingProductId,
        //                                              SessionId,
        //                                              PartnerSessionId,
        //                                              ProviderId,
        //                                              eMAId,
        //                                              QueryTimeout)));

        //    return results.
        //               Where(result => result.Result != RemoteStartEVSEResultType.Unspecified).
        //               First();

        //}

        //#endregion

        //#region (internal) SendRemoteStop(...)

        //internal async Task<RemoteStopEVSEResult> SendRemoteStop(DateTime            Timestamp,
        //                                                         CSServer           Sender,
        //                                                         CancellationToken   CancellationToken,
        //                                                         EventTracking_Id    EventTrackingId,
        //                                                         EVSE_Id             EVSEId,
        //                                                         ChargingSession_Id  SessionId,
        //                                                         ChargingSession_Id  PartnerSessionId,
        //                                                         EVSP_Id             ProviderId,
        //                                                         TimeSpan?           QueryTimeout  = null)
        //{

        //    var OnRemoteStopLocal = OnRemoteStop;
        //    if (OnRemoteStopLocal == null)
        //        return RemoteStopEVSEResult.Error(SessionId);

        //    var results = await Task.WhenAll(OnRemoteStopLocal.
        //                                         GetInvocationList().
        //                                         Select(subscriber => (subscriber as OnRemoteStopDelegate)
        //                                             (Timestamp,
        //                                              this,
        //                                              CancellationToken,
        //                                              EventTrackingId,
        //                                              EVSEId,
        //                                              SessionId,
        //                                              PartnerSessionId,
        //                                              ProviderId,
        //                                              QueryTimeout)));

        //    return results.
        //               Where(result => result.Result != RemoteStopEVSEResultType.Unspecified).
        //               First();

        //}

        //#endregion


    }

}
