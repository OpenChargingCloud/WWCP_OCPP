/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

    public interface IChargePointServer
    {

        event OnResetRequestDelegate                    OnResetRequest;
        event OnResetDelegate                           OnReset;
        event OnResetResponseDelegate                   OnResetResponse;

        event OnChangeAvailabilityRequestDelegate       OnChangeAvailabilityRequest;
        event OnChangeAvailabilityDelegate              OnChangeAvailability;
        event OnChangeAvailabilityResponseDelegate      OnChangeAvailabilityResponse;

        event OnGetConfigurationRequestDelegate         OnGetConfigurationRequest;
        event OnGetConfigurationDelegate                OnGetConfiguration;
        event OnGetConfigurationResponseDelegate        OnGetConfigurationResponse;

        event OnChangeConfigurationRequestDelegate      OnChangeConfigurationRequest;
        event OnChangeConfigurationDelegate             OnChangeConfiguration;
        event OnChangeConfigurationResponseDelegate     OnChangeConfigurationResponse;

        event OnIncomingDataTransferRequestDelegate     OnIncomingDataTransferRequest;
        event OnIncomingDataTransferDelegate            OnIncomingDataTransfer;
        event OnIncomingDataTransferResponseDelegate    OnIncomingDataTransferResponse;

        event OnGetDiagnosticsRequestDelegate           OnGetDiagnosticsRequest;
        event OnGetDiagnosticsDelegate                  OnGetDiagnostics;
        event OnGetDiagnosticsResponseDelegate          OnGetDiagnosticsResponse;

        event OnTriggerMessageRequestDelegate           OnTriggerMessageRequest;
        event OnTriggerMessageDelegate                  OnTriggerMessage;
        event OnTriggerMessageResponseDelegate          OnTriggerMessageResponse;

        event OnUpdateFirmwareRequestDelegate           OnUpdateFirmwareRequest;
        event OnUpdateFirmwareDelegate                  OnUpdateFirmware;
        event OnUpdateFirmwareResponseDelegate          OnUpdateFirmwareResponse;


        event OnReserveNowRequestDelegate               OnReserveNowRequest;
        event OnReserveNowDelegate                      OnReserveNow;
        event OnReserveNowResponseDelegate              OnReserveNowResponse;

        event OnCancelReservationRequestDelegate        OnCancelReservationRequest;
        event OnCancelReservationDelegate               OnCancelReservation;
        event OnCancelReservationResponseDelegate       OnCancelReservationResponse;

        event OnRemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;
        event OnRemoteStartTransactionDelegate          OnRemoteStartTransaction;
        event OnRemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        event OnRemoteStopTransactionRequestDelegate    OnRemoteStopTransactionRequest;
        event OnRemoteStopTransactionDelegate           OnRemoteStopTransaction;
        event OnRemoteStopTransactionResponseDelegate   OnRemoteStopTransactionResponse;

        event OnSetChargingProfileRequestDelegate       OnSetChargingProfileRequest;
        event OnSetChargingProfileDelegate              OnSetChargingProfile;
        event OnSetChargingProfileResponseDelegate      OnSetChargingProfileResponse;

        event OnClearChargingProfileRequestDelegate     OnClearChargingProfileRequest;
        event OnClearChargingProfileDelegate            OnClearChargingProfile;
        event OnClearChargingProfileResponseDelegate    OnClearChargingProfileResponse;

        event OnGetCompositeScheduleRequestDelegate     OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleDelegate            OnGetCompositeSchedule;
        event OnGetCompositeScheduleResponseDelegate    OnGetCompositeScheduleResponse;

        event OnUnlockConnectorRequestDelegate          OnUnlockConnectorRequest;
        event OnUnlockConnectorDelegate                 OnUnlockConnector;
        event OnUnlockConnectorResponseDelegate         OnUnlockConnectorResponse;



        event OnGetLocalListVersionRequestDelegate      OnGetLocalListVersionRequest;
        event OnGetLocalListVersionDelegate             OnGetLocalListVersion;
        event OnGetLocalListVersionResponseDelegate     OnGetLocalListVersionResponse;

        event OnSendLocalListRequestDelegate            OnSendLocalListRequest;
        event OnSendLocalListDelegate                   OnSendLocalList;
        event OnSendLocalListResponseDelegate           OnSendLocalListResponse;

        event OnClearCacheRequestDelegate               OnClearCacheRequest;
        event OnClearCacheDelegate                      OnClearCache;
        event OnClearCacheResponseDelegate              OnClearCacheResponse;

    }

}
