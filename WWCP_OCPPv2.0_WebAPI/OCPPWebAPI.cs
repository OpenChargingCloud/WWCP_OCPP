/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using social.OpenData.UsersAPI;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// OCPP WebAPI extention methods.
    /// </summary>
    public static class ExtentionMethods
    {

        #region ParseChargeBoxId(this HTTPRequest, OCPPWebAPI, out ChargeBoxId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charge box identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseChargeBoxId(this HTTPRequest           HTTPRequest,
                                               OCPPWebAPI                 OCPPWebAPI,
                                               out ChargeBox_Id?          ChargeBoxId,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            ChargeBoxId   = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargeBoxId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargeBox  (this HTTPRequest, OCPPWebAPI, out ChargeBoxId, out ChargeBox, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charge box identification.</param>
        /// <param name="ChargeBox">The resolved charge box.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseChargeBox(this HTTPRequest           HTTPRequest,
                                             OCPPWebAPI                 OCPPWebAPI,
                                             out ChargeBox_Id?          ChargeBoxId,
                                             out ChargeBox?             ChargeBox,
                                             out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            ChargeBoxId   = null;
            ChargeBox     = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargeBoxId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!OCPPWebAPI.CSMS.TryGetChargeBox(ChargeBoxId.Value, out ChargeBox)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPPWebAPI.DefaultHTTPServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
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


        public TestCSMS                          CSMS       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPP+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPPWebAPI(TestCSMS                           TestCSMS,
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
                   false)// Autostart

        {

            this.CSMS       = TestCSMS;

            this.HTTPRealm           = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins          = HTTPLogins   ?? Array.Empty<KeyValuePair<string, string>>();
            this.HTMLTemplate        = HTMLTemplate ?? GetResourceString("template.html");

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = this.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryIntervall:           TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            RegisterURITemplates();

            #region HTTP-SSEs: ChargePoint   -> CSMS

            #region OnBootNotificationRequest/-Response

            this.CSMS.OnBootNotificationRequest += async (logTimestamp,
                                                                   sender,
                                                                   request) =>

                await this.EventLog.SubmitEvent("OnBootNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnBootNotificationResponse += async (logTimestamp,
                                                                    sender,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                await this.EventLog.SubmitEvent("OnBootNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnHeartbeatRequest/-Response

            this.CSMS.OnHeartbeatRequest += async (logTimestamp,
                                                            sender,
                                                            request) =>

                await this.EventLog.SubmitEvent("OnHeartbeatRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));

            this.CSMS.OnHeartbeatResponse += async (logTimestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                await this.EventLog.SubmitEvent("OnHeartbeatResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnAuthorizeRequest/-Response

            this.CSMS.OnAuthorizeRequest += async (logTimestamp,
                                                            sender,
                                                            request) =>

                await this.EventLog.SubmitEvent("OnAuthorizeRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnAuthorizeResponse += async (logTimestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                await this.EventLog.SubmitEvent("OnAuthorizeResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnStatusNotificationRequest/-Response

            this.CSMS.OnStatusNotificationRequest += async (logTimestamp,
                                                                     sender,
                                                                     request) =>

                await this.EventLog.SubmitEvent("OnStatusNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnStatusNotificationResponse += async (logTimestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnStatusNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnMeterValuesRequest/-Response

            this.CSMS.OnMeterValuesRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnMeterValuesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnMeterValuesResponse += async (logTimestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                await this.EventLog.SubmitEvent("OnMeterValuesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnIncomingDataTransferRequest/-Response

            this.CSMS.OnIncomingDataTransferRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnIncomingDataTransferRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnIncomingDataTransferResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnIncomingDataTransferResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnFirmwareStatusNotificationRequest/-Response

            this.CSMS.OnFirmwareStatusNotificationRequest += async (logTimestamp,
                                                                             sender,
                                                                             request) =>

                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnFirmwareStatusNotificationResponse += async (logTimestamp,
                                                                              sender,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #endregion

            #region HTTP-SSEs: CSMS -> ChargePoint

            #region OnResetRequest/-Response

            this.CSMS.OnResetRequest += async (logTimestamp,
                                                        sender,
                                                        request) =>

                await this.EventLog.SubmitEvent("OnResetRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnResetResponse += async (logTimestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                await this.EventLog.SubmitEvent("OnResetResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnChangeAvailabilityRequest/-Response

            this.CSMS.OnChangeAvailabilityRequest += async (logTimestamp,
                                                                     sender,
                                                                     request) =>

                await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnChangeAvailabilityResponse += async (logTimestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnChangeAvailabilityResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnDataTransferRequest/-Response

            this.CSMS.OnDataTransferRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnDataTransferRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnDataTransferResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnDataTransferResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnTriggerMessageRequest/-Response

            this.CSMS.OnTriggerMessageRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnTriggerMessageResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnTriggerMessageResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUpdateFirmwareRequest/-Response

            this.CSMS.OnUpdateFirmwareRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnUpdateFirmwareResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnUpdateFirmwareResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnReserveNowRequest/-Response

            this.CSMS.OnReserveNowRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnReserveNowRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnReserveNowResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnReserveNowResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCancelReservationRequest/-Response

            this.CSMS.OnCancelReservationRequest += async (logTimestamp,
                                                                    sender,
                                                                    request) =>

                await this.EventLog.SubmitEvent("OnCancelReservationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnCancelReservationResponse += async (logTimestamp,
                                                                     sender,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                await this.EventLog.SubmitEvent("OnCancelReservationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetChargingProfileRequest/-Response

            this.CSMS.OnSetChargingProfileRequest += async (logTimestamp,
                                                                     sender,
                                                                     request) =>

                await this.EventLog.SubmitEvent("OnSetChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnSetChargingProfileResponse += async (logTimestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnSetChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearChargingProfileRequest/-Response

            this.CSMS.OnClearChargingProfileRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnClearChargingProfileResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnClearChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetCompositeScheduleRequest/-Response

            this.CSMS.OnGetCompositeScheduleRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnGetCompositeScheduleResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUnlockConnectorRequest/-Response

            this.CSMS.OnUnlockConnectorRequest += async (logTimestamp,
                                                                  sender,
                                                                  request) =>

                await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnUnlockConnectorResponse += async (logTimestamp,
                                                                   sender,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                await this.EventLog.SubmitEvent("OnUnlockConnectorResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnGetLocalListVersionRequest/-Response

            this.CSMS.OnGetLocalListVersionRequest += async (logTimestamp,
                                                                      sender,
                                                                      request) =>

                await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnGetLocalListVersionResponse += async (logTimestamp,
                                                                       sender,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                await this.EventLog.SubmitEvent("OnGetLocalListVersionResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSendLocalListRequest/-Response

            this.CSMS.OnSendLocalListRequest += async (logTimestamp,
                                                                sender,
                                                                request) =>

                await this.EventLog.SubmitEvent("OnSendLocalListRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnSendLocalListResponse += async (logTimestamp,
                                                                 sender,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                await this.EventLog.SubmitEvent("OnSendLocalListResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearCacheRequest/-Response

            this.CSMS.OnClearCacheRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnClearCacheRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            this.CSMS.OnClearCacheResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnClearCacheResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #endregion

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String? GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String? MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            HTTPServer.RegisterResourcesFolder(this,
                                               HTTPHostname.Any,
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
            //                                         AccessControlAllowMethods  = "OPTIONS, GET",
            //                                         AccessControlAllowHeaders  = "Authorization",
            //                                         ContentType                = HTTPContentType.HTML_UTF8,
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


            #region ~/chargeBoxIds

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "chargeBoxIds",
                              HTTPContentType.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = DefaultHTTPServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = "OPTIONS, GET",
                                          AccessControlAllowHeaders  = "Authorization",
                                          ContentType                = HTTPContentType.JSON_UTF8,
                                          Content                    = JSONArray.Create(
                                                                           CSMS.ChargeBoxIds.Select(chargeBoxId => new JObject(new JProperty("@id", chargeBoxId.ToString())))
                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #region ~/chargeBoxes

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "chargeBoxes",
                              HTTPContentType.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = DefaultHTTPServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = "OPTIONS, GET",
                                          AccessControlAllowHeaders  = "Authorization",
                                          ContentType                = HTTPContentType.JSON_UTF8,
                                          Content                    = JSONArray.Create(
                                                                           CSMS.ChargeBoxes.Select(chargeBox => chargeBox.ToJSON())
                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = "close"
                                      }.AsImmutable);

                              });

            #endregion


            #region ~/chargeBoxes/{chargeBoxId}

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "chargeBoxes/{chargeBoxId}",
                              HTTPContentType.JSON_UTF8,
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

                                  #region Check ChargeBoxId URL parameter

                                  if (!Request.ParseChargeBox(this,
                                                              out ChargeBox_Id?          ChargeBoxId,
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
                                          AccessControlAllowMethods  = "OPTIONS, GET",
                                          AccessControlAllowHeaders  = "Authorization",
                                          ContentType                = HTTPContentType.JSON_UTF8,
                                          Content                    = ChargeBox.ToJSON().ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                          Connection                 = "close"
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
                              HTTPContentType.HTML_UTF8,
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
                                                 AccessControlAllowMethods  = "GET",
                                                 AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                 ContentType                = HTTPContentType.HTML_UTF8,
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
