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
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charge point for testing.
    /// </summary>
    public class TestChargePoint : IEventSender
    {

        #region Data

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly TimeSpan DefaultSendHeartbeatEvery = TimeSpan.FromMinutes(5);

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a central system.
        /// </summary>
        public ICPClient                CPClient                    { get; }


        /// <summary>
        /// The charge box identification.
        /// </summary>
        public ChargeBox_Id             ChargeBoxId                 { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxId.ToString();

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        public String                   ChargePointModel            { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        public I18NString               Description                 { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargeBoxSerialNumber       { get; }

        /// <summary>
        /// The optional firmware version of the charge point.
        /// </summary>
        public String                   FirmwareVersion             { get; }

        /// <summary>
        /// The optional ICCID of the charge point's SIM card.
        /// </summary>
        public String                   Iccid                       { get; }

        /// <summary>
        /// The optional IMSI of the charge point’s SIM card.
        /// </summary>
        public String                   IMSI                        { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charge point.
        /// </summary>
        public String                   MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charge point.
        /// </summary>
        public String                   MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charge point.
        /// </summary>
        public String                   MeterPublicKey              { get; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; }


        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                 DefaultRequestTimeout       { get; }

        #endregion

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

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
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charge box description.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
        public TestChargePoint(ChargeBox_Id  ChargeBoxId,
                               String        ChargePointVendor,
                               String        ChargePointModel,

                               I18NString    Description               = null,
                               String        ChargePointSerialNumber   = null,
                               String        ChargeBoxSerialNumber     = null,
                               String        FirmwareVersion           = null,
                               String        Iccid                     = null,
                               String        IMSI                      = null,
                               String        MeterType                 = null,
                               String        MeterSerialNumber         = null,
                               String        MeterPublicKey            = null,

                               TimeSpan?     SendHeartbeatEvery        = null,

                               TimeSpan?     DefaultRequestTimeout     = null)

        {

            if (ChargeBoxId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxId),        "The given charge box identification must not be null or empty!");

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model must not be null or empty!");


            this.ChargeBoxId              = ChargeBoxId;
            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;

            this.Description              = Description;
            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;

            this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

        }

        #endregion


        #region SendBootNotification         (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.BootNotificationResponse>

            SendBootNotification(CancellationToken?  CancellationToken   = null,
                                 EventTracking_Id    EventTrackingId     = null,
                                 TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new BootNotificationRequest(ChargeBoxId,
                                                                ChargePointVendor,
                                                                ChargePointModel,

                                                                ChargePointSerialNumber,
                                                                ChargeBoxSerialNumber,
                                                                FirmwareVersion,
                                                                Iccid,
                                                                IMSI,
                                                                MeterType,
                                                                MeterSerialNumber,

                                                                Request_Id.Random(),
                                                                requestTimestamp,
                                                                EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var response  = await CPClient.SendBootNotification(request,

                                                                requestTimestamp,
                                                                CancellationToken,
                                                                EventTrackingId,
                                                                RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response.Content,
                                                   Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion

        #region SendHeartbeat                (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.HeartbeatResponse>

            SendHeartbeat(CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new HeartbeatRequest(ChargeBoxId,

                                                         Request_Id.Random(),
                                                         requestTimestamp,
                                                         EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(requestTimestamp,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var response = await CPClient.SendHeartbeat(request,

                                                        requestTimestamp,
                                                        CancellationToken,
                                                        EventTrackingId,
                                                        RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                            this,
                                            request,
                                            response.Content,
                                            Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion


        #region Authorize                    (IdTag, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.AuthorizeResponse>

            Authorize(IdToken             IdTag,

                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new AuthorizeRequest(ChargeBoxId,
                                                         IdTag,

                                                         Request_Id.Random(),
                                                         requestTimestamp,
                                                         EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(requestTimestamp,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            var response = await CPClient.Authorize(request,

                                                    requestTimestamp,
                                                    CancellationToken,
                                                    EventTrackingId,
                                                    RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                            this,
                                            request,
                                            response.Content,
                                            Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response.Content;

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StartTransactionResponse>

            StartTransaction(Connector_Id        ConnectorId,
                             IdToken             IdTag,
                             DateTime            TransactionTimestamp,
                             UInt64              MeterStart,
                             Reservation_Id?     ReservationId       = null,

                             CancellationToken?  CancellationToken   = null,
                             EventTracking_Id    EventTrackingId     = null,
                             TimeSpan?           RequestTimeout      = null)

            {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StartTransactionRequest(ChargeBoxId,
                                                                ConnectorId,
                                                                IdTag,
                                                                TransactionTimestamp,
                                                                MeterStart,
                                                                ReservationId,

                                                                Request_Id.Random(),
                                                                requestTimestamp,
                                                                EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            var response = await CPClient.StartTransaction(request,

                                                           requestTimestamp,
                                                           CancellationToken,
                                                           EventTrackingId,
                                                           RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response.Content,
                                                   Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return response.Content;

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StatusNotificationResponse>

            StatusNotification(Connector_Id           ConnectorId,
                               ChargePointStatus      Status,
                               ChargePointErrorCodes  ErrorCode,
                               String                 Info                = null,
                               DateTime?              StatusTimestamp     = null,
                               String                 VendorId            = null,
                               String                 VendorErrorCode     = null,

                               CancellationToken?     CancellationToken   = null,
                               EventTracking_Id       EventTrackingId     = null,
                               TimeSpan?              RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StatusNotificationRequest(ChargeBoxId,
                                                                  ConnectorId,
                                                                  Status,
                                                                  ErrorCode,
                                                                  Info,
                                                                  StatusTimestamp,
                                                                  VendorId,
                                                                  VendorErrorCode,

                                                                  Request_Id.Random(),
                                                                  requestTimestamp,
                                                                  EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(requestTimestamp,
                                                    this,
                                                    request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.StatusNotification(request,

                                                             requestTimestamp,
                                                             CancellationToken,
                                                             EventTrackingId,
                                                             RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response.Content,
                                                     Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response.Content;

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.MeterValuesResponse>

            MeterValues(Connector_Id             ConnectorId,
                        IEnumerable<MeterValue>  MeterValues,
                        Transaction_Id?          TransactionId       = null,

                        CancellationToken?       CancellationToken   = null,
                        EventTracking_Id         EventTrackingId     = null,
                        TimeSpan?                RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new MeterValuesRequest(ChargeBoxId,
                                                           ConnectorId,
                                                           MeterValues,
                                                           TransactionId,

                                                           Request_Id.Random(),
                                                           requestTimestamp,
                                                           EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(requestTimestamp,
                                             this,
                                             request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            var response = await CPClient.MeterValues(request,

                                                      requestTimestamp,
                                                      CancellationToken,
                                                      EventTrackingId,
                                                      RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                              this,
                                              request,
                                              response.Content,
                                              Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response.Content;

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.StopTransactionResponse>

            StopTransaction(Transaction_Id           TransactionId,
                            DateTime                 TransactionTimestamp,
                            UInt64                   MeterStop,
                            IdToken?                 IdTag               = null,
                            Reasons?                 Reason              = null,
                            IEnumerable<MeterValue>  TransactionData     = null,

                            CancellationToken?       CancellationToken   = null,
                            EventTracking_Id         EventTrackingId     = null,
                            TimeSpan?                RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new StopTransactionRequest(ChargeBoxId,
                                                               TransactionId,
                                                               TransactionTimestamp,
                                                               MeterStop,
                                                               IdTag,
                                                               Reason,
                                                               TransactionData,

                                                               Request_Id.Random(),
                                                               requestTimestamp,
                                                               EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(requestTimestamp,
                                                 this,
                                                 request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            var response = await CPClient.StopTransaction(request,

                                                          requestTimestamp,
                                                          CancellationToken,
                                                          EventTrackingId,
                                                          RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                  this,
                                                  request,
                                                  response.Content,
                                                  Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return response.Content;

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.DataTransferResponse>

            DataTransfer(String              VendorId,
                         String              MessageId           = null,
                         String              Data                = null,

                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new DataTransferRequest(ChargeBoxId,
                                                            VendorId,
                                                            MessageId,
                                                            Data,

                                                            Request_Id.Random(),
                                                            requestTimestamp,
                                                            EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(requestTimestamp,
                                              this,
                                              request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response = await CPClient.DataTransfer(request,

                                                       requestTimestamp,
                                                       CancellationToken,
                                                       EventTrackingId,
                                                       RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(Timestamp.Now,
                                               this,
                                               request,
                                               response.Content,
                                               Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion

        #region DiagnosticsStatusNotification(Status, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.DiagnosticsStatusNotificationResponse>

            DiagnosticsStatusNotification(DiagnosticsStatus   Status,

                                          CancellationToken?  CancellationToken  = null,
                                          EventTracking_Id    EventTrackingId    = null,
                                          TimeSpan?           RequestTimeout     = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new DiagnosticsStatusNotificationRequest(ChargeBoxId,
                                                                             Status,

                                                                             Request_Id.Random(),
                                                                             requestTimestamp,
                                                                             EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(requestTimestamp,
                                                               this,
                                                               request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.DiagnosticsStatusNotification(request,

                                                                        requestTimestamp,
                                                                        CancellationToken,
                                                                        EventTrackingId,
                                                                        RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response.Content,
                                                                Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion

        #region FirmwareStatusNotification   (Status, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.FirmwareStatusNotificationResponse>

            FirmwareStatusNotification(FirmwareStatus      Status,

                                       CancellationToken?  CancellationToken  = null,
                                       EventTracking_Id    EventTrackingId    = null,
                                       TimeSpan?           RequestTimeout     = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new FirmwareStatusNotificationRequest(ChargeBoxId,
                                                                          Status,

                                                                          Request_Id.Random(),
                                                                          requestTimestamp,
                                                                          EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(requestTimestamp,
                                                            this,
                                                            request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            var response = await CPClient.FirmwareStatusNotification(request,

                                                                     requestTimestamp,
                                                                     CancellationToken,
                                                                     EventTrackingId,
                                                                     RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response.Content,
                                                             Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion


    }

}
