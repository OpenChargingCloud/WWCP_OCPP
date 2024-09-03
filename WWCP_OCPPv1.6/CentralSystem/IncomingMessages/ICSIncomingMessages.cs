/*
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
    public interface ICSIncomingMessages //: OCPP.CSMS.ICSMSIncomingMessages
    {

        #region WebSocket connection

        ///// <summary>
        ///// An event sent whenever the HTTP connection switched successfully to web socket.
        ///// </summary>
        //event OnCentralSystemNewWebSocketConnectionDelegate?    OnCentralSystemNewWebSocketConnection;

        ///// <summary>
        ///// An event sent whenever a web socket close frame was received.
        ///// </summary>
        //event OnCentralSystemCloseMessageReceivedDelegate?      OnCentralSystemCloseMessageReceived;

        ///// <summary>
        ///// An event sent whenever a TCP connection was closed.
        ///// </summary>
        //event OnCentralSystemTCPConnectionClosedDelegate?       OnCentralSystemTCPConnectionClosed;

        #endregion


        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        event OnBootNotificationDelegate           OnBootNotification;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        event OnHeartbeatDelegate           OnHeartbeat;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        event OnAuthorizeDelegate           OnAuthorize;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        event OnStartTransactionDelegate           OnStartTransaction;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        event OnStatusNotificationDelegate           OnStatusNotification;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        event OnMeterValuesDelegate           OnMeterValues;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        event OnStopTransactionDelegate           OnStopTransaction;

        #endregion


        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever an incoming DataTransfer request was received.
        /// </summary>
        event OnIncomingDataTransferDelegate           OnIncomingDataTransfer;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationDelegate           OnDiagnosticsStatusNotification;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationDelegate           OnFirmwareStatusNotification;

        #endregion


        // Security extensions

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSecurityEventNotificationDelegate OnSecurityEventNotification;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnLogStatusNotificationDelegate           OnLogStatusNotification;

        #endregion

        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSignCertificateDelegate           OnSignCertificate;

        #endregion

        #region OnSignedFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnSignedFirmwareStatusNotificationDelegate           OnSignedFirmwareStatusNotification;

        #endregion


    }

}
