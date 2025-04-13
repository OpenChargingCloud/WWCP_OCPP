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

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// The OCPP HTTP WebSocket server.
    /// </summary>
    public partial class OCPPWebSocketServer : WWCPWebSocketServer,
                                               IOCPPWebSocketServer
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const   String                                              DefaultHTTPServiceName  = $"GraphDefined OCPP WebSocket Server";

        protected readonly ConcurrentDictionary<Request_Id, SendRequestState>  requests                = [];

        public new const   String                                              LogfileName             = "CSMSWSServer.log";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP HTTP WebSocket server.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// 
        /// <param name="HTTPServiceName">An optional identification string for the HTTP service.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="Description">An optional description of this HTTP WebSocket service.</param>
        /// 
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// 
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public OCPPWebSocketServer(NetworkingNode.INetworkingNode                                  NetworkingNode,

                                   String?                                                         HTTPServiceName              = DefaultHTTPServiceName,
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

                                   DNSClient?                                                      DNSClient                    = null,
                                   Boolean                                                         AutoStart                    = true)

            : base(NetworkingNode,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   IPAddress,
                   TCPPort,
                   Description,

                   RequireAuthentication,
                   SecWebSocketProtocols ?? throw new ArgumentNullException(nameof(SecWebSocketProtocols), "The given enumeration of accepted OCPP versions must not be null!"),
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
                   false)

        {

            //this.Logger       = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                  LoggingPath,
            //                                                                  LoggingContext,
            //                                                                  LogfileCreator);

            //if (AutoStart)
            //    Start();

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

                foreach (var webSocketConnection in GetConnectionsFor(JSONRequestMessage.Destination.Next))
                {

                    JSONRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                            ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONRequestMessage.ToJSON(),
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.EventTrackingId,
                                                      JSONRequestMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(JSONResponseMessage.Destination.Next))
                {

                    JSONResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                             ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONResponseMessage.ToJSON(),
                                                      JSONResponseMessage.ResponseTimestamp,
                                                      JSONResponseMessage.EventTrackingId,
                                                      JSONResponseMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(JSONRequestErrorMessage.Destination.Next))
                {

                    JSONRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                 ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONRequestErrorMessage.ToJSON(),
                                                      JSONRequestErrorMessage.ResponseTimestamp,
                                                      JSONRequestErrorMessage.EventTrackingId,
                                                      JSONRequestErrorMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(JSONResponseErrorMessage.Destination.Next))
                {

                    JSONResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                  ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONResponseErrorMessage.ToJSON(),
                                                      JSONResponseErrorMessage.ResponseTimestamp,
                                                      JSONResponseErrorMessage.EventTrackingId,
                                                      JSONResponseErrorMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(JSONSendMessage.Destination.Next))
                {

                    JSONSendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                         ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONSendMessage.ToJSON(),
                                                      JSONSendMessage.MessageTimestamp,
                                                      JSONSendMessage.EventTrackingId,
                                                      JSONSendMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(BinaryRequestMessage.Destination.Next))
                {

                    BinaryRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                              ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryRequestMessage.ToByteArray(),
                                                      BinaryRequestMessage.RequestTimestamp,
                                                      BinaryRequestMessage.EventTrackingId,
                                                      BinaryRequestMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(BinaryResponseMessage.Destination.Next))
                {

                    BinaryResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                               ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryResponseMessage.ToByteArray(),
                                                      BinaryResponseMessage.ResponseTimestamp,
                                                      BinaryResponseMessage.EventTrackingId,
                                                      BinaryResponseMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(BinaryRequestErrorMessage.Destination.Next))
                {

                    BinaryRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                   ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryRequestErrorMessage.ToByteArray(),
                                                      BinaryRequestErrorMessage.ResponseTimestamp,
                                                      BinaryRequestErrorMessage.EventTrackingId,
                                                      BinaryRequestErrorMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(BinaryResponseErrorMessage.Destination.Next))
                {

                    BinaryResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                    ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryResponseErrorMessage.ToByteArray(),
                                                      BinaryResponseErrorMessage.ResponseTimestamp,
                                                      BinaryResponseErrorMessage.EventTrackingId,
                                                      BinaryResponseErrorMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

                foreach (var webSocketConnection in GetConnectionsFor(BinarySendMessage.Destination.Next))
                {

                    BinarySendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                           ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinarySendMessage.ToByteArray(),
                                                      BinarySendMessage.MessageTimestamp,
                                                      BinarySendMessage.EventTrackingId,
                                                      BinarySendMessage.CancellationToken
                                                  );

                    if (sentMessageResult.Result == SentMessageResults.Success)
                        return sentMessageResult;

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

        #region (private) HandleErrors(Module, Caller, ExceptionOccurred)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccurred)
        {

            DebugX.LogException(ExceptionOccurred, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
