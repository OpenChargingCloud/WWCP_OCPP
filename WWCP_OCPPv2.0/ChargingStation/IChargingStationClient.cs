/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_0.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The common interface of all charge point clients.
    /// </summary>
    public interface IChargingStationClient : IHTTPClient,
                                              IEventSender
    {

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the CSMS.
        /// </summary>
        event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the CSMS.
        /// </summary>
        event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be send to the CSMS.
        /// </summary>
        event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be send to the CSMS.
        /// </summary>
        event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be send to the CSMS.
        /// </summary>
        event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to the CSMS.
        /// </summary>
        event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be send to the CSMS.
        /// </summary>
        event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion


        #region SendBootNotification

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public Task<BootNotificationResponse> SendBootNotification(BootNotificationRequest Request);

        #endregion

        #region SendHeartbeat

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        public Task<HeartbeatResponse> SendHeartbeat(HeartbeatRequest Request);

        #endregion


        // SendReservationStatusUpdate

        #region Authorize

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        public Task<AuthorizeResponse> Authorize(AuthorizeRequest Request);

        #endregion

        #region SendTransactionEvent

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A transaction event request.</param>
        public Task<TransactionEventResponse> SendTransactionEvent(TransactionEventRequest Request);

        #endregion

        #region SendStatusNotification

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        public Task<StatusNotificationResponse> SendStatusNotification(StatusNotificationRequest Request);

        #endregion

        #region SendMeterValues

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        public Task<MeterValuesResponse> SendMeterValues(MeterValuesRequest Request);

        #endregion

        #region TransferData

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public Task<CSMS.DataTransferResponse> TransferData(DataTransferRequest Request);

        #endregion


        #region SendLogStatusNotification

        /// <summary>
        /// Send a log status notification to the CSMS.
        /// </summary>
        /// <param name="Request">A log status notification request.</param>
        public Task<LogStatusNotificationResponse>

            SendLogStatusNotification(LogStatusNotificationRequest  Request);

        #endregion

        #region SendSecurityEventNotification

        /// <summary>
        /// Send a security event notification to the CSMS.
        /// </summary>
        /// <param name="Request">A security event notification request.</param>
        public Task<SecurityEventNotificationResponse>

            SendSecurityEventNotification(SecurityEventNotificationRequest  Request);

        #endregion


        #region SignCertificate

        /// <summary>
        /// Send certificate signing request to the CSMS.
        /// </summary>
        /// <param name="Request">A sign certificate request.</param>
        public Task<SignCertificateResponse>

            SignCertificate(SignCertificateRequest  Request);

        #endregion

        // Get15118EVCertificate
        // GetCertificateStatus

        // NotifyEVChargingNeeds
        // ClearedChargingLimit
        // NotifyChargingLimit
        // ReportChargingProfiles

        // NotifyCustomerInformation
        // NotifyDisplayMessages

        // NotifyEvent
        // NotifyMonitoringReport
        // NotifyReport


        #region SendFirmwareStatusNotification

        /// <summary>
        /// Send a firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        public Task<FirmwareStatusNotificationResponse> SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request);

        #endregion

        // PublishFirmwareStatusNotification


    }

}
