/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using social.OpenData.UsersAPI;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
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
        public static Boolean ParseChargeBoxId(this HTTPRequest          HTTPRequest,
                                               OCPPWebAPI                OCPPWebAPI,
                                               out ChargeBox_Id?         ChargeBoxId,
                                               out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  == null)
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
        public static Boolean ParseChargeBox(this HTTPRequest          HTTPRequest,
                                             OCPPWebAPI                OCPPWebAPI,
                                             out ChargeBox_Id?         ChargeBoxId,
                                             out ChargeBox             ChargeBox,
                                             out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  == null)
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

            if (!OCPPWebAPI.CentralSystem.TryGetChargeBox(ChargeBoxId.Value, out ChargeBox)) {

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
        public new const           String                           DefaultHTTPServerName       = "GraphDefined OCPP v1.6 WebAPI";

        /// <summary>
        /// The default HTTP URI prefix.
        /// </summary>
        public new static readonly HTTPPath                         DefaultURLPathPrefix        = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String DefaultHTTPRealm = "Open Charging Cloud OCPPPlus WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const       String                               HTTPRoot                    = "cloud.charging.open.protocols.OCPPv1_6.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPPPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPPPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id                   EventLogId                  = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        //public HTTPPath?                                    URLPathPrefix1      { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                       HTTPRealm           { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>    HTTPLogins          { get; }


        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                     EventLog            { get; }


        public TestCentralSystem CentralSystem { get; }

        /// <summary>
        /// The DNS client to use.
        /// </summary>
        public DNSClient                                    DNSClient           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPP+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPPWebAPI(TestCentralSystem                          TestCentralSystem,
                          HTTPServer                                 HTTPServer,
                          //HTTPPath?                                  URLPathPrefix1   = null,
                          HTTPPath?                                  URLPathPrefix    = null,
                          HTTPPath?                                  BasePath         = null,
                          String                                     HTTPRealm        = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>  HTTPLogins       = null,
                          String                                     HTMLTemplate     = null)

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

            this.CentralSystem   = TestCentralSystem;

            //this.URLPathPrefix1      = URLPathPrefix1;
            this.HTTPRealm           = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins          = HTTPLogins    ?? new KeyValuePair<String, String>[0];
            this.DNSClient           = HTTPServer.DNSClient;

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = HTTPServer.AddJSONEventSource(EventIdentification:      EventLogId,
                                                                     URLTemplate:              this.URLPathPrefix + "/events",
                                                                     MaxNumberOfCachedEvents:  10000,
                                                                     RetryIntervall:           TimeSpan.FromSeconds(5),
                                                                     EnableLogging:            true,
                                                                     LogfilePrefix:            LogfilePrefix);

            RegisterURITemplates();

            this.HTMLTemplate        = HTMLTemplate ?? GetResourceString("template.html");

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

        protected override String GetResourceString(String ResourceName)

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

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(OCPPWebAPI.HTTPRoot, typeof(OCPPWebAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
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

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
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
                                                                                      CentralSystem.ChargeBoxIds.Select(chargeBoxId => new JObject(new JProperty("@id", chargeBoxId.ToString())))
                                                                                  ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region ~/chargeBoxes

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
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
                                                                                      CentralSystem.ChargeBoxes.Select(chargeBox => chargeBox.ToJSON())
                                                                                  ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion


            #region ~/chargeBoxes/{chargeBoxId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
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
                                                                         out ChargeBox_Id?         ChargeBoxId,
                                                                         out ChargeBox             ChargeBox,
                                                                         out HTTPResponse.Builder  Response))
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


        }

        #endregion


    }

}
