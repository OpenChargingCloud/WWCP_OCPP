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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// The common interface of all HTTP WebSocket charging station clients.
    /// </summary>
    public interface IChargingStationWebSocketClient : IChargingStationClient
    {

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnBootNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnFirmwareStatusNotificationWSResponse;

        #endregion

        #region OnPublishFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a publish firmware status notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnPublishFirmwareStatusNotificationWSResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnHeartbeatWSResponse;

        #endregion

        #region OnNotifyEventRequest/-Response

        /// <summary>
        /// An event fired whenever a notify event HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify event HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyEventWSResponse;

        #endregion

        #region OnSecurityEventNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a security event notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnSecurityEventNotificationWSResponse;

        #endregion

        #region OnNotifyReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify report HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyReportWSResponse;

        #endregion

        #region OnNotifyMonitoringReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify monitoring report HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyMonitoringReportWSResponse;

        #endregion

        #region OnLogStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a log status notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnLogStatusNotificationWSResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnDataTransferWSResponse;

        #endregion


        #region OnSignCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a sign certificate HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnSignCertificateWSResponse;

        #endregion

        #region OnGet15118EVCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnGet15118EVCertificateWSResponse;

        #endregion

        #region OnGetCertificateStatusRequest/-Response

        /// <summary>
        /// An event fired whenever a get certificate status HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate status HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnGetCertificateStatusWSResponse;

        #endregion


        #region OnReservationStatusUpdateRequest/-Response

        /// <summary>
        /// An event fired whenever a reservation status update HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a reservation status update HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnReservationStatusUpdateWSResponse;

        #endregion

        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnAuthorizeWSResponse;

        #endregion

        #region OnNotifyEVChargingNeedsRequest/-Response

        /// <summary>
        /// An event fired whenever a notify EV charging needs HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyEVChargingNeedsWSResponse;

        #endregion

        #region OnTransactionEventRequest/-Response

        /// <summary>
        /// An event fired whenever a transaction event HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnTransactionEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a transaction event HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler OnTransactionEventWSResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnMeterValuesWSResponse;

        #endregion

        #region OnNotifyChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a notify charging limit HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyChargingLimitWSResponse;

        #endregion

        #region OnClearedChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a cleared charging limit HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnClearedChargingLimitWSResponse;

        #endregion

        #region OnReportChargingProfilesRequest/-Response

        /// <summary>
        /// An event fired whenever a report charging profiles HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a report charging profiles HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnReportChargingProfilesWSResponse;

        #endregion


        #region OnNotifyDisplayMessagesRequest/-Response

        /// <summary>
        /// An event fired whenever a notify display messages HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify display messages HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyDisplayMessagesWSResponse;

        #endregion

        #region OnNotifyCustomerInformationRequest/-Response

        /// <summary>
        /// An event fired whenever a notify customer information HTTP WebSocket request will be sent to the CSMS.
        /// </summary>
        event ClientRequestLogHandler   OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information HTTP WebSocket request was received.
        /// </summary>
        event ClientResponseLogHandler  OnNotifyCustomerInformationWSResponse;

        #endregion


    }

}
