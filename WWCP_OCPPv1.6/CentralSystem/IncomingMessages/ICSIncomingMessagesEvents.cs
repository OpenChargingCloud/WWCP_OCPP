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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The common interface of a central system server.
    /// </summary>
    public interface ICSIncomingMessagesEvents : OCPP.CSMS.ICSMSIncomingMessagesEvents
    {

        #region OnBootNotification (Request/-Response)

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event OnBootNotificationRequestDelegate    OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        event OnBootNotificationResponseDelegate   OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        event OnHeartbeatRequestDelegate    OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        event OnHeartbeatResponseDelegate   OnHeartbeatResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeRequestDelegate    OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        event OnAuthorizeResponseDelegate   OnAuthorizeResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        event OnStartTransactionRequestDelegate    OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        event OnStartTransactionResponseDelegate   OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationRequestDelegate    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        event OnStatusNotificationResponseDelegate   OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesRequestDelegate    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        event OnMeterValuesResponseDelegate   OnMeterValuesResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        event OnStopTransactionRequestDelegate    OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        event OnStopTransactionResponseDelegate   OnStopTransactionResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        event OnIncomingDataTransferRequestDelegate    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        event OnIncomingDataTransferResponseDelegate   OnIncomingDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationRequestDelegate    OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        event OnDiagnosticsStatusNotificationResponseDelegate   OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate   OnFirmwareStatusNotificationResponse;

        #endregion


        // Security extensions

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSecurityEventNotificationRequestDelegate OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnSecurityEventNotificationResponseDelegate OnSecurityEventNotificationResponse;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnLogStatusNotificationRequestDelegate    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnLogStatusNotificationResponseDelegate   OnLogStatusNotificationResponse;

        #endregion

        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSignCertificateRequestDelegate    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnSignCertificateResponseDelegate   OnSignCertificateResponse;

        #endregion

        #region OnSignedFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSignedFirmwareStatusNotificationRequestDelegate    OnSignedFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnSignedFirmwareStatusNotificationResponseDelegate   OnSignedFirmwareStatusNotificationResponse;

        #endregion


    }

}
