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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// The OCPP HTTP Web Socket server.
    /// </summary>
    public partial class OCPPWebSocketServer : WWCPWebSocketServer,
                                               IOCPPWebSocketServer
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const   String                                              DefaultHTTPServiceName  = $"GraphDefined OCPP {Version.String} Web Socket Server";

        protected readonly ConcurrentDictionary<Request_Id, SendRequestState>  requests                = [];

        public new const   String                                              LogfileName             = "CSMSWSServer.log";

        #endregion

        #region Properties

        /// <summary>
        /// The parent OCPP adapter.
        /// </summary>
        public OCPPAdapter  OCPPAdapter    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP HTTP Web Socket server.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
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
        public OCPPWebSocketServer(NetworkingNode.INetworkingNode                                  NetworkingNode,

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

            : base(NetworkingNode,
                   HTTPServiceName,
                   IPAddress,
                   TCPPort,
                   Description,

                   RequireAuthentication,
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

            this.OCPPAdapter  = NetworkingNode.OCPP;

            #region Wire On(JSON/Binary)MessageReceived

            OnJSONMessageReceived += (requestTimestamp,
                                      wwcpWebSocketServer,
                                      webSocketServerConnection,
                                      eventTrackingId,
                                      messageTimestamp,
                                      sourceNodeId,
                                      jsonMessage,
                                      cancellationToken) =>

                OCPPAdapter.IN.ProcessJSONMessage(
                    messageTimestamp,
                    webSocketServerConnection,
                    sourceNodeId,
                    jsonMessage,
                    eventTrackingId,
                    cancellationToken
                );


            OnBinaryMessageReceived += (requestTimestamp,
                                        wwcpWebSocketServer,
                                        webSocketServerConnection,
                                        eventTrackingId,
                                        messageTimestamp,
                                        sourceNodeId,
                                        binaryMessage,
                                        cancellationToken) =>

                OCPPAdapter.IN.ProcessBinaryMessage(
                    messageTimestamp,
                    webSocketServerConnection,
                    sourceNodeId,
                    binaryMessage,
                    eventTrackingId,
                    cancellationToken
                );

            #endregion

            //this.Logger       = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                             LoggingPath,
            //                                                             LoggingContext,
            //                                                             LogfileCreator);

            if (AutoStart)
                Start();

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

                    JSONRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                            ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONRequestMessage.ToJSON(),
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.Destination.Next,
                                                      JSONRequestMessage.EventTrackingId,
                                                      JSONRequestMessage.CancellationToken
                                                  );

                    //var jsonMessage  = JSONRequestMessage.ToJSON();

                    //var sentStatus   = await SendTextMessage(
                    //                             webSocketConnection,
                    //                             jsonMessage.ToString(Formatting.None),
                    //                             JSONRequestMessage.EventTrackingId,
                    //                             JSONRequestMessage.CancellationToken
                    //                         );

                    //await LogEvent(
                    //          OnJSONMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              JSONRequestMessage.EventTrackingId,
                    //              JSONRequestMessage.RequestTimestamp,
                    //              jsonMessage,
                    //              sentStatus,
                    //              JSONRequestMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(JSONResponseMessage.Destination.Next))
                {

                    JSONResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                             ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONResponseMessage.ToJSON(),
                                                      JSONResponseMessage.ResponseTimestamp,
                                                      JSONResponseMessage.Destination.Next,
                                                      JSONResponseMessage.EventTrackingId,
                                                      JSONResponseMessage.CancellationToken
                                                  );

                    //var jsonMessage  = JSONResponseMessage.ToJSON();

                    //var sentStatus   = await SendTextMessage(
                    //                             webSocketConnection,
                    //                             jsonMessage.ToString(Formatting.None),
                    //                             JSONResponseMessage.EventTrackingId,
                    //                             JSONResponseMessage.CancellationToken
                    //                         );

                    //await LogEvent(
                    //          OnJSONMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              JSONResponseMessage.EventTrackingId,
                    //              JSONResponseMessage.ResponseTimestamp,
                    //              jsonMessage,
                    //              sentStatus,
                    //              JSONResponseMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(JSONRequestErrorMessage.Destination.Next))
                {

                    JSONRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                 ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONRequestErrorMessage.ToJSON(),
                                                      JSONRequestErrorMessage.ResponseTimestamp,
                                                      JSONRequestErrorMessage.Destination.Next,
                                                      JSONRequestErrorMessage.EventTrackingId,
                                                      JSONRequestErrorMessage.CancellationToken
                                                  );

                    //var jsonMessage  = JSONRequestErrorMessage.ToJSON();

                    //var sentStatus   = await SendTextMessage(
                    //                             webSocketConnection,
                    //                             jsonMessage.ToString(Formatting.None),
                    //                             JSONRequestErrorMessage.EventTrackingId,
                    //                             JSONRequestErrorMessage.CancellationToken
                    //                         );

                    //await LogEvent(
                    //          OnJSONMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              JSONRequestErrorMessage.EventTrackingId,
                    //              JSONRequestErrorMessage.ResponseTimestamp,
                    //              jsonMessage,
                    //              sentStatus,
                    //              JSONRequestErrorMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(JSONResponseErrorMessage.Destination.Next))
                {

                    JSONResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                  ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONResponseErrorMessage.ToJSON(),
                                                      JSONResponseErrorMessage.ResponseTimestamp,
                                                      JSONResponseErrorMessage.Destination.Next,
                                                      JSONResponseErrorMessage.EventTrackingId,
                                                      JSONResponseErrorMessage.CancellationToken
                                                  );

                    //var jsonMessage  = JSONResponseErrorMessage.ToJSON();

                    //var sentStatus   = await SendTextMessage(
                    //                             webSocketConnection,
                    //                             jsonMessage.ToString(Formatting.None),
                    //                             JSONResponseErrorMessage.EventTrackingId,
                    //                             JSONResponseErrorMessage.CancellationToken
                    //                         );

                    //await LogEvent(
                    //          OnJSONMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              JSONResponseErrorMessage.EventTrackingId,
                    //              JSONResponseErrorMessage.ResponseTimestamp,
                    //              jsonMessage,
                    //              sentStatus,
                    //              JSONResponseErrorMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(JSONSendMessage.Destination.Next))
                {

                    JSONSendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                         ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendJSONMessage(
                                                      webSocketConnection,
                                                      JSONSendMessage.ToJSON(),
                                                      JSONSendMessage.MessageTimestamp,
                                                      JSONSendMessage.Destination.Next,
                                                      JSONSendMessage.EventTrackingId,
                                                      JSONSendMessage.CancellationToken
                                                  );

                    //var jsonMessage  = JSONSendMessage.ToJSON();

                    //var sentStatus   = await SendTextMessage(
                    //                             webSocketConnection,
                    //                             jsonMessage.ToString(Formatting.None),
                    //                             JSONSendMessage.EventTrackingId,
                    //                             JSONSendMessage.CancellationToken
                    //                         );

                    //await LogEvent(
                    //          OnJSONMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              JSONSendMessage.EventTrackingId,
                    //              JSONSendMessage.MessageTimestamp,
                    //              jsonMessage,
                    //              sentStatus,
                    //              JSONSendMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryRequestMessage.Destination.Next))
                {

                    BinaryRequestMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                              ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryRequestMessage.ToByteArray(),
                                                      BinaryRequestMessage.RequestTimestamp,
                                                      BinaryRequestMessage.Destination.Next,
                                                      BinaryRequestMessage.EventTrackingId,
                                                      BinaryRequestMessage.CancellationToken
                                                  );

                    //var binaryMessage  = BinaryRequestMessage.ToByteArray();

                    //var sentStatus     = await SendBinaryMessage(
                    //                               webSocketConnection,
                    //                               binaryMessage,
                    //                               BinaryRequestMessage.EventTrackingId,
                    //                               BinaryRequestMessage.CancellationToken
                    //                           );

                    //await LogEvent(
                    //          OnBinaryMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              BinaryRequestMessage.EventTrackingId,
                    //              BinaryRequestMessage.RequestTimestamp,
                    //              binaryMessage,
                    //              sentStatus,
                    //              BinaryRequestMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryResponseMessage.Destination.Next))
                {

                    BinaryResponseMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                               ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryResponseMessage.ToByteArray(),
                                                      BinaryResponseMessage.ResponseTimestamp,
                                                      BinaryResponseMessage.Destination.Next,
                                                      BinaryResponseMessage.EventTrackingId,
                                                      BinaryResponseMessage.CancellationToken
                                                  );

                    //var binaryMessage  = BinaryResponseMessage.ToByteArray();

                    //var sentStatus     = await SendBinaryMessage(
                    //                               webSocketConnection,
                    //                               binaryMessage,
                    //                               BinaryResponseMessage.EventTrackingId,
                    //                               BinaryResponseMessage.CancellationToken
                    //                           );

                    //await LogEvent(
                    //          OnBinaryMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              BinaryResponseMessage.EventTrackingId,
                    //              BinaryResponseMessage.ResponseTimestamp,
                    //              binaryMessage,
                    //              sentStatus,
                    //              BinaryResponseMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryRequestErrorMessage.Destination.Next))
                {

                    BinaryRequestErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                   ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryRequestErrorMessage.ToByteArray(),
                                                      BinaryRequestErrorMessage.ResponseTimestamp,
                                                      BinaryRequestErrorMessage.Destination.Next,
                                                      BinaryRequestErrorMessage.EventTrackingId,
                                                      BinaryRequestErrorMessage.CancellationToken
                                                  );

                    //var binaryMessage  = BinaryRequestErrorMessage.ToByteArray();

                    //var sentStatus     = await SendBinaryMessage(
                    //                               webSocketConnection,
                    //                               binaryMessage,
                    //                               BinaryRequestErrorMessage.EventTrackingId,
                    //                               BinaryRequestErrorMessage.CancellationToken
                    //                           );

                    //await LogEvent(
                    //          OnBinaryMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              BinaryRequestErrorMessage.EventTrackingId,
                    //              BinaryRequestErrorMessage.ResponseTimestamp,
                    //              binaryMessage,
                    //              sentStatus,
                    //              BinaryRequestErrorMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(BinaryResponseErrorMessage.Destination.Next))
                {

                    BinaryResponseErrorMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                                    ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinaryResponseErrorMessage.ToByteArray(),
                                                      BinaryResponseErrorMessage.ResponseTimestamp,
                                                      BinaryResponseErrorMessage.Destination.Next,
                                                      BinaryResponseErrorMessage.EventTrackingId,
                                                      BinaryResponseErrorMessage.CancellationToken
                                                  );

                    //var binaryMessage  = BinaryResponseErrorMessage.ToByteArray();

                    //var sentStatus     = await SendBinaryMessage(
                    //                               webSocketConnection,
                    //                               binaryMessage,
                    //                               BinaryResponseErrorMessage.EventTrackingId,
                    //                               BinaryResponseErrorMessage.CancellationToken
                    //                           );

                    //await LogEvent(
                    //          OnBinaryMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              BinaryResponseErrorMessage.EventTrackingId,
                    //              BinaryResponseErrorMessage.ResponseTimestamp,
                    //              binaryMessage,
                    //              sentStatus,
                    //              BinaryResponseErrorMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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

                foreach (var webSocketConnection in LookupNetworkingNode(BinarySendMessage.Destination.Next))
                {

                    BinarySendMessage.NetworkingMode = webSocketConnection.TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode)
                                                           ?? NetworkingMode.Standard;

                    var sentMessageResult = await SendBinaryMessage(
                                                      webSocketConnection,
                                                      BinarySendMessage.ToByteArray(),
                                                      BinarySendMessage.MessageTimestamp,
                                                      BinarySendMessage.Destination.Next,
                                                      BinarySendMessage.EventTrackingId,
                                                      BinarySendMessage.CancellationToken
                                                  );

                    //var binaryMessage  = BinarySendMessage.ToByteArray();

                    //var sentStatus     = await SendBinaryMessage(
                    //                               webSocketConnection,
                    //                               binaryMessage,
                    //                               BinarySendMessage.EventTrackingId,
                    //                               BinarySendMessage.CancellationToken
                    //                           );

                    //await LogEvent(
                    //          OnBinaryMessageSent,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              Timestamp.Now,
                    //              this,
                    //              webSocketConnection,
                    //              BinarySendMessage.EventTrackingId,
                    //              BinarySendMessage.MessageTimestamp,
                    //              binaryMessage,
                    //              sentStatus,
                    //              BinarySendMessage.CancellationToken
                    //          )
                    //      );

                    //if (sentStatus == SentStatus.Success)
                    //    return SentMessageResult.Success(webSocketConnection);

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



        private IEnumerable<WebSocketServerConnection> LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId)
        {

            if (NetworkingNodeId == NetworkingNode_Id.Zero)
                return [];

            if (NetworkingNodeId == NetworkingNode_Id.Broadcast)
                return WebSocketConnections;

            var lookUpNetworkingNodeId = NetworkingNodeId;

            if (OCPPAdapter.NetworkingNode.Routing.LookupNetworkingNode(lookUpNetworkingNodeId, out var reachability) &&
                reachability is not null)
            {
                lookUpNetworkingNodeId = reachability.DestinationId;
            }

            if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
            {
                lookUpNetworkingNodeId = networkingHubId;
                return WebSocketConnections.Where (connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(WebSocketKeys.NetworkingNodeId) == lookUpNetworkingNodeId);
            }

            return WebSocketConnections.Where(connection => connection.TryGetCustomDataAs<NetworkingNode_Id>(WebSocketKeys.NetworkingNodeId) == lookUpNetworkingNodeId).ToArray();

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
