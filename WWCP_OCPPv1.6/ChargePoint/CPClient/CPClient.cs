/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP CP client.
    /// </summary>
    public partial class CPClient : ASOAPClient
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
        public String          ChargeBoxIdentity
            => ClientId;

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
        /// <param name="ClientCert">The TLS client certificate to use.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URIPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPClient(String                               ChargeBoxIdentity,
                        String                               From,
                        String                               To,

                        String                               Hostname,
                        IPPort                               RemotePort                  = null,
                        RemoteCertificateValidationCallback  RemoteCertificateValidator  = null,
                        X509Certificate                      ClientCert                  = null,
                        String                               HTTPVirtualHost             = null,
                        String                               URIPrefix                   = null,
                        String                               HTTPUserAgent               = DefaultHTTPUserAgent,
                        TimeSpan?                            RequestTimeout              = null,
                        DNSClient                            DNSClient                   = null,
                        String                               LoggingContext              = CPClientLogger.DefaultContext,
                        Func<String, String, String>         LogFileCreator              = null)

            : base(ChargeBoxIdentity,
                   Hostname,
                   RemotePort ?? DefaultRemotePort,
                   RemoteCertificateValidator,
                   ClientCert,
                   HTTPVirtualHost,
                   URIPrefix.Trim().IsNotNullOrEmpty() ? URIPrefix.Trim() : "",
                   HTTPUserAgent,
                   RequestTimeout,
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
        /// <param name="ClientCert">The TLS client certificate to use.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URIPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public CPClient(String                               ChargeBoxIdentity,
                        String                               From,
                        String                               To,

                        CPClientLogger                       Logger,
                        String                               Hostname,
                        IPPort                               RemotePort                  = null,
                        RemoteCertificateValidationCallback  RemoteCertificateValidator  = null,
                        X509Certificate                      ClientCert                  = null,
                        String                               HTTPVirtualHost             = null,
                        String                               URIPrefix                   = null,
                        String                               HTTPUserAgent               = DefaultHTTPUserAgent,
                        TimeSpan?                            RequestTimeout              = null,
                        DNSClient                            DNSClient                   = null)

            : base(ChargeBoxIdentity,
                   Hostname,
                   RemotePort ?? DefaultRemotePort,
                   RemoteCertificateValidator,
                   ClientCert,
                   HTTPVirtualHost,
                   URIPrefix.Trim().IsNotNullOrEmpty() ? URIPrefix.Trim() : "",
                   HTTPUserAgent,
                   RequestTimeout,
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


        #region Authorize(...)

        /// <summary>
        /// Authorize the given IdTag.
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

                OnAuthorizeRequest?.Invoke(DateTime.Now,
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


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    RemotePort,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemoteCertificateValidator,
                                                    ClientCert,
                                                    UserAgent,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/Authorize",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new AuthorizeRequest(IdTag).ToXML()),
                                                 "Authorize",
                                                 RequestLogDelegate:   OnAuthorizeSOAPRequest,
                                                 ResponseLogDelegate:  OnAuthorizeSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 QueryTimeout:         RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(AuthorizeResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
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
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<AuthorizeResponse>.OK(new AuthorizeResponse(Result.OK("Nothing to upload!")));


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(DateTime.Now,
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

        #region BootNotification(...)

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

                OnBootNotificationRequest?.Invoke(DateTime.Now,
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


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    RemotePort,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemoteCertificateValidator,
                                                    ClientCert,
                                                    UserAgent,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/BootNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new BootNotificationRequest(ChargePointVendor,
                                                                                                ChargePointModel,
                                                                                                ChargePointSerialNumber,
                                                                                                FirmwareVersion,
                                                                                                Iccid,
                                                                                                IMSI,
                                                                                                MeterType,
                                                                                                MeterSerialNumber).ToXML()),
                                                 "BootNotification",
                                                 RequestLogDelegate:   OnBootNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnBootNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 QueryTimeout:         RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(BootNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<BootNotificationResponse>(httpresponse,
                                                                                                       new BootNotificationResponse(
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
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<BootNotificationResponse>.OK(new BootNotificationResponse(Result.OK("Nothing to upload!")));


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(DateTime.Now,
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

                OnHeartbeatRequest?.Invoke(DateTime.Now,
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
                                                    RemotePort,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    RemoteCertificateValidator,
                                                    ClientCert,
                                                    UserAgent,
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
                                                 QueryTimeout:         RequestTimeout,

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

                OnHeartbeatResponse?.Invoke(DateTime.Now,
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


    }

}
