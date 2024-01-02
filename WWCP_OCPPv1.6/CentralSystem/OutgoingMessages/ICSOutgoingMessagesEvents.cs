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

using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The common interface of all OCPP v1.6 central system clients.
    /// </summary>
    public interface ICSOutgoingMessagesEvents : OCPP.ICSMSOutgoingMessagesEvents
    {

        #region Reset                          (Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to a charge box.
        /// </summary>
        event OnResetRequestDelegate?                           OnResetRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnResetResponseDelegate?                          OnResetResponse;

        #endregion

        #region ChangeAvailability             (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to a charge box.
        /// </summary>
        event OnChangeAvailabilityRequestDelegate?              OnChangeAvailabilityRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        event OnChangeAvailabilityResponseDelegate?             OnChangeAvailabilityResponse;

        #endregion

        #region GetConfiguration               (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetConfiguration request will be sent to a charge box.
        /// </summary>
        event OnGetConfigurationRequestDelegate?                OnGetConfigurationRequest;

        /// <summary>
        /// An event fired whenever a response to a GetConfiguration request was received.
        /// </summary>
        event OnGetConfigurationResponseDelegate?               OnGetConfigurationResponse;

        #endregion

        #region ChangeConfiguration            (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeConfiguration request will be sent to a charge box.
        /// </summary>
        event OnChangeConfigurationRequestDelegate?             OnChangeConfigurationRequest;

        /// <summary>
        /// An event fired whenever a response to a ChangeConfiguration request was received.
        /// </summary>
        event OnChangeConfigurationResponseDelegate?            OnChangeConfigurationResponse;

        #endregion

        #region DataTransfer                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to a charge box.
        /// </summary>
        event OnDataTransferRequestDelegate?                    OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate?                   OnDataTransferResponse;

        #endregion

        #region GetDiagnostics                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDiagnostics request will be sent to a charge box.
        /// </summary>
        event OnGetDiagnosticsRequestDelegate?                  OnGetDiagnosticsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetDiagnostics request was received.
        /// </summary>
        event OnGetDiagnosticsResponseDelegate?                 OnGetDiagnosticsResponse;

        #endregion

        #region TriggerMessage                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to a charge box.
        /// </summary>
        event OnTriggerMessageRequestDelegate?                  OnTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        event OnTriggerMessageResponseDelegate?                 OnTriggerMessageResponse;

        #endregion

        #region UpdateFirmware                 (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to a charge box.
        /// </summary>
        event OnUpdateFirmwareRequestDelegate?                  OnUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        event OnUpdateFirmwareResponseDelegate?                 OnUpdateFirmwareResponse;

        #endregion


        #region ReserveNow                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to a charge box.
        /// </summary>
        event OnReserveNowRequestDelegate?                      OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        event OnReserveNowResponseDelegate?                     OnReserveNowResponse;

        #endregion

        #region CancelReservation              (Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to a charge box.
        /// </summary>
        event OnCancelReservationRequestDelegate?               OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        event OnCancelReservationResponseDelegate?              OnCancelReservationResponse;

        #endregion

        #region RemoteStartTransaction         (Request/-Response)

        /// <summary>
        /// An event fired whenever a RemoteStartTransaction request will be sent to a charge box.
        /// </summary>
        event OnRemoteStartTransactionRequestDelegate?          OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RemoteStartTransaction request was received.
        /// </summary>
        event OnRemoteStartTransactionResponseDelegate?         OnRemoteStartTransactionResponse;

        #endregion

        #region RemoteStopTransaction          (Request/-Response)

        /// <summary>
        /// An event fired whenever a RemoteStopTransaction request will be sent to a charge box.
        /// </summary>
        event OnRemoteStopTransactionRequestDelegate?           OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a response to a RemoteStopTransaction request was received.
        /// </summary>
        event OnRemoteStopTransactionResponseDelegate?          OnRemoteStopTransactionResponse;

        #endregion

        #region SetChargingProfile             (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to a charge box.
        /// </summary>
        event OnSetChargingProfileRequestDelegate?              OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        event OnSetChargingProfileResponseDelegate?             OnSetChargingProfileResponse;

        #endregion

        #region ClearChargingProfile           (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to a charge box.
        /// </summary>
        event OnClearChargingProfileRequestDelegate?            OnClearChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        event OnClearChargingProfileResponseDelegate?           OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeSchedule           (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to a charge box.
        /// </summary>
        event OnGetCompositeScheduleRequestDelegate?            OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        event OnGetCompositeScheduleResponseDelegate?           OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnector                (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to a charge box.
        /// </summary>
        event OnUnlockConnectorRequestDelegate?                 OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        event OnUnlockConnectorResponseDelegate?                OnUnlockConnectorResponse;

        #endregion


        #region GetLocalListVersion            (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to a charge box.
        /// </summary>
        event OnGetLocalListVersionRequestDelegate?             OnGetLocalListVersionRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        event OnGetLocalListVersionResponseDelegate?            OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalList                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to a charge box.
        /// </summary>
        event OnSendLocalListRequestDelegate?                   OnSendLocalListRequest;

        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        event OnSendLocalListResponseDelegate?                  OnSendLocalListResponse;

        #endregion

        #region ClearCache                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to a charge box.
        /// </summary>
        event OnClearCacheRequestDelegate?                      OnClearCacheRequest;

        /// <summary>
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        event OnClearCacheResponseDelegate?                     OnClearCacheResponse;

        #endregion



        // Security extensions

        #region CertificateSigned              (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to a charge box.
        /// </summary>
        event OnCertificateSignedRequestDelegate?               OnCertificateSignedRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        event OnCertificateSignedResponseDelegate?              OnCertificateSignedResponse;

        #endregion

        #region DeleteCertificate              (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to a charge box.
        /// </summary>
        event OnDeleteCertificateRequestDelegate?               OnDeleteCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        event OnDeleteCertificateResponseDelegate?              OnDeleteCertificateResponse;

        #endregion

        #region ExtendedTriggerMessage         (Request/-Response)

        /// <summary>
        /// An event fired whenever an ExtendedTriggerMessage request will be sent to a charge box.
        /// </summary>
        event OnExtendedTriggerMessageRequestDelegate?          OnExtendedTriggerMessageRequest;

        /// <summary>
        /// An event fired whenever a response to an ExtendedTriggerMessage request was received.
        /// </summary>
        event OnExtendedTriggerMessageResponseDelegate?         OnExtendedTriggerMessageResponse;

        #endregion

        #region GetInstalledCertificateIds     (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to a charge box.
        /// </summary>
        event OnGetInstalledCertificateIdsRequestDelegate?      OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        event OnGetInstalledCertificateIdsResponseDelegate?     OnGetInstalledCertificateIdsResponse;

        #endregion

        #region GetLog                         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to a charge box.
        /// </summary>
        event OnGetLogRequestDelegate?                          OnGetLogRequest;

        /// <summary>
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        event OnGetLogResponseDelegate?                         OnGetLogResponse;

        #endregion

        #region InstallCertificate             (Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to a charge box.
        /// </summary>
        event OnInstallCertificateRequestDelegate?              OnInstallCertificateRequest;

        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        event OnInstallCertificateResponseDelegate?             OnInstallCertificateResponse;

        #endregion

        #region SignedUpdateFirmware           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedUpdateFirmware request will be sent to a charge box.
        /// </summary>
        event OnSignedUpdateFirmwareRequestDelegate?            OnSignedUpdateFirmwareRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedUpdateFirmware request was received.
        /// </summary>
        event OnSignedUpdateFirmwareResponseDelegate?           OnSignedUpdateFirmwareResponse;

        #endregion


    }

}
