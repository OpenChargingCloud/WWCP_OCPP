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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The OCPP (diagnostics) UploadAPI.
    /// </summary>
    public class UploadAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String                           DefaultHTTPServerName       = "GraphDefined OCPP v1.6 Upload API";

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public new static readonly HTTPPath                         DefaultURLPathPrefix        = HTTPPath.Parse("diagnostics");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String DefaultHTTPRealm = "Open Charging Cloud Upload API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const       String                               HTTPRoot                    = "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPPPlusJSONContentType     = new ("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPPPlusHTMLContentType     = new ("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id                   EventLogId                  = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

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
        public HTTPEventSource<JObject>?                    EventLog            { get; }


        public TestCSMS                            CSMS       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPP UploadAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public UploadAPI(TestCSMS                           TestCSMS,
                         HTTPServer                                  HTTPServer,
                         HTTPPath?                                   URLPathPrefix    = null,
                         HTTPPath?                                   BasePath         = null,
                         String                                      HTTPRealm        = DefaultHTTPRealm,
                         IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                         String?                                     HTMLTemplate     = null)

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
                   true) // AutoStart

        {

            this.CSMS  = TestCSMS;

            this.HTTPRealm      = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins     = HTTPLogins ?? Array.Empty<KeyValuePair<string, string>>();

            RegisterURITemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURITemplates()
        {

            #region PUT  ~/*

            // curl -X PUT http://127.0.0.1:9901/diagnostics/test.log -T test.log
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.PUT,
                              URLPathPrefix + "{file}",
                              HTTPDelegate: async Request => {

                                  try
                                  {

                                      var filepath  = Request.Path.ToString().Replace("..", "");
                                      var filename  = filepath.Substring(filepath.LastIndexOf("/") + 1);
                                      var file      = File.Create(filename);
                                      var data      = Request.HTTPBody;

                                      await file.WriteAsync(data, 0, data.Length);

                                      DebugX.Log("UploadAPI: Received file '" + filename + "'!");


                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.Created,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "PUT" },
                                                 Connection                 = "close"
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      DebugX.Log("UploadAPI: Could not received file: " + e.Message);

                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "PUT" },
                                                 ContentType                = HTTPContentType.TEXT_UTF8,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             };

                                  }

                              });

            #endregion


            #region PUT  ~/*

            // curl -X PUT http://127.0.0.1:9901/diagnostics/test.log -T test.log
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.POST,
                              URLPathPrefix + "{file}",
                              HTTPDelegate: async Request => {

                                  try
                                  {

                                      var filepath  = Request.Path.ToString().Replace("..", "");
                                      var filename  = filepath.Substring(filepath.LastIndexOf("/") + 1);
                                      var file      = File.Create(filename);
                                      var data      = Request.HTTPBody;

                                      await file.WriteAsync(data, 0, data.Length);

                                      DebugX.Log("UploadAPI: Received file '" + filename + "'!");


                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.Created,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "PUT" },
                                                 Connection                 = "close"
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      DebugX.Log("UploadAPI: Could not received file: " + e.Message);

                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "PUT" },
                                                 ContentType                = HTTPContentType.TEXT_UTF8,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             };

                                  }

                              });

            #endregion


            #region GET  ~/*

            // curl -X GET http://127.0.0.1:9901/fw/test.fw
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "fw/{file}",
                              HTTPDelegate: async Request => {

                                  try
                                  {

                                      var filepath  = Request.Path.ToString().Replace("..", "");
                                      var filename  = filepath.Substring(filepath.LastIndexOf("/") + 1);
                                      var data      = File.ReadAllBytes("fw/" + filename);

                                      DebugX.Log("FirmwareAPI: Fetching file '" + filename + "'!");


                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.Created,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "GET" },
                                                 ContentType                = HTTPContentType.OCTETSTREAM,
                                                 Content                    = data,
                                                 Connection                 = "close"
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      DebugX.Log("FirmwareAPI: Could not fetch file: " + e.Message);

                                      return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = new[] { "PUT" },
                                                 ContentType                = HTTPContentType.TEXT_UTF8,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             };

                                  }

                              });

            #endregion

        }

        #endregion


    }

}
