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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// OCPP WebAPI extension methods.
    /// </summary>
    public static class ExtensionMethods
    {

        #region ParseNetworkingNodeId(this HTTPRequest, OCPPWebAPI, out NetworkingNodeId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="NetworkingNodeId">The parsed unique charge box identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseNetworkingNodeId(this HTTPRequest           HTTPRequest,
                                               OCPPWebAPI                 OCPPWebAPI,
                                               out ChargeBox_Id?          NetworkingNodeId,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            NetworkingNodeId   = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            NetworkingNodeId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NetworkingNodeId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargeBox  (this HTTPRequest, OCPPWebAPI, out NetworkingNodeId, out ChargeBox, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="NetworkingNodeId">The parsed unique charge box identification.</param>
        /// <param name="ChargeBox">The resolved charge box.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseChargeBox(this HTTPRequest           HTTPRequest,
                                             OCPPWebAPI                 OCPPWebAPI,
                                             out ChargeBox_Id?          NetworkingNodeId,
                                             out ChargeBox?             ChargeBox,
                                             out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            NetworkingNodeId   = null;
            ChargeBox     = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            NetworkingNodeId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NetworkingNodeId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            if (!OCPPWebAPI.CentralSystem.TryGetChargeBox(NetworkingNodeId.Value, out ChargeBox)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP WebAPI.
    /// </summary>
    public class OCPPWebAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String          DefaultHTTPServerName       = "GraphDefined OCPP v1.6 WebAPI";

        /// <summary>
        /// The default HTTP URI prefix.
        /// </summary>
        public new static readonly HTTPPath        DefaultURLPathPrefix        = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm            = "Open Charging Cloud OCPP WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const       String              HTTPRoot                    = "cloud.charging.open.protocols.OCPPv1_6.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId                  = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                     HTTPRealm           { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  HTTPLogins          { get; }


        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                   EventLog            { get; }


        public TestCentralSystemNode                          CentralSystem       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPP+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPPWebAPI(TestCentralSystemNode                           TestCentralSystem,
                          HTTPServer                                  HTTPServer,
                          HTTPPath?                                   URLPathPrefix   = null,
                          HTTPPath?                                   BasePath        = null,
                          String                                      HTTPRealm       = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>?  HTTPLogins      = null,
                          String?                                     HTMLTemplate    = null)

            : base(HTTPServer,
                   null,
                   null, // ExternalDNSName,
                   null, // HTTPServiceName,
                   BasePath,

                   URLPathPrefix ?? DefaultURLPathPrefix,
                   null, // HTMLTemplate,
                   null, // APIVersionHashes,

                   null, // DisableMaintenanceTasks,
                   null, // MaintenanceInitialDelay,
                   null, // MaintenanceEvery,

                   null, // DisableWardenTasks,
                   null, // WardenInitialDelay,
                   null, // WardenCheckEvery,

                   null, // IsDevelopment,
                   null, // DevelopmentServers,
                   null, // DisableLogging,
                   null, // LoggingPath,
                   null, // LogfileName,
                   null, // LogfileCreator,
                   false)// AutoStart

        {

            this.CentralSystem       = TestCentralSystem;

            this.HTTPRealm           = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins          = HTTPLogins   ?? Array.Empty<KeyValuePair<string, string>>();
            //this.HTMLTemplate        = HTMLTemplate ?? GetResourceString("template.html");

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = this.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryInterval:            TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            RegisterURITemplates();

            #region HTTP-SSEs: ChargePoint   -> CentralSystem

            //#region OnBootNotificationRequest/-Response

            //this.CentralSystem.OnBootNotificationRequest += async (logTimestamp,
            //                                                       sender,
            //                                                       connection,
            //                                                       request) =>

            //    await this.EventLog.SubmitEvent("OnBootNotificationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnBootNotificationResponse += async (logTimestamp,
            //                                                        sender,
            //                                                        connection,
            //                                                        request,
            //                                                        response,
            //                                                        runtime) =>

            //    await this.EventLog.SubmitEvent("OnBootNotificationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnHeartbeatRequest/-Response

            //this.CentralSystem.OnHeartbeatRequest += async (logTimestamp,
            //                                                sender,
            //                                                connection,
            //                                                request) =>

            //    await this.EventLog.SubmitEvent("OnHeartbeatRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));

            //this.CentralSystem.OnHeartbeatResponse += async (logTimestamp,
            //                                                 sender,
            //                                                 connection,
            //                                                 request,
            //                                                 response,
            //                                                 runtime) =>

            //    await this.EventLog.SubmitEvent("OnHeartbeatResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion


            //#region OnAuthorizeRequest/-Response

            //this.CentralSystem.OnAuthorizeRequest += async (logTimestamp,
            //                                                sender,
            //                                                connection,
            //                                                request) =>

            //    await this.EventLog.SubmitEvent("OnAuthorizeRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnAuthorizeResponse += async (logTimestamp,
            //                                                 sender,
            //                                                 connection,
            //                                                 request,
            //                                                 response,
            //                                                 runtime) =>

            //    await this.EventLog.SubmitEvent("OnAuthorizeResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnStartTransactionRequest/-Response

            //this.CentralSystem.OnStartTransactionRequest += async (logTimestamp,
            //                                                       sender,
            //                                                       connection,
            //                                                       request) =>

            //    await this.EventLog.SubmitEvent("OnStartTransactionRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnStartTransactionResponse += async (logTimestamp,
            //                                                        sender,
            //                                                        connection,
            //                                                        request,
            //                                                        response,
            //                                                        runtime) =>

            //    await this.EventLog.SubmitEvent("OnStartTransactionResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnStatusNotificationRequest/-Response

            //this.CentralSystem.OnStatusNotificationRequest += async (logTimestamp,
            //                                                         sender,
            //                                                         connection,
            //                                                         request) =>

            //    await this.EventLog.SubmitEvent("OnStatusNotificationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnStatusNotificationResponse += async (logTimestamp,
            //                                                          sender,
            //                                                          connection,
            //                                                          request,
            //                                                          response,
            //                                                          runtime) =>

            //    await this.EventLog.SubmitEvent("OnStatusNotificationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnMeterValuesRequest/-Response

            //this.CentralSystem.OnMeterValuesRequest += async (logTimestamp,
            //                                                  sender,
            //                                                  connection,
            //                                                  request) =>

            //    await this.EventLog.SubmitEvent("OnMeterValuesRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnMeterValuesResponse += async (logTimestamp,
            //                                                   sender,
            //                                                   connection,
            //                                                   request,
            //                                                   response,
            //                                                   runtime) =>

            //    await this.EventLog.SubmitEvent("OnMeterValuesResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnStopTransactionRequest/-Response

            //this.CentralSystem.OnStopTransactionRequest += async (logTimestamp,
            //                                                      sender,
            //                                                      connection,
            //                                                      request) =>

            //    await this.EventLog.SubmitEvent("OnStopTransactionRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnStopTransactionResponse += async (logTimestamp,
            //                                                       sender,
            //                                                       connection,
            //                                                       request,
            //                                                       response,
            //                                                       runtime) =>

            //    await this.EventLog.SubmitEvent("OnStopTransactionResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion


            //#region OnIncomingDataTransferRequest/-Response

            //this.CentralSystem.OnIncomingDataTransferRequest += async (logTimestamp,
            //                                                           sender,
            //                                                           connection,
            //                                                           request) =>

            //    await this.EventLog.SubmitEvent("OnIncomingDataTransferRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnIncomingDataTransferResponse += async (logTimestamp,
            //                                                            sender,
            //                                                            connection,
            //                                                            request,
            //                                                            response,
            //                                                            runtime) =>

            //    await this.EventLog.SubmitEvent("OnIncomingDataTransferResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnDiagnosticsStatusNotificationRequest/-Response

            //this.CentralSystem.OnDiagnosticsStatusNotificationRequest += async (logTimestamp,
            //                                                                    sender,
            //                                                                    connection,
            //                                                                    request) =>

            //    await this.EventLog.SubmitEvent("OnDiagnosticsStatusNotificationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnDiagnosticsStatusNotificationResponse += async (logTimestamp,
            //                                                                     sender,
            //                                                                     connection,
            //                                                                     request,
            //                                                                     response,
            //                                                                     runtime) =>

            //    await this.EventLog.SubmitEvent("OnDiagnosticsStatusNotificationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnFirmwareStatusNotificationRequest/-Response

            //this.CentralSystem.OnFirmwareStatusNotificationRequest += async (logTimestamp,
            //                                                                 sender,
            //                                                                 connection,
            //                                                                 request) =>

            //    await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnFirmwareStatusNotificationResponse += async (logTimestamp,
            //                                                                  sender,
            //                                                                  connection,
            //                                                                  request,
            //                                                                  response,
            //                                                                  runtime) =>

            //    await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            #endregion

            #region HTTP-SSEs: CentralSystem -> ChargePoint

            //#region OnResetRequest/-Response

            //this.CentralSystem.OnResetRequest += async (logTimestamp,
            //                                            sender,
            //                                            request) =>

            //    await this.EventLog.SubmitEvent("OnResetRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnResetResponse += async (logTimestamp,
            //                                             sender,
            //                                             request,
            //                                             response,
            //                                             runtime) =>

            //    await this.EventLog.SubmitEvent("OnResetResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnChangeAvailabilityRequest/-Response

            //this.CentralSystem.OnChangeAvailabilityRequest += async (logTimestamp,
            //                                                         sender,
            //                                                         request) =>

            //    await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnChangeAvailabilityResponse += async (logTimestamp,
            //                                                          sender,
            //                                                          request,
            //                                                          response,
            //                                                          runtime) =>

            //    await this.EventLog.SubmitEvent("OnChangeAvailabilityResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnGetConfigurationRequest/-Response

            //this.CentralSystem.OnGetConfigurationRequest += async (logTimestamp,
            //                                                       sender,
            //                                                       request) =>

            //    await this.EventLog.SubmitEvent("OnGetConfigurationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnGetConfigurationResponse += async (logTimestamp,
            //                                                        sender,
            //                                                        request,
            //                                                        response,
            //                                                        runtime) =>

            //    await this.EventLog.SubmitEvent("OnGetConfigurationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnChangeConfigurationRequest/-Response

            //this.CentralSystem.OnChangeConfigurationRequest += async (logTimestamp,
            //                                                          sender,
            //                                                          request) =>

            //    await this.EventLog.SubmitEvent("OnChangeConfigurationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnChangeConfigurationResponse += async (logTimestamp,
            //                                                           sender,
            //                                                           request,
            //                                                           response,
            //                                                           runtime) =>

            //    await this.EventLog.SubmitEvent("OnChangeConfigurationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnDataTransferRequest/-Response

            //this.CentralSystem.OnDataTransferRequest += async (logTimestamp,
            //                                                   sender,
            //                                                   request) =>

            //    await this.EventLog.SubmitEvent("OnDataTransferRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnDataTransferResponse += async (logTimestamp,
            //                                                    sender,
            //                                                    request,
            //                                                    response,
            //                                                    runtime) =>

            //    await this.EventLog.SubmitEvent("OnDataTransferResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnGetDiagnosticsRequest/-Response

            //this.CentralSystem.OnGetDiagnosticsRequest += async (logTimestamp,
            //                                                     sender,
            //                                                     request) =>

            //    await this.EventLog.SubmitEvent("OnGetDiagnosticsRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnGetDiagnosticsResponse += async (logTimestamp,
            //                                                      sender,
            //                                                      request,
            //                                                      response,
            //                                                      runtime) =>

            //    await this.EventLog.SubmitEvent("OnGetDiagnosticsResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnTriggerMessageRequest/-Response

            //this.CentralSystem.OnTriggerMessageRequest += async (logTimestamp,
            //                                                     sender,
            //                                                     request) =>

            //    await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnTriggerMessageResponse += async (logTimestamp,
            //                                                      sender,
            //                                                      request,
            //                                                      response,
            //                                                      runtime) =>

            //    await this.EventLog.SubmitEvent("OnTriggerMessageResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnUpdateFirmwareRequest/-Response

            //this.CentralSystem.OnUpdateFirmwareRequest += async (logTimestamp,
            //                                                     sender,
            //                                                     request) =>

            //    await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnUpdateFirmwareResponse += async (logTimestamp,
            //                                                      sender,
            //                                                      request,
            //                                                      response,
            //                                                      runtime) =>

            //    await this.EventLog.SubmitEvent("OnUpdateFirmwareResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion


            //#region OnReserveNowRequest/-Response

            //this.CentralSystem.OnReserveNowRequest += async (logTimestamp,
            //                                                 sender,
            //                                                 request) =>

            //    await this.EventLog.SubmitEvent("OnReserveNowRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnReserveNowResponse += async (logTimestamp,
            //                                                  sender,
            //                                                  request,
            //                                                  response,
            //                                                  runtime) =>

            //    await this.EventLog.SubmitEvent("OnReserveNowResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnCancelReservationRequest/-Response

            //this.CentralSystem.OnCancelReservationRequest += async (logTimestamp,
            //                                                        sender,
            //                                                        request) =>

            //    await this.EventLog.SubmitEvent("OnCancelReservationRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnCancelReservationResponse += async (logTimestamp,
            //                                                         sender,
            //                                                         request,
            //                                                         response,
            //                                                         runtime) =>

            //    await this.EventLog.SubmitEvent("OnCancelReservationResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnRemoteStartTransactionRequest/-Response

            //this.CentralSystem.OnRemoteStartTransactionRequest += async (logTimestamp,
            //                                                             sender,
            //                                                             request) =>

            //    await this.EventLog.SubmitEvent("OnRemoteStartTransactionRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnRemoteStartTransactionResponse += async (logTimestamp,
            //                                                              sender,
            //                                                              request,
            //                                                              response,
            //                                                              runtime) =>

            //    await this.EventLog.SubmitEvent("OnRemoteStartTransactionResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnRemoteStopTransactionRequest/-Response

            //this.CentralSystem.OnRemoteStopTransactionRequest += async (logTimestamp,
            //                                                            sender,
            //                                                            request) =>

            //    await this.EventLog.SubmitEvent("OnRemoteStopTransactionRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnRemoteStopTransactionResponse += async (logTimestamp,
            //                                                             sender,
            //                                                             request,
            //                                                             response,
            //                                                             runtime) =>

            //    await this.EventLog.SubmitEvent("OnRemoteStopTransactionResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnSetChargingProfileRequest/-Response

            //this.CentralSystem.OnSetChargingProfileRequest += async (logTimestamp,
            //                                                         sender,
            //                                                         request) =>

            //    await this.EventLog.SubmitEvent("OnSetChargingProfileRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnSetChargingProfileResponse += async (logTimestamp,
            //                                                          sender,
            //                                                          request,
            //                                                          response,
            //                                                          runtime) =>

            //    await this.EventLog.SubmitEvent("OnSetChargingProfileResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnClearChargingProfileRequest/-Response

            //this.CentralSystem.OnClearChargingProfileRequest += async (logTimestamp,
            //                                                           sender,
            //                                                           request) =>

            //    await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnClearChargingProfileResponse += async (logTimestamp,
            //                                                            sender,
            //                                                            request,
            //                                                            response,
            //                                                            runtime) =>

            //    await this.EventLog.SubmitEvent("OnClearChargingProfileResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnGetCompositeScheduleRequest/-Response

            //this.CentralSystem.OnGetCompositeScheduleRequest += async (logTimestamp,
            //                                                           sender,
            //                                                           request) =>

            //    await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnGetCompositeScheduleResponse += async (logTimestamp,
            //                                                            sender,
            //                                                            request,
            //                                                            response,
            //                                                            runtime) =>

            //    await this.EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnUnlockConnectorRequest/-Response

            //this.CentralSystem.OnUnlockConnectorRequest += async (logTimestamp,
            //                                                      sender,
            //                                                      request) =>

            //    await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnUnlockConnectorResponse += async (logTimestamp,
            //                                                       sender,
            //                                                       request,
            //                                                       response,
            //                                                       runtime) =>

            //    await this.EventLog.SubmitEvent("OnUnlockConnectorResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion


            //#region OnGetLocalListVersionRequest/-Response

            //this.CentralSystem.OnGetLocalListVersionRequest += async (logTimestamp,
            //                                                          sender,
            //                                                          request) =>

            //    await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnGetLocalListVersionResponse += async (logTimestamp,
            //                                                           sender,
            //                                                           request,
            //                                                           response,
            //                                                           runtime) =>

            //    await this.EventLog.SubmitEvent("OnGetLocalListVersionResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnSendLocalListRequest/-Response

            //this.CentralSystem.OnSendLocalListRequest += async (logTimestamp,
            //                                                    sender,
            //                                                    request) =>

            //    await this.EventLog.SubmitEvent("OnSendLocalListRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnSendLocalListResponse += async (logTimestamp,
            //                                                     sender,
            //                                                     request,
            //                                                     response,
            //                                                     runtime) =>

            //    await this.EventLog.SubmitEvent("OnSendLocalListResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            //#region OnClearCacheRequest/-Response

            //this.CentralSystem.OnClearCacheRequest += async (logTimestamp,
            //                                                 sender,
            //                                                 request) =>

            //    await this.EventLog.SubmitEvent("OnClearCacheRequest",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
            //                                    ));


            //this.CentralSystem.OnClearCacheResponse += async (logTimestamp,
            //                                                  sender,
            //                                                  request,
            //                                                  response,
            //                                                  runtime) =>

            //    await this.EventLog.SubmitEvent("OnClearCacheResponse",
            //                                    new JObject(
            //                                        new JProperty("timestamp",        logTimestamp.           ToIso8601()),
            //                                        new JProperty("networkingNodeId",   request.DestinationId.    ToString()),
            //                                        new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
            //                                        new JProperty("request",          request.                ToJSON()),
            //                                        new JProperty("response",         response.               ToJSON()),
            //                                        new JProperty("runtime",          runtime.TotalMilliseconds)
            //                                    ));

            //#endregion

            #endregion

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                new Tuple<String, System.Reflection.Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            this.MapResourceAssemblyFolder(HTTPHostname.Any,
                                           URLPathPrefix,
                                           "cloud.charging.open.protocols.OCPPv1_6.WebAPI.HTTPRoot",
                                           DefaultFilename: "index.html");

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


            #region ~/networkingNodeIds

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "networkingNodeIds",
                              HTTPContentType.Application.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = DefaultHTTPServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ],
                                          ContentType                = HTTPContentType.Application.JSON_UTF8,
                                          Content                    = JSONArray.Create(
                                                                        //   CentralSystem.ConnectedNetworkingNodeIds.Select(networkingNodeId => new JObject(new JProperty("@id", networkingNodeId.ToString())))
                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = ConnectionType.Close
                                      }.AsImmutable);

                              });

            #endregion

            #region ~/chargeBoxes

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "chargeBoxes",
                              HTTPContentType.Application.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = DefaultHTTPServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ],
                                          ContentType                = HTTPContentType.Application.JSON_UTF8,
                                          Content                    = JSONArray.Create(
                                                                           CentralSystem.ChargeBoxes.Select(chargeBox => chargeBox.ToJSON())
                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = ConnectionType.Close
                                      }.AsImmutable);

                              });

            #endregion


            #region ~/chargeBoxes/{networkingNodeId}

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "chargeBoxes/{networkingNodeId}",
                              HTTPContentType.Application.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  //if (!TryGetHTTPUser(Request,
                                  //                    out User                   HTTPUser,
                                  //                    out HashSet<Organization>  HTTPOrganizations,
                                  //                    out HTTPResponse.Builder   Response,
                                  //                    AccessLevel:               Access_Levels.ReadOnly,
                                  //                    Recursive:                 true))
                                  //{
                                  //    return Task.FromResult(Response.AsImmutable);
                                  //}

                                  #endregion

                                  #region Check NetworkingNodeId URL parameter

                                  if (!Request.ParseChargeBox(this,
                                                              out ChargeBox_Id?          NetworkingNodeId,
                                                              out ChargeBox?             ChargeBox,
                                                              out HTTPResponse.Builder?  Response))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = DefaultHTTPServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ],
                                          ContentType                = HTTPContentType.Application.JSON_UTF8,
                                          Content                    = ChargeBox.ToJSON().ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = ConnectionType.Close
                                      }.AsImmutable);

                              });

            #endregion


            #region ~/events

            #region HTML

            // --------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
            // --------------------------------------------------------------------
            AddMethodCallback(Hostname,
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
                                                 Server                     = DefaultHTTPServerName,
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
