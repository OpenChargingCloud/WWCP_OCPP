﻿/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The common interface of all HTTP web socket charge point clients.
    /// </summary>
    public interface IChargePointWebSocketClient : ICPOutgoingMessages
    {

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnAuthorizeWSResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnStartTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnStartTransactionWSResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnMeterValuesWSResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnStopTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnStopTransactionWSResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnDataTransferWSResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnDiagnosticsStatusNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnFirmwareStatusNotificationWSResponse;

        #endregion


        // Security extensions

        #region OnLogStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a log status notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnLogStatusNotificationWSResponse;

        #endregion

        #region OnSecurityEventNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a security event notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnSecurityEventNotificationWSResponse;

        #endregion

        #region OnSignCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a sign certificate WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnSignCertificateWSResponse;

        #endregion

        #region OnSignedFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a signed firmware status notification WS request will be sent to the central system.
        /// </summary>
        event ClientRequestLogHandler   OnSignedFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a signed firmware status notification WS request was received.
        /// </summary>
        event ClientResponseLogHandler  OnSignedFirmwareStatusNotificationWSResponse;

        #endregion


    }

}
