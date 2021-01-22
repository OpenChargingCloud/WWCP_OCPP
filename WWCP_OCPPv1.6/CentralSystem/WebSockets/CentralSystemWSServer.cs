/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system HTTP/WebSocket/JSON server.
    /// </summary>
    public class CentralSystemWSServer : WebSocketServer
    {

        public class SendRequestResult
        {

            public DateTime          Timestamp           { get; }
            public ChargeBox_Id      ClientId            { get; }
            public WSRequestMessage  WSRequestMessage    { get; }
            public DateTime          Timeout             { get; }
            public JObject           Response            { get; set; }
            public WSErrorCodes?     ErrorCode           { get; set; }
            public String            ErrorDescription    { get; set; }
            public JObject           ErrorDetails        { get; set; }

            public SendRequestResult(DateTime          Timestamp,
                                     ChargeBox_Id      ClientId,
                                     WSRequestMessage  WSRequestMessage,
                                     DateTime          Timeout)
            {

                this.Timestamp         = Timestamp;
                this.ClientId          = ClientId;
                this.WSRequestMessage  = WSRequestMessage;
                this.Timeout           = Timeout;

            }

        }


        #region Data

        /// <summary>
        /// The default HTTP/SOAP/XML server name.
        /// </summary>
        public const String                     DefaultHTTPServerName   = "GraphDefined OCPP " + Version.Number + " HTTP/WebSocket/JSON Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public static readonly IPPort           DefaultHTTPServerPort   = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public static readonly HTTPPath         DefaultURLPrefix        = HTTPPath.Parse("/" + Version.Number);

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public static readonly HTTPContentType  DefaultContentType      = HTTPContentType.XMLTEXT_UTF8;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly TimeSpan         DefaultRequestTimeout   = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        public List<SendRequestResult> requests;

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>               CustomBootNotificationRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom Heartbeat requests.
        /// </summary>
        public CustomJObjectParserDelegate<HeartbeatRequest>                      CustomHeartbeatRequestParser                        { get; set; }


        /// <summary>
        /// A delegate to parse custom Authorize requests.
        /// </summary>
        public CustomJObjectParserDelegate<AuthorizeRequest>                      CustomAuthorizeRequestParser                        { get; set; }

        /// <summary>
        /// A delegate to parse custom StartTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StartTransactionRequest>               CustomStartTransactionRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom StatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<StatusNotificationRequest>             CustomStatusNotificationRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom MeterValues requests.
        /// </summary>
        public CustomJObjectParserDelegate<MeterValuesRequest>                    CustomMeterValuesRequestParser                      { get; set; }

        /// <summary>
        /// A delegate to parse custom StopTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StopTransactionRequest>                CustomStopTransactionRequestParser                  { get; set; }


        /// <summary>
        /// A delegate to parse custom DataTransfer requests.
        /// </summary>
        public CustomJObjectParserDelegate<CP.DataTransferRequest>                CustomDataTransferRequestParser                     { get; set; }

        /// <summary>
        /// A delegate to parse custom DiagnosticsStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>  CustomDiagnosticsStatusNotificationRequestParser    { get; set; }

        /// <summary>
        /// A delegate to parse custom FirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>     CustomFirmwareStatusNotificationRequestParser       { get; set; }

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                 OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event BootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event BootNotificationDelegate          OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event BootNotificationResponseDelegate  OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler                  OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler          OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event HeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event HeartbeatDelegate          OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event HeartbeatResponseDelegate  OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler           OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler            OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate          OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize WebSocket Response was sent.
        /// </summary>
        public event AccessLogHandler             OnAuthorizeWSResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                   OnStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate          OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler                    OnStartTransactionWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                     OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate          OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler                      OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler              OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate          OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler               OnMeterValuesWSResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                  OnStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate          OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler                   OnStopTransactionWSResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                       OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate          OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler                        OnIncomingDataTransferWSResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                                OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate          OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler                                 OnDiagnosticsStatusNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification WebSocket Request was received.
        /// </summary>
        public event RequestLogHandler                             OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate          OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket Response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler                              OnFirmwareStatusNotificationWSResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region CentralSystemWSServer(HTTPServerName, TCPPort = default, URLPrefix = default, ContentType = default, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the central system HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer(String           HTTPServerName            = DefaultHTTPServerName,
                                     IPPort?          TCPPort                   = null,
                                     HTTPPath?        URLPrefix                 = null,
                                     HTTPContentType  ContentType               = null,
                                     Boolean          RegisterHTTPRootService   = true,
                                     DNSClient        DNSClient                 = null,
                                     Boolean          AutoStart                 = false)

            //: base(HTTPServerName.IsNotNullOrEmpty()
            //           ? HTTPServerName
            //           : DefaultHTTPServerName,
            //       TCPPort     ?? DefaultHTTPServerPort,
            //       URLPrefix   ?? DefaultURLPrefix,
            //       ContentType ?? DefaultContentType,
            //       RegisterHTTPRootService,
            //       DNSClient,
            //       AutoStart: false)
            : base(System.Net.IPAddress.Parse("127.0.0.1"),
                   TCPPort.HasValue ? TCPPort.Value.ToUInt16() : 8000)

        {

            this.requests = new List<SendRequestResult>();

            base.OnNewConnection += ProcessNewConnection;
            base.OnTextMessage   += ProcessTextMessages;

            //if (AutoStart)
            //    Start();

        }

        #endregion

        #region CentralSystemWSServer(URLPrefix = DefaultURLPrefix)

        /// <summary>
        /// Use the given HTTP server for the central system HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        public CentralSystemWSServer(HTTPPath?   URLPrefix = null)

            //: base(SOAPServer,
            //       URLPrefix ?? DefaultURLPrefix)
            : base(System.Net.IPAddress.Parse("127.0.0.1"), 8000)

        {

            this.requests = new List<SendRequestResult>();

            base.OnNewConnection += ProcessNewConnection;
            base.OnTextMessage   += ProcessTextMessages;

        }

        #endregion

        #endregion



        protected async Task ProcessNewConnection(DateTime             RequestTimestamp,
                                                  WebSocketConnection  Connection,
                                                  EventTracking_Id     EventTrackingId,
                                                  CancellationToken    CancellationToken)
        {

            Connection.AddCustomData("chargeBoxId",
                                     ChargeBox_Id.Parse(Connection.HTTPPath.ToString().Substring(Connection.HTTPPath.ToString().LastIndexOf("/") + 1)));

        }

        #region (protected) ProcessTextMessages(RequestTimestamp, Connection, EventTrackingId, CancellationToken, TextMessage)

        /// <summary>
        /// Process all text messages of this web socket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Connection">The web socket connection.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        /// <param name="TextMessage">The received text message.</param>
        protected async Task<WebSocketTextMessageRespose> ProcessTextMessages(DateTime             RequestTimestamp,
                                                                              WebSocketConnection  Connection,
                                                                              EventTracking_Id     EventTrackingId,
                                                                              CancellationToken    CancellationToken,
                                                                              String               TextMessage)
        {

            JArray         JSON           = null;
            WSErrorMessage ErrorMessage   = null;

            try
            {

                JSON = JArray.Parse(TextMessage);

                #region MessageType 2: CALL       (Client-to-Server)

                // [
                //     2,                  // MessageType: CALL (Client-to-Server)
                //    "19223201",          // RequestId
                //    "BootNotification",  // Action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (JSON.Count   == 4 &&
                    JSON[0].Type == JTokenType.Integer &&
                    JSON[0].Value<Byte>() == 2         &&
                    JSON[1].Type == JTokenType.String  &&
                    JSON[2].Type == JTokenType.String  &&
                    JSON[3].Type == JTokenType.Object)
                {

                    #region Initial checks

                    var RequestId    = Request_Id.TryParse(JSON[1]?.Value<String>());
                    var Action       = JSON[2].Value<String>()?.Trim();
                    var RequestData  = JSON[3].Value<JObject>();
                    var chargeBoxId  = Connection.TryGetCustomData<ChargeBox_Id>("chargeBoxId");

                    if (!RequestId.HasValue)
                        ErrorMessage  = new WSErrorMessage(
                                            Request_Id.Parse("0"),
                                            WSErrorCodes.ProtocolError,
                                            "The given 'request identification' must not be null or empty!",
                                            new JObject(
                                                new JProperty("request", TextMessage)
                                            ));

                    else if (Action.IsNullOrEmpty())
                        ErrorMessage  = new WSErrorMessage(
                                            RequestId.Value,
                                            WSErrorCodes.ProtocolError,
                                            "The given 'action' must not be null or empty!",
                                            new JObject(
                                                new JProperty("request", TextMessage)
                                            ));

                    else if (!chargeBoxId.HasValue)
                        ErrorMessage = new WSErrorMessage(
                                            RequestId.Value,
                                            WSErrorCodes.ProtocolError,
                                            "The given 'charge box identity' must not be null or empty!",
                                            new JObject(
                                                new JProperty("request", TextMessage)
                                            ));

                    #endregion

                    else
                    {

                        JObject OCPPResponseJSON  = default;
                        String  ErrorResponse     = default;

                        switch (Action)
                        {

                            #region BootNotification

                            case "BootNotification":
                                {

                                    #region Send OnBootNotificationWSRequest event

                                    try
                                    {

                                        //OnBootNotificationWSRequest?.Invoke(DateTime.UtcNow,
                                        //                                    this,
                                        //                                    RequestData;

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationWSRequest));
                                    }

                                    #endregion

                                    BootNotificationResponse response = null;

                                    try
                                    {

                                        if (BootNotificationRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out BootNotificationRequest  bootNotificationRequest,
                                                                             out                          ErrorResponse,
                                                                             CustomBootNotificationRequestParser))
                                        {

                                            #region Send OnBootNotificationRequest event

                                            try
                                            {

                                                OnBootNotificationRequest?.Invoke(DateTime.UtcNow,
                                                                                  this,
                                                                                  EventTrackingId,
                                                                                  bootNotificationRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnBootNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as BootNotificationDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         bootNotificationRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = BootNotificationResponse.Failed(bootNotificationRequest);

                                            }

                                            #endregion

                                            #region Send OnBootNotificationResponse event

                                            try
                                            {

                                                OnBootNotificationResponse?.Invoke(DateTime.UtcNow,
                                                                                   this,
                                                                                   EventTrackingId,
                                                                                   bootNotificationRequest,

                                                                                   response.Result,
                                                                                   response.Status,
                                                                                   response.CurrentTime,
                                                                                   response.Interval,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'BootNotification' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'BootNotification' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnBootNotificationWSResponse event

                                    try
                                    {

                                        //OnBootNotificationWSResponse?.Invoke(DateTime.UtcNow,
                                        //                                     this,
                                        //                                     RequestData,
                                        //                                     HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region Heartbeat

                            case "Heartbeat":
                                {

                                    #region Send OnHeartbeatWSRequest event

                                    try
                                    {

                                        //OnHeartbeatWSRequest?.Invoke(DateTime.UtcNow,
                                        //                                      SOAPServer.HTTPServer,
                                        //                                      Request);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatWSRequest));
                                    }

                                    #endregion

                                    HeartbeatResponse response = null;

                                    try
                                    {

                                        if (HeartbeatRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out HeartbeatRequest  heartbeatRequest,
                                                                      out                   ErrorResponse,
                                                                      CustomHeartbeatRequestParser))
                                        {

                                            #region Send OnHeartbeatRequest event

                                            try
                                            {

                                                OnHeartbeatRequest?.Invoke(DateTime.UtcNow,
                                                                           this,
                                                                           EventTrackingId,
                                                                           heartbeatRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnHeartbeat?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as HeartbeatDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         heartbeatRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = HeartbeatResponse.Failed(heartbeatRequest);

                                            }

                                            #endregion

                                            #region Send OnHeartbeatResponse event

                                            try
                                            {

                                                OnHeartbeatResponse?.Invoke(DateTime.UtcNow,
                                                                            this,
                                                                            EventTrackingId,
                                                                            heartbeatRequest,

                                                                            response.Result,
                                                                            response.CurrentTime,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'Heartbeat' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'Heartbeat' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnHeartbeatWSResponse event

                                    try
                                    {

                                        //OnHeartbeatWSResponse?.Invoke(HTTPResponse.Timestamp,
                                        //                                        SOAPServer.HTTPServer,
                                        //                                        Request,
                                        //                                        HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region Authorize

                            case "Authorize":
                                {

                                    #region Send OnAuthorizeWSRequest event

                                    try
                                    {

                                        //OnAuthorizeWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSRequest));
                                    }

                                    #endregion

                                    AuthorizeResponse response = null;

                                    try
                                    {

                                        if (AuthorizeRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out AuthorizeRequest  authorizeRequest,
                                                                      out                   ErrorResponse,
                                                                      CustomAuthorizeRequestParser))
                                        {

                                            #region Send OnAuthorizeRequest event

                                            try
                                            {

                                                OnAuthorizeRequest?.Invoke(DateTime.UtcNow,
                                                                           this,
                                                                           EventTrackingId,
                                                                           authorizeRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnAuthorize?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         authorizeRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = AuthorizeResponse.Failed(authorizeRequest);

                                            }

                                            #endregion

                                            #region Send OnAuthorizeResponse event

                                            try
                                            {

                                                OnAuthorizeResponse?.Invoke(DateTime.UtcNow,
                                                                            this,
                                                                            EventTrackingId,
                                                                            authorizeRequest,

                                                                            response.Result,
                                                                            response.IdTagInfo,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'Authorize' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'Authorize' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnAuthorizeWSResponse event

                                    try
                                    {

                                        //OnAuthorizeWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region StartTransaction

                            case "StartTransaction":
                                {

                                    #region Send OnStartTransactionWSRequest event

                                    try
                                    {

                                        //OnStartTransactionWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionWSRequest));
                                    }

                                    #endregion

                                    StartTransactionResponse response = null;

                                    try
                                    {

                                        if (StartTransactionRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out StartTransactionRequest  startTransactionRequest,
                                                                             out                          ErrorResponse,
                                                                             CustomStartTransactionRequestParser))
                                        {

                                            #region Send OnStartTransactionRequest event

                                            try
                                            {

                                                OnStartTransactionRequest?.Invoke(DateTime.UtcNow,
                                                                                  this,
                                                                                  EventTrackingId,
                                                                                  startTransactionRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnStartTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         startTransactionRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StartTransactionResponse.Failed(startTransactionRequest);

                                            }

                                            #endregion

                                            #region Send OnStartTransactionResponse event

                                            try
                                            {

                                                OnStartTransactionResponse?.Invoke(DateTime.UtcNow,
                                                                                   this,
                                                                                   EventTrackingId,
                                                                                   startTransactionRequest,

                                                                                   response.Result,
                                                                                   response.TransactionId,
                                                                                   response.IdTagInfo,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'StartTransaction' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'StartTransaction' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnStartTransactionWSResponse event

                                    try
                                    {

                                        //OnStartTransactionWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                            #region StatusNotification

                            case "StatusNotification":
                                {

                                    #region Send OnStatusNotificationWSRequest event

                                    try
                                    {

                                        //OnStatusNotificationWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    StatusNotificationResponse response = null;

                                    try
                                    {

                                        if (StatusNotificationRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out StatusNotificationRequest  statusNotificationRequest,
                                                                             out                            ErrorResponse,
                                                                             CustomStatusNotificationRequestParser))
                                        {

                                            #region Send OnStatusNotificationRequest event

                                            try
                                            {

                                                OnStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                                                    this,
                                                                                    EventTrackingId,
                                                                                    statusNotificationRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         statusNotificationRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StatusNotificationResponse.Failed(statusNotificationRequest);

                                            }

                                            #endregion

                                            #region Send OnStatusNotificationResponse event

                                            try
                                            {

                                                OnStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                                                     this,
                                                                                     EventTrackingId,
                                                                                     statusNotificationRequest,

                                                                                     response.Result,
                                                                                     response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'StatusNotification' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'StatusNotification' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnStatusNotificationWSResponse event

                                    try
                                    {

                                        //OnStatusNotificationWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                            #region MeterValues

                            case "MeterValues":
                                {

                                    #region Send OnMeterValuesWSRequest event

                                    try
                                    {

                                        //OnMeterValuesWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesWSRequest));
                                    }

                                    #endregion

                                    MeterValuesResponse response = null;

                                    try
                                    {

                                        if (MeterValuesRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out MeterValuesRequest  meterValuesRequest,
                                                                      out                   ErrorResponse,
                                                                      CustomMeterValuesRequestParser))
                                        {

                                            #region Send OnMeterValuesRequest event

                                            try
                                            {

                                                OnMeterValuesRequest?.Invoke(DateTime.UtcNow,
                                                                             this,
                                                                             EventTrackingId,
                                                                             meterValuesRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnMeterValues?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         meterValuesRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = MeterValuesResponse.Failed(meterValuesRequest);

                                            }

                                            #endregion

                                            #region Send OnMeterValuesResponse event

                                            try
                                            {

                                                OnMeterValuesResponse?.Invoke(DateTime.UtcNow,
                                                                              this,
                                                                              EventTrackingId,
                                                                              meterValuesRequest,

                                                                              response.Result,
                                                                              response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'MeterValues' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'MeterValues' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnMeterValuesWSResponse event

                                    try
                                    {

                                        //OnMeterValuesWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                            #region StopTransaction

                            case "StopTransaction":
                                {

                                    #region Send OnStopTransactionWSRequest event

                                    try
                                    {

                                        //OnStopTransactionWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSRequest));
                                    }

                                    #endregion

                                    StopTransactionResponse response = null;

                                    try
                                    {

                                        if (StopTransactionRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out StopTransactionRequest  stopTransactionRequest,
                                                                             out                         ErrorResponse,
                                                                             CustomStopTransactionRequestParser))
                                        {

                                            #region Send OnStopTransactionRequest event

                                            try
                                            {

                                                OnStopTransactionRequest?.Invoke(DateTime.UtcNow,
                                                                                 this,
                                                                                 EventTrackingId,
                                                                                 stopTransactionRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnStopTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         stopTransactionRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StopTransactionResponse.Failed(stopTransactionRequest);

                                            }

                                            #endregion

                                            #region Send OnStopTransactionResponse event

                                            try
                                            {

                                                OnStopTransactionResponse?.Invoke(DateTime.UtcNow,
                                                                                  this,
                                                                                  EventTrackingId,
                                                                                  stopTransactionRequest,

                                                                                  response.Result,
                                                                                  response.IdTagInfo,
                                                                                  response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'StopTransaction' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'StopTransaction' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnStopTransactionWSResponse event

                                    try
                                    {

                                        //OnStopTransactionWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion


                            #region IncomingDataTransfer

                            case "IncomingDataTransfer":
                                {

                                    #region Send OnIncomingDataTransferWSRequest event

                                    try
                                    {

                                        //OnIncomingDataTransferWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
                                    }

                                    #endregion

                                    CS.DataTransferResponse response = null;

                                    try
                                    {

                                        if (CP.DataTransferRequest.TryParse(RequestData,
                                                                            RequestId.  Value,
                                                                            chargeBoxId.Value,
                                                                            out CP.DataTransferRequest  dataTransferRequest,
                                                                            out                         ErrorResponse,
                                                                            CustomDataTransferRequestParser))
                                        {

                                            #region Send OnIncomingDataTransferRequest event

                                            try
                                            {

                                                OnIncomingDataTransferRequest?.Invoke(DateTime.UtcNow,
                                                                                      this,
                                                                                      EventTrackingId,
                                                                                      dataTransferRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnIncomingDataTransfer?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         dataTransferRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = CS.DataTransferResponse.Failed(dataTransferRequest);

                                            }

                                            #endregion

                                            #region Send OnIncomingDataTransferResponse event

                                            try
                                            {

                                                OnIncomingDataTransferResponse?.Invoke(DateTime.UtcNow,
                                                                                       this,
                                                                                       EventTrackingId,
                                                                                       dataTransferRequest,

                                                                                       response.Result,
                                                                                       response.Status,
                                                                                       response.Data,
                                                                                       response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'IncomingDataTransfer' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'IncomingDataTransfer' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnIncomingDataTransferWSResponse event

                                    try
                                    {

                                        //OnIncomingDataTransferWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                            #region DiagnosticsStatusNotification

                            case "DiagnosticsStatusNotification":
                                {

                                    #region Send OnDiagnosticsStatusNotificationWSRequest event

                                    try
                                    {

                                        //OnDiagnosticsStatusNotificationWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    DiagnosticsStatusNotificationResponse response = null;

                                    try
                                    {

                                        if (DiagnosticsStatusNotificationRequest.TryParse(RequestData,
                                                                                          RequestId.  Value,
                                                                                          chargeBoxId.Value,
                                                                                          out DiagnosticsStatusNotificationRequest  diagnosticsDiagnosticsStatusNotificationRequest,
                                                                                          out                                       ErrorResponse,
                                                                                          CustomDiagnosticsStatusNotificationRequestParser))
                                        {

                                            #region Send OnDiagnosticsStatusNotificationRequest event

                                            try
                                            {

                                                OnDiagnosticsStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                                                               this,
                                                                                               EventTrackingId,
                                                                                               diagnosticsDiagnosticsStatusNotificationRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnDiagnosticsStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         diagnosticsDiagnosticsStatusNotificationRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = DiagnosticsStatusNotificationResponse.Failed(diagnosticsDiagnosticsStatusNotificationRequest);

                                            }

                                            #endregion

                                            #region Send OnDiagnosticsStatusNotificationResponse event

                                            try
                                            {

                                                OnDiagnosticsStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                                                                this,
                                                                                                EventTrackingId,
                                                                                                diagnosticsDiagnosticsStatusNotificationRequest,

                                                                                                response.Result,
                                                                                                response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'DiagnosticsStatusNotification' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'DiagnosticsStatusNotification' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnDiagnosticsStatusNotificationWSResponse event

                                    try
                                    {

                                        //OnDiagnosticsStatusNotificationWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                            #region FirmwareStatusNotification

                            case "FirmwareStatusNotification":
                                {

                                    #region Send OnFirmwareStatusNotificationWSRequest event

                                    try
                                    {

                                        //OnFirmwareStatusNotificationWSRequest?.Invoke(DateTime.UtcNow,
                                        //                             this,
                                        //                             RequestData);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    FirmwareStatusNotificationResponse response = null;

                                    try
                                    {

                                        if (FirmwareStatusNotificationRequest.TryParse(RequestData,
                                                                                       RequestId.  Value,
                                                                                       chargeBoxId.Value,
                                                                                       out FirmwareStatusNotificationRequest  firmwareStatusNotificationRequest,
                                                                                       out                                    ErrorResponse,
                                                                                       CustomFirmwareStatusNotificationRequestParser))
                                        {

                                            #region Send OnFirmwareStatusNotificationRequest event

                                            try
                                            {

                                                OnFirmwareStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                                                            this,
                                                                                            EventTrackingId,
                                                                                            firmwareStatusNotificationRequest);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            if (response == null)
                                            {

                                                var results = OnFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                                                        (DateTime.UtcNow,
                                                                         this,
                                                                         CancellationToken,
                                                                         EventTrackingId,
                                                                         firmwareStatusNotificationRequest)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = FirmwareStatusNotificationResponse.Failed(firmwareStatusNotificationRequest);

                                            }

                                            #endregion

                                            #region Send OnFirmwareStatusNotificationResponse event

                                            try
                                            {

                                                OnFirmwareStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                                                             this,
                                                                                             EventTrackingId,
                                                                                             firmwareStatusNotificationRequest,

                                                                                             response.Result,
                                                                                             response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponseJSON = response.ToJSON();

                                        }

                                        else
                                            ErrorMessage =  new WSErrorMessage(RequestId.Value,
                                                                               WSErrorCodes.FormationViolation,
                                                                               "The given 'FirmwareStatusNotification' request could not be parsed!",
                                                                               new JObject(
                                                                                   new JProperty("request", TextMessage)
                                                                              ));

                                    }
                                    catch (Exception e)
                                    {

                                        ErrorMessage = new WSErrorMessage(RequestId.Value,
                                                                          WSErrorCodes.FormationViolation,
                                                                          "Processing the given 'FirmwareStatusNotification' request led to an exception!",
                                                                          new JObject(
                                                                              new JProperty("request",     TextMessage),
                                                                              new JProperty("exception",   e.Message),
                                                                              new JProperty("stacktrace",  e.StackTrace)
                                                                          ));

                                    }


                                    #region Send OnFirmwareStatusNotificationWSResponse event

                                    try
                                    {

                                        //OnFirmwareStatusNotificationWSResponse?.Invoke(DateTime.UtcNow,
                                        //                              this,
                                        //                              Request,
                                        //                              HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                        }

                        return new WebSocketTextMessageRespose(RequestTimestamp,
                                                               TextMessage,
                                                               DateTime.UtcNow,
                                                               (OCPPResponseJSON != null

                                                                    ? new WSResponseMessage(RequestId.Value,
                                                                                            OCPPResponseJSON).ToJSON()

                                                                    : new WSErrorMessage   (RequestId.Value,
                                                                                            WSErrorCodes.ProtocolError,
                                                                                            "Invalid action '" + Action + "'!",
                                                                                            new JObject(
                                                                                                new JProperty("data", TextMessage)
                                                                                            )).ToJSON()

                                                                ).ToString(Newtonsoft.Json.Formatting.None));

                    }

                }

                #endregion

                #region MessageType 3: CALLRESULT (Server-to-Client)

                // [
                //     3,                         // MessageType: CALLRESULT (Server-to-Client)
                //    "19223201",                 // RequestId copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                else if (JSON.Count   == 3 &&
                         JSON[0].Type == JTokenType.Integer &&
                         JSON[0].Value<Byte>() == 3 &&
                         JSON[1].Type == JTokenType.String &&
                         JSON[2].Type == JTokenType.Object)
                {

                    lock (requests)
                    {
                        var request = requests.FirstOrDefault(rr => rr.WSRequestMessage.RequestId == Request_Id.Parse(JSON[1]?.Value<String>()));
                        if (request != null)
                            request.Response = JSON[2] as JObject;
                    }

                }

                #endregion

                #region MessageType 4: CALLERROR  (Server-to-Client)

                // [
                //     4,                         // MessageType: CALLERROR (Server-to-Client)
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                // Error Code                    Description
                // -----------------------------------------------------------------------------------------------
                // NotImplemented                Requested Action is not known by receiver
                // NotSupported                  Requested Action is recognized but not supported by the receiver
                // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
                // ProtocolError                 Payload for Action is incomplete
                // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
                // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
                // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
                // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
                // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
                // GenericError                  Any other error not covered by the previous ones

                else if (JSON.Count   == 5 &&
                         JSON[0].Type == JTokenType.Integer &&
                         JSON[0].Value<Byte>() == 4 &&
                         JSON[1].Type == JTokenType.String &&
                         JSON[2].Type == JTokenType.String &&
                         JSON[3].Type == JTokenType.String &&
                         JSON[4].Type == JTokenType.Object)
                {

                    lock (requests)
                    {

                        var request = requests.FirstOrDefault(rr => rr.WSRequestMessage.RequestId == Request_Id.Parse(JSON[1]?.Value<String>()));
                        if (request != null)
                        {

                            if (Enum.TryParse(JSON[2]?.Value<String>(), out WSErrorCodes errorCode))
                                request.ErrorCode = errorCode;
                            else
                                request.ErrorCode = WSErrorCodes.GenericError;

                            request.ErrorDescription  = JSON[3]?.Value<String>();
                            request.ErrorDetails      = JSON[4] as JObject;

                        }

                    }

                }

                #endregion

                else
                    ErrorMessage = new WSErrorMessage(Request_Id.Parse(JSON.Count >= 2 ? JSON[1]?.Value<String>()?.Trim() : "unknown"),
                                                      WSErrorCodes.FormationViolation,
                                                      "The given OCPP request message is invalid!",
                                                      new JObject(
                                                          new JProperty("request", TextMessage)
                                                     ));

            }
            catch (Exception e)
            {

                ErrorMessage = new WSErrorMessage(Request_Id.Parse(JSON != null && JSON.Count >= 2
                                                                       ? JSON?[1].Value<String>()?.Trim()
                                                                       : "Unknown request identification"),
                                                  WSErrorCodes.FormationViolation,
                                                  "Processing the given OCPP request message led to an exception!",
                                                  new JObject(
                                                      new JProperty("request",     TextMessage),
                                                      new JProperty("exception",   e.Message),
                                                      new JProperty("stacktrace",  e.StackTrace)
                                                  ));

            }

            return new WebSocketTextMessageRespose(RequestTimestamp,
                                                   TextMessage,
                                                   DateTime.UtcNow,
                                                   ((ErrorMessage?.ToJSON()) ?? new JArray()).ToString());

        }

        #endregion


        public enum SendJSONResults
        {
            ok,
            unknownClient,
            failed
        }

        public async Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest  Request,
                                                                                 TimeSpan?                      Timeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           "RemoteStartTransaction",
                                           Request.ToJSON(),
                                           Timeout);

            if (result?.Response != null)
            {

                if (RemoteStartTransactionResponse.TryParse(Request,
                                                            result.Response,
                                                            out RemoteStartTransactionResponse remoteStartTransactionResponse))
                {
                    return remoteStartTransactionResponse;
                }

                return new RemoteStartTransactionResponse(Request,
                                                          RemoteStartStopStatus.Unknown);

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new RemoteStartTransactionResponse(Request,
                                                          RemoteStartStopStatus.Unknown);

            }

            return new RemoteStartTransactionResponse(Request,
                                                      RemoteStartStopStatus.Unknown);

        }

        public async Task<SendRequestResult> SendRequest(Request_Id    RequestId,
                                                         ChargeBox_Id  ClientId,
                                                         String        Action,
                                                         JObject       Request,
                                                         TimeSpan?     Timeout   = null)
        {

            var endTime = DateTime.UtcNow + (Timeout ?? TimeSpan.FromMinutes(3));

            var result = await SendJSON(RequestId,
                                        ClientId,
                                        Action,
                                        Request,
                                        endTime);

            if (result == SendJSONResults.ok)
            {

                do
                {

                    try
                    {

                        await Task.Delay(25);

                        var sendRequestResult = requests.FirstOrDefault(rr => rr.WSRequestMessage.RequestId == RequestId);

                        if (sendRequestResult?.Response           != null ||
                            sendRequestResult?.ErrorCode.HasValue == true)
                        {

                            lock (requests)
                            {
                                requests.Remove(sendRequestResult);
                            }

                            return sendRequestResult;

                        }

                    }
                    catch (Exception e)
                    { }

                }
                while (DateTime.UtcNow < endTime);

                lock (requests)
                {

                    var sendRequestResult = requests.FirstOrDefault(rr => rr.WSRequestMessage.RequestId == RequestId);

                    if (sendRequestResult != null)
                    {
                        sendRequestResult.ErrorCode = WSErrorCodes.Timeout;
                        requests.Remove(sendRequestResult);
                    }

                    return sendRequestResult;

                }

            }

            #region ..., or client/network error(s)

            else
            {
                lock (requests)
                {

                    var sendRequestResult = requests.FirstOrDefault(rr => rr.WSRequestMessage.RequestId == RequestId);

                    requests.Remove(sendRequestResult);

                    return sendRequestResult;

                }
            }

            #endregion

        }


        public async Task<SendJSONResults> SendJSON(Request_Id    RequestId,
                                                    ChargeBox_Id  ClientId,
                                                    String        Action,
                                                    JObject       Data,
                                                    DateTime      Timeout)
        {

            WSRequestMessage  request  = default;
            SendRequestResult result   = default;

            try
            {

                request  = new WSRequestMessage(RequestId,
                                                Action,
                                                Data);

                result   = new SendRequestResult(DateTime.UtcNow,
                                                 ClientId,
                                                 request,
                                                 Timeout);

                requests.Add(result);


                var webSocketConnection  = WebSocketConnections.FirstOrDefault(ws => ws.HTTPPath.ToString().EndsWith(ClientId.ToString()));

                if (webSocketConnection == default)
                {
                    result.ErrorCode = WSErrorCodes.UnknownClient;
                    return SendJSONResults.unknownClient;
                }

                var networkStream        = webSocketConnection.TcpClient.GetStream();
                var WSFrame              = new WebSocketFrame(WebSocketFrame.Fin.Final,
                                                              WebSocketFrame.MaskStatus.Off,
                                                              new Byte[4],
                                                              WebSocketFrame.Opcodes.Text,
                                                              request.ToJSON().ToString(Newtonsoft.Json.Formatting.None).ToUTF8Bytes(),
                                                              WebSocketFrame.Rsv.Off,
                                                              WebSocketFrame.Rsv.Off,
                                                              WebSocketFrame.Rsv.Off);

                await networkStream.WriteAsync(WSFrame.ToByteArray());

                return SendJSONResults.ok;

            }
            catch (Exception e)
            {
                result.ErrorCode = WSErrorCodes.NetworkError;
                return SendJSONResults.failed;
            }

        }

    }

}
