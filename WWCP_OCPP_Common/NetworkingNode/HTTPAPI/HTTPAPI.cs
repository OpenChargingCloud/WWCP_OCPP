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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

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
        public readonly HTTPPath                   DefaultURLPathPrefix      = HTTPPath.Parse("httpapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String                     DefaultHTTPServerName     = $"Open Charging Cloud OCPP Networking Node HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm          = "Open Charging Cloud OCPP Networking Node HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPP.NetworkingNode.HTTPAPI.HTTPRoot.";


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


        protected readonly AOCPPNetworkingNode networkingNode;

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public  String?                                    HTTPRealm         { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public  IEnumerable<KeyValuePair<String, String>>  HTTPLogins        { get; }

        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public  HTTPEventSource<JObject>                   EventLog          { get; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public  Formatting                                 JSONFormatting    { get; set; }

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

            : base(HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath)

        {

            this.networkingNode      = NetworkingNode;
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
                                           EnableLogging:            !EventLoggingDisabled,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            RegisterURITemplates();
            AttachNetworkingNode(networkingNode);

            //DebugX.Log($"OCPP Networking Node HTTP API started on {HTTPBaseAPI.HTTPServer.IPPorts.AggregateWith(", ")}");

        }

        #endregion


        #region AttachNetworkingNode(NetworkingNode)

        public void AttachNetworkingNode(AOCPPNetworkingNode NetworkingNode)
        {

            #region Generic JSON Messages

            #region OnJSONMessageRequestSent

            //ChargingStation.OnJSONMessageRequestSent += (timestamp,
            //                                            webSocketServer,
            //                                            webSocketConnection,
            //                                            networkingNodeId,
            //                                            networkPath,
            //                                            eventTrackingId,
            //                                            requestTimestamp,
            //                                            requestMessage,
            //                                            cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            #endregion

            #region OnJSONMessageRequestReceived

            //ChargingStation.OnJSONMessageRequestReceived += (timestamp,
            //                                                webSocketServer,
            //                                                webSocketConnection,
            //                                                networkingNodeId,
            //                                                networkPath,
            //                                                eventTrackingId,
            //                                                requestTimestamp,
            //                                                requestMessage,
            //                                                cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)
            //                         ));

            #endregion


            #region OnJSONMessageResponseSent

            //ChargingStation.OnJSONMessageResponseSent += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONMessageResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion

            #region OnJSONErrorResponseSent

            //ChargingStation.OnJSONErrorResponseSent += (timestamp,
            //                                           webSocketServer,
            //                                           webSocketConnection,
            //                                           eventTrackingId,
            //                                           requestTimestamp,
            //                                           jsonRequestMessage,
            //                                           binaryRequestMessage,
            //                                           responseTimestamp,
            //                                           responseMessage,
            //                                           cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONErrorResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion



            #region OnJSONMessageResponseReceived

            //ChargingStation.OnJSONMessageResponseReceived += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONMessageResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion

            #region OnJSONErrorResponseReceived

            //ChargingStation.OnJSONErrorResponseReceived += (timestamp,
            //                                               webSocketServer,
            //                                               webSocketConnection,
            //                                               eventTrackingId,
            //                                               requestTimestamp,
            //                                               jsonRequestMessage,
            //                                               binaryRequestMessage,
            //                                               responseTimestamp,
            //                                               responseMessage,
            //                                               cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnJSONErrorResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)
            //                         ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            //ChargingStation.OnBinaryMessageRequestReceived += (timestamp,
            //                                                  webSocketServer,
            //                                                  webSocketConnection,
            //                                                  networkingNodeId,
            //                                                  networkPath,
            //                                                  eventTrackingId,
            //                                                  requestTimestamp,
            //                                                  requestMessage,
            //                                                  cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnBinaryMessageRequestReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            #endregion

            #region OnBinaryMessageResponseSent

            //ChargingStation.OnBinaryMessageResponseSent += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnBinaryMessageResponseSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

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

            //ChargingStation.OnBinaryMessageRequestSent += (timestamp,
            //                                                   webSocketServer,
            //                                                   webSocketConnection,
            //                                                   networkingNodeId,
            //                                                   networkPath,
            //                                                   eventTrackingId,
            //                                                   requestTimestamp,
            //                                                   requestMessage,
            //                                                   cancellationToken) =>

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnBinaryMessageRequestSent),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      requestMessage)  // BASE64 encoded string!
            //                         ));

            #endregion

            #region OnBinaryMessageResponseReceived

            //ChargingStation.OnBinaryMessageResponseReceived += (timestamp,
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

            //    EventLog.SubmitEvent(nameof(ChargingStation.OnBinaryMessageResponseReceived),
            //                         JSONObject.Create(
            //                             new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                             new JProperty("connection",   webSocketConnection.ToJSON()),
            //                             new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                         ));

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
            //                                "cloud.charging.open.protocols.OCPP.WebAPI.HTTPRoot",
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
                                         new JProperty("time",   Timestamp.Now.    ToIso8601())
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

            #region ~/webSocketClients

            // ---------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:5010/webSocketClients?pretty\&withMetadata
            // curl -v -H "Accept: application/json" http://127.0.0.1:6000/webSocketClients?pretty\&withMetadata
            // curl -v -H "Accept: application/json" http://127.0.0.1:7000/webSocketClients?pretty\&withMetadata
            // ---------------------------------------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(
                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "webSocketClients",
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

                    var skip            = request.QueryString.GetUInt64 ("skip");
                    var take            = request.QueryString.GetUInt64 ("take");
                    var withMetadata    = request.QueryString.GetBoolean("withMetadata", false);

                    var pretty          = request.QueryString.GetBoolean("pretty");
                    var jsonFormatting  = pretty.HasValue
                                              ? pretty.Value
                                                    ? Formatting.Indented
                                                    : Formatting.None
                                              : JSONFormatting;

                    #endregion


                    var ocppWebSocketServers  = networkingNode.WWCPWebSocketClients.
                                                    Select(ocppWebSocketClient => JSONObject.Create(

                                                                                      new JProperty("description",                  ocppWebSocketClient.Description.     ToJSON()),
                                                                                      new JProperty("remoteURL",                    ocppWebSocketClient.RemoteURL.       ToString()),
                                                                                      new JProperty("virtualHostname",              ocppWebSocketClient.VirtualHostname?.ToString()),
                                                                                      new JProperty("remoteIPAddress",              ocppWebSocketClient.RemoteIPAddress?.ToString()),
                                                                                      new JProperty("connected",                    ocppWebSocketClient.Connected),
                                                                                      new JProperty("httpUserAgent",                ocppWebSocketClient.HTTPUserAgent),
                                                                                      new JProperty("requestTimeout",               ocppWebSocketClient.RequestTimeout.TotalSeconds),
                                                                                      //new JProperty("requestTimeout",               ocppWebSocketClient.RequestTimeout.TotalSeconds),
                                                                                      new JProperty("networkingMode",               ocppWebSocketClient.NetworkingMode.ToString()),

                                                                                      new JProperty("secWebSocketProtocols",        new JArray(ocppWebSocketClient.SecWebSocketProtocols)),
                                                                                      new JProperty("disableWebSocketPings",        ocppWebSocketClient.DisableWebSocketPings),
                                                                                      new JProperty("webSocketPingEvery",           ocppWebSocketClient.WebSocketPingEvery.TotalSeconds),
                                                                                      new JProperty("slowNetworkSimulationDelay",   ocppWebSocketClient.SlowNetworkSimulationDelay?.TotalMilliseconds)
                                                                                  //    new JProperty("maxTextMessageSize",           ocppWebSocketClient.MaxTextMessageSize),
                                                                                  //    new JProperty("maxBinaryMessageSize",         ocppWebSocketClient.MaxBinaryMessageSize)

                                                                                  ));

                    var jsonResults           = ocppWebSocketServers.SkipTakeFilter(skip, take);
                    var filteredCount         = jsonResults.         ULongCount();
                    var totalCount            = ocppWebSocketServers.ULongCount();


                    return Task.FromResult(
                               new HTTPResponse.Builder(request) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Server                     = HTTPServiceName,
                                   Date                       = Timestamp.Now,
                                   AccessControlAllowOrigin   = "*",
                                   AccessControlAllowMethods  = [ "GET" ],
                                   AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                                   Content                    = withMetadata
                                                                    ? JSONObject.Create(
                                                                          new JProperty("totalCount",     totalCount),
                                                                          new JProperty("filteredCount",  filteredCount),
                                                                          new JProperty("clients",        jsonResults)
                                                                      ).ToUTF8Bytes(jsonFormatting)
                                                                    : new JArray(jsonResults).ToUTF8Bytes(jsonFormatting),
                                   Connection                 = ConnectionType.Close,
                                   Vary                       = "Accept"
                               }.AsImmutable);

                },
                AllowReplacement: URLReplacement.Allow
            );

            #endregion

            #region ~/webSocketServers

            // -------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:5010/webSocketServers?pretty
            // -------------------------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(
                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "webSocketServers",
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

                    var skip            = request.QueryString.GetUInt64 ("skip");
                    var take            = request.QueryString.GetUInt64 ("take");
                    var withMetadata    = request.QueryString.GetBoolean("withMetadata", false);

                    var pretty          = request.QueryString.GetBoolean("pretty");
                    var jsonFormatting  = pretty.HasValue
                                              ? pretty.Value
                                                    ? Formatting.Indented
                                                    : Formatting.None
                                              : JSONFormatting;

                    #endregion


                    var ocppWebSocketServers  = networkingNode.WWCPWebSocketServers.
                                                    Select(ocppWebSocketServer => JSONObject.Create(

                                                                                      new JProperty("httpServiceName",              ocppWebSocketServer.HTTPServiceName),
                                                                                      new JProperty("ipAddress",                    ocppWebSocketServer.IPAddress.  ToString()),
                                                                                      new JProperty("tcpPort",                      ocppWebSocketServer.IPPort.     ToUInt16()),
                                                                                      new JProperty("description",                  ocppWebSocketServer.Description.ToJSON()),

                                                                                      new JProperty("requireAuthentication",        ocppWebSocketServer.RequireAuthentication),

                                                                                      new JProperty("secWebSocketProtocols",        new JArray(ocppWebSocketServer.SecWebSocketProtocols)),
                                                                                      new JProperty("disableWebSocketPings",        ocppWebSocketServer.DisableWebSocketPings),
                                                                                      new JProperty("webSocketPingEvery",           ocppWebSocketServer.WebSocketPingEvery.TotalSeconds),
                                                                                      new JProperty("slowNetworkSimulationDelay",   ocppWebSocketServer.SlowNetworkSimulationDelay?.TotalMilliseconds),
                                                                                      new JProperty("maxTextMessageSize",           ocppWebSocketServer.MaxTextMessageSize),
                                                                                      new JProperty("maxBinaryMessageSize",         ocppWebSocketServer.MaxBinaryMessageSize)

                                                                                  ));

                    var jsonResults           = ocppWebSocketServers.SkipTakeFilter(skip, take);
                    var filteredCount         = jsonResults.         ULongCount();
                    var totalCount            = ocppWebSocketServers.ULongCount();


                    return Task.FromResult(
                               new HTTPResponse.Builder(request) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Server                     = HTTPServiceName,
                                   Date                       = Timestamp.Now,
                                   AccessControlAllowOrigin   = "*",
                                   AccessControlAllowMethods  = [ "GET" ],
                                   AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                                   Content                    = withMetadata
                                                                    ? JSONObject.Create(
                                                                          new JProperty("totalCount",     totalCount),
                                                                          new JProperty("filteredCount",  filteredCount),
                                                                          new JProperty("servers",        jsonResults)
                                                                      ).ToUTF8Bytes(jsonFormatting)
                                                                    : new JArray(jsonResults).ToUTF8Bytes(jsonFormatting),
                                   Connection                 = ConnectionType.Close,
                                   Vary                       = "Accept"
                               }.AsImmutable);

                },
                AllowReplacement: URLReplacement.Allow
            );

            #endregion

            #region ~/ocppAdapter

            //// --------------------------------------------------------------------------------
            //// curl -v -H "Accept: application/json" http://127.0.0.1:5010/ocppAdapter?pretty
            //// curl -v -H "Accept: application/json" http://127.0.0.1:6000/ocppAdapter?pretty
            //// curl -v -H "Accept: application/json" http://127.0.0.1:7000/ocppAdapter?pretty
            //// --------------------------------------------------------------------------------
            //HTTPBaseAPI.AddMethodCallback(
            //    HTTPHostname.Any,
            //    HTTPMethod.GET,
            //    URLPathPrefix + "ocppAdapter",
            //    HTTPContentType.Application.JSON_UTF8,
            //    HTTPDelegate: request => {

            //        #region Get HTTP user and its organizations

            //        //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //        //if (!TryGetHTTPUser(Request,
            //        //                    out User                   HTTPUser,
            //        //                    out HashSet<Organization>  HTTPOrganizations,
            //        //                    out HTTPResponse.Builder   Response,
            //        //                    Recursive:                 true))
            //        //{
            //        //    return Task.FromResult(Response.AsImmutable);
            //        //}

            //        #endregion

            //        #region Check HTTP Query parameters

            //        var pretty          = request.QueryString.GetBoolean("pretty");
            //        var jsonFormatting  = pretty.HasValue
            //                                  ? pretty.Value
            //                                        ? Formatting.Indented
            //                                        : Formatting.None
            //                                  : JSONFormatting;

            //        #endregion


            //        return Task.FromResult(
            //                   new HTTPResponse.Builder(request) {
            //                       HTTPStatusCode             = HTTPStatusCode.OK,
            //                       Server                     = HTTPServiceName,
            //                       Date                       = Timestamp.Now,
            //                       AccessControlAllowOrigin   = "*",
            //                       AccessControlAllowMethods  = [ "GET" ],
            //                       AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
            //                       ContentType                = HTTPContentType.Application.JSON_UTF8,
            //                       Content                    = networkingNode.OCPP.ToJSON().ToUTF8Bytes(jsonFormatting),
            //                       Connection                 = ConnectionType.Close,
            //                       Vary                       = "Accept"
            //                   }.AsImmutable);

            //    }
            //);

            #endregion



            #region ~/events

            //#region HTML

            //// --------------------------------------------------------------------
            //// curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
            //// --------------------------------------------------------------------
            //HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
            //                          HTTPMethod.GET,
            //                          URLPathPrefix + "events",
            //                          HTTPContentType.Text.HTML_UTF8,
            //                          HTTPDelegate: Request => {

            //                              #region Get HTTP user and its organizations

            //                              //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //                              //if (!TryGetHTTPUser(Request,
            //                              //                    out User                   HTTPUser,
            //                              //                    out HashSet<Organization>  HTTPOrganizations,
            //                              //                    out HTTPResponse.Builder   Response,
            //                              //                    Recursive:                 true))
            //                              //{
            //                              //    return Task.FromResult(Response.AsImmutable);
            //                              //}

            //                              #endregion

            //                              return Task.FromResult(
            //                                         new HTTPResponse.Builder(Request) {
            //                                             HTTPStatusCode             = HTTPStatusCode.OK,
            //                                             Server                     = HTTPServiceName,
            //                                             Date                       = Timestamp.Now,
            //                                             AccessControlAllowOrigin   = "*",
            //                                             AccessControlAllowMethods  = [ "GET" ],
            //                                             AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
            //                                             ContentType                = HTTPContentType.Text.HTML_UTF8,
            //                                             Content                    = MixWithHTMLTemplate("events.events.shtml").ToUTF8Bytes(),
            //                                             Connection                 = ConnectionType.Close,
            //                                             Vary                       = "Accept"
            //                                         }.AsImmutable);

            //                          });

            //#endregion

            #endregion


        }

        #endregion


    }

}
