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

using cloud.charging.open.protocols.OCPPv1_6.CS;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charge point HTTP/SOAP/XML server.
    /// </summary>
    public class ChargePointSOAPServer : ASOAPServer,
                                         IEventSender,
                                         IChargePointServerEvents
    {

        #region Data

        /// <summary>
        /// The default HTTP/SOAP/XML server name.
        /// </summary>
        public new const String DefaultHTTPServerName = "GraphDefined OCPP " + Version.Number + " HTTP/SOAP/XML Charge Point API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort DefaultHTTPServerPort = IPPort.Parse(2010);

        /// <summary>
        /// The default TCP service name shown e.g. on service startup.
        /// </summary>
        public const String DefaultServiceName = "OCPP " + Version.Number + " charge point API";

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new static readonly HTTPPath DefaultURLPrefix = HTTPPath.Parse("/" + Version.Number);

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public new static readonly HTTPContentType DefaultContentType = HTTPContentType.XMLTEXT_UTF8;

        /// <summary>
        /// The default query timeout.
        /// </summary>
        public new static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => SOAPServer.HTTPServer.DefaultServerName;

        #endregion

        #region Events

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnResetSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate    OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate           OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate   OnResetResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnResetSOAPResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnChangeAvailabilitySOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate    OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate           OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate   OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnChangeAvailabilitySOAPResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnGetConfigurationSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationRequestDelegate    OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetConfigurationDelegate           OnGetConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetConfigurationResponseDelegate   OnGetConfigurationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnGetConfigurationSOAPResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnChangeConfigurationSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate    OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeConfigurationDelegate           OnChangeConfiguration;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate   OnChangeConfigurationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnChangeConfigurationSOAPResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler                        OnIncomingDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler                         OnIncomingDataTransferSOAPResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnGetDiagnosticsSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate    OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetDiagnosticsDelegate           OnGetDiagnostics;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate   OnGetDiagnosticsResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnGetDiagnosticsSOAPResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnTriggerMessageSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate    OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageDelegate           OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate   OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnTriggerMessageSOAPResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnUpdateFirmwareSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate    OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate           OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate   OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnUpdateFirmwareSOAPResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now SOAP request was received.
        /// </summary>
        public event RequestLogHandler              OnReserveNowSOAPRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate    OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate           OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate   OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reserve now request was sent.
        /// </summary>
        public event AccessLogHandler               OnReserveNowSOAPResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation SOAP request was received.
        /// </summary>
        public event RequestLogHandler OnCancelReservationSOAPRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a cancel reservation request was sent.
        /// </summary>
        public event AccessLogHandler OnCancelReservationSOAPResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a remote start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler OnRemoteStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote start transaction was received.
        /// </summary>
        public event OnRemoteStartTransactionDelegate OnRemoteStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate OnRemoteStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a remote start transaction request was sent.
        /// </summary>
        public event AccessLogHandler OnRemoteStartTransactionSOAPResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a remote stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler OnRemoteStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a remote stop transaction was received.
        /// </summary>
        public event OnRemoteStopTransactionDelegate OnRemoteStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate OnRemoteStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a remote stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler OnRemoteStopTransactionSOAPResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnSetChargingProfileSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate    OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate           OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate   OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnSetChargingProfileSOAPResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnClearChargingProfileSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate    OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate           OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate   OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnClearChargingProfileSOAPResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnGetCompositeScheduleSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate    OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate           OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate   OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnGetCompositeScheduleSOAPResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnUnlockConnectorSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate    OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate           OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate   OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnUnlockConnectorSOAPResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnGetLocalListVersionSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate    OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate           OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate   OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnGetLocalListVersionSOAPResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnSendLocalListSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate    OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate           OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate   OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnSendLocalListSOAPResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset SOAP request was received.
        /// </summary>
        public event RequestLogHandler         OnClearCacheSOAPRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate    OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate           OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate   OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a SOAP response to a reset request was sent.
        /// </summary>
        public event AccessLogHandler          OnClearCacheSOAPResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region ChargePointSOAPServer(HTTPServerName, TCPPort = default, URLPrefix = default, ContentType = default, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the charge point HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public ChargePointSOAPServer(String HTTPServerName = DefaultHTTPServerName,
                                     IPPort? TCPPort = null,
                                     String ServiceName = null,
                                     HTTPPath? URLPrefix = null,
                                     HTTPContentType ContentType = null,
                                     Boolean RegisterHTTPRootService = true,
                                     DNSClient DNSClient = null,
                                     Boolean AutoStart = false)

            : base(HTTPServerName.IsNotNullOrEmpty()
                       ? HTTPServerName
                       : DefaultHTTPServerName,
                   TCPPort ?? DefaultHTTPServerPort,
                   ServiceName ?? DefaultServiceName,
                   URLPrefix ?? DefaultURLPrefix,
                   ContentType ?? DefaultContentType,
                   RegisterHTTPRootService,
                   DNSClient,
                   AutoStart: false)

        {

            RegisterURLTemplates();

            if (AutoStart)
                Start();

        }

        #endregion

        #region ChargePointSOAPServer(SOAPServer, URLPrefix = DefaultURLPrefix)

        /// <summary>
        /// Use the given HTTP server for the charge point HTTP/SOAP/XML API.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        public ChargePointSOAPServer(SOAPServer SOAPServer,
                                     HTTPPath? URLPrefix = null)

            : base(SOAPServer,
                   URLPrefix ?? DefaultURLPrefix)

        {

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        private String NextMessageId()
            => Guid.NewGuid().ToString();

        #region RegisterURLTemplates()

        /// <summary>
        /// Register all URL templates for this SOAP API.
        /// </summary>
        protected void RegisterURLTemplates()
        {

            #region / - Reset

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "Reset",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "resetRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, ResetXML) =>
                                            {

                                                #region Send OnResetSOAPRequest event

                                                var requestTimestamp = Timestamp.Now;

                                                try
                                                {

                                                    OnResetSOAPRequest?.Invoke(requestTimestamp,
                                                                               SOAPServer.HTTPServer,
                                                                               Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnResetSOAPRequest));
                                                }

                                                #endregion

                                                ResetResponse response = null;
                                                HTTPResponse HTTPResponse = null;

                                                try
                                                {

                                                    var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                    var request = ResetRequest.Parse(ResetXML,
                                                                                         Request_Id.Parse(OCPPHeader.MessageId),
                                                                                         OCPPHeader.ChargeBoxIdentity);

                                                    #region Send OnResetRequest event

                                                    try
                                                    {

                                                        OnResetRequest?.Invoke(request.RequestTimestamp,
                                                                               this,
                                                                               request);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnResetRequest));
                                                    }

                                                    #endregion

                                                    #region Call async subscribers

                                                    if (response == null)
                                                    {

                                                        var results = OnReset?.
                                                                          GetInvocationList()?.
                                                                          SafeSelect(subscriber => (subscriber as OnResetDelegate)
                                                                              (Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               Request.CancellationToken)).
                                                                          ToArray();

                                                        if (results.Length > 0)
                                                        {

                                                            await Task.WhenAll(results);

                                                            response = results.FirstOrDefault()?.Result;

                                                        }

                                                        if (results.Length == 0 || response == null)
                                                            response = ResetResponse.Failed(request);

                                                    }

                                                    #endregion

                                                    #region Send OnResetResponse event

                                                    try
                                                    {

                                                        var responseTimestamp = Timestamp.Now;

                                                        OnResetResponse?.Invoke(responseTimestamp,
                                                                                this,
                                                                                request,
                                                                                response,
                                                                                responseTimestamp - requestTimestamp);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnResetResponse));
                                                    }

                                                    #endregion


                                                    #region Create SOAPResponse

                                                    HTTPResponse = new HTTPResponse.Builder(Request)
                                                    {
                                                        HTTPStatusCode = HTTPStatusCode.OK,
                                                        Server = SOAPServer.HTTPServer.DefaultServerName,
                                                        Date = Timestamp.Now,
                                                        ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                        Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                             "/ResetResponse",
                                                                                             null,
                                                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                             OCPPHeader.To,         // Fake it!
                                                                                             OCPPHeader.From,       // Fake it!
                                                                                             response.ToXML()).ToUTF8Bytes()
                                                    };

                                                    #endregion

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/Reset");
                                                }

                                                #region Send OnResetSOAPResponse event

                                                try
                                                {

                                                    OnResetSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                this.SOAPServer.HTTPServer,
                                                                                Request,
                                                                                HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnResetSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion


            #region / - ReserveNow

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "ReserveNow",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "reserveNowRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, ReserveNowXML) =>
                                            {

                                                #region Send OnReserveNowSOAPRequest event

                                                var requestTimestamp = Timestamp.Now;

                                                try
                                                {

                                                    OnReserveNowSOAPRequest?.Invoke(requestTimestamp,
                                                                                    SOAPServer.HTTPServer,
                                                                                    Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnReserveNowSOAPRequest));
                                                }

                                                #endregion

                                                ReserveNowResponse response = null;
                                                HTTPResponse HTTPResponse = null;

                                                try
                                                {

                                                    var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                    var request = ReserveNowRequest.Parse(ReserveNowXML,
                                                                                                Request_Id.Parse(OCPPHeader.MessageId),
                                                                                                OCPPHeader.ChargeBoxIdentity);

                                                    #region Send OnReserveNowRequest event

                                                    try
                                                    {

                                                        OnReserveNowRequest?.Invoke(request.RequestTimestamp,
                                                                                    this,
                                                                                    request);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnReserveNowRequest));
                                                    }

                                                    #endregion

                                                    #region Call async subscribers

                                                    if (response == null)
                                                    {

                                                        var results = OnReserveNow?.
                                                                          GetInvocationList()?.
                                                                          SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)
                                                                              (Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               Request.CancellationToken)).
                                                                          ToArray();

                                                        if (results.Length > 0)
                                                        {

                                                            await Task.WhenAll(results);

                                                            response = results.FirstOrDefault()?.Result;

                                                        }

                                                        if (results.Length == 0 || response == null)
                                                            response = ReserveNowResponse.Failed(request);

                                                    }

                                                    #endregion

                                                    #region Send OnReserveNowResponse event

                                                    try
                                                    {

                                                        var responseTimestamp = Timestamp.Now;

                                                        OnReserveNowResponse?.Invoke(responseTimestamp,
                                                                                     this,
                                                                                     request,
                                                                                     response,
                                                                                     responseTimestamp - requestTimestamp);

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnReserveNowResponse));
                                                    }

                                                    #endregion


                                                    #region Create SOAPResponse

                                                    HTTPResponse = new HTTPResponse.Builder(Request)
                                                    {
                                                        HTTPStatusCode = HTTPStatusCode.OK,
                                                        Server = SOAPServer.HTTPServer.DefaultServerName,
                                                        Date = Timestamp.Now,
                                                        ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                        Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                             "/ReserveNowResponse",
                                                                                             null,
                                                                                             OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                             OCPPHeader.To,         // Fake it!
                                                                                             OCPPHeader.From,       // Fake it!
                                                                                             response.ToXML()).ToUTF8Bytes()
                                                    };

                                                    #endregion

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.LogException(e, nameof(CentralSystemSOAPServer) + "." + "/ReserveNow");
                                                }

                                                #region Send OnReserveNowSOAPResponse event

                                                try
                                                {

                                                    OnReserveNowSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                     this.SOAPServer.HTTPServer,
                                                                                     Request,
                                                                                     HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnReserveNowSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion

            #region / - CancelReservation

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "CancelReservation",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "cancelReservationRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, CancelReservationXML) =>
                                            {

                                                #region Send OnCancelReservationSOAPRequest event

                                                try
                                                {

                                                    OnCancelReservationSOAPRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                                           this.SOAPServer.HTTPServer,
                                                                                           Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnCancelReservationSOAPRequest));
                                                }

                                                #endregion


                                                var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                var request = CancelReservationRequest.Parse(CancelReservationXML,
                                                                                                 Request_Id.Parse(OCPPHeader.MessageId),
                                                                                                 OCPPHeader.ChargeBoxIdentity);

                                                CancelReservationResponse response = null;



                                                #region Call async subscribers

                                                if (response == null)
                                                {

                                                    var results = OnCancelReservation?.
                                                                      GetInvocationList()?.
                                                                      SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)
                                                                          (Timestamp.Now,
                                                                           this,
                                                                           request,
                                                                           Request.CancellationToken)).
                                                                      ToArray();

                                                    if (results.Length > 0)
                                                    {

                                                        await Task.WhenAll(results);

                                                        response = results.FirstOrDefault()?.Result;

                                                    }

                                                    if (results.Length == 0 || response == null)
                                                        response = CancelReservationResponse.Failed(request);

                                                }

                                                #endregion



                                                #region Create SOAPResponse

                                                var HTTPResponse = new HTTPResponse.Builder(Request)
                                                {
                                                    HTTPStatusCode = HTTPStatusCode.OK,
                                                    Server = SOAPServer.HTTPServer.DefaultServerName,
                                                    Date = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                    Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                         "/CancelReservationResponse",
                                                                                         null,
                                                                                         OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                         OCPPHeader.To,         // Fake it!
                                                                                         OCPPHeader.From,       // Fake it!
                                                                                         response.ToXML()).ToUTF8Bytes()
                                                };

                                                #endregion


                                                #region Send OnCancelReservationSOAPResponse event

                                                try
                                                {

                                                    OnCancelReservationSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                            this.SOAPServer.HTTPServer,
                                                                                            Request,
                                                                                            HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnCancelReservationSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion

            #region / - RemoteStartTransaction

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "RemoteStartTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "remoteStartTransactionRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, RemoteStartTransactionXML) =>
                                            {

                                                #region Send OnRemoteStartTransactionSOAPRequest event

                                                try
                                                {

                                                    OnRemoteStartTransactionSOAPRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                                                this.SOAPServer.HTTPServer,
                                                                                                Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnRemoteStartTransactionSOAPRequest));
                                                }

                                                #endregion


                                                var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                var request = RemoteStartTransactionRequest.Parse(RemoteStartTransactionXML,
                                                                                                      Request_Id.Parse(OCPPHeader.MessageId),
                                                                                                      OCPPHeader.ChargeBoxIdentity);

                                                RemoteStartTransactionResponse response = null;



                                                #region Call async subscribers

                                                if (response == null)
                                                {

                                                    var results = OnRemoteStartTransaction?.
                                                                      GetInvocationList()?.
                                                                      SafeSelect(subscriber => (subscriber as OnRemoteStartTransactionDelegate)
                                                                          (Timestamp.Now,
                                                                           this,
                                                                           request,
                                                                           Request.CancellationToken)).
                                                                      ToArray();

                                                    if (results.Length > 0)
                                                    {

                                                        await Task.WhenAll(results);

                                                        response = results.FirstOrDefault()?.Result;

                                                    }

                                                    if (results.Length == 0 || response == null)
                                                        response = RemoteStartTransactionResponse.Failed(request);

                                                }

                                                #endregion



                                                #region Create SOAPResponse

                                                var HTTPResponse = new HTTPResponse.Builder(Request)
                                                {
                                                    HTTPStatusCode = HTTPStatusCode.OK,
                                                    Server = SOAPServer.HTTPServer.DefaultServerName,
                                                    Date = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                    Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                         "/RemoteStartTransactionResponse",
                                                                                         NextMessageId(),
                                                                                         OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                         OCPPHeader.To,         // Fake it!
                                                                                         OCPPHeader.From,       // Fake it!
                                                                                         response.ToXML()).ToUTF8Bytes()
                                                };

                                                #endregion


                                                #region Send OnRemoteStartTransactionSOAPResponse event

                                                try
                                                {

                                                    OnRemoteStartTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                                 this.SOAPServer.HTTPServer,
                                                                                                 Request,
                                                                                                 HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnRemoteStartTransactionSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion

            #region / - RemoteStopTransaction

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "RemoteStopTransaction",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "remoteStopTransactionRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, RemoteStopTransactionXML) =>
                                            {

                                                #region Send OnRemoteStopTransactionSOAPRequest event

                                                try
                                                {

                                                    OnRemoteStopTransactionSOAPRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                                               this.SOAPServer.HTTPServer,
                                                                                               Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnRemoteStopTransactionSOAPRequest));
                                                }

                                                #endregion


                                                var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                var request = RemoteStopTransactionRequest.Parse(RemoteStopTransactionXML,
                                                                                                     Request_Id.Parse(OCPPHeader.MessageId),
                                                                                                     OCPPHeader.ChargeBoxIdentity);

                                                RemoteStopTransactionResponse response = null;



                                                #region Call async subscribers

                                                if (response == null)
                                                {

                                                    var results = OnRemoteStopTransaction?.
                                                                      GetInvocationList()?.
                                                                      SafeSelect(subscriber => (subscriber as OnRemoteStopTransactionDelegate)
                                                                          (Timestamp.Now,
                                                                           this,
                                                                           request,
                                                                           Request.CancellationToken)).
                                                                      ToArray();

                                                    if (results.Length > 0)
                                                    {

                                                        await Task.WhenAll(results);

                                                        response = results.FirstOrDefault()?.Result;

                                                    }

                                                    if (results.Length == 0 || response == null)
                                                        response = RemoteStopTransactionResponse.Failed(request);

                                                }

                                                #endregion



                                                #region Create SOAPResponse

                                                var HTTPResponse = new HTTPResponse.Builder(Request)
                                                {
                                                    HTTPStatusCode = HTTPStatusCode.OK,
                                                    Server = SOAPServer.HTTPServer.DefaultServerName,
                                                    Date = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                    Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                         "/RemoteStopTransactionResponse",
                                                                                         null,
                                                                                         OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                         OCPPHeader.To,         // Fake it!
                                                                                         OCPPHeader.From,       // Fake it!
                                                                                         response.ToXML()).ToUTF8Bytes()
                                                };

                                                #endregion


                                                #region Send OnRemoteStopTransactionSOAPResponse event

                                                try
                                                {

                                                    OnRemoteStopTransactionSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                                this.SOAPServer.HTTPServer,
                                                                                                Request,
                                                                                                HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnRemoteStopTransactionSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion


            #region / - DataTransfer

            SOAPServer.RegisterSOAPDelegate(HTTPHostname.Any,
                                            URLPrefix,
                                            "DataTransfer",
                                            XML => XML.Descendants(OCPPNS.OCPPv1_6_CP + "dataTransferRequest").FirstOrDefault(),
                                            async (Request, HeaderXML, DataTransferXML) =>
                                            {

                                                #region Send OnIncomingDataTransferSOAPRequest event

                                                try
                                                {

                                                    OnIncomingDataTransferSOAPRequest?.Invoke(Timestamp.Now,
                                                                                              this.SOAPServer.HTTPServer,
                                                                                              Request);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPRequest));
                                                }

                                                #endregion


                                                var OCPPHeader = SOAPHeader.Parse(HeaderXML);
                                                var request = CS.DataTransferRequest.Parse(DataTransferXML,
                                                                                               Request_Id.Parse(OCPPHeader.MessageId),
                                                                                               OCPPHeader.ChargeBoxIdentity);

                                                CP.DataTransferResponse response = null;



                                                #region Call async subscribers

                                                if (response == null)
                                                {

                                                    var results = OnIncomingDataTransfer?.
                                                                      GetInvocationList()?.
                                                                      SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)
                                                                          (Timestamp.Now,
                                                                           this,
                                                                           request,
                                                                           Request.CancellationToken)).
                                                                      ToArray();

                                                    if (results.Length > 0)
                                                    {

                                                        await Task.WhenAll(results);

                                                        response = results.FirstOrDefault()?.Result;

                                                    }

                                                    if (results.Length == 0 || response == null)
                                                        response = DataTransferResponse.Failed(request);

                                                }

                                                #endregion



                                                #region Create SOAPResponse

                                                var HTTPResponse = new HTTPResponse.Builder(Request)
                                                {
                                                    HTTPStatusCode = HTTPStatusCode.OK,
                                                    Server = SOAPServer.HTTPServer.DefaultServerName,
                                                    Date = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    ContentType = HTTPContentType.XMLTEXT_UTF8,
                                                    Content = SOAP.Encapsulation(OCPPHeader.ChargeBoxIdentity,
                                                                                         "/DataTransferResponse",
                                                                                         null,
                                                                                         OCPPHeader.MessageId,  // MessageId from the request as RelatesTo Id
                                                                                         OCPPHeader.To,         // Fake it!
                                                                                         OCPPHeader.From,       // Fake it!
                                                                                         response.ToXML()).ToUTF8Bytes()
                                                };

                                                #endregion


                                                #region Send OnIncomingDataTransferSOAPResponse event

                                                try
                                                {

                                                    OnIncomingDataTransferSOAPResponse?.Invoke(HTTPResponse.Timestamp,
                                                                                               SOAPServer.HTTPServer,
                                                                                               Request,
                                                                                               HTTPResponse);

                                                }
                                                catch (Exception e)
                                                {
                                                    DebugX.Log(e, nameof(ChargePointSOAPServer) + "." + nameof(OnIncomingDataTransferSOAPResponse));
                                                }

                                                #endregion

                                                return HTTPResponse;

                                            });

            #endregion

        }

        #endregion


    }

}
