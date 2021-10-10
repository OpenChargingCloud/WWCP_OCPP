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
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;

using cloud.charging.open.protocols.OCPPv1_6.CP;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system SOAP client runs at the central system
    /// and connects to a charge point to invoke methods.
    /// </summary>
    public partial class CentralSystemSOAPClient : ASOAPClient,
                                                   ICSClient
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const           String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.Number + " CP Client";

        /// <summary>
        /// The default remote TCP port to connect to.
        /// </summary>
        public new static readonly IPPort  DefaultRemotePort     = IPPort.Parse(443);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id    ChargeBoxIdentity   { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => nameof(CentralSystemSOAPClient);

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                  { get; }


        /// <summary>
        /// The attached OCPP CS client (HTTP/SOAP client) logger.
        /// </summary>
        public CSClientLogger  Logger              { get; }

        #endregion

        #region Events

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event fired whenever a cancel reservation request will be send to a charge point.
        /// </summary>
        public event OnCancelReservationRequestDelegate   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a cancel reservation SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler              OnCancelReservationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler             OnCancelReservationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        #endregion

        #region OnChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event fired whenever a change availability request will be send to a charge point.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a change availability SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler               OnChangeAvailabilitySOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a change availability SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler              OnChangeAvailabilitySOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate  OnChangeAvailabilityResponse;

        #endregion

        #region OnChangeConfigurationRequest/-Response

        /// <summary>
        /// An event fired whenever a change configuration request will be send to a charge point.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate   OnChangeConfigurationRequest;

        /// <summary>
        /// An event fired whenever a change configuration SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                OnChangeConfigurationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a change configuration SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler               OnChangeConfigurationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a change configuration request was received.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate  OnChangeConfigurationResponse;

        #endregion

        #region OnClearCacheRequest/-Response

        /// <summary>
        /// An event fired whenever a clear cache request will be send to a charge point.
        /// </summary>
        public event OnClearCacheRequestDelegate   OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a clear cache SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler       OnClearCacheSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a clear cache SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler      OnClearCacheSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a clear cache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate  OnClearCacheResponse;

        #endregion

        #region OnClearChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a clear charging profile request will be send to a charge point.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate   OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a clear charging profile SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                 OnClearChargingProfileSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a clear charging profile SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                OnClearChargingProfileSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate  OnClearChargingProfileResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to a charge point.
        /// </summary>
        public event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler         OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnGetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event fired whenever a get composite schedule request will be send to a charge point.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a get composite schedule SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                 OnGetCompositeScheduleSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get composite schedule SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                OnGetCompositeScheduleSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get composite schedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate  OnGetCompositeScheduleResponse;

        #endregion

        #region OnGetConfigurationRequest/-Response

        /// <summary>
        /// An event fired whenever a get configuration request will be send to a charge point.
        /// </summary>
        public event OnGetConfigurationRequestDelegate   OnGetConfigurationRequest;

        /// <summary>
        /// An event fired whenever a get configuration SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler             OnGetConfigurationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get configuration SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnGetConfigurationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get configuration request was received.
        /// </summary>
        public event OnGetConfigurationResponseDelegate  OnGetConfigurationResponse;

        #endregion

        #region OnGetDiagnosticsRequest/-Response

        /// <summary>
        /// An event fired whenever a get diagnostics request will be send to a charge point.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event fired whenever a get diagnostics SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler           OnGetDiagnosticsSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get diagnostics SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler          OnGetDiagnosticsSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get diagnostics request was received.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate  OnGetDiagnosticsResponse;

        #endregion

        #region OnGetLocalListVersionRequest/-Response

        /// <summary>
        /// An event fired whenever a get local list version request will be send to a charge point.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a get local list version SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                OnGetLocalListVersionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get local list version SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler               OnGetLocalListVersionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get local list version request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate  OnGetLocalListVersionResponse;

        #endregion

        #region OnRemoteStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote start transaction request will be send to a charge point.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote start transaction SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                   OnRemoteStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                  OnRemoteStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote stop transaction request will be send to a charge point.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote stop transaction SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler                  OnRemoteStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                 OnRemoteStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate  OnRemoteStopTransactionResponse;

        #endregion

        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event fired whenever a reserve now request will be send to a charge point.
        /// </summary>
        public event OnReserveNowRequestDelegate   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a reserve now SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler       OnReserveNowSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a reserve now SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler      OnReserveNowSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a reserve now request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate  OnReserveNowResponse;

        #endregion

        #region OnResetRequest/-Response

        /// <summary>
        /// An event fired whenever a reset request will be send to a charge point.
        /// </summary>
        public event OnResetRequestDelegate    OnResetRequest;

        /// <summary>
        /// An event fired whenever a reset SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler   OnResetSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a reset SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnResetSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a reset request was received.
        /// </summary>
        public event OnResetResponseDelegate   OnResetResponse;

        #endregion

        #region OnSendLocalListRequest/-Response

        /// <summary>
        /// An event fired whenever a send local list request will be send to a charge point.
        /// </summary>
        public event OnSendLocalListRequestDelegate   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a send local list SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler          OnSendLocalListSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a send local list SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler         OnSendLocalListSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a send local list request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate  OnSendLocalListResponse;

        #endregion

        #region OnSetChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a set charging profile request will be send to a charge point.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a set charging profile SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler               OnSetChargingProfileSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a set charging profile SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler              OnSetChargingProfileSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate  OnSetChargingProfileResponse;

        #endregion

        #region OnTriggerMessageRequest/-Response

        /// <summary>
        /// An event fired whenever a trigger message request will be send to a charge point.
        /// </summary>
        public event OnTriggerMessageRequestDelegate   OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a trigger message SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler           OnTriggerMessageSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a trigger message SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler          OnTriggerMessageSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageResponseDelegate  OnTriggerMessageResponse;

        #endregion

        #region OnUnlockConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a unlock connector request will be send to a charge point.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a unlock connector SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler            OnUnlockConnectorSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a unlock connector SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler           OnUnlockConnectorSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a unlock connector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate  OnUnlockConnectorResponse;

        #endregion

        #region OnUpdateFirmwareRequest/-Response

        /// <summary>
        /// An event fired whenever a update firmware request will be send to a charge point.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a update firmware SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler           OnUpdateFirmwareSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a update firmware SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler          OnUpdateFirmwareSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate  OnUpdateFirmwareResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region CentralSystemSOAPClient(ChargeBoxIdentity, Hostname, ..., LoggingContext = CSClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new central system SOAP client running at the central system
        /// and connecting to a charge point to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this OCPP charge box.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/SOAP client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="WSSLoginPassword">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CentralSystemSOAPClient(ChargeBox_Id                         ChargeBoxIdentity,
                                       String                               From,
                                       String                               To,

                                       URL                                  RemoteURL,
                                       HTTPHostname?                        VirtualHostname              = null,
                                       String                               Description                  = null,
                                       RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                                       X509Certificate                      ClientCert                   = null,
                                       String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                       HTTPPath?                            URLPathPrefix                = null,
                                       Tuple<String, String>                WSSLoginPassword             = null,
                                       TimeSpan?                            RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate       TransmissionRetryDelay       = null,
                                       UInt16?                              MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
                                       Boolean                              UseHTTPPipelining            = false,
                                       String                               LoggingContext               = CSClientLogger.DefaultContext,
                                       LogfileCreatorDelegate               LogFileCreator               = null,
                                       HTTPClientLogger                     HTTPLogger                   = null,
                                       DNSClient                            DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   HTTPUserAgent,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   WSSLoginPassword,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   UseHTTPPipelining,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity  = ChargeBoxIdentity;
            this.From               = From;
            this.To                 = To;

            this.Logger             = new CSClientLogger(this,
                                                         LoggingContext,
                                                         LogFileCreator);

        }

        #endregion

        #region CentralSystemSOAPClient(ChargeBoxIdentity, Logger, Hostname, ...)

        ///// <summary>
        ///// Create a new central system SOAP client.
        ///// </summary>
        ///// <param name="ChargeBoxIdentity">A unqiue identification of this client.</param>
        ///// <param name="From">The source URI of the SOAP message.</param>
        ///// <param name="To">The destination URI of the SOAP message.</param>
        ///// 
        ///// <param name="Hostname">The OCPP hostname to connect to.</param>
        ///// <param name="RemotePort">An optional OCPP TCP port to connect to.</param>
        ///// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        ///// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        ///// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        ///// <param name="URLPrefix">An default URI prefix.</param>
        ///// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        ///// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        ///// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        ///// <param name="DNSClient">An optional DNS client.</param>
        //public CentralSystemSOAPClient(ChargeBox_Id                         ChargeBoxIdentity,
        //                               String                               From,
        //                               String                               To,

        //                               CSClientLogger                       Logger,
        //                               HTTPHostname                         Hostname,
        //                               IPPort?                              RemotePort                   = null,
        //                               RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
        //                               LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
        //                               HTTPHostname?                        HTTPVirtualHost              = null,
        //                               HTTPPath?                            URLPrefix                    = null,
        //                               String                               HTTPUserAgent                = DefaultHTTPUserAgent,
        //                               TimeSpan?                            RequestTimeout               = null,
        //                               Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
        //                               DNSClient                            DNSClient                    = null)

        //    : base(ChargeBoxIdentity.ToString(),
        //           Hostname,
        //           RemotePort ?? DefaultRemotePort,
        //           RemoteCertificateValidator,
        //           ClientCertificateSelector,
        //           HTTPVirtualHost,
        //           URLPrefix ?? DefaultURLPrefix,
        //           null,
        //           HTTPUserAgent,
        //           RequestTimeout,
        //           null,
        //           MaxNumberOfRetries,
        //           DNSClient)

        //{

        //    #region Initial checks

        //    if (ChargeBoxIdentity.IsNullOrEmpty)
        //        throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

        //    if (From.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

        //    if (To.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

        //    #endregion

        //    this.From    = From;
        //    this.To      = To;

        //    this.Logger  = Logger ?? throw new ArgumentNullException(nameof(Logger), "The given client logger must not be null!"); ;

        //}

        #endregion

        #endregion


        private String NextMessageId()
            => Guid.NewGuid().ToString();


        // When a Central System needs to send requests to a Charge Point, the Central System MUST specify in
        // each request the “chargeBoxIdentity” of the Charge Point for which the request is intended.If the
        // receiving Charge Point is not the intended one, and it cannot relay the message to a node that knows
        // this Charge Point, then the Charge Point MUST send a SOAP Fault Response message, indicating that the
        // identity is wrong(e.g.sub-code is “IdentityMismatch”).

        #region Reset                 (Type, ...)

        /// <summary>
        /// Reset the charging station.
        /// </summary>
        /// <param name="Type">The type of reset that the charge point should perform.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ResetResponse>>

            Reset(ResetTypes          Type,

                  DateTime?           Timestamp           = null,
                  CancellationToken?  CancellationToken   = null,
                  EventTracking_Id    EventTrackingId     = null,
                  TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ResetResponse> result = null;

            #endregion

            #region Send OnResetRequest event

            try
            {

                OnResetRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                       Timestamp.Value,
                                       this,
                                       Description,
                                       EventTrackingId,
                                       ChargeBoxIdentity,
                                       Type,
                                       RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnResetRequest));
            }

            #endregion


            var request = new ResetRequest(ChargeBoxIdentity,
                                           Type);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/Reset",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "Reset",
                                                     RequestLogDelegate:   OnResetSOAPRequest,
                                                     ResponseLogDelegate:  OnResetSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          ResetResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ResetResponse>(httpresponse,
                                                                                                     new ResetResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ResetResponse>(httpresponse,
                                                                                                     new ResetResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ResetResponse>.ExceptionThrown(new ResetResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ResetResponse>.OK(new ResetResponse(request,
                                                                          Result.OK("Nothing to upload!")));


            #region Send OnResetResponse event

            try
            {

                OnResetResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             Timestamp.Value,
                                             this,
                                             Description,
                                             EventTrackingId,
                                             ChargeBoxIdentity,
                                             Type,
                                             RequestTimeout,
                                             result.Content,
                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnResetResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region ChangeAvailability    (ConnectorId, Availability, ...)

        /// <summary>
        /// Change the availability of a connector.
        /// </summary>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ChangeAvailabilityResponse>>

            ChangeAvailability(Connector_Id        ConnectorId,
                               Availabilities      Availability,

                               DateTime?           Timestamp           = null,
                               CancellationToken?  CancellationToken   = null,
                               EventTracking_Id    EventTrackingId     = null,
                               TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (ConnectorId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ConnectorId),  "The given connector identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ChangeAvailabilityResponse> result = null;

            #endregion

            #region Send OnChangeAvailabilityRequest event

            try
            {

                OnChangeAvailabilityRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    Timestamp.Value,
                                                    this,
                                                    Description,
                                                    EventTrackingId,
                                                    ChargeBoxIdentity,
                                                    ConnectorId,
                                                    Availability,
                                                    RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            var request = new ChangeAvailabilityRequest(ChargeBoxIdentity,
                                                        ConnectorId,
                                                        Availability);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/ChangeAvailability",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "ChangeAvailability",
                                                     RequestLogDelegate:   OnChangeAvailabilitySOAPRequest,
                                                     ResponseLogDelegate:  OnChangeAvailabilitySOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          ChangeAvailabilityResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ChangeAvailabilityResponse>(httpresponse,
                                                                                                     new ChangeAvailabilityResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ChangeAvailabilityResponse>(httpresponse,
                                                                                                     new ChangeAvailabilityResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ChangeAvailabilityResponse>.ExceptionThrown(new ChangeAvailabilityResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ChangeAvailabilityResponse>.OK(new ChangeAvailabilityResponse(request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnChangeAvailabilityResponse event

            try
            {

                OnChangeAvailabilityResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     Timestamp.Value,
                                                     this,
                                                     Description,
                                                     EventTrackingId,
                                                     ChargeBoxIdentity,
                                                     ConnectorId,
                                                     Availability,
                                                     RequestTimeout,
                                                     result.Content,
                                                     org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region GetConfiguration      (Keys = null, ...)

        /// <summary>
        /// Get the configuration of the charging station.
        /// </summary>
        /// <param name="Keys">An enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<GetConfigurationResponse>>

            GetConfiguration(IEnumerable<String> Keys                = null,

                             DateTime?           Timestamp           = null,
                             CancellationToken?  CancellationToken   = null,
                             EventTracking_Id    EventTrackingId     = null,
                             TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<GetConfigurationResponse> result = null;

            #endregion

            #region Send OnGetConfigurationRequest event

            try
            {

                OnGetConfigurationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  Timestamp.Value,
                                                  this,
                                                  Description,
                                                  EventTrackingId,
                                                  ChargeBoxIdentity,
                                                  Keys,
                                                  RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            var request = new GetConfigurationRequest(ChargeBoxIdentity,
                                                      Keys);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/GetConfiguration",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "GetConfiguration",
                                                     RequestLogDelegate:   OnGetConfigurationSOAPRequest,
                                                     ResponseLogDelegate:  OnGetConfigurationSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          GetConfigurationResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<GetConfigurationResponse>(httpresponse,
                                                                                                     new GetConfigurationResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<GetConfigurationResponse>(httpresponse,
                                                                                                     new GetConfigurationResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<GetConfigurationResponse>.ExceptionThrown(new GetConfigurationResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<GetConfigurationResponse>.OK(new GetConfigurationResponse(request,
                                                                                                Result.OK("Nothing to upload!")));


            #region Send OnGetConfigurationResponse event

            try
            {

                OnGetConfigurationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   Description,
                                                   EventTrackingId,
                                                   ChargeBoxIdentity,
                                                   Keys,
                                                   RequestTimeout,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetConfigurationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region ChangeConfiguration   (Key, Value, ...)

        /// <summary>
        /// Change a configuration key within the charging station.
        /// </summary>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ChangeConfigurationResponse>>

            ChangeConfiguration(String              Key,
                                String              Value,

                                DateTime?           Timestamp           = null,
                                CancellationToken?  CancellationToken   = null,
                                EventTracking_Id    EventTrackingId     = null,
                                TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Key?.Trim().IsNullOrEmpty() == true)
                throw new ArgumentNullException(nameof(Key),  "The given configuration key must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ChangeConfigurationResponse> result = null;

            #endregion

            #region Send OnChangeConfigurationRequest event

            try
            {

                OnChangeConfigurationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     Timestamp.Value,
                                                     this,
                                                     Description,
                                                     EventTrackingId,
                                                     ChargeBoxIdentity,
                                                     Key,
                                                     Value,
                                                     RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            var request = new ChangeConfigurationRequest(ChargeBoxIdentity,
                                                         Key,
                                                         Value);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/ChangeConfiguration",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "ChangeConfiguration",
                                                     RequestLogDelegate:   OnChangeConfigurationSOAPRequest,
                                                     ResponseLogDelegate:  OnChangeConfigurationSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          ChangeConfigurationResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ChangeConfigurationResponse>(httpresponse,
                                                                                                     new ChangeConfigurationResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ChangeConfigurationResponse>(httpresponse,
                                                                                                     new ChangeConfigurationResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ChangeConfigurationResponse>.ExceptionThrown(new ChangeConfigurationResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ChangeConfigurationResponse>.OK(new ChangeConfigurationResponse(request,
                                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnChangeConfigurationResponse event

            try
            {

                OnChangeConfigurationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                      Timestamp.Value,
                                                      this,
                                                      Description,
                                                      EventTrackingId,
                                                      ChargeBoxIdentity,
                                                      Key,
                                                      Value,
                                                      RequestTimeout,
                                                      result.Content,
                                                      org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region DataTransfer          (VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">The charge point model identification.</param>
        /// <param name="Data">The serial number of the charge point.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<CP.DataTransferResponse>>

            DataTransfer(String              VendorId,
                         String              MessageId           = null,
                         String              Data                = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (VendorId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorId),  "The given vendor identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<CP.DataTransferResponse> result = null;

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              Timestamp.Value,
                                              this,
                                              Description,
                                              EventTrackingId,
                                              VendorId,
                                              MessageId,
                                              Data,
                                              RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var request = new DataTransferRequest(ChargeBoxIdentity,
                                                  VendorId,
                                                  MessageId,
                                                  Data);


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/DataTransfer",
                                                                    NextMessageId(),
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "DataTransfer",
                                                 RequestLogDelegate:   OnDataTransferSOAPRequest,
                                                 ResponseLogDelegate:  OnDataTransferSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      CP.DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<CP.DataTransferResponse>(httpresponse,
                                                                                                      new CP.DataTransferResponse(
                                                                                                          request,
                                                                                                          Result.Format(
                                                                                                              "Invalid SOAP => " +
                                                                                                              httpresponse.HTTPBody.ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<CP.DataTransferResponse>(httpresponse,
                                                                                                      new CP.DataTransferResponse(
                                                                                                          request,
                                                                                                          Result.Server(
                                                                                                               httpresponse.HTTPStatusCode.ToString() +
                                                                                                               " => " +
                                                                                                               httpresponse.HTTPBody.      ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<CP.DataTransferResponse>.ExceptionThrown(new CP.DataTransferResponse(
                                                                                                                      request,
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<CP.DataTransferResponse>.OK(new CP.DataTransferResponse(request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                               Timestamp.Value,
                                               this,
                                               Description,
                                               EventTrackingId,
                                               VendorId,
                                               MessageId,
                                               Data,
                                               RequestTimeout,
                                               result.Content,
                                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region GetDiagnostics        (Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Get diagonstic data from the charging station.
        /// </summary>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<GetDiagnosticsResponse>>

            GetDiagnostics(String              Location,
                           DateTime?           StartTime           = null,
                           DateTime?           StopTime            = null,
                           Byte?               Retries             = null,
                           TimeSpan?           RetryInterval       = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken?  CancellationToken   = null,
                           EventTracking_Id    EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Location?.Trim().IsNullOrEmpty() == true)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<GetDiagnosticsResponse> result = null;

            #endregion

            #region Send OnGetDiagnosticsRequest event

            try
            {

                OnGetDiagnosticsRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                Timestamp.Value,
                                                this,
                                                Description,
                                                EventTrackingId,
                                                ChargeBoxIdentity,
                                                Location,
                                                StartTime,
                                                StopTime,
                                                Retries,
                                                RetryInterval,
                                                RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            var request = new GetDiagnosticsRequest(ChargeBoxIdentity,
                                                    Location,
                                                    StartTime,
                                                    StopTime,
                                                    Retries,
                                                    RetryInterval);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/GetDiagnostics",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "GetDiagnostics",
                                                     RequestLogDelegate:   OnGetDiagnosticsSOAPRequest,
                                                     ResponseLogDelegate:  OnGetDiagnosticsSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          GetDiagnosticsResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<GetDiagnosticsResponse>(httpresponse,
                                                                                                     new GetDiagnosticsResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<GetDiagnosticsResponse>(httpresponse,
                                                                                                     new GetDiagnosticsResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<GetDiagnosticsResponse>.ExceptionThrown(new GetDiagnosticsResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<GetDiagnosticsResponse>.OK(new GetDiagnosticsResponse(request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnGetDiagnosticsResponse event

            try
            {

                OnGetDiagnosticsResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 Timestamp.Value,
                                                 this,
                                                 Description,
                                                 EventTrackingId,
                                                 ChargeBoxIdentity,
                                                 Location,
                                                 StartTime,
                                                 StopTime,
                                                 Retries,
                                                 RetryInterval,
                                                 RequestTimeout,
                                                 result.Content,
                                                 org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetDiagnosticsResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region TriggerMessage        (RequestedMessage, ConnectorId = null, ...)

        /// <summary>
        /// Set a message trigger within the charging station.
        /// </summary>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<TriggerMessageResponse>>

            TriggerMessage(MessageTriggers     RequestedMessage,
                           Connector_Id?       ConnectorId         = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken?  CancellationToken   = null,
                           EventTracking_Id    EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<TriggerMessageResponse> result = null;

            #endregion

            #region Send OnTriggerMessageRequest event

            try
            {

                OnTriggerMessageRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                Timestamp.Value,
                                                this,
                                                Description,
                                                EventTrackingId,
                                                ChargeBoxIdentity,
                                                RequestedMessage,
                                                ConnectorId,
                                                RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            var request = new TriggerMessageRequest(ChargeBoxIdentity,
                                                    RequestedMessage,
                                                    ConnectorId);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/TriggerMessage",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "TriggerMessage",
                                                     RequestLogDelegate:   OnTriggerMessageSOAPRequest,
                                                     ResponseLogDelegate:  OnTriggerMessageSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          TriggerMessageResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<TriggerMessageResponse>(httpresponse,
                                                                                                     new TriggerMessageResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<TriggerMessageResponse>(httpresponse,
                                                                                                     new TriggerMessageResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<TriggerMessageResponse>.ExceptionThrown(new TriggerMessageResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<TriggerMessageResponse>.OK(new TriggerMessageResponse(request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnTriggerMessageResponse event

            try
            {

                OnTriggerMessageResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 Timestamp.Value,
                                                 this,
                                                 Description,
                                                 EventTrackingId,
                                                 ChargeBoxIdentity,
                                                 RequestedMessage,
                                                 ConnectorId,
                                                 RequestTimeout,
                                                 result.Content,
                                                 org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region UpdateFirmware        (Location, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Reserve a connector for the given IdTag and till the given timestamp.
        /// </summary>
        /// <param name="Location">The URI where to download the firmware.</param>
        /// <param name="RetrieveDate">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<UpdateFirmwareResponse>>

            UpdateFirmware(String              Location,
                           DateTime            RetrieveDate,
                           Byte?               Retries            = null,
                           TimeSpan?           RetryInterval      = null,

                           DateTime?           Timestamp          = null,
                           CancellationToken?  CancellationToken  = null,
                           EventTracking_Id    EventTrackingId    = null,
                           TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (Location?.Trim().IsNullOrEmpty() == true)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<UpdateFirmwareResponse> result = null;

            #endregion

            #region Send OnUpdateFirmwareRequest event

            try
            {

                OnUpdateFirmwareRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                Timestamp.Value,
                                                this,
                                                Description,
                                                EventTrackingId,
                                                ChargeBoxIdentity,
                                                Location,
                                                RetrieveDate,
                                                Retries,
                                                RetryInterval,
                                                RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            var request = new UpdateFirmwareRequest(ChargeBoxIdentity,
                                                    Location,
                                                    RetrieveDate,
                                                    Retries,
                                                    RetryInterval);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/UpdateFirmware",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "UpdateFirmware",
                                                     RequestLogDelegate:   OnUpdateFirmwareSOAPRequest,
                                                     ResponseLogDelegate:  OnUpdateFirmwareSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          UpdateFirmwareResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<UpdateFirmwareResponse>(httpresponse,
                                                                                                     new UpdateFirmwareResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<UpdateFirmwareResponse>(httpresponse,
                                                                                                     new UpdateFirmwareResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<UpdateFirmwareResponse>.ExceptionThrown(new UpdateFirmwareResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<UpdateFirmwareResponse>.OK(new UpdateFirmwareResponse(request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnUpdateFirmwareResponse event

            try
            {

                OnUpdateFirmwareResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 Timestamp.Value,
                                                 this,
                                                 Description,
                                                 EventTrackingId,
                                                 ChargeBoxIdentity,
                                                 Location,
                                                 RetrieveDate,
                                                 Retries,
                                                 RetryInterval,
                                                 RequestTimeout,
                                                 result.Content,
                                                 org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region ReserveNow            (Request, ...)

        /// <summary>
        /// Reserve a connector for the given IdTag and till the given timestamp.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ReserveNowResponse>>

            ReserveNow(ReserveNowRequest   Request,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given reserve now request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ReserveNowResponse> result = null;

            #endregion

            #region Send OnReserveNowRequest event

            try
            {

                OnReserveNowRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            this,
                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/ReserveNow",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        Request.ToXML()),
                                                     "ReserveNow",
                                                     RequestLogDelegate:   OnReserveNowSOAPRequest,
                                                     ResponseLogDelegate:  OnReserveNowSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                          ReserveNowResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ReserveNowResponse>(httpresponse,
                                                                                                     new ReserveNowResponse(
                                                                                                         Request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ReserveNowResponse>(httpresponse,
                                                                                                     new ReserveNowResponse(
                                                                                                         Request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ReserveNowResponse>.ExceptionThrown(new ReserveNowResponse(
                                                                                                                     Request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ReserveNowResponse>.OK(new ReserveNowResponse(Request,
                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnReserveNowResponse event

            try
            {

                OnReserveNowResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             this,
                                             Request,
                                             result.Content,
                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region CancelReservation     (ReservationId, ...)

        /// <summary>
        /// Cancel the given reservation.
        /// </summary>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<CancelReservationResponse>>

            CancelReservation(Reservation_Id      ReservationId,

                              DateTime?           Timestamp           = null,
                              CancellationToken?  CancellationToken   = null,
                              EventTracking_Id    EventTrackingId     = null,
                              TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (ReservationId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ReservationId),  "The given reservation identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<CancelReservationResponse> result = null;

            #endregion

            #region Send OnCancelReservationRequest event

            try
            {

                OnCancelReservationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   Description,
                                                   EventTrackingId,
                                                   ChargeBoxIdentity,
                                                   ReservationId,
                                                   RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            var request = new CancelReservationRequest(ChargeBoxIdentity,
                                                       ReservationId);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/CancelReservation",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "CancelReservation",
                                                     RequestLogDelegate:   OnCancelReservationSOAPRequest,
                                                     ResponseLogDelegate:  OnCancelReservationSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          CancelReservationResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<CancelReservationResponse>(httpresponse,
                                                                                                            new CancelReservationResponse(
                                                                                                                request,
                                                                                                                Result.Format(
                                                                                                                    "Invalid SOAP => " +
                                                                                                                    httpresponse.HTTPBody.ToUTF8String()
                                                                                                                )
                                                                                                            ),
                                                                                                            IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<CancelReservationResponse>(httpresponse,
                                                                                                            new CancelReservationResponse(
                                                                                                                request,
                                                                                                                Result.Server(
                                                                                                                     httpresponse.HTTPStatusCode.ToString() +
                                                                                                                     " => " +
                                                                                                                     httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                )
                                                                                                            ),
                                                                                                            IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<CancelReservationResponse>.ExceptionThrown(new CancelReservationResponse(
                                                                                                                            request,
                                                                                                                            Result.Format(exception.Message +
                                                                                                                                          " => " +
                                                                                                                                          exception.StackTrace)),
                                                                                                                        exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<CancelReservationResponse>.OK(new CancelReservationResponse(request,
                                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnCancelReservationResponse event

            try
            {

                OnCancelReservationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    Timestamp.Value,
                                                    this,
                                                    Description,
                                                    EventTrackingId,
                                                    ChargeBoxIdentity,
                                                    ReservationId,
                                                    RequestTimeout,
                                                    result.Content,
                                                    org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region RemoteStartTransaction(IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Starte a charging transaction at the given connector.
        /// </summary>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<RemoteStartTransactionResponse>>

            RemoteStartTransaction(IdToken             IdTag,
                                   Connector_Id?       ConnectorId         = null,
                                   ChargingProfile     ChargingProfile     = null,

                                   DateTime?           Timestamp           = null,
                                   CancellationToken?  CancellationToken   = null,
                                   EventTracking_Id    EventTrackingId     = null,
                                   TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (IdTag == null)
                throw new ArgumentNullException(nameof(IdTag),  "The given identification tag must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<RemoteStartTransactionResponse> result = null;

            #endregion

            #region Send OnRemoteStartTransactionRequest event

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                        Timestamp.Value,
                                                        this,
                                                        Description,
                                                        EventTrackingId,
                                                        ChargeBoxIdentity,
                                                        IdTag,
                                                        ConnectorId,
                                                        ChargingProfile,
                                                        RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            var request = new RemoteStartTransactionRequest(ChargeBoxIdentity,
                                                            IdTag,
                                                            ConnectorId,
                                                            ChargingProfile);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/RemoteStartTransaction",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "RemoteStartTransaction",
                                                     ContentType:          HTTPContentType.XMLTEXT_UTF8,
                                                     RequestLogDelegate:   OnRemoteStartTransactionSOAPRequest,
                                                     ResponseLogDelegate:  OnRemoteStartTransactionSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          RemoteStartTransactionResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<RemoteStartTransactionResponse>(httpresponse,
                                                                                                                 new RemoteStartTransactionResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(
                                                                                                                         "Invalid SOAP => " +
                                                                                                                         httpresponse.HTTPBody.ToUTF8String()
                                                                                                                     )
                                                                                                                 ),
                                                                                                                 IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<RemoteStartTransactionResponse>(httpresponse,
                                                                                                                 new RemoteStartTransactionResponse(
                                                                                                                     request,
                                                                                                                     Result.Server(
                                                                                                                          httpresponse.HTTPStatusCode.ToString() +
                                                                                                                          " => " +
                                                                                                                          httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                     )
                                                                                                                 ),
                                                                                                                 IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<RemoteStartTransactionResponse>.ExceptionThrown(new RemoteStartTransactionResponse(
                                                                                                                                 request,
                                                                                                                                 Result.Format(exception.Message +
                                                                                                                                               " => " +
                                                                                                                                               exception.StackTrace)),
                                                                                                                             exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<RemoteStartTransactionResponse>.OK(new RemoteStartTransactionResponse(request,
                                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnRemoteStartTransactionResponse event

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                         Timestamp.Value,
                                                         this,
                                                         Description,
                                                         EventTrackingId,
                                                         ChargeBoxIdentity,
                                                         IdTag,
                                                         ConnectorId,
                                                         ChargingProfile,
                                                         RequestTimeout,
                                                         result.Content,
                                                         org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region RemoteStopTransaction (TransactionId, ...)

        /// <summary>
        /// Stop the given charging transaction.
        /// </summary>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<RemoteStopTransactionResponse>>

            RemoteStopTransaction(Transaction_Id      TransactionId,

                                  DateTime?           Timestamp           = null,
                                  CancellationToken?  CancellationToken   = null,
                                  EventTracking_Id    EventTrackingId     = null,
                                  TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (TransactionId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<RemoteStopTransactionResponse> result = null;

            #endregion

            #region Send OnRemoteStopTransactionRequest event

            try
            {

                OnRemoteStopTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                       Timestamp.Value,
                                                       this,
                                                       Description,
                                                       EventTrackingId,
                                                       ChargeBoxIdentity,
                                                       TransactionId,
                                                       RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            var request = new RemoteStopTransactionRequest(ChargeBoxIdentity,
                                                           TransactionId);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/RemoteStopTransaction",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "RemoteStopTransaction",
                                                     RequestLogDelegate:   OnRemoteStopTransactionSOAPRequest,
                                                     ResponseLogDelegate:  OnRemoteStopTransactionSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          RemoteStopTransactionResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<RemoteStopTransactionResponse>(httpresponse,
                                                                                                                new RemoteStopTransactionResponse(
                                                                                                                    request,
                                                                                                                    Result.Format(
                                                                                                                        "Invalid SOAP => " +
                                                                                                                        httpresponse.HTTPBody.ToUTF8String()
                                                                                                                    )
                                                                                                                ),
                                                                                                                IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<RemoteStopTransactionResponse>(httpresponse,
                                                                                                                new RemoteStopTransactionResponse(
                                                                                                                    request,
                                                                                                                    Result.Server(
                                                                                                                         httpresponse.HTTPStatusCode.ToString() +
                                                                                                                         " => " +
                                                                                                                         httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                    )
                                                                                                                ),
                                                                                                                IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<RemoteStopTransactionResponse>.ExceptionThrown(new RemoteStopTransactionResponse(
                                                                                                                                request,
                                                                                                                                Result.Format(exception.Message +
                                                                                                                                              " => " +
                                                                                                                                              exception.StackTrace)),
                                                                                                                            exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<RemoteStopTransactionResponse>.OK(new RemoteStopTransactionResponse(request,
                                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnRemoteStopTransactionResponse event

            try
            {

                OnRemoteStopTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                        Timestamp.Value,
                                                        this,
                                                        Description,
                                                        EventTrackingId,
                                                        ChargeBoxIdentity,
                                                        TransactionId,
                                                        RequestTimeout,
                                                        result.Content,
                                                        org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStopTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SetChargingProfile    (ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Set a charging profile within the charging station.
        /// </summary>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<SetChargingProfileResponse>>

            SetChargingProfile(Connector_Id        ConnectorId,
                               ChargingProfile     ChargingProfile,

                               DateTime?           Timestamp           = null,
                               CancellationToken?  CancellationToken   = null,
                               EventTracking_Id    EventTrackingId     = null,
                               TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (ConnectorId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ConnectorId),      "The given connector identification must not be null or empty!");

            if (ChargingProfile == null)
                throw new ArgumentNullException(nameof(ChargingProfile),  "The given charging profile identification must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<SetChargingProfileResponse> result = null;

            #endregion

            #region Send OnSetChargingProfileRequest event

            try
            {

                OnSetChargingProfileRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    Timestamp.Value,
                                                    this,
                                                    Description,
                                                    EventTrackingId,
                                                    ChargeBoxIdentity,
                                                    ConnectorId,
                                                    ChargingProfile,
                                                    RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            var request = new SetChargingProfileRequest(ChargeBoxIdentity,
                                                        ConnectorId,
                                                        ChargingProfile);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/SetChargingProfile",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "SetChargingProfile",
                                                     RequestLogDelegate:   OnSetChargingProfileSOAPRequest,
                                                     ResponseLogDelegate:  OnSetChargingProfileSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          SetChargingProfileResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<SetChargingProfileResponse>(httpresponse,
                                                                                                     new SetChargingProfileResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<SetChargingProfileResponse>(httpresponse,
                                                                                                     new SetChargingProfileResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<SetChargingProfileResponse>.ExceptionThrown(new SetChargingProfileResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<SetChargingProfileResponse>.OK(new SetChargingProfileResponse(request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnSetChargingProfileResponse event

            try
            {

                OnSetChargingProfileResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     Timestamp.Value,
                                                     this,
                                                     Description,
                                                     EventTrackingId,
                                                     ChargeBoxIdentity,
                                                     ConnectorId,
                                                     ChargingProfile,
                                                     RequestTimeout,
                                                     result.Content,
                                                     org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region ClearChargingProfile  (ChargingProfileId = null, ConnectorId = null, ChargingProfilePurpose = null, StackLevel = null, ...)

        /// <summary>
        /// Clear a charging profile within the charging station.
        /// </summary>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ClearChargingProfileResponse>>

            ClearChargingProfile(ChargingProfile_Id?       ChargingProfileId        = null,
                                 Connector_Id?             ConnectorId              = null,
                                 ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                 UInt32?                   StackLevel               = null,

                                 DateTime?                 Timestamp                = null,
                                 CancellationToken?        CancellationToken        = null,
                                 EventTracking_Id          EventTrackingId          = null,
                                 TimeSpan?                 RequestTimeout           = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ClearChargingProfileResponse> result = null;

            #endregion

            #region Send OnClearChargingProfileRequest event

            try
            {

                OnClearChargingProfileRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                      Timestamp.Value,
                                                      this,
                                                      Description,
                                                      EventTrackingId,
                                                      ChargeBoxIdentity,
                                                      ChargingProfileId,
                                                      ConnectorId,
                                                      ChargingProfilePurpose,
                                                      StackLevel,
                                                      RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            var request = new ClearChargingProfileRequest(ChargeBoxIdentity,
                                                          ChargingProfileId,
                                                          ConnectorId,
                                                          ChargingProfilePurpose,
                                                          StackLevel);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/ClearChargingProfile",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "ClearChargingProfile",
                                                     RequestLogDelegate:   OnClearChargingProfileSOAPRequest,
                                                     ResponseLogDelegate:  OnClearChargingProfileSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          ClearChargingProfileResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ClearChargingProfileResponse>(httpresponse,
                                                                                                     new ClearChargingProfileResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ClearChargingProfileResponse>(httpresponse,
                                                                                                     new ClearChargingProfileResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ClearChargingProfileResponse>.ExceptionThrown(new ClearChargingProfileResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ClearChargingProfileResponse>.OK(new ClearChargingProfileResponse(request,
                                                                                                        Result.OK("Nothing to upload!")));


            #region Send OnClearChargingProfileResponse event

            try
            {

                OnClearChargingProfileResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                       Timestamp.Value,
                                                       this,
                                                       Description,
                                                       EventTrackingId,
                                                       ChargeBoxIdentity,
                                                       ChargingProfileId,
                                                       ConnectorId,
                                                       ChargingProfilePurpose,
                                                       StackLevel,
                                                       RequestTimeout,
                                                       result.Content,
                                                       org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region GetCompositeSchedule  (ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Get the composite schedule.
        /// </summary>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<GetCompositeScheduleResponse>>

            GetCompositeSchedule(Connector_Id        ConnectorId,
                                 TimeSpan            Duration,
                                 ChargingRateUnits?  ChargingRateUnit    = null,

                                 DateTime?           Timestamp           = null,
                                 CancellationToken?  CancellationToken   = null,
                                 EventTracking_Id    EventTrackingId     = null,
                                 TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (ConnectorId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ConnectorId),  "The given connector identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<GetCompositeScheduleResponse> result = null;

            #endregion

            #region Send OnGetCompositeScheduleRequest event

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                      Timestamp.Value,
                                                      this,
                                                      Description,
                                                      EventTrackingId,
                                                      ChargeBoxIdentity,
                                                      ConnectorId,
                                                      Duration,
                                                      ChargingRateUnit,
                                                      RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            var request = new GetCompositeScheduleRequest(ChargeBoxIdentity,
                                                          ConnectorId,
                                                          Duration,
                                                          ChargingRateUnit);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/GetCompositeSchedule",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "GetCompositeSchedule",
                                                     RequestLogDelegate:   OnGetCompositeScheduleSOAPRequest,
                                                     ResponseLogDelegate:  OnGetCompositeScheduleSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          GetCompositeScheduleResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<GetCompositeScheduleResponse>(httpresponse,
                                                                                                     new GetCompositeScheduleResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<GetCompositeScheduleResponse>(httpresponse,
                                                                                                     new GetCompositeScheduleResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<GetCompositeScheduleResponse>.ExceptionThrown(new GetCompositeScheduleResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<GetCompositeScheduleResponse>.OK(new GetCompositeScheduleResponse(request,
                                                                                                        Result.OK("Nothing to upload!")));


            #region Send OnGetCompositeScheduleResponse event

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                       Timestamp.Value,
                                                       this,
                                                       Description,
                                                       EventTrackingId,
                                                       ChargeBoxIdentity,
                                                       ConnectorId,
                                                       Duration,
                                                       ChargingRateUnit,
                                                       RequestTimeout,
                                                       result.Content,
                                                       org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region UnlockConnector       (ConnectorId, ...)

        /// <summary>
        /// Unlock the given connector within the charging station.
        /// </summary>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<UnlockConnectorResponse>>

            UnlockConnector(Connector_Id        ConnectorId,

                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (ConnectorId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ConnectorId),  "The given connector identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<UnlockConnectorResponse> result = null;

            #endregion

            #region Send OnUnlockConnectorRequest event

            try
            {

                OnUnlockConnectorRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 Timestamp.Value,
                                                 this,
                                                 Description,
                                                 EventTrackingId,
                                                 ChargeBoxIdentity,
                                                 ConnectorId,
                                                 RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            var request = new UnlockConnectorRequest(ChargeBoxIdentity,
                                                     ConnectorId);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/UnlockConnector",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "UnlockConnector",
                                                     RequestLogDelegate:   OnUnlockConnectorSOAPRequest,
                                                     ResponseLogDelegate:  OnUnlockConnectorSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          UnlockConnectorResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<UnlockConnectorResponse>(httpresponse,
                                                                                                     new UnlockConnectorResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<UnlockConnectorResponse>(httpresponse,
                                                                                                     new UnlockConnectorResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<UnlockConnectorResponse>.ExceptionThrown(new UnlockConnectorResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<UnlockConnectorResponse>.OK(new UnlockConnectorResponse(request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnUnlockConnectorResponse event

            try
            {

                OnUnlockConnectorResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  Timestamp.Value,
                                                  this,
                                                  Description,
                                                  EventTrackingId,
                                                  ChargeBoxIdentity,
                                                  ConnectorId,
                                                  RequestTimeout,
                                                  result.Content,
                                                  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region GetLocalListVersion   (...)

        /// <summary>
        /// Get the version of the local list within the charging station.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<GetLocalListVersionResponse>>

            GetLocalListVersion(DateTime?           Timestamp           = null,
                                CancellationToken?  CancellationToken   = null,
                                EventTracking_Id    EventTrackingId     = null,
                                TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<GetLocalListVersionResponse> result = null;

            #endregion

            #region Send OnGetLocalListVersionRequest event

            try
            {

                OnGetLocalListVersionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     Timestamp.Value,
                                                     this,
                                                     Description,
                                                     EventTrackingId,
                                                     ChargeBoxIdentity,
                                                     RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            var request = new GetLocalListVersionRequest(ChargeBoxIdentity);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/GetLocalListVersion",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "GetLocalListVersion",
                                                     RequestLogDelegate:   OnGetLocalListVersionSOAPRequest,
                                                     ResponseLogDelegate:  OnGetLocalListVersionSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          GetLocalListVersionResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<GetLocalListVersionResponse>(httpresponse,
                                                                                                     new GetLocalListVersionResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<GetLocalListVersionResponse>(httpresponse,
                                                                                                     new GetLocalListVersionResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<GetLocalListVersionResponse>.ExceptionThrown(new GetLocalListVersionResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<GetLocalListVersionResponse>.OK(new GetLocalListVersionResponse(request,
                                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnGetLocalListVersionResponse event

            try
            {

                OnGetLocalListVersionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                      Timestamp.Value,
                                                      this,
                                                      Description,
                                                      EventTrackingId,
                                                      ChargeBoxIdentity,
                                                      RequestTimeout,
                                                      result.Content,
                                                      org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendLocalList         (ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Send a local list to the charging station.
        /// </summary>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<SendLocalListResponse>>

            SendLocalList(UInt64                          ListVersion,
                          UpdateTypes                     UpdateType,
                          IEnumerable<AuthorizationData>  LocalAuthorizationList   = null,

                          DateTime?                       Timestamp                = null,
                          CancellationToken?              CancellationToken        = null,
                          EventTracking_Id                EventTrackingId          = null,
                          TimeSpan?                       RequestTimeout           = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<SendLocalListResponse> result = null;

            #endregion

            #region Send OnSendLocalListRequest event

            try
            {

                OnSendLocalListRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                               Timestamp.Value,
                                               this,
                                               Description,
                                               EventTrackingId,
                                               ChargeBoxIdentity,
                                               ListVersion,
                                               UpdateType,
                                               LocalAuthorizationList,
                                               RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            var request = new SendLocalListRequest(ChargeBoxIdentity,
                                                   ListVersion,
                                                   UpdateType,
                                                   LocalAuthorizationList);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/SendLocalList",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "SendLocalList",
                                                     RequestLogDelegate:   OnSendLocalListSOAPRequest,
                                                     ResponseLogDelegate:  OnSendLocalListSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          SendLocalListResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<SendLocalListResponse>(httpresponse,
                                                                                                     new SendLocalListResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<SendLocalListResponse>(httpresponse,
                                                                                                     new SendLocalListResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<SendLocalListResponse>.ExceptionThrown(new SendLocalListResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<SendLocalListResponse>.OK(new SendLocalListResponse(request,
                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnSendLocalListResponse event

            try
            {

                OnSendLocalListResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                               Timestamp.Value,
                                               this,
                                               Description,
                                               EventTrackingId,
                                               ChargeBoxIdentity,
                                               ListVersion,
                                               UpdateType,
                                               LocalAuthorizationList,
                                               RequestTimeout,
                                               result.Content,
                                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region ClearCache            (...)

        /// <summary>
        /// Clear the cache of the charging station.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ClearCacheResponse>>

            ClearCache(DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<ClearCacheResponse> result = null;

            #endregion

            #region Send OnClearCacheRequest event

            try
            {

                OnClearCacheRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            Timestamp.Value,
                                            this,
                                            Description,
                                            EventTrackingId,
                                            ChargeBoxIdentity,
                                            RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            var request = new ClearCacheRequest(ChargeBoxIdentity);


            try
            {

                using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                        VirtualHostname,
                                                        true,
                                                        null,
                                                        RemoteCertificateValidator,
                                                        ClientCertificateSelector,
                                                        ClientCert,
                                                        HTTPUserAgent,
                                                        URLPathPrefix,
                                                        WSSLoginPassword,
                                                        RequestTimeout,
                                                        TransmissionRetryDelay,
                                                        MaxNumberOfRetries,
                                                        UseHTTPPipelining,
                                                        HTTPLogger,
                                                        DNSClient))
                {

                    result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                        "/ClearCache",
                                                                        NextMessageId(),
                                                                        From,
                                                                        To,
                                                                        request.ToXML()),
                                                     "ClearCache",
                                                     RequestLogDelegate:   OnClearCacheSOAPRequest,
                                                     ResponseLogDelegate:  OnClearCacheSOAPResponse,
                                                     CancellationToken:    CancellationToken,
                                                     EventTrackingId:      EventTrackingId,
                                                     RequestTimeout:       RequestTimeout,

                                                     #region OnSuccess

                                                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                          ClearCacheResponse.Parse),

                                                     #endregion

                                                     #region OnSOAPFault

                                                     OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                         SendSOAPError(timestamp, this, httpresponse.Content);

                                                         return new HTTPResponse<ClearCacheResponse>(httpresponse,
                                                                                                     new ClearCacheResponse(
                                                                                                         request,
                                                                                                         Result.Format(
                                                                                                             "Invalid SOAP => " +
                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnHTTPError

                                                     OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                         SendHTTPError(timestamp, this, httpresponse);

                                                         return new HTTPResponse<ClearCacheResponse>(httpresponse,
                                                                                                     new ClearCacheResponse(
                                                                                                         request,
                                                                                                         Result.Server(
                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                              " => " +
                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                         )
                                                                                                     ),
                                                                                                     IsFault: true);

                                                     },

                                                     #endregion

                                                     #region OnException

                                                     OnException: (timestamp, sender, exception) => {

                                                         SendException(timestamp, sender, exception);

                                                         return HTTPResponse<ClearCacheResponse>.ExceptionThrown(new ClearCacheResponse(
                                                                                                                     request,
                                                                                                                     Result.Format(exception.Message +
                                                                                                                                   " => " +
                                                                                                                                   exception.StackTrace)),
                                                                                                                 exception);

                                                     }

                                                     #endregion

                                                    );

                }

            }
            catch (Exception e)
            {

            }

            if (result == null)
                result = HTTPResponse<ClearCacheResponse>.OK(new ClearCacheResponse(request,
                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnClearCacheResponse event

            try
            {

                OnClearCacheResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             Timestamp.Value,
                                             this,
                                             Description,
                                             EventTrackingId,
                                             ChargeBoxIdentity,
                                             RequestTimeout,
                                             result.Content,
                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return result;

        }

        #endregion

    }

}
