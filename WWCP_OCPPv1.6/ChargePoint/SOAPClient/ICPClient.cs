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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The common interface of all OCPP CP Clients.
    /// </summary>
    public interface ICPClient : IHTTPClient, IEventSender
    {

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler             OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler      OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be send to the central system.
        /// </summary>
        event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler      OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler     OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction request will be send to the central system.
        /// </summary>
        event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler             OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler            OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction request was received.
        /// </summary>
        event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be send to the central system.
        /// </summary>
        event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler               OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler              OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be send to the central system.
        /// </summary>
        event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler        OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler       OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction request will be send to the central system.
        /// </summary>
        event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler            OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction request was received.
        /// </summary>
        event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the central system.
        /// </summary>
        event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler         OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification request will be send to the central system.
        /// </summary>
        event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler                          OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the central system.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification SOAP request will be send to the central system.
        /// </summary>
        event ClientRequestLogHandler                       OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion


        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public Task<HTTPResponse<BootNotificationResponse>>

            SendBootNotification(BootNotificationRequest  Request,

                                 DateTime?                Timestamp           = null,
                                 CancellationToken?       CancellationToken   = null,
                                 EventTracking_Id         EventTrackingId     = null,
                                 TimeSpan?                RequestTimeout      = null);


        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A Heartbeat request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public Task<HTTPResponse<HeartbeatResponse>>

            SendHeartbeat(HeartbeatRequest    Request,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null);


    }

}
