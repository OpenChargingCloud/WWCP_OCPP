/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{


    public delegate Task OnNewCentralSystemWSConnectionDelegate(DateTime             Timestamp,
                                                                ICentralSystem       CentralSystem,
                                                                WebSocketConnection  NewWebSocketConnection,
                                                                EventTracking_Id     EventTrackingId,
                                                                CancellationToken    CancellationToken);


    /// <summary>
    /// The central system HTTP/WebSocket/JSON server.
    /// </summary>
    public class CentralSystemWSServer : WebSocketServer,
                                         ICentralSystem
    {

        #region (enum)  SendJSONResults

        public enum SendJSONResults
        {
            ok,
            unknownClient,
            failed
        }

        #endregion

        #region (class) SendRequestState

        public class SendRequestState
        {

            public DateTime                       Timestamp           { get; }
            public ChargeBox_Id                   ChargeBoxId         { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage    { get; }
            public DateTime                       Timeout             { get; }

            public JObject?                       Response            { get; set; }
            public OCPP_WebSocket_ErrorCodes?     ErrorCode           { get; set; }
            public String?                        ErrorDescription    { get; set; }
            public JObject?                       ErrorDetails        { get; set; }

            public SendRequestState(DateTime                       Timestamp,
                                    ChargeBox_Id                   ChargeBoxId,
                                    OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                    DateTime                       Timeout,

                                    JObject?                       Response           = null,
                                    OCPP_WebSocket_ErrorCodes?     ErrorCode          = null,
                                    String?                        ErrorDescription   = null,
                                    JObject?                       ErrorDetails       = null)
            {

                this.Timestamp         = Timestamp;
                this.ChargeBoxId       = ChargeBoxId;
                this.WSRequestMessage  = WSRequestMessage;
                this.Timeout           = Timeout;

                this.Response          = Response;
                this.ErrorCode         = ErrorCode;
                this.ErrorDescription  = ErrorDescription;
                this.ErrorDetails      = ErrorDetails;

            }

        }

        #endregion


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


        private readonly Dictionary<ChargeBox_Id, Tuple<WebSocketConnection, DateTime>> connectedChargingBoxes;

        private const String LogfileName = "CentralSystemWSServer.log";

        private const Newtonsoft.Json.Formatting JSONFormating = Newtonsoft.Json.Formatting.None;

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id { get; }


        public readonly Dictionary<Request_Id, SendRequestState> requests;


        public IEnumerable<ChargeBox_Id> ChargeBoxIds
            => connectedChargingBoxes.Keys;

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean                                                            RequireAuthentication                               { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public Dictionary<String, String?>                                        ChargingBoxLogins                                   { get; }

        #endregion

        #region Events

        public event OnNewCentralSystemWSConnectionDelegate? OnNewCentralSystemWSConnection;


        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?          OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event BootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate?          OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event BootNotificationResponseDelegate?    OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a boot notification was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?         OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?   OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event HeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?          OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event HeartbeatResponseDelegate?    OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a heartbeat was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?  OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?   OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event AuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?          OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event AuthorizeResponseDelegate?    OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize web socket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?  OnAuthorizeWSResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?        OnStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event StartTransactionRequestDelegate?   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate?        OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event StartTransactionResponseDelegate?  OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a start transaction request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?       OnStartTransactionWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?          OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event StatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?        OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event StatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?         OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?   OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event MeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate?        OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event MeterValuesResponseDelegate?  OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a meter values request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?  OnMeterValuesWSResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?       OnStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event StopTransactionRequestDelegate?   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate?        OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event StopTransactionResponseDelegate?  OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a stop transaction request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?      OnStopTransactionWSResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event IncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?        OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event IncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a data transfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnIncomingDataTransferWSResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event DiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate?        OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event DiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a diagnostics status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnDiagnosticsStatusNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event FirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?        OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event FirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a firmware status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnFirmwareStatusNotificationWSResponse;

        #endregion

        #endregion

        #region Custom JSON parser/serializer delegates

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>?               CustomBootNotificationRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom Heartbeat requests.
        /// </summary>
        public CustomJObjectParserDelegate<HeartbeatRequest>?                      CustomHeartbeatRequestParser                        { get; set; }


        /// <summary>
        /// A delegate to parse custom Authorize requests.
        /// </summary>
        public CustomJObjectParserDelegate<AuthorizeRequest>?                      CustomAuthorizeRequestParser                        { get; set; }

        /// <summary>
        /// A delegate to parse custom StartTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StartTransactionRequest>?               CustomStartTransactionRequestParser                 { get; set; }

        /// <summary>
        /// A delegate to parse custom StatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<StatusNotificationRequest>?             CustomStatusNotificationRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom MeterValues requests.
        /// </summary>
        public CustomJObjectParserDelegate<MeterValuesRequest>?                    CustomMeterValuesRequestParser                      { get; set; }

        /// <summary>
        /// A delegate to parse custom StopTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StopTransactionRequest>?                CustomStopTransactionRequestParser                  { get; set; }


        /// <summary>
        /// A delegate to parse custom DataTransfer requests.
        /// </summary>
        public CustomJObjectParserDelegate<CP.DataTransferRequest>?                CustomDataTransferRequestParser                     { get; set; }

        /// <summary>
        /// A delegate to parse custom DiagnosticsStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestParser    { get; set; }

        /// <summary>
        /// A delegate to parse custom FirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?     CustomFirmwareStatusNotificationRequestParser       { get; set; }



        public CustomJObjectSerializerDelegate<ResetRequest>?                   CustomResetRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?      CustomChangeAvailabilityRequestSerializer        { get; set; }

        public CustomJObjectSerializerDelegate<GetConfigurationRequest>?        CustomGetConfigurationRequestSerializer          { get; set; }

        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?     CustomChangeConfigurationRequestSerializer       { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?            CustomDataTransferRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<GetDiagnosticsRequest>?          CustomGetDiagnosticsRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?          CustomTriggerMessageRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?          CustomUpdateFirmwareRequestSerializer            { get; set; }



        public CustomJObjectSerializerDelegate<ReserveNowRequest>?              CustomReserveNowRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>?       CustomCancelReservationRequestSerializer         { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?  CustomRemoteStartTransactionRequestSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                CustomChargingProfileSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?               CustomChargingScheduleSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?         CustomChargingSchedulePeriodSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>?   CustomRemoteStopTransactionRequestSerializer     { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?      CustomSetChargingProfileRequestSerializer        { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?    CustomClearChargingProfileRequestSerializer      { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?    CustomGetCompositeScheduleRequestSerializer      { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?         CustomUnlockConnectorRequestSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?     CustomGetLocalListVersionRequestSerializer       { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?           CustomSendLocalListRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?              CustomAuthorizationDataSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                      CustomIdTagInfoResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>?              CustomClearCacheRequestSerializer                { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize a new HTTP server for the central system HTTP/WebSocket/JSON API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer(String       HTTPServerName          = DefaultHTTPServerName,
                                     IIPAddress?  IPAddress               = null,
                                     IPPort?      TCPPort                 = null,
                                     Boolean      RequireAuthentication   = true,
                                     DNSClient?   DNSClient               = null,
                                     Boolean      AutoStart               = false)

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   HTTPServerName,
                   DNSClient,
                   false)

        {

            this.requests                        = new Dictionary<Request_Id, SendRequestState>();
            this.RequireAuthentication           = RequireAuthentication;
            this.ChargingBoxLogins               = new Dictionary<String, String?>();
            this.connectedChargingBoxes          = new Dictionary<ChargeBox_Id, Tuple<WebSocketConnection, DateTime>>();

            base.OnValidateWebSocketConnection  += ValidateWebSocketConnection;
            base.OnNewWebSocketConnection       += ProcessNewWebSocketConnection;
            //base.OnTextMessage                  += ProcessTextMessages;

            if (AutoStart)
                Start();

        }

        #endregion


        #region (protected) ValidateWebSocketConnection  (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private async Task<HTTPResponse?> ValidateWebSocketConnection(DateTime             LogTimestamp,
                                                                      WebSocketServer      Server,
                                                                      WebSocketConnection  Connection,
                                                                      EventTracking_Id     EventTrackingId,
                                                                      CancellationToken    CancellationToken)
        {

            #region Verify 'Sec-WebSocket-Protocol'...

            var secWebSocketProtocols = Connection.Request.SecWebSocketProtocol?.Split(',')?.Select(protocol => protocol?.Trim()).ToArray();

            if (secWebSocketProtocols is null)
            {

                DebugX.Log("Missing 'Sec-WebSocket-Protocol' HTTP header!");

                return new HTTPResponse.Builder(HTTPStatusCode.BadRequest) {
                           Server       = HTTPServiceName,
                           Date         = Timestamp.Now,
                           ContentType  = HTTPContentType.JSON_UTF8,
                           Content      = JSONObject.Create(
                                              new JProperty("description",
                                              JSONObject.Create(
                                                  new JProperty("en", "Missing 'Sec-WebSocket-Protocol' HTTP header!")
                                              ))).ToUTF8Bytes(),
                           Connection   = "close"
                       }.AsImmutable;

            }
            else if (!secWebSocketProtocols.Contains("ocpp1.6"))
            {

                DebugX.Log("This web socket service only supports 'ocpp1.6'!");

                return new HTTPResponse.Builder(HTTPStatusCode.BadRequest) {
                           Server       = HTTPServiceName,
                           Date         = Timestamp.Now,
                           ContentType  = HTTPContentType.JSON_UTF8,
                           Content      = JSONObject.Create(
                                              new JProperty("description",
                                                  JSONObject.Create(
                                                      new JProperty("en", "This web socket service only supports 'ocpp1.6'!")
                                              ))).ToUTF8Bytes(),
                           Connection   = "close"
                       }.AsImmutable;

            }

            #endregion

            #region Verify HTTP Authentication

            if (RequireAuthentication)
            {

                if (Connection.Request.Authorization is HTTPBasicAuthentication basicAuthentication)
                {

                    if (ChargingBoxLogins.TryGetValue(basicAuthentication.Username, out String? Password) &&
                        basicAuthentication.Password == Password)
                    {
                        DebugX.Log(nameof(CentralSystemWSServer), " connection from " + Connection.RemoteSocket + " using authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);
                        return null;
                    }
                    else
                        DebugX.Log(nameof(CentralSystemWSServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(CentralSystemWSServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return new HTTPResponse.Builder(HTTPStatusCode.Unauthorized) {
                           Server      = HTTPServiceName,
                           Date        = Timestamp.Now,
                           Connection  = "close"
                       }.AsImmutable;

            }

            #endregion

            return null;

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        protected Task ProcessNewWebSocketConnection(DateTime             LogTimestamp,
                                                     WebSocketServer      Server,
                                                     WebSocketConnection  Connection,
                                                     EventTracking_Id     EventTrackingId,
                                                     CancellationToken    CancellationToken)
        {

            if (!Connection.HasCustomData("chargeBoxId") &&
                ChargeBox_Id.TryParse(Connection.Request.Path.ToString().Substring(Connection.Request.Path.ToString().LastIndexOf("/") + 1), out var chargeBoxId))
            {
                Connection.AddCustomData("chargeBoxId", chargeBoxId);
                connectedChargingBoxes.Add(chargeBoxId, new Tuple<WebSocketConnection, DateTime>(Connection, Timestamp.Now));
            }

            var OnNewCentralSystemWSConnectionLocal = OnNewCentralSystemWSConnection;
            if (OnNewCentralSystemWSConnectionLocal is not null)
            {

                OnNewCentralSystemWSConnection?.Invoke(LogTimestamp,
                                                       this,
                                                       Connection,
                                                       EventTrackingId,
                                                       CancellationToken);

            }

            return Task.CompletedTask;

        }

        #endregion


        #region (protected) ProcessTextMessages          (Connection, EventTrackingId, OCPPTextMessage, CancellationToken)

        /// <summary>
        /// Process all text messages of this web socket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="Connection">The web socket connection.</param>
        /// <param name="OCPPMessage">The received OCPP message.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        public override async Task<WebSocketTextMessageResponse> ProcessTextMessage(DateTime             RequestTimestamp,
                                                                                    EventTracking_Id     EventTrackingId,
                                                                                    WebSocketConnection  Connection,
                                                                                    String               OCPPMessage,
                                                                                    CancellationToken    CancellationToken)
        {

            if (OCPPMessage.Trim().IsNullOrEmpty())
            {

                DebugX.Log(nameof(CentralSystemWSServer) + " The given OCPP message must not be null or empty!");

                // "No response" to the charge point!
                return new WebSocketTextMessageResponse(
                           EventTrackingId,
                           RequestTimestamp,
                           OCPPMessage,
                           Timestamp.Now,
                           new JArray().ToString(JSONFormating)
                       );

            }

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                var JSON = JArray.Parse(OCPPMessage);

                File.AppendAllText(LogfileName,
                                   String.Concat("Timestamp: ",        Timestamp.Now.ToIso8601(),                                                    Environment.NewLine,
                                                 "ChargeBoxId: ",      Connection.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId")?.ToString() ?? "-",  Environment.NewLine,
                                                 "Message received: ", JSON.ToString(Newtonsoft.Json.Formatting.Indented),                           Environment.NewLine,
                                                 "--------------------------------------------------------------------------------------------",     Environment.NewLine));

                #region MessageType 2: CALL        (A request from a charge point)

                // [
                //     2,                  // MessageType: CALL
                //    "19223201",          // A unique request identification
                //    "BootNotification",  // The OCPP action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (JSON.Count             == 4                   &&
                    JSON[0].Type           == JTokenType.Integer  &&
                    JSON[0].Value<Byte>()  == 2                   &&
                    JSON[1].Type == JTokenType.String             &&
                    JSON[2].Type == JTokenType.String             &&
                    JSON[3].Type == JTokenType.Object)
                {

                    #region Initial checks

                    var chargeBoxId  = Connection.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId");
                    var requestId    = Request_Id.TryParse(JSON[1]?.Value<String>() ?? "");
                    var action       = JSON[2]?.Value<String>()?.Trim();
                    var requestData  = JSON[3]?.Value<JObject>();

                    if (!chargeBoxId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId ?? Request_Id.Parse("0"),
                                                 OCPP_WebSocket_ErrorCodes.ProtocolError,
                                                 "The given 'charge box identity' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPMessage)
                                                 )
                                             );

                    else if (!requestId.HasValue)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 Request_Id.Parse("0"),
                                                 OCPP_WebSocket_ErrorCodes.ProtocolError,
                                                 "The given 'request identification' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPMessage)
                                                 )
                                             );

                    else if (action is null || action.IsNullOrEmpty())
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 OCPP_WebSocket_ErrorCodes.ProtocolError,
                                                 "The given 'action' must not be null or empty!",
                                                 new JObject(
                                                     new JProperty("request", OCPPMessage)
                                                 )
                                             );

                    else if (requestData is null)
                        OCPPErrorResponse  = new OCPP_WebSocket_ErrorMessage(
                                                 requestId.Value,
                                                 OCPP_WebSocket_ErrorCodes.ProtocolError,
                                                 "The given request JSON payload must not be null!",
                                                 new JObject(
                                                     new JProperty("request", OCPPMessage)
                                                 )
                                             );

                    #endregion

                    else
                    {

                        switch (action)
                        {

                            #region BootNotification

                            case "BootNotification":
                                {

                                    #region Send OnBootNotificationWSRequest event

                                    try
                                    {

                                        OnBootNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (BootNotificationRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomBootNotificationRequestParser) && request is not null) {

                                            #region Send OnBootNotificationRequest event

                                            try
                                            {

                                                OnBootNotificationRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            BootNotificationResponse? response = null;

                                            var responseTasks = OnBootNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = BootNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnBootNotificationResponse event

                                            try
                                            {

                                                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'BootNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'BootNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnBootNotificationWSResponse event

                                    try
                                    {

                                        OnBootNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             JSON,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationWSResponse));
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

                                        OnHeartbeatWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (HeartbeatRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargeBoxId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomHeartbeatRequestParser) && request is not null) {

                                            #region Send OnHeartbeatRequest event

                                            try
                                            {

                                                OnHeartbeatRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            HeartbeatResponse? response = null;

                                            var responseTasks = OnHeartbeat?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = HeartbeatResponse.Failed(request);

                                            #endregion

                                            #region Send OnHeartbeatResponse event

                                            try
                                            {

                                                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'Heartbeat' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'Heartbeat' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnHeartbeatWSResponse event

                                    try
                                    {

                                        OnHeartbeatWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      JSON,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnHeartbeatWSResponse));
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

                                        OnAuthorizeWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (AuthorizeRequest.TryParse(requestData,
                                                                      requestId.Value,
                                                                      chargeBoxId.Value,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomAuthorizeRequestParser) && request is not null) {

                                            #region Send OnAuthorizeRequest event

                                            try
                                            {

                                                OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            AuthorizeResponse? response = null;

                                            var responseTasks = OnAuthorize?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = AuthorizeResponse.Failed(request);

                                            #endregion

                                            #region Send OnAuthorizeResponse event

                                            try
                                            {

                                                OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'Authorize' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'Authorize' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnAuthorizeWSResponse event

                                    try
                                    {

                                        OnAuthorizeWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      JSON,
                                                                      OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSResponse));
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

                                        OnStartTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (StartTransactionRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomStartTransactionRequestParser) && request is not null) {

                                            #region Send OnStartTransactionRequest event

                                            try
                                            {

                                                OnStartTransactionRequest?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            StartTransactionResponse? response = null;

                                            var responseTasks = OnStartTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = StartTransactionResponse.Failed(request);

                                            #endregion

                                            #region Send OnStartTransactionResponse event

                                            try
                                            {

                                                OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                                                   this,
                                                                                   request,
                                                                                   response,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'StartTransaction' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'StartTransaction' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                ));

                                    }

                                    #region Send OnStartTransactionWSResponse event

                                    try
                                    {

                                        OnStartTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             JSON,
                                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStartTransactionWSResponse));
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

                                        OnStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (StatusNotificationRequest.TryParse(requestData,
                                                                               requestId.Value,
                                                                               chargeBoxId.Value,
                                                                               out var request,
                                                                               out var errorResponse,
                                                                               CustomStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnStatusNotificationRequest event

                                            try
                                            {

                                                OnStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            StatusNotificationResponse? response = null;

                                            var responseTasks = OnStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = StatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnStatusNotificationResponse event

                                            try
                                            {

                                                OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request,
                                                                                     response,
                                                                                     response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'StatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'StatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnStatusNotificationWSResponse event

                                    try
                                    {

                                        OnStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               JSON,
                                                                               OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStatusNotificationWSResponse));
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

                                        OnMeterValuesWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (MeterValuesRequest.TryParse(requestData,
                                                                        requestId.Value,
                                                                        chargeBoxId.Value,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomMeterValuesRequestParser) && request is not null) {

                                            #region Send OnMeterValuesRequest event

                                            try
                                            {

                                                OnMeterValuesRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            MeterValuesResponse? response = null;

                                            var responseTasks = OnMeterValues?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = MeterValuesResponse.Failed(request);

                                            #endregion

                                            #region Send OnMeterValuesResponse event

                                            try
                                            {

                                                OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'MeterValues' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'MeterValues' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",     OCPPMessage),
                                                                    new JProperty("exception",   e.Message),
                                                                    new JProperty("stacktrace",  e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnMeterValuesWSResponse event

                                    try
                                    {

                                        OnMeterValuesWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        JSON,
                                                                        OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnMeterValuesWSResponse));
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

                                        OnStopTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (StopTransactionRequest.TryParse(requestData,
                                                                             requestId.Value,
                                                                             chargeBoxId.Value,
                                                                             out var request,
                                                                             out var errorResponse,
                                                                             CustomStopTransactionRequestParser) && request is not null) {

                                            #region Send OnStopTransactionRequest event

                                            try
                                            {

                                                OnStopTransactionRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            StopTransactionResponse? response = null;

                                            var responseTasks = OnStopTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = StopTransactionResponse.Failed(request);

                                            #endregion

                                            #region Send OnStopTransactionResponse event

                                            try
                                            {

                                                OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'StopTransaction' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'StopTransaction' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnStopTransactionWSResponse event

                                    try
                                    {

                                        OnStopTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            JSON,
                                                                            OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            #region IncomingDataTransfer

                            case "DataTransfer":
                                {

                                    #region Send OnIncomingDataTransferWSRequest event

                                    try
                                    {

                                        OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (CP.DataTransferRequest.TryParse(requestData,
                                                                            requestId.Value,
                                                                            chargeBoxId.Value,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomDataTransferRequestParser) && request is not null) {

                                            #region Send OnIncomingDataTransferRequest event

                                            try
                                            {

                                                OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            DataTransferResponse? response = null;

                                            var responseTasks = OnIncomingDataTransfer?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = DataTransferResponse.Failed(request);

                                            #endregion

                                            #region Send OnIncomingDataTransferResponse event

                                            try
                                            {

                                                OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       request,
                                                                                       response,
                                                                                       response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'IncomingDataTransfer' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'IncomingDataTransfer' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnIncomingDataTransferWSResponse event

                                    try
                                    {

                                        OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 JSON,
                                                                                 OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
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

                                        OnDiagnosticsStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                         this,
                                                                                         JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (DiagnosticsStatusNotificationRequest.TryParse(requestData,
                                                                                          requestId.Value,
                                                                                          chargeBoxId.Value,
                                                                                          out var request,
                                                                                          out var errorResponse,
                                                                                          CustomDiagnosticsStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnDiagnosticsStatusNotificationRequest event

                                            try
                                            {

                                                OnDiagnosticsStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                               this,
                                                                                               request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            DiagnosticsStatusNotificationResponse? response = null;

                                            var responseTasks = OnDiagnosticsStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = DiagnosticsStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnDiagnosticsStatusNotificationResponse event

                                            try
                                            {

                                                OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                                this,
                                                                                                request,
                                                                                                response,
                                                                                                response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'DiagnosticsStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'DiagnosticsStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnDiagnosticsStatusNotificationWSResponse event

                                    try
                                    {

                                        OnDiagnosticsStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                          this,
                                                                                          JSON,
                                                                                          OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSResponse));
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

                                        OnFirmwareStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationWSRequest));
                                    }

                                    #endregion

                                    try
                                    {

                                        if (FirmwareStatusNotificationRequest.TryParse(requestData,
                                                                                       requestId.Value,
                                                                                       chargeBoxId.Value,
                                                                                       out var request,
                                                                                       out var errorResponse,
                                                                                       CustomFirmwareStatusNotificationRequestParser) && request is not null) {

                                            #region Send OnFirmwareStatusNotificationRequest event

                                            try
                                            {

                                                OnFirmwareStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                                            this,
                                                                                            request);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                                            }

                                            #endregion

                                            #region Call async subscribers

                                            FirmwareStatusNotificationResponse? response = null;

                                            var responseTasks = OnFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                            if (responseTasks?.Length > 0)
                                            {
                                                await Task.WhenAll(responseTasks);
                                                response = responseTasks.FirstOrDefault()?.Result;
                                            }

                                            if (responseTasks is null || response is null)
                                                response = FirmwareStatusNotificationResponse.Failed(request);

                                            #endregion

                                            #region Send OnFirmwareStatusNotificationResponse event

                                            try
                                            {

                                                OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                                             this,
                                                                                             request,
                                                                                             response,
                                                                                             response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                                            }

                                            #endregion

                                            OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                                               requestId.Value,
                                                               response.ToJSON()
                                                           );

                                        }

                                        else
                                            OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                    requestId.Value,
                                                                    OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                    "The given 'FirmwareStatusNotification' request could not be parsed!",
                                                                    new JObject(
                                                                        new JProperty("request",       OCPPMessage),
                                                                        new JProperty("errorResponse", errorResponse)
                                                                    )
                                                                );

                                    }
                                    catch (Exception e)
                                    {

                                        OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                                                requestId.Value,
                                                                OCPP_WebSocket_ErrorCodes.FormationViolation,
                                                                "Processing the given 'FirmwareStatusNotification' request led to an exception!",
                                                                JSONObject.Create(
                                                                    new JProperty("request",    OCPPMessage),
                                                                    new JProperty("exception",  e.Message),
                                                                    new JProperty("stacktrace", e.StackTrace)
                                                                )
                                                            );

                                    }

                                    #region Send OnFirmwareStatusNotificationWSResponse event

                                    try
                                    {

                                        OnFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       JSON,
                                                                                       OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion


                            default:

                                // It does not make much sense to send this error to a charging station as no one will read it there!
                                DebugX.Log(nameof(CentralSystemWSServer) + " The OCPP message action '" + action + "' is unkown!");

                                //OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                //                   requestId.Value,
                                //                   new JObject()
                                //               );

                                break;

                        }

                    }

                }

                #endregion

                #region MessageType 3: CALLRESULT  (A response from charge point)

                // [
                //     3,                         // MessageType: CALLRESULT
                //    "19223201",                 // The request identification copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                else if (JSON.Count             == 3         &&
                         JSON[0].Type == JTokenType.Integer  &&
                         JSON[0].Value<Byte>()  == 3         &&
                         JSON[1].Type == JTokenType.String   &&
                         JSON[2].Type == JTokenType.Object)
                {
                    lock (requests)
                    {
                        if (Request_Id.TryParse(JSON[1]?.Value<String>() ?? "", out var requestId) &&
                            requests.TryGetValue(requestId, out var request))
                        {
                            request.Response = JSON[2] as JObject;
                        }
                    }
                }

                #endregion

                #region MessageType 4: CALLERROR   (A charge point reports an error on a received request)

                // [
                //     4,                         // MessageType: CALLERROR
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

                else if (JSON.Count             == 5                   &&
                         JSON[0].Type           == JTokenType.Integer  &&
                         JSON[0].Value<Byte>()  == 4                   &&
                         JSON[1].Type == JTokenType.String             &&
                         JSON[2].Type == JTokenType.String             &&
                         JSON[3].Type == JTokenType.String             &&
                         JSON[4].Type == JTokenType.Object)
                {

                    lock (requests)
                    {
                        if (Request_Id.TryParse(JSON[1]?.Value<String>() ?? "", out var requestId) &&
                            requests.TryGetValue(requestId, out var request))
                        {

                            // ToDo: Refactor 
                            if (OCPP_WebSocket_ErrorCodes.TryParse(JSON[2]?.Value<String>() ?? "", out var errorCode))
                                request.ErrorCode = errorCode;
                            else
                                request.ErrorCode = OCPP_WebSocket_ErrorCodes.GenericError;

                            request.Response          = null;
                            request.ErrorDescription  = JSON[3]?.Value<String>();
                            request.ErrorDetails      = JSON[4] as JObject;

                        }
                    }

                    //// No response to the charge point!
                    //return null;

                }

                #endregion

                else
                {

                    // It does not make much sense to send this error to a charging station as no one will read it there!
                    DebugX.Log(nameof(CentralSystemWSServer) + " The OCPP message '" + OCPPMessage + "' is invalid!");

                    //new WSErrorMessage(Request_Id.Parse(JSON.Count >= 2 ? JSON[1]?.Value<String>()?.Trim() : "unknown"),
                    //                                  WSErrorCodes.FormationViolation,
                    //                                  "The given OCPP request message is invalid!",
                    //                                  new JObject(
                    //                                      new JProperty("request", TextMessage)
                    //                                 ));

                    //// No response to the charge point!
                    //return null;

                }

            }
            catch (Exception e)
            {

                // It does not make much sense to send this error to a charging station as no one will read it there!
                DebugX.LogException(e, "The OCPP message '" + OCPPMessage + "' received in " + nameof(CentralSystemWSServer) + " led to an exception!");

                //ErrorMessage = new WSErrorMessage(Request_Id.Parse(JSON != null && JSON.Count >= 2
                //                                                       ? JSON?[1].Value<String>()?.Trim()
                //                                                       : "Unknown request identification"),
                //                                  WSErrorCodes.FormationViolation,
                //                                  "Processing the given OCPP request message led to an exception!",
                //                                  new JObject(
                //                                      new JProperty("request",     TextMessage),
                //                                      new JProperty("exception",   e.Message),
                //                                      new JProperty("stacktrace",  e.StackTrace)
                //                                  ));

            }

            // The response to the charge point...
            return new WebSocketTextMessageResponse(
                       EventTrackingId,
                       RequestTimestamp,
                       OCPPMessage,
                       Timestamp.Now,
                       (OCPPResponse?.     ToJSON() ??
                        OCPPErrorResponse?.ToJSON() ??
                        new JArray())?.ToString(JSONFormating) ?? ""
                   );

        }

        #endregion



        #region SendRequest(RequestId, ChargeBoxId, OCPPAction, JSONPayload, Timeout = null)

        public async Task<SendRequestState> SendRequest(Request_Id    RequestId,
                                                        ChargeBox_Id  ChargeBoxId,
                                                        String        OCPPAction,
                                                        JObject       JSONPayload,
                                                        TimeSpan?     Timeout   = null)
        {

            var endTime         = Timestamp.Now + (Timeout ?? TimeSpan.FromSeconds(10));

            var sendJSONResult  = await SendJSON(RequestId,
                                                 ChargeBoxId,
                                                 OCPPAction,
                                                 JSONPayload,
                                                 endTime);

            if (sendJSONResult == SendJSONResults.ok)
            {

                #region Wait for a response... till timeout

                do
                {

                    try
                    {

                        await Task.Delay(25);

                        if (requests.TryGetValue(RequestId, out var sendRequestState) &&
                            sendRequestState?.Response is not null ||
                            sendRequestState?.ErrorCode.HasValue == true)
                        {

                            lock (requests)
                            {
                                requests.Remove(RequestId);
                            }

                            return sendRequestState;

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(String.Concat(nameof(CentralSystemWSServer), ".", nameof(SendRequest), " exception occured: ", e.Message));
                    }

                }
                while (Timestamp.Now < endTime);

                #endregion

                #region When timeout...

                lock (requests)
                {
                    if (requests.TryGetValue(RequestId, out var sendRequestState) && sendRequestState is not null)
                    {
                        sendRequestState.ErrorCode = OCPP_WebSocket_ErrorCodes.Timeout;
                        requests.Remove(RequestId);
                        return sendRequestState;
                    }
                }

                #endregion

            }

            #region ..., or client/network error(s)

            else
            {
                lock (requests)
                {
                    if (requests.TryGetValue(RequestId, out var sendRequestState) && sendRequestState is not null)
                    {
                        sendRequestState.ErrorCode = OCPP_WebSocket_ErrorCodes.Timeout;
                        requests.Remove(RequestId);
                        return sendRequestState;
                    }
                }
            }

            #endregion


            // Just in case...
            var now = Timestamp.Now;

            return new SendRequestState(now,
                                        ChargeBoxId,
                                        new OCPP_WebSocket_RequestMessage(
                                            RequestId,
                                            OCPPAction,
                                            JSONPayload
                                        ),
                                        now,

                                        new JObject(),
                                        OCPP_WebSocket_ErrorCodes.InternalError);

        }

        #endregion

        #region SendJSON   (RequestId, ChargeBoxId, Action, JSON, RequestTimeout)

        /// <summary>
        /// Send (and forget) the given JSON.
        /// </summary>
        /// <param name="RequestId">A unique request identification.</param>
        /// <param name="ChargeBoxId">A charge box identification.</param>
        /// <param name="Action">An OCPP action.</param>
        /// <param name="JSON">The JSON payload.</param>
        /// <param name="RequestTimeout">A request timeout.</param>
        public async Task<SendJSONResults> SendJSON(Request_Id    RequestId,
                                                    ChargeBox_Id  ChargeBoxId,
                                                    String        Action,
                                                    JObject       JSON,
                                                    DateTime      RequestTimeout)
        {

            OCPP_WebSocket_RequestMessage?  wsRequestMessage   = null;
            SendRequestState?               result             = null;

            try
            {

                wsRequestMessage  = new OCPP_WebSocket_RequestMessage(RequestId,
                                                                      Action,
                                                                      JSON);

                result            = new SendRequestState(Timestamp.Now,
                                                         ChargeBoxId,
                                                         wsRequestMessage,
                                                         RequestTimeout);

                requests.Add(result.WSRequestMessage.RequestId,
                             result);


                var webSocketConnection  = WebSocketConnections.LastOrDefault(ws => ws.TryGetCustomDataAs<ChargeBox_Id>("chargeBoxId") == ChargeBoxId);
                if (webSocketConnection is null)
                {
                    result.ErrorCode = OCPP_WebSocket_ErrorCodes.UnknownClient;
                    return SendJSONResults.unknownClient;
                }

                var networkStream        = webSocketConnection.TcpClient.GetStream();
                var WSFrame              = new WebSocketFrame(
                                               WebSocketFrame.Fin.Final,
                                               WebSocketFrame.MaskStatus.Off,
                                               new Byte[4],
                                               WebSocketFrame.Opcodes.Text,
                                               wsRequestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.None).ToUTF8Bytes(),
                                               WebSocketFrame.Rsv.Off,
                                               WebSocketFrame.Rsv.Off,
                                               WebSocketFrame.Rsv.Off
                                           );

                await networkStream.WriteAsync(WSFrame.ToByteArray());

                File.AppendAllText(LogfileName,
                                   String.Concat("Timestamp: ",    Timestamp.Now.ToIso8601(),                                                    Environment.NewLine,
                                                 "ChargeBoxId: ",  ChargeBoxId.ToString(),                                                       Environment.NewLine,
                                                 "Message sent: ", wsRequestMessage.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented),      Environment.NewLine,
                                                 "--------------------------------------------------------------------------------------------", Environment.NewLine));

                return SendJSONResults.ok;

            }
            catch (Exception)
            {
                result.ErrorCode = OCPP_WebSocket_ErrorCodes.NetworkError;
                return SendJSONResults.failed;
            }

        }

        #endregion


        #region Reset                 (Request, RequestTimeout = null)

        public async Task<ResetResponse> Reset(ResetRequest  Request,
                                               TimeSpan?     RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomResetRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ResetResponse.TryParse(Request,
                                           result.Response,
                                           out var resetResponse,
                                           out var errorResponse))
                {
                    return resetResponse!;
                }

                return new ResetResponse(Request,
                                         Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ResetResponse(Request,
                                         Result.Unknown(result.ErrorDescription));

            }

            return new ResetResponse(Request,
                                     Result.Unknown());

        }

        #endregion

        #region ChangeAvailability    (Request, RequestTimeout = null)

        public async Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest  Request,
                                                                         TimeSpan?                  RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomChangeAvailabilityRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                                        result.Response,
                                                        out var changeAvailabilityResponse,
                                                        out var errorResponse))
                {
                    return changeAvailabilityResponse!;
                }

                return new ChangeAvailabilityResponse(Request,
                                                      Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ChangeAvailabilityResponse(Request,
                                                      Result.Unknown(result.ErrorDescription));

            }

            return new ChangeAvailabilityResponse(Request,
                                                  Result.Unknown());

        }

        #endregion

        #region GetConfiguration      (Request, RequestTimeout = null)

        public async Task<GetConfigurationResponse> GetConfiguration(GetConfigurationRequest  Request,
                                                                     TimeSpan?                RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomGetConfigurationRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (GetConfigurationResponse.TryParse(Request,
                                                      result.Response,
                                                      out var changeConfigurationResponse,
                                                      out var errorResponse))
                {
                    return changeConfigurationResponse!;
                }

                return new GetConfigurationResponse(Request,
                                                    Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new GetConfigurationResponse(Request,
                                                    Result.Unknown(result.ErrorDescription));

            }

            return new GetConfigurationResponse(Request,
                                                Result.Unknown());

        }

        #endregion

        #region ChangeConfiguration   (Request, RequestTimeout = null)

        public async Task<ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest  Request,
                                                                           TimeSpan?                   RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomChangeConfigurationRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ChangeConfigurationResponse.TryParse(Request,
                                                         result.Response,
                                                         out var changeConfigurationResponse,
                                                         out var errorResponse))
                {
                    return changeConfigurationResponse!;
                }

                return new ChangeConfigurationResponse(Request,
                                                       Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ChangeConfigurationResponse(Request,
                                                       Result.Unknown(result.ErrorDescription));

            }

            return new ChangeConfigurationResponse(Request,
                                                   Result.Unknown());

        }

        #endregion

        #region DataTransfer          (Request, RequestTimeout = null)

        public async Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest  Request,
                                                                TimeSpan?            RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomDataTransferRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (CP.DataTransferResponse.TryParse(Request,
                                                     result.Response,
                                                     out var dataTransferResponse,
                                                     out var errorResponse))
                {
                    return dataTransferResponse!;
                }

                return new CP.DataTransferResponse(Request,
                                                   Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new CP.DataTransferResponse(Request,
                                                   Result.Unknown(result.ErrorDescription));

            }

            return new CP.DataTransferResponse(Request,
                                               Result.Unknown());

        }

        #endregion

        #region GetDiagnostics        (Request, RequestTimeout = null)

        public async Task<GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest  Request,
                                                                 TimeSpan?              RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomGetDiagnosticsRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (GetDiagnosticsResponse.TryParse(Request,
                                                    result.Response,
                                                    out var getDiagnosticsResponse,
                                                    out var errorResponse))
                {
                    return getDiagnosticsResponse!;
                }

                return new GetDiagnosticsResponse(Request,
                                                  Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new GetDiagnosticsResponse(Request,
                                                  Result.Unknown(result.ErrorDescription));

            }

            return new GetDiagnosticsResponse(Request,
                                              Result.Unknown());

        }

        #endregion

        #region TriggerMessage        (Request, RequestTimeout = null)

        public async Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest  Request,
                                                                 TimeSpan?              RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomTriggerMessageRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (TriggerMessageResponse.TryParse(Request,
                                                    result.Response,
                                                    out var triggerMessageResponse,
                                                    out var errorResponse))
                {
                    return triggerMessageResponse!;
                }

                return new TriggerMessageResponse(Request,
                                                  Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new TriggerMessageResponse(Request,
                                                  Result.Unknown(result.ErrorDescription));

            }

            return new TriggerMessageResponse(Request,
                                              Result.Unknown());

        }

        #endregion

        #region UpdateFirmware        (Request, RequestTimeout = null)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest  Request,
                                                                 TimeSpan?              RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomUpdateFirmwareRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    result.Response,
                                                    out var updateFirmwareResponse,
                                                    out var errorResponse))
                {
                    return updateFirmwareResponse!;
                }

                return new UpdateFirmwareResponse(Request,
                                                  Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new UpdateFirmwareResponse(Request,
                                                  Result.Unknown(result.ErrorDescription));

            }

            return new UpdateFirmwareResponse(Request,
                                              Result.Unknown());

        }

        #endregion


        #region ReserveNow            (Request, RequestTimeout = null)

        public async Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request,
                                                         TimeSpan?          RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomReserveNowRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ReserveNowResponse.TryParse(Request,
                                                result.Response,
                                                out var reserveNowResponse,
                                                out var errorReponse))
                {
                    return reserveNowResponse!;
                }

                return new ReserveNowResponse(Request,
                                              Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ReserveNowResponse(Request,
                                              Result.Unknown(result.ErrorDescription));

            }

            return new ReserveNowResponse(Request,
                                          Result.Unknown());

        }

        #endregion

        #region CancelReservation     (Request, RequestTimeout = null)

        public async Task<CancelReservationResponse> CancelReservation(CancelReservationRequest  Request,
                                                                       TimeSpan?                 RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomCancelReservationRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (CancelReservationResponse.TryParse(Request,
                                                       result.Response,
                                                       out var cancelReservationResponse,
                                                       out var errorResponse))
                {
                    return cancelReservationResponse!;
                }

                return new CancelReservationResponse(Request,
                                                     Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new CancelReservationResponse(Request,
                                                     Result.Unknown(result.ErrorDescription));

            }

            return new CancelReservationResponse(Request,
                                                 Result.Unknown());

        }

        #endregion

        #region RemoteStartTransaction(Request, RequestTimeout = null)

        public async Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest  Request,
                                                                                 TimeSpan?                      RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomRemoteStartTransactionRequestSerializer,
                                                          CustomChargingProfileSerializer,
                                                          CustomChargingScheduleSerializer,
                                                          CustomChargingSchedulePeriodSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (RemoteStartTransactionResponse.TryParse(Request,
                                                            result.Response,
                                                            out var remoteStartTransactionResponse,
                                                            out var errorResponse))
                {
                    return remoteStartTransactionResponse!;
                }

                return new RemoteStartTransactionResponse(Request,
                                                          Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new RemoteStartTransactionResponse(Request,
                                                          Result.Unknown(result.ErrorDescription));

            }

            return new RemoteStartTransactionResponse(Request,
                                                      Result.Unknown());

        }

        #endregion

        #region RemoteStopTransaction (Request, RequestTimeout = null)

        public async Task<RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest  Request,
                                                                               TimeSpan?                     RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomRemoteStopTransactionRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (RemoteStopTransactionResponse.TryParse(Request,
                                                           result.Response,
                                                           out var remoteStopTransactionResponse,
                                                           out var errorResponse))
                {
                    return remoteStopTransactionResponse!;
                }

                return new RemoteStopTransactionResponse(Request,
                                                         Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new RemoteStopTransactionResponse(Request,
                                                         Result.Unknown(result.ErrorDescription));

            }

            return new RemoteStopTransactionResponse(Request,
                                                     Result.Unknown());

        }

        #endregion

        #region SetChargingProfile    (Request, RequestTimeout = null)

        public async Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest  Request,
                                                                         TimeSpan?                  RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomSetChargingProfileRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (SetChargingProfileResponse.TryParse(Request,
                                                        result.Response,
                                                        out var setChargingProfileResponse,
                                                        out var errorResponse))
                {
                    return setChargingProfileResponse!;
                }

                return new SetChargingProfileResponse(Request,
                                                      Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new SetChargingProfileResponse(Request,
                                                      Result.Unknown(result.ErrorDescription));

            }

            return new SetChargingProfileResponse(Request,
                                                  Result.Unknown());

        }

        #endregion

        #region ClearChargingProfile  (Request, RequestTimeout = null)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest  Request,
                                                                             TimeSpan?                    RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomClearChargingProfileRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          result.Response,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse))
                {
                    return clearChargingProfileResponse!;
                }

                return new ClearChargingProfileResponse(Request,
                                                        Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ClearChargingProfileResponse(Request,
                                                        Result.Unknown(result.ErrorDescription));

            }

            return new ClearChargingProfileResponse(Request,
                                                    Result.Unknown());

        }

        #endregion

        #region GetCompositeSchedule  (Request, RequestTimeout = null)


        public async Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest  Request,
                                                                             TimeSpan?                    RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomGetCompositeScheduleRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (GetCompositeScheduleResponse.TryParse(Request,
                                                          result.Response,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse))
                {
                    return clearChargingProfileResponse!;
                }

                return new GetCompositeScheduleResponse(Request,
                                                        Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new GetCompositeScheduleResponse(Request,
                                                        Result.Unknown(result.ErrorDescription));

            }

            return new GetCompositeScheduleResponse(Request,
                                                    Result.Unknown());

        }

        #endregion

        #region UnlockConnector       (Request, RequestTimeout = null)

        public async Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest  Request,
                                                                   TimeSpan?               RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomUnlockConnectorRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (UnlockConnectorResponse.TryParse(Request,
                                                     result.Response,
                                                     out var unlockConnectorResponse,
                                                     out var errorResponse))
                {
                    return unlockConnectorResponse!;
                }

                return new UnlockConnectorResponse(Request,
                                                   Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new UnlockConnectorResponse(Request,
                                                   Result.Unknown(result.ErrorDescription));

            }

            return new UnlockConnectorResponse(Request,
                                               Result.Unknown());

        }

        #endregion


        #region GetLocalListVersion   (Request, RequestTimeout = null)

        public async Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest  Request,
                                                                           TimeSpan?                   RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomGetLocalListVersionRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (GetLocalListVersionResponse.TryParse(Request,
                                                         result.Response,
                                                         out var unlockConnectorResponse,
                                                         out var errorResponse))
                {
                    return unlockConnectorResponse!;
                }

                return new GetLocalListVersionResponse(Request,
                                                       Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new GetLocalListVersionResponse(Request,
                                                       Result.Unknown(result.ErrorDescription));

            }

            return new GetLocalListVersionResponse(Request,
                                                   Result.Unknown());

        }

        #endregion

        #region SendLocalList         (Request, RequestTimeout = null)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest  Request,
                                                               TimeSpan?             RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomSendLocalListRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (SendLocalListResponse.TryParse(Request,
                                                   result.Response,
                                                   out var unlockConnectorResponse,
                                                   out var errorResponse))
                {
                    return unlockConnectorResponse!;
                }

                return new SendLocalListResponse(Request,
                                                 Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new SendLocalListResponse(Request,
                                                 Result.Unknown(result.ErrorDescription));

            }

            return new SendLocalListResponse(Request,
                                             Result.Unknown());

        }

        #endregion

        #region ClearCache            (Request, RequestTimeout = null)

        public async Task<ClearCacheResponse> ClearCache(ClearCacheRequest  Request,
                                                         TimeSpan?          RequestTimeout = null)
        {

            var result = await SendRequest(Request.RequestId,
                                           Request.ChargeBoxId,
                                           Request.WebSocketAction,
                                           Request.ToJSON(CustomClearCacheRequestSerializer),
                                           RequestTimeout ?? Request.RequestTimeout);

            if (result?.Response is not null)
            {

                if (ClearCacheResponse.TryParse(Request,
                                                result.Response,
                                                out var unlockConnectorResponse,
                                                out var errorResponse))
                {
                    return unlockConnectorResponse!;
                }

                return new ClearCacheResponse(Request,
                                              Result.Unknown());

            }

            if (result?.ErrorCode.HasValue == true)
            {

                return new ClearCacheResponse(Request,
                                              Result.Unknown(result.ErrorDescription));

            }

            return new ClearCacheResponse(Request,
                                          Result.Unknown());

        }

        #endregion


    }

}
