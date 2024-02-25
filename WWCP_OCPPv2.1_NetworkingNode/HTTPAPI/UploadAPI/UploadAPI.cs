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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// The OCPP Upload API for charging station logs, diagnostics, ...
    /// </summary>
    public class UploadAPI : org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String                              DefaultHTTPServerName   = $"Open Charging Cloud OCPP {Version.String} Networking Node Upload API";

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public new static readonly HTTPPath                            DefaultURLPathPrefix    = HTTPPath.Parse("upload");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public     const           String                              DefaultHTTPRealm        = "Open Charging Cloud OCPP Networking Node Upload API";

        private           readonly Dictionary<String, FileUploadAuthentication>  validFileUploadAuths    = [];

        #endregion

        #region Properties

        /// <summary>
        /// The parent networking node.
        /// </summary>
        public ANetworkingNode                            NetworkingNode    { get; }

        /// <summary>
        /// The location to store the received files within the file system.
        /// </summary>
        public String                                     FileSystemPath    { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                     HTTPRealm         { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  HTTPLogins        { get; }

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever a new file was uploaded.
        /// </summary>
        public event UploadedFileReceivedDelegate?  OnUploadedFileReceived;

        /// <summary>
        /// An event called whenever an error during file upload occured.
        /// </summary>
        public event UploadErrorDelegate?           OnUploadError;

        /// <summary>
        /// An event called whenever an exception during file upload occured.
        /// </summary>
        public event UploadExceptionDelegate?       OnUploadException;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPP UploadAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="FileSystemPath">The location to store the received files within the file system.</param>
        /// 
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public UploadAPI(ANetworkingNode                             NetworkingNode,
                         HTTPServer                                  HTTPServer,
                         String                                      FileSystemPath,

                         HTTPPath?                                   URLPathPrefix   = null,
                         HTTPPath?                                   BasePath        = null,
                         String                                      HTTPRealm       = DefaultHTTPRealm,
                         IEnumerable<KeyValuePair<String, String>>?  HTTPLogins      = null)

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

            this.NetworkingNode  = NetworkingNode;
            this.FileSystemPath  = FileSystemPath;

            this.HTTPRealm       = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins      = HTTPLogins ?? [];

            RegisterURITemplates();

        }

        #endregion


        #region GenerateNewFileUploadAuthentication(Length = 30, Timeout = null)

        /// <summary>
        /// Generate a new file upload authentication.
        /// </summary>
        /// <param name="Length">The optional requested length of the path prefix.</param>
        /// <param name="Timeout">The optional timeout of the requested file upload authentication.</param>
        public FileUploadAuthentication GenerateNewFileUploadAuthentication(UInt16     Length    = 30,
                                                                            TimeSpan?  Timeout   = null)
        {

            var auth = new FileUploadAuthentication(
                           RandomExtensions.RandomString(Length),
                           Timestamp.Now + (Timeout ?? TimeSpan.FromMinutes(15))
                       );

            validFileUploadAuths.Add(auth.PathPrefix,
                                     auth);

            return auth;

        }

        #endregion


        #region (private) SendUploadedFileInfo(Timestamp, UploadedFileInfo, ...)

        private async Task SendUploadedFileInfo(DateTime           Timestamp,
                                                UploadedFileInfos  UploadedFileInfo,
                                                CancellationToken  CancellationToken = default)
        {

            var onUploadedFileReceived = OnUploadedFileReceived;
            if (onUploadedFileReceived is not null)
            {
                try
                {

                    await Task.WhenAll(onUploadedFileReceived.GetInvocationList().
                                           OfType<UploadedFileReceivedDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp,
                                                                         UploadedFileInfo,
                                                                         CancellationToken
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(UploadAPI),
                              nameof(OnUploadedFileReceived),
                              e,
                              CancellationToken
                          );
                }
            }

        }

        #endregion


        //ToDo: Maybe the uploaded file should be send to the CSMS asap!

        #region (private) RegisterURLTemplates()

        private void RegisterURITemplates()
        {

            #region PUT   ~/*

            // curl -X PUT http://127.0.0.1:9901/diagnostics/test.log -T test.log
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.PUT,
                              URLPathPrefix + "{file}",
                              HTTPDelegate: async request => {

                                  try
                                  {

                                      #region Validate the request path and file name

                                      var filePath        = request.Path.ToString();

                                      // Avoid directory/path traversal attacks!
                                      if (filePath.Contains("../"))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "PUT" ],
                                                     Connection                 = "close"
                                                 };


                                      var fileUploadAuth  = request.ParsedURLParameters[0];

                                      if (!validFileUploadAuths.ContainsKey(fileUploadAuth))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "PUT" ],
                                                     Connection                 = "close"
                                                 };


                                      var fileName        = request.ParsedURLParameters[1];

                                      if (fileName.Contains('/'))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "PUT" ],
                                                     Connection                 = "close"
                                                 };

                                      #endregion

                                      Directory.CreateDirectory(Path.Combine(FileSystemPath, fileUploadAuth));

                                      var fileStream   = File.Create(Path.Combine(FileSystemPath, fileName));
                                      var fileContent  = request.HTTPBody;

                                      await fileStream.WriteAsync(fileContent, request.CancellationToken);

                                      var fileLength   = (UInt64) fileStream.Length;
                                      fileStream.Close();

                                      await SendUploadedFileInfo(
                                                Timestamp.Now,
                                                new UploadedFileInfos(
                                                    Timestamp.Now,
                                                    fileName,
                                                    fileLength
                                                ),
                                                request.CancellationToken
                                            );


                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.Created,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "PUT" ],
                                                 Connection                 = "close"
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      await HandleErrors(
                                                nameof(UploadAPI),
                                                $"PUT {request.Path}",
                                                e,
                                                request.CancellationToken
                                            );

                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "PUT" ],
                                                 ContentType                = HTTPContentType.Text.PLAIN,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             };

                                  }

                              });

            #endregion

            #region POST  ~/*

            // curl -X PUT http://127.0.0.1:9901/diagnostics/test.log -T test.log
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.POST,
                              URLPathPrefix + "{file}",
                              HTTPDelegate: async request => {

                                  try
                                  {

                                      #region Validate the request path and file name

                                      var filePath        = request.Path.ToString();

                                      // Avoid directory/path traversal attacks!
                                      if (filePath.Contains("../"))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "POST" ],
                                                     Connection                 = "close"
                                                 };


                                      var fileUploadAuth  = request.ParsedURLParameters[0];

                                      if (!validFileUploadAuths.ContainsKey(fileUploadAuth))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "POST" ],
                                                     Connection                 = "close"
                                                 };


                                      var fileName        = request.ParsedURLParameters[1];

                                      if (fileName.Contains('/'))
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "POST" ],
                                                     Connection                 = "close"
                                                 };

                                      #endregion

                                      Directory.CreateDirectory(Path.Combine(FileSystemPath, fileUploadAuth));

                                      var fileStream   = File.Create(Path.Combine(FileSystemPath, fileName));
                                      var fileContent  = request.HTTPBody;

                                      await fileStream.WriteAsync(fileContent, request.CancellationToken);

                                      var fileLength   = (UInt64) fileStream.Length;
                                      fileStream.Close();

                                      await SendUploadedFileInfo(
                                                Timestamp.Now,
                                                new UploadedFileInfos(
                                                    Timestamp.Now,
                                                    fileName,
                                                    fileLength
                                                ),
                                                request.CancellationToken
                                            );


                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.Created,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "POST" ],
                                                 Connection                 = "close"
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      await HandleErrors(
                                                nameof(UploadAPI),
                                                $"POST {request.Path}",
                                                e,
                                                request.CancellationToken
                                            );

                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "POST" ],
                                                 ContentType                = HTTPContentType.Text.PLAIN,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             };

                                  }

                              });

            #endregion

        }

        #endregion


        #region HandleErrors(Module, Caller, ErrorResponse, ...)

        public async Task HandleErrors(String             Module,
                                       String             Caller,
                                       String             ErrorResponse,
                                       CancellationToken  CancellationToken = default)
        {

            var onUploadException = OnUploadError;
            if (onUploadException is not null)
            {
                try
                {

                    await Task.WhenAll(onUploadException.GetInvocationList().
                                           OfType<UploadErrorDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         Module,
                                                                         Caller,
                                                                         ErrorResponse,
                                                                         CancellationToken
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(UploadAPI),
                              nameof(OnUploadException),
                              e,
                              CancellationToken
                          );
                }
            }

        }

        #endregion

        #region HandleErrors(Module, Caller, ExceptionOccured, ...)

        public async Task HandleErrors(String             Module,
                                       String             Caller,
                                       Exception          ExceptionOccured,
                                       CancellationToken  CancellationToken = default)
        {

            var onUploadException = OnUploadException;
            if (onUploadException is not null)
            {
                try
                {

                    await Task.WhenAll(onUploadException.GetInvocationList().
                                           OfType<UploadExceptionDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         Module,
                                                                         Caller,
                                                                         ExceptionOccured,
                                                                         CancellationToken
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(UploadAPI));
                }
            }

        }

        #endregion


    }

}
