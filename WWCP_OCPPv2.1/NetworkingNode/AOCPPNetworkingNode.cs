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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public abstract class AOCPPNetworkingNode : OCPP.NetworkingNode.AOCPPNetworkingNode,
                                                INetworkingNode
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public    const   String                                              DefaultHTTPServiceName  = $"GraphDefined OCPP {Version.String} WebSocket Server";

        //private          readonly  HashSet<SignaturePolicy>                                    signaturePolicies            = [];

        //private                    Int64                                                       internalRequestId            = 900000;

        //private          readonly  List<EnqueuedRequest>                                       EnqueuedRequests             = [];

        private DateTimeOffset lastRoutesBroadcast = Timestamp.Now;

        #endregion

        #region Properties

        public OCPPAdapter  OCPP    { get; }

        #endregion

        #region Controllers

        #region Generics

        public IEnumerable<T> GetComponentConfigs<T>(String Name)
            where T : ComponentConfig

            => OCPP.TryGetComponentConfig(Name, out var controllerList)
                   ? controllerList.Cast<T>().ToList()
                   : [];

        public IEnumerable<T> GetComponentConfigs<T>(String  Name,
                                                     String  Instance)
            where T : ComponentConfig

            => OCPP.GetComponentConfigs<T>(
                   Name,
                   Instance
               );

        #endregion


        #region ClockController(s)

        /// <summary>
        /// All Clock Controllers
        /// </summary>
        public IEnumerable<ClockController>  ClockControllers()
            => GetComponentConfigs<ClockController>(nameof(OCPPv2_1.ClockController));

        /// <summary>
        /// The Clock Controller for the given instance.
        /// </summary>
        /// <param name="Instance">The name of the instance.</param>
        public ClockController?              ClockController(String Instance)
            => GetComponentConfigs<ClockController>(nameof(ClockController), Instance).FirstOrDefault();

        #endregion

        #region TimeClientController(s)

        /// <summary>
        /// All Time Controllers
        /// </summary>
        public IEnumerable<NTPClientGroupController>  TimeClientControllers()
            => GetComponentConfigs<NTPClientGroupController>(nameof(OCPPv2_1.NTPClientGroupController));

        /// <summary>
        /// The Time Controller for the given instance.
        /// </summary>
        /// <param name="Instance">The name of the instance.</param>
        public NTPClientGroupController?              TimeClientController(String Instance)
            => GetComponentConfigs<NTPClientGroupController>(nameof(TimeClientController), Instance).FirstOrDefault();

        /// <summary>
        /// The legal Time Controller.
        /// </summary>
        public NTPClientGroupController?              LegalTimeClientController
            => TimeClientController("legal");

        /// <summary>
        /// The "csms" Time Controller.
        /// </summary>
        public NTPClientGroupController?              CSMSTimeClientController
            => TimeClientController("csms");

        #endregion

        #region NetworkTimeClientController(s)

        /// <summary>
        /// All Network Time Controllers
        /// </summary>
        public IEnumerable<NTPClientController>  NetworkTimeClientControllers()
            => GetComponentConfigs<NTPClientController>(nameof(OCPPv2_1.NTPClientController));

        /// <summary>
        /// The Network Time Controller for the given instance.
        /// </summary>
        /// <param name="Instance">The name of the instance.</param>
        public NTPClientController?              NetworkTimeClientController(String Instance)
            => GetComponentConfigs<NTPClientController>(nameof(NetworkTimeClientController), Instance).FirstOrDefault();

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP v2.1 Networking Node.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public AOCPPNetworkingNode(NetworkingNode_Id  Id,
                                   I18NString?        Description                 = null,
                                   CustomData?        CustomData                  = null,

                                   SignaturePolicy?   SignaturePolicy             = null,
                                   SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                   HTTPExtAPI?        HTTPExtAPI                  = null,
                                   WebSocketServer?   ControlWebSocketServer      = null,

                                   Boolean            DisableSendHeartbeats       = false,
                                   TimeSpan?          SendHeartbeatsEvery         = null,
                                   TimeSpan?          DefaultRequestTimeout       = null,

                                   Boolean            DisableMaintenanceTasks     = false,
                                   TimeSpan?          MaintenanceEvery            = null,
                                   DNSClient?         DNSClient                   = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPExtAPI,
                   ControlWebSocketServer,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,
                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient)

        {

            this.OCPP = new OCPPAdapter(
                            this,
                            DisableSendHeartbeats,
                            SendHeartbeatsEvery,
                            DefaultRequestTimeout,
                            SignaturePolicy,
                            ForwardingSignaturePolicy
                        );

        }

        #endregion


        #region ConnectWebSocketClient(...)

        public override async Task<HTTPResponse>

            ConnectOCPPWebSocketClient(URL                                                             RemoteURL,
                                       HTTPHostname?                                                   VirtualHostname              = null,
                                       I18NString?                                                     Description                  = null,
                                       Boolean?                                                        PreferIPv4                   = null,
                                       RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                       X509Certificate?                                                ClientCert                   = null,
                                       SslProtocols?                                                   TLSProtocol                  = null,
                                       String?                                                         HTTPUserAgent                = null,
                                       IHTTPAuthentication?                                            HTTPAuthentication           = null,
                                       TimeSpan?                                                       RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate?                                 TransmissionRetryDelay       = null,
                                       UInt16?                                                         MaxNumberOfRetries           = 3,
                                       UInt32?                                                         InternalBufferSize           = null,

                                       IEnumerable<String>?                                            SecWebSocketProtocols        = null,
                                       NetworkingMode?                                                 NetworkingMode               = null,
                                       NetworkingNode_Id?                                              NextHopNetworkingNodeId      = null,
                                       IEnumerable<NetworkingNode_Id>?                                 RoutingNetworkingNodeIds     = null,

                                       Boolean                                                         DisableWebSocketPings        = false,
                                       TimeSpan?                                                       WebSocketPingEvery           = null,
                                       TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                       Boolean                                                         DisableMaintenanceTasks      = false,
                                       TimeSpan?                                                       MaintenanceEvery             = null,

                                       String?                                                         LoggingPath                  = null,
                                       String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
                                       LogfileCreatorDelegate?                                         LogfileCreator               = null,
                                       HTTPClientLogger?                                               HTTPLogger                   = null,
                                       DNSClient?                                                      DNSClient                    = null,

                                       EventTracking_Id?                                               EventTrackingId              = null,
                                       CancellationToken                                               CancellationToken            = default)

        {

            #region Create new OCPP WebSocket client

            var ocppWebSocketClient = new OCPP.WebSockets.OCPPWebSocketClient(

                                          this,

                                          RemoteURL,
                                          VirtualHostname,
                                          Description,
                                          PreferIPv4,
                                          RemoteCertificateValidator,
                                          LocalCertificateSelector,
                                          ClientCert,
                                          TLSProtocol,
                                          HTTPUserAgent,
                                          HTTPAuthentication,
                                          RequestTimeout,
                                          TransmissionRetryDelay,
                                          MaxNumberOfRetries,
                                          InternalBufferSize,

                                          SecWebSocketProtocols ?? [
                                                                       Version.WebSocketSubProtocolId,
                                                                      "ocpp2.0.1"
                                                                   ],
                                          NetworkingMode,

                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DisableMaintenanceTasks,
                                          MaintenanceEvery,

                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator,
                                          HTTPLogger,
                                          DNSClient

                                      );

            #endregion

            #region Wire On(JSON/Binary)MessageReceived

            ocppWebSocketClient.OnJSONMessageReceived += (requestTimestamp,
                                                          wwcpWebSocketClient,
                                                          webSocketClientConnection,
                                                          eventTrackingId,
                                                          messageTimestamp,
                                                          sourceNodeId,
                                                          jsonMessage,
                                                          cancellationToken) =>

                OCPP.IN.ProcessJSONMessage(
                    messageTimestamp,
                    webSocketClientConnection,
                    sourceNodeId,
                    jsonMessage,
                    eventTrackingId,
                    cancellationToken
                );


            ocppWebSocketClient.OnBinaryMessageReceived += (requestTimestamp,
                                                            wwcpWebSocketClient,
                                                            webSocketClientConnection,
                                                            eventTrackingId,
                                                            messageTimestamp,
                                                            sourceNodeId,
                                                            binaryMessage,
                                                            cancellationToken) =>

                OCPP.IN.ProcessBinaryMessage(
                    messageTimestamp,
                    webSocketClientConnection,
                    sourceNodeId,
                    binaryMessage,
                    eventTrackingId,
                    cancellationToken
                );

            #endregion

            return await ConnectWebSocketClient(

                             ocppWebSocketClient,

                             NextHopNetworkingNodeId,
                             RoutingNetworkingNodeIds,
                             EventTrackingId,
                             CancellationToken

                         );

            //wwcpWebSocketClients.Add(ocppWebSocketClient);

            //var connectResponse = await ocppWebSocketClient.Connect(
            //                                EventTrackingId:      EventTrackingId ?? EventTracking_Id.New,
            //                                RequestTimeout:       RequestTimeout,
            //                                MaxNumberOfRetries:   MaxNumberOfRetries,
            //                                HTTPRequestBuilder:   (httpRequestBuilder) => {
            //                                                          if (NetworkingMode == NetworkingNode.NetworkingMode.OverlayNetwork)
            //                                                              httpRequestBuilder.SetHeaderField(OCPPAdapter.X_OCPP_NetworkingMode, NetworkingMode.ToString());
            //                                                      },
            //                                CancellationToken:    CancellationToken
            //                            );

            //if (connectResponse.Item2.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols &&
            //    connectResponse.Item1 is not null)
            //{

            //    if (NextHopNetworkingNodeId is not null)
            //    {

            //        connectResponse.Item1.TryAddCustomData(
            //            OCPPAdapter.NetworkingNodeId_WebSocketKey,
            //            NextHopNetworkingNodeId
            //        );

            //        OCPP.Routing.AddOrUpdateStaticRouting(
            //            NextHopNetworkingNodeId.Value,
            //            ocppWebSocketClient,
            //            0,
            //            1,
            //            Timestamp.Now
            //        );

            //    }

            //    if (RoutingNetworkingNodeIds is not null && RoutingNetworkingNodeIds.Any())
            //        OCPP.Routing.AddOrUpdateStaticRouting(
            //            RoutingNetworkingNodeIds,
            //            ocppWebSocketClient,
            //            0,
            //            1,
            //            Timestamp.Now
            //        );

            //}

            //return connectResponse.Item2;

        }

        #endregion

        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP WebSocket service.</param>
        /// 
        /// <param name="AutoStart">Start the server immediately.</param>
        public override OCPP.WebSockets.OCPPWebSocketServer

            AttachWebSocketServer(String?                                                         HTTPServiceName              = DefaultHTTPServiceName,
                                  IIPAddress?                                                     IPAddress                    = null,
                                  IPPort?                                                         TCPPort                      = null,
                                  I18NString?                                                     Description                  = null,

                                  Boolean                                                         RequireAuthentication        = true,
                                  IEnumerable<String>?                                            SecWebSocketProtocols        = null,
                                  SubprotocolSelectorDelegate?                                    SubprotocolSelector          = null,
                                  Boolean                                                         DisableWebSocketPings        = false,
                                  TimeSpan?                                                       WebSocketPingEvery           = null,
                                  TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                  Func<X509Certificate2>?                                         ServerCertificateSelector    = null,
                                  RemoteTLSClientCertificateValidationHandler<IWebSocketServer>?  ClientCertificateValidator   = null,
                                  LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                  SslProtocols?                                                   AllowedTLSProtocols          = null,
                                  Boolean?                                                        ClientCertificateRequired    = null,
                                  Boolean?                                                        CheckCertificateRevocation   = null,

                                  ServerThreadNameCreatorDelegate?                                ServerThreadNameCreator      = null,
                                  ServerThreadPriorityDelegate?                                   ServerThreadPrioritySetter   = null,
                                  Boolean?                                                        ServerThreadIsBackground     = null,
                                  ConnectionIdBuilder?                                            ConnectionIdBuilder          = null,
                                  TimeSpan?                                                       ConnectionTimeout            = null,
                                  UInt32?                                                         MaxClientConnections         = null,

                                  Boolean                                                         AutoStart                    = true)

        {

            #region Create new OCPP WebSocket server

            var ocppWebSocketServer = new OCPP.WebSockets.OCPPWebSocketServer(

                                          this,

                                          HTTPServiceName ?? DefaultHTTPServiceName,
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
                                          SecWebSocketProtocols ?? [
                                                                      "ocpp2.0.1",
                                                                       Version.WebSocketSubProtocolId
                                                                   ],
                                          SubprotocolSelector,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          ServerCertificateSelector,
                                          ClientCertificateValidator,
                                          LocalCertificateSelector,
                                          AllowedTLSProtocols,
                                          ClientCertificateRequired,
                                          CheckCertificateRevocation,

                                          ServerThreadNameCreator,
                                          ServerThreadPrioritySetter,
                                          ServerThreadIsBackground,
                                          ConnectionIdBuilder,
                                          ConnectionTimeout,
                                          MaxClientConnections,

                                          DNSClient,
                                          AutoStart: false

                                      );

            #endregion

            #region Wire On(JSON/Binary)MessageReceived with OCPP Adapter

            ocppWebSocketServer.OnJSONMessageReceived2 += (requestTimestamp,
                                                           wwcpWebSocketServer,
                                                           webSocketServerConnection,
                                                           messageTimestamp,
                                                           eventTrackingId,
                                                           sourceNodeId,
                                                           jsonMessage,
                                                           cancellationToken) =>

                OCPP.IN.ProcessJSONMessage(
                    messageTimestamp,
                    webSocketServerConnection,
                    sourceNodeId,
                    jsonMessage,
                    eventTrackingId,
                    cancellationToken
                );


            ocppWebSocketServer.OnBinaryMessageReceived2 += (requestTimestamp,
                                                             wwcpWebSocketServer,
                                                             webSocketServerConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             sourceNodeId,
                                                             binaryMessage,
                                                             cancellationToken) =>

                OCPP.IN.ProcessBinaryMessage(
                    messageTimestamp,
                    webSocketServerConnection,
                    sourceNodeId,
                    binaryMessage,
                    eventTrackingId,
                    cancellationToken
                );

            #endregion


            WireWebSocketServer(ocppWebSocketServer);

            if (AutoStart)
                ocppWebSocketServer.Start();

            return ocppWebSocketServer;

        }

        #endregion


        #region (Timer) DoMaintenance(State)

        //protected override async Task DoMaintenanceAsync(Object State)
        //{

        //    await Task.Delay(1);

        //    if (Timestamp.Now > lastRoutesBroadcast + TimeSpan.FromSeconds(10))
        //    {

        //        lastRoutesBroadcast = Timestamp.Now;

        //        var routes = Routing.GetNetworkRoutingInformation();
        //        if (routes.Any())
        //        {

        //        //    var notifyNetworkTopologyMessage = new NotifyNetworkTopologyMessage(
        //        //                                           Destination:                  SourceRouting.Broadcast,
        //        //                                           NetworkTopologyInformation:   new NetworkTopologyInformation(
        //        //                                                                             RoutingNode:  Id,
        //        //                                                                             Routes:       routes,
        //        //                                                                             NotBefore:    null,
        //        //                                                                             NotAfter:     null,
        //        //                                                                             Priority:     null
        //        //                                                                         )
        //        //                                       );

        //        //    var rr = OCPP.OUT.NotifyNetworkTopology(notifyNetworkTopologyMessage);

        //        }

        //    }

        //}

        #endregion


        #region LogEvent(OCPPIO, Logger, LogHandler, ...)

        //public async Task LogEvent<TDelegate>(String                                             OCPPIO,
        //                                      TDelegate?                                         Logger,
        //                                      Func<TDelegate, Task>                              LogHandler,
        //                                      [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
        //                                      [CallerMemberName()]                       String  OCPPCommand   = "")

        //    where TDelegate : Delegate

        //{
        //    if (Logger is not null)
        //    {
        //        try
        //        {

        //            await Task.WhenAll(
        //                      Logger.GetInvocationList().
        //                             OfType<TDelegate>().
        //                             Select(LogHandler)
        //                  );

        //        }
        //        catch (Exception e)
        //        {
        //            await HandleErrors(OCPPIO, $"{OCPPCommand}.{EventName}", e);
        //        }
        //    }
        //}

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ErrorResponse)

        //public virtual Task HandleErrors(String  Module,
        //                                 String  Caller,
        //                                 String  ErrorResponse)
        //{

        //    DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

        //    return Task.CompletedTask;

        //}

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccurred)

        //public virtual Task HandleErrors(String     Module,
        //                                 String     Caller,
        //                                 Exception  ExceptionOccurred)
        //{

        //    DebugX.LogException(ExceptionOccurred, Caller);

        //    return Task.CompletedTask;

        //}

        #endregion


    }

}
