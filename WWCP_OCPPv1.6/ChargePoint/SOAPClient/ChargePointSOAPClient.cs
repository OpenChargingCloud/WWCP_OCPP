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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charge point SOAP client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargePointSOAPClient : ASOAPClient,
                                                 IChargePointSOAPClient
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const           String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.String + " CP Client";

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
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                  { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                    { get; }

        /// <summary>
        /// The optional error message when this client closed the HTTP SOAP connection.
        /// </summary>
        public String?         ClientCloseMessage    { get; private set; }

        /// <summary>
        /// The attached OCPP CP client (HTTP/SOAP client) logger.
        /// </summary>
        public CPClientLogger  Logger                { get; }

        #endregion

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification Request will be sent to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate?    OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?              OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification Request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?   OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat Request will be sent to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate?    OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?       OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?      OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat Request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?   OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize Request will be sent to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate?    OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?       OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?      OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize Request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?   OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction Request will be sent to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate?    OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?              OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction Request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate?   OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification Request will be sent to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?    OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?               OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification Request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?   OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values Request will be sent to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate?    OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?         OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?        OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values Request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?   OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction Request will be sent to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate?    OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?             OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?            OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction Request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate?   OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer Request will be sent to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate?    OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?          OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer Request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?   OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification Request will be sent to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?    OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                           OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?                          OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification Request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?   OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification Request will be sent to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification SOAP Request will be sent to the central system.
        /// </summary>
        public event ClientRequestLogHandler?                        OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification Request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?   OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion

        #region Constructor(s)

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
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargePointSOAPClient(ChargeBox_Id                         ChargeBoxIdentity,
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
                                     String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                     HTTPPath?                            URLPathPrefix                = null,
                                     Tuple<String, String>?               WSSLoginPassword             = null,
                                     HTTPContentType?                     HTTPContentType              = null,
                                     TimeSpan?                            RequestTimeout               = null,
                                     TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                     UInt16?                              MaxNumberOfRetries           = null,
                                     UInt32?                              InternalBufferSize           = null,
                                     Boolean                              UseHTTPPipelining            = false,

                                     String?                              LoggingPath                  = null,
                                     String                               LoggingContext               = CPClientLogger.DefaultContext,
                                     LogfileCreatorDelegate?              LogfileCreator               = null,
                                     Boolean?                             DisableLogging               = false,
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
                   null,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   WSSLoginPassword,
                   HTTPContentType,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   DisableLogging,
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
                                                         LoggingPath,
                                                         LoggingContext,
                                                         LogfileCreator);

        }

        #endregion


        // <cs:chargeBoxIdentity se:mustUnderstand="true">CP1234</cs:chargeBoxIdentity>


        #region SendBootNotification             (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public async Task<BootNotificationResponse> SendBootNotification(BootNotificationRequest Request)
        {

            #region Send OnBootNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBootNotificationRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            HTTPResponse<BootNotificationResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                   "/" + Request.Action,
                                                                   Request_Id.NewRandom().ToString(),
                                                                   From,
                                                                   To,
                                                                   Request.ToXML()),
                                                Request.Action,
                                                RequestLogDelegate:   OnBootNotificationSOAPRequest,
                                                ResponseLogDelegate:  OnBootNotificationSOAPResponse,
                                                CancellationToken:    Request.CancellationToken,
                                                EventTrackingId:      Request.EventTrackingId,
                                                RequestTimeout:       Request.RequestTimeout,

                #region OnSuccess

                OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                     BootNotificationResponse.Parse),

                #endregion

                #region OnSOAPFault

                OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                    SendSOAPError(timestamp, this, httpresponse.Content);

                    return HTTPResponse<BootNotificationResponse>.IsFault(httpresponse,
                                                                          new BootNotificationResponse(
                                                                              Request,
                                                                              Result.Format(
                                                                                  "Invalid SOAP => " +
                                                                                  httpresponse.HTTPBody.ToUTF8String()
                                                                              )
                                                                          ));

                },

                #endregion

                #region OnHTTPError

                OnHTTPError: (timestamp, soapclient, httpresponse) => {

                    SendHTTPError(timestamp, this, httpresponse);

                    return HTTPResponse<BootNotificationResponse>.IsFault(httpresponse,
                                                                          new BootNotificationResponse(
                                                                              Request,
                                                                              Result.Server(
                                                                                   httpresponse.HTTPStatusCode.ToString() +
                                                                                   " => " +
                                                                                   httpresponse.HTTPBody.      ToUTF8String()
                                                                              )
                                                                          ));

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

            result ??= HTTPResponse<BootNotificationResponse>.OK(new BootNotificationResponse(Request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBootNotificationResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   result.Content,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendHeartbeat                    (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public async Task<HeartbeatResponse> SendHeartbeat(HeartbeatRequest Request)

        {

            #region Send OnHeartbeatRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnHeartbeatRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            HTTPResponse<HeartbeatResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/Heartbeat",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "Heartbeat",
                                                 RequestLogDelegate:   OnHeartbeatSOAPRequest,
                                                 ResponseLogDelegate:  OnHeartbeatSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, HeartbeatResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<HeartbeatResponse>.IsFault(httpresponse,
                                                                                                    new HeartbeatResponse(
                                                                                                        Request,
                                                                                                        Result.Format(
                                                                                                            "Invalid SOAP => " +
                                                                                                            httpresponse.HTTPBody.ToUTF8String()
                                                                                                        )
                                                                                                    ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<HeartbeatResponse>.IsFault(httpresponse,
                                                                                                    new HeartbeatResponse(
                                                                                                        Request,
                                                                                                        Result.Server(
                                                                                                             httpresponse.HTTPStatusCode.ToString() +
                                                                                                             " => " +
                                                                                                             httpresponse.HTTPBody.      ToUTF8String()
                                                                                                        )
                                                                                                    ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<HeartbeatResponse>.ExceptionThrown(new HeartbeatResponse(
                                                                                                                Request,
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<HeartbeatResponse>.OK(new HeartbeatResponse(Request,
                                                                                Result.OK("Nothing to upload!")));


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            result.Content,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        #region Authorize                        (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public async Task<AuthorizeResponse> Authorize(AuthorizeRequest Request)

        {

            #region Send OnAuthorizeRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            HTTPResponse<AuthorizeResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/Authorize",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "Authorize",
                                                 RequestLogDelegate:   OnAuthorizeSOAPRequest,
                                                 ResponseLogDelegate:  OnAuthorizeSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      AuthorizeResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<AuthorizeResponse>.IsFault(httpresponse,
                                                                                                    new AuthorizeResponse(
                                                                                                        Request,
                                                                                                        Result.Format(
                                                                                                            "Invalid SOAP => " +
                                                                                                            httpresponse.HTTPBody.ToUTF8String()
                                                                                                        )
                                                                                                    ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<AuthorizeResponse>.IsFault(httpresponse,
                                                                                                    new AuthorizeResponse(
                                                                                                        Request,
                                                                                                        Result.Server(
                                                                                                             httpresponse.HTTPStatusCode.ToString() +
                                                                                                             " => " +
                                                                                                             httpresponse.HTTPBody.      ToUTF8String()
                                                                                                        )
                                                                                                    ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<AuthorizeResponse>.ExceptionThrown(new AuthorizeResponse(
                                                                                                                Request,
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<AuthorizeResponse>.OK(new AuthorizeResponse(Request,
                                                                                Result.OK("Nothing to upload!")));


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            result.Content,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region StartTransaction                 (Request)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A start transaction request.</param>
        public async Task<StartTransactionResponse> StartTransaction(StartTransactionRequest Request)

        {

            #region Send OnStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStartTransactionRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            HTTPResponse<StartTransactionResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StartTransaction",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StartTransaction",
                                                 RequestLogDelegate:   OnStartTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStartTransactionSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      StartTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<StartTransactionResponse>.IsFault(httpresponse,
                                                                                                           new StartTransactionResponse(
                                                                                                               Request,
                                                                                                               Result.Format(
                                                                                                                   "Invalid SOAP => " +
                                                                                                                   httpresponse.HTTPBody.ToUTF8String()
                                                                                                               )
                                                                                                           ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<StartTransactionResponse>.IsFault(httpresponse,
                                                                                                           new StartTransactionResponse(
                                                                                                               Request,
                                                                                                               Result.Server(
                                                                                                                    httpresponse.HTTPStatusCode.ToString() +
                                                                                                                    " => " +
                                                                                                                    httpresponse.HTTPBody.      ToUTF8String()
                                                                                                               )
                                                                                                           ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StartTransactionResponse>.ExceptionThrown(new StartTransactionResponse(
                                                                                                                       Request,
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<StartTransactionResponse>.OK(new StartTransactionResponse(Request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStartTransactionResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   result.Content,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendStatusNotification           (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public async Task<StatusNotificationResponse> SendStatusNotification(StatusNotificationRequest Request)

        {

            #region Send OnStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStatusNotificationRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            HTTPResponse<StatusNotificationResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    false,
                                                    null,
                                                    PreferIPv4,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    TLSProtocol,
                                                    HTTPUserAgent,
                                                    null,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    HTTPContentType,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    InternalBufferSize,
                                                    UseHTTPPipelining,
                                                    DisableLogging,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StatusNotification",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StatusNotification",
                                                 RequestLogDelegate:   OnStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnStatusNotificationSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      StatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<StatusNotificationResponse>.IsFault(httpresponse,
                                                                                                             new StatusNotificationResponse(
                                                                                                                 Request,
                                                                                                                 Result.Format(
                                                                                                                     "Invalid SOAP => " +
                                                                                                                     httpresponse.HTTPBody.ToUTF8String()
                                                                                                                 )
                                                                                                             ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<StatusNotificationResponse>.IsFault(httpresponse,
                                                                                                             new StatusNotificationResponse(
                                                                                                                 Request,
                                                                                                                 Result.Server(
                                                                                                                      httpresponse.HTTPStatusCode.ToString() +
                                                                                                                      " => " +
                                                                                                                      httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                 )
                                                                                                             ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StatusNotificationResponse>.ExceptionThrown(new StatusNotificationResponse(
                                                                                                                         Request,
                                                                                                                         Result.Format(exception.Message +
                                                                                                                                       " => " +
                                                                                                                                       exception.StackTrace)),
                                                                                                                     exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<StatusNotificationResponse>.OK(new StatusNotificationResponse(Request,
                                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     result.Content,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendMeterValues                  (Request)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public async Task<MeterValuesResponse> SendMeterValues(MeterValuesRequest Request)

        {

            #region Send OnMeterValuesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnMeterValuesRequest?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            HTTPResponse<MeterValuesResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/MeterValues",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "MeterValues",
                                                 RequestLogDelegate:   OnMeterValuesSOAPRequest,
                                                 ResponseLogDelegate:  OnMeterValuesSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      MeterValuesResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<MeterValuesResponse>.IsFault(httpresponse,
                                                                                                      new MeterValuesResponse(
                                                                                                          Request,
                                                                                                          Result.Format(
                                                                                                              "Invalid SOAP => " +
                                                                                                              httpresponse.HTTPBody.ToUTF8String()
                                                                                                          )
                                                                                                      ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<MeterValuesResponse>.IsFault(httpresponse,
                                                                                                      new MeterValuesResponse(
                                                                                                          Request,
                                                                                                          Result.Server(
                                                                                                               httpresponse.HTTPStatusCode.ToString() +
                                                                                                               " => " +
                                                                                                               httpresponse.HTTPBody.      ToUTF8String()
                                                                                                          )
                                                                                                      ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<MeterValuesResponse>.ExceptionThrown(new MeterValuesResponse(
                                                                                                                  Request,
                                                                                                                  Result.Format(exception.Message +
                                                                                                                                " => " +
                                                                                                                                exception.StackTrace)),
                                                                                                              exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<MeterValuesResponse>.OK(new MeterValuesResponse(Request,
                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnMeterValuesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnMeterValuesResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              result.Content,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region StopTransaction                  (Request)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        public async Task<StopTransactionResponse> StopTransaction(StopTransactionRequest Request)

        {

            #region Send OnStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStopTransactionRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            HTTPResponse<StopTransactionResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StopTransaction",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StopTransaction",
                                                 RequestLogDelegate:   OnStopTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStopTransactionSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, StopTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<StopTransactionResponse>.IsFault(httpresponse,
                                                                                                          new StopTransactionResponse(
                                                                                                              Request,
                                                                                                              Result.Format(
                                                                                                                  "Invalid SOAP => " +
                                                                                                                  httpresponse.HTTPBody.ToUTF8String()
                                                                                                              )
                                                                                                          ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<StopTransactionResponse>.IsFault(httpresponse,
                                                                                                          new StopTransactionResponse(
                                                                                                              Request,
                                                                                                              Result.Server(
                                                                                                                   httpresponse.HTTPStatusCode.ToString() +
                                                                                                                   " => " +
                                                                                                                   httpresponse.HTTPBody.      ToUTF8String()
                                                                                                              )
                                                                                                          ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StopTransactionResponse>.ExceptionThrown(new StopTransactionResponse(
                                                                                                                      Request,
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<StopTransactionResponse>.OK(new StopTransactionResponse(Request,
                                                                                            Result.OK("Nothing to upload!")));


            #region Send OnStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStopTransactionResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  result.Content,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        #region TransferData                     (Request)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public async Task<CS.DataTransferResponse> TransferData(DataTransferRequest Request)

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
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            HTTPResponse<CS.DataTransferResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/DataTransfer",
                                                                    Request_Id.NewRandom().ToString(),
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
                                                                                                      CS.DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<CS.DataTransferResponse>.IsFault(httpresponse,
                                                                                                          new CS.DataTransferResponse(
                                                                                                              Request,
                                                                                                              Result.Format(
                                                                                                                  "Invalid SOAP => " +
                                                                                                                  httpresponse.HTTPBody.ToUTF8String()
                                                                                                              )
                                                                                                          ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<CS.DataTransferResponse>.IsFault(httpresponse,
                                                                                                          new CS.DataTransferResponse(
                                                                                                              Request,
                                                                                                              Result.Server(
                                                                                                                   httpresponse.HTTPStatusCode.ToString() +
                                                                                                                   " => " +
                                                                                                                   httpresponse.HTTPBody.      ToUTF8String()
                                                                                                              )
                                                                                                          ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<CS.DataTransferResponse>.ExceptionThrown(new CS.DataTransferResponse(
                                                                                                                      Request,
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<CS.DataTransferResponse>.OK(new CS.DataTransferResponse(Request,
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
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendDiagnosticsStatusNotification(Request)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Request">A diagnostics status notification request.</param>
        public async Task<DiagnosticsStatusNotificationResponse> SendDiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest Request)
        {

            #region Send OnDiagnosticsStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
                                                               this,
                                                               Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            HTTPResponse<DiagnosticsStatusNotificationResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/DiagnosticsStatusNotification",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "DiagnosticsStatusNotification",
                                                 RequestLogDelegate:   OnDiagnosticsStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnDiagnosticsStatusNotificationSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      DiagnosticsStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<DiagnosticsStatusNotificationResponse>.IsFault(httpresponse,
                                                                                                                        new DiagnosticsStatusNotificationResponse(
                                                                                                                            Request,
                                                                                                                            Result.Format(
                                                                                                                                "Invalid SOAP => " +
                                                                                                                                httpresponse.HTTPBody.ToUTF8String()
                                                                                                                            )
                                                                                                                        ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<DiagnosticsStatusNotificationResponse>.IsFault(httpresponse,
                                                                                                                        new DiagnosticsStatusNotificationResponse(
                                                                                                                            Request,
                                                                                                                            Result.Server(
                                                                                                                                 httpresponse.HTTPStatusCode.ToString() +
                                                                                                                                 " => " +
                                                                                                                                 httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                            )
                                                                                                                        ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<DiagnosticsStatusNotificationResponse>.ExceptionThrown(new DiagnosticsStatusNotificationResponse(
                                                                                                                                    Request,
                                                                                                                                    Result.Format(exception.Message +
                                                                                                                                                  " => " +
                                                                                                                                                  exception.StackTrace)),
                                                                                                                                exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<DiagnosticsStatusNotificationResponse>.OK(new DiagnosticsStatusNotificationResponse(Request,
                                                                                                                        Result.OK("Nothing to upload!")));


            #region Send OnDiagnosticsStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(endTime,
                                                                this,
                                                                Request,
                                                                result.Content,
                                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion

        #region SendFirmwareStatusNotification   (Request)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public async Task<FirmwareStatusNotificationResponse> SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request)
        {

            #region Send OnFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            HTTPResponse<FirmwareStatusNotificationResponse>? result = null;

            using (var soapClient = new SOAPClient(RemoteURL,
                                                   VirtualHostname,
                                                   false,
                                                   null,
                                                   PreferIPv4,
                                                   RemoteCertificateValidator,
                                                   ClientCertificateSelector,
                                                   ClientCert,
                                                   TLSProtocol,
                                                   HTTPUserAgent,
                                                   null,
                                                   URLPathPrefix,
                                                   WSSLoginPassword,
                                                   HTTPContentType,
                                                   RequestTimeout,
                                                   TransmissionRetryDelay,
                                                   MaxNumberOfRetries,
                                                   InternalBufferSize,
                                                   UseHTTPPipelining,
                                                   DisableLogging,
                                                   HTTPLogger,
                                                   DNSClient))
            {

                result = await soapClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/FirmwareStatusNotification",
                                                                    Request_Id.NewRandom().ToString(),
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "FirmwareStatusNotification",
                                                 RequestLogDelegate:   OnFirmwareStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnFirmwareStatusNotificationSOAPResponse,
                                                 CancellationToken:    Request.CancellationToken,
                                                 EventTrackingId:      Request.EventTrackingId,
                                                 RequestTimeout:       Request.RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      FirmwareStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return HTTPResponse<FirmwareStatusNotificationResponse>.IsFault(httpresponse,
                                                                                                                     new FirmwareStatusNotificationResponse(
                                                                                                                         Request,
                                                                                                                         Result.Format(
                                                                                                                             "Invalid SOAP => " +
                                                                                                                             httpresponse.HTTPBody.ToUTF8String()
                                                                                                                         )
                                                                                                                     ));

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return HTTPResponse<FirmwareStatusNotificationResponse>.IsFault(httpresponse,
                                                                                                                     new FirmwareStatusNotificationResponse(
                                                                                                                         Request,
                                                                                                                         Result.Server(
                                                                                                                              httpresponse.HTTPStatusCode.ToString() +
                                                                                                                              " => " +
                                                                                                                              httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                         )
                                                                                                                     ));

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<FirmwareStatusNotificationResponse>.ExceptionThrown(new FirmwareStatusNotificationResponse(
                                                                                                                                 Request,
                                                                                                                                 Result.Format(exception.Message +
                                                                                                                                               " => " +
                                                                                                                                               exception.StackTrace)),
                                                                                                                             exception);

                                                 }

                                                 #endregion

                                                );

            }

            result ??= HTTPResponse<FirmwareStatusNotificationResponse>.OK(new FirmwareStatusNotificationResponse(Request,
                                                                                                                  Result.OK("Nothing to upload!")));


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             result.Content,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointSOAPClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return result.Content;

        }

        #endregion


        // Security extensions

        #region Security extensions

        public Task<LogStatusNotificationResponse> SendLogStatusNotification(LogStatusNotificationRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SecurityEventNotificationResponse> SendSecurityEventNotification(SecurityEventNotificationRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SignCertificateResponse> SendCertificateSigningRequest(SignCertificateRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SignedFirmwareStatusNotificationResponse> SignedFirmwareStatusNotification(SignedFirmwareStatusNotificationRequest Request)
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}
