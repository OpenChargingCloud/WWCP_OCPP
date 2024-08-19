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
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
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
                                                                                NetworkingNode_Id                  DestinationId,
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
                                                                                NetworkingNode_Id                  DestinationId,
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
                                                                                NetworkingNode_Id                  DestinationId,
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
        public  const           String                                                                               DefaultHTTPServiceName            = $"GraphDefined OCPP {Version.String} Web Socket Server";

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
        public OCPPAdapter                                       OCPPAdapter              { get; }

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

        ///// <summary>
        ///// The JSON formatting to use.
        ///// </summary>
        //public Formatting                                        JSONFormatting           { get; set; }
        //    = Formatting.None;

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


        /// <summary>
        /// An event sent whenever a JSON message was sent.
        /// </summary>
        public event     OnWebSocketServerJSONMessageSentDelegate?         OnJSONMessageSent;

        /// <summary>
        /// An event sent whenever a JSON message was received.
        /// </summary>
        public event     OnWebSocketServerJSONMessageReceivedDelegate?     OnJSONMessageReceived;


        /// <summary>
        /// An event sent whenever a binary message was sent.
        /// </summary>
        public new event OnWebSocketServerBinaryMessageSentDelegate?       OnBinaryMessageSent;

        /// <summary>
        /// An event sent whenever a binary message was received.
        /// </summary>
        public new event OnWebSocketServerBinaryMessageReceivedDelegate?   OnBinaryMessageReceived;

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
        public OCPPWebSocketServer(OCPPAdapter                                                     OCPPAdapter,

                                   String?                                                         HTTPServiceName              = DefaultHTTPServiceName,
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

                                   DNSClient?                                                      DNSClient                    = null,
                                   Boolean                                                         AutoStart                    = true)

            : base(IPAddress,
                   TCPPort               ?? IPPort.Auto,
                   HTTPServiceName       ?? DefaultHTTPServiceName,
                   Description,

                   SecWebSocketProtocols ?? [
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

            this.OCPPAdapter                     = OCPPAdapter;

            this.RequireAuthentication           = RequireAuthentication;

            //this.Logger                          = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                                LoggingPath,
            //                                                                                LoggingContext,
            //                                                                                LogfileCreator);

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
        public HTTPBasicAuthentication AddOrUpdateHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
                                                                String             Password)
        {

            NetworkingNodeLogins.AddOrUpdate(
                                     NetworkingNodeId,
                                     Password,
                                     (chargingStationId, password) => Password
                                 );

            return HTTPBasicAuthentication.Create(
                       NetworkingNodeId.ToString(),
                       Password
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

                DebugX.Log($"{nameof(OCPPWebSocketServer)} connection from {Connection.RemoteSocket}: Missing 'Sec-WebSocket-Protocol' HTTP header!");

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

                DebugX.Log($"{nameof(OCPPWebSocketServer)} connection from {Connection.RemoteSocket}: {error}");

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
                        DebugX.Log($"{nameof(OCPPWebSocketServer)} connection from {Connection.RemoteSocket} using authorization: '{basicAuthentication.Username}' / '{basicAuthentication.Password}'");
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log($"{nameof(OCPPWebSocketServer)} connection from {Connection.RemoteSocket} invalid authorization: '{basicAuthentication.Username}' / '{basicAuthentication.Password}'!");

                }
                else
                    DebugX.Log($"{nameof(OCPPWebSocketServer)} connection from {Connection.RemoteSocket} missing authorization!");

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

                #region Register new NetworkingNode

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


                #region Store the Networking Mode within the HTTP Web Socket connection

                if (Connection.HTTPRequest.TryGetHeaderField(NetworkingNode.OCPPAdapter.X_OCPP_NetworkingMode, out var networkingModeString) &&
                    Enum.TryParse<NetworkingMode>(networkingModeString?.ToString(), out var networkingMode))
                {
                    Connection.TryAddCustomData(
                                   NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey,
                                   networkingMode
                               );
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

            #region else: Close connection

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

        #region (protected) ProcessCloseMessage           (LogTimestamp, Server, Connection, Frame, EventTrackingId, StatusCode, Reason, CancellationToken)

        protected async Task ProcessCloseMessage(DateTime                          LogTimestamp,
                                                 IWebSocketServer                  Server,
                                                 WebSocketServerConnection         Connection,
                                                 WebSocketFrame                    Frame,
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

        #region (protected) ProcessTextMessage   (RequestTimestamp, WebSocketConnection, TextMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="WebSocketConnection">The WebSocket connection.</param>
        /// <param name="TextMessage">The received text message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime                   RequestTimestamp,
                                                                                    WebSocketServerConnection  WebSocketConnection,
                                                                                    String                     TextMessage,
                                                                                    EventTracking_Id           EventTrackingId,
                                                                                    CancellationToken          CancellationToken)
        {

            #region Initial checks

            if (TextMessage == "[]" ||
                TextMessage.IsNullOrEmpty())
            {

                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessTextMessage),
                          "Received an empty text message!"
                      );

                return new WebSocketTextMessageResponse(
                           RequestTimestamp,
                           TextMessage,
                           Timestamp.Now,
                           "Received an empty text message!",
                           EventTrackingId,
                           CancellationToken
                       );

            }

            #endregion

            WebSocketTextMessageResponse? textMessageResponse = null;

            try
            {

                var jsonMessage   = JArray.Parse(TextMessage);
                var sourceNodeId  = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey);

                await LogEvent(
                          OnJSONMessageReceived,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              WebSocketConnection,
                              EventTrackingId,
                              RequestTimestamp,
                              jsonMessage,
                              CancellationToken
                          )
                      );

                textMessageResponse = await OCPPAdapter.IN.ProcessJSONMessage(
                                                RequestTimestamp,
                                                WebSocketConnection,
                                                jsonMessage,
                                                EventTrackingId,
                                                CancellationToken
                                            );

            }
            catch (Exception e)
            {

                textMessageResponse = new WebSocketTextMessageResponse(
                                          RequestTimestamp,
                                          TextMessage,
                                          Timestamp.Now,
                                          $"[{nameof(OCPPWebSocketServer)}] {e.Message}",
                                          EventTrackingId,
                                          CancellationToken
                                      );

            }

            textMessageResponse ??= new WebSocketTextMessageResponse(
                                            RequestTimestamp,
                                            TextMessage,
                                            Timestamp.Now,
                                            "Unknown Error!",
                                            EventTrackingId,
                                            CancellationToken
                                        );

            return textMessageResponse;

        }

        #endregion

        #region (protected) ProcessBinaryMessage (RequestTimestamp, WebSocketConnection, BinaryMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="BinaryMessage">The received binary message.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime                   RequestTimestamp,
                                                                                        WebSocketServerConnection  WebSocketConnection,
                                                                                        Byte[]                     BinaryMessage,
                                                                                        EventTracking_Id           EventTrackingId,
                                                                                        CancellationToken          CancellationToken)
        {

            #region Initial checks

            if (BinaryMessage.Length == 0)
            {

                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessTextMessage),
                          "Received an empty binary message!"
                      );

                return new WebSocketBinaryMessageResponse(
                           RequestTimestamp,
                           BinaryMessage,
                           Timestamp.Now,
                           "Received an empty binary message!".ToUTF8Bytes(),
                           EventTrackingId,
                           CancellationToken
                       );

            }

            #endregion

            WebSocketBinaryMessageResponse? binaryMessageResponse = null;

            try
            {

                var sourceNodeId = WebSocketConnection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey);

                await LogEvent(
                          OnBinaryMessageReceived,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              WebSocketConnection,
                              EventTrackingId,
                              RequestTimestamp,
                              BinaryMessage,
                              CancellationToken
                          )
                      );

                binaryMessageResponse = await OCPPAdapter.IN.ProcessBinaryMessage(
                                                  RequestTimestamp,
                                                  WebSocketConnection,
                                                  BinaryMessage,
                                                  EventTrackingId,
                                                  CancellationToken
                                              );

            }
            catch (Exception e)
            {

                binaryMessageResponse = new WebSocketBinaryMessageResponse(
                                            RequestTimestamp,
                                            BinaryMessage,
                                            Timestamp.Now,
                                            $"[{nameof(OCPPWebSocketServer)}] {e.Message}".ToUTF8Bytes(),
                                            EventTrackingId,
                                            CancellationToken
                                        );

            }

            binaryMessageResponse ??= new WebSocketBinaryMessageResponse(
                                          RequestTimestamp,
                                          BinaryMessage,
                                          Timestamp.Now,
                                          "Unknown Error!".ToUTF8Bytes(),
                                          EventTrackingId,
                                          CancellationToken
                                      );

            return binaryMessageResponse;

        }

        #endregion


        // Send data...

        #region SendJSONRequest         (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON OCPP request message.</param>
        public async Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(JSONRequestMessage.Destination.Next))
                {

                    JSONRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(OCPPAdapter.NetworkingMode_WebSocketKey)
                                                            ?? NetworkingMode.Standard;

                    var jsonMessage  = JSONRequestMessage.ToJSON();

                    var sentStatus   = await SendTextMessage(
                                                 webSocketConnection,
                                                 jsonMessage.ToString(Formatting.None),
                                                 JSONRequestMessage.EventTrackingId,
                                                 JSONRequestMessage.CancellationToken
                                             );

                    await LogEvent(
                              OnJSONMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  JSONRequestMessage.EventTrackingId,
                                  JSONRequestMessage.RequestTimestamp,
                                  jsonMessage,
                                  sentStatus,
                                  JSONRequestMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendJSONResponse        (JSONResponseMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP response message.
        /// </summary>
        /// <param name="JSONResponseMessage">A JSON OCPP response message.</param>
        public async Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(JSONResponseMessage.Destination.Next))
                {

                    JSONResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(OCPPAdapter.NetworkingMode_WebSocketKey)
                                                             ?? NetworkingMode.Standard;

                    var jsonMessage  = JSONResponseMessage.ToJSON();

                    var sentStatus   = await SendTextMessage(
                                                 webSocketConnection,
                                                 jsonMessage.ToString(Formatting.None),
                                                 JSONResponseMessage.EventTrackingId,
                                                 JSONResponseMessage.CancellationToken
                                             );

                    await LogEvent(
                              OnJSONMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  JSONResponseMessage.EventTrackingId,
                                  JSONResponseMessage.ResponseTimestamp,
                                  jsonMessage,
                                  sentStatus,
                                  JSONResponseMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendJSONRequestError    (JSONRequestErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP error message.
        /// </summary>
        /// <param name="JSONRequestErrorMessage">A JSON OCPP error message.</param>
        public async Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(JSONRequestErrorMessage.Destination.Next))
                {

                    JSONRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                                 ?? NetworkingMode.Standard;

                    var jsonMessage  = JSONRequestErrorMessage.ToJSON();

                    var sentStatus   = await SendTextMessage(
                                                 webSocketConnection,
                                                 jsonMessage.ToString(Formatting.None),
                                                 JSONRequestErrorMessage.EventTrackingId,
                                                 JSONRequestErrorMessage.CancellationToken
                                             );

                    await LogEvent(
                              OnJSONMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  JSONRequestErrorMessage.EventTrackingId,
                                  JSONRequestErrorMessage.ResponseTimestamp,
                                  jsonMessage,
                                  sentStatus,
                                  JSONRequestErrorMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendJSONResponseError   (JSONResponseErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP error message.
        /// </summary>
        /// <param name="JSONResponseErrorMessage">A JSON OCPP error message.</param>
        public async Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(JSONResponseErrorMessage.Destination.Next))
                {

                    JSONResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                                  ?? NetworkingMode.Standard;

                    var jsonMessage  = JSONResponseErrorMessage.ToJSON();

                    var sentStatus   = await SendTextMessage(
                                                 webSocketConnection,
                                                 jsonMessage.ToString(Formatting.None),
                                                 JSONResponseErrorMessage.EventTrackingId,
                                                 JSONResponseErrorMessage.CancellationToken
                                             );

                    await LogEvent(
                              OnJSONMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  JSONResponseErrorMessage.EventTrackingId,
                                  JSONResponseErrorMessage.ResponseTimestamp,
                                  jsonMessage,
                                  sentStatus,
                                  JSONResponseErrorMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendJSONSendMessage     (JSONSendMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP send message.
        /// </summary>
        /// <param name="JSONSendMessage">A JSON OCPP send message.</param>
        public async Task<SentMessageResult> SendJSONSendMessage(OCPP_JSONSendMessage JSONSendMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(JSONSendMessage.Destination.Next))
                {

                    JSONSendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                         ?? NetworkingMode.Standard;

                    var jsonMessage  = JSONSendMessage.ToJSON();

                    var sentStatus   = await SendTextMessage(
                                                 webSocketConnection,
                                                 jsonMessage.ToString(Formatting.None),
                                                 JSONSendMessage.EventTrackingId,
                                                 JSONSendMessage.CancellationToken
                                             );

                    await LogEvent(
                              OnJSONMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  JSONSendMessage.EventTrackingId,
                                  JSONSendMessage.MessageTimestamp,
                                  jsonMessage,
                                  sentStatus,
                                  JSONSendMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion


        #region SendBinaryRequest       (BinaryRequestMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP request message.
        /// </summary>
        /// <param name="BinaryRequestMessage">A binary OCPP request message.</param>
        public async Task<SentMessageResult> SendBinaryRequest(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryRequestMessage.Destination.Next))
                {

                    BinaryRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                              ?? NetworkingMode.Standard;

                    var binaryMessage  = BinaryRequestMessage.ToByteArray();

                    var sentStatus     = await SendBinaryMessage(
                                                   webSocketConnection,
                                                   binaryMessage,
                                                   BinaryRequestMessage.EventTrackingId,
                                                   BinaryRequestMessage.CancellationToken
                                               );

                    await LogEvent(
                              OnBinaryMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  BinaryRequestMessage.EventTrackingId,
                                  BinaryRequestMessage.RequestTimestamp,
                                  binaryMessage,
                                  sentStatus,
                                  BinaryRequestMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendBinaryResponse      (BinaryResponseMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP response message.
        /// </summary>
        /// <param name="BinaryResponseMessage">A binary OCPP response message.</param>
        public async Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryResponseMessage.Destination.Next))
                {

                    BinaryResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                               ?? NetworkingMode.Standard;

                    var binaryMessage  = BinaryResponseMessage.ToByteArray();

                    var sentStatus     = await SendBinaryMessage(
                                                   webSocketConnection,
                                                   binaryMessage,
                                                   BinaryResponseMessage.EventTrackingId,
                                                   BinaryResponseMessage.CancellationToken
                                               );

                    await LogEvent(
                              OnBinaryMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  BinaryResponseMessage.EventTrackingId,
                                  BinaryResponseMessage.ResponseTimestamp,
                                  binaryMessage,
                                  sentStatus,
                                  BinaryResponseMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendBinaryRequestError  (BinaryRequestErrorMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP error message.
        /// </summary>
        /// <param name="BinaryRequestErrorMessage">A binary OCPP error message.</param>
        public async Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryRequestErrorMessage.Destination.Next))
                {

                    BinaryRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                                   ?? NetworkingMode.Standard;

                    var binaryMessage  = BinaryRequestErrorMessage.ToByteArray();

                    var sentStatus     = await SendBinaryMessage(
                                                   webSocketConnection,
                                                   binaryMessage,
                                                   BinaryRequestErrorMessage.EventTrackingId,
                                                   BinaryRequestErrorMessage.CancellationToken
                                               );

                    await LogEvent(
                              OnBinaryMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  BinaryRequestErrorMessage.EventTrackingId,
                                  BinaryRequestErrorMessage.ResponseTimestamp,
                                  binaryMessage,
                                  sentStatus,
                                  BinaryRequestErrorMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendBinaryResponseError (BinaryResponseErrorMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP error message.
        /// </summary>
        /// <param name="BinaryResponseErrorMessage">A binary OCPP error message.</param>
        public async Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryResponseErrorMessage.Destination.Next))
                {

                    BinaryResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                                    ?? NetworkingMode.Standard;

                    var binaryMessage  = BinaryResponseErrorMessage.ToByteArray();

                    var sentStatus     = await SendBinaryMessage(
                                                   webSocketConnection,
                                                   binaryMessage,
                                                   BinaryResponseErrorMessage.EventTrackingId,
                                                   BinaryResponseErrorMessage.CancellationToken
                                               );

                    await LogEvent(
                              OnBinaryMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  BinaryResponseErrorMessage.EventTrackingId,
                                  BinaryResponseErrorMessage.ResponseTimestamp,
                                  binaryMessage,
                                  sentStatus,
                                  BinaryResponseErrorMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion

        #region SendBinarySendMessage   (BinarySendMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP send message.
        /// </summary>
        /// <param name="BinarySendMessage">A binary OCPP send message.</param>
        public async Task<SentMessageResult> SendBinarySendMessage(OCPP_BinarySendMessage BinarySendMessage)
        {

            try
            {

                foreach (var webSocketConnection in LookupNetworkingNode(BinarySendMessage.Destination.Next))
                {

                    BinarySendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(NetworkingNode.OCPPAdapter.NetworkingMode_WebSocketKey)
                                                           ?? NetworkingMode.Standard;

                    var binaryMessage  = BinarySendMessage.ToByteArray();

                    var sentStatus     = await SendBinaryMessage(
                                                   webSocketConnection,
                                                   binaryMessage,
                                                   BinarySendMessage.EventTrackingId,
                                                   BinarySendMessage.CancellationToken
                                               );

                    await LogEvent(
                              OnBinaryMessageSent,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  this,
                                  webSocketConnection,
                                  BinarySendMessage.EventTrackingId,
                                  BinarySendMessage.MessageTimestamp,
                                  binaryMessage,
                                  sentStatus,
                                  BinarySendMessage.CancellationToken
                              )
                          );

                    if (sentStatus == SentStatus.Success)
                        return SentMessageResult.Success(webSocketConnection);

                    RemoveConnection(webSocketConnection);

                }

                return SentMessageResult.UnknownClient();

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e);
            }

        }

        #endregion



        private IEnumerable<WebSocketServerConnection> LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId)
        {

            if (NetworkingNodeId == NetworkingNode_Id.Zero)
                return [];

            var lookUpNetworkingNodeId = NetworkingNodeId;

            if (OCPPAdapter.LookupNetworkingNode(lookUpNetworkingNodeId, out var reachability) &&
                reachability is not null)
            {
                lookUpNetworkingNodeId = reachability.DestinationId;
            }

            if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
            {
                lookUpNetworkingNodeId = networkingHubId;
                return WebSocketConnections.Where (connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey) == lookUpNetworkingNodeId);
            }

            return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(NetworkingNode.OCPPAdapter.NetworkingNodeId_WebSocketKey) == lookUpNetworkingNodeId).ToArray();
                            //            Select(x => new Tuple<WebSocketServerConnection, NetworkingMode>(x, NetworkingNodeId == lookUpNetworkingNodeId ? NetworkingMode.Standard : NetworkingMode.OverlayNetwork));

        }

        public void AddStaticRouting(NetworkingNode_Id DestinationId,
                                     NetworkingNode_Id NetworkingHubId)
        {

            reachableViaNetworkingHubs.TryAdd(DestinationId,
                                              NetworkingHubId);

        }

        public void RemoveStaticRouting(NetworkingNode_Id DestinationId,
                                        NetworkingNode_Id NetworkingHubId)
        {

            reachableViaNetworkingHubs.TryRemove(new KeyValuePair<NetworkingNode_Id, NetworkingNode_Id>(DestinationId, NetworkingHubId));

        }



        #region (private) LogEvent(Logger, LogHandler, ...)

        private async Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                               Func<TDelegate, Task>                              LogHandler,
                                               [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                               [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

        {
            if (Logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              Logger.GetInvocationList().
                                     OfType<TDelegate>().
                                     Select(LogHandler)
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(nameof(OCPPWebSocketClient), $"{OCPPCommand}.{EventName}", e);
                }
            }
        }

        #endregion

        #region (private) HandleErrors(Module, Caller, ErrorResponse)

        private Task HandleErrors(String  Module,
                                  String  Caller,
                                  String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return Task.CompletedTask;

        }

        #endregion

        #region (private) HandleErrors(Module, Caller, ExceptionOccured)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
