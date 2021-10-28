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

        #region (class) SendRequestResult

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

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id { get; }


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

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean                                                            RequireAuthentication                               { get; }

        /// <summary>
        /// Logins and passwords for HTTP Basic Authentication.
        /// </summary>
        public Dictionary<String, String>                                         ChargingBoxLogins                                   { get; }

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler               OnBootNotificationWSRequest;

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
        /// An event sent whenever a WebSocket response to a boot notification was sent.
        /// </summary>
        public event WSResponseLogHandler              OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler        OnHeartbeatWSRequest;

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
        /// An event sent whenever a WebSocket response to a heartbeat was sent.
        /// </summary>
        public event WSResponseLogHandler       OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler          OnAuthorizeWSRequest;

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
        /// An event sent whenever an authorize WebSocket response was sent.
        /// </summary>
        public event WSResponseLogHandler         OnAuthorizeWSResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                 OnStartTransactionWSRequest;

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
        /// An event sent whenever a WebSocket response to a start transaction request was sent.
        /// </summary>
        public event WSResponseLogHandler                OnStartTransactionWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                   OnStatusNotificationWSRequest;

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
        /// An event sent whenever a WebSocket response to a status notification request was sent.
        /// </summary>
        public event WSResponseLogHandler                  OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler            OnMeterValuesWSRequest;

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
        /// An event sent whenever a WebSocket response to a meter values request was sent.
        /// </summary>
        public event WSResponseLogHandler           OnMeterValuesWSResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                OnStopTransactionWSRequest;

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
        /// An event sent whenever a WebSocket response to a stop transaction request was sent.
        /// </summary>
        public event WSResponseLogHandler               OnStopTransactionWSResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                     OnIncomingDataTransferWSRequest;

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
        /// An event sent whenever a WebSocket response to a data transfer request was sent.
        /// </summary>
        public event WSResponseLogHandler                    OnIncomingDataTransferWSResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                              OnDiagnosticsStatusNotificationWSRequest;

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
        /// An event sent whenever a WebSocket response to a diagnostics status notification request was sent.
        /// </summary>
        public event WSResponseLogHandler                             OnDiagnosticsStatusNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification WebSocket request was received.
        /// </summary>
        public event WSRequestLogHandler                           OnFirmwareStatusNotificationWSRequest;

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
        /// An event sent whenever a WebSocket response to a firmware status notification request was sent.
        /// </summary>
        public event WSResponseLogHandler                          OnFirmwareStatusNotificationWSResponse;

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ResetRequest>                   CustomResetRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>      CustomChangeAvailabilityRequestSerializer        { get; set; }

        public CustomJObjectSerializerDelegate<GetConfigurationRequest>        CustomGetConfigurationRequestSerializer          { get; set; }

        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>     CustomChangeConfigurationRequestSerializer       { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>            CustomDataTransferRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<GetDiagnosticsRequest>          CustomGetDiagnosticsRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>          CustomTriggerMessageRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>          CustomUpdateFirmwareRequestSerializer            { get; set; }



        public CustomJObjectSerializerDelegate<ReserveNowRequest>              CustomReserveNowRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>       CustomCancelReservationRequestSerializer         { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>  CustomRemoteStartTransactionRequestSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>                CustomChargingProfileSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>               CustomChargingScheduleSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>         CustomChargingSchedulePeriodSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>   CustomRemoteStopTransactionRequestSerializer     { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>      CustomSetChargingProfileRequestSerializer        { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>    CustomClearChargingProfileRequestSerializer      { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>    CustomGetCompositeScheduleRequestSerializer      { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>         CustomUnlockConnectorRequestSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>     CustomGetLocalListVersionRequestSerializer       { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>           CustomSendLocalListRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>              CustomAuthorizationDataSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>                      CustomIdTagInfoResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>              CustomClearCacheRequestSerializer                { get; set; }

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
        public CentralSystemWSServer(String      HTTPServerName          = DefaultHTTPServerName,
                                     IIPAddress  IPAddress               = null,
                                     IPPort?     TCPPort                 = null,
                                     Boolean     RequireAuthentication   = true,
                                     DNSClient   DNSClient               = null,
                                     Boolean     AutoStart               = false)

            : base(IPAddress,
                   TCPPort ?? IPPort.Parse(8000),
                   DNSClient,
                   false)

        {

            this.requests                  = new List<SendRequestResult>();
            this.RequireAuthentication     = RequireAuthentication;
            this.ChargingBoxLogins         = new Dictionary<String, String>();

            base.OnWebSocketHandshake     += ProcessWebSocketHandshake;
            base.OnNewWebSocketConnection += ProcessNewWebSocketConnection;
            base.OnTextMessage            += ProcessTextMessages;

            if (AutoStart)
                Start();

        }

        #endregion


        #region (protected) ProcessWebSocketHandshake    (LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        private async Task<HTTPResponse> ProcessWebSocketHandshake(DateTime             LogTimestamp,
                                                                   WebSocketServer      Server,
                                                                   WebSocketConnection  Connection,
                                                                   EventTracking_Id     EventTrackingId,
                                                                   CancellationToken    CancellationToken)
        {

            //ToDo: Validate whether "Sec-WebSocket-Protocol" == "ocpp1.6" or "ocpp1.6, ocpp2.0"

            if (RequireAuthentication)
            {

                if (HTTPBasicAuthentication.TryParse(Connection?.GetHTTPHeader("Authorization"), out HTTPBasicAuthentication basicAuthentication))
                {

                    if (ChargingBoxLogins.TryGetValue(basicAuthentication.Username, out String Password) &&
                        basicAuthentication.Password == Password)
                    {
                        return null;
                    }
                    else
                        DebugX.Log(nameof(CentralSystemWSServer), " connection from " + Connection.RemoteSocket + " invalid authorization: " + basicAuthentication.Username + "/" + basicAuthentication.Password);

                }
                else
                    DebugX.Log(nameof(CentralSystemWSServer), " connection from " + Connection.RemoteSocket + " missing authorization!");

                return new HTTPResponse.Builder(HTTPStatusCode.Unauthorized) {
                               Date        = Timestamp.Now,
                               Connection  = "close"
                           }.AsImmutable;

            }

            return null;

        }

        #endregion

        #region (protected) ProcessNewWebSocketConnection(LogTimestamp, Server, Connection, EventTrackingId, CancellationToken)

        protected async Task ProcessNewWebSocketConnection(DateTime             LogTimestamp,
                                                           WebSocketServer      Server,
                                                           WebSocketConnection  Connection,
                                                           EventTracking_Id     EventTrackingId,
                                                           CancellationToken    CancellationToken)
        {

            if (!Connection.HasCustomData("chargeBoxId"))
            {

                var chargeBoxId = Connection.HTTPPath.ToString().Substring(Connection.HTTPPath.ToString().LastIndexOf("/") + 1);

                if (chargeBoxId.IsNotNullOrEmpty())
                    Connection.AddCustomData("chargeBoxId", ChargeBox_Id.Parse(chargeBoxId));

            }

        }

        #endregion

        #region (protected) ProcessTextMessages          (RequestTimestamp, Server, Connection, TextMessage, EventTrackingId, CancellationToken)

        /// <summary>
        /// Process all text messages of this web socket API.
        /// </summary>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="Server">The web socket server.</param>
        /// <param name="Connection">The web socket connection.</param>
        /// <param name="TextMessage">The received text message.</param>
        /// <param name="EventTrackingId">The event tracking identification.</param>
        /// <param name="CancellationToken">The cancellation token.</param>
        protected async Task<WebSocketTextMessageRespose> ProcessTextMessages(DateTime             RequestTimestamp,
                                                                              WebSocketServer      Server,
                                                                              WebSocketConnection  Connection,
                                                                              String               TextMessage,
                                                                              EventTracking_Id     EventTrackingId,
                                                                              CancellationToken    CancellationToken)
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

                                        OnBootNotificationWSRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            JSON);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBootNotificationWSRequest));
                                    }

                                    #endregion

                                    BootNotificationResponse response = null;

                                    try
                                    {

                                        if (BootNotificationRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out BootNotificationRequest  request,
                                                                             out                          ErrorResponse,
                                                                             CustomBootNotificationRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnBootNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as BootNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = BootNotificationResponse.Failed(request);

                                            }

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

                                        OnBootNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             JSON,
                                                                             new WSResponseMessage(RequestId.Value,
                                                                                                   OCPPResponseJSON).ToJSON());

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

                                    HeartbeatResponse response = null;

                                    try
                                    {

                                        if (HeartbeatRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out HeartbeatRequest  request,
                                                                      out                   ErrorResponse,
                                                                      CustomHeartbeatRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnHeartbeat?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as HeartbeatDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = HeartbeatResponse.Failed(request);

                                            }

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

                                        OnHeartbeatWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      JSON,
                                                                      new WSResponseMessage(RequestId.Value,
                                                                                            OCPPResponseJSON).ToJSON());

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

                                    AuthorizeResponse response = null;

                                    try
                                    {

                                        if (AuthorizeRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out AuthorizeRequest  request,
                                                                      out                   ErrorResponse,
                                                                      CustomAuthorizeRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnAuthorize?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = AuthorizeResponse.Failed(request);

                                            }

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

                                        OnAuthorizeWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      JSON,
                                                                      new WSResponseMessage(RequestId.Value,
                                                                                            OCPPResponseJSON).ToJSON());

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

                                    StartTransactionResponse response = null;

                                    try
                                    {

                                        if (StartTransactionRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out StartTransactionRequest  request,
                                                                             out                          ErrorResponse,
                                                                             CustomStartTransactionRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnStartTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStartTransactionDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StartTransactionResponse.Failed(request);

                                            }

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

                                        OnStartTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             JSON,
                                                                             new WSResponseMessage(RequestId.Value,
                                                                                                   OCPPResponseJSON).ToJSON());

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

                                    StatusNotificationResponse response = null;

                                    try
                                    {

                                        if (StatusNotificationRequest.TryParse(RequestData,
                                                                               RequestId.  Value,
                                                                               chargeBoxId.Value,
                                                                               out StatusNotificationRequest  request,
                                                                               out                            ErrorResponse,
                                                                               CustomStatusNotificationRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StatusNotificationResponse.Failed(request);

                                            }

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

                                        OnStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               JSON,
                                                                               new WSResponseMessage(RequestId.Value,
                                                                                                     OCPPResponseJSON).ToJSON());

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

                                    MeterValuesResponse response = null;

                                    try
                                    {

                                        if (MeterValuesRequest.TryParse(RequestData,
                                                                      RequestId.  Value,
                                                                      chargeBoxId.Value,
                                                                      out MeterValuesRequest  request,
                                                                      out                     ErrorResponse,
                                                                      CustomMeterValuesRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnMeterValues?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = MeterValuesResponse.Failed(request);

                                            }

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

                                        OnMeterValuesWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        JSON,
                                                                        new WSResponseMessage(RequestId.Value,
                                                                                              OCPPResponseJSON).ToJSON());

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

                                    StopTransactionResponse response = null;

                                    try
                                    {

                                        if (StopTransactionRequest.TryParse(RequestData,
                                                                             RequestId.  Value,
                                                                             chargeBoxId.Value,
                                                                             out StopTransactionRequest  request,
                                                                             out                         ErrorResponse,
                                                                             CustomStopTransactionRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnStopTransaction?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = StopTransactionResponse.Failed(request);

                                            }

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

                                        OnStopTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            JSON,
                                                                            new WSResponseMessage(RequestId.Value,
                                                                                                  OCPPResponseJSON).ToJSON());

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

                                    DataTransferResponse response = null;

                                    try
                                    {

                                        if (CP.DataTransferRequest.TryParse(RequestData,
                                                                            RequestId.  Value,
                                                                            chargeBoxId.Value,
                                                                            out CP.DataTransferRequest  request,
                                                                            out                         ErrorResponse,
                                                                            CustomDataTransferRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnIncomingDataTransfer?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = DataTransferResponse.Failed(request);

                                            }

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

                                        OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 JSON,
                                                                                 new WSResponseMessage(RequestId.Value,
                                                                                                       OCPPResponseJSON).ToJSON());

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

                                    DiagnosticsStatusNotificationResponse response = null;

                                    try
                                    {

                                        if (DiagnosticsStatusNotificationRequest.TryParse(RequestData,
                                                                                          RequestId.  Value,
                                                                                          chargeBoxId.Value,
                                                                                          out DiagnosticsStatusNotificationRequest  request,
                                                                                          out                                       ErrorResponse,
                                                                                          CustomDiagnosticsStatusNotificationRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnDiagnosticsStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = DiagnosticsStatusNotificationResponse.Failed(request);

                                            }

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

                                        OnDiagnosticsStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                          this,
                                                                                          JSON,
                                                                                          new WSResponseMessage(RequestId.Value,
                                                                                                                OCPPResponseJSON).ToJSON());

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

                                    FirmwareStatusNotificationResponse response = null;

                                    try
                                    {

                                        if (FirmwareStatusNotificationRequest.TryParse(RequestData,
                                                                                       RequestId.  Value,
                                                                                       chargeBoxId.Value,
                                                                                       out FirmwareStatusNotificationRequest  request,
                                                                                       out                                    ErrorResponse,
                                                                                       CustomFirmwareStatusNotificationRequestParser))
                                        {

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

                                            if (response == null)
                                            {

                                                var results = OnFirmwareStatusNotification?.
                                                                    GetInvocationList()?.
                                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)
                                                                        (Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         CancellationToken)).
                                                                    ToArray();

                                                if (results?.Length > 0)
                                                {

                                                    await Task.WhenAll(results);

                                                    response = results.FirstOrDefault()?.Result;

                                                }

                                                if (results == null || response == null)
                                                    response = FirmwareStatusNotificationResponse.Failed(request);

                                            }

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

                                        OnFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                                                       this,
                                                                                       JSON,
                                                                                       new WSResponseMessage(RequestId.Value,
                                                                                                             OCPPResponseJSON).ToJSON());

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
                                    }

                                    #endregion

                                }
                                break;

                                #endregion

                        }

                        return new WebSocketTextMessageRespose(RequestTimestamp,
                                                               TextMessage,
                                                               Timestamp.Now,
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
                                                   Timestamp.Now,
                                                   ((ErrorMessage?.ToJSON()) ?? new JArray()).ToString());

        }

        #endregion


        #region SendRequest(RequestId, ClientId, Action, Request, Timeout = null)

        public async Task<SendRequestResult> SendRequest(Request_Id    RequestId,
                                                         ChargeBox_Id  ClientId,
                                                         String        Action,
                                                         JObject       Request,
                                                         TimeSpan?     Timeout   = null)
        {

            var endTime = Timestamp.Now + (Timeout ?? TimeSpan.FromMinutes(3));

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
                while (Timestamp.Now < endTime);

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

        #endregion

        #region SendJSON   (RequestId, ClientId, Action, Data,    Timeout)

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

                result   = new SendRequestResult(Timestamp.Now,
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

            if (result?.Response != null)
            {

                if (ResetResponse.TryParse(Request,
                                           result.Response,
                                           out ResetResponse resetResponse))
                {
                    return resetResponse;
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

            if (result?.Response != null)
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                                       result.Response,
                                                       out ChangeAvailabilityResponse changeAvailabilityResponse))
                {
                    return changeAvailabilityResponse;
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

            if (result?.Response != null)
            {

                if (GetConfigurationResponse.TryParse(Request,
                                                      result.Response,
                                                      out GetConfigurationResponse changeConfigurationResponse))
                {
                    return changeConfigurationResponse;
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

            if (result?.Response != null)
            {

                if (ChangeConfigurationResponse.TryParse(Request,
                                                         result.Response,
                                                         out ChangeConfigurationResponse changeConfigurationResponse))
                {
                    return changeConfigurationResponse;
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

            if (result?.Response != null)
            {

                if (CP.DataTransferResponse.TryParse(Request,
                                                     result.Response,
                                                     out CP.DataTransferResponse dataTransferResponse))
                {
                    return dataTransferResponse;
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

            if (result?.Response != null)
            {

                if (GetDiagnosticsResponse.TryParse(Request,
                                                    result.Response,
                                                    out GetDiagnosticsResponse getDiagnosticsResponse))
                {
                    return getDiagnosticsResponse;
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

            if (result?.Response != null)
            {

                if (TriggerMessageResponse.TryParse(Request,
                                                    result.Response,
                                                    out TriggerMessageResponse triggerMessageResponse))
                {
                    return triggerMessageResponse;
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

            if (result?.Response != null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    result.Response,
                                                    out UpdateFirmwareResponse updateFirmwareResponse))
                {
                    return updateFirmwareResponse;
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

            if (result?.Response != null)
            {

                if (ReserveNowResponse.TryParse(Request,
                                                result.Response,
                                                out ReserveNowResponse reserveNowResponse))
                {
                    return reserveNowResponse;
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

            if (result?.Response != null)
            {

                if (CancelReservationResponse.TryParse(Request,
                                                       result.Response,
                                                       out CancelReservationResponse cancelReservationResponse))
                {
                    return cancelReservationResponse;
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

            if (result?.Response != null)
            {

                if (RemoteStartTransactionResponse.TryParse(Request,
                                                            result.Response,
                                                            out RemoteStartTransactionResponse remoteStartTransactionResponse))
                {
                    return remoteStartTransactionResponse;
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

            if (result?.Response != null)
            {

                if (RemoteStopTransactionResponse.TryParse(Request,
                                                           result.Response,
                                                           out RemoteStopTransactionResponse remoteStopTransactionResponse))
                {
                    return remoteStopTransactionResponse;
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

            if (result?.Response != null)
            {

                if (SetChargingProfileResponse.TryParse(Request,
                                                        result.Response,
                                                        out SetChargingProfileResponse setChargingProfileResponse))
                {
                    return setChargingProfileResponse;
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

            if (result?.Response != null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          result.Response,
                                                          out ClearChargingProfileResponse clearChargingProfileResponse))
                {
                    return clearChargingProfileResponse;
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

            if (result?.Response != null)
            {

                if (GetCompositeScheduleResponse.TryParse(Request,
                                                          result.Response,
                                                          out GetCompositeScheduleResponse clearChargingProfileResponse))
                {
                    return clearChargingProfileResponse;
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

            if (result?.Response != null)
            {

                if (UnlockConnectorResponse.TryParse(Request,
                                                     result.Response,
                                                     out UnlockConnectorResponse unlockConnectorResponse))
                {
                    return unlockConnectorResponse;
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

            if (result?.Response != null)
            {

                if (GetLocalListVersionResponse.TryParse(Request,
                                                         result.Response,
                                                         out GetLocalListVersionResponse unlockConnectorResponse))
                {
                    return unlockConnectorResponse;
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

            if (result?.Response != null)
            {

                if (SendLocalListResponse.TryParse(Request,
                                                   result.Response,
                                                   out SendLocalListResponse unlockConnectorResponse))
                {
                    return unlockConnectorResponse;
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

            if (result?.Response != null)
            {

                if (ClearCacheResponse.TryParse(Request,
                                                result.Response,
                                                out ClearCacheResponse unlockConnectorResponse))
                {
                    return unlockConnectorResponse;
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
