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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    public interface ICPIncomingMessagesEvents : OCPP.CS.ICSIncomingMessagesEvents
    {

        event OnResetRequestDelegate                     OnResetRequest;
        event OnResetResponseDelegate                    OnResetResponse;

        event OnChangeAvailabilityRequestDelegate        OnChangeAvailabilityRequest;
        event OnChangeAvailabilityResponseDelegate       OnChangeAvailabilityResponse;

        event OnGetConfigurationRequestDelegate          OnGetConfigurationRequest;
        event OnGetConfigurationResponseDelegate         OnGetConfigurationResponse;

        event OnChangeConfigurationRequestDelegate       OnChangeConfigurationRequest;
        event OnChangeConfigurationResponseDelegate      OnChangeConfigurationResponse;

        event OnIncomingDataTransferRequestDelegate      OnIncomingDataTransferRequest;
        event OnIncomingDataTransferResponseDelegate     OnIncomingDataTransferResponse;

        event OnGetDiagnosticsRequestDelegate            OnGetDiagnosticsRequest;
        event OnGetDiagnosticsResponseDelegate           OnGetDiagnosticsResponse;

        event OnTriggerMessageRequestDelegate            OnTriggerMessageRequest;
        event OnTriggerMessageResponseDelegate           OnTriggerMessageResponse;

        event OnUpdateFirmwareRequestDelegate            OnUpdateFirmwareRequest;
        event OnUpdateFirmwareResponseDelegate           OnUpdateFirmwareResponse;


        event OnReserveNowRequestDelegate                OnReserveNowRequest;
        event OnReserveNowResponseDelegate               OnReserveNowResponse;

        event OnCancelReservationRequestDelegate         OnCancelReservationRequest;
        event OnCancelReservationResponseDelegate        OnCancelReservationResponse;

        event OnRemoteStartTransactionRequestDelegate    OnRemoteStartTransactionRequest;
        event OnRemoteStartTransactionResponseDelegate   OnRemoteStartTransactionResponse;

        event OnRemoteStopTransactionRequestDelegate     OnRemoteStopTransactionRequest;
        event OnRemoteStopTransactionResponseDelegate    OnRemoteStopTransactionResponse;

        event OnSetChargingProfileRequestDelegate        OnSetChargingProfileRequest;
        event OnSetChargingProfileResponseDelegate       OnSetChargingProfileResponse;

        event OnClearChargingProfileRequestDelegate      OnClearChargingProfileRequest;
        event OnClearChargingProfileResponseDelegate     OnClearChargingProfileResponse;

        event OnGetCompositeScheduleRequestDelegate      OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleResponseDelegate     OnGetCompositeScheduleResponse;

        event OnUnlockConnectorRequestDelegate           OnUnlockConnectorRequest;
        event OnUnlockConnectorResponseDelegate          OnUnlockConnectorResponse;



        event OnGetLocalListVersionRequestDelegate       OnGetLocalListVersionRequest;
        event OnGetLocalListVersionResponseDelegate      OnGetLocalListVersionResponse;

        event OnSendLocalListRequestDelegate             OnSendLocalListRequest;
        event OnSendLocalListResponseDelegate            OnSendLocalListResponse;

        event OnClearCacheRequestDelegate                OnClearCacheRequest;
        event OnClearCacheResponseDelegate               OnClearCacheResponse;

    }

}
