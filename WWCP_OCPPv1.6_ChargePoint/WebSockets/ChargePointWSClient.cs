/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

        /// <summary>
    /// The delegate for the HTTP web socket request log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketClient">The sending WebSocket client.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task WSClientRequestLogHandler(DateTime         Timestamp,
                                                   WebSocketClient  WebSocketClient,
                                                   JArray           Request);

    /// <summary>
    /// The delegate for the HTTP web socket response log.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="WebSocketClient">The sending WebSocket client.</param>
    /// <param name="Request">The incoming WebSocket request.</param>
    /// <param name="Response">The outgoing WebSocket response.</param>
    public delegate Task WSClientResponseLogHandler(DateTime         Timestamp,
                                                    WebSocketClient  WebSocketClient,
                                                    JArray           Request,
                                                    JArray           Response);


    public delegate Task  OnWebSocketClientTextMessageResponseDelegate  (DateTime              Timestamp,
                                                                         ChargePointWSClient   Client,
                                                                         EventTracking_Id      EventTrackingId,
                                                                         DateTime              RequestTimestamp,
                                                                         String                RequestMessage,
                                                                         DateTime              ResponseTimestamp,
                                                                         String                ResponseMessage);


    public delegate Task  OnWebSocketClientBinaryMessageResponseDelegate(DateTime              Timestamp,
                                                                         ChargePointWSClient   Client,
                                                                         EventTracking_Id      EventTrackingId,
                                                                         DateTime              RequestTimestamp,
                                                                         Byte[]                RequestMessage,
                                                                         DateTime              ResponseTimestamp,
                                                                         Byte[]                ResponseMessage);


    /// <summary>
    /// The charge point HTTP web socket client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : WebSocketClient,
                                               IChargePointWebSocketClient,
                                               IChargePointServer
    {

        #region (class) SendRequestState

        public class SendRequestState2
        {

            public DateTime                 Timestamp            { get; }
            public OCPP_JSONRequestMessage  WSRequestMessage     { get; }
            public DateTime                 Timeout              { get; }

            public DateTime?                ResponseTimestamp    { get; set; }
            public JObject?                 Response             { get; set; }

            public ResultCode?              ErrorCode            { get; set; }
            public String?                  ErrorDescription     { get; set; }
            public JObject?                 ErrorDetails         { get; set; }


            public Boolean                  NoErrors
                 => !ErrorCode.HasValue;

            public Boolean                  HasErrors
                 =>  ErrorCode.HasValue;


            public SendRequestState2(DateTime                 Timestamp,
                                     OCPP_JSONRequestMessage  WSRequestMessage,
                                     DateTime                 Timeout,

                                     DateTime?                ResponseTimestamp   = null,
                                     JObject?                 Response            = null,

                                     ResultCode?              ErrorCode           = null,
                                     String?                  ErrorDescription    = null,
                                     JObject?                 ErrorDetails        = null)
            {

                this.Timestamp          = Timestamp;
                this.WSRequestMessage   = WSRequestMessage;
                this.Timeout            = Timeout;

                this.ResponseTimestamp  = ResponseTimestamp;
                this.Response           = Response;

                this.ErrorCode          = ErrorCode;
                this.ErrorDescription   = ErrorDescription;
                this.ErrorDetails       = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public  new const  String    DefaultHTTPUserAgent    = "GraphDefined OCPP " + Version.String + " CP WebSocket Client";

        private     const  String    LogfileName             = "ChargePointWSClient.log";

        public static      TimeSpan  DefaultRequestTimeout   = TimeSpan.FromSeconds(30);

        public readonly ConcurrentDictionary<Request_Id, SendRequestState> requests = [];

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public NetworkingNode_Id                     ChargeBoxIdentity               { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From                            { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                              { get; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                            JSONFormatting                  { get; set; } = Formatting.None;

        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        public ChargePointWSClient.CPClientLogger    Logger                          { get; }

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<StatusInfo>?                                CustomStatusInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                CustomMeterValueSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                              CustomSampledValueSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                       CustomCertificateHashDataSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                          CustomChargingScheduleSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                    CustomChargingSchedulePeriodSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<ConfigurationKey>?                          CustomConfigurationKeySerializer                            { get; set; }


        // Security extensions
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                            CustomSignatureSerializer                                   { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <OCPP.Signature>?                            CustomBinarySignatureSerializer                             { get; set; }

        #endregion

        #region Events

        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseReceived;
        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseSent;

        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point websocket client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargePointWSClient(NetworkingNode_Id                    ChargeBoxIdentity,
                                   String                               From,
                                   String                               To,

                                   URL                                  RemoteURL,
                                   HTTPHostname?                        VirtualHostname              = null,
                                   String?                              Description                  = null,
                                   Boolean?                             PreferIPv4                   = null,
                                   RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                   X509Certificate?                     ClientCert                   = null,
                                   SslProtocols?                        TLSProtocol                  = null,
                                   String?                              HTTPUserAgent                = DefaultHTTPUserAgent,
                                   IHTTPAuthentication?                 HTTPAuthentication           = null,
                                   TimeSpan?                            RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                   UInt16?                              MaxNumberOfRetries           = 3,
                                   UInt32?                              InternalBufferSize           = null,

                                   IEnumerable<String>?                 SecWebSocketProtocols        = null,

                                   Boolean                              DisableMaintenanceTasks      = false,
                                   TimeSpan?                            MaintenanceEvery             = null,
                                   Boolean                              DisableWebSocketPings        = false,
                                   TimeSpan?                            WebSocketPingEvery           = null,
                                   TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                   String?                              LoggingPath                  = null,
                                   String                               LoggingContext               = CPClientLogger.DefaultContext,
                                   LogfileCreatorDelegate?              LogfileCreator               = null,
                                   HTTPClientLogger?                    HTTPLogger                   = null,
                                   DNSClient?                           DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout ?? DefaultRequestTimeout,
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

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity                  = ChargeBoxIdentity;
            this.From                               = From;
            this.To                                 = To;

            //this.Logger                             = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                                   LoggingPath,
            //                                                                                   LoggingContext,
            //                                                                                   LogfileCreator);

        }

        #endregion



        #region SendRequest(NetworkingNodeId, NetworkPath, Action, RequestId, JSONMessage)

        public async Task<OCPP_JSONRequestMessage> SendRequest(NetworkingNode_Id?  NetworkingNodeId,
                                                               NetworkPath?        NetworkPath,
                                                               String              Action,
                                                               Request_Id          RequestId,
                                                               JObject             JSONMessage)
        {

            OCPP_JSONRequestMessage? jsonRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 NetworkingNodeId ?? ChargeBoxIdentity,
                                                 NetworkPath      ?? NetworkPath.Empty,
                                                 RequestId,
                                                 Action,
                                                 JSONMessage
                                             );

                        await SendText(jsonRequestMessage.
                                           ToJSON().
                                           ToString(JSONFormatting));

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromJSONRequest(
                                            Timestamp.Now,
                                            ChargeBoxIdentity,
                                            Timestamp.Now + RequestTimeout,
                                            jsonRequestMessage
                                        ));

                    }
                    else
                    {

                        jsonRequestMessage = new OCPP_JSONRequestMessage(
                                                 NetworkingNodeId,
                                                 NetworkPath,
                                                 RequestId,
                                                 Action,
                                                 JSONMessage,
                                                 ErrorMessage: "Invalid HTTP Web Socket connection!"
                                             );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    jsonRequestMessage = new OCPP_JSONRequestMessage(
                                             NetworkingNodeId,
                                             NetworkPath,
                                             RequestId,
                                             Action,
                                             JSONMessage,
                                             ErrorMessage: e.Message
                                         );

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }

            else
                jsonRequestMessage = new OCPP_JSONRequestMessage(
                                         NetworkingNodeId,
                                         NetworkPath,
                                         RequestId,
                                         Action,
                                         JSONMessage,
                                         ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                     );

            return jsonRequestMessage;

        }

        #endregion

        #region SendRequest(NetworkingNodeId, NetworkPath, Action, RequestId, BinaryMessage)

        public async Task<OCPP_BinaryRequestMessage> SendRequest(NetworkingNode_Id?  NetworkingNodeId,
                                                                 NetworkPath?        NetworkPath,
                                                                 String              Action,
                                                                 Request_Id          RequestId,
                                                                 Byte[]              BinaryMessage)
        {

            OCPP_BinaryRequestMessage? binaryRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   NetworkingNodeId ?? ChargeBoxIdentity,
                                                   NetworkPath      ?? NetworkPath.Empty,
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage
                                               );

                        await SendBinary(binaryRequestMessage.ToByteArray());

                        requests.TryAdd(RequestId,
                                        SendRequestState.FromBinaryRequest(
                                            Timestamp.Now,
                                            ChargeBoxIdentity,
                                            Timestamp.Now + RequestTimeout,
                                            binaryRequestMessage
                                        ));

                    }
                    else
                    {

                        binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                                   NetworkingNodeId,
                                                   NetworkPath,
                                                   RequestId,
                                                   Action,
                                                   BinaryMessage,
                                                   ErrorMessage: "Invalid HTTP Web Socket connection!"
                                               );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                               NetworkingNodeId,
                                               NetworkPath,
                                               RequestId,
                                               Action,
                                               BinaryMessage,
                                               ErrorMessage: e.Message
                                           );

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }

            else
                binaryRequestMessage = new OCPP_BinaryRequestMessage(
                                           NetworkingNodeId,
                                           NetworkPath,
                                           RequestId,
                                           Action,
                                           BinaryMessage,
                                           ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                       );

            return binaryRequestMessage;

        }

        #endregion


        #region (private) WaitForResponse(JSONRequestMessage)

        private async Task<SendRequestState> WaitForResponse(OCPP_JSONRequestMessage JSONRequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(JSONRequestMessage.RequestId, out var aSendRequestState) &&
                        aSendRequestState is SendRequestState sendRequestState &&
                       (sendRequestState?.JSONResponse is not null ||
                        sendRequestState?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(JSONRequestMessage.RequestId, out _);

                        return sendRequestState;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargePointWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromJSONRequest(

                       Timestamp.Now,
                       ChargeBoxIdentity,
                       endTime,
                       JSONRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion

        #region (private) WaitForResponse(BinaryRequestMessage)

        private async Task<SendRequestState> WaitForResponse(OCPP_BinaryRequestMessage BinaryRequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(BinaryRequestMessage.RequestId, out var aSendRequestState) &&
                        aSendRequestState is SendRequestState sendRequestState &&
                       (sendRequestState?.BinaryResponse is not null ||
                        sendRequestState?.ErrorCode.HasValue == true))
                    {

                        requests.TryRemove(BinaryRequestMessage.RequestId, out _);

                        return sendRequestState;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargePointWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return SendRequestState.FromBinaryRequest(

                       Timestamp.Now,
                       ChargeBoxIdentity,
                       endTime,
                       BinaryRequestMessage,

                       ErrorCode:  ResultCode.Timeout

                   );

        }

        #endregion


    }

}
