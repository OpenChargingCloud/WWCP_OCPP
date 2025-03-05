﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using HermodHTTP = org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LocalController
{

    /// <summary>
    /// OCPP Local Controller WebAPI extensions.
    /// </summary>
    public static class WebAPIExtensions
    {

    }


    /// <summary>
    /// The OCPP Local Controller WebAPI.
    /// </summary>
    public class WebAPI : AHTTPAPIExtension<HTTPExtAPI>,
                          IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath  DefaultURLPathPrefix   = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String    DefaultHTTPServerName  = $"Open Charging Cloud OCPP {Version.String} Local Controller WebAPI";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String    DefaultHTTPRealm       = "OCPP Local Controller WebAPI";

        /// <summary>
        /// The HTTP root for embedded resources.
        /// </summary>
        public const    String    HTTPRoot               = "cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.WebAPI.HTTPRoot.";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given local controller WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="LocalController">A Local Controller node.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPServerName">An optional HTTP server name for the WebAPI.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix for the WebAPI.</param>
        /// <param name="BasePath">An optional base path for the WebAPI.</param>
        /// <param name="HTTPRealm">An optional HTTP realm, when HTTP Basic Authentication is used with the WebAPI.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        /// <param name="HTMLTemplate"></param>
        public WebAPI(ALocalControllerNode                        LocalController,
                      HTTPExtAPI                                  HTTPAPI,
                      String?                                     HTTPServerName   = null,
                      HTTPPath?                                   URLPathPrefix    = null,
                      HTTPPath?                                   BasePath         = null,
                      String                                      HTTPRealm        = DefaultHTTPRealm,
                      IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                      String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath ?? HTTPPath.Parse("webapi"))

        {

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.HTMLTemplate = HTMLTemplate ?? GetResourceString("template.html");

            RegisterURITemplates();

            DebugX.Log($"OCPP {Version.String} Local Controller WebAPI started on {HTTPAPI.HTTPServer.IPSockets.AggregateWith(", ")}{URLPathPrefix}");

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => base.GetResourceStream(
                   ResourceName,
                   new Tuple<String, Assembly>(WebAPI.            HTTPRoot,  typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.        HTTPRoot,  typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => base.GetResourceMemoryStream(
                   ResourceName,
                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot,          typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => base.GetResourceString(
                   ResourceName,
                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot,          typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => base.GetResourceBytes(
                   ResourceName,
                   new Tuple<String, Assembly>(WebAPI.HTTPRoot,              typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => base.MixWithHTMLTemplate(
                   ResourceName,
                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot,          typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => base.MixWithHTMLTemplate(
                   ResourceName,
                   HTMLConverter,
                   new Tuple<String, Assembly>(WebAPI.HTTPRoot,              typeof(WebAPI).            Assembly),
                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                   new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => base.MixWithHTMLTemplate(
                   Template,
                   ResourceName,
                   [
                       new Tuple<String, Assembly>(WebAPI.HTTPRoot,              typeof(WebAPI).            Assembly),
                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot,          typeof(HTTPExtAPI).        Assembly),
                       new Tuple<String, Assembly>(HermodHTTP.HTTPAPI.HTTPRoot,  typeof(HermodHTTP.HTTPAPI).Assembly)
                   ],
                   Content
               );

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            HTTPBaseAPI.MapResourceAssemblyFolder(
                HTTPHostname.Any,
                URLPathPrefix,
                HTTPRoot,
                typeof(WebAPI).Assembly,
                "index.html",
                false
            );

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
            //// curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
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
