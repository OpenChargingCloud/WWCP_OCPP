﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.LocalController
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


        protected readonly  List<ALocalControllerNode>                                      localControllers   = [];

        protected readonly  ConcurrentDictionary<ChargingStation_Id, ChargingStation>  chargingStations  = [];

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of registered networking nodes.
        /// </summary>
        public              IEnumerable<ALocalControllerNode>                               NetworkingNodes
            => localControllers;

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
        /// Attach the given local controller WebAPI to the given HTTP API.
        /// </summary>
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
        /// Attach the given local controller WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="LocalController">A local controller.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ALocalControllerNode                            LocalController,
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

            AttachLocalController(LocalController);

        }

        #endregion

        #endregion


        #region AttachLocalController(LocalController)

        public void AttachLocalController(ALocalControllerNode LocalController)
        {

            localControllers.Add(LocalController);


            // Wire HTTP Server Sent Events

            #region Generic JSON Messages

            #region OnJSONMessageRequestReceived

            LocalController.OnJSONMessageRequestReceived += (timestamp,
                                                            webSocketServer,
                                                            webSocketConnection,
                                                            networkingNodeId,
                                                            networkPath,
                                                            eventTrackingId,
                                                            requestTimestamp,
                                                            requestMessage,
                                                            cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseSent

            LocalController.OnJSONMessageResponseSent += (timestamp,
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

                EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseSent

            //LocalController.OnJSONErrorResponseSent += (timestamp,
            //                                           webSocketServer,
            //                                           webSocketConnection,
            //                                           eventTrackingId,
            //                                           requestTimestamp,
            //                                           jsonRequestMessage,
            //                                           binaryRequestMessage,
            //                                           responseTimestamp,
            //                                           responseMessage,
            //                                           cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONErrorResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion


            #region OnJSONMessageRequestSent

            LocalController.OnJSONMessageRequestSent += (timestamp,
                                                        webSocketServer,
                                                        webSocketConnection,
                                                        networkingNodeId,
                                                        networkPath,
                                                        eventTrackingId,
                                                        requestTimestamp,
                                                        requestMessage,
                                                        cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseReceived

            LocalController.OnJSONMessageResponseReceived += (timestamp,
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

                EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseReceived

            //LocalController.OnJSONErrorResponseReceived += (timestamp,
            //                                               webSocketServer,
            //                                               webSocketConnection,
            //                                               eventTrackingId,
            //                                               requestTimestamp,
            //                                               jsonRequestMessage,
            //                                               binaryRequestMessage,
            //                                               responseTimestamp,
            //                                               responseMessage,
            //                                               cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONErrorResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            LocalController.OnBinaryMessageRequestReceived += (timestamp,
                                                              webSocketServer,
                                                              webSocketConnection,
                                                              networkingNodeId,
                                                              networkPath,
                                                              eventTrackingId,
                                                              requestTimestamp,
                                                              requestMessage,
                                                              cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseSent

            LocalController.OnBinaryMessageResponseSent += (timestamp,
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

                EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageResponseSent),
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

            LocalController.OnBinaryMessageRequestSent += (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               networkingNodeId,
                                                               networkPath,
                                                               eventTrackingId,
                                                               requestTimestamp,
                                                               requestMessage,
                                                               cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseReceived

            LocalController.OnBinaryMessageResponseReceived += (timestamp,
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

                EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageResponseReceived),
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

            LocalController.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGet15118EVCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGet15118EVCertificateRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGet15118EVCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGet15118EVCertificateResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGet15118EVCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGet15118EVCertificateResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGet15118EVCertificateResponseSent),
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

            LocalController.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCertificateStatusRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetCertificateStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCertificateStatusRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetCertificateStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCertificateStatusResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetCertificateStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCertificateStatusResponseSent),
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

            LocalController.OCPP.IN.OnGetCRLRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCRLRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetCRLRequestSent += (timestamp,
                                                            sender,
                                                            //connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCRLRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetCRLResponseReceived += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCRLResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetCRLResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCRLResponseSent),
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

            LocalController.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSignCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSignCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSignCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSignCertificateResponseSent),
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

            LocalController.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAuthorizeRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request,
                                                               sendMessageResult) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAuthorizeRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAuthorizeResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAuthorizeResponseSent),
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

            LocalController.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearedChargingLimitRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearedChargingLimitRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearedChargingLimitRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearedChargingLimitResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearedChargingLimitResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearedChargingLimitResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearedChargingLimitResponseSent),
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

            LocalController.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnMeterValuesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnMeterValuesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnMeterValuesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnMeterValuesResponseSent),
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

            LocalController.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyChargingLimitRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyChargingLimitRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyChargingLimitRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyChargingLimitResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyChargingLimitResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyChargingLimitResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyChargingLimitResponseSent),
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

            LocalController.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent),
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

            LocalController.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  //connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent),
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

            LocalController.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyPriorityChargingRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyPriorityChargingRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyPriorityChargingRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyPriorityChargingResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyPriorityChargingResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyPriorityChargingResponseSent),
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

            LocalController.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifySettlementRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifySettlementRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifySettlementRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifySettlementResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifySettlementResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifySettlementResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifySettlementResponseSent),
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

            LocalController.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent),
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

            LocalController.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReportChargingProfilesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnReportChargingProfilesRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReportChargingProfilesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnReportChargingProfilesResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReportChargingProfilesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnReportChargingProfilesResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReportChargingProfilesResponseSent),
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

            LocalController.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReservationStatusUpdateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnReservationStatusUpdateRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReservationStatusUpdateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnReservationStatusUpdateResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReservationStatusUpdateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnReservationStatusUpdateResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReservationStatusUpdateResponseSent),
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

            LocalController.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnStatusNotificationResponseSent),
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

            LocalController.OCPP.IN.OnTransactionEventRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTransactionEventRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnTransactionEventRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTransactionEventRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnTransactionEventResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTransactionEventResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnTransactionEventResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTransactionEventResponseSent),
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

            LocalController.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyCustomerInformationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyCustomerInformationRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyCustomerInformationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyCustomerInformationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyCustomerInformationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyCustomerInformationResponseSent),
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

            LocalController.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyDisplayMessagesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyDisplayMessagesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyDisplayMessagesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyDisplayMessagesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyDisplayMessagesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyDisplayMessagesResponseSent),
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

            LocalController.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnLogStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnLogStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnLogStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnLogStatusNotificationResponseSent),
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

            LocalController.OCPP.IN.OnNotifyEventRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEventRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyEventRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEventRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyEventResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyEventResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyEventResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyEventResponseSent),
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

            LocalController.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyMonitoringReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyMonitoringReportRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyMonitoringReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyMonitoringReportResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyMonitoringReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyMonitoringReportResponseSent),
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

            LocalController.OCPP.IN.OnNotifyReportRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyReportRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyReportResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyReportResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyReportResponseSent),
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

            LocalController.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSecurityEventNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSecurityEventNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSecurityEventNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSecurityEventNotificationResponseSent),
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

            LocalController.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBootNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      sendMessageResult) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBootNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBootNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBootNotificationResponseSent),
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

            LocalController.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnFirmwareStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnFirmwareStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnFirmwareStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnFirmwareStatusNotificationResponseSent),
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

            LocalController.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnHeartbeatRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnHeartbeatRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnHeartbeatResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnHeartbeatResponseSent),
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

            LocalController.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                          sender,
                                                                                          connection,
                                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                       sender,
                                                                                       //connection,
                                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                           sender,
                                                                                           //connection,
                                                                                           request,
                                                                                           response,
                                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                        sender,
                                                                                        connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent),
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

            LocalController.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCertificateSignedRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCertificateSignedRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCertificateSignedResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCertificateSignedResponseSent),
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

            LocalController.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDeleteCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDeleteCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDeleteCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDeleteCertificateResponseSent),
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

            LocalController.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent),
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

            LocalController.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnInstallCertificateRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnInstallCertificateRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnInstallCertificateResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnInstallCertificateResponseSent),
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

            LocalController.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyCRLRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyCRLRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyCRLRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyCRLResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyCRLResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyCRLResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyCRLResponseSent),
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

            LocalController.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCancelReservationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCancelReservationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCancelReservationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCancelReservationResponseSent),
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

            LocalController.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearChargingProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearChargingProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearChargingProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearChargingProfileResponseSent),
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

            LocalController.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetChargingProfilesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetChargingProfilesRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetChargingProfilesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetChargingProfilesResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetChargingProfilesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetChargingProfilesResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetChargingProfilesResponseSent),
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

            LocalController.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCompositeScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCompositeScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCompositeScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCompositeScheduleResponseSent),
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

            LocalController.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetTransactionStatusRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetTransactionStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetTransactionStatusRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetTransactionStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetTransactionStatusResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetTransactionStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetTransactionStatusResponseSent),
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

            LocalController.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent),
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

            LocalController.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRequestStartTransactionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnRequestStartTransactionRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRequestStartTransactionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnRequestStartTransactionResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRequestStartTransactionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnRequestStartTransactionResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRequestStartTransactionResponseSent),
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

            LocalController.OCPP.IN.OnRequestStopTransactionRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRequestStopTransactionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnRequestStopTransactionRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRequestStopTransactionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnRequestStopTransactionResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRequestStopTransactionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnRequestStopTransactionResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRequestStopTransactionResponseSent),
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

            LocalController.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReserveNowRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReserveNowRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReserveNowResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReserveNowResponseSent),
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

            LocalController.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetChargingProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetChargingProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetChargingProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetChargingProfileResponseSent),
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

            LocalController.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnlockConnectorRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnlockConnectorRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnlockConnectorResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnlockConnectorResponseSent),
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

            LocalController.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateDynamicScheduleRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateDynamicScheduleRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateDynamicScheduleResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUpdateDynamicScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateDynamicScheduleResponseSent),
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

            LocalController.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUsePriorityChargingRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUsePriorityChargingRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUsePriorityChargingRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUsePriorityChargingResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUsePriorityChargingResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUsePriorityChargingResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUsePriorityChargingResponseSent),
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

            LocalController.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearDisplayMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearDisplayMessageRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearDisplayMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearDisplayMessageResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearDisplayMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearDisplayMessageResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearDisplayMessageResponseSent),
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

            LocalController.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCostUpdatedRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCostUpdatedRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCostUpdatedRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCostUpdatedResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCostUpdatedResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCostUpdatedResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCostUpdatedResponseSent),
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

            LocalController.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCustomerInformationRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCustomerInformationRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCustomerInformationRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCustomerInformationResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCustomerInformationResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCustomerInformationResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCustomerInformationResponseSent),
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

            LocalController.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetDisplayMessagesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetDisplayMessagesRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetDisplayMessagesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetDisplayMessagesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetDisplayMessagesResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetDisplayMessagesResponseSent),
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

            LocalController.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetDisplayMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetDisplayMessageRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetDisplayMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetDisplayMessageResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetDisplayMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetDisplayMessageResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetDisplayMessageResponseSent),
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

            LocalController.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnChangeAvailabilityRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnChangeAvailabilityRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnChangeAvailabilityResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnChangeAvailabilityResponseSent),
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

            LocalController.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearVariableMonitoringRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearVariableMonitoringRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearVariableMonitoringRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearVariableMonitoringResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearVariableMonitoringResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearVariableMonitoringResponseSent),
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

            LocalController.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetBaseReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetBaseReportRequestSent += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetBaseReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetBaseReportResponseReceived += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetBaseReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetBaseReportResponseSent),
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

            LocalController.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLogRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                            sender,
                                                            //connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLogRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLogResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLogResponseSent),
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

            LocalController.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetMonitoringReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetMonitoringReportRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetMonitoringReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetMonitoringReportResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetMonitoringReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetMonitoringReportResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetMonitoringReportResponseSent),
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

            LocalController.OCPP.IN.OnGetReportRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetReportRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetReportRequestSent += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetReportRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetReportResponseReceived += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetReportResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetReportResponseSent),
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

            LocalController.OCPP.IN.OnGetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetVariablesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetVariablesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetVariablesResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetVariablesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetVariablesResponseSent),
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

            LocalController.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetMonitoringBaseRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetMonitoringBaseRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetMonitoringBaseRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetMonitoringBaseResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetMonitoringBaseResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetMonitoringBaseResponseSent),
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

            LocalController.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetMonitoringLevelRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetMonitoringLevelRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetMonitoringLevelRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetMonitoringLevelResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetMonitoringLevelResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetMonitoringLevelResponseSent),
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

            LocalController.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetNetworkProfileRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetNetworkProfileRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetNetworkProfileRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetNetworkProfileResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetNetworkProfileResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetNetworkProfileResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetNetworkProfileResponseSent),
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

            LocalController.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetVariableMonitoringRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetVariableMonitoringRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetVariableMonitoringRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetVariableMonitoringResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetVariableMonitoringResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetVariableMonitoringResponseSent),
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

            LocalController.OCPP.IN.OnSetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetVariablesRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  //connection,
                                                                  request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetVariablesRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetVariablesResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetVariablesResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetVariablesResponseSent),
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

            LocalController.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTriggerMessageRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTriggerMessageRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTriggerMessageResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTriggerMessageResponseSent),
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

            LocalController.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPublishFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnPublishFirmwareRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPublishFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnPublishFirmwareResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnPublishFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnPublishFirmwareResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnPublishFirmwareResponseSent),
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

            LocalController.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnResetRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                           sender,
                                                           //connection,
                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnResetRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                               sender,
                                                               //connection,
                                                               request,
                                                               response,
                                                               runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnResetResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnResetResponseSent),
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

            LocalController.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnpublishFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUnpublishFirmwareRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnpublishFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnpublishFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUnpublishFirmwareResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnpublishFirmwareResponseSent),
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

            LocalController.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateFirmwareRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateFirmwareRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateFirmwareResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateFirmwareResponseSent),
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

            LocalController.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAFRRSignalRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnAFRRSignalRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAFRRSignalRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnAFRRSignalResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAFRRSignalResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnAFRRSignalResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAFRRSignalResponseSent),
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

            LocalController.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearCacheRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                                sender,
                                                                //connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearCacheRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearCacheResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearCacheResponseSent),
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

            LocalController.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLocalListVersionRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLocalListVersionRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLocalListVersionResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLocalListVersionResponseSent),
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

            LocalController.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSendLocalListRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSendLocalListRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSendLocalListResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSendLocalListResponseSent),
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

            LocalController.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDataTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                                   sender,
                                                                   //connection,
                                                                   request,
                                                                   sendMessageResult) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDataTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDataTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDataTransferResponseSent),
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

            LocalController.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBinaryDataTransferRequestReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            LocalController.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         sendMessageResult) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBinaryDataTransferRequestSent),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            LocalController.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBinaryDataTransferResponseReceived),
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64()),
                                         new JProperty("response",    response.  ToBinary().ToBase64()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBinaryDataTransferResponseSent),
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

        }

        #endregion


    }

}
