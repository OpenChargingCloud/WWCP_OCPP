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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.LocalController
{

    /// <summary>
    /// The OCPP Download API for charging station firmware updates, ...
    /// </summary>
    public class DownloadAPI : org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const            String    DefaultHTTPServerName   = $"Open Charging Cloud OCPP {Version.String} Networking Node Download API";

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public new static readonly  HTTPPath  DefaultURLPathPrefix    = HTTPPath.Parse("downloads");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public     const            String    DefaultHTTPRealm        = "Open Charging Cloud OCPP Networking Node Download API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const            String    HTTPRoot                = "cloud.charging.open.protocols.OCPPv1_6.NetworkingNode.CSMS.HTTPAPI.DownloadAPI.HTTPRoot";

        #endregion

        #region Properties

        /// <summary>
        /// The parent networking node.
        /// </summary>
        public ALocalControllerNode                            NetworkingNode    { get; }

        /// <summary>
        /// The optional location of the served files within the file system.
        /// </summary>
        public String?                                    FileSystemPath    { get; }

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
        /// An event called whenever a file was downloaded.
        /// </summary>
        public event FileDownloadedDelegate?     OnFileDownloaded;

        /// <summary>
        /// An event called whenever an error during file download occured.
        /// </summary>
        public event DownloadErrorDelegate?      OnDownloadError;

        /// <summary>
        /// An event called whenever an exception during file download occured.
        /// </summary>
        public event DownloadExceptionDelegate?  OnDownloadException;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach an OCPP Download API to the given HTTP server.
        /// </summary>
        /// <param name="LocalController">A networking node.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// 
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="FileSystemPath">The optional location of the served files within the file system.</param>
        /// 
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public DownloadAPI(ALocalControllerNode                        LocalController,
                           HTTPServer                                  HTTPServer,

                           HTTPPath?                                   BasePath        = null,
                           HTTPPath?                                   URLPathPrefix   = null,
                           String?                                     FileSystemPath  = null,

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

            this.NetworkingNode  = LocalController;

            this.FileSystemPath  = FileSystemPath;

            this.HTTPRealm       = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins      = HTTPLogins ?? [];

            RegisterURITemplates();

            DebugX.Log($"OCPP {Version.String} LocalController DownloadAPI started on {HTTPServer.IPSockets.AggregateWith(", ")}{URLPathPrefix}");

        }

        #endregion


        #region (private) SendFileDownloaded(Timestamp, DownloadedFileInfo, ...)

        private async Task SendFileDownloaded(DateTime             Timestamp,
                                              DownloadedFileInfos  DownloadedFileInfo,
                                              CancellationToken    CancellationToken = default)
        {

            var onDownloadedFileReceived = OnFileDownloaded;
            if (onDownloadedFileReceived is not null)
            {
                try
                {

                    await Task.WhenAll(onDownloadedFileReceived.GetInvocationList().
                                           OfType<FileDownloadedDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp,
                                                                         DownloadedFileInfo,
                                                                         CancellationToken
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(DownloadAPI),
                              nameof(OnFileDownloaded),
                              e,
                              CancellationToken
                          );
                }
            }

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURITemplates()
        {

            #region GET  ~/*

            // curl http://127.0.0.1:9901/downloads/LICENSE.txt
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "{file}",
                              HTTPDelegate: async request => {

                                  try
                                  {

                                      var filePath    = request.Path.ToString();

                                      // Avoid directory/path traversal attacks!
                                      if (!filePath.StartsWith(URLPathPrefix.ToString()) ||
                                           filePath.Contains("../"))
                                      {
                                          return new HTTPResponse.Builder(request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "GET" ],
                                                     Connection                 = ConnectionType.Close
                                                 };
                                      }

                                          filePath    = filePath[URLPathPrefix.ToString().Length..];
                                      var fileName    = filePath[(filePath.LastIndexOf('/') + 1)..];
                                      var fileStream  = typeof(DownloadAPI).Assembly.GetManifestResourceStream(HTTPRoot + filePath.Replace('/', '.'));

                                      var fileContent = fileStream is not null
                                                            ? fileStream.ToByteArray()
                                                            : FileSystemPath is not null
                                                                  ? File.ReadAllBytes(Path.Combine(FileSystemPath, fileName))
                                                                  : [];

                                      await SendFileDownloaded(
                                                Timestamp.Now,
                                                new DownloadedFileInfos(
                                                    fileName,
                                                    (UInt64) fileContent.Length,
                                                    Timestamp.Now
                                                ),
                                                request.CancellationToken
                                            );

                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "GET" ],
                                                 ContentType                = HTTPContentType.ForFileExtension(
                                                                                  fileName.LastIndexOf('.') > 0
                                                                                      ? fileName[(fileName.LastIndexOf('.') + 1)..]
                                                                                      : "",
                                                                                  HTTPContentType.Application.OCTETSTREAM
                                                                              ).First(),
                                                 Content                    = fileContent,
                                                 Connection                 = ConnectionType.Close
                                             };

                                  }
                                  catch (Exception e)
                                  {

                                      await HandleErrors(
                                                nameof(UploadAPI),
                                                $"GET {request.Path}",
                                                e,
                                                request.CancellationToken
                                            );

                                      return new HTTPResponse.Builder(request) {
                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = Timestamp.Now,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = [ "GET" ],
                                                 ContentType                = HTTPContentType.Text.PLAIN,
                                                 Content                    = e.Message.ToUTF8Bytes(),
                                                 Connection                 = ConnectionType.Close
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

            var onDownloadException = OnDownloadError;
            if (onDownloadException is not null)
            {
                try
                {

                    await Task.WhenAll(onDownloadException.GetInvocationList().
                                           OfType<DownloadErrorDelegate>().
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
                              nameof(DownloadAPI),
                              nameof(OnDownloadException),
                              e,
                              CancellationToken
                          );
                }
            }

        }

        #endregion

        #region HandleErrors(Module, Caller, ExceptionOccurred, ...)

        public async Task HandleErrors(String             Module,
                                       String             Caller,
                                       Exception          ExceptionOccurred,
                                       CancellationToken  CancellationToken = default)
        {

            var onDownloadException = OnDownloadException;
            if (onDownloadException is not null)
            {
                try
                {

                    await Task.WhenAll(onDownloadException.GetInvocationList().
                                           OfType<DownloadExceptionDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                         Timestamp.Now,
                                                                         Module,
                                                                         Caller,
                                                                         ExceptionOccurred,
                                                                         CancellationToken
                                                                     )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, nameof(DownloadAPI));
                }
            }

        }

        #endregion


    }

}
