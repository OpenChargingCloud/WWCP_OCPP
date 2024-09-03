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

    public interface ICPIncomingMessages// : OCPP.CS.ICSIncomingMessages
    {

        event OnResetDelegate                    OnReset;
        event OnChangeAvailabilityDelegate       OnChangeAvailability;
        event OnGetConfigurationDelegate         OnGetConfiguration;
        event OnChangeConfigurationDelegate      OnChangeConfiguration;
        event OnIncomingDataTransferDelegate     OnIncomingDataTransfer;
        event OnGetDiagnosticsDelegate           OnGetDiagnostics;
        event OnTriggerMessageDelegate           OnTriggerMessage;
        event OnUpdateFirmwareDelegate           OnUpdateFirmware;

        event OnReserveNowDelegate               OnReserveNow;
        event OnCancelReservationDelegate        OnCancelReservation;
        event OnRemoteStartTransactionDelegate   OnRemoteStartTransaction;
        event OnRemoteStopTransactionDelegate    OnRemoteStopTransaction;
        event OnSetChargingProfileDelegate       OnSetChargingProfile;
        event OnClearChargingProfileDelegate     OnClearChargingProfile;
        event OnGetCompositeScheduleDelegate     OnGetCompositeSchedule;
        event OnUnlockConnectorDelegate          OnUnlockConnector;

        event OnGetLocalListVersionDelegate      OnGetLocalListVersion;
        event OnSendLocalListDelegate            OnSendLocalList;
        event OnClearCacheDelegate               OnClearCache;

    }

}
