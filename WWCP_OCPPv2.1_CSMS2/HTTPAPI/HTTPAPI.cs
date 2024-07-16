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

using System.Reflection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// OCPP Networking Node HTTP API extensions.
    /// </summary>
    public static class HTTPAPIExtensions
    {

        #region ParseChargingStationId(this HTTPRequest, OCPPHTTPAPI, out ChargingStationId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargingStationId">The parsed unique charging station identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargingStationId(this HTTPRequest                                HTTPRequest,
                                                     HTTPAPI                                         OCPPHTTPAPI,
                                                     [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                     [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargingStationId  = null;
            HTTPResponse       = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargingStationId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargingStationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargingStation  (this HTTPRequest, OCPPHTTPAPI, out ChargingStationId, out ChargingStation, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargingStationId">The parsed unique charging station identification.</param>
        /// <param name="ChargingStation">The resolved charging station.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargingStation(this HTTPRequest                                HTTPRequest,
                                                   HTTPAPI                                         OCPPHTTPAPI,
                                                   [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                   [NotNullWhen(true)]  out ChargingStation?       ChargingStation,
                                                   [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargingStationId  = null;
            ChargingStation    = null;
            HTTPResponse       = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargingStationId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargingStationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!OCPPHTTPAPI.TryGetChargingStation(ChargingStationId.Value, out ChargingStation)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP Networking Node HTTP API.
    /// </summary>
    public class HTTPAPI : AHTTPAPIExtension<HTTPExtAPI>,
                           IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath                   DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String                     DefaultHTTPServerName     = $"Open Charging Cloud OCPP {Version.String} Networking Node HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm          = "Open Charging Cloud OCPP Networking Node HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.HTTPAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType   = new ("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType   = new ("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId                = HTTPEventSource_Id.Parse("OCPPEvents");


        protected readonly  List<ACSMS>                                      networkingNodes   = [];

        protected readonly  ConcurrentDictionary<ChargingStation_Id, ChargingStation>  chargingStations  = [];

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of registered networking nodes.
        /// </summary>
        public              IEnumerable<ACSMS>                               NetworkingNodes
            => networkingNodes;

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public              String?                                                    HTTPRealm         { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public              IEnumerable<KeyValuePair<String, String>>                  HTTPLogins        { get; }

        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public              HTTPEventSource<JObject>                                   EventLog          { get; }

        #endregion

        #region Constructor(s)

        #region HTTPAPI(...)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="NetworkingNode">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName   = null,
                       HTTPPath?                                   URLPathPrefix    = null,
                       HTTPPath?                                   BasePath         = null,
                       String                                      HTTPRealm        = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                       String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins ?? [];

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = HTTPBaseAPI.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryIntervall:           TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            RegisterURITemplates();

        }

        #endregion

        #region HTTPAPI(CSMS, ...)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="NetworkingNode">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ACSMS                             NetworkingNode,
                       HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName   = null,
                       HTTPPath?                                   URLPathPrefix    = null,
                       HTTPPath?                                   BasePath         = null,
                       String                                      HTTPRealm        = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                       String?                                     HTMLTemplate     = null)

            : this(HTTPAPI,
                   HTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTTPRealm,
                   HTTPLogins,
                   HTMLTemplate)

        {

            AttachCSMS(NetworkingNode);

        }

        #endregion

        #endregion


        #region AttachCSMS(NetworkingNode)

        public void AttachCSMS(ACSMS NetworkingNode)
        {

            networkingNodes.Add(NetworkingNode);


            // Wire HTTP Server Sent Events

            #region Generic JSON Messages

            #region OnJSONMessageRequestReceived

            NetworkingNode.OnJSONMessageRequestReceived += (timestamp,
                                                            webSocketServer,
                                                            webSocketConnection,
                                                            networkingNodeId,
                                                            networkPath,
                                                            eventTrackingId,
                                                            requestTimestamp,
                                                            requestMessage,
                                                            cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseSent

            NetworkingNode.OnJSONMessageResponseSent += (timestamp,
                                                         webSocketServer,
                                                         webSocketConnection,
                                                         networkingNodeId,
                                                         networkPath,
                                                         eventTrackingId,
                                                         requestTimestamp,
                                                         jsonRequestMessage,
                                                         binaryRequestMessage,
                                                         responseTimestamp,
                                                         responseMessage,
                                                         cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseSent

            NetworkingNode.OnJSONErrorResponseSent += (timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       jsonRequestMessage,
                                                       binaryRequestMessage,
                                                       responseTimestamp,
                                                       responseMessage,
                                                       cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion


            #region OnJSONMessageRequestSent

            NetworkingNode.OnJSONMessageRequestSent += (timestamp,
                                                        webSocketServer,
                                                        webSocketConnection,
                                                        networkingNodeId,
                                                        networkPath,
                                                        eventTrackingId,
                                                        requestTimestamp,
                                                        requestMessage,
                                                        cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseReceived

            NetworkingNode.OnJSONMessageResponseReceived += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             networkingNodeId,
                                                             networkPath,
                                                             eventTrackingId,
                                                             requestTimestamp,
                                                             jsonRequestMessage,
                                                             binaryRequestMessage,
                                                             responseTimestamp,
                                                             responseMessage,
                                                             cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseReceived

            NetworkingNode.OnJSONErrorResponseReceived += (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           jsonRequestMessage,
                                                           binaryRequestMessage,
                                                           responseTimestamp,
                                                           responseMessage,
                                                           cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            NetworkingNode.OnBinaryMessageRequestReceived += (timestamp,
                                                              webSocketServer,
                                                              webSocketConnection,
                                                              networkingNodeId,
                                                              networkPath,
                                                              eventTrackingId,
                                                              requestTimestamp,
                                                              requestMessage,
                                                              cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseSent

            NetworkingNode.OnBinaryMessageResponseSent += (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           networkingNodeId,
                                                           networkPath,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           jsonRequestMessage,
                                                           binaryRequestMessage,
                                                           responseTimestamp,
                                                           responseMessage,
                                                           cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryErrorResponseSent

            //NetworkingNode.OnBinaryErrorResponseSent += (timestamp,
            //                                                  webSocketServer,
            //                                                  webSocketConnection,
            //                                                  eventTrackingId,
            //                                                  requestTimestamp,
            //                                                  jsonRequestMessage,
            //                                                  binaryRequestMessage,
            //                                                  responseTimestamp,
            //                                                  responseMessage) =>

            //    EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryErrorResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

            #endregion


            #region OnBinaryMessageRequestSent

            NetworkingNode.OnBinaryMessageRequestSent += (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               networkingNodeId,
                                                               networkPath,
                                                               eventTrackingId,
                                                               requestTimestamp,
                                                               requestMessage,
                                                               cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseReceived

            NetworkingNode.OnBinaryMessageResponseReceived += (timestamp,
                                                                    webSocketServer,
                                                                    webSocketConnection,
                                                                    networkingNodeId,
                                                                    networkPath,
                                                                    eventTrackingId,
                                                                    requestTimestamp,
                                                                    jsonRequestMessage,
                                                                    binaryRequestMessage,
                                                                    responseTimestamp,
                                                                    responseMessage,
                                                                    cancellationToken) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryErrorResponseReceived

            //NetworkingNode.OnBinaryErrorResponseReceived += (timestamp,
            //                                                      webSocketServer,
            //                                                      webSocketConnection,
            //                                                      eventTrackingId,
            //                                                      requestTimestamp,
            //                                                      jsonRequestMessage,
            //                                                      binaryRequestMessage,
            //                                                      responseTimestamp,
            //                                                      responseMessage) =>

            //    EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryErrorResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

            #endregion

            #endregion


            #region ChargingStation  -> CSMS            Message exchanged

            #region Certificates

            #region OnGet15118EVCertificate

            NetworkingNode.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGet15118EVCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGet15118EVCertificateRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGet15118EVCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGet15118EVCertificateResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGet15118EVCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGet15118EVCertificateResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGet15118EVCertificateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCertificateStatus

            NetworkingNode.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCertificateStatusRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCertificateStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCertificateStatusRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetCertificateStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCertificateStatusResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCertificateStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCertificateStatusResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCRL

            NetworkingNode.OCPP.IN.OnGetCRLRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCRLRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCRLRequestSent += (timestamp,
                                                            sender,
                                                            //connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCRLRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetCRLResponseReceived += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCRLResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCRLResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCRLResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSignCertificate

            NetworkingNode.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSignCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSignCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSignCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSignCertificateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            NetworkingNode.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnAuthorizeRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnAuthorizeRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnAuthorizeResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnAuthorizeResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearedChargingLimit

            NetworkingNode.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearedChargingLimitRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnClearedChargingLimitRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearedChargingLimitRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnClearedChargingLimitResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearedChargingLimitResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnClearedChargingLimitResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearedChargingLimitResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnMeterValues

            NetworkingNode.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnMeterValuesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnMeterValuesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnMeterValuesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnMeterValuesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyChargingLimit

            NetworkingNode.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyChargingLimitRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyChargingLimitRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyChargingLimitRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyChargingLimitResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyChargingLimitResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyChargingLimitResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyChargingLimitResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingNeeds

            NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingSchedule

            NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  //connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyPriorityCharging

            NetworkingNode.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyPriorityChargingRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyPriorityChargingResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyPriorityChargingResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifySettlement

            NetworkingNode.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifySettlementRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifySettlementRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifySettlementRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifySettlementResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifySettlementResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifySettlementResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifySettlementResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPullDynamicScheduleUpdate

            NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReportChargingProfiles

            NetworkingNode.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReportChargingProfilesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnReportChargingProfilesRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReportChargingProfilesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnReportChargingProfilesResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReportChargingProfilesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnReportChargingProfilesResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReportChargingProfilesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReservationStatusUpdate

            NetworkingNode.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReservationStatusUpdateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnReservationStatusUpdateRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReservationStatusUpdateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnReservationStatusUpdateResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReservationStatusUpdateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnReservationStatusUpdateResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReservationStatusUpdateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnStatusNotification

            NetworkingNode.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnStatusNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTransactionEvent

            NetworkingNode.OCPP.IN.OnTransactionEventRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnTransactionEventRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnTransactionEventRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnTransactionEventRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnTransactionEventResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnTransactionEventResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnTransactionEventResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnTransactionEventResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation

            NetworkingNode.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyCustomerInformationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyCustomerInformationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyCustomerInformationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyDisplayMessages

            NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnLogStatusNotification

            NetworkingNode.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnLogStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnLogStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnLogStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnLogStatusNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEvent

            NetworkingNode.OCPP.IN.OnNotifyEventRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEventRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEventRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEventRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyEventResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyEventResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyEventResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyEventResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyMonitoringReport

            NetworkingNode.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyMonitoringReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyMonitoringReportResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyMonitoringReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyReport

            NetworkingNode.OCPP.IN.OnNotifyReportRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyReportRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyReportResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyReportResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyReportResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSecurityEventNotification

            NetworkingNode.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSecurityEventNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSecurityEventNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSecurityEventNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSecurityEventNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            NetworkingNode.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnBootNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnBootNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnBootNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnBootNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnFirmwareStatusNotification

            NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnHeartbeat

            NetworkingNode.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnHeartbeatRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnHeartbeatRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnHeartbeatResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnHeartbeatResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPublishFirmwareStatusNotification

            NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                          sender,
                                                                                          connection,
                                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                       sender,
                                                                                       //connection,
                                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                           sender,
                                                                                           //connection,
                                                                                           request,
                                                                                           response,
                                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                        sender,
                                                                                        connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region CSMS             -> ChargingStation Message exchanged

            #region Certificates

            #region OnCertificateSigned

            NetworkingNode.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCertificateSignedRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCertificateSignedRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCertificateSignedResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCertificateSignedResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnDeleteCertificate

            NetworkingNode.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnDeleteCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnDeleteCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnDeleteCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnDeleteCertificateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetInstalledCertificateIds

            NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnInstallCertificate

            NetworkingNode.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnInstallCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnInstallCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnInstallCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnInstallCertificateResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyCRL

            NetworkingNode.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyCRLRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyCRLRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyCRLRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyCRLResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyCRLResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyCRLResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyCRLResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnCancelReservation

            NetworkingNode.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCancelReservationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCancelReservationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCancelReservationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCancelReservationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearChargingProfile

            NetworkingNode.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearChargingProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearChargingProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearChargingProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearChargingProfileResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetChargingProfiles

            NetworkingNode.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetChargingProfilesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetChargingProfilesRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetChargingProfilesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetChargingProfilesResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetChargingProfilesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetChargingProfilesResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetChargingProfilesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCompositeSchedule

            NetworkingNode.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCompositeScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCompositeScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetCompositeScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetCompositeScheduleResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetTransactionStatus

            NetworkingNode.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetTransactionStatusRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetTransactionStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetTransactionStatusRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetTransactionStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetTransactionStatusResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetTransactionStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetTransactionStatusResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStartTransaction

            NetworkingNode.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnRequestStartTransactionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnRequestStartTransactionRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnRequestStartTransactionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnRequestStartTransactionResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnRequestStartTransactionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnRequestStartTransactionResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnRequestStartTransactionResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStopTransaction

            NetworkingNode.OCPP.IN.OnRequestStopTransactionRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnRequestStopTransactionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnRequestStopTransactionRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnRequestStopTransactionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnRequestStopTransactionResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnRequestStopTransactionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnRequestStopTransactionResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnRequestStopTransactionResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReserveNow

            NetworkingNode.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReserveNowRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReserveNowRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnReserveNowResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnReserveNowResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetChargingProfile

            NetworkingNode.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetChargingProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetChargingProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetChargingProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetChargingProfileResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnlockConnector

            NetworkingNode.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUnlockConnectorRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUnlockConnectorRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUnlockConnectorResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUnlockConnectorResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateDynamicSchedule

            NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUsePriorityCharging

            NetworkingNode.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUsePriorityChargingRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnUsePriorityChargingRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUsePriorityChargingRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnUsePriorityChargingResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUsePriorityChargingResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnUsePriorityChargingResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUsePriorityChargingResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage

            NetworkingNode.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearDisplayMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnClearDisplayMessageRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearDisplayMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnClearDisplayMessageResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearDisplayMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnClearDisplayMessageResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearDisplayMessageResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCostUpdated

            NetworkingNode.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCostUpdatedRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnCostUpdatedRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCostUpdatedRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnCostUpdatedResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCostUpdatedResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnCostUpdatedResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCostUpdatedResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCustomerInformation

            NetworkingNode.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCustomerInformationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnCustomerInformationRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCustomerInformationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnCustomerInformationResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnCustomerInformationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnCustomerInformationResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnCustomerInformationResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetDisplayMessages

            NetworkingNode.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetDisplayMessagesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetDisplayMessagesRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetDisplayMessagesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetDisplayMessagesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetDisplayMessagesResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetDisplayMessagesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetDisplayMessage

            NetworkingNode.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetDisplayMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetDisplayMessageRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetDisplayMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetDisplayMessageResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetDisplayMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetDisplayMessageResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetDisplayMessageResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnChangeAvailability

            NetworkingNode.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnChangeAvailabilityRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnChangeAvailabilityRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnChangeAvailabilityResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnChangeAvailabilityResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearVariableMonitoring

            NetworkingNode.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearVariableMonitoringRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnClearVariableMonitoringRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearVariableMonitoringRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearVariableMonitoringResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnClearVariableMonitoringResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearVariableMonitoringResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetBaseReport

            NetworkingNode.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetBaseReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetBaseReportRequestSent += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetBaseReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetBaseReportResponseReceived += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetBaseReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetBaseReportResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLog

            NetworkingNode.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetLogRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                            sender,
                                                            //connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetLogRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetLogResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetLogResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetMonitoringReport

            NetworkingNode.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetMonitoringReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetMonitoringReportRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetMonitoringReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetMonitoringReportResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetMonitoringReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetMonitoringReportResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetMonitoringReportResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetReport

            NetworkingNode.OCPP.IN.OnGetReportRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetReportRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetReportResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetReportResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetVariables

            NetworkingNode.OCPP.IN.OnGetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetVariablesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetVariablesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetVariablesResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetVariablesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetVariablesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringBase

            NetworkingNode.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetMonitoringBaseRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetMonitoringBaseRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetMonitoringBaseRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetMonitoringBaseResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetMonitoringBaseResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetMonitoringBaseResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringLevel

            NetworkingNode.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetMonitoringLevelRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetMonitoringLevelRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetMonitoringLevelRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetMonitoringLevelResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetMonitoringLevelResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetMonitoringLevelResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetNetworkProfile

            NetworkingNode.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetNetworkProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetNetworkProfileRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetNetworkProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetNetworkProfileResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetNetworkProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetNetworkProfileResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetNetworkProfileResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariableMonitoring

            NetworkingNode.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetVariableMonitoringRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetVariableMonitoringRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetVariableMonitoringRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetVariableMonitoringResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetVariableMonitoringResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetVariableMonitoringResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariables

            NetworkingNode.OCPP.IN.OnSetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetVariablesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetVariablesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSetVariablesResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSetVariablesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSetVariablesResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTriggerMessage

            NetworkingNode.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnTriggerMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnTriggerMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnTriggerMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnTriggerMessageResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware

            NetworkingNode.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPublishFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnPublishFirmwareRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPublishFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnPublishFirmwareResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnPublishFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnPublishFirmwareResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnPublishFirmwareResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReset

            NetworkingNode.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnResetRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                           sender,
                                                           //connection,
                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnResetRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request,
                                                               response,
                                                               runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnResetResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnResetResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnpublishFirmware

            NetworkingNode.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUnpublishFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnUnpublishFirmwareRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUnpublishFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUnpublishFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnUnpublishFirmwareResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUnpublishFirmwareResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateFirmware

            NetworkingNode.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUpdateFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUpdateFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnUpdateFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnUpdateFirmwareResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal

            NetworkingNode.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnAFRRSignalRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnAFRRSignalRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnAFRRSignalRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnAFRRSignalResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnAFRRSignalResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnAFRRSignalResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnAFRRSignalResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region LocalList

            #region OnClearCache

            NetworkingNode.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearCacheRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearCacheRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnClearCacheResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnClearCacheResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLocalListVersion

            NetworkingNode.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetLocalListVersionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetLocalListVersionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnGetLocalListVersionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnGetLocalListVersionResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSendLocalList

            NetworkingNode.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSendLocalListRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSendLocalListRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnSendLocalListResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnSendLocalListResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region CSMS            <-> ChargingStation Message exchanged

            #region OnDataTransfer

            NetworkingNode.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnDataTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnDataTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            NetworkingNode.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnDataTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnDataTransferResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnBinaryDataTransfer

            NetworkingNode.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnBinaryDataTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            NetworkingNode.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnBinaryDataTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            NetworkingNode.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.IN.OnBinaryDataTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64()),
                                         new JProperty("response",    response.  ToBinary().ToBase64()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            NetworkingNode.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(NetworkingNode.OCPP.OUT.OnBinaryDataTransferResponseSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64()),
                                         new JProperty("response",    response.  ToBinary().ToBase64()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages


        }

        #endregion


        #region TryGetChargingStation(ChargingStationId, out ChargingStation)

        public Boolean TryGetChargingStation(ChargingStation_Id ChargingStationId, out ChargingStation? ChargingStation)
        {

            if (chargingStations.TryGetValue(ChargingStationId, out ChargingStation))
                return true;

            ChargingStation = null;
            return false;

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => base.GetResourceStream(ResourceName,
                                 new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                 new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => base.GetResourceMemoryStream(ResourceName,
                                       new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                       new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => base.GetResourceString(ResourceName,
                                 new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                 new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => base.GetResourceBytes(ResourceName,
                                new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => base.MixWithHTMLTemplate(ResourceName,
                                   new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                   new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => base.MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                   new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => base.MixWithHTMLTemplate(Template,
                                   ResourceName,
                                   new[] {
                                       new Tuple<string, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                       new Tuple<string, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<string, Assembly>(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI.   HTTPRoot, typeof(org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI).   Assembly)
                                   },
                                   Content);

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            //HTTPBaseAPI.RegisterResourcesFolder(this,
            //                                HTTPHostname.Any,
            //                                URLPathPrefix,
            //                                "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot",
            //                                DefaultFilename: "index.html");

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             URLPathPrefix,
            //                             HTTPDelegate: Request => {

            //                                 return Task.FromResult(
            //                                     new HTTPResponse.Builder(Request) {
            //                                         HTTPStatusCode             = HTTPStatusCode.OK,
            //                                         Server                     = DefaultHTTPServerName,
            //                                         Date                       = Timestamp.Now,
            //                                         AccessControlAllowOrigin   = "*",
            //                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
            //                                         AccessControlAllowHeaders  = [ "Authorization" ],
            //                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
            //                                         Content                    = ("<html><body>" +
            //                                                                          "This is an Open Charge Point Protocol v1.6 HTTP service!<br /><br />" +
            //                                                                          "<ul>" +
            //                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/chargeBoxes\">Charge Boxes</a></li>" +
            //                                                                          "</ul>" +
            //                                                                       "<body></html>").ToUTF8Bytes(),
            //                                         Connection                 = "close"
            //                                     }.AsImmutable);

            //                             });

            #endregion


            #region ~/events

            #region HTML

            // --------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
            // --------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                      HTTPMethod.GET,
                                      URLPathPrefix + "events",
                                      HTTPContentType.Text.HTML_UTF8,
                                      HTTPDelegate: Request => {

                                          #region Get HTTP user and its organizations

                                          //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                          //if (!TryGetHTTPUser(Request,
                                          //                    out User                   HTTPUser,
                                          //                    out HashSet<Organization>  HTTPOrganizations,
                                          //                    out HTTPResponse.Builder   Response,
                                          //                    Recursive:                 true))
                                          //{
                                          //    return Task.FromResult(Response.AsImmutable);
                                          //}

                                          #endregion

                                          return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServiceName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "GET" ],
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("events.events.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                      });

            #endregion

            #endregion


        }

        #endregion


    }

}
