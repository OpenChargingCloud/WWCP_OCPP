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

using cloud.charging.open.protocols.OCPPv1_6.CS;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charge point SOAP client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargePointSOAPClient : ASOAPClient,
                                                 ICPClient
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
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                  { get; }


        /// <summary>
        /// The attached OCPP CP client (HTTP/SOAP client) logger.
        /// </summary>
        public CPClientLogger  Logger              { get; }

        #endregion

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction request will be send to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler               OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler              OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler        OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler       OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction request will be send to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler            OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP request will be send to the central system.
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

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification request will be send to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                          OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification SOAP request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                       OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification SOAP request was received.
        /// </summary>
        public event ClientResponseLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region ChargePointSOAPClient(ChargeBoxIdentity, Hostname, ..., LoggingContext = CPClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new charge point SOAP client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
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
        public ChargePointSOAPClient(ChargeBox_Id                         ChargeBoxIdentity,
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
                                     String                               LoggingContext               = CPClientLogger.DefaultContext,
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

            this.Logger             = new CPClientLogger(this,
                                                         LoggingContext,
                                                         LogFileCreator);

        }

        #endregion

        #region ChargePointSOAPClient(ChargeBoxIdentity, Logger, Hostname, ...)

        /// <summary>
        /// Create a new charge point SOAP client.
        /// </summary>
        /// <param name="ChargeBoxIdentity">A unqiue identification of this client.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="Hostname">The OCPP hostname to connect to.</param>
        /// <param name="RemotePort">An optional OCPP TCP port to connect to.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URLPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        //public ChargePointSOAPClient(String                               ChargeBoxIdentity,
        //                             String                               From,
        //                             String                               To,

        //                             CPClientLogger                       Logger,
        //                             HTTPHostname                         Hostname,
        //                             IPPort?                              RemotePort                   = null,
        //                             RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
        //                             LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
        //                             HTTPHostname?                        HTTPVirtualHost              = null,
        //                             HTTPPath?                            URLPrefix                    = null,
        //                             String                               HTTPUserAgent                = DefaultHTTPUserAgent,
        //                             TimeSpan?                            RequestTimeout               = null,
        //                             Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
        //                             DNSClient                            DNSClient                    = null)

        //    : base(ChargeBoxIdentity,
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

        //    if (ChargeBoxIdentity.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

        //    if (From.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

        //    if (To.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

        //    #endregion

        //    this.From    = From;
        //    this.To      = To;

        //    this.Logger  = Logger ?? throw new ArgumentNullException(nameof(Hostname), "The given hostname must not be null or empty!");

        //}

        #endregion

        #endregion


        // <cs:chargeBoxIdentity se:mustUnderstand="true">CP1234</cs:chargeBoxIdentity>


        #region SendBootNotification             (Request, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<BootNotificationResponse>>

            SendBootNotification(BootNotificationRequest  Request,

                                 DateTime?                Timestamp                = null,
                                 CancellationToken?       CancellationToken        = null,
                                 EventTracking_Id         EventTrackingId          = null,
                                 TimeSpan?                RequestTimeout           = null)

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


            HTTPResponse<BootNotificationResponse> result = null;

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  Timestamp.Value,
                                                  this,
                                                  Description,
                                                  EventTrackingId,
                                                  Request,
                                                  RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


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
                                                                    "/BootNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "BootNotification",
                                                 RequestLogDelegate:   OnBootNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnBootNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, BootNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<BootNotificationResponse>(httpresponse,
                                                                                                       new BootNotificationResponse(
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

                                                     return new HTTPResponse<BootNotificationResponse>(httpresponse,
                                                                                                       new BootNotificationResponse(
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

                                                     return HTTPResponse<BootNotificationResponse>.ExceptionThrown(new BootNotificationResponse(
                                                                                                                       Request,
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<BootNotificationResponse>.OK(new BootNotificationResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   Description,
                                                   EventTrackingId,
                                                   Request,
                                                   RequestTimeout,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region Heartbeat                    (...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<HeartbeatResponse>>

            Heartbeat(DateTime?           Timestamp          = null,
                      CancellationToken?  CancellationToken  = null,
                      EventTracking_Id    EventTrackingId    = null,
                      TimeSpan?           RequestTimeout     = null)

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


            HTTPResponse<HeartbeatResponse> result = null;

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           Timestamp.Value,
                                           this,
                                           Description,
                                           EventTrackingId,
                                           RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var request = new HeartbeatRequest(ChargeBoxIdentity);


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
                                                                    "/Heartbeat",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "Heartbeat",
                                                 RequestLogDelegate:   OnHeartbeatSOAPRequest,
                                                 ResponseLogDelegate:  OnHeartbeatSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request, HeartbeatResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<HeartbeatResponse>(httpresponse,
                                                                                                new HeartbeatResponse(
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

                                                     return new HTTPResponse<HeartbeatResponse>(httpresponse,
                                                                                                new HeartbeatResponse(
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

                                                     return HTTPResponse<HeartbeatResponse>.ExceptionThrown(new HeartbeatResponse(
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
                result = HTTPResponse<HeartbeatResponse>.OK(new HeartbeatResponse(request, Result.OK("Nothing to upload!")));


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            Timestamp.Value,
                                            this,
                                            Description,
                                            EventTrackingId,
                                            RequestTimeout,
                                            result.Content,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region Authorize                    (IdTag, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<AuthorizeResponse>>

            Authorize(IdToken             IdTag,

                      DateTime?           Timestamp          = null,
                      CancellationToken?  CancellationToken  = null,
                      EventTracking_Id    EventTrackingId    = null,
                      TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (IdTag == null)
                throw new ArgumentNullException(nameof(IdTag),  "The given identification tag info must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<AuthorizeResponse> result = null;

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           Timestamp.Value,
                                           this,
                                           Description,
                                           EventTrackingId,
                                           IdTag,
                                           RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var request = new AuthorizeRequest(ChargeBoxIdentity,
                                               IdTag);


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
                                                                    "/Authorize",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "Authorize",
                                                 RequestLogDelegate:   OnAuthorizeSOAPRequest,
                                                 ResponseLogDelegate:  OnAuthorizeSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      AuthorizeResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
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

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
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

                                                     return HTTPResponse<AuthorizeResponse>.ExceptionThrown(new AuthorizeResponse(
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
                result = HTTPResponse<AuthorizeResponse>.OK(new AuthorizeResponse(request, Result.OK("Nothing to upload!")));


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            Timestamp.Value,
                                            this,
                                            Description,
                                            EventTrackingId,
                                            IdTag,
                                            RequestTimeout,
                                            result.Content,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region StartTransaction             (ConnectorId, IdTag, TransactionTimestamp, MeterStart, ReservationId = null, ...)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="TransactionTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StartTransactionResponse>>

            StartTransaction(Connector_Id        ConnectorId,
                             IdToken             IdTag,
                             DateTime            TransactionTimestamp,
                             UInt64              MeterStart,
                             Reservation_Id?     ReservationId      = null,

                             DateTime?           Timestamp          = null,
                             CancellationToken?  CancellationToken  = null,
                             EventTracking_Id    EventTrackingId    = null,
                             TimeSpan?           RequestTimeout     = null)

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


            HTTPResponse<StartTransactionResponse> result = null;

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  Timestamp.Value,
                                                  this,
                                                  Description,
                                                  EventTrackingId,
                                                  ConnectorId,
                                                  IdTag,
                                                  TransactionTimestamp,
                                                  MeterStart,
                                                  ReservationId,
                                                  RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            var request = new StartTransactionRequest(ChargeBoxIdentity,
                                                      ConnectorId,
                                                      IdTag,
                                                      TransactionTimestamp,
                                                      MeterStart,
                                                      ReservationId);


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
                                                                    "/StartTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "StartTransaction",
                                                 RequestLogDelegate:   OnStartTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStartTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request, StartTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StartTransactionResponse>(httpresponse,
                                                                                                       new StartTransactionResponse(
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

                                                     return new HTTPResponse<StartTransactionResponse>(httpresponse,
                                                                                                       new StartTransactionResponse(
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

                                                     return HTTPResponse<StartTransactionResponse>.ExceptionThrown(new StartTransactionResponse(
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
                result = HTTPResponse<StartTransactionResponse>.OK(new StartTransactionResponse(request, Result.OK("Nothing to upload!")));


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   Description,
                                                   EventTrackingId,
                                                   ConnectorId,
                                                   IdTag,
                                                   TransactionTimestamp,
                                                   MeterStart,
                                                   ReservationId,
                                                   RequestTimeout,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region StatusNotification           (ConnectorId, Status, ErrorCode, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
        /// <param name="VendorErrorCode">A vendor-specific error code.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StatusNotificationResponse>>

            StatusNotification(Connector_Id           ConnectorId,
                               ChargePointStatus      Status,
                               ChargePointErrorCodes  ErrorCode,
                               String                 Info               = null,
                               DateTime?              StatusTimestamp    = null,
                               String                 VendorId           = null,
                               String                 VendorErrorCode    = null,

                               DateTime?              Timestamp          = null,
                               CancellationToken?     CancellationToken  = null,
                               EventTracking_Id       EventTrackingId    = null,
                               TimeSpan?              RequestTimeout     = null)

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


            HTTPResponse<StatusNotificationResponse> result = null;

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    Timestamp.Value,
                                                    this,
                                                    Description,
                                                    EventTrackingId,
                                                    ConnectorId,
                                                    Status,
                                                    ErrorCode,
                                                    Info,
                                                    StatusTimestamp,
                                                    VendorId,
                                                    VendorErrorCode,
                                                    RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var request = new StatusNotificationRequest(ChargeBoxIdentity,
                                                        ConnectorId,
                                                        Status,
                                                        ErrorCode,
                                                        Info,
                                                        StatusTimestamp,
                                                        VendorId,
                                                        VendorErrorCode);


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
                                                                    "/StatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "StatusNotification",
                                                 RequestLogDelegate:   OnStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      StatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StatusNotificationResponse>(httpresponse,
                                                                                                         new StatusNotificationResponse(
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

                                                     return new HTTPResponse<StatusNotificationResponse>(httpresponse,
                                                                                                         new StatusNotificationResponse(
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

                                                     return HTTPResponse<StatusNotificationResponse>.ExceptionThrown(new StatusNotificationResponse(
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
                result = HTTPResponse<StatusNotificationResponse>.OK(new StatusNotificationResponse(request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     Timestamp.Value,
                                                     this,
                                                     Description,
                                                     EventTrackingId,
                                                     ConnectorId,
                                                     Status,
                                                     ErrorCode,
                                                     Info,
                                                     StatusTimestamp,
                                                     VendorId,
                                                     VendorErrorCode,
                                                     RequestTimeout,
                                                     result.Content,
                                                     org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region MeterValues                  (ConnectorId, TransactionId = null, MeterValues = null, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<MeterValuesResponse>>

            MeterValues(Connector_Id             ConnectorId,
                        Transaction_Id?          TransactionId      = null,
                        IEnumerable<MeterValue>  MeterValues        = null,

                        DateTime?                Timestamp          = null,
                        CancellationToken?       CancellationToken  = null,
                        EventTracking_Id         EventTrackingId    = null,
                        TimeSpan?                RequestTimeout     = null)

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


            HTTPResponse<MeterValuesResponse> result = null;

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             Timestamp.Value,
                                             this,
                                             Description,
                                             EventTrackingId,
                                             ConnectorId,
                                             TransactionId,
                                             MeterValues,
                                             RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var request = new MeterValuesRequest(ChargeBoxIdentity,
                                                 ConnectorId,
                                                 MeterValues,
                                                 TransactionId);


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
                                                                    "/MeterValues",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "MeterValues",
                                                 RequestLogDelegate:   OnMeterValuesSOAPRequest,
                                                 ResponseLogDelegate:  OnMeterValuesSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      MeterValuesResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<MeterValuesResponse>(httpresponse,
                                                                                                  new MeterValuesResponse(
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

                                                     return new HTTPResponse<MeterValuesResponse>(httpresponse,
                                                                                                  new MeterValuesResponse(
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

                                                     return HTTPResponse<MeterValuesResponse>.ExceptionThrown(new MeterValuesResponse(
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
                result = HTTPResponse<MeterValuesResponse>.OK(new MeterValuesResponse(request,
                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              Timestamp.Value,
                                              this,
                                              Description,
                                              EventTrackingId,
                                              ConnectorId,
                                              TransactionId,
                                              MeterValues,
                                              RequestTimeout,
                                              result.Content,
                                              org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region StopTransaction              (TransactionId, TransactionTimestamp, MeterStop, ...)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="TransactionTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StopTransactionResponse>>

            StopTransaction(Transaction_Id           TransactionId,
                            DateTime                 TransactionTimestamp,
                            UInt64                   MeterStop,
                            IdToken?                 IdTag              = null,
                            Reasons?                 Reason             = null,
                            IEnumerable<MeterValue>  TransactionData    = null,

                            DateTime?                Timestamp          = null,
                            CancellationToken?       CancellationToken  = null,
                            EventTracking_Id         EventTrackingId    = null,
                            TimeSpan?                RequestTimeout     = null)

        {

            #region Initial checks

            if (TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<StopTransactionResponse> result = null;

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 Timestamp.Value,
                                                 this,
                                                 Description,
                                                 EventTrackingId,
                                                 TransactionId,
                                                 TransactionTimestamp,
                                                 MeterStop,
                                                 IdTag,
                                                 Reason,
                                                 TransactionData,
                                                 RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            var request = new StopTransactionRequest(ChargeBoxIdentity,
                                                     TransactionId,
                                                     TransactionTimestamp,
                                                     MeterStop,
                                                     IdTag,
                                                     Reason,
                                                     TransactionData);


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
                                                                    "/StopTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "StopTransaction",
                                                 RequestLogDelegate:   OnStopTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStopTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request, StopTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StopTransactionResponse>(httpresponse,
                                                                                                      new StopTransactionResponse(
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

                                                     return new HTTPResponse<StopTransactionResponse>(httpresponse,
                                                                                                      new StopTransactionResponse(
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

                                                     return HTTPResponse<StopTransactionResponse>.ExceptionThrown(new StopTransactionResponse(
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
                result = HTTPResponse<StopTransactionResponse>.OK(new StopTransactionResponse(request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   Description,
                                                   EventTrackingId,
                                                   TransactionId,
                                                   TransactionTimestamp,
                                                   MeterStop,
                                                   IdTag,
                                                   Reason,
                                                   TransactionData,
                                                   RequestTimeout,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region DataTransfer                 (VendorId, MessageId = null, Data = null, ...)

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
        public async Task<HTTPResponse<DataTransferResponse>>

            DataTransfer(String              VendorId,
                         String              MessageId          = null,
                         String              Data               = null,

                         DateTime?           Timestamp          = null,
                         CancellationToken?  CancellationToken  = null,
                         EventTracking_Id    EventTrackingId    = null,
                         TimeSpan?           RequestTimeout     = null)

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


            HTTPResponse<DataTransferResponse> result = null;

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
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var request = new CS.DataTransferRequest(ChargeBoxIdentity,
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
                                                                    null,
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
                                                                                                      DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<DataTransferResponse>(httpresponse,
                                                                                                   new DataTransferResponse(
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

                                                     return new HTTPResponse<DataTransferResponse>(httpresponse,
                                                                                                   new DataTransferResponse(
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

                                                     return HTTPResponse<DataTransferResponse>.ExceptionThrown(new DataTransferResponse(
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
                result = HTTPResponse<DataTransferResponse>.OK(new DataTransferResponse(request,
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
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region DiagnosticsStatusNotification(Status, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<DiagnosticsStatusNotificationResponse>>

            DiagnosticsStatusNotification(DiagnosticsStatus   Status,

                                          DateTime?           Timestamp          = null,
                                          CancellationToken?  CancellationToken  = null,
                                          EventTracking_Id    EventTrackingId    = null,
                                          TimeSpan?           RequestTimeout     = null)

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


            HTTPResponse<DiagnosticsStatusNotificationResponse> result = null;

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                               Timestamp.Value,
                                                               this,
                                                               Description,
                                                               EventTrackingId,
                                                               Status,
                                                               RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            var request = new DiagnosticsStatusNotificationRequest(ChargeBoxIdentity,
                                                                   Status);


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
                                                                    "/DiagnosticsStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "DiagnosticsStatusNotification",
                                                 RequestLogDelegate:   OnDiagnosticsStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnDiagnosticsStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      DiagnosticsStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<DiagnosticsStatusNotificationResponse>(httpresponse,
                                                                                                                    new DiagnosticsStatusNotificationResponse(
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

                                                     return new HTTPResponse<DiagnosticsStatusNotificationResponse>(httpresponse,
                                                                                                                    new DiagnosticsStatusNotificationResponse(
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

                                                     return HTTPResponse<DiagnosticsStatusNotificationResponse>.ExceptionThrown(new DiagnosticsStatusNotificationResponse(
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
                result = HTTPResponse<DiagnosticsStatusNotificationResponse>.OK(new DiagnosticsStatusNotificationResponse(request,
                                                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                Timestamp.Value,
                                                                this,
                                                                Description,
                                                                EventTrackingId,
                                                                Status,
                                                                RequestTimeout,
                                                                result.Content,
                                                                org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region FirmwareStatusNotification   (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<FirmwareStatusNotificationResponse>>

            FirmwareStatusNotification(FirmwareStatus      Status,

                                       DateTime?           Timestamp          = null,
                                       CancellationToken?  CancellationToken  = null,
                                       EventTracking_Id    EventTrackingId    = null,
                                       TimeSpan?           RequestTimeout     = null)

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


            HTTPResponse<FirmwareStatusNotificationResponse> result = null;

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                            Timestamp.Value,
                                                            this,
                                                            Description,
                                                            EventTrackingId,
                                                            Status,
                                                            RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var request = new FirmwareStatusNotificationRequest(ChargeBoxIdentity,
                                                                Status);


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
                                                                    "/FirmwareStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    request.ToXML()),
                                                 "FirmwareStatusNotification",
                                                 RequestLogDelegate:   OnFirmwareStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnFirmwareStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(request,
                                                                                                      FirmwareStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<FirmwareStatusNotificationResponse>(httpresponse,
                                                                                                                 new FirmwareStatusNotificationResponse(
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

                                                     return new HTTPResponse<FirmwareStatusNotificationResponse>(httpresponse,
                                                                                                                 new FirmwareStatusNotificationResponse(
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

                                                     return HTTPResponse<FirmwareStatusNotificationResponse>.ExceptionThrown(new FirmwareStatusNotificationResponse(
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
                result = HTTPResponse<FirmwareStatusNotificationResponse>.OK(new FirmwareStatusNotificationResponse(request,
                                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                             Timestamp.Value,
                                                             this,
                                                             Description,
                                                             EventTrackingId,
                                                             Status,
                                                             RequestTimeout,
                                                             result.Content,
                                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion


    }

}
