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
using System.Security.Authentication;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel,
                                                 ICentralSystemChannel
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const            String       DefaultHTTPServiceName  = $"GraphDefined OCPP {OCPPv1_6.Version.String} HTTP/WebSocket/JSON CSMS API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly  IPPort       DefaultHTTPServerPort   = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public new static readonly  HTTPPath     DefaultURLPrefix        = HTTPPath.Parse("/" + OCPPv1_6.Version.String);

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => HTTPServiceName;

        public IEnumerable<ChargeBox_Id> NetworkingNodeIds
            => throw new NotImplementedException();

        #endregion

        #region Events

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<CertificateHashData>?        CustomCertificateHashDataSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?            CustomChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?           CustomChargingScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?     CustomChargingSchedulePeriodSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareImage>?              CustomFirmwareImageSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?          CustomAuthorizationDataSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                  CustomIdTagInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?              CustomLogParametersSerializer                           { get; set; }


        public CustomJObjectSerializerDelegate<SignaturePolicy>?            CustomSignaturePolicySerializer                         { get; set; }

        public NetworkingNode_Id ChargeBoxIdentity => throw new NotImplementedException();

        public string From => throw new NotImplementedException();

        public string To => throw new NotImplementedException();


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize a new HTTP server for the CSMS HTTP/WebSocket/JSON API.
        /// </summary>
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer(NetworkingNode_Id                    NetworkingNodeId,

                                     String                               HTTPServiceName              = DefaultHTTPServiceName,
                                     IIPAddress?                          IPAddress                    = null,
                                     IPPort?                              TCPPort                      = null,

                                     Boolean                              RequireAuthentication        = true,
                                     Boolean                              DisableWebSocketPings        = false,
                                     TimeSpan?                            WebSocketPingEvery           = null,
                                     TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                     Func<X509Certificate2>?              ServerCertificateSelector    = null,
                                     RemoteCertificateValidationHandler?  ClientCertificateValidator   = null,
                                     LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                     SslProtocols?                        AllowedTLSProtocols          = null,
                                     Boolean?                             ClientCertificateRequired    = null,
                                     Boolean?                             CheckCertificateRevocation   = null,

                                     ServerThreadNameCreatorDelegate?     ServerThreadNameCreator      = null,
                                     ServerThreadPriorityDelegate?        ServerThreadPrioritySetter   = null,
                                     Boolean?                             ServerThreadIsBackground     = null,
                                     ConnectionIdBuilder?                 ConnectionIdBuilder          = null,
                                     TimeSpan?                            ConnectionTimeout            = null,
                                     UInt32?                              MaxClientConnections         = null,

                                     DNSClient?                           DNSClient                    = null,
                                     Boolean                              AutoStart                    = false)

            : base(NetworkingNodeId,
                   [
                       Version.WebSocketSubProtocolId
                   ],
                   HTTPServiceName,
                   IPAddress,
                   TCPPort,

                   RequireAuthentication,
                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   ClientCertificateSelector,
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
                   AutoStart)

        {

            //base.OnValidateTCPConnection        += ValidateTCPConnection;
            //base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            //base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            //base.OnCloseMessageReceived         += ProcessCloseMessage;

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var method in typeof(CentralSystemWSServer).
                                       GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                            Where(method            => method.Name.StartsWith("Receive_") &&
                                                 (method.ReturnType == typeof(Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONRequestErrorMessage?>>) ||
                                                  method.ReturnType == typeof(Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONRequestErrorMessage?>>))))
            {

                var processorName = method.Name[8..];

                if (incomingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                incomingMessageProcessorsLookup.Add(processorName,
                                                    method);

            }

            #endregion

            if (AutoStart)
                Start();

        }

        #endregion


        #region AddOrUpdateHTTPBasicAuth(NetworkingNodeId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        /// <param name="Password">The password of the charging station.</param>
        public void AddOrUpdateHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
                                             String             Password)
        {

            NetworkingNodeLogins.AddOrUpdate(NetworkingNodeId,
                                          Password,
                                          (chargingStationId, password) => Password);

        }

        #endregion

        #region RemoveHTTPBasicAuth     (NetworkingNodeId)

        /// <summary>
        /// Remove the given HTTP Basic Authentication for the given networking node.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the networking node.</param>
        public Boolean RemoveHTTPBasicAuth(NetworkingNode_Id NetworkingNodeId)
        {

            if (NetworkingNodeLogins.ContainsKey(NetworkingNodeId))
                return NetworkingNodeLogins.TryRemove(NetworkingNodeId, out _);

            return true;

        }

        #endregion



        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  Exception,
                                  String?    Description   = null)
        {

            DebugX.LogException(Exception, $"{Module}.{Caller}{(Description is not null ? $" {Description}" : "")}");

            return Task.CompletedTask;

        }

    }

}
