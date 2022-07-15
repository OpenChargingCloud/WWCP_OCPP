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

    public interface IChargePointServerEvents
    {

        event ResetRequestDelegate                    OnResetRequest;
        event OnResetDelegate                           OnReset;
        event ResetResponseDelegate                   OnResetResponse;

        event ChangeAvailabilityRequestDelegate       OnChangeAvailabilityRequest;
        event OnChangeAvailabilityDelegate              OnChangeAvailability;
        event ChangeAvailabilityResponseDelegate      OnChangeAvailabilityResponse;

        event GetConfigurationRequestDelegate         OnGetConfigurationRequest;
        event OnGetConfigurationDelegate                OnGetConfiguration;
        event GetConfigurationResponseDelegate        OnGetConfigurationResponse;

        event ChangeConfigurationRequestDelegate      OnChangeConfigurationRequest;
        event OnChangeConfigurationDelegate             OnChangeConfiguration;
        event ChangeConfigurationResponseDelegate     OnChangeConfigurationResponse;

        event IncomingDataTransferRequestDelegate     OnIncomingDataTransferRequest;
        event OnIncomingDataTransferDelegate            OnIncomingDataTransfer;
        event IncomingDataTransferResponseDelegate    OnIncomingDataTransferResponse;

        event GetDiagnosticsRequestDelegate           OnGetDiagnosticsRequest;
        event OnGetDiagnosticsDelegate                  OnGetDiagnostics;
        event GetDiagnosticsResponseDelegate          OnGetDiagnosticsResponse;

        event TriggerMessageRequestDelegate           OnTriggerMessageRequest;
        event OnTriggerMessageDelegate                  OnTriggerMessage;
        event TriggerMessageResponseDelegate          OnTriggerMessageResponse;

        event UpdateFirmwareRequestDelegate           OnUpdateFirmwareRequest;
        event OnUpdateFirmwareDelegate                  OnUpdateFirmware;
        event UpdateFirmwareResponseDelegate          OnUpdateFirmwareResponse;


        event ReserveNowRequestDelegate               OnReserveNowRequest;
        event OnReserveNowDelegate                      OnReserveNow;
        event ReserveNowResponseDelegate              OnReserveNowResponse;

        event CancelReservationRequestDelegate        OnCancelReservationRequest;
        event OnCancelReservationDelegate               OnCancelReservation;
        event CancelReservationResponseDelegate       OnCancelReservationResponse;

        event RemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;
        event OnRemoteStartTransactionDelegate          OnRemoteStartTransaction;
        event RemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        event RemoteStopTransactionRequestDelegate    OnRemoteStopTransactionRequest;
        event OnRemoteStopTransactionDelegate           OnRemoteStopTransaction;
        event RemoteStopTransactionResponseDelegate   OnRemoteStopTransactionResponse;

        event SetChargingProfileRequestDelegate       OnSetChargingProfileRequest;
        event OnSetChargingProfileDelegate              OnSetChargingProfile;
        event SetChargingProfileResponseDelegate      OnSetChargingProfileResponse;

        event ClearChargingProfileRequestDelegate     OnClearChargingProfileRequest;
        event OnClearChargingProfileDelegate            OnClearChargingProfile;
        event ClearChargingProfileResponseDelegate    OnClearChargingProfileResponse;

        event GetCompositeScheduleRequestDelegate     OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleDelegate            OnGetCompositeSchedule;
        event GetCompositeScheduleResponseDelegate    OnGetCompositeScheduleResponse;

        event UnlockConnectorRequestDelegate          OnUnlockConnectorRequest;
        event OnUnlockConnectorDelegate                 OnUnlockConnector;
        event UnlockConnectorResponseDelegate         OnUnlockConnectorResponse;



        event GetLocalListVersionRequestDelegate      OnGetLocalListVersionRequest;
        event OnGetLocalListVersionDelegate             OnGetLocalListVersion;
        event GetLocalListVersionResponseDelegate     OnGetLocalListVersionResponse;

        event SendLocalListRequestDelegate            OnSendLocalListRequest;
        event OnSendLocalListDelegate                   OnSendLocalList;
        event SendLocalListResponseDelegate           OnSendLocalListResponse;

        event ClearCacheRequestDelegate               OnClearCacheRequest;
        event OnClearCacheDelegate                      OnClearCache;
        event ClearCacheResponseDelegate              OnClearCacheResponse;

    }

}
