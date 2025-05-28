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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CMS
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

        protected readonly ACommissioningServiceNode commissioningService;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given OCPP Commissioning Service WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="CommissioningService">An OCPP Commissioning Service.</param>
        /// <param name="HTTPExtAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ACommissioningServiceNode                   CommissioningService,
                       HTTPExtAPI                                  HTTPExtAPI,
                       String?                                     HTTPServerName         = null,
                       HTTPPath?                                   URLPathPrefix          = null,
                       HTTPPath?                                   BasePath               = null,

                       Boolean                                     EventLoggingDisabled   = true,

                       String                                      HTTPRealm              = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins             = null,
                       Formatting                                  JSONFormatting         = Formatting.None)

            : base(CommissioningService,
                   HTTPExtAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,

                   EventLoggingDisabled,

                   HTTPRealm,
                   HTTPLogins,
                   JSONFormatting)

        {

            this.commissioningService = CommissioningService;

            RegisterURITemplates();
            AttachCommissioningService(commissioningService);

            DebugX.Log($"CSMS HTTP API started on {HTTPBaseAPI.HTTPServer.IPPorts.AggregateWith(", ")}");

        }

        #endregion



        #region AttachCommissioningService(CommissioningService)

        public void AttachCommissioningService(ACommissioningServiceNode CommissioningService)
        {

            // Wire HTTP Server Sent Events

            #region Generic JSON Messages

            //#region OnJSONMessageRequestReceived

            //CSMS.OnJSONMessageRequestReceived += (timestamp,
            //                                                webSocketServer,
            //                                                webSocketConnection,
            //                                                networkingNodeId,
            //                                                networkPath,
            //                                                eventTrackingId,
            //                                                requestTimestamp,
            //                                                requestMessage,
            //                                                cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            //#endregion

            //#region OnJSONMessageResponseSent

            //CSMS.OnJSONMessageResponseSent += (timestamp,
            //                                             webSocketServer,
            //                                             webSocketConnection,
            //                                             networkingNodeId,
            //                                             networkPath,
            //                                             eventTrackingId,
            //                                             requestTimestamp,
            //                                             jsonRequestMessage,
            //                                             binaryRequestMessage,
            //                                             responseTimestamp,
            //                                             responseMessage,
            //                                             cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            //#endregion

            //#region OnJSONErrorResponseSent

            ////NetworkingNode.OnJSONErrorResponseSent += (timestamp,
            ////                                           webSocketServer,
            ////                                           webSocketConnection,
            ////                                           eventTrackingId,
            ////                                           requestTimestamp,
            ////                                           jsonRequestMessage,
            ////                                           binaryRequestMessage,
            ////                                           responseTimestamp,
            ////                                           responseMessage,
            ////                                           cancellationToken) =>

            ////    EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseSent),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)
            ////                         ));

            //#endregion


            //#region OnJSONMessageRequestSent

            //CSMS.OnJSONMessageRequestSent += (timestamp,
            //                                            webSocketServer,
            //                                            webSocketConnection,
            //                                            networkingNodeId,
            //                                            networkPath,
            //                                            eventTrackingId,
            //                                            requestTimestamp,
            //                                            requestMessage,
            //                                            cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            //#endregion

            //#region OnJSONMessageResponseReceived

            //CSMS.OnJSONMessageResponseReceived += (timestamp,
            //                                                 webSocketServer,
            //                                                 webSocketConnection,
            //                                                 networkingNodeId,
            //                                                 networkPath,
            //                                                 eventTrackingId,
            //                                                 requestTimestamp,
            //                                                 jsonRequestMessage,
            //                                                 binaryRequestMessage,
            //                                                 responseTimestamp,
            //                                                 responseMessage,
            //                                                 cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            //#endregion

            //#region OnJSONErrorResponseReceived

            ////NetworkingNode.OnJSONErrorResponseReceived += (timestamp,
            ////                                               webSocketServer,
            ////                                               webSocketConnection,
            ////                                               eventTrackingId,
            ////                                               requestTimestamp,
            ////                                               jsonRequestMessage,
            ////                                               binaryRequestMessage,
            ////                                               responseTimestamp,
            ////                                               responseMessage,
            ////                                               cancellationToken) =>

            ////    EventLog.SubmitEvent(nameof(NetworkingNode.OnJSONErrorResponseReceived),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)
            ////                         ));

            //#endregion

            #endregion

            #region Generic Binary Messages

            //#region OnBinaryMessageRequestReceived

            //CSMS.OnBinaryMessageRequestReceived += (timestamp,
            //                                                  webSocketServer,
            //                                                  webSocketConnection,
            //                                                  networkingNodeId,
            //                                                  networkPath,
            //                                                  eventTrackingId,
            //                                                  requestTimestamp,
            //                                                  requestMessage,
            //                                                  cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryMessageResponseSent

            //CSMS.OnBinaryMessageResponseSent += (timestamp,
            //                                               webSocketServer,
            //                                               webSocketConnection,
            //                                               networkingNodeId,
            //                                               networkPath,
            //                                               eventTrackingId,
            //                                               requestTimestamp,
            //                                               jsonRequestMessage,
            //                                               binaryRequestMessage,
            //                                               responseTimestamp,
            //                                               responseMessage,
            //                                               cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryErrorResponseSent

            ////NetworkingNode.OnBinaryErrorResponseSent += (timestamp,
            ////                                                  webSocketServer,
            ////                                                  webSocketConnection,
            ////                                                  eventTrackingId,
            ////                                                  requestTimestamp,
            ////                                                  jsonRequestMessage,
            ////                                                  binaryRequestMessage,
            ////                                                  responseTimestamp,
            ////                                                  responseMessage) =>

            ////    EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryErrorResponseSent),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            ////                         ));

            //#endregion


            //#region OnBinaryMessageRequestSent

            //CSMS.OnBinaryMessageRequestSent += (timestamp,
            //                                                   webSocketServer,
            //                                                   webSocketConnection,
            //                                                   networkingNodeId,
            //                                                   networkPath,
            //                                                   eventTrackingId,
            //                                                   requestTimestamp,
            //                                                   requestMessage,
            //                                                   cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryMessageResponseReceived

            //CSMS.OnBinaryMessageResponseReceived += (timestamp,
            //                                                        webSocketServer,
            //                                                        webSocketConnection,
            //                                                        networkingNodeId,
            //                                                        networkPath,
            //                                                        eventTrackingId,
            //                                                        requestTimestamp,
            //                                                        jsonRequestMessage,
            //                                                        binaryRequestMessage,
            //                                                        responseTimestamp,
            //                                                        responseMessage,
            //                                                        cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryErrorResponseReceived

            ////NetworkingNode.OnBinaryErrorResponseReceived += (timestamp,
            ////                                                      webSocketServer,
            ////                                                      webSocketConnection,
            ////                                                      eventTrackingId,
            ////                                                      requestTimestamp,
            ////                                                      jsonRequestMessage,
            ////                                                      binaryRequestMessage,
            ////                                                      responseTimestamp,
            ////                                                      responseMessage) =>

            ////    EventLog.SubmitEvent(nameof(NetworkingNode.OnBinaryErrorResponseReceived),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            ////                         ));

            //#endregion

            #endregion


            #region ChargingStation  -> CSMS            Message exchanged

            #region Certificates

            #region OnGet15118EVCertificate

            CommissioningService.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGet15118EVCertificateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGet15118EVCertificateRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGet15118EVCertificateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGet15118EVCertificateResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGet15118EVCertificateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGet15118EVCertificateResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGet15118EVCertificateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGet15118EVCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetCertificateStatus

            CommissioningService.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetCertificateStatusRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetCertificateStatusRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetCertificateStatusRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetCertificateStatusResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetCertificateStatusResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetCertificateStatusResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetCertificateStatusResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetCRL

            //CSMS.OCPP.IN.OnGetCRLRequestReceived += (timestamp,
            //                                         sender,
            //                                         connection,
            //                                         request,
            //                                         ct) =>

            //    NotifyRequestReceived(
            //        nameof(CSMS.OCPP.IN.OnGetCRLRequestReceived),
            //        timestamp,
            //        sender,
            //        connection,
            //        request,
            //        request.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLRequestSerializer,
            //            NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        ct
            //    );

            //CSMS.OCPP.OUT.OnGetCRLRequestSent += (timestamp,
            //                                      sender,
            //                                      connection,
            //                                      request,
            //                                      sentMessageResult,
            //                                      ct) =>

            //    NotifyRequestSent(
            //        nameof(CSMS.OCPP.OUT.OnGetCRLRequestSent),
            //        timestamp,
            //        sender,
            //        connection,
            //        request,
            //        request.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLRequestSerializer,
            //            NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        sentMessageResult,
            //        ct
            //    );


            //CSMS.OCPP.IN.OnGetCRLResponseReceived += (timestamp,
            //                                          sender,
            //                                          connection,
            //                                          request,
            //                                          response,
            //                                          runtime,
            //                                          ct) =>

            //    NotifyResponseReceived(
            //        nameof(CSMS.OCPP.IN.OnGetCRLResponseReceived),
            //        timestamp,
            //        sender,
            //        connection,
            //        request,
            //        request?.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLRequestSerializer,
            //            NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        response,
            //        response.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLResponseSerializer,
            //            NetworkingNode.OCPP.CustomStatusInfoSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        runtime,
            //        ct
            //    );

            //CSMS.OCPP.OUT.OnGetCRLResponseSent += (timestamp,
            //                                       sender,
            //                                       connection,
            //                                       request,
            //                                       response,
            //                                       runtime,
            //                                       sentMessageResult,
            //                                       ct) =>

            //    NotifyResponseSent(
            //        nameof(CSMS.OCPP.OUT.OnGetCRLResponseSent),
            //        timestamp,
            //        sender,
            //        connection,
            //        request,
            //        request?.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLRequestSerializer,
            //            NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        response,
            //        response.ToJSON(
            //            true,
            //            NetworkingNode.OCPP.CustomGetCRLResponseSerializer,
            //            NetworkingNode.OCPP.CustomStatusInfoSerializer,
            //            NetworkingNode.OCPP.CustomSignatureSerializer,
            //            NetworkingNode.OCPP.CustomCustomDataSerializer
            //        ),
            //        runtime,
            //        sentMessageResult,
            //        ct
            //    );

            #endregion

            #region OnSignCertificate

            CommissioningService.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSignCertificateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           sentMessageResult,
                                                           ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSignCertificateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSignCertificateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSignCertificateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSignCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            CommissioningService.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnAuthorizeRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     sentMessageResult,
                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnAuthorizeRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnAuthorizeResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnAuthorizeResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnClearedChargingLimit

            CommissioningService.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearedChargingLimitRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearedChargingLimitRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearedChargingLimitRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnClearedChargingLimitResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearedChargingLimitResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearedChargingLimitResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearedChargingLimitResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearedChargingLimitResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnMeterValues

            CommissioningService.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnMeterValuesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnMeterValuesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnMeterValuesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnMeterValuesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomMeterValuesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyChargingLimit

            CommissioningService.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyChargingLimitRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingLimitSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyChargingLimitRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyChargingLimitRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingLimitSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyChargingLimitResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyChargingLimitResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingLimitSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyChargingLimitResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyChargingLimitResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingLimitSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyEVChargingNeeds

            CommissioningService.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingNeedsSerializer,
                        NetworkingNode.OCPP.CustomACChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingNeedsSerializer,
                        NetworkingNode.OCPP.CustomACChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingNeedsSerializer,
                        NetworkingNode.OCPP.CustomACChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingNeedsSerializer,
                        NetworkingNode.OCPP.CustomACChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                        NetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                        NetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingNeedsResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyEVChargingSchedule

            CommissioningService.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    sentMessageResult,
                                                                    ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     sentMessageResult,
                                                                     ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEVChargingScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyPriorityCharging

            CommissioningService.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyPriorityChargingRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyPriorityChargingRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyPriorityChargingRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyPriorityChargingResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyPriorityChargingResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyPriorityChargingResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyPriorityChargingResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifySettlement

            CommissioningService.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifySettlementRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementRequestSerializer,
                        NetworkingNode.OCPP.CustomContactSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifySettlementRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifySettlementRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementRequestSerializer,
                        NetworkingNode.OCPP.CustomContactSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifySettlementResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifySettlementResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementRequestSerializer,
                        NetworkingNode.OCPP.CustomContactSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifySettlementResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifySettlementResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementRequestSerializer,
                        NetworkingNode.OCPP.CustomContactSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifySettlementResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnPullDynamicScheduleUpdate

            CommissioningService.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateResponseSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPullDynamicScheduleUpdateResponseSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnReportChargingProfiles

            CommissioningService.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnReportChargingProfilesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnReportChargingProfilesRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnReportChargingProfilesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnReportChargingProfilesResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnReportChargingProfilesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnReportChargingProfilesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnReportChargingProfilesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReportChargingProfilesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnReservationStatusUpdate

            CommissioningService.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnReservationStatusUpdateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnReservationStatusUpdateRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnReservationStatusUpdateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnReservationStatusUpdateResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnReservationStatusUpdateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnReservationStatusUpdateResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnReservationStatusUpdateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReservationStatusUpdateResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnStatusNotification

            CommissioningService.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnStatusNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnStatusNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnStatusNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnStatusNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnTransactionEvent

            CommissioningService.OCPP.IN.OnTransactionEventRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnTransactionEventRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventRequestSerializer,
                        NetworkingNode.OCPP.CustomTransactionSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignedMeterValueSerializer,
                        NetworkingNode.OCPP.CustomUnitsOfMeasureSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnTransactionEventRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnTransactionEventRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventRequestSerializer,
                        NetworkingNode.OCPP.CustomTransactionSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignedMeterValueSerializer,
                        NetworkingNode.OCPP.CustomUnitsOfMeasureSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnTransactionEventResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnTransactionEventResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventRequestSerializer,
                        NetworkingNode.OCPP.CustomTransactionSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignedMeterValueSerializer,
                        NetworkingNode.OCPP.CustomUnitsOfMeasureSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventResponseSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnTransactionEventResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnTransactionEventResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventRequestSerializer,
                        NetworkingNode.OCPP.CustomTransactionSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomMeterValueSerializer,
                        NetworkingNode.OCPP.CustomSampledValueSerializer,
                        NetworkingNode.OCPP.CustomSignedMeterValueSerializer,
                        NetworkingNode.OCPP.CustomUnitsOfMeasureSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTransactionEventResponseSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation

            CommissioningService.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyCustomerInformationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyCustomerInformationRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyCustomerInformationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyCustomerInformationResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyCustomerInformationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyCustomerInformationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCustomerInformationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyDisplayMessages

            CommissioningService.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyDisplayMessagesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyDisplayMessagesRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyDisplayMessagesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyDisplayMessagesResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyDisplayMessagesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyDisplayMessagesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region DeviceModel

            #region OnLogStatusNotification

            CommissioningService.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnLogStatusNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnLogStatusNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnLogStatusNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnLogStatusNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomLogStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyEvent

            CommissioningService.OCPP.IN.OnNotifyEventRequestReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEventRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                        NetworkingNode.OCPP.CustomEventDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEventRequestSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEventRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                        NetworkingNode.OCPP.CustomEventDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyEventResponseReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyEventResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                        NetworkingNode.OCPP.CustomEventDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyEventResponseSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyEventResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventRequestSerializer,
                        NetworkingNode.OCPP.CustomEventDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyEventResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyMonitoringReport

            CommissioningService.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyMonitoringReportRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableMonitoringSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyMonitoringReportRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyMonitoringReportRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableMonitoringSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyMonitoringReportResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyMonitoringReportResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableMonitoringSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyMonitoringReportResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableMonitoringSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyMonitoringReportResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyReport

            CommissioningService.OCPP.IN.OnNotifyReportRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyReportRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportRequestSerializer,
                        NetworkingNode.OCPP.CustomReportDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableAttributeSerializer,
                        NetworkingNode.OCPP.CustomVariableCharacteristicsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyReportRequestSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyReportRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportRequestSerializer,
                        NetworkingNode.OCPP.CustomReportDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableAttributeSerializer,
                        NetworkingNode.OCPP.CustomVariableCharacteristicsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyReportResponseReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyReportResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportRequestSerializer,
                        NetworkingNode.OCPP.CustomReportDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableAttributeSerializer,
                        NetworkingNode.OCPP.CustomVariableCharacteristicsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyReportResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyReportResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportRequestSerializer,
                        NetworkingNode.OCPP.CustomReportDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomVariableAttributeSerializer,
                        NetworkingNode.OCPP.CustomVariableCharacteristicsSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyReportResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSecurityEventNotification

            CommissioningService.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSecurityEventNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSecurityEventNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSecurityEventNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSecurityEventNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSecurityEventNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            CommissioningService.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnBootNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingStationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnBootNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingStationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnBootNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingStationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnBootNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingStationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnFirmwareStatusNotification

            CommissioningService.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnFirmwareStatusNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnFirmwareStatusNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnFirmwareStatusNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnFirmwareStatusNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnHeartbeat

            CommissioningService.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnHeartbeatRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     sentMessageResult,
                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnHeartbeatRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnHeartbeatResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnHeartbeatResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnPublishFirmwareStatusNotification

            CommissioningService.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             sentMessageResult,
                                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              sentMessageResult,
                                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareStatusNotificationResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #endregion

            #region CSMS             -> ChargingStation Message exchanged

            #region Certificates

            #region OnCertificateSigned

            CommissioningService.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnCertificateSignedRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnCertificateSignedRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnCertificateSignedResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnCertificateSignedResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCertificateSignedResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnDeleteCertificate

            CommissioningService.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnDeleteCertificateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnDeleteCertificateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnDeleteCertificateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnDeleteCertificateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDeleteCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetInstalledCertificateIds

            CommissioningService.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsResponseSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataChainSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetInstalledCertificateIdsResponseSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataChainSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnInstallCertificate

            CommissioningService.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnInstallCertificateRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnInstallCertificateRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnInstallCertificateResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnInstallCertificateResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomInstallCertificateResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyCRL

            CommissioningService.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyCRLRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyCRLRequestSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     sentMessageResult,
                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyCRLRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyCRLResponseReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyCRLResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyCRLResponseSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyCRLResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyCRLResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Charging

            #region OnCancelReservation

            CommissioningService.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnCancelReservationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnCancelReservationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnCancelReservationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnCancelReservationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCancelReservationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnClearChargingProfile

            CommissioningService.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearChargingProfileRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearChargingProfileRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearChargingProfileResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearChargingProfileResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearChargingProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetChargingProfiles

            CommissioningService.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetChargingProfilesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileCriterionSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetChargingProfilesRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetChargingProfilesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileCriterionSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetChargingProfilesResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetChargingProfilesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileCriterionSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetChargingProfilesResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetChargingProfilesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileCriterionSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetChargingProfilesResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetCompositeSchedule

            CommissioningService.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetCompositeScheduleRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetCompositeScheduleRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetCompositeScheduleResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomCompositeScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetCompositeScheduleResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomCompositeScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetTransactionStatus

            CommissioningService.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetTransactionStatusRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetTransactionStatusRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetTransactionStatusRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetTransactionStatusResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetTransactionStatusResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetTransactionStatusResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetTransactionStatusResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetTransactionStatusResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            CommissioningService.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnRequestStartTransaction

            CommissioningService.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnRequestStartTransactionRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnRequestStartTransactionRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnRequestStartTransactionRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnRequestStartTransactionResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnRequestStartTransactionResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnRequestStartTransactionResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnRequestStartTransactionResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnRequestStopTransaction

            CommissioningService.OCPP.IN.OnRequestStopTransactionRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnRequestStopTransactionRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnRequestStopTransactionRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnRequestStopTransactionRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnRequestStopTransactionResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnRequestStopTransactionResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnRequestStopTransactionResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnRequestStopTransactionResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomRequestStopTransactionResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnReserveNow

            CommissioningService.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnReserveNowRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnReserveNowRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnReserveNowResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       response,
                                                       runtime,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnReserveNowResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomReserveNowResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetChargingProfile

            CommissioningService.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetChargingProfileRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetChargingProfileRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetChargingProfileResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetChargingProfileResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingProfileSerializer,
                        NetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleSerializer,
                        NetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                        NetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                        NetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                        NetworkingNode.OCPP.CustomSalesTariffSerializer,
                        NetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                        NetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                        NetworkingNode.OCPP.CustomConsumptionCostSerializer,
                        NetworkingNode.OCPP.CustomCostSerializer,

                        NetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                        NetworkingNode.OCPP.CustomPriceRuleSerializer,
                        NetworkingNode.OCPP.CustomTaxRuleSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                        NetworkingNode.OCPP.CustomOverstayRuleSerializer,
                        NetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                        NetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                        NetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetChargingProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnUnlockConnector

            CommissioningService.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnUnlockConnectorRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           sentMessageResult,
                                                           ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnUnlockConnectorRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnUnlockConnectorResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnUnlockConnectorResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnlockConnectorResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnUpdateDynamicSchedule

            CommissioningService.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnUpdateDynamicScheduleRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnUpdateDynamicScheduleRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnUpdateDynamicScheduleResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnUpdateDynamicScheduleResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnUpdateDynamicScheduleResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleRequestSerializer,
                        NetworkingNode.OCPP.CustomChargingScheduleUpdateSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateDynamicScheduleResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnUsePriorityCharging

            CommissioningService.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnUsePriorityChargingRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnUsePriorityChargingRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnUsePriorityChargingRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnUsePriorityChargingResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnUsePriorityChargingResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnUsePriorityChargingResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnUsePriorityChargingResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUsePriorityChargingResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage

            CommissioningService.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearDisplayMessageRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearDisplayMessageRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearDisplayMessageRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnClearDisplayMessageResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearDisplayMessageResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearDisplayMessageResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearDisplayMessageResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearDisplayMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnCostUpdated

            CommissioningService.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnCostUpdatedRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnCostUpdatedRequestSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnCostUpdatedRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnCostUpdatedResponseReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnCostUpdatedResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnCostUpdatedResponseSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnCostUpdatedResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCostUpdatedResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnCustomerInformation

            CommissioningService.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnCustomerInformationRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnCustomerInformationRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnCustomerInformationRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnCustomerInformationResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnCustomerInformationResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnCustomerInformationResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnCustomerInformationResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationRequestSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomCustomerInformationResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetDisplayMessages

            CommissioningService.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetDisplayMessagesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetDisplayMessagesRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetDisplayMessagesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetDisplayMessagesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetDisplayMessagesResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetDisplayMessagesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetDisplayMessage

            CommissioningService.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetDisplayMessageRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetDisplayMessageRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetDisplayMessageRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetDisplayMessageResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetDisplayMessageResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetDisplayMessageResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetDisplayMessageResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomMessageInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetDisplayMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region DeviceModel

            #region OnChangeAvailability

            CommissioningService.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnChangeAvailabilityRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnChangeAvailabilityRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnChangeAvailabilityResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnChangeAvailabilityResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnClearVariableMonitoring

            CommissioningService.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearVariableMonitoringRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearVariableMonitoringRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearVariableMonitoringRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearVariableMonitoringResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                        NetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearVariableMonitoringResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearVariableMonitoringResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                        NetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetBaseReport

            CommissioningService.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetBaseReportRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetBaseReportRequestSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetBaseReportRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetBaseReportResponseReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetBaseReportResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          sentMessageResult,
                                                          ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetBaseReportResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetLog

            CommissioningService.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetLogRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogRequestSerializer,
                        NetworkingNode.OCPP.CustomLogParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  sentMessageResult,
                                                  ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetLogRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogRequestSerializer,
                        NetworkingNode.OCPP.CustomLogParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime,
                                                      ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetLogResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogRequestSerializer,
                        NetworkingNode.OCPP.CustomLogParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   response,
                                                   runtime,
                                                   sentMessageResult,
                                                   ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetLogResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogRequestSerializer,
                        NetworkingNode.OCPP.CustomLogParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLogResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetMonitoringReport

            CommissioningService.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetMonitoringReportRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetMonitoringReportRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetMonitoringReportRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetMonitoringReportResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetMonitoringReportResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetMonitoringReportResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetMonitoringReportResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetReport

            CommissioningService.OCPP.IN.OnGetReportRequestReceived += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetReportRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetReportRequestSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     sentMessageResult,
                                                     ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetReportRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetReportResponseReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetReportResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetReportResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportRequestSerializer,
                        NetworkingNode.OCPP.CustomComponentVariableSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetReportResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetVariables

            CommissioningService.OCPP.IN.OnGetVariablesRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetVariablesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomGetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetVariablesRequestSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetVariablesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomGetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetVariablesResponseReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetVariablesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomGetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesResponseSerializer,
                        NetworkingNode.OCPP.CustomGetVariableResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetVariablesResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetVariablesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomGetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetVariablesResponseSerializer,
                        NetworkingNode.OCPP.CustomGetVariableResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetMonitoringBase

            CommissioningService.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetMonitoringBaseRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetMonitoringBaseRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetMonitoringBaseRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetMonitoringBaseResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetMonitoringBaseResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetMonitoringBaseResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringBaseResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetMonitoringLevel

            CommissioningService.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetMonitoringLevelRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetMonitoringLevelRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetMonitoringLevelRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetMonitoringLevelResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetMonitoringLevelResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetMonitoringLevelResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetMonitoringLevelResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetNetworkProfile

            CommissioningService.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetNetworkProfileRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomNetworkConnectionProfileSerializer,
                        NetworkingNode.OCPP.CustomVPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomAPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetNetworkProfileRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetNetworkProfileRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomNetworkConnectionProfileSerializer,
                        NetworkingNode.OCPP.CustomVPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomAPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetNetworkProfileResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetNetworkProfileResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomNetworkConnectionProfileSerializer,
                        NetworkingNode.OCPP.CustomVPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomAPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetNetworkProfileResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        sentMessageResult,
                                                                        ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetNetworkProfileResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileRequestSerializer,
                        NetworkingNode.OCPP.CustomNetworkConnectionProfileSerializer,
                        NetworkingNode.OCPP.CustomVPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomAPNConfigurationSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetNetworkProfileResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetVariableMonitoring

            CommissioningService.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetVariableMonitoringRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomPeriodicEventStreamParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetVariableMonitoringRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetVariableMonitoringRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomPeriodicEventStreamParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetVariableMonitoringResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomPeriodicEventStreamParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringResponseSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetVariableMonitoringResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetVariableMonitoringResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringRequestSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomPeriodicEventStreamParametersSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariableMonitoringResponseSerializer,
                        NetworkingNode.OCPP.CustomSetMonitoringResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSetVariables

            CommissioningService.OCPP.IN.OnSetVariablesRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetVariablesRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomSetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetVariablesRequestSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetVariablesRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomSetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSetVariablesResponseReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSetVariablesResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomSetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesResponseSerializer,
                        NetworkingNode.OCPP.CustomSetVariableResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSetVariablesResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSetVariablesResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                        NetworkingNode.OCPP.CustomSetVariableDataSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSetVariablesResponseSerializer,
                        NetworkingNode.OCPP.CustomSetVariableResultSerializer,
                        NetworkingNode.OCPP.CustomComponentSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomVariableSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnTriggerMessage

            CommissioningService.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnTriggerMessageRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          sentMessageResult,
                                                          ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnTriggerMessageRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnTriggerMessageResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           sentMessageResult,
                                                           ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnTriggerMessageResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageRequestSerializer,
                        NetworkingNode.OCPP.CustomEVSESerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomTriggerMessageResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware

            CommissioningService.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnPublishFirmwareRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnPublishFirmwareRequestSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           sentMessageResult,
                                                           ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnPublishFirmwareRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnPublishFirmwareResponseReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnPublishFirmwareResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnPublishFirmwareResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            sentMessageResult,
                                                            ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnPublishFirmwareResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomPublishFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnReset

            CommissioningService.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnResetRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 sentMessageResult,
                                                 ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnResetRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     response,
                                                     runtime,
                                                     ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnResetResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  response,
                                                  runtime,
                                                  sentMessageResult,
                                                  ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnResetResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomResetResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnUnpublishFirmware

            CommissioningService.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnUnpublishFirmwareRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnUnpublishFirmwareRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnUnpublishFirmwareRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnUnpublishFirmwareResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnUnpublishFirmwareResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnUnpublishFirmwareResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnUpdateFirmware

            CommissioningService.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnUpdateFirmwareRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomFirmwareSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          sentMessageResult,
                                                          ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnUpdateFirmwareRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomFirmwareSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnUpdateFirmwareResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomFirmwareSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime,
                                                           sentMessageResult,
                                                           ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnUpdateFirmwareResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                        NetworkingNode.OCPP.CustomFirmwareSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomUpdateFirmwareResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal

            CommissioningService.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnAFRRSignalRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnAFRRSignalRequestSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnAFRRSignalRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnAFRRSignalResponseReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnAFRRSignalResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnAFRRSignalResponseSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       response,
                                                       runtime,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnAFRRSignalResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #region LocalList

            #region OnClearCache

            CommissioningService.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearCacheRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      sentMessageResult,
                                                      ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearCacheRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnClearCacheResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       response,
                                                       runtime,
                                                       sentMessageResult,
                                                       ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnClearCacheResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomClearCacheResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnGetLocalListVersion

            CommissioningService.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetLocalListVersionRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetLocalListVersionRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnGetLocalListVersionResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                sentMessageResult,
                                                                ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnGetLocalListVersionResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomGetLocalListVersionResponseSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnSendLocalList

            CommissioningService.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnSendLocalListRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                        NetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnSendLocalListRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                        NetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnSendLocalListResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                        NetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime,
                                                          sentMessageResult,
                                                          ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnSendLocalListResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                        NetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                        NetworkingNode.OCPP.CustomIdTokenSerializer,
                        NetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                        NetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                        NetworkingNode.OCPP.CustomMessageContentSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomSendLocalListResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #endregion

            #endregion

            #region CSMS            <-> ChargingStation Message exchanged

            #region OnDataTransfer

            CommissioningService.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnDataTransferRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        sentMessageResult,
                                                        ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnDataTransferRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime,
                                                            ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnDataTransferResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    ct
                );

            CommissioningService.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime,
                                                         sentMessageResult,
                                                         ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnDataTransferResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    response,
                    response.ToJSON(
                        true,
                        NetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomSignatureSerializer,
                        NetworkingNode.OCPP.CustomCustomDataSerializer
                    ),
                    runtime,
                    sentMessageResult,
                    ct
                );

            #endregion

            #region OnBinaryDataTransfer

            CommissioningService.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 ct) =>

                NotifyRequestReceived(
                    nameof(CommissioningService.OCPP.IN.OnBinaryDataTransferRequestReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    ct
                );

            CommissioningService.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              sentMessageResult,
                                                              ct) =>

                NotifyRequestSent(
                    nameof(CommissioningService.OCPP.OUT.OnBinaryDataTransferRequestSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    sentMessageResult,
                    ct
                );


            CommissioningService.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  ct) =>

                NotifyResponseReceived(
                    nameof(CommissioningService.OCPP.IN.OnBinaryDataTransferResponseReceived),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    response,
                    response.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    runtime,
                    ct
                ); 

            CommissioningService.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               response,
                                                               runtime,
                                                               sentMessageResult,
                                                               ct) =>

                NotifyResponseSent(
                    nameof(CommissioningService.OCPP.OUT.OnBinaryDataTransferResponseSent),
                    timestamp,
                    sender,
                    connection,
                    request,
                    request?.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    response,
                    response.ToBinary(
                        NetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                        NetworkingNode.OCPP.CustomStatusInfoSerializer,
                        NetworkingNode.OCPP.CustomBinarySignatureSerializer,
                        true
                    ),
                    runtime,
                    sentMessageResult,
                    ct
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

            //// --------------------------------------------------------------------
            //// curl -v -H "Accept: application/json" http://127.0.0.1:5010/events
            //// --------------------------------------------------------------------
            //HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
            //                              HTTPMethod.GET,
            //                              URLPathPrefix + "events",
            //                              HTTPContentType.Text.HTML_UTF8,
            //                              HTTPDelegate: Request => {

            //                                  #region Get HTTP user and its organizations

            //                                  //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //                                  //if (!TryGetHTTPUser(Request,
            //                                  //                    out User                   HTTPUser,
            //                                  //                    out HashSet<Organization>  HTTPOrganizations,
            //                                  //                    out HTTPResponse.Builder   Response,
            //                                  //                    Recursive:                 true))
            //                                  //{
            //                                  //    return Task.FromResult(Response.AsImmutable);
            //                                  //}

            //                                  #endregion

            //                                  return Task.FromResult(
            //                                             new HTTPResponse.Builder(Request) {
            //                                                 HTTPStatusCode             = HTTPStatusCode.OK,
            //                                                 Server                     = HTTPServiceName,
            //                                                 Date                       = Timestamp.Now,
            //                                                 AccessControlAllowOrigin   = "*",
            //                                                 AccessControlAllowMethods  = [ "GET" ],
            //                                                 AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
            //                                                 ContentType                = HTTPContentType.Text.HTML_UTF8,
            //                                                 Content                    = MixWithHTMLTemplate("events.events.shtml").ToUTF8Bytes(),
            //                                                 Connection                 = ConnectionType.Close,
            //                                                 Vary                       = "Accept"
            //                                             }.AsImmutable);

            //                              });

            #endregion

            #endregion


        }

        #endregion


    }

}
