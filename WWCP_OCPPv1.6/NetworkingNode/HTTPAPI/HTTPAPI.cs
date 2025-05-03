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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    /// <summary>
    /// The OCPP v1.6 Networking Node HTTP API.
    /// </summary>
    public class HTTPAPI : OCPP.NetworkingNode.HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const String  DefaultHTTPServerName  = $"Open Charging Cloud OCPP {Version.String} Networking Node HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public new const String  DefaultHTTPRealm       = $"Open Charging Cloud OCPP {Version.String} Networking Node HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const String  HTTPRoot               = "cloud.charging.open.protocols.OCPPv1_6.NetworkingNode.HTTPAPI.HTTPRoot.";

        #endregion

        #region Properties

        public AOCPPNetworkingNode  NetworkingNode    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="NetworkingNode">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(AOCPPNetworkingNode                         NetworkingNode,
                       HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName         = null,
                       HTTPPath?                                   URLPathPrefix          = null,
                       HTTPPath?                                   BasePath               = null,

                       Boolean                                     EventLoggingDisabled   = true,

                       String                                      HTTPRealm              = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins             = null,
                       Formatting                                  JSONFormatting         = Formatting.None)

            : base(NetworkingNode,
                   HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,

                   EventLoggingDisabled,

                   HTTPRealm,
                   HTTPLogins,
                   JSONFormatting)

        {

            this.NetworkingNode = NetworkingNode;

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURITemplates();
            AttachNetworkingNode(networkingNode);

            DebugX.Log($"OCPP {Version.String} Networking Node HTTP API started on {HTTPBaseAPI.HTTPServer.IPPorts.AggregateWith(", ")}");

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

            HTTPBaseAPI.HTTPServer.AddAuth(request => {

                if (request.Path.ToString() == "/systemInfo"       ||
                    request.Path.ToString() == "/webSocketClients" ||
                    request.Path.ToString() == "/webSocketServers" ||
                    request.Path.ToString() == "/ocppAdapter"      ||
                    request.Path.ToString() == "/connections")
                {
                    return HTTPExtAPI.Anonymous;
                }

                return null;

            });


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


            #region ~/systemInfo

            // ------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:5010/systemInfo
            // curl -v -H "Accept: application/json" http://127.0.0.1:6000/systemInfo
            // curl -v -H "Accept: application/json" http://127.0.0.1:7000/systemInfo
            // ------------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(
                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "systemInfo",
                HTTPContentType.Application.JSON_UTF8,
                HTTPDelegate: request => {

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

                    #region Check HTTP Query parameters

                    var pretty          = request.QueryString.GetBoolean("pretty");
                    var jsonFormatting  = pretty.HasValue
                                              ? pretty.Value
                                                    ? Formatting.Indented
                                                    : Formatting.None
                                              : JSONFormatting;

                    #endregion


                    var systemInfo = JSONObject.Create(
                                         new JProperty("id",     networkingNode.Id.ToString()),
                                         new JProperty("time",   Timestamp.Now.    ToISO8601())
                                     );

                    return Task.FromResult(
                               new HTTPResponse.Builder(request) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Server                     = HTTPServiceName,
                                   Date                       = Timestamp.Now,
                                   AccessControlAllowOrigin   = "*",
                                   AccessControlAllowMethods  = [ "GET" ],
                                   AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                                   Content                    = systemInfo.ToUTF8Bytes(jsonFormatting),
                                   Connection                 = ConnectionType.Close,
                                   Vary                       = "Accept"
                               }.AsImmutable);

                },
                AllowReplacement: URLReplacement.Allow
            );

            #endregion

            #region ~/ocppAdapter

            // --------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:5010/ocppAdapter?pretty
            // curl -v -H "Accept: application/json" http://127.0.0.1:6000/ocppAdapter?pretty
            // curl -v -H "Accept: application/json" http://127.0.0.1:7000/ocppAdapter?pretty
            // --------------------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(
                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "ocppAdapter",
                HTTPContentType.Application.JSON_UTF8,
                HTTPDelegate: request => {

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

                    #region Check HTTP Query parameters

                    var pretty          = request.QueryString.GetBoolean("pretty");
                    var jsonFormatting  = pretty.HasValue
                                              ? pretty.Value
                                                    ? Formatting.Indented
                                                    : Formatting.None
                                              : JSONFormatting;

                    #endregion


                    return Task.FromResult(
                               new HTTPResponse.Builder(request) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Server                     = HTTPServiceName,
                                   Date                       = Timestamp.Now,
                                   AccessControlAllowOrigin   = "*",
                                   AccessControlAllowMethods  = [ "GET" ],
                                   AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                                   Content                    = NetworkingNode.OCPP.ToJSON().ToUTF8Bytes(jsonFormatting),
                                   Connection                 = ConnectionType.Close,
                                   Vary                       = "Accept"
                               }.AsImmutable);

                },
                AllowReplacement: URLReplacement.Allow
            );

            #endregion


        }

        #endregion


    }

}
