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

//using System.Reflection;
//using System.Collections.Concurrent;
//using System.Security.Authentication;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod;
//using org.GraphDefined.Vanaheimr.Hermod.DNS;
//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
//using org.GraphDefined.Vanaheimr.Hermod.Sockets;

//using cloud.charging.open.protocols.WWCP.NetworkingNode;
//using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
//using cloud.charging.open.protocols.WWCP;
//using cloud.charging.open.protocols.WWCP.WebSockets;
//using cloud.charging.open.protocols.OCPP.WebSockets;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
//{

//    /// <summary>
//    /// The OCPP HTTP WebSocket server.
//    /// </summary>
//    public partial class OCPPWebSocketServer : OCPP.WebSockets.OCPPWebSocketServer
//    {

//        #region Data

//        /// <summary>
//        /// The default HTTP server name.
//        /// </summary>
//        public new const   String                                              DefaultHTTPServiceName  = $"GraphDefined OCPP {Version.String} WebSocket Server";

//      //  protected readonly ConcurrentDictionary<Request_Id, SendRequestState>  requests                = [];

//        public new const   String                                              LogfileName             = "CSMSWSServer.log";

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The parent OCPP adapter.
//        /// </summary>
//        public OCPPAdapter  OCPPAdapter    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new OCPP HTTP WebSocket server.
//        /// </summary>
//        /// <param name="NetworkingNode">The parent networking node.</param>
//        /// 
//        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
//        /// <param name="IPAddress">An IP address to listen on.</param>
//        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
//        /// <param name="Description">An optional description of this HTTP WebSocket service.</param>
//        /// 
//        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
//        /// 
//        /// <param name="DNSClient">An optional DNS client to use.</param>
//        /// <param name="AutoStart">Start the server immediately.</param>
//        public OCPPWebSocketServer(NetworkingNode.INetworkingNode                                  NetworkingNode,

//                                   String?                                                         HTTPServiceName              = DefaultHTTPServiceName,
//                                   IIPAddress?                                                     IPAddress                    = null,
//                                   IPPort?                                                         TCPPort                      = null,
//                                   I18NString?                                                     Description                  = null,

//                                   Boolean                                                         RequireAuthentication        = true,
//                                   IEnumerable<String>?                                            SecWebSocketProtocols        = null,
//                                   Boolean                                                         DisableWebSocketPings        = false,
//                                   TimeSpan?                                                       WebSocketPingEvery           = null,
//                                   TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

//                                   Func<X509Certificate2>?                                         ServerCertificateSelector    = null,
//                                   RemoteTLSClientCertificateValidationHandler<IWebSocketServer>?  ClientCertificateValidator   = null,
//                                   LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
//                                   SslProtocols?                                                   AllowedTLSProtocols          = null,
//                                   Boolean?                                                        ClientCertificateRequired    = null,
//                                   Boolean?                                                        CheckCertificateRevocation   = null,

//                                   ServerThreadNameCreatorDelegate?                                ServerThreadNameCreator      = null,
//                                   ServerThreadPriorityDelegate?                                   ServerThreadPrioritySetter   = null,
//                                   Boolean?                                                        ServerThreadIsBackground     = null,
//                                   ConnectionIdBuilder?                                            ConnectionIdBuilder          = null,
//                                   TimeSpan?                                                       ConnectionTimeout            = null,
//                                   UInt32?                                                         MaxClientConnections         = null,

//                                   DNSClient?                                                      DNSClient                    = null,
//                                   Boolean                                                         AutoStart                    = true)

//            : base(NetworkingNode,
//                   HTTPServiceName,
//                   IPAddress,
//                   TCPPort,
//                   Description,

//                   RequireAuthentication,
//                   SecWebSocketProtocols ?? [
//                                               "ocpp2.0.1",
//                                                Version.WebSocketSubProtocolId
//                                            ],
//                   DisableWebSocketPings,
//                   WebSocketPingEvery,
//                   SlowNetworkSimulationDelay,

//                   ServerCertificateSelector,
//                   ClientCertificateValidator,
//                   LocalCertificateSelector,
//                   AllowedTLSProtocols,
//                   ClientCertificateRequired,
//                   CheckCertificateRevocation,

//                   ServerThreadNameCreator,
//                   ServerThreadPrioritySetter,
//                   ServerThreadIsBackground,
//                   ConnectionIdBuilder,
//                   ConnectionTimeout,
//                   MaxClientConnections,

//                   DNSClient,
//                   false)

//        {

//            this.OCPPAdapter  = NetworkingNode.OCPP;

//            #region Wire On(JSON/Binary)MessageReceived

//            OnJSONMessageReceived += (requestTimestamp,
//                                      wwcpWebSocketServer,
//                                      webSocketServerConnection,
//                                      eventTrackingId,
//                                      messageTimestamp,
//                                      sourceNodeId,
//                                      jsonMessage,
//                                      cancellationToken) =>

//                OCPPAdapter.IN.ProcessJSONMessage(
//                    messageTimestamp,
//                    webSocketServerConnection,
//                    sourceNodeId,
//                    jsonMessage,
//                    eventTrackingId,
//                    cancellationToken
//                );


//            OnBinaryMessageReceived += (requestTimestamp,
//                                        wwcpWebSocketServer,
//                                        webSocketServerConnection,
//                                        eventTrackingId,
//                                        messageTimestamp,
//                                        sourceNodeId,
//                                        binaryMessage,
//                                        cancellationToken) =>

//                OCPPAdapter.IN.ProcessBinaryMessage(
//                    messageTimestamp,
//                    webSocketServerConnection,
//                    sourceNodeId,
//                    binaryMessage,
//                    eventTrackingId,
//                    cancellationToken
//                );

//            #endregion

//            //this.Logger       = new ChargePointwebsocketClient.CPClientLogger(this,
//            //                                                             LoggingPath,
//            //                                                             LoggingContext,
//            //                                                             LogfileCreator);

//            //if (AutoStart)
//            //    Start();

//        }

//        #endregion



//        protected override IEnumerable<WebSocketServerConnection> LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId)
//        {

//            if (NetworkingNodeId == NetworkingNode_Id.Zero)
//                return [];

//            if (NetworkingNodeId == NetworkingNode_Id.Broadcast)
//                return WebSocketConnections;

//            var lookUpNetworkingNodeId = NetworkingNodeId;

//            if (OCPPAdapter.NetworkingNode.Routing.LookupNetworkingNode(lookUpNetworkingNodeId, out var reachability) &&
//                reachability is not null)
//            {
//                lookUpNetworkingNodeId = reachability.DestinationId;
//            }

//            if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
//            {
//                lookUpNetworkingNodeId = networkingHubId;
//                return WebSocketConnections.Where (connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(WebSocketKeys.NetworkingNodeId) == lookUpNetworkingNodeId);
//            }

//            return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(WebSocketKeys.NetworkingNodeId) == lookUpNetworkingNodeId).ToArray();

//        }


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
