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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system SOAP client runs at the central system
    /// and connects to a charge point to invoke methods.
    /// </summary>
    public partial class CentralSystemSOAPClient : ASOAPClient,
                                                   ICentralSystemSOAPClient
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const           String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.Number + " Central System SOAP Client";

        /// <summary>
        /// The default remote TCP port to connect to.
        /// </summary>
        public new static readonly IPPort  DefaultRemotePort     = IPPort.Parse(443);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id    ChargeBoxIdentity    { get; }

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                 { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                   { get; }

        /// <summary>
        /// The attached OCPP CS client (HTTP/SOAP client) logger.
        /// </summary>
        public CSClientLogger  Logger               { get; }

        #endregion

        #region Events

        #region OnResetRequest/-Response

        /// <summary>
        /// An event fired whenever a reset request will be send to a charge point.
        /// </summary>
        public event OnResetRequestDelegate?     OnResetRequest;

        /// <summary>
        /// An event fired whenever a reset SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?    OnResetSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a reset SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?   OnResetSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a reset request was received.
        /// </summary>
        public event OnResetResponseDelegate?    OnResetResponse;

        #endregion

        #region OnChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event fired whenever a change availability request will be send to a charge point.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?    OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a change availability SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                OnChangeAvailabilitySOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a change availability SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?               OnChangeAvailabilitySOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?   OnChangeAvailabilityResponse;

        #endregion

        #region OnGetConfigurationRequest/-Response

        /// <summary>
        /// An event fired whenever a get configuration request will be send to a charge point.
        /// </summary>
        public event OnGetConfigurationRequestDelegate?    OnGetConfigurationRequest;

        /// <summary>
        /// An event fired whenever a get configuration SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?              OnGetConfigurationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get configuration SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnGetConfigurationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get configuration request was received.
        /// </summary>
        public event OnGetConfigurationResponseDelegate?   OnGetConfigurationResponse;

        #endregion

        #region OnChangeConfigurationRequest/-Response

        /// <summary>
        /// An event fired whenever a change configuration request will be send to a charge point.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate?    OnChangeConfigurationRequest;

        /// <summary>
        /// An event fired whenever a change configuration SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                 OnChangeConfigurationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a change configuration SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnChangeConfigurationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a change configuration request was received.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate?   OnChangeConfigurationResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to a charge point.
        /// </summary>
        public event OnDataTransferRequestDelegate?    OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?          OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?   OnDataTransferResponse;

        #endregion

        #region OnGetDiagnosticsRequest/-Response

        /// <summary>
        /// An event fired whenever a get diagnostics request will be send to a charge point.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate?    OnGetDiagnosticsRequest;

        /// <summary>
        /// An event fired whenever a get diagnostics SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?            OnGetDiagnosticsSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get diagnostics SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?           OnGetDiagnosticsSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get diagnostics request was received.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate?   OnGetDiagnosticsResponse;

        #endregion

        #region OnTriggerMessageRequest/-Response

        /// <summary>
        /// An event fired whenever a trigger message request will be send to a charge point.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?    OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a trigger message SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?            OnTriggerMessageSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a trigger message SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?           OnTriggerMessageSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?   OnTriggerMessageResponse;

        #endregion

        #region OnUpdateFirmwareRequest/-Response

        /// <summary>
        /// An event fired whenever a update firmware request will be send to a charge point.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?    OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a update firmware SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?            OnUpdateFirmwareSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a update firmware SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?           OnUpdateFirmwareSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?   OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event fired whenever a reserve now request will be send to a charge point.
        /// </summary>
        public event OnReserveNowRequestDelegate?    OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a reserve now SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?        OnReserveNowSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a reserve now SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnReserveNowSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a reserve now request was received.
        /// </summary>
        public event OnReserveNowResponseDelegate?   OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event fired whenever a cancel reservation request will be send to a charge point.
        /// </summary>
        public event OnCancelReservationRequestDelegate?    OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a cancel reservation SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?               OnCancelReservationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnCancelReservationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?   OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote start transaction request will be send to a charge point.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate?    OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote start transaction SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                    OnRemoteStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnRemoteStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction request was received.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate?   OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote stop transaction request will be send to a charge point.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate?    OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote stop transaction SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                   OnRemoteStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnRemoteStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate?   OnRemoteStopTransactionResponse;

        #endregion

        #region OnSetChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a set charging profile request will be send to a charge point.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?    OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a set charging profile SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                OnSetChargingProfileSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a set charging profile SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?               OnSetChargingProfileSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?   OnSetChargingProfileResponse;

        #endregion

        #region OnClearChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a clear charging profile request will be send to a charge point.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?    OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a clear charging profile SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                  OnClearChargingProfileSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a clear charging profile SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                 OnClearChargingProfileSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?   OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event fired whenever a get composite schedule request will be send to a charge point.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?    OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a get composite schedule SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                  OnGetCompositeScheduleSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get composite schedule SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                 OnGetCompositeScheduleSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get composite schedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?   OnGetCompositeScheduleResponse;

        #endregion

        #region OnUnlockConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a unlock connector request will be send to a charge point.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?    OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a unlock connector SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?             OnUnlockConnectorSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a unlock connector SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?            OnUnlockConnectorSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a unlock connector request was received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?   OnUnlockConnectorResponse;

        #endregion


        #region OnGetLocalListVersionRequest/-Response

        /// <summary>
        /// An event fired whenever a get local list version request will be send to a charge point.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?    OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a get local list version SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?                 OnGetLocalListVersionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a get local list version SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnGetLocalListVersionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a get local list version request was received.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?   OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalListRequest/-Response

        /// <summary>
        /// An event fired whenever a send local list request will be send to a charge point.
        /// </summary>
        public event OnSendLocalListRequestDelegate?    OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a send local list SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?           OnSendLocalListSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a send local list SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnSendLocalListSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a send local list request was received.
        /// </summary>
        public event OnSendLocalListResponseDelegate?   OnSendLocalListResponse;

        #endregion

        #region OnClearCacheRequest/-Response

        /// <summary>
        /// An event fired whenever a clear cache request will be send to a charge point.
        /// </summary>
        public event OnClearCacheRequestDelegate?    OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a clear cache SOAP request will be send to a charge point.
        /// </summary>
        public event ClientRequestLogHandler?        OnClearCacheSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a clear cache SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnClearCacheSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a clear cache request was received.
        /// </summary>
        public event OnClearCacheResponseDelegate?   OnClearCacheResponse;

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
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CentralSystemSOAPClient(ChargeBox_Id                          ChargeBoxIdentity,
                                       String                                From,
                                       String                                To,

                                       URL                                   RemoteURL,
                                       HTTPHostname?                         VirtualHostname              = null,
                                       String?                               Description                  = null,
                                       RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                       X509Certificate?                      ClientCert                   = null,
                                       SslProtocols?                         TLSProtocol                  = null,
                                       Boolean?                              PreferIPv4                   = null,
                                       String                                HTTPUserAgent                = DefaultHTTPUserAgent,
                                       HTTPPath?                             URLPathPrefix                = null,
                                       Tuple<String, String>?                WSSLoginPassword             = null,
                                       HTTPContentType?                      HTTPContentType              = null,
                                       TimeSpan?                             RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                       UInt16?                               MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
                                       Boolean                               UseHTTPPipelining            = false,

                                       String?                               LoggingPath                  =  null,
                                       String                                LoggingContext               = CSClientLogger.DefaultContext,
                                       LogfileCreatorDelegate?               LogfileCreator               = null,
                                       HTTPClientLogger?                     HTTPLogger                   = null,
                                       DNSClient?                            DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   PreferIPv4,
                   HTTPUserAgent,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   WSSLoginPassword,
                   HTTPContentType,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   UseHTTPPipelining,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From), "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),   "The given SOAP message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity  = ChargeBoxIdentity;
            this.From               = From;
            this.To                 = To;

            this.Logger             = new CSClientLogger(this,
                                                         LoggingPath,
                                                         LoggingContext,
                                                         LogfileCreator);

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


        private static String NextMessageId()
            => Guid.NewGuid().ToString();


        // When a Central System needs to send requests to a Charge Point, the Central System MUST specify in
        // each request the “chargeBoxIdentity” of the Charge Point for which the request is intended.If the
        // receiving Charge Point is not the intended one, and it cannot relay the message to a node that knows
        // this Charge Point, then the Charge Point MUST send a SOAP Fault Response message, indicating that the
        // identity is wrong(e.g.sub-code is “IdentityMismatch”).

        #region Reset                 (Request)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        public async Task<ResetResponse>

            Reset(ResetRequest Request)

        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnResetRequest));
            }

            #endregion


            HTTPResponse<ResetResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnResetSOAPRequest,
                                                    ResponseLogDelegate:  OnResetSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         ResetResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<ResetResponse>(httpresponse,
                                                               new ResetResponse(
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

                        return new HTTPResponse<ResetResponse>(httpresponse,
                                                               new ResetResponse(
                                                                   Request,
                                                                   Result.Server(
                                                                        httpresponse.HTTPStatusCode.ToString() +
                                                                        " => " +
                                                                        httpresponse.HTTPBody.ToUTF8String()
                                                                   )
                                                               ),
                                                               IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) =>
                    {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<ResetResponse>.ExceptionThrown(new ResetResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(Reset));
            }

            result ??= HTTPResponse<ResetResponse>.OK(new ResetResponse(Request,
                                                                        Result.OK("Nothing to upload!")));


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        result.Content,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnResetResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region ChangeAvailability    (Request)

        /// <summary>
        /// Change the availability of the given charge box connector.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        public async Task<ChangeAvailabilityResponse>

            ChangeAvailability(ChangeAvailabilityRequest Request)

        {

            #region Send OnChangeAvailabilityRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            HTTPResponse<ChangeAvailabilityResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnChangeAvailabilitySOAPRequest,
                                                    ResponseLogDelegate:  OnChangeAvailabilitySOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         ChangeAvailabilityResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<ChangeAvailabilityResponse>(httpresponse,
                                                                            new ChangeAvailabilityResponse(
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

                        return new HTTPResponse<ChangeAvailabilityResponse>(httpresponse,
                                                                            new ChangeAvailabilityResponse(
                                                                                Request,
                                                                                Result.Server(
                                                                                     httpresponse.HTTPStatusCode.ToString() +
                                                                                     " => " +
                                                                                     httpresponse.HTTPBody.ToUTF8String()
                                                                                )
                                                                            ),
                                                                            IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<ChangeAvailabilityResponse>.ExceptionThrown(new ChangeAvailabilityResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(ChangeAvailability));
            }

            result ??= HTTPResponse<ChangeAvailabilityResponse>.OK(new ChangeAvailabilityResponse(Request,
                                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     result.Content,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region GetConfiguration      (Request)

        /// <summary>
        /// Get the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A get configuration request.</param>
        public async Task<GetConfigurationResponse>

            GetConfiguration(GetConfigurationRequest Request)

        {

            #region Send OnGetConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetConfigurationRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            HTTPResponse<GetConfigurationResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnGetConfigurationSOAPRequest,
                                                    ResponseLogDelegate:  OnGetConfigurationSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         GetConfigurationResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<GetConfigurationResponse>(httpresponse,
                                                                          new GetConfigurationResponse(
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

                        return new HTTPResponse<GetConfigurationResponse>(httpresponse,
                                                                          new GetConfigurationResponse(
                                                                              Request,
                                                                              Result.Server(
                                                                                   httpresponse.HTTPStatusCode.ToString() +
                                                                                   " => " +
                                                                                   httpresponse.HTTPBody.ToUTF8String()
                                                                              )
                                                                          ),
                                                                          IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<GetConfigurationResponse>.ExceptionThrown(new GetConfigurationResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(GetConfiguration));
            }

            result ??= HTTPResponse<GetConfigurationResponse>.OK(new GetConfigurationResponse(Request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnGetConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetConfigurationResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   result.Content,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetConfigurationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region ChangeConfiguration   (Request)

        /// <summary>
        /// Change the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A change configuration request.</param>
        public async Task<ChangeConfigurationResponse>

            ChangeConfiguration(ChangeConfigurationRequest Request)

        {

            #region Send OnChangeConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            HTTPResponse<ChangeConfigurationResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnChangeConfigurationSOAPRequest,
                                                    ResponseLogDelegate:  OnChangeConfigurationSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         ChangeConfigurationResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<ChangeConfigurationResponse>(httpresponse,
                                                                             new ChangeConfigurationResponse(
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

                        return new HTTPResponse<ChangeConfigurationResponse>(httpresponse,
                                                                             new ChangeConfigurationResponse(
                                                                                 Request,
                                                                                 Result.Server(
                                                                                      httpresponse.HTTPStatusCode.ToString() +
                                                                                      " => " +
                                                                                      httpresponse.HTTPBody.ToUTF8String()
                                                                                 )
                                                                             ),
                                                                             IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<ChangeConfigurationResponse>.ExceptionThrown(new ChangeConfigurationResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(ChangeConfiguration));
            }

            result ??= HTTPResponse<ChangeConfigurationResponse>.OK(new ChangeConfigurationResponse(Request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnChangeConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      result.Content,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region DataTransfer          (Request)

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public async Task<CP.DataTransferResponse>

            DataTransfer(DataTransferRequest Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            HTTPResponse<CP.DataTransferResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    "DataTransfer",
                                                    RequestLogDelegate:   OnDataTransferSOAPRequest,
                                                    ResponseLogDelegate:  OnDataTransferSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         CP.DataTransferResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<CP.DataTransferResponse>(httpresponse,
                                                                         new CP.DataTransferResponse(
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

                        return new HTTPResponse<CP.DataTransferResponse>(httpresponse,
                                                                         new CP.DataTransferResponse(
                                                                             Request,
                                                                             Result.Server(
                                                                                  httpresponse.HTTPStatusCode.ToString() +
                                                                                  " => " +
                                                                                  httpresponse.HTTPBody.ToUTF8String()
                                                                             )
                                                                         ),
                                                                         IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<CP.DataTransferResponse>.ExceptionThrown(new CP.DataTransferResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(DataTransfer));
            }

            result ??= HTTPResponse<CP.DataTransferResponse>.OK(new CP.DataTransferResponse(Request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               result.Content,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region GetDiagnostics        (Request)

        /// <summary>
        /// Upload diagnostics data of the given charge box to the given file location.
        /// </summary>
        /// <param name="Request">A get diagnostics request.</param>
        public async Task<GetDiagnosticsResponse>

            GetDiagnostics(GetDiagnosticsRequest Request)

        {

            #region Send OnGetDiagnosticsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsRequest?.Invoke(startTime,
                                                this,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            HTTPResponse<GetDiagnosticsResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnGetDiagnosticsSOAPRequest,
                                                    ResponseLogDelegate:  OnGetDiagnosticsSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         GetDiagnosticsResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<GetDiagnosticsResponse>(httpresponse,
                                                                        new GetDiagnosticsResponse(
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

                        return new HTTPResponse<GetDiagnosticsResponse>(httpresponse,
                                                                        new GetDiagnosticsResponse(
                                                                            Request,
                                                                            Result.Server(
                                                                                 httpresponse.HTTPStatusCode.ToString() +
                                                                                 " => " +
                                                                                 httpresponse.HTTPBody.ToUTF8String()
                                                                            )
                                                                        ),
                                                                        IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<GetDiagnosticsResponse>.ExceptionThrown(new GetDiagnosticsResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(GetDiagnostics));
            }

            result ??= HTTPResponse<GetDiagnosticsResponse>.OK(new GetDiagnosticsResponse(Request,
                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnGetDiagnosticsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 result.Content,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetDiagnosticsResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region TriggerMessage        (Request)

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        public async Task<TriggerMessageResponse>

            TriggerMessage(TriggerMessageRequest Request)

        {

            #region Send OnTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageRequest?.Invoke(startTime,
                                                this,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            HTTPResponse<TriggerMessageResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnTriggerMessageSOAPRequest,
                                                    ResponseLogDelegate:  OnTriggerMessageSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         TriggerMessageResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<TriggerMessageResponse>(httpresponse,
                                                                        new TriggerMessageResponse(
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

                        return new HTTPResponse<TriggerMessageResponse>(httpresponse,
                                                                        new TriggerMessageResponse(
                                                                            Request,
                                                                            Result.Server(
                                                                                 httpresponse.HTTPStatusCode.ToString() +
                                                                                 " => " +
                                                                                 httpresponse.HTTPBody.ToUTF8String()
                                                                            )
                                                                        ),
                                                                        IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<TriggerMessageResponse>.ExceptionThrown(new TriggerMessageResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(TriggerMessage));
            }

            result ??= HTTPResponse<TriggerMessageResponse>.OK(new TriggerMessageResponse(Request,
                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 result.Content,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region UpdateFirmware        (Request)

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        public async Task<UpdateFirmwareResponse>

            UpdateFirmware(UpdateFirmwareRequest Request)

        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                                this,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            HTTPResponse<UpdateFirmwareResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnUpdateFirmwareSOAPRequest,
                                                    ResponseLogDelegate:  OnUpdateFirmwareSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         UpdateFirmwareResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<UpdateFirmwareResponse>(httpresponse,
                                                                        new UpdateFirmwareResponse(
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

                        return new HTTPResponse<UpdateFirmwareResponse>(httpresponse,
                                                                        new UpdateFirmwareResponse(
                                                                            Request,
                                                                            Result.Server(
                                                                                 httpresponse.HTTPStatusCode.ToString() +
                                                                                 " => " +
                                                                                 httpresponse.HTTPBody.ToUTF8String()
                                                                            )
                                                                        ),
                                                                        IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<UpdateFirmwareResponse>.ExceptionThrown(new UpdateFirmwareResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(UpdateFirmware));
            }

            result ??= HTTPResponse<UpdateFirmwareResponse>.OK(new UpdateFirmwareResponse(Request,
                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 result.Content,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        #region ReserveNow            (Request)

        /// <summary>
        /// Create a charging reservation at the given charge box connector.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        public async Task<ReserveNowResponse>

            ReserveNow(ReserveNowRequest Request)

        {

            #region Send OnReserveNowRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowRequest?.Invoke(startTime,
                                            this,
                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            HTTPResponse<ReserveNowResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnReserveNowSOAPRequest,
                                                    ResponseLogDelegate:  OnReserveNowSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

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
                                                                             httpresponse.HTTPBody.ToUTF8String()
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(ReserveNow));
            }

            result ??= HTTPResponse<ReserveNowResponse>.OK(new ReserveNowResponse(Request,
                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             result.Content,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region CancelReservation     (Request)

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        public async Task<CancelReservationResponse>

            CancelReservation(CancelReservationRequest Request)

        {

            #region Send OnCancelReservationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationRequest?.Invoke(startTime,
                                                   this,
                                                   Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            HTTPResponse<CancelReservationResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnCancelReservationSOAPRequest,
                                                    ResponseLogDelegate:  OnCancelReservationSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         CancelReservationResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<CancelReservationResponse>(httpresponse,
                                                                           new CancelReservationResponse(
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

                        return new HTTPResponse<CancelReservationResponse>(httpresponse,
                                                                           new CancelReservationResponse(
                                                                               Request,
                                                                               Result.Server(
                                                                                    httpresponse.HTTPStatusCode.ToString() +
                                                                                    " => " +
                                                                                    httpresponse.HTTPBody.ToUTF8String()
                                                                               )
                                                                           ),
                                                                           IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<CancelReservationResponse>.ExceptionThrown(new CancelReservationResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(CancelReservation));
            }

            result ??= HTTPResponse<CancelReservationResponse>.OK(new CancelReservationResponse(Request,
                                                                                                Result.OK("Nothing to upload!")));


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    result.Content,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region RemoteStartTransaction(Request)

        /// <summary>
        /// Start a charging session at the given charge box connector using the given charging profile.
        /// </summary>
        /// <param name="Request">A remote start transaction request.</param>
        public async Task<RemoteStartTransactionResponse>

            RemoteStartTransaction(RemoteStartTransactionRequest Request)

        {

            #region Send OnRemoteStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            HTTPResponse<RemoteStartTransactionResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnRemoteStartTransactionSOAPRequest,
                                                    ResponseLogDelegate:  OnRemoteStartTransactionSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         RemoteStartTransactionResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<RemoteStartTransactionResponse>(httpresponse,
                                                                                new RemoteStartTransactionResponse(
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

                        return new HTTPResponse<RemoteStartTransactionResponse>(httpresponse,
                                                                                new RemoteStartTransactionResponse(
                                                                                    Request,
                                                                                    Result.Server(
                                                                                         httpresponse.HTTPStatusCode.ToString() +
                                                                                         " => " +
                                                                                         httpresponse.HTTPBody.ToUTF8String()
                                                                                    )
                                                                                ),
                                                                                IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<RemoteStartTransactionResponse>.ExceptionThrown(new RemoteStartTransactionResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(RemoteStartTransaction));
            }

            result ??= HTTPResponse<RemoteStartTransactionResponse>.OK(new RemoteStartTransactionResponse(Request,
                                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnRemoteStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         result.Content,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region RemoteStopTransaction (Request)

        /// <summary>
        /// Stop a charging session at the given charge box.
        /// </summary>
        /// <param name="Request">A remote stop transaction request.</param>
        public async Task<RemoteStopTransactionResponse>

            RemoteStopTransaction(RemoteStopTransactionRequest Request)

        {

            #region Send OnRemoteStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            HTTPResponse<RemoteStopTransactionResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnRemoteStopTransactionSOAPRequest,
                                                    ResponseLogDelegate:  OnRemoteStopTransactionSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         RemoteStopTransactionResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<RemoteStopTransactionResponse>(httpresponse,
                                                                               new RemoteStopTransactionResponse(
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

                        return new HTTPResponse<RemoteStopTransactionResponse>(httpresponse,
                                                                               new RemoteStopTransactionResponse(
                                                                                   Request,
                                                                                   Result.Server(
                                                                                        httpresponse.HTTPStatusCode.ToString() +
                                                                                        " => " +
                                                                                        httpresponse.HTTPBody.ToUTF8String()
                                                                                   )
                                                                               ),
                                                                               IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<RemoteStopTransactionResponse>.ExceptionThrown(new RemoteStopTransactionResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(RemoteStopTransaction));
            }

            result ??= HTTPResponse<RemoteStopTransactionResponse>.OK(new RemoteStopTransactionResponse(Request,
                                                                                                        Result.OK("Nothing to upload!")));


            #region Send OnRemoteStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        result.Content,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnRemoteStopTransactionResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SetChargingProfile    (Request)

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        public async Task<SetChargingProfileResponse>

            SetChargingProfile(SetChargingProfileRequest Request)

        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            HTTPResponse<SetChargingProfileResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnSetChargingProfileSOAPRequest,
                                                    ResponseLogDelegate:  OnSetChargingProfileSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         SetChargingProfileResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<SetChargingProfileResponse>(httpresponse,
                                                                            new SetChargingProfileResponse(
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

                        return new HTTPResponse<SetChargingProfileResponse>(httpresponse,
                                                                            new SetChargingProfileResponse(
                                                                                Request,
                                                                                Result.Server(
                                                                                     httpresponse.HTTPStatusCode.ToString() +
                                                                                     " => " +
                                                                                     httpresponse.HTTPBody.ToUTF8String()
                                                                                )
                                                                            ),
                                                                            IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<SetChargingProfileResponse>.ExceptionThrown(new SetChargingProfileResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(SetChargingProfile));
            }

            result ??= HTTPResponse<SetChargingProfileResponse>.OK(new SetChargingProfileResponse(Request,
                                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     result.Content,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region ClearChargingProfile  (Request)

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        public async Task<ClearChargingProfileResponse>

            ClearChargingProfile(ClearChargingProfileRequest Request)

        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            HTTPResponse<ClearChargingProfileResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnClearChargingProfileSOAPRequest,
                                                    ResponseLogDelegate:  OnClearChargingProfileSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         ClearChargingProfileResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<ClearChargingProfileResponse>(httpresponse,
                                                                              new ClearChargingProfileResponse(
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

                        return new HTTPResponse<ClearChargingProfileResponse>(httpresponse,
                                                                              new ClearChargingProfileResponse(
                                                                                  Request,
                                                                                  Result.Server(
                                                                                       httpresponse.HTTPStatusCode.ToString() +
                                                                                       " => " +
                                                                                       httpresponse.HTTPBody.ToUTF8String()
                                                                                  )
                                                                              ),
                                                                              IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<ClearChargingProfileResponse>.ExceptionThrown(new ClearChargingProfileResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(ClearChargingProfile));
            }

            result ??= HTTPResponse<ClearChargingProfileResponse>.OK(new ClearChargingProfileResponse(Request,
                                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       result.Content,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region GetCompositeSchedule  (Request)

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        public async Task<GetCompositeScheduleResponse>

            GetCompositeSchedule(GetCompositeScheduleRequest Request)

        {

            #region Send OnGetCompositeScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            HTTPResponse<GetCompositeScheduleResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnGetCompositeScheduleSOAPRequest,
                                                    ResponseLogDelegate:  OnGetCompositeScheduleSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         GetCompositeScheduleResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<GetCompositeScheduleResponse>(httpresponse,
                                                                              new GetCompositeScheduleResponse(
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

                        return new HTTPResponse<GetCompositeScheduleResponse>(httpresponse,
                                                                              new GetCompositeScheduleResponse(
                                                                                  Request,
                                                                                  Result.Server(
                                                                                       httpresponse.HTTPStatusCode.ToString() +
                                                                                       " => " +
                                                                                       httpresponse.HTTPBody.ToUTF8String()
                                                                                  )
                                                                              ),
                                                                              IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<GetCompositeScheduleResponse>.ExceptionThrown(new GetCompositeScheduleResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(GetCompositeSchedule));
            }

            result ??= HTTPResponse<GetCompositeScheduleResponse>.OK(new GetCompositeScheduleResponse(Request,
                                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       result.Content,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region UnlockConnector       (Request)

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        public async Task<UnlockConnectorResponse>

            UnlockConnector(UnlockConnectorRequest Request)

        {

            #region Send OnUnlockConnectorRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            HTTPResponse<UnlockConnectorResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnUnlockConnectorSOAPRequest,
                                                    ResponseLogDelegate:  OnUnlockConnectorSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         UnlockConnectorResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<UnlockConnectorResponse>(httpresponse,
                                                                         new UnlockConnectorResponse(
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

                        return new HTTPResponse<UnlockConnectorResponse>(httpresponse,
                                                                         new UnlockConnectorResponse(
                                                                             Request,
                                                                             Result.Server(
                                                                                  httpresponse.HTTPStatusCode.ToString() +
                                                                                  " => " +
                                                                                  httpresponse.HTTPBody.ToUTF8String()
                                                                             )
                                                                         ),
                                                                         IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<UnlockConnectorResponse>.ExceptionThrown(new UnlockConnectorResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(UnlockConnector));
            }

            result ??= HTTPResponse<UnlockConnectorResponse>.OK(new UnlockConnectorResponse(Request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  result.Content,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        #region GetLocalListVersion   (Request)

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        public async Task<GetLocalListVersionResponse>

            GetLocalListVersion(GetLocalListVersionRequest Request)

        {

            #region Send OnGetLocalListVersionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            HTTPResponse<GetLocalListVersionResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnGetLocalListVersionSOAPRequest,
                                                    ResponseLogDelegate:  OnGetLocalListVersionSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         GetLocalListVersionResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<GetLocalListVersionResponse>(httpresponse,
                                                                             new GetLocalListVersionResponse(
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

                        return new HTTPResponse<GetLocalListVersionResponse>(httpresponse,
                                                                             new GetLocalListVersionResponse(
                                                                                 Request,
                                                                                 Result.Server(
                                                                                      httpresponse.HTTPStatusCode.ToString() +
                                                                                      " => " +
                                                                                      httpresponse.HTTPBody.ToUTF8String()
                                                                                 )
                                                                             ),
                                                                             IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<GetLocalListVersionResponse>.ExceptionThrown(new GetLocalListVersionResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(GetLocalListVersion));
            }

            result ??= HTTPResponse<GetLocalListVersionResponse>.OK(new GetLocalListVersionResponse(Request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      result.Content,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendLocalList         (Request)

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        public async Task<SendLocalListResponse>

            SendLocalList(SendLocalListRequest Request)

        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                               this,
                                               Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            HTTPResponse<SendLocalListResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnSendLocalListSOAPRequest,
                                                    ResponseLogDelegate:  OnSendLocalListSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                     OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                          SendLocalListResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<SendLocalListResponse>(httpresponse,
                                                                       new SendLocalListResponse(
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

                        return new HTTPResponse<SendLocalListResponse>(httpresponse,
                                                                       new SendLocalListResponse(
                                                                           Request,
                                                                           Result.Server(
                                                                                httpresponse.HTTPStatusCode.ToString() +
                                                                                " => " +
                                                                                httpresponse.HTTPBody.ToUTF8String()
                                                                           )
                                                                       ),
                                                                       IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<SendLocalListResponse>.ExceptionThrown(new SendLocalListResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(SendLocalList));
            }

            result ??= HTTPResponse<SendLocalListResponse>.OK(new SendLocalListResponse(Request,
                                                                                        Result.OK("Nothing to upload!")));


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                result.Content,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region ClearCache            (Request)

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        public async Task<ClearCacheResponse>

            ClearCache(ClearCacheRequest Request)

        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                            this,
                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            HTTPResponse<ClearCacheResponse>? result = null;

            try
            {

                using (var soapClient = new SOAPClient(RemoteURL,
                                                       VirtualHostname,
                                                       true,
                                                       null,
                                                       RemoteCertificateValidator,
                                                       ClientCertificateSelector,
                                                       ClientCert,
                                                       TLSProtocol,
                                                       PreferIPv4,
                                                       HTTPUserAgent,
                                                       URLPathPrefix,
                                                       WSSLoginPassword,
                                                       HTTPContentType,
                                                       RequestTimeout,
                                                       TransmissionRetryDelay,
                                                       MaxNumberOfRetries,
                                                       UseHTTPPipelining,
                                                       HTTPLogger,
                                                       DNSClient))
                {

                    result = await soapClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                       "/" + Request.Action,
                                                                       NextMessageId(),
                                                                       From,
                                                                       To,
                                                                       Request.ToXML()),
                                                    Request.Action,
                                                    RequestLogDelegate:   OnClearCacheSOAPRequest,
                                                    ResponseLogDelegate:  OnClearCacheSOAPResponse,
                                                    CancellationToken:    Request.CancellationToken,
                                                    EventTrackingId:      Request.EventTrackingId,
                                                    RequestTimeout:       Request.RequestTimeout,

                    #region OnSuccess

                    OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                         ClearCacheResponse.Parse),

                    #endregion

                    #region OnSOAPFault

                    OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                        SendSOAPError(timestamp, this, httpresponse.Content);

                        return new HTTPResponse<ClearCacheResponse>(httpresponse,
                                                                    new ClearCacheResponse(
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

                        return new HTTPResponse<ClearCacheResponse>(httpresponse,
                                                                    new ClearCacheResponse(
                                                                        Request,
                                                                        Result.Server(
                                                                             httpresponse.HTTPStatusCode.ToString() +
                                                                             " => " +
                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                        )
                                                                    ),
                                                                    IsFault: true);

                    },

                    #endregion

                    #region OnException

                    OnException: (timestamp, sender, exception) => {

                        SendException(timestamp, sender, exception);

                        return HTTPResponse<ClearCacheResponse>.ExceptionThrown(new ClearCacheResponse(
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
                DebugX.LogException(e, nameof(CentralSystemSOAPClient) + "." + nameof(ClearCache));
            }

            result ??= HTTPResponse<ClearCacheResponse>.OK(new ClearCacheResponse(Request,
                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             result.Content,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemSOAPClient) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        #region Security extensions are not defined for SOAP!

        public Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetLogResponse> GetLog(GetLogRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request)
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}
