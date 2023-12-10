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

using System.Collections.Concurrent;
using System.Security.Authentication;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The central system HTTP/WebSocket/JSON server.
    /// </summary>
    public class CentralSystemWSServer : ACSMSWSServer,
                                         ICSMSChannel
    {



        #region Events

        #region CSMS -> Charging Station

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a get configuration request was sent.
        /// </summary>
        public event OnGetConfigurationRequestDelegate?   OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a get configuration request was sent.
        /// </summary>
        public event OnGetConfigurationResponseDelegate?  OnGetConfigurationResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a change configuration request was sent.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate?   OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a change configuration request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate?  OnChangeConfigurationResponse;

        #endregion

        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a get diagnostics request was sent.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a response to a get diagnostics request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now request was sent.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate?   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote start transaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate?  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a remote stop transaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a get composite schedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a get composite schedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an unlock connector request was sent.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an unlock connector request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a get local list version request was sent.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a get local list version request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a send local list request was sent.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a send local list request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a clear cache request was sent.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a clear cache request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion


        // Security extensions

        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region OnExtendedTriggerMessage

        /// <summary>
        /// An event sent whenever an extended trigger message request was sent.
        /// </summary>
        public event OnExtendedTriggerMessageRequestDelegate?   OnExtendedTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to an extended trigger message request was sent.
        /// </summary>
        public event OnExtendedTriggerMessageResponseDelegate?  OnExtendedTriggerMessageResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log request was sent.
        /// </summary>
        public event OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        public event OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion

        #region OnSignedUpdateFirmware

        /// <summary>
        /// An event sent whenever a signed update firmware request was sent.
        /// </summary>
        public event OnSignedUpdateFirmwareRequestDelegate?   OnSignedUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a signed update firmware request was sent.
        /// </summary>
        public event OnSignedUpdateFirmwareResponseDelegate?  OnSignedUpdateFirmwareResponse;

        #endregion

        #endregion

        #region CSMS <- Charging Station

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
        public event OnBootNotificationDelegate?            OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a boot notification was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnBootNotificationWSResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?            OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a heartbeat was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnHeartbeatWSResponse;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?            OnAuthorize;

        /// <summary>
        /// An event sent whenever an authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an authorize web socket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnAuthorizeWSResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?           OnStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionRequestDelegate?    OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnStartTransactionDelegate?           OnStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a start transaction request was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate?   OnStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a start transaction request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?          OnStartTransactionWSResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?             OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?    OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?           OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a status notification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?   OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?            OnStatusNotificationWSResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?      OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?    OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnMeterValuesDelegate?           OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a meter values request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?   OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a meter values request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?     OnMeterValuesWSResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?          OnStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate?    OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate?           OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a stop transaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate?   OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a stop transaction request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?         OnStopTransactionWSResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?               OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferRequestDelegate?    OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferDelegate?           OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnIncomingDataTransferResponseDelegate?   OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a data transfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?              OnIncomingDataTransferWSResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                        OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?    OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate?           OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a diagnostics status notification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?   OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a diagnostics status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                       OnDiagnosticsStatusNotificationWSResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?    OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?           OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?   OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a firmware status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnFirmwareStatusNotificationWSResponse;

        #endregion


        // Security extensions

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a log status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?    OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationDelegate?           OnLogStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a log status notification request was sent.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?   OnLogStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a log status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnLogStatusNotificationWSResponse;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a security event notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                    OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?    OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?           OnSecurityEventNotification;

        /// <summary>
        /// An event sent whenever a response to a security event notification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?   OnSecurityEventNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a security event notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                   OnSecurityEventNotificationWSResponse;

        #endregion

        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a sign certificate web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?          OnSignCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateRequestDelegate?    OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateDelegate?           OnSignCertificate;

        /// <summary>
        /// An event sent whenever a response to a sign certificate request was sent.
        /// </summary>
        public event OnSignCertificateResponseDelegate?   OnSignCertificateResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a sign certificate request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?         OnSignCertificateWSResponse;

        #endregion

        #region OnSignedFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification web socket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                           OnSignedFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationRequestDelegate?    OnSignedFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationDelegate?           OnSignedFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationResponseDelegate?   OnSignedFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a web socket response to a firmware status notification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                          OnSignedFirmwareStatusNotificationWSResponse;

        #endregion

        #endregion

        #endregion

        #region Custom JSON parser/serializer delegates

        /// <summary>
        /// A delegate to parse custom BootNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<BootNotificationRequest>?                  CustomBootNotificationRequestParser                    { get; set; }

        /// <summary>
        /// A delegate to parse custom Heartbeat requests.
        /// </summary>
        public CustomJObjectParserDelegate<HeartbeatRequest>?                         CustomHeartbeatRequestParser                           { get; set; }


        /// <summary>
        /// A delegate to parse custom Authorize requests.
        /// </summary>
        public CustomJObjectParserDelegate<AuthorizeRequest>?                         CustomAuthorizeRequestParser                           { get; set; }

        /// <summary>
        /// A delegate to parse custom StartTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StartTransactionRequest>?                  CustomStartTransactionRequestParser                    { get; set; }

        /// <summary>
        /// A delegate to parse custom StatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<StatusNotificationRequest>?                CustomStatusNotificationRequestParser                  { get; set; }

        /// <summary>
        /// A delegate to parse custom MeterValues requests.
        /// </summary>
        public CustomJObjectParserDelegate<MeterValuesRequest>?                       CustomMeterValuesRequestParser                         { get; set; }

        /// <summary>
        /// A delegate to parse custom StopTransaction requests.
        /// </summary>
        public CustomJObjectParserDelegate<StopTransactionRequest>?                   CustomStopTransactionRequestParser                     { get; set; }


        /// <summary>
        /// A delegate to parse custom DiagnosticsStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?     CustomDiagnosticsStatusNotificationRequestParser       { get; set; }

        /// <summary>
        /// A delegate to parse custom FirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?        CustomFirmwareStatusNotificationRequestParser          { get; set; }


        // Security extensions

        /// <summary>
        /// A delegate to parse custom LogStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<LogStatusNotificationRequest>?             CustomLogStatusNotificationRequestParser               { get; set; }

        /// <summary>
        /// A delegate to parse custom SecurityEventNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<SecurityEventNotificationRequest>?         CustomSecurityEventNotificationRequestParser           { get; set; }

        /// <summary>
        /// A delegate to parse custom SignCertificate requests.
        /// </summary>
        public CustomJObjectParserDelegate<SignCertificateRequest>?                   CustomSignCertificateRequestParser                     { get; set; }

        /// <summary>
        /// A delegate to parse custom SignedFirmwareStatusNotification requests.
        /// </summary>
        public CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestParser    { get; set; }


        public CustomJObjectSerializerDelegate<ResetRequest>?                         CustomResetRequestSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?            CustomChangeAvailabilityRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<GetConfigurationRequest>?              CustomGetConfigurationRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?           CustomChangeConfigurationRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<OCPP.CSMS.DataTransferRequest>?        CustomDataTransferRequestSerializer                    { get; set; }

        public CustomJObjectSerializerDelegate<GetDiagnosticsRequest>?                CustomGetDiagnosticsRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                CustomTriggerMessageRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                CustomUpdateFirmwareRequestSerializer                  { get; set; }



        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                    CustomReserveNowRequestSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CancelReservationRequest>?             CustomCancelReservationRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?        CustomRemoteStartTransactionRequestSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                      CustomChargingProfileSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                     CustomChargingScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?               CustomChargingSchedulePeriodSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>?         CustomRemoteStopTransactionRequestSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?            CustomSetChargingProfileRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?          CustomClearChargingProfileRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?          CustomGetCompositeScheduleRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?               CustomUnlockConnectorRequestSerializer                 { get; set; }


        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?           CustomGetLocalListVersionRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                 CustomSendLocalListRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                    CustomAuthorizationDataSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                            CustomIdTagInfoResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                    CustomClearCacheRequestSerializer                      { get; set; }


        // Security extensions
        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?             CustomCertificateSignedRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?             CustomDeleteCertificateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?        CustomExtendedTriggerMessageRequestSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?    CustomGetInstalledCertificateIdsRequestSerializer      { get; set; }
        public CustomJObjectSerializerDelegate<GetLogRequest>?                        CustomGetLogRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?            CustomInstallCertificateRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?          CustomSignedUpdateFirmwareRequestSerializer            { get; set; }

        #endregion











        #region Reset                 (Request)

        public async Task<ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnResetRequest));
            }

            #endregion


            ResetResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomResetRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ResetResponse.TryParse(Request,
                                           sendRequestState.JSONResponse.Payload,
                                           out var resetResponse,
                                           out var errorResponse) &&
                    resetResponse is not null)
                {
                    response = resetResponse;
                }

                response ??= new ResetResponse(Request,
                                               Result.Format(errorResponse));

            }

            response ??= new ResetResponse(Request,
                                           Result.FromSendRequestState(sendRequestState));


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability    (Request)

        public async Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        {

            #region Send OnChangeAvailabilityRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            ChangeAvailabilityResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.NetworkingNodeId,
                                             Request.Action,
                                             Request.ToJSON(CustomChangeAvailabilityRequestSerializer),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var changeAvailabilityResponse,
                                                        out var errorResponse) &&
                    changeAvailabilityResponse is not null)
                {
                    response = changeAvailabilityResponse;
                }

                response ??= new ChangeAvailabilityResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new ChangeAvailabilityResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConfiguration      (Request)

        public async Task<GetConfigurationResponse> GetConfiguration(GetConfigurationRequest Request)
        {

            #region Send OnGetConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetConfigurationRequest?.Invoke(startTime,
                                                  this,
                                                  Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            GetConfigurationResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.NetworkingNodeId,
                                             Request.Action,
                                             Request.ToJSON(CustomGetConfigurationRequestSerializer),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetConfigurationResponse.TryParse(Request,
                                                      sendRequestState.JSONResponse.Payload,
                                                      out var getConfigurationResponse,
                                                      out var errorResponse) &&
                    getConfigurationResponse is not null)
                {
                    response = getConfigurationResponse;
                }

                response ??= new GetConfigurationResponse(Request,
                                                          Result.Format(errorResponse));

            }

            response ??= new GetConfigurationResponse(Request,
                                                      Result.FromSendRequestState(sendRequestState));


            #region Send OnGetConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetConfigurationResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeConfiguration   (Request)

        public async Task<ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request)
        {

            #region Send OnChangeConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            ChangeConfigurationResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.NetworkingNodeId,
                                             Request.Action,
                                             Request.ToJSON(CustomChangeConfigurationRequestSerializer),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ChangeConfigurationResponse.TryParse(Request,
                                                         sendRequestState.JSONResponse.Payload,
                                                         out var changeConfigurationResponse,
                                                         out var errorResponse) &&
                    changeConfigurationResponse is not null)
                {
                    response = changeConfigurationResponse;
                }

                response ??= new ChangeConfigurationResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new ChangeConfigurationResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


            #region Send OnChangeConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DataTransfer          (Request)

        public async Task<OCPP.CS.DataTransferResponse> DataTransfer(OCPP.CSMS.DataTransferRequest Request)
        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            OCPP.CS.DataTransferResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomDataTransferRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (OCPP.CS.DataTransferResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var dataTransferResponse,
                                                          out var errorResponse) &&
                    dataTransferResponse is not null)
                {
                    response = dataTransferResponse;
                }

                response ??= new OCPP.CS.DataTransferResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new OCPP.CS.DataTransferResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDiagnostics        (Request)

        public async Task<GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest Request)
        {

            #region Send OnGetDiagnosticsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            GetDiagnosticsResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetDiagnosticsRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetDiagnosticsResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var getDiagnosticsResponse,
                                                    out var errorResponse) &&
                    getDiagnosticsResponse is not null)
                {
                    response = getDiagnosticsResponse;
                }

                response ??= new GetDiagnosticsResponse(Request,
                                                        Result.Format(errorResponse));

            }

            response ??= new GetDiagnosticsResponse(Request,
                                                    Result.FromSendRequestState(sendRequestState));


            #region Send OnGetDiagnosticsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetDiagnosticsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage        (Request)

        public async Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        {

            #region Send OnTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            TriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomTriggerMessageRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (TriggerMessageResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var triggerMessageResponse,
                                                    out var errorResponse) &&
                    triggerMessageResponse is not null)
                {
                    response = triggerMessageResponse;
                }

                response ??= new TriggerMessageResponse(Request,
                                                        Result.Format(errorResponse));

            }

            response ??= new TriggerMessageResponse(Request,
                                                    Result.FromSendRequestState(sendRequestState));


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware        (Request)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomUpdateFirmwareRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var updateFirmwareResponse,
                                                    out var errorResponse) &&
                    updateFirmwareResponse is not null)
                {
                    response = updateFirmwareResponse;
                }

                response ??= new UpdateFirmwareResponse(Request,
                                                        Result.Format(errorResponse));

            }

            response ??= new UpdateFirmwareResponse(Request,
                                                    Result.FromSendRequestState(sendRequestState));


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow            (Request)

        public async Task<ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        {

            #region Send OnReserveNowRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            ReserveNowResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomReserveNowRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ReserveNowResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var reserveNowResponse,
                                                    out var errorResponse) &&
                    reserveNowResponse is not null)
                {
                    response = reserveNowResponse;
                }

                response ??= new ReserveNowResponse(Request,
                                                    Result.Format(errorResponse));

            }

            response ??= new ReserveNowResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation     (Request)

        public async Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        {

            #region Send OnCancelReservationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            CancelReservationResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomCancelReservationRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (CancelReservationResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var cancelReservationResponse,
                                                       out var errorResponse) &&
                    cancelReservationResponse is not null)
                {
                    response = cancelReservationResponse;
                }

                response ??= new CancelReservationResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new CancelReservationResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStartTransaction(Request)

        public async Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request)
        {

            #region Send OnRemoteStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            RemoteStartTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomRemoteStartTransactionRequestSerializer,
                                                         CustomChargingProfileSerializer,
                                                         CustomChargingScheduleSerializer,
                                                         CustomChargingSchedulePeriodSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (RemoteStartTransactionResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var remoteStartTransactionResponse,
                                                            out var errorResponse) &&
                    remoteStartTransactionResponse is not null)
                {
                    response = remoteStartTransactionResponse;
                }

                response ??= new RemoteStartTransactionResponse(Request,
                                                                Result.Format(errorResponse));

            }

            response ??= new RemoteStartTransactionResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));


            #region Send OnRemoteStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStopTransaction (Request)

        public async Task<RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest Request)
        {

            #region Send OnRemoteStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionRequest?.Invoke(startTime,
                                                       this,
                                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            RemoteStopTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomRemoteStopTransactionRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (RemoteStopTransactionResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var remoteStopTransactionResponse,
                                                           out var errorResponse) &&
                    remoteStopTransactionResponse is not null)
                {
                    response = remoteStopTransactionResponse;
                }

                response ??= new RemoteStopTransactionResponse(Request,
                                                               Result.Format(errorResponse));

            }

            response ??= new RemoteStopTransactionResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));


            #region Send OnRemoteStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile    (Request)

        public async Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            SetChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomSetChargingProfileRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (SetChargingProfileResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var setChargingProfileResponse,
                                                        out var errorResponse) &&
                    setChargingProfileResponse is not null)
                {
                    response = setChargingProfileResponse;
                }

                response ??= new SetChargingProfileResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new SetChargingProfileResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile  (Request)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomClearChargingProfileRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse) &&
                    clearChargingProfileResponse is not null)
                {
                    response = clearChargingProfileResponse;
                }

                response ??= new ClearChargingProfileResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new ClearChargingProfileResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule  (Request)


        public async Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        {

            #region Send OnGetCompositeScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            GetCompositeScheduleResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetCompositeScheduleRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetCompositeScheduleResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var getCompositeScheduleResponse,
                                                          out var errorResponse) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new GetCompositeScheduleResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new GetCompositeScheduleResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector       (Request)

        public async Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        {

            #region Send OnUnlockConnectorRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequest?.Invoke(startTime,
                                                 this,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            UnlockConnectorResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomUnlockConnectorRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (UnlockConnectorResponse.TryParse(Request,
                                                     sendRequestState.JSONResponse.Payload,
                                                     out var unlockConnectorResponse,
                                                     out var errorResponse) &&
                    unlockConnectorResponse is not null)
                {
                    response = unlockConnectorResponse;
                }

                response ??= new UnlockConnectorResponse(Request,
                                                         Result.Format(errorResponse));

            }

            response ??= new UnlockConnectorResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion   (Request)

        public async Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        {

            #region Send OnGetLocalListVersionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            GetLocalListVersionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetLocalListVersionRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetLocalListVersionResponse.TryParse(Request,
                                                         sendRequestState.JSONResponse.Payload,
                                                         out var getLocalListVersionResponse,
                                                         out var errorResponse) &&
                    getLocalListVersionResponse is not null)
                {
                    response = getLocalListVersionResponse;
                }

                response ??= new GetLocalListVersionResponse(Request,
                                                             Result.Format(errorResponse));

            }

            response ??= new GetLocalListVersionResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList         (Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            SendLocalListResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomSendLocalListRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (SendLocalListResponse.TryParse(Request,
                                                   sendRequestState.JSONResponse.Payload,
                                                   out var sendLocalListResponse,
                                                   out var errorResponse) &&
                    sendLocalListResponse is not null)
                {
                    response = sendLocalListResponse;
                }

                response ??= new SendLocalListResponse(Request,
                                                       Result.Format(errorResponse));

            }

            response ??= new SendLocalListResponse(Request,
                                                   Result.FromSendRequestState(sendRequestState));


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache            (Request)

        public async Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            ClearCacheResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomClearCacheRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ClearCacheResponse.TryParse(Request,
                                                sendRequestState.JSONResponse.Payload,
                                                out var clearCacheResponse,
                                                out var errorResponse) &&
                    clearCacheResponse is not null)
                {
                    response = clearCacheResponse;
                }

                response ??= new ClearCacheResponse(Request,
                                                    Result.Format(errorResponse));

            }

            response ??= new ClearCacheResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion



        // Security extensions

        #region CertificateSigned         (Request)

        /// <summary>
        /// Send the signed certificate to the charge point.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        public async Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            CertificateSignedResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomCertificateSignedRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (CertificateSignedResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var certificateSignedResponse,
                                                       out var errorResponse) &&
                    certificateSignedResponse is not null)
                {
                    response = certificateSignedResponse;
                }

                response ??= new CertificateSignedResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new CertificateSignedResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate         (Request)

        /// <summary>
        /// Delete the given certificate on the charge point.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        public async Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {

            #region Send OnDeleteCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            DeleteCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomDeleteCertificateRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (DeleteCertificateResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var deleteCertificateResponse,
                                                       out var errorResponse) &&
                    deleteCertificateResponse is not null)
                {
                    response = deleteCertificateResponse;
                }

                response ??= new DeleteCertificateResponse(Request,
                                                           Result.Format(errorResponse));

            }

            response ??= new DeleteCertificateResponse(Request,
                                                       Result.FromSendRequestState(sendRequestState));


            #region Send OnDeleteCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ExtendedTriggerMessage    (Request)

        /// <summary>
        /// Send an extended trigger message to the charge point.
        /// </summary>
        /// <param name="Request">A extended trigger message request.</param>
        public async Task<ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request)
        {

            #region Send OnExtendedTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnExtendedTriggerMessageRequest));
            }

            #endregion


            ExtendedTriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomExtendedTriggerMessageRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (ExtendedTriggerMessageResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var extendedTriggerMessageResponse,
                                                            out var errorResponse) &&
                    extendedTriggerMessageResponse is not null)
                {
                    response = extendedTriggerMessageResponse;
                }

                response ??= new ExtendedTriggerMessageResponse(Request,
                                                                Result.Format(errorResponse));

            }

            response ??= new ExtendedTriggerMessageResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));


            #region Send OnExtendedTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnExtendedTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds(Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charge point.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        public async Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {

            #region Send OnGetInstalledCertificateIdsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
                                                            this,
                                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            GetInstalledCertificateIdsResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetInstalledCertificateIdsRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetInstalledCertificateIdsResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var getInstalledCertificateIdsResponse,
                                                                out var errorResponse) &&
                    getInstalledCertificateIdsResponse is not null)
                {
                    response = getInstalledCertificateIdsResponse;
                }

                response ??= new GetInstalledCertificateIdsResponse(Request,
                                                                    Result.Format(errorResponse));

            }

            response ??= new GetInstalledCertificateIdsResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                    (Request)

        /// <summary>
        /// Retrieve log files from the charge point.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        public async Task<GetLogResponse> GetLog(GetLogRequest Request)
        {

            #region Send OnGetLogRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLogRequest?.Invoke(startTime,
                                        this,
                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            GetLogResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomGetLogRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (GetLogResponse.TryParse(Request,
                                            sendRequestState.JSONResponse.Payload,
                                            out var getLogResponse,
                                            out var errorResponse) &&
                    getLogResponse is not null)
                {
                    response = getLogResponse;
                }

                response ??= new GetLogResponse(Request,
                                                Result.Format(errorResponse));

            }

            response ??= new GetLogResponse(Request,
                                            Result.FromSendRequestState(sendRequestState));


            #region Send OnGetLogResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLogResponse?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate        (Request)

        /// <summary>
        /// Install the given certificate within the charge point.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        public async Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {

            #region Send OnInstallCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnInstallCertificateRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            InstallCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomInstallCertificateRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (InstallCertificateResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var installCertificateResponse,
                                                        out var errorResponse) &&
                    installCertificateResponse is not null)
                {
                    response = installCertificateResponse;
                }

                response ??= new InstallCertificateResponse(Request,
                                                            Result.Format(errorResponse));

            }

            response ??= new InstallCertificateResponse(Request,
                                                        Result.FromSendRequestState(sendRequestState));


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SignedUpdateFirmware      (Request)

        /// <summary>
        /// Update the firmware of the charge point.
        /// </summary>
        /// <param name="Request">A signed update firmware request.</param>
        public async Task<SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request)
        {

            #region Send OnSignedUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSignedUpdateFirmwareRequest));
            }

            #endregion


            SignedUpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.NetworkingNodeId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomSignedUpdateFirmwareRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (SignedUpdateFirmwareResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var signedUpdateFirmwareResponse,
                                                          out var errorResponse) &&
                    signedUpdateFirmwareResponse is not null)
                {
                    response = signedUpdateFirmwareResponse;
                }

                response ??= new SignedUpdateFirmwareResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new SignedUpdateFirmwareResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnSignedUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSignedUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
