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
using System.Collections.Concurrent;
using System.Security.Authentication;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using System.Security.Cryptography.X509Certificates;
using cloud.charging.open.protocols.OCPP.CSMS;
using System.Collections.Generic;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Common Connection Management

    /// <summary>
    /// A delegate for logging new HTTP Web Socket connections.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="NewConnection">The new HTTP Web Socket connection.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="SharedSubprotocols">An enumeration of shared HTTP Web Sockets subprotocols.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeNewWebSocketConnectionDelegate        (DateTime                           Timestamp,
                                                                                OCPPWebSocketServer                NetworkingNodeChannel,
                                                                                WebSocketServerConnection          NewConnection,
                                                                                NetworkingNode_Id                  NetworkingNodeId,
                                                                                IEnumerable<String>                SharedSubprotocols,
                                                                                EventTracking_Id                   EventTrackingId,
                                                                                CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a HTTP Web Socket CLOSE message.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusCode">The HTTP Web Socket Closing Status Code.</param>
    /// <param name="Reason">An optional HTTP Web Socket closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeCloseMessageReceivedDelegate          (DateTime                           Timestamp,
                                                                                OCPPWebSocketServer                NetworkingNodeChannel,
                                                                                WebSocketServerConnection          Connection,
                                                                                NetworkingNode_Id                  NetworkingNodeId,
                                                                                EventTracking_Id                   EventTrackingId,
                                                                                WebSocketFrame.ClosingStatusCode   StatusCode,
                                                                                String?                            Reason,
                                                                                CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate for logging a closed TCP connection.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="NetworkingNodeChannel">The HTTP Web Socket channel.</param>
    /// <param name="Connection">The HTTP Web Socket connection to be closed.</param>
    /// <param name="NetworkingNodeId">The sending OCPP networking node/charging station identification.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="Reason">An optional closing reason.</param>
    /// <param name="CancellationToken">A token to cancel the processing.</param>
    public delegate Task OnNetworkingNodeTCPConnectionClosedDelegate           (DateTime                           Timestamp,
                                                                                OCPPWebSocketServer                NetworkingNodeChannel,
                                                                                WebSocketServerConnection          Connection,
                                                                                NetworkingNode_Id                  NetworkingNodeId,
                                                                                EventTracking_Id                   EventTrackingId,
                                                                                String?                            Reason,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// The OCPP HTTP Web Socket server.
    /// </summary>
    public partial class OCPPWebSocketServer : WebSocketServer,
                                               IOCPPWebSocketServer
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public  const           String                                                                               DefaultHTTPServiceName
            = $"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly  IPPort                                                                               DefaultHTTPServerPort             = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public static readonly  HTTPPath                                                                             DefaultURLPrefix                  = HTTPPath.Parse("/" + Version.String);

        protected readonly      Dictionary<String, MethodInfo>                                                       incomingMessageProcessorsLookup   = [];
        protected readonly      ConcurrentDictionary<NetworkingNode_Id, Tuple<WebSocketServerConnection, DateTime>>  connectedNetworkingNodes          = [];
        protected readonly      ConcurrentDictionary<NetworkingNode_Id, NetworkingNode_Id>                           reachableViaNetworkingHubs        = [];
        protected readonly      ConcurrentDictionary<Request_Id, SendRequestState>                                   requests                          = [];

        public const            String                                                                               LogfileName                       = "CSMSWSServer.log";

        #endregion

        #region Properties

        /// <summary>
        /// The parent OCPP adapter.
        /// </summary>
        public IOCPPAdapter                                      OCPPAdapter { get; }

        /// <summary>
        /// The enumeration of all connected networking nodes.
        /// </summary>
        public IEnumerable<NetworkingNode_Id>                    NetworkingNodeIds
            => connectedNetworkingNodes.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all networking nodes.
        /// </summary>
        public Boolean                                           RequireAuthentication    { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public ConcurrentDictionary<NetworkingNode_Id, String?>  NetworkingNodeLogins     { get; }
            = new();

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                                        JSONFormatting           { get; set; }
            = Formatting.None;

        /// <summary>
        /// The request timeout for messages sent by this HTTP WebSocket server.
        /// </summary>
        public TimeSpan?                                         RequestTimeout           { get; set; }

        #endregion

        #region Events

        #region Common Connection Management

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnNetworkingNodeNewWebSocketConnectionDelegate?  OnNetworkingNodeNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnNetworkingNodeCloseMessageReceivedDelegate?    OnNetworkingNodeCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnNetworkingNodeTCPConnectionClosedDelegate?     OnNetworkingNodeTCPConnectionClosed;

        #endregion

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?   OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a text message was sent.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?  OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?    OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a text message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?   OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?  OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?    OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?   OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP HTTP Web Socket server.
        /// </summary>
        /// <param name="OCPPAdapter">The parent OCPP adapter.</param>
        /// 
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP Web Socket service.</param>
        /// 
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// 
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer(IOCPPAdapter                                                    OCPPAdapter,

                                   String?                                                         HTTPServiceName              = DefaultHTTPServiceName,
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

                                   DNSClient?                                                      DNSClient                    = null,
                                   Boolean                                                         AutoStart                    = false)

            : base(IPAddress,
                   TCPPort         ?? IPPort.Parse(8000),
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   Description,

                   [
                      "ocpp2.0.1",
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
                   false)

        {

            this.OCPPAdapter = OCPPAdapter;

            this.RequireAuthentication = RequireAuthentication;

            //this.Logger          = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                LoggingPath,
            //                                                                LoggingContext,
            //                                                                LogfileCreator);

            base.OnValidateTCPConnection        += ValidateTCPConnection;
            base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            base.OnCloseMessageReceived         += ProcessCloseMessage;

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

            NetworkingNodeLogins.AddOrUpdate(
                                     NetworkingNodeId,
                                     Password,
                                     (chargingStationId, password) => Password
                                 );

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


        // Connection management...

        #region (protected) ValidateTCPConnection         (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<ConnectionFilterResponse> ValidateTCPConnection(DateTime                      LogTimestamp,
                                                                     IWebSocketServer              Server,
                                                                     System.Net.Sockets.TcpClient  Connection,
                                                                     EventTracking_Id              EventTrackingId,
                                                                     CancellationToken             CancellationToken)
        {

            return Task.FromResult(ConnectionFilterResponse.Accepted());

        }

        #endregion

        #region (protected) ValidateWebSocketConnection   (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<HTTPResponse?> ValidateWebSocketConnection(DateTime                   LogTimestamp,
                                                                IWebSocketServer           Server,
                                                                WebSocketServerConnection  Connection,
                                                                EventTracking_Id           EventTrackingId,
                                                                CancellationToken          CancellationToken)
        {

            #region Verify 'Sec-WebSocket-Protocol'...

            if (Connection.HTTPRequest?.SecWebSocketProtocol is null ||
                Connection.HTTPRequest?.SecWebSocketProtocol.Any() == false)
            {

                DebugX.Log($"{nameof(AOCPPWebSocketServer)} connection from {Connection.RemoteSocket}: Missing 'Sec-WebSocket-Protocol' HTTP header!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.Application.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                     JSONObject.Create(
                                                         new JProperty("en", "Missing 'Sec-WebSocket-Protocol' HTTP header!")
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }
            else if (!SecWebSocketProtocols.Overlaps(Connection.HTTPRequest?.SecWebSocketProtocol ?? Array.Empty<String>()))
            {

                var error = $"This WebSocket service only supports {SecWebSocketProtocols.Select(id => $"'{id}'").AggregateWith(", ")}!";

                DebugX.Log($"{nameof(AOCPPWebSocketServer)} connection from {Connection.RemoteSocket}: {error}");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.BadRequest,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               ContentType     = HTTPContentType.Application.JSON_UTF8,
                               Content         = JSONObject.Create(
                                                     new JProperty("description",
                                                         JSONObject.Create(
                                                             new JProperty("en", error)
                                                     ))).ToUTF8Bytes(),
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            #region Verify HTTP Authentication

            if (RequireAuthentication)
            {

                if (Connection.HTTPRequest?.Authorization is HTTPBasicAuthentication basicAuthentication)
                {

                    if (NetworkingNodeLogins.TryGetValue(NetworkingNode_Id.Parse(basicAuthentication.Username), out var password) &&
                        basicAuthentication.Password == password)
                    {
                        DebugX.Log($"{nameof(AOCPPWebSocketServer)} connection from {Connection.RemoteSocket} using authorization: '{basicAuthentication.Username}' / '{basicAuthentication.Password}'");
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log($"{nameof(AOCPPWebSocketServer)} connection from {Connection.RemoteSocket} invalid authorization: '{basicAuthentication.Username}' / '{basicAuthentication.Password}'!");

                }
                else
                    DebugX.Log($"{nameof(AOCPPWebSocketServer)} connection from {Connection.RemoteSocket} missing authorization!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               Connection      = "close"
                           }.AsImmutable
                       );

            }

            #endregion

            return Task.FromResult<HTTPResponse?>(null);

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection (LogTimestamp, Server, Connection, SharedSubprotocols, EventTrackingId, CancellationToken)

        protected async Task ProcessNewWebSocketConnection(DateTime                   LogTimestamp,
                                                           IWebSocketServer           Server,
                                                           WebSocketServerConnection  Connection,
                                                           IEnumerable<String>        SharedSubprotocols,
                                                           EventTracking_Id           EventTrackingId,
                                                           CancellationToken          CancellationToken)
        {

            if (Connection.HTTPRequest is null)
                return;

            NetworkingNode_Id? networkingNodeId = null;

            #region Parse TLS Client Certificate CommonName, or...

            // We already validated and therefore trust this certificate!
            if (Connection.HTTPRequest.ClientCertificate is not null)
            {

                var x509CommonName = Connection.HTTPRequest.ClientCertificate.GetNameInfo(X509NameType.SimpleName, forIssuer: false);

                if (NetworkingNode_Id.TryParse(x509CommonName, out var networkingNodeId1))
                {
                    networkingNodeId = networkingNodeId1;
                }

            }

            #endregion

            #region ...check HTTP Basic Authentication, or...

            else if (Connection.HTTPRequest.Authorization is HTTPBasicAuthentication httpBasicAuthentication &&
                     NetworkingNode_Id.TryParse(httpBasicAuthentication.Username, out var networkingNodeId2))
            {
                networkingNodeId = networkingNodeId2;
            }

            #endregion


            //ToDo: This might be a DOS attack vector!

            #region ...try to get the NetworkingNodeId from the HTTP request path suffix

            else
            {

                var path = Connection.HTTPRequest.Path.ToString();

                if (NetworkingNode_Id.TryParse(path[(path.LastIndexOf('/') + 1)..],
                    out var networkingNodeId3))
                {
                    networkingNodeId = networkingNodeId3;
                }

            }

            #endregion


            if (networkingNodeId.HasValue)
            {

                #region Store the NetworkingNodeId within the HTTP Web Socket connection

                Connection.TryAddCustomData(
                                NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey,
                                networkingNodeId.Value
                            );

                #endregion

                #region Register new Networking Node

                if (!connectedNetworkingNodes.TryAdd(networkingNodeId.Value,
                                                     new Tuple<WebSocketServerConnection, DateTime>(
                                                         Connection,
                                                         Timestamp.Now
                                                     )))
                {

                    DebugX.Log($"{nameof(OCPPWebSocketServer)} Duplicate networking node '{networkingNodeId.Value}' detected: Trying to close old one!");

                    if (connectedNetworkingNodes.TryRemove(networkingNodeId.Value, out var oldConnection))
                    {
                        try
                        {
                            await oldConnection.Item1.Close(
                                      WebSocketFrame.ClosingStatusCode.NormalClosure,
                                      "Newer connection detected!",
                                      CancellationToken
                                  );
                        }
                        catch (Exception e)
                        {
                            DebugX.Log($"{nameof(OCPPWebSocketServer)} Closing old HTTP Web Socket connection from {oldConnection.Item1.RemoteSocket} failed: {e.Message}");
                        }
                    }

                    connectedNetworkingNodes.TryAdd(networkingNodeId.Value,
                                                    new Tuple<WebSocketServerConnection, DateTime>(
                                                        Connection,
                                                        Timestamp.Now
                                                    ));

                }

                #endregion


                #region Send OnNewNetworkingNodeWSConnection event

                var onNetworkingNodeNewWebSocketConnection = OnNetworkingNodeNewWebSocketConnection;
                if (onNetworkingNodeNewWebSocketConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNetworkingNodeNewWebSocketConnection.GetInvocationList().
                                               OfType<OnNetworkingNodeNewWebSocketConnectionDelegate>().
                                               Select(loggingDelegate => loggingDelegate.Invoke(
                                                                             LogTimestamp,
                                                                             this,
                                                                             Connection,
                                                                             networkingNodeId.Value,
                                                                             SharedSubprotocols,
                                                                             EventTrackingId,
                                                                             CancellationToken
                                                                         )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(OCPPWebSocketServer),
                                  nameof(OnNetworkingNodeNewWebSocketConnection),
                                  e
                              );
                    }

                }

                #endregion

            }

            #region Close connection

            else
            {

                DebugX.Log($"{nameof(OCPPWebSocketServer)} Could not get NetworkingNodeId from HTTP Web Socket connection ({Connection.RemoteSocket}): Closing connection!");

                try
                {
                    await Connection.Close(
                              WebSocketFrame.ClosingStatusCode.PolicyViolation,
                              "Could not get NetworkingNodeId from HTTP Web Socket connection!",
                              CancellationToken
                          );
                }
                catch (Exception e)
                {
                    DebugX.Log($"{nameof(OCPPWebSocketServer)} Closing HTTP Web Socket connection ({Connection.RemoteSocket}) failed: {e.Message}");
                }

            }

            #endregion

        }

        #endregion

        #region (protected) ProcessCloseMessage           (LogTimestamp, Server, Connection, EventTrackingId, StatusCode, Reason, CancellationToken)

        protected async Task ProcessCloseMessage(DateTime                          LogTimestamp,
                                                 IWebSocketServer                  Server,
                                                 WebSocketServerConnection         Connection,
                                                 EventTracking_Id                  EventTrackingId,
                                                 WebSocketFrame.ClosingStatusCode  StatusCode,
                                                 String?                           Reason,
                                                 CancellationToken                 CancellationToken)
        {

            if (Connection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey, out var networkingNodeId))
            {

                connectedNetworkingNodes.TryRemove(networkingNodeId, out _);

                #region Send OnNetworkingNodeCloseMessageReceived event

                var logger = OnNetworkingNodeCloseMessageReceived;
                if (logger is not null)
                {

                    try
                    {
                        await Task.WhenAll(logger.GetInvocationList().
                                                  OfType<OnNetworkingNodeCloseMessageReceivedDelegate>().
                                                  Select(loggingDelegate => loggingDelegate.Invoke(LogTimestamp,
                                                                                                   this,
                                                                                                   Connection,
                                                                                                   networkingNodeId,
                                                                                                   EventTrackingId,
                                                                                                   StatusCode,
                                                                                                   Reason,
                                                                                                   CancellationToken)).
                                                  ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(OCPPWebSocketServer),
                                  nameof(OnNetworkingNodeCloseMessageReceived),
                                  e
                              );
                    }

                }

                #endregion

            }

        }

        #endregion


        // Receive data...

        #region (protected) ProcessTextMessage   (RequestTimestamp, ServerConnection, TextMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="ServerConnection">The WebSocket connection.</param>
        /// <param name="TextMessage">The received text message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime                   RequestTimestamp,
                                                                                    WebSocketServerConnection  ServerConnection,
                                                                                    String                     TextMessage,
                                                                                    EventTracking_Id           EventTrackingId,
                                                                                    CancellationToken          CancellationToken)
        {

            OCPP_JSONResponseMessage?     OCPPResponse       = null;
            OCPP_JSONRequestErrorMessage? OCPPErrorResponse  = null;

            try
            {

                var jsonArray            = JArray.Parse(TextMessage);
                var sourceNodeId         = ServerConnection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey);

                var textMessageResponse  = await OCPPAdapter.IN.ProcessJSONMessage(
                                                     RequestTimestamp,
                                                     ServerConnection,
                                                     jsonArray,
                                                     EventTrackingId,
                                                     CancellationToken
                                                 );

                return textMessageResponse;

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.InternalError(
                                        nameof(OCPPWebSocketServer),
                                        EventTrackingId,
                                        TextMessage,
                                        e
                                    );

            }

            return null;

        }

        #endregion

        #region (protected) ProcessBinaryMessage (RequestTimestamp, ServerConnection, BinaryMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="BinaryMessage">The received binary message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime RequestTimestamp,
                                                                                        WebSocketServerConnection ServerConnection,
                                                                                        Byte[] BinaryMessage,
                                                                                        EventTracking_Id EventTrackingId,
                                                                                        CancellationToken CancellationToken)
        {

            OCPP_BinaryResponseMessage? OCPPResponse       = null;
            OCPP_JSONRequestErrorMessage?      OCPPErrorResponse  = null;

            try
            {

                var sourceNodeId = ServerConnection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey);

                var textMessageResponse = await OCPPAdapter.IN.ProcessBinaryMessage(RequestTimestamp,
                                                                                    ServerConnection,
                                                                                    BinaryMessage,
                                                                                    EventTrackingId,
                                                                                    CancellationToken);

                return textMessageResponse;

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.InternalError(
                                        nameof(OCPPWebSocketServer),
                                        EventTrackingId,
                                        BinaryMessage,
                                        e
                                    );

            }

            return null;

        }

        #endregion


        // Send data...

        #region SendJSONRequest       (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON OCPP request message.</param>
        public async Task<SendOCPPMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(JSONRequestMessage.DestinationId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    JSONRequestMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                    var ocppTextMessage = JSONRequestMessage.ToJSON().ToString(Formatting.None);


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendTextMessage(
                                                            webSocketConnection.Item1,
                                                            ocppTextMessage,
                                                            JSONRequestMessage.EventTrackingId,
                                                            JSONRequestMessage.CancellationToken
                                                        ))
                        {

                            //requests.TryAdd(RequestMessage.RequestId,
                            //                SendRequestState.FromJSONRequest(
                            //                    Timestamp.Now,
                            //                    RequestMessage.DestinationNodeId,
                            //                    RequestMessage.RequestTimeout ?? (RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout)),
                            //                    RequestMessage
                            //                ));

                            #region OnJSONMessageRequestSent

                            //var onJSONMessageRequestSent = OnJSONMessageRequestSent;
                            //if (onJSONMessageRequestSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onJSONMessageRequestSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageRequestSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion

        #region SendJSONResponse      (JSONResponseMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP response message.
        /// </summary>
        /// <param name="JSONResponseMessage">A JSON OCPP response message.</param>
        public async Task<SendOCPPMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(JSONResponseMessage.DestinationId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    JSONResponseMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                    var ocppTextMessage = JSONResponseMessage.ToJSON().ToString(Formatting.None);


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendTextMessage(
                                                            webSocketConnection.Item1,
                                                            ocppTextMessage,
                                                            JSONResponseMessage.EventTrackingId,
                                                            JSONResponseMessage.CancellationToken
                                                        ))
                        {

                            //responses.TryAdd(ResponseMessage.ResponseId,
                            //                SendResponseState.FromJSONResponse(
                            //                    Timestamp.Now,
                            //                    ResponseMessage.DestinationNodeId,
                            //                    ResponseMessage.ResponseTimeout ?? (ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout)),
                            //                    ResponseMessage
                            //                ));

                            #region OnJSONMessageResponseSent

                            //var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                            //if (onJSONMessageResponseSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageResponseSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion

        #region SendJSONRequestError  (JSONRequestErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP error message.
        /// </summary>
        /// <param name="JSONRequestErrorMessage">A JSON OCPP error message.</param>
        public async Task<SendOCPPMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(JSONRequestErrorMessage.DestinationNodeId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    JSONRequestErrorMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                    var ocppTextMessage = JSONRequestErrorMessage.ToJSON().ToString(Formatting.None);


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendTextMessage(
                                                            webSocketConnection.Item1,
                                                            ocppTextMessage,
                                                            JSONRequestErrorMessage.EventTrackingId,
                                                            JSONRequestErrorMessage.CancellationToken
                                                        ))
                        {

                            //responses.TryAdd(ResponseMessage.ResponseId,
                            //                SendResponseState.FromJSONResponse(
                            //                    Timestamp.Now,
                            //                    ResponseMessage.DestinationNodeId,
                            //                    ResponseMessage.ResponseTimeout ?? (ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout)),
                            //                    ResponseMessage
                            //                ));

                            #region OnJSONMessageResponseSent

                            //var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                            //if (onJSONMessageResponseSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageResponseSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion

        #region SendJSONResponseError (JSONResponseErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP error message.
        /// </summary>
        /// <param name="JSONResponseErrorMessage">A JSON OCPP error message.</param>
        public async Task<SendOCPPMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(JSONResponseErrorMessage.DestinationNodeId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    JSONResponseErrorMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                    var ocppTextMessage = JSONResponseErrorMessage.ToJSON().ToString(Formatting.None);


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendTextMessage(
                                                            webSocketConnection.Item1,
                                                            ocppTextMessage,
                                                            JSONResponseErrorMessage.EventTrackingId,
                                                            JSONResponseErrorMessage.CancellationToken
                                                        ))
                        {

                            //responses.TryAdd(ResponseMessage.ResponseId,
                            //                SendResponseState.FromJSONResponse(
                            //                    Timestamp.Now,
                            //                    ResponseMessage.DestinationNodeId,
                            //                    ResponseMessage.ResponseTimeout ?? (ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout)),
                            //                    ResponseMessage
                            //                ));

                            #region OnJSONMessageResponseSent

                            //var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                            //if (onJSONMessageResponseSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageResponseSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion


        #region SendBinaryRequest     (BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP request message.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary OCPP request message.</param>
        public async Task<SendOCPPMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(BinaryRequestMessage.DestinationId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    BinaryRequestMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                    var ocppBinaryMessage = BinaryRequestMessage.ToByteArray();


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendBinaryMessage(
                                                            webSocketConnection.Item1,
                                                            ocppBinaryMessage,
                                                            BinaryRequestMessage.EventTrackingId,
                                                            BinaryRequestMessage.CancellationToken
                                                        ))
                        {

                            //requests.TryAdd(RequestMessage.RequestId,
                            //                SendRequestState.FromJSONRequest(
                            //                    Timestamp.Now,
                            //                    RequestMessage.DestinationNodeId,
                            //                    RequestMessage.RequestTimeout ?? (RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout)),
                            //                    RequestMessage
                            //                ));

                            #region OnBinaryMessageRequestSent

                            //var onBinaryMessageRequestSent = OnBinaryMessageRequestSent;
                            //if (onBinaryMessageRequestSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onBinaryMessageRequestSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnBinaryMessageRequestSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion

        #region SendBinaryResponse    (BinaryResponseMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP response message.
        /// </summary>
        /// <param name="BinaryResponseMessage">A binary OCPP response message.</param>
        public async Task<SendOCPPMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(BinaryResponseMessage.DestinationId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey);

                    BinaryResponseMessage.NetworkingMode = webSocketConnections.First().Item2;
                    //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                    var ocppBinaryMessage = BinaryResponseMessage.ToByteArray();


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendBinaryMessage(
                                                            webSocketConnection.Item1,
                                                            ocppBinaryMessage,
                                                            BinaryResponseMessage.EventTrackingId,
                                                            BinaryResponseMessage.CancellationToken
                                                        ))
                        {

                            //responses.TryAdd(ResponseMessage.ResponseId,
                            //                SendResponseState.FromJSONResponse(
                            //                    Timestamp.Now,
                            //                    ResponseMessage.DestinationNodeId,
                            //                    ResponseMessage.ResponseTimeout ?? (ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout)),
                            //                    ResponseMessage
                            //                ));

                            #region OnBinaryMessageResponseSent

                            //var onBinaryMessageResponseSent = OnBinaryMessageResponseSent;
                            //if (onBinaryMessageResponseSent is not null)
                            //{
                            //    try
                            //    {

                            //        await Task.WhenAll(onBinaryMessageResponseSent.GetInvocationList().
                            //                               OfType<OnWebSocketTextMessageDelegate>().
                            //                               Select(loggingDelegate => loggingDelegate.Invoke(
                            //                                                              Timestamp.Now,
                            //                                                              this,
                            //                                                              webSocketConnection.Item1,
                            //                                                              EventTrackingId,
                            //                                                              ocppTextMessage,
                            //                                                              CancellationToken
                            //                                                          )).
                            //                               ToArray());

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnBinaryMessageResponseSent));
                            //    }
                            //}

                            #endregion

                            break;

                        }

                        RemoveConnection(webSocketConnection.Item1);

                    }

                    return SendOCPPMessageResult.Success;

                }
                else
                    return SendOCPPMessageResult.UnknownClient;

            }
            catch (Exception)
            {
                return SendOCPPMessageResult.TransmissionFailed;
            }

        }

        #endregion



        private IEnumerable<Tuple<WebSocketServerConnection, NetworkingMode>> LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId)
        {

            if (NetworkingNodeId == NetworkingNode_Id.Zero)
                return Array.Empty<Tuple<WebSocketServerConnection, NetworkingMode>>();

            var lookUpNetworkingNodeId = NetworkingNodeId;

            if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
            {
                lookUpNetworkingNodeId = networkingHubId;
                return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey) == lookUpNetworkingNodeId).
                    Select(x => new Tuple<WebSocketServerConnection, NetworkingMode>(x, NetworkingMode.OverlayNetwork));
            }

            return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey) == lookUpNetworkingNodeId).
                                        Select(x => new Tuple<WebSocketServerConnection, NetworkingMode>(x, NetworkingMode.Standard));

        }

        public void AddStaticRouting(NetworkingNode_Id DestinationNodeId,
                                     NetworkingNode_Id NetworkingHubId)
        {

            reachableViaNetworkingHubs.TryAdd(DestinationNodeId,
                                              NetworkingHubId);

        }

        public void RemoveStaticRouting(NetworkingNode_Id DestinationNodeId,
                                        NetworkingNode_Id NetworkingHubId)
        {

            reachableViaNetworkingHubs.TryRemove(new KeyValuePair<NetworkingNode_Id, NetworkingNode_Id>(DestinationNodeId, NetworkingHubId));

        }



        #region (protected) HandleErrors(Module, Caller, Exception, Description = null)
        protected Task HandleErrors(String     Module,
                                    String     Caller,
                                    Exception  Exception,
                                    String?    Description   = null)
        {

            DebugX.LogException(Exception, $"{Module}.{Caller}{(Description is not null ? $" {Description}" : "")}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
