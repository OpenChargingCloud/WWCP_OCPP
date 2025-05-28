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

using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.LocalController
{

    /// <summary>
    /// OCPP Networking Node HTTP API extensions.
    /// </summary>
    public static class HTTPAPIExtensions
    {

        #region ParseChargePointId(this HTTPRequest, OCPPHTTPAPI, out ChargePointId,                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge point identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargePointId">The parsed unique charge point identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargePointId(this HTTPRequest                                HTTPRequest,
                                                 HTTPAPI                                         OCPPHTTPAPI,
                                                 [NotNullWhen(true)]  out ChargePoint_Id?        ChargePointId,
                                                 [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargePointId  = null;
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

            ChargePointId = ChargePoint_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargePointId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge point identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargePoint  (this HTTPRequest, OCPPHTTPAPI, out ChargePointId, out ChargePoint, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge point identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargePointId">The parsed unique charge point identification.</param>
        /// <param name="ChargePoint">The resolved charge point.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargePoint(this HTTPRequest                                HTTPRequest,
                                               HTTPAPI                                         OCPPHTTPAPI,
                                               [NotNullWhen(true)]  out ChargePoint_Id?        ChargePointId,
                                               [NotNullWhen(true)]  out ChargePoint?           ChargePoint,
                                               [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargePointId  = null;
            ChargePoint    = null;
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

            ChargePointId = ChargePoint_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargePointId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge point identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            if (!OCPPHTTPAPI.TryGetChargePoint(ChargePointId.Value, out ChargePoint)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charge point identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
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
    public class HTTPAPI : NetworkingNode.HTTPAPI
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
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPPv1_6.NetworkingNode.HTTPAPI.HTTPRoot.";


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


        protected readonly ALocalControllerNode localController;

        protected readonly  ConcurrentDictionary<ChargePoint_Id, ChargePoint>  ChargePoints  = [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given local controller WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="HTTPExtAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ALocalControllerNode                        LocalController,
                       HTTPExtAPI                                  HTTPExtAPI,
                       String?                                     HTTPServerName         = null,
                       HTTPPath?                                   URLPathPrefix          = null,
                       HTTPPath?                                   BasePath               = null,

                       Boolean                                     EventLoggingDisabled   = true,

                       String                                      HTTPRealm              = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins             = null,
                       Formatting                                  JSONFormatting         = Formatting.None)

            : base(LocalController,
                   HTTPExtAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,

                   EventLoggingDisabled,

                   HTTPRealm,
                   HTTPLogins,
                   JSONFormatting)

        {

            this.localController = LocalController;

            RegisterURITemplates();
            AttachLocalController(localController);

        }

        #endregion


        #region AttachLocalController(LocalController)

        public void AttachLocalController(ALocalControllerNode LocalController)
        {

            // Wire HTTP Server Sent Events

            #region Generic JSON Messages

            //#region OnJSONMessageRequestReceived

            //LocalController.OnJSONMessageRequestReceived += (timestamp,
            //                                                webSocketServer,
            //                                                webSocketConnection,
            //                                                networkingNodeId,
            //                                                networkPath,
            //                                                eventTrackingId,
            //                                                requestTimestamp,
            //                                                requestMessage,
            //                                                cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            //#endregion

            //#region OnJSONMessageResponseSent

            //LocalController.OnJSONMessageResponseSent += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            //#endregion

            //#region OnJSONErrorResponseSent

            ////LocalController.OnJSONErrorResponseSent += (timestamp,
            ////                                           webSocketServer,
            ////                                           webSocketConnection,
            ////                                           eventTrackingId,
            ////                                           requestTimestamp,
            ////                                           jsonRequestMessage,
            ////                                           binaryRequestMessage,
            ////                                           responseTimestamp,
            ////                                           responseMessage,
            ////                                           cancellationToken) =>

            ////    EventLog.SubmitEvent(nameof(LocalController.OnJSONErrorResponseSent),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)
            ////                         ));

            //#endregion


            //#region OnJSONMessageRequestSent

            //LocalController.OnJSONMessageRequestSent += (timestamp,
            //                                            webSocketServer,
            //                                            webSocketConnection,
            //                                            networkingNodeId,
            //                                            networkPath,
            //                                            eventTrackingId,
            //                                            requestTimestamp,
            //                                            requestMessage,
            //                                            cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            //#endregion

            //#region OnJSONMessageResponseReceived

            //LocalController.OnJSONMessageResponseReceived += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(LocalController.OnJSONMessageResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            //#endregion

            //#region OnJSONErrorResponseReceived

            ////LocalController.OnJSONErrorResponseReceived += (timestamp,
            ////                                               webSocketServer,
            ////                                               webSocketConnection,
            ////                                               eventTrackingId,
            ////                                               requestTimestamp,
            ////                                               jsonRequestMessage,
            ////                                               binaryRequestMessage,
            ////                                               responseTimestamp,
            ////                                               responseMessage,
            ////                                               cancellationToken) =>

            ////    EventLog.SubmitEvent(nameof(LocalController.OnJSONErrorResponseReceived),
            ////                         JSONObject.Create(
            ////                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            ////                             new JProperty("connection",   webSocketConnection.ToJSON()),
            ////                             new JProperty("message",      responseMessage)
            ////                         ));

            //#endregion

            #endregion

            #region Generic Binary Messages

            //#region OnBinaryMessageRequestReceived

            //LocalController.OnBinaryMessageRequestReceived += (timestamp,
            //                                                   webSocketServer,
            //                                                   webSocketConnection,
            //                                                   networkingNodeId,
            //                                                   networkPath,
            //                                                   requestTimestamp,
            //                                                   eventTrackingId,
            //                                                   requestMessage,
            //                                                   cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryMessageResponseSent

            //LocalController.OnBinaryMessageResponseSent += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageResponseSent),
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

            //LocalController.OnBinaryMessageRequestSent += (timestamp,
            //                                                   webSocketServer,
            //                                                   webSocketConnection,
            //                                                   networkingNodeId,
            //                                                   networkPath,
            //                                                   eventTrackingId,
            //                                                   requestTimestamp,
            //                                                   requestMessage,
            //                                                   cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToISO8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            //#endregion

            //#region OnBinaryMessageResponseReceived

            //LocalController.OnBinaryMessageResponseReceived += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(LocalController.OnBinaryMessageResponseReceived),
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


            #region ChargePoint  -> CSMS            Message exchanged

            #region Certificates

            #region OnSignCertificate

            LocalController.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSignCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSignCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSignCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSignCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAuthorizeRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAuthorizeRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",           timestamp. ToISO8601()),
                                         new JProperty("sender",              sender.Id),
                                         new JProperty("connection",          connection?.ToJSON()),
                                         new JProperty("request",             request.   ToJSON()),
                                         new JProperty("sentMessageResult",   sentMessageResult.ToString())
                                         
                                     ));


            LocalController.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnAuthorizeResponseReceived),
                                     JSONObject.Create(
                                               new JProperty("timestamp",    timestamp. ToISO8601()),
                                               new JProperty("sender",       sender.Id),
                                               new JProperty("connection",   connection?.ToJSON()),
                                               new JProperty("request",      request?.  ToJSON()),
                                               new JProperty("response",     response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",      runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            LocalController.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnAuthorizeResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",           timestamp. ToISO8601()),
                                         new JProperty("sender",              sender.Id),
                                         new JProperty("connection",          connection?.ToJSON()),
                                         new JProperty("request",             request?.  ToJSON()),
                                         new JProperty("response",            response.  ToJSON()),
                                         new JProperty("runtime",             runtime.   TotalMilliseconds),
                                         new JProperty("sentMessageResult",   sentMessageResult.ToString())
                                     ));

            #endregion

            #region OnMeterValues

            LocalController.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnMeterValuesRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnMeterValuesRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnMeterValuesResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnMeterValuesResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnStatusNotification

            LocalController.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnLogStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnLogStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnLogStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnLogStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSecurityEventNotification

            LocalController.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSecurityEventNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                sentMessageResult,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSecurityEventNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSecurityEventNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSecurityEventNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBootNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBootNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBootNotificationResponseReceived),
                                     JSONObject.Create(
                                               new JProperty("timestamp",   timestamp. ToISO8601()),
                                               new JProperty("sender",      sender.Id),
                                               new JProperty("connection",  connection?.ToJSON()),
                                               new JProperty("request",     request?.  ToJSON()),
                                               new JProperty("response",    response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            LocalController.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBootNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnFirmwareStatusNotification

            LocalController.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnFirmwareStatusNotificationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnFirmwareStatusNotificationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime,
                                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnFirmwareStatusNotificationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime,
                                                                                  sentMessageResult,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnFirmwareStatusNotificationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnHeartbeat

            LocalController.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnHeartbeatRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                sentMessageResult,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnHeartbeatRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnHeartbeatResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnHeartbeatResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region CSMS             -> ChargePoint Message exchanged

            #region Certificates

            #region OnCertificateSigned

            LocalController.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCertificateSignedRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCertificateSignedRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCertificateSignedResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCertificateSignedResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnDeleteCertificate

            LocalController.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDeleteCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDeleteCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDeleteCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDeleteCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetInstalledCertificateIds

            LocalController.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 sentMessageResult,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime,
                                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime,
                                                                                  sentMessageResult,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnInstallCertificate

            LocalController.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnInstallCertificateRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnInstallCertificateRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnInstallCertificateResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnInstallCertificateResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCancelReservationRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        sentMessageResult,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCancelReservationRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnCancelReservationResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnCancelReservationResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearChargingProfile

            LocalController.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearChargingProfileRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearChargingProfileRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearChargingProfileResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearChargingProfileResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCompositeSchedule

            LocalController.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCompositeScheduleRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCompositeScheduleRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetCompositeScheduleResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime,
                                                                            sentMessageResult,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetCompositeScheduleResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRemoteStartTransaction

            LocalController.OCPP.IN.OnRemoteStartTransactionRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRemoteStartTransactionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnRemoteStartTransactionRequestSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRemoteStartTransactionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnRemoteStartTransactionResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime,
                                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRemoteStartTransactionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnRemoteStartTransactionResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime,
                                                                               sentMessageResult,
                                                                               ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRemoteStartTransactionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRemoteStopTransaction

            LocalController.OCPP.IN.OnRemoteStopTransactionRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRemoteStopTransactionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnRemoteStopTransactionRequestSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             sentMessageResult,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRemoteStopTransactionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnRemoteStopTransactionResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime,
                                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnRemoteStopTransactionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnRemoteStopTransactionResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              sentMessageResult,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnRemoteStopTransactionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReserveNow

            LocalController.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReserveNowRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReserveNowRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnReserveNowResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnReserveNowResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetChargingProfile

            LocalController.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetChargingProfileRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetChargingProfileRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSetChargingProfileResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSetChargingProfileResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnlockConnector

            LocalController.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnlockConnectorRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnlockConnectorRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUnlockConnectorResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       sentMessageResult,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUnlockConnectorResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnChangeAvailabilityRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnChangeAvailabilityRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnChangeAvailabilityResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          entMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnChangeAvailabilityResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLog

            LocalController.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLogRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             sentMessageResult,
                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLogRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 runtime,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLogResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime,
                                                              sentMessageResult,
                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLogResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTriggerMessage

            LocalController.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTriggerMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTriggerMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnTriggerMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnTriggerMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnReset

            LocalController.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnResetRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            sentMessageResult,
                                                            ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnResetRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime,
                                                                ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnResetResponseReceived),
                                     JSONObject.Create(
                                               new JProperty("timestamp",   timestamp. ToISO8601()),
                                               new JProperty("sender",      sender.Id),
                                               new JProperty("connection",  connection?.ToJSON()),
                                               new JProperty("request",     request?.  ToJSON()),
                                               new JProperty("response",    response.  ToJSON()),
                                         runtime.HasValue
                                             ? new JProperty("runtime",     runtime.Value.TotalMilliseconds)
                                             : null
                                     ));


            LocalController.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime,
                                                             sentMessageResult,
                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnResetResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateFirmware

            LocalController.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateFirmwareRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateFirmwareRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnUpdateFirmwareResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime,
                                                                      sentMessageResult,
                                                                      ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnUpdateFirmwareResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
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

            LocalController.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearCacheRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 sentMessageResult,
                                                                 ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearCacheRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnClearCacheResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime,
                                                                  sentMessageResult,
                                                                  ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnClearCacheResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLocalListVersion

            LocalController.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLocalListVersionRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLocalListVersionRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime,
                                                                              ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnGetLocalListVersionResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime,
                                                                           sentMessageResult,
                                                                           ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnGetLocalListVersionResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSendLocalList

            LocalController.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSendLocalListRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSendLocalListRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime,
                                                                        ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnSendLocalListResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request?.  ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime?.  TotalMilliseconds)
                                     ));


            LocalController.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime,
                                                                     sentMessageResult,
                                                                     ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnSendLocalListResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region CSMS            <-> ChargePoint Message exchanged

            #region OnDataTransfer

            LocalController.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnDataTransferRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   sentMessageResult,
                                                                   ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnDataTransferRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            LocalController.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime,
                                                                       cancellationToken) =>

                EventLog.SubmitEvent(
                    nameof(LocalController.OCPP.IN.OnBinaryDataTransferResponseReceived),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToISO8601()),
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


            LocalController.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime,
                                                                    sentMessageResult,
                                                                    ct) =>

                EventLog.SubmitEvent(
                    nameof(LocalController.OCPP.OUT.OnBinaryDataTransferResponseSent),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToISO8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToJSON())
                            : null,
                              new JProperty("response",    response.  ToJSON()),
                              new JProperty("runtime",     runtime.TotalMilliseconds)
                    )
                );

            #endregion

            #region OnBinaryDataTransfer

            LocalController.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            cancellationToken) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.IN.OnBinaryDataTransferRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            LocalController.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         sentMessageResult,
                                                                         ct) =>

                EventLog.SubmitEvent(nameof(LocalController.OCPP.OUT.OnBinaryDataTransferRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",   timestamp. ToISO8601()),
                                         new JProperty("sender",      sender.Id),
                                         new JProperty("connection",  connection?.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            LocalController.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime,
                                                                             cancellationToken) =>

                EventLog.SubmitEvent(
                    nameof(LocalController.OCPP.IN.OnBinaryDataTransferResponseReceived),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToISO8601()),
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


            LocalController.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime,
                                                                          sentMessageResult,
                                                                          ct) =>

                EventLog.SubmitEvent(
                    nameof(LocalController.OCPP.OUT.OnBinaryDataTransferResponseSent),
                    JSONObject.Create(
                              new JProperty("timestamp",   timestamp. ToISO8601()),
                              new JProperty("sender",      sender.Id),
                              new JProperty("connection",  connection?.ToJSON()),
                        request is not null
                            ? new JProperty("request",     request.   ToBinary().ToBase64())
                            : null,
                              new JProperty("response",    response.  ToBinary().ToBase64()),
                              new JProperty("runtime",     runtime.   TotalMilliseconds)
                    )
                );

            #endregion

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages


        }

        #endregion


        #region TryGetChargePoint(ChargePointId, out ChargePoint)

        public Boolean TryGetChargePoint(ChargePoint_Id ChargePointId, out ChargePoint? ChargePoint)
        {

            if (ChargePoints.TryGetValue(ChargePointId, out ChargePoint))
                return true;

            ChargePoint = null;
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
            //                                "cloud.charging.open.protocols.OCPPv1_6.WebAPI.HTTPRoot",
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

        }

        #endregion


    }

}
