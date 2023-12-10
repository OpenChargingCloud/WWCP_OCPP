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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnResetRequest));
            }

            #endregion


            ResetResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnResetResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            ChangeAvailabilityResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnChangeAvailabilityResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            GetConfigurationResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetConfigurationResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            ChangeConfigurationResponse? response = null;

            var sendRequestState = await SendRequest(
                                             Request.RequestId,
                                             Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DataTransfer          (Request)

        public async Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest Request)
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CP.DataTransferResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
                                                     Request.Action,
                                                     Request.ToJSON(CustomDataTransferRequestSerializer),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.JSONResponse is not null)
            {

                if (CP.DataTransferResponse.TryParse(Request,
                                                     sendRequestState.JSONResponse.Payload,
                                                     out var dataTransferResponse,
                                                     out var errorResponse) &&
                    dataTransferResponse is not null)
                {
                    response = dataTransferResponse;
                }

                response ??= new CP.DataTransferResponse(Request,
                                                         Result.Format(errorResponse));

            }

            response ??= new CP.DataTransferResponse(Request,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnDataTransferResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            GetDiagnosticsResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetDiagnosticsResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            TriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnTriggerMessageResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnUpdateFirmwareResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            ReserveNowResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnReserveNowResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            CancelReservationResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnCancelReservationResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            RemoteStartTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnRemoteStartTransactionResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            RemoteStopTransactionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnRemoteStopTransactionResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            SetChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSetChargingProfileResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnClearChargingProfileResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            GetCompositeScheduleResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetCompositeScheduleResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            UnlockConnectorResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnUnlockConnectorResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            GetLocalListVersionResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetLocalListVersionResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            SendLocalListResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSendLocalListResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            ClearCacheResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnClearCacheResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            CertificateSignedResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnCertificateSignedResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            DeleteCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnDeleteCertificateResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnExtendedTriggerMessageRequest));
            }

            #endregion


            ExtendedTriggerMessageResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnExtendedTriggerMessageResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            GetInstalledCertificateIdsResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetInstalledCertificateIdsResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            GetLogResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnGetLogResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            InstallCertificateResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnInstallCertificateResponse));
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSignedUpdateFirmwareRequest));
            }

            #endregion


            SignedUpdateFirmwareResponse? response = null;

            var sendRequestState = await SendRequest(Request.RequestId,
                                                     Request.ChargeBoxId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer_old) + "." + nameof(OnSignedUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
