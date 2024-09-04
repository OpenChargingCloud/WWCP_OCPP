///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Security.Authentication;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod;
//using org.GraphDefined.Vanaheimr.Hermod.DNS;
//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using org.GraphDefined.Vanaheimr.Hermod.Logging;
//using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

//using cloud.charging.open.protocols.WWCP.WebSockets;
//using cloud.charging.open.protocols.WWCP.NetworkingNode;
//using cloud.charging.open.protocols.OCPP.NetworkingNode;
//using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
//{

//    /// <summary>
//    /// The networking node HTTP WebSocket client runs on a networking node
//    /// and connects to a CSMS to invoke methods.
//    /// </summary>
//    public partial class OCPPWebSocketClient : OCPP.WebSockets.OCPPWebSocketClient
//    {

//        #region Data

//        /// <summary>
//        /// The default HTTP user agent string.
//        /// </summary>
//        public new const String  DefaultHTTPUserAgent   = $"GraphDefined OCPP {Version.String} WebSocket Client";

//        private    const String  LogfileName            = "NetworkingNodeWSClient.log";

//        #endregion

//        #region Properties

//        public OCPPAdapter  OCPPAdapter    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new networking node websocket client running on a networking node
//        /// and connecting to a CSMS to invoke methods.
//        /// </summary>
//        /// <param name="NetworkingNode">The parent networking node.</param>
//        /// 
//        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
//        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
//        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
//        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
//        /// <param name="LocalCertificateSelector">A delegate to select a TLS client certificate.</param>
//        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
//        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
//        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
//        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
//        /// <param name="RequestTimeout">An optional Request timeout.</param>
//        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
//        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
//        /// <param name="LoggingPath">The logging path.</param>
//        /// <param name="LoggingContext">An optional context for logging client methods.</param>
//        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
//        /// <param name="HTTPLogger">A HTTP logger.</param>
//        /// <param name="DNSClient">The DNS client to use.</param>
//        public OCPPWebSocketClient(NetworkingNode.INetworkingNode                                  NetworkingNode,

//                                   URL                                                             RemoteURL,
//                                   HTTPHostname?                                                   VirtualHostname              = null,
//                                   I18NString?                                                     Description                  = null,
//                                   Boolean?                                                        PreferIPv4                   = null,
//                                   RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
//                                   LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
//                                   X509Certificate?                                                ClientCert                   = null,
//                                   SslProtocols?                                                   TLSProtocol                  = null,
//                                   String                                                          HTTPUserAgent                = DefaultHTTPUserAgent,
//                                   IHTTPAuthentication?                                            HTTPAuthentication           = null,
//                                   TimeSpan?                                                       RequestTimeout               = null,
//                                   TransmissionRetryDelayDelegate?                                 TransmissionRetryDelay       = null,
//                                   UInt16?                                                         MaxNumberOfRetries           = 3,
//                                   UInt32?                                                         InternalBufferSize           = null,

//                                   IEnumerable<String>?                                            SecWebSocketProtocols        = null,
//                                   NetworkingMode?                                                 NetworkingMode               = null,

//                                   Boolean                                                         DisableWebSocketPings        = false,
//                                   TimeSpan?                                                       WebSocketPingEvery           = null,
//                                   TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

//                                   Boolean                                                         DisableMaintenanceTasks      = false,
//                                   TimeSpan?                                                       MaintenanceEvery             = null,

//                                   String?                                                         LoggingPath                  = null,
//                                   String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
//                                   LogfileCreatorDelegate?                                         LogfileCreator               = null,
//                                   HTTPClientLogger?                                               HTTPLogger                   = null,
//                                   DNSClient?                                                      DNSClient                    = null)

//            : base(NetworkingNode,
//                   RemoteURL,
//                   VirtualHostname,
//                   Description,
//                   PreferIPv4,
//                   RemoteCertificateValidator,
//                   LocalCertificateSelector,
//                   ClientCert,
//                   TLSProtocol,
//                   HTTPUserAgent ?? DefaultHTTPUserAgent,
//                   HTTPAuthentication,
//                   RequestTimeout,
//                   TransmissionRetryDelay,
//                   MaxNumberOfRetries,
//                   InternalBufferSize,

//                   SecWebSocketProtocols,
//                   NetworkingMode,

//                   DisableWebSocketPings,
//                   WebSocketPingEvery,
//                   SlowNetworkSimulationDelay,

//                   DisableMaintenanceTasks,
//                   MaintenanceEvery,

//                   LoggingPath,
//                   LoggingContext,
//                   LogfileCreator,
//                   HTTPLogger,
//                   DNSClient)

//        {

//            this.OCPPAdapter  = NetworkingNode.OCPP;

//            #region Wire On(JSON/Binary)MessageReceived

//            OnJSONMessageReceived += (requestTimestamp,
//                                      wwcpWebSocketClient,
//                                      webSocketClientConnection,
//                                      eventTrackingId,
//                                      messageTimestamp,
//                                      sourceNodeId,
//                                      jsonMessage,
//                                      cancellationToken) =>

//                OCPPAdapter.IN.ProcessJSONMessage(
//                    messageTimestamp,
//                    webSocketClientConnection,
//                    sourceNodeId,
//                    jsonMessage,
//                    eventTrackingId,
//                    cancellationToken
//                );


//            OnBinaryMessageReceived += (requestTimestamp,
//                                        wwcpWebSocketClient,
//                                        webSocketClientConnection,
//                                        eventTrackingId,
//                                        messageTimestamp,
//                                        sourceNodeId,
//                                        binaryMessage,
//                                        cancellationToken) =>

//                OCPPAdapter.IN.ProcessBinaryMessage(
//                    messageTimestamp,
//                    webSocketClientConnection,
//                    sourceNodeId,
//                    binaryMessage,
//                    eventTrackingId,
//                    cancellationToken
//                );

//            #endregion

//            //this.Logger       = new ChargePointwebsocketClient.CPClientLogger(this,
//            //                                                                  LoggingPath,
//            //                                                                  LoggingContext,
//            //                                                                  LogfileCreator);

//        }

//        #endregion


//        #region (private) LogEvent(Logger, LogHandler, ...)

//        private async Task LogEvent<TDelegate>(TDelegate?                                         Logger,
//                                               Func<TDelegate, Task>                              LogHandler,
//                                               [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
//                                               [CallerMemberName()]                       String  OCPPCommand   = "")

//            where TDelegate : Delegate

//        {
//            if (Logger is not null)
//            {
//                try
//                {

//                    await Task.WhenAll(
//                              Logger.GetInvocationList().
//                                     OfType<TDelegate>().
//                                     Select(LogHandler)
//                          );

//                }
//                catch (Exception e)
//                {
//                    await HandleErrors(nameof(OCPPWebSocketClient), $"{OCPPCommand}.{EventName}", e);
//                }
//            }
//        }

//        #endregion

//        #region (private) HandleErrors(Module, Caller, ErrorResponse)

//        private Task HandleErrors(String  Module,
//                                  String  Caller,
//                                  String  ErrorResponse)
//        {

//            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

//            return Task.CompletedTask;

//        }

//        #endregion

//        #region (private) HandleErrors(Module, Caller, ExceptionOccured)

//        private Task HandleErrors(String     Module,
//                                  String     Caller,
//                                  Exception  ExceptionOccured)
//        {

//            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

//            return Task.CompletedTask;

//        }

//        #endregion


//    }

//}
