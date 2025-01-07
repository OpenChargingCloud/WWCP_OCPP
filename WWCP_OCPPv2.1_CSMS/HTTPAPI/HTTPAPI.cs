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

using System.Reflection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// OCPP CSMS HTTP API extensions.
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
                    Connection      = ConnectionType.Close
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
                    Connection      = ConnectionType.Close
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
                    Connection      = ConnectionType.Close
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
                    Connection      = ConnectionType.Close
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
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP CSMS HTTP API.
    /// </summary>
    public class HTTPAPI : NetworkingNode.HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly        HTTPPath            DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const           String              DefaultHTTPServerName     = $"Open Charging Cloud OCPP {Version.String} Networking Node HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const           String              DefaultHTTPRealm          = "Open Charging Cloud OCPP Networking Node HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const           String              HTTPRoot                  = "cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.HTTPAPI.HTTPRoot.";


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


        protected readonly  ConcurrentDictionary<ChargingStation_Id, ChargingStation>  chargingStations  = [];

        protected readonly ACSMSNode csms;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="CSMS">An OCPP charging station management system.</param>
        /// <param name="HTTPExtAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ACSMSNode                                   CSMS,
                       HTTPExtAPI                                  HTTPExtAPI,
                       String?                                     HTTPServerName         = null,
                       HTTPPath?                                   URLPathPrefix          = null,
                       HTTPPath?                                   BasePath               = null,

                       Boolean                                     EventLoggingDisabled   = true,

                       String                                      HTTPRealm              = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins             = null,
                       Formatting                                  JSONFormatting         = Formatting.None)

            : base(CSMS,
                   HTTPExtAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,

                   EventLoggingDisabled,

                   HTTPRealm,
                   HTTPLogins,
                   JSONFormatting)

        {

            this.csms = CSMS;

            RegisterURITemplates();
            AttachCSMS(csms);

            DebugX.Log($"CSMS HTTP API started on {HTTPBaseAPI.HTTPServer.IPPorts.AggregateWith(", ")}");

        }

        #endregion


        #region AttachCSMS(CSMS)

        public void AttachCSMS(ACSMSNode CSMS)
        {

            // Wire HTTP Server Sent Events

            #region Generic JSON Messages

            #region OnJSONMessageRequestReceived

            CSMS.OnJSONMessageRequestReceived += (timestamp,
                                                            webSocketServer,
                                                            webSocketConnection,
                                                            networkingNodeId,
                                                            networkPath,
                                                            eventTrackingId,
                                                            requestTimestamp,
                                                            requestMessage,
                                                            cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseSent

            CSMS.OnJSONMessageResponseSent += (timestamp,
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

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseSent

            //NetworkingNode.OnJSONErrorResponseSent += (timestamp,
            //                                           webSocketServer,
            //                                           webSocketConnection,
            //                                           eventTrackingId,
            //                                           requestTimestamp,
            //                                           jsonRequestMessage,
            //                                           binaryRequestMessage,
            //                                           responseTimestamp,
            //                                           responseMessage,
            //                                           cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion


            #region OnJSONMessageRequestSent

            CSMS.OnJSONMessageRequestSent += (timestamp,
                                                        webSocketServer,
                                                        webSocketConnection,
                                                        networkingNodeId,
                                                        networkPath,
                                                        eventTrackingId,
                                                        requestTimestamp,
                                                        requestMessage,
                                                        cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseReceived

            CSMS.OnJSONMessageResponseReceived += (timestamp,
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

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseReceived

            //NetworkingNode.OnJSONErrorResponseReceived += (timestamp,
            //                                               webSocketServer,
            //                                               webSocketConnection,
            //                                               eventTrackingId,
            //                                               requestTimestamp,
            //                                               jsonRequestMessage,
            //                                               binaryRequestMessage,
            //                                               responseTimestamp,
            //                                               responseMessage,
            //                                               cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            CSMS.OnBinaryMessageRequestReceived += (timestamp,
                                                              webSocketServer,
                                                              webSocketConnection,
                                                              networkingNodeId,
                                                              networkPath,
                                                              eventTrackingId,
                                                              requestTimestamp,
                                                              requestMessage,
                                                              cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseSent

            CSMS.OnBinaryMessageResponseSent += (timestamp,
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

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseSent),
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

            CSMS.OnBinaryMessageRequestSent += (timestamp,
                                                               webSocketServer,
                                                               webSocketConnection,
                                                               networkingNodeId,
                                                               networkPath,
                                                               eventTrackingId,
                                                               requestTimestamp,
                                                               requestMessage,
                                                               cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseReceived

            CSMS.OnBinaryMessageResponseReceived += (timestamp,
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

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseReceived),
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

            CSMS.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGet15118EVCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGet15118EVCertificateRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGet15118EVCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGet15118EVCertificateResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGet15118EVCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGet15118EVCertificateResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGet15118EVCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCertificateStatus

            CSMS.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCertificateStatusRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetCertificateStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCertificateStatusRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetCertificateStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCertificateStatusResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetCertificateStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCertificateStatusResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCRL

            CSMS.OCPP.IN.OnGetCRLRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCRLRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetCRLRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCRLRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetCRLResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCRLResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetCRLResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCRLResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSignCertificate

            CSMS.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSignCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSignCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSignCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSignCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            CSMS.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnAuthorizeRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnAuthorizeRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnAuthorizeResponseReceived),
                                     JSONObject.Create(
                                               new JProperty("timestamp",   timestamp. ToIso8601()),
                                               new JProperty("sender",      sender.Id),
                                               new JProperty("connection",  connection?.ToJSON()),
                                               new JProperty("request",     request?.  ToJSON()),
                                               new JProperty("response",    response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            CSMS.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnAuthorizeResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearedChargingLimit

            CSMS.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearedChargingLimitRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnClearedChargingLimitRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearedChargingLimitRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnClearedChargingLimitResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearedChargingLimitResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnClearedChargingLimitResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearedChargingLimitResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnMeterValues

            CSMS.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnMeterValuesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnMeterValuesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnMeterValuesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnMeterValuesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyChargingLimit

            CSMS.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyChargingLimitRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyChargingLimitRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyChargingLimitRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyChargingLimitResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyChargingLimitResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyChargingLimitResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyChargingLimitResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingNeeds

            CSMS.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResul,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResul,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingSchedule

            CSMS.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               sentMessageResult,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyPriorityCharging

            CSMS.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyPriorityChargingRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyPriorityChargingRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyPriorityChargingRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyPriorityChargingResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyPriorityChargingResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyPriorityChargingResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifySettlement

            CSMS.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifySettlementRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifySettlementRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifySettlementRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifySettlementResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifySettlementResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifySettlementResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifySettlementResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPullDynamicScheduleUpdate

            CSMS.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               sentMessageResult,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReportChargingProfiles

            CSMS.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReportChargingProfilesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnReportChargingProfilesRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReportChargingProfilesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnReportChargingProfilesResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReportChargingProfilesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnReportChargingProfilesResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReportChargingProfilesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReservationStatusUpdate

            CSMS.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReservationStatusUpdateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnReservationStatusUpdateRequestSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReservationStatusUpdateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnReservationStatusUpdateResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReservationStatusUpdateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnReservationStatusUpdateResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReservationStatusUpdateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnStatusNotification

            CSMS.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTransactionEvent

            CSMS.OCPP.IN.OnTransactionEventRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnTransactionEventRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnTransactionEventRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnTransactionEventRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnTransactionEventResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnTransactionEventResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnTransactionEventResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnTransactionEventResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation

            CSMS.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyCustomerInformationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyCustomerInformationRequestSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               sentMessageResult,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyCustomerInformationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyCustomerInformationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyCustomerInformationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyCustomerInformationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyDisplayMessages

            CSMS.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyDisplayMessagesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyDisplayMessagesRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyDisplayMessagesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyDisplayMessagesResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyDisplayMessagesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyDisplayMessagesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnLogStatusNotification

            CSMS.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnLogStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnLogStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnLogStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnLogStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEvent

            CSMS.OCPP.IN.OnNotifyEventRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEventRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyEventRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEventRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyEventResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyEventResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyEventResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyEventResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyMonitoringReport

            CSMS.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyMonitoringReportRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyMonitoringReportRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyMonitoringReportRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyMonitoringReportResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyMonitoringReportResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyMonitoringReportResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyReport

            CSMS.OCPP.IN.OnNotifyReportRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyReportRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyReportRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyReportRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyReportResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyReportResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyReportResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyReportResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSecurityEventNotification

            CSMS.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSecurityEventNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               sentMessageResult,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSecurityEventNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSecurityEventNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSecurityEventNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            CSMS.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnBootNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnBootNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnBootNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            CSMS.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnBootNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnFirmwareStatusNotification

            CSMS.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnFirmwareStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnFirmwareStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnFirmwareStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnFirmwareStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnHeartbeat

            CSMS.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnHeartbeatRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnHeartbeatRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnHeartbeatResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnHeartbeatResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPublishFirmwareStatusNotification

            CSMS.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                          sender,
                                                                                          connection,
                                                                                          request,
                                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                       sender,
                                                                                       connection,
                                                                                       request,
                                                                                       sentMessageResult,
                                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                           sender,
                                                                                           connection,
                                                                                           request,
                                                                                           response,
                                                                                           runtime,
                                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                        sender,
                                                                                        connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime,
                                                                                        sentMessageResult,
                                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
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

            CSMS.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCertificateSignedRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCertificateSignedRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCertificateSignedResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCertificateSignedResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnDeleteCertificate

            CSMS.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnDeleteCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnDeleteCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnDeleteCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnDeleteCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetInstalledCertificateIds

            CSMS.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnInstallCertificate

            CSMS.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnInstallCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnInstallCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnInstallCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnInstallCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyCRL

            CSMS.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyCRLRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyCRLRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyCRLRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyCRLResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyCRLResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyCRLResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyCRLResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnCancelReservation

            CSMS.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCancelReservationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCancelReservationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCancelReservationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCancelReservationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearChargingProfile

            CSMS.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearChargingProfileRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearChargingProfileRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearChargingProfileResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearChargingProfileResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetChargingProfiles

            CSMS.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetChargingProfilesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetChargingProfilesRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetChargingProfilesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetChargingProfilesResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetChargingProfilesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetChargingProfilesResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetChargingProfilesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCompositeSchedule

            CSMS.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCompositeScheduleRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCompositeScheduleRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetCompositeScheduleResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetCompositeScheduleResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetTransactionStatus

            CSMS.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetTransactionStatusRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetTransactionStatusRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetTransactionStatusRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetTransactionStatusResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetTransactionStatusResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetTransactionStatusResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetTransactionStatusResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            CSMS.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime,
                                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime,
                                                                                  sentMessageResult,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStartTransaction

            CSMS.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnRequestStartTransactionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnRequestStartTransactionRequestSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnRequestStartTransactionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnRequestStartTransactionResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnRequestStartTransactionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnRequestStartTransactionResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnRequestStartTransactionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStopTransaction

            CSMS.OCPP.IN.OnRequestStopTransactionRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnRequestStopTransactionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnRequestStopTransactionRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnRequestStopTransactionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnRequestStopTransactionResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnRequestStopTransactionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnRequestStopTransactionResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnRequestStopTransactionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReserveNow

            CSMS.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReserveNowRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReserveNowRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnReserveNowResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnReserveNowResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetChargingProfile

            CSMS.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetChargingProfileRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetChargingProfileRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetChargingProfileResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetChargingProfileResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnlockConnector

            CSMS.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUnlockConnectorRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUnlockConnectorRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUnlockConnectorResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUnlockConnectorResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateDynamicSchedule

            CSMS.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUpdateDynamicScheduleRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUpdateDynamicScheduleRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUpdateDynamicScheduleResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnUpdateDynamicScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUpdateDynamicScheduleResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUsePriorityCharging

            CSMS.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUsePriorityChargingRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnUsePriorityChargingRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUsePriorityChargingRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnUsePriorityChargingResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUsePriorityChargingResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnUsePriorityChargingResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUsePriorityChargingResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage

            CSMS.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearDisplayMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnClearDisplayMessageRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearDisplayMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnClearDisplayMessageResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearDisplayMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnClearDisplayMessageResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearDisplayMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCostUpdated

            CSMS.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCostUpdatedRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnCostUpdatedRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCostUpdatedRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnCostUpdatedResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCostUpdatedResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnCostUpdatedResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCostUpdatedResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCustomerInformation

            CSMS.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCustomerInformationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnCustomerInformationRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCustomerInformationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnCustomerInformationResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnCustomerInformationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnCustomerInformationResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnCustomerInformationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetDisplayMessages

            CSMS.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetDisplayMessagesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetDisplayMessagesRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetDisplayMessagesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetDisplayMessagesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetDisplayMessagesResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetDisplayMessagesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetDisplayMessage

            CSMS.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetDisplayMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetDisplayMessageRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetDisplayMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetDisplayMessageResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetDisplayMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetDisplayMessageResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetDisplayMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnChangeAvailability

            CSMS.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnChangeAvailabilityRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        entMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnChangeAvailabilityRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnChangeAvailabilityResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         entMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnChangeAvailabilityResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearVariableMonitoring

            CSMS.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearVariableMonitoringRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnClearVariableMonitoringRequestSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearVariableMonitoringRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearVariableMonitoringResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnClearVariableMonitoringResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearVariableMonitoringResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetBaseReport

            CSMS.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetBaseReportRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetBaseReportRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetBaseReportRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetBaseReportResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetBaseReportResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetBaseReportResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLog

            CSMS.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetLogRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetLogRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetLogResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetLogResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetMonitoringReport

            CSMS.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetMonitoringReportRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetMonitoringReportRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetMonitoringReportRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetMonitoringReportResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetMonitoringReportResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetMonitoringReportResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetMonitoringReportResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetReport

            CSMS.OCPP.IN.OnGetReportRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetReportRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetReportRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetReportRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetReportResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetReportResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetReportResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetVariables

            CSMS.OCPP.IN.OnGetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetVariablesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetVariablesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetVariablesResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetVariablesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetVariablesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringBase

            CSMS.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetMonitoringBaseRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetMonitoringBaseRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetMonitoringBaseRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetMonitoringBaseResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetMonitoringBaseResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetMonitoringBaseResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringLevel

            CSMS.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetMonitoringLevelRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetMonitoringLevelRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetMonitoringLevelRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetMonitoringLevelResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetMonitoringLevelResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetMonitoringLevelResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetNetworkProfile

            CSMS.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetNetworkProfileRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetNetworkProfileRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetNetworkProfileRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetNetworkProfileResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetNetworkProfileResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetNetworkProfileResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetNetworkProfileResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariableMonitoring

            CSMS.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetVariableMonitoringRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetVariableMonitoringRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetVariableMonitoringRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetVariableMonitoringResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetVariableMonitoringResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetVariableMonitoringResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariables

            CSMS.OCPP.IN.OnSetVariablesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetVariablesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSetVariablesRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetVariablesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSetVariablesResponseReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSetVariablesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSetVariablesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSetVariablesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTriggerMessage

            CSMS.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnTriggerMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnTriggerMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnTriggerMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnTriggerMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware

            CSMS.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPublishFirmwareRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnPublishFirmwareRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPublishFirmwareRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnPublishFirmwareResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnPublishFirmwareResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnPublishFirmwareResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnPublishFirmwareResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReset

            CSMS.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnResetRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           sentMessageResult,
                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnResetRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnResetResponseReceived),
                                     JSONObject.Create(
                                               new JProperty("timestamp",   timestamp. ToIso8601()),
                                               new JProperty("sender",      sender.Id),
                                               new JProperty("connection",  connection?.ToJSON()),
                                               new JProperty("request",     request?.  ToJSON()),
                                               new JProperty("response",    response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            CSMS.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            sentMessageResult,
                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnResetResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnpublishFirmware

            CSMS.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUnpublishFirmwareRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnUnpublishFirmwareRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUnpublishFirmwareRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUnpublishFirmwareResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnUnpublishFirmwareResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUnpublishFirmwareResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateFirmware

            CSMS.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUpdateFirmwareRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUpdateFirmwareRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnUpdateFirmwareResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnUpdateFirmwareResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal

            CSMS.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnAFRRSignalRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnAFRRSignalRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnAFRRSignalRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnAFRRSignalResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnAFRRSignalResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnAFRRSignalResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnAFRRSignalResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region LocalList

            #region OnClearCache

            CSMS.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearCacheRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearCacheRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnClearCacheResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnClearCacheResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLocalListVersion

            CSMS.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetLocalListVersionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetLocalListVersionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnGetLocalListVersionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnGetLocalListVersionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSendLocalList

            CSMS.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSendLocalListRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSendLocalListRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnSendLocalListResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            CSMS.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnSendLocalListResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region CSMS            <-> ChargingStation Message exchanged

            #region OnDataTransfer

            CSMS.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnDataTransferRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnDataTransferRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            CSMS.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      cancellationToken) =>

                EventLog.SubmitEvent(
                    nameof(CSMS.OCPP.IN.OnBinaryDataTransferResponseReceived),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToIso8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToJSON())
                            : null,
                              new JProperty("response",    response.  ToJSON()),
                        runtime.HasValue
                            ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                            : null
                    )
                );


            CSMS.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(
                    nameof(CSMS.OCPP.OUT.OnBinaryDataTransferResponseSent),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToIso8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToJSON())
                            : null,
                              new JProperty("response",    response.  ToJSON()),
                        runtime.HasValue
                            ? new JProperty("runtime", runtime.Value.TotalMilliseconds)
                            : null
                    )
                );

            #endregion

            #region OnBinaryDataTransfer

            CSMS.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.IN.OnBinaryDataTransferRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            CSMS.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(CSMS.OCPP.OUT.OnBinaryDataTransferRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            CSMS.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            cancellationToken) =>

                EventLog.SubmitEvent(
                    nameof(CSMS.OCPP.IN.OnBinaryDataTransferResponseReceived),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToIso8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToBinary().ToBase64())
                            : null,
                              new JProperty("response",    response.  ToBinary().ToBase64()),
                        runtime.HasValue
                            ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                            : null
                    )
                );


            CSMS.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(
                    nameof(CSMS.OCPP.OUT.OnBinaryDataTransferResponseSent),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToIso8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToBinary().ToBase64())
                            : null,
                              new JProperty("response",    response.  ToBinary().ToBase64()),
                        runtime.HasValue
                            ? new JProperty("runtime", runtime.Value.TotalMilliseconds)
                            : null
                    )
                );

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

            //HTTPBaseAPI.HTTPServer.AddAuth(request => {

            //    if (request.Path.ToString() == "/systemInfo" ||
            //        request.Path.ToString() == "/servers"    ||
            //        request.Path.ToString() == "/connections")
            //    {
            //        return HTTPExtAPI.Anonymous;
            //    }

            //    return null;

            //});


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
            //                                         Connection                 = ConnectionType.Close
            //                                     }.AsImmutable);

            //                             });

            #endregion




            #region ~/events

            #region HTML

            // --------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:5010/events
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
                                                             Connection                 = ConnectionType.Close,
                                                             Vary                       = "Accept"
                                                         }.AsImmutable);

                                          });

            #endregion

            #endregion


        }

        #endregion


    }

}
