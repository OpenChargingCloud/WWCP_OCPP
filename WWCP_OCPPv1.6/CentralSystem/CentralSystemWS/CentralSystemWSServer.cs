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
using System.Threading.Tasks;

using cloud.charging.open.protocols.OCPPv1_6.CP;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using Newtonsoft.Json.Linq;
using System.Threading;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system HTTP/WebSocket/JSON server.
    /// </summary>
    public class CentralSystemWSServer : WebSocketServer
    {

        #region Data

        /// <summary>
        /// The default HTTP/SOAP/XML server name.
        /// </summary>
        public new const String DefaultHTTPServerName = "GraphDefined OCPP " + Version.Number + " HTTP/SOAP/XML Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort DefaultHTTPServerPort = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new static readonly HTTPPath DefaultURLPrefix = HTTPPath.Parse("/" + Version.Number);

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public new static readonly HTTPContentType DefaultContentType = HTTPContentType.XMLTEXT_UTF8;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public new static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>  CustomBootNotificationRequestParser    { get; set; }

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                 OnBootNotificationSOAPRequest;

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
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler                  OnBootNotificationSOAPResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler          OnHeartbeatSOAPRequest;

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
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler           OnHeartbeatSOAPResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler            OnAuthorizeSOAPRequest;

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
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler             OnAuthorizeSOAPResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                   OnStartTransactionSOAPRequest;

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
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler                    OnStartTransactionSOAPResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                     OnStatusNotificationSOAPRequest;

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
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler                      OnStatusNotificationSOAPResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler              OnMeterValuesSOAPRequest;

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
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler               OnMeterValuesSOAPResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler                  OnStopTransactionSOAPRequest;

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
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler                   OnStopTransactionSOAPResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler                       OnIncomingDataTransferSOAPRequest;

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
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler                        OnIncomingDataTransferSOAPResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                                OnDiagnosticsStatusNotificationSOAPRequest;

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
        /// An event sent whenever a SOAP response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler                                 OnDiagnosticsStatusNotificationSOAPResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                             OnFirmwareStatusNotificationSOAPRequest;

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
        /// An event sent whenever a SOAP response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler                              OnFirmwareStatusNotificationSOAPResponse;

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

            base.OnTextMessage += ProcessTextMessages;

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

            base.OnTextMessage += ProcessTextMessages;

        }

        #endregion

        #endregion


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

                JSON = JArray.Parse(TextMessage?.Trim());

                #region MessageType 2: CALL       (Client-to-Server)

                // [
                //     2,                         // MessageType: CALL (Client-to-Server)
                //    "19223201",                 // RequestId
                //    "BootNotification",         // Action
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

                                        //OnBootNotificationSOAPRequest?.Invoke(DateTime.UtcNow,
                                        //                                      SOAPServer.HTTPServer,
                                        //                                      Request);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPRequest));
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

                                                OnBootNotificationRequest?.Invoke(bootNotificationRequest.RequestTimestamp,
                                                                                  this,
                                                                                  EventTrackingId,

                                                                                  chargeBoxId.Value,

                                                                                  bootNotificationRequest.ChargePointVendor,
                                                                                  bootNotificationRequest.ChargePointModel,
                                                                                  bootNotificationRequest.ChargePointSerialNumber,
                                                                                  bootNotificationRequest.ChargeBoxSerialNumber,
                                                                                  bootNotificationRequest.FirmwareVersion,
                                                                                  bootNotificationRequest.Iccid,
                                                                                  bootNotificationRequest.IMSI,
                                                                                  bootNotificationRequest.MeterType,
                                                                                  bootNotificationRequest.MeterSerialNumber);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationRequest));
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

                                                OnBootNotificationResponse?.Invoke(response.ResponseTimestamp,
                                                                                   this,
                                                                                   EventTrackingId,

                                                                                   chargeBoxId.Value,

                                                                                   bootNotificationRequest.ChargePointVendor,
                                                                                   bootNotificationRequest.ChargePointModel,
                                                                                   bootNotificationRequest.ChargePointSerialNumber,
                                                                                   bootNotificationRequest.ChargeBoxSerialNumber,
                                                                                   bootNotificationRequest.FirmwareVersion,
                                                                                   bootNotificationRequest.Iccid,
                                                                                   bootNotificationRequest.IMSI,
                                                                                   bootNotificationRequest.MeterType,
                                                                                   bootNotificationRequest.MeterSerialNumber,

                                                                                   response.Result,
                                                                                   response.Status,
                                                                                   response.CurrentTime,
                                                                                   response.Interval,
                                                                                   response.Runtime);

                                            }
                                            catch (Exception e)
                                            {
                                                e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationResponse));
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

                                        //OnBootNotificationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                        //                                        SOAPServer.HTTPServer,
                                        //                                        Request,
                                        //                                        HTTPResponse);

                                    }
                                    catch (Exception e)
                                    {
                                        e.Log(nameof(CentralSystemSOAPServer) + "." + nameof(OnBootNotificationSOAPResponse));
                                    }

                                    #endregion

                                }
                                break;

                            #endregion

                        }


                        if (OCPPResponseJSON != null)
                            return new WebSocketTextMessageRespose(RequestTimestamp,
                                                                   TextMessage,
                                                                   DateTime.UtcNow,
                                                                   new WSResponseMessage(WSMessageTypes.CALLRESULT,
                                                                                         RequestId.Value,
                                                                                         OCPPResponseJSON).ToJSON().ToString());

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
                                                   ErrorMessage.ToJSON().ToString());

        }

        #endregion


    }

}
