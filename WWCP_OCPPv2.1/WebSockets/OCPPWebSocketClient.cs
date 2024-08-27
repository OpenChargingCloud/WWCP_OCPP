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
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// The networking node HTTP WebSocket client runs on a networking node
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketClient : WWCPWebSocketClient,
                                               IOCPPWebSocketClient
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const String  DefaultHTTPUserAgent   = $"GraphDefined OCPP {Version.String} Web Socket Client";

        private    const String  LogfileName            = "NetworkingNodeWSClient.log";

        #endregion

        #region Properties

        public OCPPAdapter  OCPPAdapter    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node websocket client running on a networking node
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="NetworkingNode">The parent networking node.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="LocalCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public OCPPWebSocketClient(NetworkingNode.INetworkingNode                                  NetworkingNode,

                                   URL                                                             RemoteURL,
                                   HTTPHostname?                                                   VirtualHostname              = null,
                                   I18NString?                                                     Description                  = null,
                                   Boolean?                                                        PreferIPv4                   = null,
                                   RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionHandler?                               LocalCertificateSelector     = null,
                                   X509Certificate?                                                ClientCert                   = null,
                                   SslProtocols?                                                   TLSProtocol                  = null,
                                   String                                                          HTTPUserAgent                = DefaultHTTPUserAgent,
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

            : base(NetworkingNode,
                   RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent ?? DefaultHTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,

                   SecWebSocketProtocols,
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
                   DNSClient)

        {

            this.OCPPAdapter  = NetworkingNode.OCPP;

            #region Wire On(JSON/Binary)MessageReceived

            OnJSONMessageReceived += (requestTimestamp,
                                      wwcpWebSocketClient,
                                      webSocketClientConnection,
                                      eventTrackingId,
                                      messageTimestamp,
                                      sourceNodeId,
                                      jsonMessage,
                                      cancellationToken) =>

                OCPPAdapter.IN.ProcessJSONMessage(
                    messageTimestamp,
                    webSocketClientConnection,
                    sourceNodeId,
                    jsonMessage,
                    eventTrackingId,
                    cancellationToken
                );


            OnBinaryMessageReceived += (requestTimestamp,
                                        wwcpWebSocketClient,
                                        webSocketClientConnection,
                                        eventTrackingId,
                                        messageTimestamp,
                                        sourceNodeId,
                                        binaryMessage,
                                        cancellationToken) =>

                OCPPAdapter.IN.ProcessBinaryMessage(
                    messageTimestamp,
                    webSocketClientConnection,
                    sourceNodeId,
                    binaryMessage,
                    eventTrackingId,
                    cancellationToken
                );

            #endregion

            //this.Logger       = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                  LoggingPath,
            //                                                                  LoggingContext,
            //                                                                  LogfileCreator);

        }

        #endregion


        #region SendJSONRequest         (JSONRequestMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP request message.
        /// </summary>
        /// <param name="JSONRequestMessage">A JSON OCPP request message.</param>
        public async Task<SentMessageResult> SendJSONRequest(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            try
            {

                JSONRequestMessage.NetworkingMode = NetworkingMode;
                //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                return await SendJSONMessage(
                                 JSONRequestMessage.ToJSON(),
                                 JSONRequestMessage.RequestTimestamp,
                                 JSONRequestMessage.EventTrackingId,
                                 JSONRequestMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendJSONResponse        (JSONResponseMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP request message.
        /// </summary>
        /// <param name="JSONResponseMessage">A JSON OCPP request message.</param>
        public async Task<SentMessageResult> SendJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)
        {

            try
            {

                JSONResponseMessage.NetworkingMode = NetworkingMode;
                //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                return await SendJSONMessage(
                                 JSONResponseMessage.ToJSON(),
                                 JSONResponseMessage.ResponseTimestamp,
                                 JSONResponseMessage.EventTrackingId,
                                 JSONResponseMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendJSONRequestError    (JSONRequestErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP request message.
        /// </summary>
        /// <param name="JSONRequestErrorMessage">A JSON OCPP request message.</param>
        public async Task<SentMessageResult> SendJSONRequestError(OCPP_JSONRequestErrorMessage JSONRequestErrorMessage)
        {

            try
            {

                JSONRequestErrorMessage.NetworkingMode = NetworkingMode;
                //ErrorMessage.ErrorTimeout ??= ErrorMessage.ErrorTimestamp + (ErrorTimeout ?? DefaultErrorTimeout);

                return await SendJSONMessage(
                                 JSONRequestErrorMessage.ToJSON(),
                                 JSONRequestErrorMessage.ResponseTimestamp,
                                 JSONRequestErrorMessage.EventTrackingId,
                                 JSONRequestErrorMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendJSONResponseError   (JSONResponseErrorMessage)

        /// <summary>
        /// Send (and forget) the given JSON OCPP response message.
        /// </summary>
        /// <param name="JSONResponseErrorMessage">A JSON OCPP response message.</param>
        public async Task<SentMessageResult> SendJSONResponseError(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)
        {

            try
            {

                JSONResponseErrorMessage.NetworkingMode = NetworkingMode;
                //ErrorMessage.ErrorTimeout ??= ErrorMessage.ErrorTimestamp + (ErrorTimeout ?? DefaultErrorTimeout);

                return await SendJSONMessage(
                                 JSONResponseErrorMessage.ToJSON(),
                                 JSONResponseErrorMessage.ResponseTimestamp,
                                 JSONResponseErrorMessage.EventTrackingId,
                                 JSONResponseErrorMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
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

                JSONSendMessage.NetworkingMode = NetworkingMode;
                //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                return await SendJSONMessage(
                                 JSONSendMessage.ToJSON(),
                                 JSONSendMessage.MessageTimestamp,
                                 JSONSendMessage.EventTrackingId,
                                 JSONSendMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
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

                BinaryRequestMessage.NetworkingMode = NetworkingMode;
                //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                return await SendBinaryMessage(
                                 BinaryRequestMessage.ToByteArray(),
                                 BinaryRequestMessage.RequestTimestamp,
                                 BinaryRequestMessage.EventTrackingId,
                                 BinaryRequestMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendBinaryResponse      (BinaryResponseMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP request message.
        /// </summary>
        /// <param name="BinaryResponseMessage">A binary OCPP request message.</param>
        public async Task<SentMessageResult> SendBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)
        {

            try
            {

                BinaryResponseMessage.NetworkingMode = NetworkingMode;
                //ResponseMessage.ResponseTimeout ??= ResponseMessage.ResponseTimestamp + (ResponseTimeout ?? DefaultResponseTimeout);

                return await SendBinaryMessage(
                                 BinaryResponseMessage.ToByteArray(),
                                 BinaryResponseMessage.ResponseTimestamp,
                                 BinaryResponseMessage.EventTrackingId,
                                 BinaryResponseMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendBinaryRequestError  (BinaryRequestErrorMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP request message.
        /// </summary>
        /// <param name="BinaryRequestErrorMessage">A binary OCPP request message.</param>
        public async Task<SentMessageResult> SendBinaryRequestError(OCPP_BinaryRequestErrorMessage BinaryRequestErrorMessage)
        {

            try
            {

                BinaryRequestErrorMessage.NetworkingMode = NetworkingMode;
                //ErrorMessage.ErrorTimeout ??= ErrorMessage.ErrorTimestamp + (ErrorTimeout ?? DefaultErrorTimeout);

                return await SendBinaryMessage(
                                 BinaryRequestErrorMessage.ToByteArray(),
                                 BinaryRequestErrorMessage.ResponseTimestamp,
                                 BinaryRequestErrorMessage.EventTrackingId,
                                 BinaryRequestErrorMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
            }

        }

        #endregion

        #region SendBinaryResponseError (BinaryResponseErrorMessage)

        /// <summary>
        /// Send (and forget) the given binary OCPP response message.
        /// </summary>
        /// <param name="BinaryResponseErrorMessage">A binary OCPP response message.</param>
        public async Task<SentMessageResult> SendBinaryResponseError(OCPP_BinaryResponseErrorMessage BinaryResponseErrorMessage)
        {

            try
            {

                BinaryResponseErrorMessage.NetworkingMode = NetworkingMode;
                //ErrorMessage.ErrorTimeout ??= ErrorMessage.ErrorTimestamp + (ErrorTimeout ?? DefaultErrorTimeout);

                return await SendBinaryMessage(
                                 BinaryResponseErrorMessage.ToByteArray(),
                                 BinaryResponseErrorMessage.ResponseTimestamp,
                                 BinaryResponseErrorMessage.EventTrackingId,
                                 BinaryResponseErrorMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
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

                BinarySendMessage.NetworkingMode = NetworkingMode;
                //RequestMessage.RequestTimeout ??= RequestMessage.RequestTimestamp + (RequestTimeout ?? DefaultRequestTimeout);

                return await SendBinaryMessage(
                                 BinarySendMessage.ToByteArray(),
                                 BinarySendMessage.MessageTimestamp,
                                 BinarySendMessage.EventTrackingId,
                                 BinarySendMessage.CancellationToken
                             );

            }
            catch (Exception e)
            {
                return SentMessageResult.TransmissionFailed(e, webSocketClientConnection);
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
