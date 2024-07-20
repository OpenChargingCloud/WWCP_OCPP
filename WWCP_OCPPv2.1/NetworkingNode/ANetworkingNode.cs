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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public abstract class ANetworkingNode : INetworkingNode
    {

        #region Data

        protected        readonly  HashSet<OCPPWebSocketServer>  ocppWebSocketServers            = [];
        protected        readonly  List<OCPPWebSocketClient>     ocppWebSocketClients            = [];

        protected static readonly  TimeSpan                    SemaphoreSlimTimeout        = TimeSpan.FromSeconds(5);

        private          readonly  HashSet<SignaturePolicy>                                                      signaturePolicies            = [];

        //private                  readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<CSMS.ICSMSChannel, DateTime>>   connectedNetworkingNodes     = [];
        protected        readonly  ConcurrentDictionary<NetworkingNode_Id, NetworkingNode_Id>                    reachableViaNetworkingHubs   = [];

        private                    Int64                                                                         internalRequestId            = 900000;

        private readonly           List<EnqueuedRequest>       EnqueuedRequests            = [];

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public  readonly           TimeSpan                    DefaultMaintenanceEvery     = TimeSpan.FromSeconds(1);
        private static readonly    SemaphoreSlim               MaintenanceSemaphore        = new (1, 1);
        private readonly           Timer                       MaintenanceTimer;

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => Id.ToString();

        /// <summary>
        /// The unique identification of this networking node.
        /// </summary>
        public NetworkingNode_Id           Id                         { get; }

        /// <summary>
        /// An optional multi-language networking node description.
        /// </summary>
        [Optional]
        public I18NString?                 Description                { get; }



        public CustomData                  CustomData                 { get; }


        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean                     DisableMaintenanceTasks    { get; set; }

        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan                    MaintenanceEvery           { get; }


        public DNSClient                   DNSClient                  { get; }
        public IOCPPAdapter                OCPP                       { get; }




        ///// <summary>
        ///// The enumeration of all signature policies.
        ///// </summary>
        //public IEnumerable<SignaturePolicy>  SignaturePolicies
        //    => signaturePolicies;

        ///// <summary>
        ///// The currently active signature policy.
        ///// </summary>
        //public SignaturePolicy               SignaturePolicy
        //    => SignaturePolicies.First();


        public String? ClientCloseMessage
            => "Bye!";


        public IEnumerable<OCPPWebSocketClient> OCPPWebSocketClients
            => ocppWebSocketClients;

        public IEnumerable<OCPPWebSocketServer> OCPPWebSocketServers
            => ocppWebSocketServers;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public ANetworkingNode(NetworkingNode_Id  Id,
                               I18NString?        Description                 = null,
                               CustomData?        CustomData                  = null,

                               SignaturePolicy?   SignaturePolicy             = null,
                               SignaturePolicy?   ForwardingSignaturePolicy   = null,

                               Boolean            DisableSendHeartbeats       = false,
                               TimeSpan?          SendHeartbeatsEvery         = null,
                               TimeSpan?          DefaultRequestTimeout       = null,

                               Boolean            DisableMaintenanceTasks     = false,
                               TimeSpan?          MaintenanceEvery            = null,
                               DNSClient?         DNSClient                   = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id), "The given networking node identification must not be null or empty!");

            this.Id                       = Id;
            this.Description              = Description;
            this.CustomData               = CustomData       ?? new CustomData(Vendor_Id.GraphDefined);

            this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
            this.MaintenanceEvery         = MaintenanceEvery ?? DefaultMaintenanceEvery;
            this.MaintenanceTimer         = new Timer(
                                                DoMaintenanceSync,
                                                null,
                                                this.MaintenanceEvery,
                                                this.MaintenanceEvery
                                            );

            this.DNSClient                = DNSClient        ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.OCPP                     = new OCPPAdapter(
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

        public async Task<HTTPResponse> ConnectWebSocketClient(NetworkingNode_Id                                               NetworkingNodeId,
                                                               URL                                                             RemoteURL,
                                                               HTTPHostname?                                                   VirtualHostname              = null,
                                                               String?                                                         Description                  = null,
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

                                                               Boolean                                                         DisableWebSocketPings        = false,
                                                               TimeSpan?                                                       WebSocketPingEvery           = null,
                                                               TimeSpan?                                                       SlowNetworkSimulationDelay   = null,

                                                               Boolean                                                         DisableMaintenanceTasks      = false,
                                                               TimeSpan?                                                       MaintenanceEvery             = null,

                                                               String?                                                         LoggingPath                  = null,
                                                               String                                                          LoggingContext               = null, //CPClientLogger.DefaultContext,
                                                               LogfileCreatorDelegate?                                         LogfileCreator               = null,
                                                               HTTPClientLogger?                                               HTTPLogger                   = null,
                                                               DNSClient?                                                      DNSClient                    = null)
        {

            var ocppWebSocketClient = new OCPPWebSocketClient(

                                          OCPP,

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

            ocppWebSocketClients.Add(ocppWebSocketClient);

            var connectResponse = await ocppWebSocketClient.Connect();
            if (connectResponse.Item2.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols &&
                connectResponse.Item1 is not null)
            {

                connectResponse.Item1.TryAddCustomData(OCPPAdapter.NetworkingNodeId_WebSocketKey,
                                                       NetworkingNodeId);

                OCPP.AddStaticRouting(NetworkingNodeId,
                                      ocppWebSocketClient,
                                      0,
                                      Timestamp.Now);

            }

            return connectResponse.Item2;

        }

        #endregion


        #region AttachWebSocketServer(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        /// 
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer AttachWebSocketServer(String?                                                         HTTPServiceName              = null,
                                                         IIPAddress?                                                     IPAddress                    = null,
                                                         IPPort?                                                         TCPPort                      = null,
                                                         I18NString?                                                     Description                  = null,

                                                         Boolean                                                         RequireAuthentication        = true,
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

                                                         Boolean                                                         AutoStart                    = false)
        {

            var ocppWebSocketServer = new OCPPWebSocketServer(

                                          OCPP,

                                          HTTPServiceName,
                                          IPAddress,
                                          TCPPort,
                                          Description,

                                          RequireAuthentication,
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

            WireWebSocketServer(ocppWebSocketServer);

            if (AutoStart)
                ocppWebSocketServer.Start();

            return ocppWebSocketServer;

        }

        #endregion

        #region AddOrUpdateHTTPBasicAuth(NetworkingNodeId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        /// <param name="Password">The password of the networking node.</param>
        public void AddOrUpdateHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
                                             String             Password)
        {

            foreach (var webSocketServer in ocppWebSocketServers)
            {
                webSocketServer.AddOrUpdateHTTPBasicAuth(NetworkingNodeId, Password);
            }

        }

        #endregion

        #region RemoveHTTPBasicAuth(NetworkingNodeId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        public void RemoveHTTPBasicAuth(NetworkingNode_Id NetworkingNodeId)
        {

            foreach (var webSocketServer in ocppWebSocketServers)
            {
                webSocketServer.RemoveHTTPBasicAuth(NetworkingNodeId);
            }

        }

        #endregion

        protected abstract void WireWebSocketServer(OCPPWebSocketServer OCPPWebSocketServer);


        #region Shutdown(Message = null, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            await Task.WhenAll(ocppWebSocketServers.
                                   Select (ocppWebSocketServer => ocppWebSocketServer.Shutdown(Message, Wait)).
                                   ToArray());

        }

        #endregion


        #region NextRequestId

        public Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        #endregion


        public Byte[] GetEncryptionKey(NetworkingNode_Id  DestinationId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }

        public Byte[] GetDecryptionKey(NetworkingNode_Id  SourceNodeId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }


        public UInt64 GetEncryptionNonce(NetworkingNode_Id  DestinationId,
                                         UInt16?            KeyId   = null)
        {
            return 1;
        }

        public UInt64 GetEncryptionCounter(NetworkingNode_Id  DestinationId,
                                           UInt16?            KeyId   = null)
        {
            return 1;
        }


        #region (Timer) DoMaintenance(State)

        private void DoMaintenanceSync(Object? State)
        {
            if (!DisableMaintenanceTasks)
                DoMaintenance(State).Wait();
        }

        protected internal virtual async Task _DoMaintenance(Object State)
        {

            foreach (var enqueuedRequest in EnqueuedRequests.ToArray())
            {
                //if (CSClient is ChargingStationWSClient wsClient)
                //{

                //    var response = await wsClient.SendRequest(
                //                             enqueuedRequest.DestinationId,
                //                             enqueuedRequest.Command,
                //                             enqueuedRequest.Request.RequestId,
                //                             enqueuedRequest.RequestJSON
                //                         );

                //    enqueuedRequest.ResponseAction(response);

                //    EnqueuedRequests.Remove(enqueuedRequest);

                //}
            }

        }

        private async Task DoMaintenance(Object State)
        {

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    await _DoMaintenance(State);

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }
            else
                DebugX.LogT("Could not aquire the maintenance tasks lock!");

        }

        #endregion



        #region (virtual) HandleErrors(Module, Caller, ErrorResponse)

        public virtual Task HandleErrors(String  Module,
                                         String  Caller,
                                         String  ErrorResponse)
        {

            return Task.CompletedTask;

        }

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccured)

        public virtual Task HandleErrors(String     Module,
                                         String     Caller,
                                         Exception  ExceptionOccured)
        {

            return Task.CompletedTask;

        }

        #endregion


    }

}
