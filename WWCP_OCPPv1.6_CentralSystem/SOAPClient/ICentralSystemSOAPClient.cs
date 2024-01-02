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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    public interface ICentralSystemSOAPClient : ICSOutgoingMessages,
                                                ICSOutgoingMessagesEvents,
                                                IHTTPClient
    {

        event ClientRequestLogHandler?   OnResetSOAPRequest;
        event ClientResponseLogHandler?  OnResetSOAPResponse;

        event ClientRequestLogHandler?   OnChangeAvailabilitySOAPRequest;
        event ClientResponseLogHandler?  OnChangeAvailabilitySOAPResponse;

        event ClientRequestLogHandler?   OnGetConfigurationSOAPRequest;
        event ClientResponseLogHandler?  OnGetConfigurationSOAPResponse;

        event ClientRequestLogHandler?   OnChangeConfigurationSOAPRequest;
        event ClientResponseLogHandler?  OnChangeConfigurationSOAPResponse;

        event ClientRequestLogHandler?   OnDataTransferSOAPRequest;
        event ClientResponseLogHandler?  OnDataTransferSOAPResponse;

        event ClientRequestLogHandler?   OnGetDiagnosticsSOAPRequest;
        event ClientResponseLogHandler?  OnGetDiagnosticsSOAPResponse;

        event ClientRequestLogHandler?   OnTriggerMessageSOAPRequest;
        event ClientResponseLogHandler?  OnTriggerMessageSOAPResponse;

        event ClientRequestLogHandler?   OnUpdateFirmwareSOAPRequest;
        event ClientResponseLogHandler?  OnUpdateFirmwareSOAPResponse;


        event ClientRequestLogHandler?   OnReserveNowSOAPRequest;
        event ClientResponseLogHandler?  OnReserveNowSOAPResponse;

        event ClientRequestLogHandler?   OnCancelReservationSOAPRequest;
        event ClientResponseLogHandler?  OnCancelReservationSOAPResponse;

        event ClientRequestLogHandler?   OnRemoteStartTransactionSOAPRequest;
        event ClientResponseLogHandler?  OnRemoteStartTransactionSOAPResponse;

        event ClientRequestLogHandler?   OnRemoteStopTransactionSOAPRequest;
        event ClientResponseLogHandler?  OnRemoteStopTransactionSOAPResponse;

        event ClientRequestLogHandler?   OnSetChargingProfileSOAPRequest;
        event ClientResponseLogHandler?  OnSetChargingProfileSOAPResponse;

        event ClientRequestLogHandler?   OnClearChargingProfileSOAPRequest;
        event ClientResponseLogHandler?  OnClearChargingProfileSOAPResponse;

        event ClientRequestLogHandler?   OnGetCompositeScheduleSOAPRequest;
        event ClientResponseLogHandler?  OnGetCompositeScheduleSOAPResponse;

        event ClientRequestLogHandler?   OnUnlockConnectorSOAPRequest;
        event ClientResponseLogHandler?  OnUnlockConnectorSOAPResponse;


        event ClientRequestLogHandler?   OnGetLocalListVersionSOAPRequest;
        event ClientResponseLogHandler?  OnGetLocalListVersionSOAPResponse;

        event ClientRequestLogHandler?   OnSendLocalListSOAPRequest;
        event ClientResponseLogHandler?  OnSendLocalListSOAPResponse;

        event ClientRequestLogHandler?   OnClearCacheSOAPRequest;
        event ClientResponseLogHandler?  OnClearCacheSOAPResponse;

    }

}
