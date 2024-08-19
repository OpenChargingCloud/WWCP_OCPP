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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// The networking node HTTP WebSocket client runs on a networking node
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketClient : WebSocketClient,
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

        public OCPPAdapter     OCPPAdapter      { get; }

        public NetworkingMode  NetworkingMode   { get; } = NetworkingMode.Standard;

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a JSON message was sent.
        /// </summary>
        public     event OnWebSocketClientJSONMessageSentDelegate?         OnJSONMessageSent;

        /// <summary>
        /// An event sent whenever a JSON message was received.
        /// </summary>
        public     event OnWebSocketClientJSONMessageReceivedDelegate?     OnJSONMessageReceived;


        /// <summary>
        /// An event sent whenever a binary message was sent.
        /// </summary>
        public new event OnWebSocketClientBinaryMessageSentDelegate?       OnBinaryMessageSent;

        /// <summary>
        /// An event sent whenever a binary message was received.
        /// </summary>
        public new event OnWebSocketClientBinaryMessageReceivedDelegate?   OnBinaryMessageReceived;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node websocket client running on a networking node
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
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
        public OCPPWebSocketClient(OCPPAdapter                                                     OCPPAdapter,

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

            : base(RemoteURL,
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

            this.OCPPAdapter     = OCPPAdapter;
            this.NetworkingMode  = NetworkingMode ?? NetworkingNode.NetworkingMode.Standard;

            //this.Logger          = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                LoggingPath,
            //                                                                LoggingContext,
            //                                                                LogfileCreator);

        }

        #endregion


        #region ProcessWebSocketTextFrame   (RequestTimestamp, ClientConnection, TextMessage,   EventTrackingId, CancellationToken)

        public override async Task ProcessWebSocketTextFrame(DateTime                   RequestTimestamp,
                                                             WebSocketClientConnection  ClientConnection,
                                                             EventTracking_Id           EventTrackingId,
                                                             String                     TextMessage,
                                                             CancellationToken          CancellationToken)
        {

            #region Initial checks

            if (TextMessage == "[]" ||
                TextMessage.IsNullOrEmpty())
            {

                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessWebSocketBinaryFrame),
                          "Received an empty text message!"
                      );

                return;

            }

            #endregion

            try
            {

                var jsonMessage = JArray.Parse(TextMessage);

                await LogEvent(
                          OnJSONMessageReceived,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              EventTrackingId,
                              RequestTimestamp,
                              jsonMessage,
                              CancellationToken
                          )
                      );

                await OCPPAdapter.IN.ProcessJSONMessage(
                          RequestTimestamp,
                          ClientConnection,
                          jsonMessage,
                          EventTrackingId,
                          CancellationToken
                      );

            }
            catch (Exception e)
            {
                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessWebSocketTextFrame),
                          e
                      );
            }

        }

        #endregion

        #region ProcessWebSocketBinaryFrame (RequestTimestamp, ClientConnection, BinaryMessage, EventTrackingId, CancellationToken)

        public override async Task ProcessWebSocketBinaryFrame(DateTime                   RequestTimestamp,
                                                               WebSocketClientConnection  ClientConnection,
                                                               EventTracking_Id           EventTrackingId,
                                                               Byte[]                     BinaryMessage,
                                                               CancellationToken          CancellationToken)
        {

            #region Initial checks

            if (BinaryMessage.Length == 0)
            {

                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessWebSocketBinaryFrame),
                          "Received an empty binary message!"
                      );

                return;

            }

            #endregion

            try
            {

                await LogEvent(
                          OnBinaryMessageReceived,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              EventTrackingId,
                              RequestTimestamp,
                              BinaryMessage,
                              CancellationToken
                          )
                      );

                await OCPPAdapter.IN.ProcessBinaryMessage(
                          RequestTimestamp,
                          ClientConnection,
                          BinaryMessage,
                          EventTrackingId,
                          CancellationToken
                      );

            }
            catch (Exception e)
            {
                await HandleErrors(
                          nameof(OCPPWebSocketClient),
                          nameof(ProcessWebSocketBinaryFrame),
                          e
                      );
            }

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

                var jsonMessage  = JSONRequestMessage.ToJSON();

                var sentStatus   = await SendTextMessage(
                                             jsonMessage.ToString(Formatting.None),
                                             JSONRequestMessage.EventTrackingId,
                                             JSONRequestMessage.CancellationToken
                                         );

                await LogEvent(
                          OnJSONMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              JSONRequestMessage.EventTrackingId,
                              JSONRequestMessage.RequestTimestamp,
                              jsonMessage,
                              sentStatus,
                              JSONRequestMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var jsonMessage  = JSONResponseMessage.ToJSON();

                var sentStatus   =  await SendTextMessage(
                                              jsonMessage.ToString(Formatting.None),
                                              JSONResponseMessage.EventTrackingId,
                                              JSONResponseMessage.CancellationToken
                                          );

                await LogEvent(
                          OnJSONMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              JSONResponseMessage.EventTrackingId,
                              JSONResponseMessage.ResponseTimestamp,
                              jsonMessage,
                              sentStatus,
                              JSONResponseMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var jsonMessage  = JSONRequestErrorMessage.ToJSON();

                var sentStatus   = await SendTextMessage(
                                             jsonMessage.ToString(Formatting.None),
                                             JSONRequestErrorMessage.EventTrackingId,
                                             JSONRequestErrorMessage.CancellationToken
                                         );

                await LogEvent(
                          OnJSONMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              JSONRequestErrorMessage.EventTrackingId,
                              JSONRequestErrorMessage.ResponseTimestamp,
                              jsonMessage,
                              sentStatus,
                              JSONRequestErrorMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var jsonMessage  = JSONResponseErrorMessage.ToJSON();

                var sentStatus   = await SendTextMessage(
                                             jsonMessage.ToString(Formatting.None),
                                             JSONResponseErrorMessage.EventTrackingId,
                                             JSONResponseErrorMessage.CancellationToken
                                         );

                await LogEvent(
                          OnJSONMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              JSONResponseErrorMessage.EventTrackingId,
                              JSONResponseErrorMessage.ResponseTimestamp,
                              jsonMessage,
                              sentStatus,
                              JSONResponseErrorMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var jsonMessage  = JSONSendMessage.ToJSON();

                var sentStatus   = await SendTextMessage(
                                             jsonMessage.ToString(Formatting.None),
                                             JSONSendMessage.EventTrackingId,
                                             JSONSendMessage.CancellationToken
                                         );

                await LogEvent(
                          OnJSONMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              JSONSendMessage.EventTrackingId,
                              JSONSendMessage.MessageTimestamp,
                              jsonMessage,
                              sentStatus,
                              JSONSendMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var binaryMessage  = BinaryRequestMessage.ToByteArray();

                var sentStatus     = await SendBinaryMessage(
                                               binaryMessage,
                                               BinaryRequestMessage.EventTrackingId,
                                               BinaryRequestMessage.CancellationToken
                                           );

                await LogEvent(
                          OnBinaryMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              BinaryRequestMessage.EventTrackingId,
                              BinaryRequestMessage.RequestTimestamp,
                              binaryMessage,
                              sentStatus,
                              BinaryRequestMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var binaryMessage  = BinaryResponseMessage.ToByteArray();

                var sentStatus     = await SendBinaryMessage(
                                               binaryMessage,
                                               BinaryResponseMessage.EventTrackingId,
                                               BinaryResponseMessage.CancellationToken
                                           );

                await LogEvent(
                          OnBinaryMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              BinaryResponseMessage.EventTrackingId,
                              BinaryResponseMessage.ResponseTimestamp,
                              binaryMessage,
                              sentStatus,
                              BinaryResponseMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var binaryMessage  = BinaryRequestErrorMessage.ToByteArray();

                var sentStatus     = await SendBinaryMessage(
                                               binaryMessage,
                                               BinaryRequestErrorMessage.EventTrackingId,
                                               BinaryRequestErrorMessage.CancellationToken
                                           );

                await LogEvent(
                          OnBinaryMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              BinaryRequestErrorMessage.EventTrackingId,
                              BinaryRequestErrorMessage.ResponseTimestamp,
                              binaryMessage,
                              sentStatus,
                              BinaryRequestErrorMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var binaryMessage  = BinaryResponseErrorMessage.ToByteArray();

                var sentStatus     = await SendBinaryMessage(
                                               binaryMessage,
                                               BinaryResponseErrorMessage.EventTrackingId,
                                               BinaryResponseErrorMessage.CancellationToken
                                           );

                await LogEvent(
                          OnBinaryMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              BinaryResponseErrorMessage.EventTrackingId,
                              BinaryResponseErrorMessage.ResponseTimestamp,
                              binaryMessage,
                              sentStatus,
                              BinaryResponseErrorMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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

                var binaryMessage  = BinarySendMessage.ToByteArray();

                var sentStatus     = await SendBinaryMessage(
                                               binaryMessage,
                                               BinarySendMessage.EventTrackingId,
                                               BinarySendMessage.CancellationToken
                                           );

                await LogEvent(
                          OnBinaryMessageSent,
                          loggingDelegate => loggingDelegate.Invoke(
                              Timestamp.Now,
                              this,
                              webSocketClientConnection,
                              BinarySendMessage.EventTrackingId,
                              BinarySendMessage.MessageTimestamp,
                              binaryMessage,
                              sentStatus,
                              BinarySendMessage.CancellationToken
                          )
                      );

                return sentStatus switch {
                           SentStatus.Success  => SentMessageResult.Success(webSocketClientConnection),
                           SentStatus.Error    => SentMessageResult.Unknown(),
                           _                   => SentMessageResult.Unknown(),
                       };

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
