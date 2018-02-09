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

using org.GraphDefined.WWCP.OCPPv1_6.CP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP CS client.
    /// </summary>
    public partial class CSClient : ASOAPClient,
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
        /// The attached OCPP CS client (HTTP/SOAP client) logger.
        /// </summary>
        public CSClientLogger  Logger              { get; }

        #endregion

        #region Events

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

        #endregion

        #region Constructor(s)

        #region CSClient(ChargeBoxIdentity, Hostname, ..., LoggingContext = CSClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new OCPP CS client.
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
        public CSClient(String                               ChargeBoxIdentity,
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
                        String                               LoggingContext               = CSClientLogger.DefaultContext,
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

            this.Logger     = new CSClientLogger(this,
                                                 LoggingContext,
                                                 LogFileCreator);

        }

        #endregion

        #region CSClient(ChargeBoxIdentity, Logger, Hostname, ...)

        /// <summary>
        /// Create a new OCPP CS client.
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
        public CSClient(String                               ChargeBoxIdentity,
                        String                               From,
                        String                               To,

                        CSClientLogger                       Logger,
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


        #region ReserveNow            (ChargeBoxIdentity, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Reserve a connector for the given IdTag and till the given timestamp.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<ReserveNowResponse>>

            ReserveNow(ChargeBox_Id        ChargeBoxIdentity,
                       Connector_Id        ConnectorId,
                       Reservation_Id      ReservationId,
                       DateTime            ExpiryDate,
                       IdToken             IdTag,
                       IdToken?            ParentIdTag        = null,

                       DateTime?           Timestamp          = null,
                       CancellationToken?  CancellationToken  = null,
                       EventTracking_Id    EventTrackingId    = null,
                       TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity == null)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null!");

            if (ConnectorId       == null)
                throw new ArgumentNullException(nameof(ConnectorId),        "The given connector identification must not be null!");

            if (ReservationId     == null)
                throw new ArgumentNullException(nameof(ReservationId),      "The given reservation identification must not be null!");

            if (IdTag             == null)
                throw new ArgumentNullException(nameof(IdTag),              "The given identification tag must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = DateTime.Now;

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

                OnReserveNowRequest?.Invoke(DateTime.Now,
                                            Timestamp.Value,
                                            this,
                                            ClientId,
                                            EventTrackingId,
                                            ChargeBoxIdentity,
                                            ConnectorId,
                                            ReservationId,
                                            ExpiryDate,
                                            IdTag,
                                            ParentIdTag,
                                            RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    HTTPPort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/ReserveNow",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new ReserveNowRequest(ConnectorId,
                                                                                          ReservationId,
                                                                                          ExpiryDate,
                                                                                          IdTag,
                                                                                          ParentIdTag).ToXML()),
                                                 "ReserveNow",
                                                 RequestLogDelegate:   OnReserveNowSOAPRequest,
                                                 ResponseLogDelegate:  OnReserveNowSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(ReserveNowResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<ReserveNowResponse>(httpresponse,
                                                                                                 new ReserveNowResponse(
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
                                                                                                                 Result.Format(exception.Message +
                                                                                                                               " => " +
                                                                                                                               exception.StackTrace)),
                                                                                                             exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<ReserveNowResponse>.OK(new ReserveNowResponse(Result.OK("Nothing to upload!")));


            #region Send OnReserveNowResponse event

            try
            {

                OnReserveNowResponse?.Invoke(DateTime.Now,
                                             Timestamp.Value,
                                             this,
                                             ClientId,
                                             EventTrackingId,
                                             ChargeBoxIdentity,
                                             ConnectorId,
                                             ReservationId,
                                             ExpiryDate,
                                             IdTag,
                                             ParentIdTag,
                                             RequestTimeout,
                                             result.Content,
                                             DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region CancelReservation     (ChargeBoxIdentity, ReservationId, ...)

        /// <summary>
        /// Cancel the given reservation.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<CancelReservationResponse>>

            CancelReservation(ChargeBox_Id        ChargeBoxIdentity,
                              Reservation_Id      ReservationId,

                              DateTime?           Timestamp          = null,
                              CancellationToken?  CancellationToken  = null,
                              EventTracking_Id    EventTrackingId    = null,
                              TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (ReservationId == null)
                throw new ArgumentNullException(nameof(ReservationId),  "The given reservation identification must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = DateTime.Now;

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

                OnCancelReservationRequest?.Invoke(DateTime.Now,
                                                   Timestamp.Value,
                                                   this,
                                                   ClientId,
                                                   EventTrackingId,
                                                   ChargeBoxIdentity,
                                                   ReservationId,
                                                   RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    HTTPPort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/CancelReservation",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new CancelReservationRequest(ReservationId).ToXML()),
                                                 "CancelReservation",
                                                 RequestLogDelegate:   OnCancelReservationSOAPRequest,
                                                 ResponseLogDelegate:  OnCancelReservationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(CancelReservationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<CancelReservationResponse>(httpresponse,
                                                                                                        new CancelReservationResponse(
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
                                                                                                                        Result.Format(exception.Message +
                                                                                                                                      " => " +
                                                                                                                                      exception.StackTrace)),
                                                                                                                    exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<CancelReservationResponse>.OK(new CancelReservationResponse(Result.OK("Nothing to upload!")));


            #region Send OnCancelReservationResponse event

            try
            {

                OnCancelReservationResponse?.Invoke(DateTime.Now,
                                                    Timestamp.Value,
                                                    this,
                                                    ClientId,
                                                    EventTrackingId,
                                                    ChargeBoxIdentity,
                                                    ReservationId,
                                                    RequestTimeout,
                                                    result.Content,
                                                    DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region RemoteStartTransaction(ChargeBoxIdentity, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Starte a charging transaction at the given connector.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<RemoteStartTransactionResponse>>

            RemoteStartTransaction(ChargeBox_Id        ChargeBoxIdentity,
                                   IdToken             IdTag,
                                   Connector_Id?       ConnectorId        = null,
                                   ChargingProfile     ChargingProfile    = null,

                                   DateTime?           Timestamp          = null,
                                   CancellationToken?  CancellationToken  = null,
                                   EventTracking_Id    EventTrackingId    = null,
                                   TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity == null)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null!");

            if (IdTag == null)
                throw new ArgumentNullException(nameof(IdTag),              "The given identification tag must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = DateTime.Now;

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

                OnRemoteStartTransactionRequest?.Invoke(DateTime.Now,
                                                        Timestamp.Value,
                                                        this,
                                                        ClientId,
                                                        EventTrackingId,
                                                        ChargeBoxIdentity,
                                                        IdTag,
                                                        ConnectorId,
                                                        ChargingProfile,
                                                        RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    HTTPPort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/RemoteStartTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new RemoteStartTransactionRequest(IdTag,
                                                                                                      ConnectorId,
                                                                                                      ChargingProfile).ToXML()),
                                                 "RemoteStartTransaction",
                                                 RequestLogDelegate:   OnRemoteStartTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnRemoteStartTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(RemoteStartTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<RemoteStartTransactionResponse>(httpresponse,
                                                                                                             new RemoteStartTransactionResponse(
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
                                                                                                                             Result.Format(exception.Message +
                                                                                                                                           " => " +
                                                                                                                                           exception.StackTrace)),
                                                                                                                         exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<RemoteStartTransactionResponse>.OK(new RemoteStartTransactionResponse(Result.OK("Nothing to upload!")));


            #region Send OnRemoteStartTransactionResponse event

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(DateTime.Now,
                                                         Timestamp.Value,
                                                         this,
                                                         ClientId,
                                                         EventTrackingId,
                                                         ChargeBoxIdentity,
                                                         IdTag,
                                                         ConnectorId,
                                                         ChargingProfile,
                                                         RequestTimeout,
                                                         result.Content,
                                                         DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return result;


        }

        #endregion

        #region RemoteStopTransaction (ChargeBoxIdentity, TransactionId, ...)

        /// <summary>
        /// Stop the given charging transaction.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<RemoteStopTransactionResponse>>

            RemoteStopTransaction(ChargeBox_Id        ChargeBoxIdentity,
                                  Transaction_Id      TransactionId,

                                  DateTime?           Timestamp          = null,
                                  CancellationToken?  CancellationToken  = null,
                                  EventTracking_Id    EventTrackingId    = null,
                                  TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity == null)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null!");

            if (TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),      "The given transaction identification must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = DateTime.Now;

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

                OnRemoteStopTransactionRequest?.Invoke(DateTime.Now,
                                                       Timestamp.Value,
                                                       this,
                                                       ClientId,
                                                       EventTrackingId,
                                                       ChargeBoxIdentity,
                                                       TransactionId,
                                                       RequestTimeout);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    HTTPPort,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    UserAgent,
                                                    RequestTimeout,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(ChargeBoxIdentity,
                                                                    "/RemoteStopTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    new RemoteStopTransactionRequest(TransactionId).ToXML()),
                                                 "RemoteStopTransaction",
                                                 RequestLogDelegate:   OnRemoteStopTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnRemoteStopTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(RemoteStopTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<RemoteStopTransactionResponse>(httpresponse,
                                                                                                            new RemoteStopTransactionResponse(
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
                                                                                                                            Result.Format(exception.Message +
                                                                                                                                          " => " +
                                                                                                                                          exception.StackTrace)),
                                                                                                                        exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<RemoteStopTransactionResponse>.OK(new RemoteStopTransactionResponse(Result.OK("Nothing to upload!")));


            #region Send OnRemoteStopTransactionResponse event

            try
            {

                OnRemoteStopTransactionResponse?.Invoke(DateTime.Now,
                                                        Timestamp.Value,
                                                        this,
                                                        ClientId,
                                                        EventTrackingId,
                                                        ChargeBoxIdentity,
                                                        TransactionId,
                                                        RequestTimeout,
                                                        result.Content,
                                                        DateTime.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                e.Log(nameof(CSClient) + "." + nameof(OnRemoteStopTransactionResponse));
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
        public async Task<HTTPResponse<CP.DataTransferResponse>>

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


            HTTPResponse<CP.DataTransferResponse> result = null;

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(DateTime.Now,
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
                e.Log(nameof(CSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(Hostname,
                                                    HTTPVirtualHost,
                                                    URIPrefix,
                                                    HTTPPort,
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
                                                                    new DataTransferRequest(VendorId,
                                                                                            MessageId,
                                                                                            Data).ToXML()),
                                                 "DataTransfer",
                                                 RequestLogDelegate:   OnDataTransferSOAPRequest,
                                                 ResponseLogDelegate:  OnDataTransferSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(CP.DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<CP.DataTransferResponse>(httpresponse,
                                                                                                      new CP.DataTransferResponse(
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
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<CP.DataTransferResponse>.OK(new CP.DataTransferResponse(Result.OK("Nothing to upload!")));


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(DateTime.Now,
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
                e.Log(nameof(CSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result;


        }

        #endregion



    }

}
