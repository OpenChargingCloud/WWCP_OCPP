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
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using System.Text.Json.Nodes;

#endregion

namespace cloud.charging.open.protocols.OCPP.CSMS
{

    /// <summary>
    /// An OCPP HTTP Web Socket server runs on a CSMS or a networking node
    /// accepting connections from charging stations or other networking nodes
    /// to invoke OCPP commands.
    /// </summary>
    public abstract class AOCPPWebSocketServer : WebSocketServer,
                                                 ICSMSChannel
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const            String                                                                                DefaultHTTPServiceName            = $"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly  IPPort                                                                                DefaultHTTPServerPort             = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP server URI prefix.
        /// </summary>
        public static readonly  HTTPPath                                                                              DefaultURLPrefix                  = HTTPPath.Parse("/" + Version.String);

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly  TimeSpan                                                                              DefaultRequestTimeout             = TimeSpan.FromSeconds(30);

        protected readonly      Dictionary<String, MethodInfo>                                                        incomingMessageProcessorsLookup   = [];
        protected readonly      ConcurrentDictionary<NetworkingNode_Id, Tuple<WebSocketServerConnection, DateTime>>   connectedNetworkingNodes          = [];
        protected readonly      ConcurrentDictionary<NetworkingNode_Id, NetworkingNode_Id>                            reachableViaNetworkingHubs        = [];
        protected readonly      ConcurrentDictionary<Request_Id, SendRequestState>                                    requests                          = [];

        public    const         String                                                                                networkingNodeId_WebSocketKey     = "networkingNodeId";
        public    const         String                                                                                networkingMode_WebSocketKey       = "networkingMode";
        public    const         String                                                                                LogfileName                       = "CSMSWSServer.log";

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => HTTPServiceName;

        /// <summary>
        /// The enumeration of all connected networking nodes.
        /// </summary>
        public IEnumerable<NetworkingNode_Id> NetworkingNodeIds
            => connectedNetworkingNodes.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all networking nodes.
        /// </summary>
        public Boolean                                            RequireAuthentication    { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public ConcurrentDictionary<NetworkingNode_Id, String?>   NetworkingNodeLogins     { get; }
            = new();

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                                         JSONFormatting           { get; set; }
            = Formatting.None;

        /// <summary>
        /// The request timeout for messages sent by this HTTP WebSocket server.
        /// </summary>
        public TimeSpan?                                          RequestTimeout           { get; set; }

        #endregion

        #region Events

        #region Common Connection Management

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnCSMSNewWebSocketConnectionDelegate?    OnCSMSNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCSMSCloseMessageReceivedDelegate?      OnCSMSCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnCSMSTCPConnectionClosedDelegate?       OnCSMSTCPConnectionClosed;

        #endregion

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a text message was sent.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a text message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a text message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a text message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a text message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<OCPP.Signature>?                                       CustomBinarySignatureSerializer                              { get; set; }

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
        public AOCPPWebSocketServer(IEnumerable<String>                  SupportedOCPPWebSocketSubprotocols,
                                    String                               HTTPServiceName              = DefaultHTTPServiceName,
                                    IIPAddress?                          IPAddress                    = null,
                                    IPPort?                              TCPPort                      = null,

                                    Boolean                              RequireAuthentication        = true,
                                    Boolean                              DisableWebSocketPings        = false,
                                    TimeSpan?                            WebSocketPingEvery           = null,
                                    TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                    ServerCertificateSelectorDelegate?   ServerCertificateSelector    = null,
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

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   HTTPServiceName,

                   SupportedOCPPWebSocketSubprotocols,
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
                   false)

        {

            this.RequireAuthentication           = RequireAuthentication;

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


        // Connection management...

        #region (protected) ValidateTCPConnection        (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private Task<ConnectionFilterResponse> ValidateTCPConnection(DateTime                      LogTimestamp,
                                                                     IWebSocketServer              Server,
                                                                     System.Net.Sockets.TcpClient  Connection,
                                                                     EventTracking_Id              EventTrackingId,
                                                                     CancellationToken             CancellationToken)
        {

            return Task.FromResult(ConnectionFilterResponse.Accepted());

        }

        #endregion

        #region (protected) ValidateWebSocketConnection  (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

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

                DebugX.Log("Missing 'Sec-WebSocket-Protocol' HTTP header!");

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

                var error = $"This WebSocket service only supports {(SecWebSocketProtocols.Select(id => $"'{id}'").AggregateWith(", "))}!";

                DebugX.Log(error);

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
                        DebugX.Log(nameof(AOCPPWebSocketServer), " connection from " + Connection.RemoteSocket + " using authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);
                        return Task.FromResult<HTTPResponse?>(null);
                    }
                    else
                        DebugX.Log(nameof(AOCPPWebSocketServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(AOCPPWebSocketServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return Task.FromResult<HTTPResponse?>(
                           new HTTPResponse.Builder() {
                               HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                               Server          = HTTPServiceName,
                               Date            = Timestamp.Now,
                               Connection      = "close"
                           }.AsImmutable);

            }

            #endregion

            return Task.FromResult<HTTPResponse?>(null);

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, SharedSubprotocols, CancellationToken)

        protected async Task ProcessNewWebSocketConnection(DateTime                   LogTimestamp,
                                                           IWebSocketServer           Server,
                                                           WebSocketServerConnection  Connection,
                                                           EventTracking_Id           EventTrackingId,
                                                           IEnumerable<String>        SharedSubprotocols,
                                                           CancellationToken          CancellationToken)
        {

            #region Store the networking node/charging station identification within the Web Socket connection

            if (!Connection.TryGetCustomDataAs(networkingNodeId_WebSocketKey, out NetworkingNode_Id networkingNodeId) &&
                 Connection.HTTPRequest is not null)
            {

                //ToDo: TLS certificates

                #region HTTP Basic Authentication is used

                if (Connection.HTTPRequest.Authorization is HTTPBasicAuthentication httpBasicAuthentication)
                {

                    if (NetworkingNode_Id.TryParse(httpBasicAuthentication.Username, out networkingNodeId))
                    {

                        // Add the networking node/charging station identification to the Web Socket connection
                        Connection.TryAddCustomData(networkingNodeId_WebSocketKey, networkingNodeId);

                        if (!connectedNetworkingNodes.TryGetValue(networkingNodeId, out var value))
                            connectedNetworkingNodes.TryAdd(networkingNodeId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                        else
                        {

                            DebugX.Log($"{nameof(AOCPPWebSocketServer)} Duplicate networking node '{networkingNodeId}' detected!");

                            var oldNetworkingNode_WebSocketConnection = value.Item1;

                            connectedNetworkingNodes.TryRemove(networkingNodeId, out _);
                            connectedNetworkingNodes.TryAdd   (networkingNodeId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                            try
                            {
                                oldNetworkingNode_WebSocketConnection.Close();
                            }
                            catch (Exception e)
                            {
                                DebugX.Log($"{nameof(AOCPPWebSocketServer)} Closing old HTTP WebSocket connection failed: {e.Message}");
                            }

                        }

                    }

                }

                #endregion

                #region No authentication at all...

                else if (NetworkingNode_Id.TryParse(Connection.HTTPRequest.Path.ToString()[(Connection.HTTPRequest.Path.ToString().LastIndexOf("/") + 1)..], out networkingNodeId))
                {

                    // Add the charging station identification to the WebSocket connection
                    Connection.TryAddCustomData(networkingNodeId_WebSocketKey, networkingNodeId);

                    if (!connectedNetworkingNodes.TryGetValue(networkingNodeId, out Tuple<WebSocketServerConnection, DateTime>? value))
                         connectedNetworkingNodes.TryAdd(networkingNodeId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                    else
                    {

                        DebugX.Log($"{nameof(AOCPPWebSocketServer)} Duplicate charging station '{networkingNodeId}' detected!");

                        var oldChargingStation_WebSocketConnection = value.Item1;

                        connectedNetworkingNodes.TryRemove(networkingNodeId, out _);
                        connectedNetworkingNodes.TryAdd   (networkingNodeId, new Tuple<WebSocketServerConnection, DateTime>(Connection, Timestamp.Now));

                        try
                        {
                            oldChargingStation_WebSocketConnection.Close();
                        }
                        catch (Exception e)
                        {
                            DebugX.Log($"{nameof(AOCPPWebSocketServer)} Closing old HTTP WebSocket connection failed: {e.Message}");
                        }

                    }

                }

                #endregion

            }

            #endregion

            #region Send OnNewCSMSWSConnection event

            var logger = OnCSMSNewWebSocketConnection;
            if (logger is not null)
            {

                var loggerTasks = logger.GetInvocationList().
                                         OfType <OnCSMSNewWebSocketConnectionDelegate>().
                                         Select (loggingDelegate => loggingDelegate.Invoke(LogTimestamp,
                                                                                           this,
                                                                                           Connection,
                                                                                           networkingNodeId,
                                                                                           EventTrackingId,
                                                                                           SharedSubprotocols,
                                                                                           CancellationToken)).
                                         ToArray();

                try
                {
                    await Task.WhenAll(loggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(AOCPPWebSocketServer),
                              nameof(OnCSMSNewWebSocketConnection),
                              e
                          );
                }

            }

            #endregion

        }

        #endregion

        #region (protected) ProcessCloseMessage          (LogTimestamp, Server, Connection, EventTrackingId, StatusCode, Reason, CancellationToken)

        protected async Task ProcessCloseMessage(DateTime                          LogTimestamp,
                                                 IWebSocketServer                  Server,
                                                 WebSocketServerConnection         Connection,
                                                 EventTracking_Id                  EventTrackingId,
                                                 WebSocketFrame.ClosingStatusCode  StatusCode,
                                                 String?                           Reason,
                                                 CancellationToken                 CancellationToken)
        {

            if (Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey, out var networkingNodeId))
            {

                connectedNetworkingNodes.TryRemove(networkingNodeId, out _);

                #region Send OnCSMSCloseMessageReceived event

                var logger = OnCSMSCloseMessageReceived;
                if (logger is not null)
                {

                    var loggerTasks = logger.GetInvocationList().
                                             OfType <OnCSMSCloseMessageReceivedDelegate>().
                                             Select (loggingDelegate => loggingDelegate.Invoke(LogTimestamp,
                                                                                               this,
                                                                                               Connection,
                                                                                               networkingNodeId,
                                                                                               EventTrackingId,
                                                                                               StatusCode,
                                                                                               Reason,
                                                                                               CancellationToken)).
                                             ToArray();

                    try
                    {
                        await Task.WhenAll(loggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(AOCPPWebSocketServer),
                                  nameof(OnCSMSCloseMessageReceived),
                                  e
                              );
                    }

                }

                #endregion

            }

        }

        #endregion


        // Receive data...

        #region (protected) ProcessTextMessage           (RequestTimestamp, Connection, TextMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="TextMessage">The received text message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime                   RequestTimestamp,
                                                                                    WebSocketServerConnection  Connection,
                                                                                    String                     TextMessage,
                                                                                    EventTracking_Id           EventTrackingId,
                                                                                    CancellationToken          CancellationToken)
        {

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                var jsonArray     = JArray.Parse(TextMessage);
                var sourceNodeId  = Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey);

                if      (OCPP_JSONRequestMessage. TryParse(jsonArray, out var jsonRequest,  out var requestParsingError,  RequestTimestamp, EventTrackingId, sourceNodeId, CancellationToken) && jsonRequest       is not null)
                {

                    #region OnTextMessageRequestReceived

                    var onJSONMessageRequestReceived = OnJSONMessageRequestReceived;
                    if (onJSONMessageRequestReceived is not null)
                    {
                        try
                        {

                            await Task.WhenAll(onJSONMessageRequestReceived.GetInvocationList().
                                                   OfType <OnWebSocketJSONMessageRequestDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  Connection,
                                                                                  jsonRequest.DestinationNodeId,
                                                                                  jsonRequest.NetworkPath,
                                                                                  EventTrackingId,
                                                                                  Timestamp.Now,
                                                                                  jsonArray,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageRequestReceived));
                        }
                    }

                    #endregion

                    #region Try to call the matching 'incoming message processor'...

                    if (incomingMessageProcessorsLookup.TryGetValue(jsonRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                        //ToDo: Maybe this could be done via code generation!
                        var result = methodInfo.Invoke(this,
                                                       [ jsonRequest.RequestTimestamp,
                                                         Connection,
                                                         jsonRequest.DestinationNodeId,
                                                         jsonRequest.NetworkPath,
                                                         jsonRequest.EventTrackingId,
                                                         jsonRequest.RequestId,
                                                         jsonRequest.Payload,
                                                         jsonRequest.CancellationToken ]);

                        if (result is Task<Tuple<OCPP_JSONResponseMessage?, OCPP_JSONErrorMessage?>> textProcessor) {
                            (OCPPResponse, OCPPErrorResponse) = await textProcessor;
                        }

                        else
                            DebugX.Log($"Received undefined '{jsonRequest.Action}' JSON request message handler within {nameof(AOCPPWebSocketServer)}!");

                    }

                    #endregion

                    #region ...or error!

                    else
                    {

                        DebugX.Log($"Received unknown '{jsonRequest.Action}' JSON request message handler within {nameof(AOCPPWebSocketServer)}!");

                        OCPPErrorResponse = new OCPP_JSONErrorMessage(
                                                Timestamp.Now,
                                                EventTracking_Id.New,
                                                jsonRequest.RequestId,
                                                ResultCode.ProtocolError,
                                                $"The OCPP message '{jsonRequest.Action}' is unkown!",
                                                new JObject(
                                                    new JProperty("request", TextMessage)
                                                )
                                            );

                    }

                    #endregion


                    #region OnJSONMessageResponseSent

                    var now = Timestamp.Now;

                    if (OCPPResponse is not null || OCPPErrorResponse is not null)
                    {

                        var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                        if (onJSONMessageResponseSent is not null)
                        {
                            try
                            {

                                await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                                                       OfType <OnWebSocketJSONMessageResponseDelegate>().
                                                       Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      now,
                                                                                      this,
                                                                                      Connection,
                                                                                      jsonRequest.DestinationNodeId,
                                                                                      jsonRequest.NetworkPath,
                                                                                      EventTrackingId,
                                                                                      RequestTimestamp,
                                                                                      jsonArray,
                                                                                      [],
                                                                                      now,
                                                                                      //ToDo: For some use cases returning an error to a charging station might be useless!
                                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? [],
                                                                                      CancellationToken
                                                                                  )).
                                                       ToArray());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageResponseSent));
                            }
                        }

                    }

                    #endregion

                }

                else if (OCPP_JSONResponseMessage.TryParse(jsonArray, out var jsonResponse, out var responseParsingError, sourceNodeId) && jsonResponse      is not null)
                {

                    if (requests.TryGetValue(jsonResponse.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.JSONResponse       = jsonResponse;

                        #region OnJSONMessageResponseReceived

                        var onJSONMessageResponseReceived = OnJSONMessageResponseReceived;
                        if (onJSONMessageResponseReceived is not null)
                        {
                            try
                            {

                                await Task.WhenAll(onJSONMessageResponseReceived.GetInvocationList().
                                                       OfType <OnWebSocketJSONMessageResponseDelegate>().
                                                       Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      Timestamp.Now,
                                                                                      this,
                                                                                      Connection,
                                                                                      jsonResponse.DestinationNodeId,
                                                                                      jsonResponse.NetworkPath,
                                                                                      EventTrackingId,
                                                                                      sendRequestState.RequestTimestamp,
                                                                                      sendRequestState.JSONRequest?.  ToJSON()      ?? [],
                                                                                      sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                                                                                      Timestamp.Now,
                                                                                      sendRequestState.JSONResponse.  ToJSON(),
                                                                                      CancellationToken
                                                                                  )).
                                                       ToArray());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageResponseReceived));
                            }
                        }

                        #endregion

                    }

                    else
                        DebugX.Log($"Received an unknown OCPP response with identificaiton '{jsonResponse.RequestId}' within {nameof(AOCPPWebSocketServer)}:{Environment.NewLine}'{TextMessage}'!");

                    // No response to the charging station!

                }

                else if (OCPP_JSONErrorMessage.   TryParse(jsonArray, out var jsonErrorResponse,                          sourceNodeId) && jsonErrorResponse is not null)
                {

                    if (requests.TryGetValue(jsonErrorResponse.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        // ToDo: Refactor 
                        if (ResultCode.TryParse(jsonArray[2]?.Value<String>() ?? "", out var errorCode))
                            sendRequestState.ErrorCode = errorCode;
                        else
                            sendRequestState.ErrorCode = ResultCode.GenericError;

                        sendRequestState.JSONResponse      = null;
                        sendRequestState.ErrorDescription  = jsonArray[3]?.Value<String>();
                        sendRequestState.ErrorDetails      = jsonArray[4] as JObject;

                        #region OnJSONErrorResponseReceived

                        var onJSONErrorResponseReceived = OnJSONErrorResponseReceived;
                        if (onJSONErrorResponseReceived is not null)
                        {
                            try
                            {

                                await Task.WhenAll(onJSONErrorResponseReceived.GetInvocationList().
                                                       OfType <OnWebSocketTextErrorResponseDelegate>().
                                                       Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      Timestamp.Now,
                                                                                      this,
                                                                                      Connection,
                                                                                      EventTrackingId,
                                                                                      sendRequestState.RequestTimestamp,
                                                                                      sendRequestState.JSONRequest?.  ToJSON().ToString(JSONFormatting) ?? "",
                                                                                      sendRequestState.BinaryRequest?.ToByteArray()                     ?? [],
                                                                                      Timestamp.Now,
                                                                                      sendRequestState.JSONResponse?. ToString() ?? "",
                                                                                      CancellationToken
                                                                                  )).
                                                       ToArray());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONErrorResponseReceived));
                            }
                        }

                        #endregion

                    }

                    // No response to the charging station!

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a JSON request message within {nameof(AOCPPWebSocketServer)}: '{requestParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a JSON response message within {nameof(AOCPPWebSocketServer)}: '{responseParsingError}'{Environment.NewLine}'{TextMessage}'!");

                else
                    DebugX.Log($"Received unknown text message within {nameof(AOCPPWebSocketServer)}: '{TextMessage}'!");

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.InternalError(
                                        nameof(AOCPPWebSocketServer),
                                        EventTrackingId,
                                        TextMessage,
                                        e
                                    );

            }


            #region OnJSONErrorResponseSent

            if (OCPPErrorResponse is not null)
            {

                var now = Timestamp.Now;

                var onJSONErrorResponseSent = OnJSONErrorResponseSent;
                if (onJSONErrorResponseSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONErrorResponseSent.GetInvocationList().
                                               OfType <OnWebSocketTextErrorResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              now,
                                                                              this,
                                                                              Connection,
                                                                              EventTrackingId,
                                                                              RequestTimestamp,
                                                                              TextMessage,
                                                                              [],
                                                                              now,
                                                                              (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting) ?? "",
                                                                              CancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONErrorResponseSent));
                    }
                }

            }

            #endregion


            // The response to the charging station... might be empty!
            return new WebSocketTextMessageResponse(
                       RequestTimestamp,
                       TextMessage,
                       Timestamp.Now,
                       (OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON())?.ToString(JSONFormatting) ?? String.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region (protected) ProcessBinaryMessage         (RequestTimestamp, Connection, BinaryMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this WebSocket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The WebSocket connection.</param>
        /// <param name="BinaryMessage">The received binary message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketBinaryMessageResponse> ProcessBinaryMessage(DateTime                   RequestTimestamp,
                                                                                        WebSocketServerConnection  Connection,
                                                                                        Byte[]                     BinaryMessage,
                                                                                        EventTracking_Id           EventTrackingId,
                                                                                        CancellationToken          CancellationToken)
        {

            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                var sourceNodeId = Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey);

                     if (OCPP_BinaryRequestMessage. TryParse(BinaryMessage, out var binaryRequest,  out var requestParsingError,  RequestTimestamp, EventTrackingId, sourceNodeId) && binaryRequest  is not null)
                {

                    #region OnBinaryMessageRequestReceived

                    var onBinaryMessageRequestReceived = OnBinaryMessageRequestReceived;
                    if (onBinaryMessageRequestReceived is not null)
                    {
                        try
                        {

                            await Task.WhenAll(onBinaryMessageRequestReceived.GetInvocationList().
                                                   OfType <OnWebSocketBinaryMessageRequestDelegate>().
                                                   Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  this,
                                                                                  Connection,
                                                                                  binaryRequest.DestinationNodeId,
                                                                                  binaryRequest.NetworkPath,
                                                                                  EventTrackingId,
                                                                                  Timestamp.Now,
                                                                                  BinaryMessage,
                                                                                  CancellationToken
                                                                              )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnBinaryMessageRequestReceived));
                        }
                    }

                    #endregion

                    #region Try to call the matching 'incoming message processor'

                    if (incomingMessageProcessorsLookup.TryGetValue(binaryRequest.Action, out var methodInfo) &&
                        methodInfo is not null)
                    {

                        var result = methodInfo.Invoke(this,
                                                       [ binaryRequest.RequestTimestamp,
                                                         Connection,
                                                         binaryRequest.DestinationNodeId,
                                                         binaryRequest.NetworkPath,
                                                         binaryRequest.EventTrackingId,
                                                         binaryRequest.RequestId,
                                                         binaryRequest.Payload,
                                                         binaryRequest.CancellationToken ]);

                        if (result is Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>> binaryProcessor)
                        {
                            (OCPPResponse, OCPPErrorResponse) = await binaryProcessor;
                        }

                        else
                            DebugX.Log($"Received undefined '{binaryRequest.Action}' binary request message handler within {nameof(AOCPPWebSocketServer)}!");

                    }

                    #endregion

                    else
                    {

                        DebugX.Log($"Received unknown '{binaryRequest.Action}' binary request message handler within {nameof(AOCPPWebSocketServer)}!");

                        OCPPErrorResponse = new OCPP_JSONErrorMessage(
                                                Timestamp.Now,
                                                EventTracking_Id.New,
                                                binaryRequest.RequestId,
                                                ResultCode.ProtocolError,
                                                $"The OCPP message '{binaryRequest.Action}' is unkown!",
                                                new JObject(
                                                    new JProperty("request", BinaryMessage.ToBase64())
                                                )
                                            );

                    }

                }

                else if (OCPP_BinaryResponseMessage.TryParse(BinaryMessage, out var binaryResponse, out var responseParsingError, sourceNodeId) && binaryResponse is not null)
                {

                    if (requests.TryGetValue(binaryResponse.RequestId, out var sendRequestState) &&
                        sendRequestState is not null)
                    {

                        #region Get LastHop

                        var lastHop            = Connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey) ?? NetworkingNode_Id.Zero;

                        if (binaryResponse.NetworkingMode == NetworkingMode.OverlayNetwork &&
                            binaryResponse.NetworkPath is not null &&
                            binaryResponse.NetworkPath.Last != NetworkingNode_Id.Zero)
                        {
                            lastHop            = binaryResponse.NetworkPath.Last;
                        }

                        #endregion

                        #region Append network path

                        // Note: Keep "var networkPath2", as "var networkPath" let to runtime errors!
                        var networkPath2        = binaryResponse.NetworkPath ?? NetworkPath.Empty;

                        if (networkPath2.Last != lastHop)
                            networkPath2        = networkPath2.Append(lastHop);

                        #endregion


                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.BinaryResponse     = binaryResponse;

                        #region OnBinaryMessageResponseReceived

                        var onBinaryMessageResponseReceived = OnBinaryMessageResponseReceived;
                        if (onBinaryMessageResponseReceived is not null)
                        {
                            try
                            {

                                await Task.WhenAll(onBinaryMessageResponseReceived.GetInvocationList().
                                                       OfType <OnWebSocketBinaryMessageResponseDelegate>().
                                                       Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      Timestamp.Now,
                                                                                      this,
                                                                                      Connection,
                                                                                      binaryResponse.DestinationNodeId,
                                                                                      networkPath2,
                                                                                      EventTrackingId,
                                                                                      sendRequestState.RequestTimestamp,
                                                                                      sendRequestState.JSONRequest?.  ToJSON()      ?? [],
                                                                                      sendRequestState.BinaryRequest?.ToByteArray() ?? [],
                                                                                      sendRequestState.ResponseTimestamp.Value,
                                                                                      sendRequestState.BinaryResponse.ToByteArray(),
                                                                                      CancellationToken
                                                                                  )).
                                                       ToArray());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnBinaryMessageResponseReceived));
                            }
                        }

                        #endregion

                    }

                    else
                        DebugX.Log(nameof(AOCPPWebSocketServer), " Received unknown binary OCPP response message!");

                }

                else if (requestParsingError  is not null)
                    DebugX.Log($"Failed to parse a binary request message within {nameof(AOCPPWebSocketServer)}: '{requestParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else if (responseParsingError is not null)
                    DebugX.Log($"Failed to parse a binary response message within {nameof(AOCPPWebSocketServer)}: '{responseParsingError}'{Environment.NewLine}'{BinaryMessage.ToBase64()}'!");

                else
                    DebugX.Log($"Received unknown binary message within {nameof(AOCPPWebSocketServer)}: '{BinaryMessage.ToBase64()}'!");

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.InternalError(
                                        nameof(AOCPPWebSocketServer),
                                        EventTrackingId,
                                        BinaryMessage,
                                        e
                                    );

            }


            return new WebSocketBinaryMessageResponse(
                       RequestTimestamp,
                       BinaryMessage,
                       Timestamp.Now,
                       OCPPResponse?.ToByteArray() ?? [],
                       EventTrackingId
                   );

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
                return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey) == lookUpNetworkingNodeId).
                    Select(x => new Tuple<WebSocketServerConnection, NetworkingMode>(x, NetworkingMode.OverlayNetwork));
            }

            return WebSocketConnections.Where (connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(networkingNodeId_WebSocketKey) == lookUpNetworkingNodeId).
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



        // Send data...

        #region SendJSONData     (EventTrackingId, DestinationNodeId, NetworkPath, RequestId, Action, JSONData,   RequestTimeout, ...)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
        /// <param name="NetworkPath">The network path.</param>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="JSONData">The JSON payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public async Task<SendOCPPMessageResult> SendJSONData(EventTracking_Id   EventTrackingId,
                                                               NetworkingNode_Id  DestinationNodeId,
                                                               NetworkPath        NetworkPath,
                                                               Request_Id         RequestId,
                                                               String             Action,
                                                               JObject            JSONData,
                                                               DateTime           RequestTimeout,
                                                               CancellationToken  CancellationToken   = default)
        {

            try
            {

                var webSocketConnections  = LookupNetworkingNode(DestinationNodeId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode      = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(networkingMode_WebSocketKey);

                    var jsonRequestMessage  = new OCPP_JSONRequestMessage(
                                                  Timestamp.Now,
                                                  EventTracking_Id.New,
                                                  webSocketConnections.First().Item2,
                                                  DestinationNodeId,
                                                  NetworkPath,
                                                  RequestId,
                                                  Action,
                                                  JSONData
                                              );

                    var ocppTextMessage     = jsonRequestMessage.ToJSON().ToString(Formatting.None);


                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendTextMessage(
                                                            webSocketConnection.Item1,
                                                            ocppTextMessage,
                                                            EventTrackingId,
                                                            CancellationToken
                                                        ))
                        {

                            requests.TryAdd(RequestId,
                                            SendRequestState.FromJSONRequest(
                                                Timestamp.Now,
                                                DestinationNodeId,
                                                RequestTimeout,
                                                jsonRequestMessage
                                            ));

                            #region OnJSONMessageRequestSent

                            var onJSONMessageRequestSent = OnJSONMessageRequestSent;
                            if (onJSONMessageRequestSent is not null)
                            {
                                try
                                {

                                    await Task.WhenAll(onJSONMessageRequestSent.GetInvocationList().
                                                           OfType<OnWebSocketTextMessageDelegate>().
                                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                          Timestamp.Now,
                                                                                          this,
                                                                                          webSocketConnection.Item1,
                                                                                          EventTrackingId,
                                                                                          ocppTextMessage,
                                                                                          CancellationToken
                                                                                      )).
                                                           ToArray());

                                }
                                catch (Exception e)
                                {
                                    DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnJSONMessageRequestSent));
                                }
                            }

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

        #region SendBinaryData   (EventTrackingId, DestinationNodeId, NetworkPath, RequestId, Action, BinaryData, RequestTimeout, ...)

        /// <summary>
        /// Send (and forget) the given binary data.
        /// </summary>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
        /// <param name="NetworkPath">The network path.</param>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="BinaryData">The binary payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public async Task<SendOCPPMessageResult> SendBinaryData(EventTracking_Id   EventTrackingId,
                                                                 NetworkingNode_Id  DestinationNodeId,
                                                                 NetworkPath        NetworkPath,
                                                                 Request_Id         RequestId,
                                                                 String             Action,
                                                                 Byte[]             BinaryData,
                                                                 DateTime           RequestTimeout,
                                                                 CancellationToken  CancellationToken   = default)
        {

            try
            {

                var webSocketConnections = LookupNetworkingNode(DestinationNodeId).ToArray();

                if (webSocketConnections.Length != 0)
                {

                    var networkingMode        = webSocketConnections.First().Item1.TryGetCustomDataAs<NetworkingMode>(networkingMode_WebSocketKey);

                    var binaryRequestMessage  = new OCPP_BinaryRequestMessage(
                                                    Timestamp.Now,
                                                    EventTracking_Id.New,
                                                    webSocketConnections.First().Item2,
                                                    DestinationNodeId,
                                                    NetworkPath,
                                                    RequestId,
                                                    Action,
                                                    BinaryData
                                                );

                    requests.TryAdd(RequestId,
                                    SendRequestState.FromBinaryRequest(
                                        Timestamp.Now,
                                        DestinationNodeId,
                                        RequestTimeout,
                                        binaryRequestMessage
                                    ));

                    var ocppBinaryMessage     = binaryRequestMessage.ToByteArray();

                    foreach (var webSocketConnection in webSocketConnections)
                    {

                        if (SendStatus.Success == await SendBinaryMessage(
                                                            webSocketConnection.Item1,
                                                            ocppBinaryMessage,
                                                            EventTrackingId,
                                                            CancellationToken
                                                        ))
                        {

                            #region OnBinaryMessageRequestSent

                            var requestLogger = OnBinaryMessageRequestSent;
                            if (requestLogger is not null)
                            {

                                var loggerTasks = requestLogger.GetInvocationList().
                                                                OfType <OnWebSocketBinaryMessageDelegate>().
                                                                Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                                                  this,
                                                                                                                  webSocketConnection.Item1,
                                                                                                                  EventTrackingId,
                                                                                                                  ocppBinaryMessage,
                                                                                                                  CancellationToken)).
                                                                ToArray();

                                try
                                {
                                    await Task.WhenAll(loggerTasks);
                                }
                                catch (Exception e)
                                {
                                    DebugX.Log(e, nameof(AOCPPWebSocketServer) + "." + nameof(OnBinaryMessageRequestSent));
                                }

                            }

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


        #region SendJSONAndWait  (EventTrackingId, NetworkingNodeId, NetworkPath, RequestId, OCPPAction, JSONPayload,   RequestTimeout = null)

        public async Task<SendRequestState> SendJSONAndWait(EventTracking_Id   EventTrackingId,
                                                            NetworkingNode_Id  NetworkingNodeId,
                                                            NetworkPath        NetworkPath,
                                                            Request_Id         RequestId,
                                                            String             OCPPAction,
                                                            JObject            JSONPayload,
                                                            TimeSpan?          RequestTimeout,
                                                            CancellationToken  CancellationToken   = default)
        {

            var endTime         = Timestamp.Now + (RequestTimeout ?? this.RequestTimeout ?? DefaultRequestTimeout);

            var sendJSONResult  = await SendJSONData(
                                            EventTrackingId,
                                            NetworkingNodeId,
                                            NetworkPath,
                                            RequestId,
                                            OCPPAction,
                                            JSONPayload,
                                            endTime,
                                            CancellationToken
                                        );

            if (sendJSONResult == SendOCPPMessageResult.Success) {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25, CancellationToken);

                        if (requests.TryGetValue(RequestId, out var sendRequestState) &&
                           (sendRequestState?.JSONResponse   is not null ||
                            sendRequestState?.BinaryResponse is not null ||
                            sendRequestState?.ErrorCode.HasValue == true))
                        {

                            requests.TryRemove(RequestId, out _);

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(AOCPPWebSocketServer), ".", nameof(SendJSONAndWait), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                if (requests.TryGetValue(RequestId, out var sendRequestState2) &&
                    sendRequestState2 is not null)
                {
                    sendRequestState2.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                if (requests.TryGetValue(RequestId, out var sendRequestState3) &&
                    sendRequestState3 is not null)
                {
                    sendRequestState3.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState3;
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return SendRequestState.FromJSONRequest(

                       now,
                       NetworkingNodeId,
                       now,
                       new OCPP_JSONRequestMessage(
                           Timestamp.Now,
                           EventTracking_Id.New,
                           NetworkingMode.Standard,
                           NetworkingNodeId,
                           NetworkPath,
                           RequestId,
                           OCPPAction,
                           JSONPayload
                       ),
                       now,

                       ErrorCode:  ResultCode.InternalError

                   );

        }

        #endregion

        #region SendBinaryAndWait(EventTrackingId, NetworkingNodeId, NetworkPath, RequestId, OCPPAction, BinaryPayload, RequestTimeout = null)

        public async Task<SendRequestState> SendBinaryAndWait(EventTracking_Id    EventTrackingId,
                                                              NetworkingNode_Id   NetworkingNodeId,
                                                              NetworkPath         NetworkPath,
                                                              Request_Id          RequestId,
                                                              String              OCPPAction,
                                                              Byte[]              BinaryPayload,
                                                              TimeSpan?           RequestTimeout,
                                                              CancellationToken   CancellationToken   = default)
        {

            var endTime         = Timestamp.Now + (RequestTimeout ?? this.RequestTimeout ?? DefaultRequestTimeout);

            var sendJSONResult  = await SendBinaryData(
                                            EventTrackingId,
                                            NetworkingNodeId,
                                            NetworkPath,
                                            RequestId,
                                            OCPPAction,
                                            BinaryPayload,
                                            endTime,
                                            CancellationToken
                                        );

            if (sendJSONResult == SendOCPPMessageResult.Success) {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25, CancellationToken);

                        if (requests.TryGetValue(RequestId, out var sendRequestState) &&
                           (sendRequestState?.JSONResponse   is not null ||
                            sendRequestState?.BinaryResponse is not null ||
                            sendRequestState?.ErrorCode.HasValue == true))
                        {

                            requests.TryRemove(RequestId, out _);

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(AOCPPWebSocketServer), ".", nameof(SendJSONAndWait), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                if (requests.TryGetValue(RequestId, out var sendRequestState2) &&
                    sendRequestState2 is not null)
                {
                    sendRequestState2.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState2;
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                if (requests.TryGetValue(RequestId, out var sendRequestState3) &&
                    sendRequestState3 is not null)
                {
                    sendRequestState3.ErrorCode = ResultCode.Timeout;
                    requests.TryRemove(RequestId, out _);
                    return sendRequestState3;
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return SendRequestState.FromBinaryRequest(

                       now,
                       NetworkingNodeId,
                       now,
                       new OCPP_BinaryRequestMessage(
                           Timestamp.Now,
                           EventTracking_Id.New,
                           NetworkingMode.Standard,
                           NetworkingNodeId,
                           NetworkPath,
                           RequestId,
                           OCPPAction,
                           BinaryPayload
                       ),
                       now,

                       ErrorCode:  ResultCode.InternalError

                   );

        }

        #endregion



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
