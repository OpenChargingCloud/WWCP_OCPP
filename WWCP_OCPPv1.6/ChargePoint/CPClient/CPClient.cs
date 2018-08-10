/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

using org.GraphDefined.WWCP.OCPPv1_6.CS;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP CP client.
    /// </summary>
    public partial class CPClient : ASOAPClient,
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
        /// The unique identification of this OCPP charge box.
        /// </summary>
        public ChargeBox_Id    ChargeBoxIdentity
            => ChargeBox_Id.Parse(ClientId);

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

        #region CPClient(ChargeBoxIdentity, Hostname, ..., LoggingContext = CPClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new OCPP CP client.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this OCPP charge box.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="Hostname">The OCPP hostname to connect to.</param>
        /// <param name="RemotePort">An optional OCPP TCP port to connect to.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URIPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPClient(String                               ChargeBoxIdentity,
                        String                               From,
                        String                               To,

                        String                               Hostname,
                        IPPort?                              RemotePort                   = null,
                        RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                        LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                        String                               HTTPVirtualHost              = null,
                        HTTPURI?                             URIPrefix                    = null,
                        String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                        TimeSpan?                            RequestTimeout               = null,
                        Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
                        DNSClient                            DNSClient                    = null,
                        String                               LoggingContext               = CPClientLogger.DefaultContext,
                        LogfileCreatorDelegate               LogFileCreator               = null)

            : base(ChargeBoxIdentity,
                   Hostname,
                   RemotePort ?? DefaultRemotePort,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   HTTPVirtualHost,
                   URIPrefix ?? DefaultURIPrefix,
                   null,
                   HTTPUserAgent,
                   RequestTimeout,
                   MaxNumberOfRetries,
                   DNSClient)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");


            if (Hostname.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Hostname),           "The given hostname must not be null or empty!");

            #endregion

            this.From       = From;
            this.To         = To;

            this.Logger     = new CPClientLogger(this,
                                                 LoggingContext,
                                                 LogFileCreator);

        }

        #endregion

        #region CPClient(ChargeBoxIdentity, Logger, Hostname, ...)

        /// <summary>
        /// Create a new OCPP CP client.
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
        /// <param name="URIPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public CPClient(String                               ChargeBoxIdentity,
                        String                               From,
                        String                               To,

                        CPClientLogger                       Logger,
                        String                               Hostname,
                        IPPort?                              RemotePort                   = null,
                        RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                        LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                        String                               HTTPVirtualHost              = null,
                        HTTPURI?                             URIPrefix                    = null,
                        String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                        TimeSpan?                            RequestTimeout               = null,
                        Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
                        DNSClient                            DNSClient                    = null)

            : base(ChargeBoxIdentity,
                   Hostname,
                   RemotePort ?? DefaultRemotePort,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   HTTPVirtualHost,
                   URIPrefix ?? DefaultURIPrefix,
                   null,
                   HTTPUserAgent,
                   RequestTimeout,
                   MaxNumberOfRetries,
                   DNSClient)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");


            if (Logger == null)
                throw new ArgumentNullException(nameof(Logger),             "The given client logger must not be null!");

            if (Hostname.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Hostname),           "The given hostname must not be null or empty!");

            #endregion

            this.From       = From;
            this.To         = To;

            this.Logger     = Logger;

        }

        #endregion

        #endregion


        #region BootNotification(ChargePointVendor, ChargePointModel, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// <param name="ChargePointSerialNumber">The serial number of the charge point.</param>
        /// <param name="FirmwareVersion">The firmware version of the charge point.</param>
        /// <param name="Iccid">The ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">The IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">The meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">The serial number of the main power meter of the charge point.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<BootNotificationResponse>>

            BootNotification(String              ChargePointVendor,
                             String              ChargePointModel,
                             String              ChargePointSerialNumber  = null,
                             String              FirmwareVersion          = null,
                             String              Iccid                    = null,
                             String              IMSI                     = null,
                             String              MeterType                = null,
                             String              MeterSerialNumber        = null,

                             DateTime?           Timestamp                = null,
                             CancellationToken?  CancellationToken        = null,
                             EventTracking_Id    EventTrackingId          = null,
                             TimeSpan?           RequestTimeout           = null)

        {

            #region Initial checks

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor identification must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model identification must not be null or empty!");


            if (!Timestamp.HasValue)
                Timestamp = DateTime.Now;

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

                OnBootNotificationRequest?.Invoke(DateTime.UtcNow,
                                                  Timestamp.Value,
                                                  this,
                                                  ClientId,
                                                  EventTrackingId,
                                                  ChargePointVendor,
                                                  ChargePointModel,
                                                  ChargePointSerialNumber,
                                                  FirmwareVersion,
                                                  Iccid,
                                                  IMSI,
                                                  MeterType,
                                                  MeterSerialNumber,
                                                  RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var Request = new BootNotificationRequest(ChargePointVendor,
                                                      ChargePointModel,
                                                      ChargePointSerialNumber,
                                                      FirmwareVersion,
                                                      Iccid,
                                                      IMSI,
                                                      MeterType,
                                                      MeterSerialNumber);

            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
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

                OnBootNotificationResponse?.Invoke(DateTime.UtcNow,
                                                   Timestamp.Value,
                                                   this,
                                                   ClientId,
                                                   EventTrackingId,
                                                   ChargePointVendor,
                                                   ChargePointModel,
                                                   ChargePointSerialNumber,
                                                   FirmwareVersion,
                                                   Iccid,
                                                   IMSI,
                                                   MeterType,
                                                   MeterSerialNumber,
                                                   RequestTimeout,
                                                   result.Content,
                                                   DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region Heartbeat(...)

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
                Timestamp = DateTime.Now;

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

                OnHeartbeatRequest?.Invoke(DateTime.UtcNow,
                                           Timestamp.Value,
                                           this,
                                           ClientId,
                                           EventTrackingId,
                                           RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/Heartbeat",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new HeartbeatRequest().ToXML()),
                                                 "Heartbeat",
                                                 RequestLogDelegate:   OnHeartbeatSOAPRequest,
                                                 ResponseLogDelegate:  OnHeartbeatSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(HeartbeatResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<HeartbeatResponse>(httpresponse,
                                                                                                new HeartbeatResponse(
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
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<HeartbeatResponse>.OK(new HeartbeatResponse(Result.OK("Nothing to upload!")));


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(DateTime.UtcNow,
                                            Timestamp.Value,
                                            this,
                                            ClientId,
                                            EventTrackingId,
                                            RequestTimeout,
                                            result.Content,
                                            DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return result;


        }

        #endregion


        #region Authorize         (IdTag, ...)

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
                Timestamp = DateTime.Now;

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

                OnAuthorizeRequest?.Invoke(DateTime.UtcNow,
                                           Timestamp.Value,
                                           this,
                                           ClientId,
                                           EventTrackingId,
                                           IdTag,
                                           RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion

            var Request = new AuthorizeRequest(IdTag);


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/Authorize",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "Authorize",
                                                 RequestLogDelegate:   OnAuthorizeSOAPRequest,
                                                 ResponseLogDelegate:  OnAuthorizeSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, AuthorizeResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
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

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
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

            if (result == null)
                result = HTTPResponse<AuthorizeResponse>.OK(new AuthorizeResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(DateTime.UtcNow,
                                            Timestamp.Value,
                                            this,
                                            ClientId,
                                            EventTrackingId,
                                            IdTag,
                                            RequestTimeout,
                                            result.Content,
                                            DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region StartTransaction  (ConnectorId, IdTag, TransactionTimestamp, MeterStart, ReservationId = null, ...)

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
                Timestamp = DateTime.Now;

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

                OnStartTransactionRequest?.Invoke(DateTime.UtcNow,
                                                  Timestamp.Value,
                                                  this,
                                                  ClientId,
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
                e.Log(nameof(CPClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/StartTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new StartTransactionRequest(ConnectorId,
                                                                                                IdTag,
                                                                                                TransactionTimestamp,
                                                                                                MeterStart,
                                                                                                ReservationId).ToXML()),
                                                 "StartTransaction",
                                                 RequestLogDelegate:   OnStartTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStartTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(StartTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StartTransactionResponse>(httpresponse,
                                                                                                       new StartTransactionResponse(
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
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StartTransactionResponse>.OK(new StartTransactionResponse(Result.OK("Nothing to upload!")));


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(DateTime.UtcNow,
                                                   Timestamp.Value,
                                                   this,
                                                   ClientId,
                                                   EventTrackingId,
                                                   ConnectorId,
                                                   IdTag,
                                                   TransactionTimestamp,
                                                   MeterStart,
                                                   ReservationId,
                                                   RequestTimeout,
                                                   result.Content,
                                                   DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region StatusNotification(ConnectorId, Status, ErrorCode, ...)

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
                Timestamp = DateTime.Now;

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

                OnStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                    Timestamp.Value,
                                                    this,
                                                    ClientId,
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
                e.Log(nameof(CPClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/StatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new StatusNotificationRequest(ConnectorId,
                                                                                                  Status,
                                                                                                  ErrorCode,
                                                                                                  Info,
                                                                                                  StatusTimestamp,
                                                                                                  VendorId,
                                                                                                  VendorErrorCode).ToXML()),
                                                 "StatusNotification",
                                                 RequestLogDelegate:   OnStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(StatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StatusNotificationResponse>(httpresponse,
                                                                                                         new StatusNotificationResponse(
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
                                                                                                                         Result.Format(exception.Message +
                                                                                                                                       " => " +
                                                                                                                                       exception.StackTrace)),
                                                                                                                     exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StatusNotificationResponse>.OK(new StatusNotificationResponse(Result.OK("Nothing to upload!")));


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                     Timestamp.Value,
                                                     this,
                                                     ClientId,
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
                                                     DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region MeterValues       (ConnectorId, TransactionId = null, MeterValues = null, ...)

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
                Timestamp = DateTime.Now;

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

                OnMeterValuesRequest?.Invoke(DateTime.UtcNow,
                                             Timestamp.Value,
                                             this,
                                             ClientId,
                                             EventTrackingId,
                                             ConnectorId,
                                             TransactionId,
                                             MeterValues,
                                             RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/MeterValues",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new MeterValuesRequest(ConnectorId,
                                                                                           TransactionId,
                                                                                           MeterValues).ToXML()),
                                                 "MeterValues",
                                                 RequestLogDelegate:   OnMeterValuesSOAPRequest,
                                                 ResponseLogDelegate:  OnMeterValuesSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(MeterValuesResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<MeterValuesResponse>(httpresponse,
                                                                                                  new MeterValuesResponse(
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
                                                                                                                  Result.Format(exception.Message +
                                                                                                                                " => " +
                                                                                                                                exception.StackTrace)),
                                                                                                              exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<MeterValuesResponse>.OK(new MeterValuesResponse(Result.OK("Nothing to upload!")));


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(DateTime.UtcNow,
                                              Timestamp.Value,
                                              this,
                                              ClientId,
                                              EventTrackingId,
                                              ConnectorId,
                                              TransactionId,
                                              MeterValues,
                                              RequestTimeout,
                                              result.Content,
                                              DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region StopTransaction   (TransactionId, TransactionTimestamp, MeterStop, ...)

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
                Timestamp = DateTime.Now;

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

                OnStopTransactionRequest?.Invoke(DateTime.UtcNow,
                                                 Timestamp.Value,
                                                 this,
                                                 ClientId,
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
                e.Log(nameof(CPClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/StopTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new StopTransactionRequest(TransactionId,
                                                                                               TransactionTimestamp,
                                                                                               MeterStop,
                                                                                               IdTag,
                                                                                               Reason,
                                                                                               TransactionData).ToXML()),
                                                 "StopTransaction",
                                                 RequestLogDelegate:   OnStopTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStopTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(StopTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StopTransactionResponse>(httpresponse,
                                                                                                      new StopTransactionResponse(
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
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StopTransactionResponse>.OK(new StopTransactionResponse(Result.OK("Nothing to upload!")));


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(DateTime.UtcNow,
                                                   Timestamp.Value,
                                                   this,
                                                   ClientId,
                                                   EventTrackingId,
                                                   TransactionId,
                                                   TransactionTimestamp,
                                                   MeterStop,
                                                   IdTag,
                                                   Reason,
                                                   TransactionData,
                                                   RequestTimeout,
                                                   result.Content,
                                                   DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return result;


        }

        #endregion


        #region DataTransfer(VendorId, MessageId = null, Data = null, ...)

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
                Timestamp = DateTime.Now;

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

                OnDataTransferRequest?.Invoke(DateTime.UtcNow,
                                              Timestamp.Value,
                                              this,
                                              ClientId,
                                              EventTrackingId,
                                              VendorId,
                                              MessageId,
                                              Data,
                                              RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion

            var Request = new DataTransferRequest(VendorId,
                                                  MessageId,
                                                  Data);


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/DataTransfer",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "DataTransfer",
                                                 RequestLogDelegate:   OnDataTransferSOAPRequest,
                                                 ResponseLogDelegate:  OnDataTransferSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<DataTransferResponse>(httpresponse,
                                                                                                   new DataTransferResponse(
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
                                                                                                                   Result.Format(exception.Message +
                                                                                                                                 " => " +
                                                                                                                                 exception.StackTrace)),
                                                                                                               exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<DataTransferResponse>.OK(new DataTransferResponse(Result.OK("Nothing to upload!")));


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(DateTime.UtcNow,
                                                   Timestamp.Value,
                                                   this,
                                                   ClientId,
                                                   EventTrackingId,
                                                   VendorId,
                                                   MessageId,
                                                   Data,
                                                   RequestTimeout,
                                                   result.Content,
                                                   DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnDataTransferResponse));
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
                Timestamp = DateTime.Now;

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

                OnDiagnosticsStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                               Timestamp.Value,
                                                               this,
                                                               ClientId,
                                                               EventTrackingId,
                                                               Status,
                                                               RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/DiagnosticsStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new DiagnosticsStatusNotificationRequest(Status).ToXML()),
                                                 "DiagnosticsStatusNotification",
                                                 RequestLogDelegate:   OnDiagnosticsStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnDiagnosticsStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(DiagnosticsStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<DiagnosticsStatusNotificationResponse>(httpresponse,
                                                                                                                    new DiagnosticsStatusNotificationResponse(
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
                                                                                                                                    Result.Format(exception.Message +
                                                                                                                                                  " => " +
                                                                                                                                                  exception.StackTrace)),
                                                                                                                                exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<DiagnosticsStatusNotificationResponse>.OK(new DiagnosticsStatusNotificationResponse(Result.OK("Nothing to upload!")));


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                                Timestamp.Value,
                                                                this,
                                                                ClientId,
                                                                EventTrackingId,
                                                                Status,
                                                                RequestTimeout,
                                                                result.Content,
                                                                DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
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
                Timestamp = DateTime.Now;

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

                OnFirmwareStatusNotificationRequest?.Invoke(DateTime.UtcNow,
                                                            Timestamp.Value,
                                                            this,
                                                            ClientId,
                                                            EventTrackingId,
                                                            Status,
                                                            RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemotePort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/FirmwareStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new FirmwareStatusNotificationRequest(Status).ToXML()),
                                                 "FirmwareStatusNotification",
                                                 RequestLogDelegate:   OnFirmwareStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnFirmwareStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(FirmwareStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<FirmwareStatusNotificationResponse>(httpresponse,
                                                                                                                 new FirmwareStatusNotificationResponse(
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
                                                                                                                                 Result.Format(exception.Message +
                                                                                                                                               " => " +
                                                                                                                                               exception.StackTrace)),
                                                                                                                             exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<FirmwareStatusNotificationResponse>.OK(new FirmwareStatusNotificationResponse(Result.OK("Nothing to upload!")));


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(DateTime.UtcNow,
                                                             Timestamp.Value,
                                                             this,
                                                             ClientId,
                                                             EventTrackingId,
                                                             Status,
                                                             RequestTimeout,
                                                             result.Content,
                                                             DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CPClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return result;


        }

        #endregion


    }

}
