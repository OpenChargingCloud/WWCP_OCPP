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

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    public abstract class AOCPPNetworkingNode : OCPP.NetworkingNode.AOCPPNetworkingNode,
                                                INetworkingNode
    {

        #region Data

        //private          readonly  HashSet<SignaturePolicy>                                    signaturePolicies            = [];

        //private                    Int64                                                       internalRequestId            = 900000;

        //private          readonly  List<EnqueuedRequest>                                       EnqueuedRequests             = [];

        private DateTime lastRoutesBroadcast = Timestamp.Now;

        #endregion

        #region Properties

        public OCPPAdapter  OCPP    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP v1.6 Networking Node.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public AOCPPNetworkingNode(NetworkingNode_Id  Id,
                                   I18NString?        Description                 = null,
                                   CustomData?        CustomData                  = null,

                                   SignaturePolicy?   SignaturePolicy             = null,
                                   SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                   HTTPExtAPI?        HTTPExtAPI                  = null,

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

            NextHopNetworkingNodeId ??= NetworkingNode_Id.CentralSystem;

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
                                                                      "ocpp2.0.1",
                                                                       Version.WebSocketSubProtocolId
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

            AttachWebSocketServer(String?                                                         HTTPServiceName              = null,
                                  IIPAddress?                                                     IPAddress                    = null,
                                  IPPort?                                                         TCPPort                      = null,
                                  I18NString?                                                     Description                  = null,

                                  Boolean                                                         RequireAuthentication        = true,
                                  IEnumerable<String>?                                            SecWebSocketProtocols        = null,
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

                                          HTTPServiceName       ?? "GraphDefined OCPP v1.6 WebSocket Service",
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
                                          SecWebSocketProtocols ?? [
                                                                       Version.WebSocketSubProtocolId
                                                                   ],
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

            #region Wire On(JSON/Binary)MessageReceived

            ocppWebSocketServer.OnJSONMessageReceived += (requestTimestamp,
                                                          wwcpWebSocketServer,
                                                          webSocketServerConnection,
                                                          eventTrackingId,
                                                          messageTimestamp,
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


            ocppWebSocketServer.OnBinaryMessageReceived += (requestTimestamp,
                                                            wwcpWebSocketServer,
                                                            webSocketServerConnection,
                                                            eventTrackingId,
                                                            messageTimestamp,
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

        #region (protected) WireWebSocketServer(WebSocketServer)

        //protected void WireWebSocketServer(OCPP.WebSockets.OCPPWebSocketServer WebSocketServer)
        //{

        //    base.WireWebSocketServer(WebSocketServer);

        //    // (Generic) Error Handling

        //}

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

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccured)

        //public virtual Task HandleErrors(String     Module,
        //                                 String     Caller,
        //                                 Exception  ExceptionOccured)
        //{

        //    DebugX.LogException(ExceptionOccured, Caller);

        //    return Task.CompletedTask;

        //}

        #endregion


    }

}
