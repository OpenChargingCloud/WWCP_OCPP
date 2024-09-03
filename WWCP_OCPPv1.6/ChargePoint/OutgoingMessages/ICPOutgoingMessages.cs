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

#region Usings

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The common interface of all charge point clients.
    /// </summary>
    public interface ICPOutgoingMessages //: OCPP.CS.ICSOutgoingMessages
    {

        String?  ClientCloseMessage    { get; }


        #region BootNotification

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public Task<BootNotificationResponse> BootNotification(BootNotificationRequest Request);

        #endregion

        #region SendHeartbeat

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public Task<HeartbeatResponse> Heartbeat(HeartbeatRequest Request);

        #endregion

        #region DiagnosticsStatusNotification

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Request">A diagnostics status notification request.</param>
        public Task<DiagnosticsStatusNotificationResponse> DiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest Request);

        #endregion

        #region FirmwareStatusNotification

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public Task<FirmwareStatusNotificationResponse> FirmwareStatusNotification(FirmwareStatusNotificationRequest Request);

        #endregion


        #region Authorize

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public Task<AuthorizeResponse> Authorize(AuthorizeRequest Request);

        #endregion

        #region StartTransaction

        /// <summary>
        /// Send a start transaction information.
        /// </summary>
        /// <param name="Request">A start transaction request.</param>
        public Task<StartTransactionResponse> StartTransaction(StartTransactionRequest Request);

        #endregion

        #region StatusNotification

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public Task<StatusNotificationResponse> StatusNotification(StatusNotificationRequest Request);

        #endregion

        #region SendMeterValues

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public Task<MeterValuesResponse> MeterValues(MeterValuesRequest Request);

        #endregion

        #region StopTransaction

        /// <summary>
        /// Send a stop transaction information.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        public Task<StopTransactionResponse> StopTransaction(StopTransactionRequest Request);

        #endregion


        #region DataTransfer

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public Task<CS.DataTransferResponse> DataTransfer(DataTransferRequest Request);

        #endregion


        // Security extensions

        #region LogStatusNotification

        /// <summary>
        /// Send a log status notification to the central system.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
        public Task<LogStatusNotificationResponse>

            LogStatusNotification(LogStatusNotificationRequest  Request);

        #endregion

        #region SecurityEventNotification

        /// <summary>
        /// Send a security event notification to the central system.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
        public Task<SecurityEventNotificationResponse>

            SecurityEventNotification(SecurityEventNotificationRequest  Request);

        #endregion

        #region SignCertificate

        /// <summary>
        /// Send certificate signing request to the central system.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<SignCertificateResponse>

            SignCertificate(SignCertificateRequest  Request);

        #endregion

        #region SignedFirmwareStatusNotification

        /// <summary>
        /// Send a signed firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A signed firmware status notification request.</param>
        public Task<SignedFirmwareStatusNotificationResponse>

            SignedFirmwareStatusNotification(SignedFirmwareStatusNotificationRequest  Request);

        #endregion


    }

}
