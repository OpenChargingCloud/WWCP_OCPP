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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The OCPP Charging Station Management System WebAPI.
    /// </summary>
    public class WebAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public new readonly HTTPPath  DefaultURLPathPrefix   = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const    String    DefaultHTTPServerName  = $"Open Charging Cloud OCPP {Version.String} CSMS WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const    String    HTTPRoot               = "cloud.charging.open.protocols.OCPPv2_1.CSMS.HTTPAPI.WebAPI.HTTPRoot.";

        #endregion

        #region Properties


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="CSMS">A Charging Station Management System.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public WebAPI(ACSMS                                       CSMS,
                      HTTPExtAPI                                  HTTPAPI,
                      String?                                     HTTPServerName   = null,
                      HTTPPath?                                   URLPathPrefix    = null,
                      HTTPPath?                                   BasePath         = null,
                      String                                      HTTPRealm        = DefaultHTTPRealm,
                      IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                      String?                                     HTMLTemplate     = null)

            : base(CSMS,
                   HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;


            this.HTMLTemplate = HTMLTemplate ?? GetResourceString("template.html");

            RegisterURITemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => MixWithHTMLTemplate(Template,
                                   ResourceName,
                                   [
                                       new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
                                   ],
                                   Content);

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            HTTPBaseAPI.HTTPServer.AddAuth  (request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(URLPathPrefix)                      ||
                    request.Path.StartsWith(URLPathPrefix + "/js")              ||
                    request.Path.StartsWith(URLPathPrefix + "/events")          ||
                    request.Path.StartsWith(URLPathPrefix + "/chargingStation") ||
                    request.Path.StartsWith(URLPathPrefix + "/chargingStations"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            #region /shared/UsersAPI

            //HTTPBaseAPI.RegisterResourcesFolder(this,
            //                                    HTTPHostname.Any,
            //                                    URLPathPrefix + "shared/UsersAPI",
            //                                    HTTPRoot.Substring(0, HTTPRoot.Length - 1),
            //                                    typeof(UsersAPI).Assembly);

            #endregion


            #region / (HTTPRoot)

            HTTPBaseAPI.MapResourceAssemblyFolder(
                HTTPHostname.Any,
                URLPathPrefix,
                HTTPRoot, //"default",
                DefaultFilename:       "index.html",
                RequireAuthentication:  false
            );

            #endregion


            #region ~/chargingStationIds

            //HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
            //                              HTTPMethod.GET,
            //                              URLPathPrefix + "chargingStationIds",
            //                              HTTPContentType.Application.JSON_UTF8,
            //                              HTTPDelegate: Request => {

            //                                  return Task.FromResult(
            //                                      new HTTPResponse.Builder(Request) {
            //                                          HTTPStatusCode             = HTTPStatusCode.OK,
            //                                          Server                     = HTTPServiceName,
            //                                          Date                       = Timestamp.Now,
            //                                          AccessControlAllowOrigin   = "*",
            //                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
            //                                          AccessControlAllowHeaders  = [ "Authorization" ],
            //                                          ContentType                = HTTPContentType.Application.JSON_UTF8,
            //                                          Content                    = JSONArray.Create(
            //                                                                           chargingStations.Keys.Select(chargingStationId => new JObject(new JProperty("@id", chargingStationId.ToString())))
            //                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
            //                                          Connection                 = "close"
            //                                      }.AsImmutable);

            //                              });

            #endregion

            #region ~/chargingStations

            //HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
            //                              HTTPMethod.GET,
            //                              URLPathPrefix + "chargingStations",
            //                              HTTPContentType.Application.JSON_UTF8,
            //                              HTTPDelegate: Request => {

            //                                  return Task.FromResult(
            //                                      new HTTPResponse.Builder(Request) {
            //                                          HTTPStatusCode             = HTTPStatusCode.OK,
            //                                          Server                     = HTTPServiceName,
            //                                          Date                       = Timestamp.Now,
            //                                          AccessControlAllowOrigin   = "*",
            //                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
            //                                          AccessControlAllowHeaders  = [ "Authorization" ],
            //                                          ContentType                = HTTPContentType.Application.JSON_UTF8,
            //                                          Content                    = JSONArray.Create(
            //                                                                           chargingStations.Values.Select(chargingStation => chargingStation.ToJSON())
            //                                                                       ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
            //                                          Connection                 = "close"
            //                                      }.AsImmutable);

            //                              });

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
                                                         Content                    = MixWithHTMLTemplate("events.index.shtml").ToUTF8Bytes(),
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
