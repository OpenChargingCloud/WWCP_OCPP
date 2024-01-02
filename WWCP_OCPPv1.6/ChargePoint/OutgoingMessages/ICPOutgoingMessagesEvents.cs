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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    public interface ICPOutgoingMessagesEvents : OCPP.CS.ICSOutgoingMessagesEvents
    {

        #region OnBootNotification                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the central system.
        /// </summary>
        event OnBootNotificationRequestDelegate                       OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        event OnBootNotificationResponseDelegate                      OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat                        (Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the central system.
        /// </summary>
        event OnHeartbeatRequestDelegate                              OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseDelegate                             OnHeartbeatResponse;

        #endregion

        #region OnDiagnosticsStatusNotification    (Request/-Response)

        /// <summary>
        /// An event fired whenever a DiagnosticsStatusNotification request will be sent to the central system.
        /// </summary>
        event OnDiagnosticsStatusNotificationRequestDelegate          OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a DiagnosticsStatusNotification request was received.
        /// </summary>
        event OnDiagnosticsStatusNotificationResponseDelegate         OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification       (Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent to the central system.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate             OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate            OnFirmwareStatusNotificationResponse;

        #endregion


        #region OnAuthorize                        (Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the central system.
        /// </summary>
        event OnAuthorizeRequestDelegate                              OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        event OnAuthorizeResponseDelegate                             OnAuthorizeResponse;

        #endregion

        #region OnStartTransaction                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a StartTransaction request will be sent to the central system.
        /// </summary>
        event OnStartTransactionRequestDelegate                       OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a StartTransaction request was received.
        /// </summary>
        event OnStartTransactionResponseDelegate                      OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification               (Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request will be sent to the central system.
        /// </summary>
        event OnStatusNotificationRequestDelegate                     OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        event OnStatusNotificationResponseDelegate                    OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request will be sent to the central system.
        /// </summary>
        event OnMeterValuesRequestDelegate                            OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        event OnMeterValuesResponseDelegate                           OnMeterValuesResponse;

        #endregion

        #region OnStopTransaction                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a StopTransaction request will be sent to the central system.
        /// </summary>
        event OnStopTransactionRequestDelegate                        OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a StopTransaction request was received.
        /// </summary>
        event OnStopTransactionResponseDelegate                       OnStopTransactionResponse;

        #endregion


        #region OnDataTransfer                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the central system.
        /// </summary>
        event OnDataTransferRequestDelegate                          OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate                         OnDataTransferResponse;

        #endregion


        // Security Extensions

        #region OnLogStatusNotification            (Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request will be sent to the central system.
        /// </summary>
        event OnLogStatusNotificationRequestDelegate?                 OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        event OnLogStatusNotificationResponseDelegate?                OnLogStatusNotificationResponse;

        #endregion

        #region OnSecurityEventNotification        (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request will be sent to the central system.
        /// </summary>
        event OnSecurityEventNotificationRequestDelegate?             OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        event OnSecurityEventNotificationResponseDelegate?            OnSecurityEventNotificationResponse;

        #endregion

        #region OnSignCertificate                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request will be sent to the central system.
        /// </summary>
        event OnSignCertificateRequestDelegate?                       OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        event OnSignCertificateResponseDelegate?                      OnSignCertificateResponse;

        #endregion

        #region OnSignedFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedFirmwareStatusNotification request will be sent to the central system.
        /// </summary>
        event OnSignedFirmwareStatusNotificationRequestDelegate?     OnSignedFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedFirmwareStatusNotification request was received.
        /// </summary>
        event OnSignedFirmwareStatusNotificationResponseDelegate?    OnSignedFirmwareStatusNotificationResponse;

        #endregion


    }

}
