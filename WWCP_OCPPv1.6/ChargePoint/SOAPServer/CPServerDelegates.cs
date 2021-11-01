/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Threading;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    #region OnReset

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ResetRequestDelegate(DateTime          Timestamp,
                             IEventSender      Sender,
                             CS.ResetRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ResetResponse>

        OnResetDelegate(DateTime            Timestamp,
                        IEventSender        Sender,
                        CS.ResetRequest     Request,
                        CancellationToken   CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ResetResponseDelegate(DateTime           Timestamp,
                              IEventSender       Sender,
                              CS.ResetRequest    Request,
                              CP.ResetResponse   Response,
                              TimeSpan           Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ChangeAvailabilityRequestDelegate(DateTime                       Timestamp,
                                          IEventSender                   Sender,
                                          CS.ChangeAvailabilityRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeAvailabilityResponse>

        OnChangeAvailabilityDelegate(DateTime                       Timestamp,
                                     IEventSender                   Sender,
                                     CS.ChangeAvailabilityRequest   Request,
                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ChangeAvailabilityResponseDelegate(DateTime                        Timestamp,
                                           IEventSender                    Sender,
                                           CS.ChangeAvailabilityRequest    Request,
                                           CP.ChangeAvailabilityResponse   Response,
                                           TimeSpan                        Runtime);

    #endregion

    #region OnGetConfiguration

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        GetConfigurationRequestDelegate(DateTime                     Timestamp,
                                        IEventSender                 Sender,
                                        CS.GetConfigurationRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetConfigurationResponse>

        OnGetConfigurationDelegate(DateTime                     Timestamp,
                                   IEventSender                 Sender,
                                   CS.GetConfigurationRequest   Request,
                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        GetConfigurationResponseDelegate(DateTime                      Timestamp,
                                         IEventSender                  Sender,
                                         CS.GetConfigurationRequest    Request,
                                         CP.GetConfigurationResponse   Response,
                                         TimeSpan                      Runtime);

    #endregion

    #region OnChangeConfiguration

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ChangeConfigurationRequestDelegate(DateTime                        Timestamp,
                                           IEventSender                    Sender,
                                           CS.ChangeConfigurationRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeConfigurationResponse>

        OnChangeConfigurationDelegate(DateTime                        Timestamp,
                                      IEventSender                    Sender,
                                      CS.ChangeConfigurationRequest   Request,
                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ChangeConfigurationResponseDelegate(DateTime                         Timestamp,
                                            IEventSender                     Sender,
                                            CS.ChangeConfigurationRequest    Request,
                                            CP.ChangeConfigurationResponse   Response,
                                            TimeSpan                         Runtime);

    #endregion

    #region OnIncomingDataTransfer

    /// <summary>
    /// A data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        IncomingDataTransferRequestDelegate(DateTime                 Timestamp,
                                            IEventSender             Sender,
                                            CS.DataTransferRequest   Request);


    /// <summary>
    /// A data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                 Timestamp,
                                       IEventSender             Sender,
                                       CS.DataTransferRequest   Request,
                                       CancellationToken        CancellationToken);


    /// <summary>
    /// A data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        IncomingDataTransferResponseDelegate(DateTime                  Timestamp,
                                             IEventSender              Sender,
                                             CS.DataTransferRequest    Request,
                                             CP.DataTransferResponse   Response,
                                             TimeSpan                  Runtime);

    #endregion

    #region OnGetDiagnostics

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        GetDiagnosticsRequestDelegate(DateTime                   Timestamp,
                                      IEventSender               Sender,
                                      CS.GetDiagnosticsRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetDiagnosticsResponse>

        OnGetDiagnosticsDelegate(DateTime                   Timestamp,
                                 IEventSender               Sender,
                                 CS.GetDiagnosticsRequest   Request,
                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        GetDiagnosticsResponseDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       CS.GetDiagnosticsRequest    Request,
                                       CP.GetDiagnosticsResponse   Response,
                                       TimeSpan                    Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        TriggerMessageRequestDelegate(DateTime                   Timestamp,
                                      IEventSender               Sender,
                                      CS.TriggerMessageRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<TriggerMessageResponse>

        OnTriggerMessageDelegate(DateTime                   Timestamp,
                                 IEventSender               Sender,
                                 CS.TriggerMessageRequest   Request,
                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        TriggerMessageResponseDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       CS.TriggerMessageRequest    Request,
                                       CP.TriggerMessageResponse   Response,
                                       TimeSpan                    Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        UpdateFirmwareRequestDelegate(DateTime                   Timestamp,
                                      IEventSender               Sender,
                                      CS.UpdateFirmwareRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateFirmwareResponse>

        OnUpdateFirmwareDelegate(DateTime                   Timestamp,
                                 IEventSender               Sender,
                                 CS.UpdateFirmwareRequest   Request,
                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        UpdateFirmwareResponseDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       CS.UpdateFirmwareRequest    Request,
                                       CP.UpdateFirmwareResponse   Response,
                                       TimeSpan                    Runtime);

    #endregion


    #region OnReserveNow

    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ReserveNowRequestDelegate(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  CS.ReserveNowRequest   Request);


    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReserveNowResponse>

        OnReserveNowDelegate(DateTime               Timestamp,
                             IEventSender           Sender,
                             CS.ReserveNowRequest   Request,
                             CancellationToken      CancellationToken);


    /// <summary>
    /// A reserve now response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ReserveNowResponseDelegate(DateTime                Timestamp,
                                   IEventSender            Sender,
                                   CS.ReserveNowRequest    Request,
                                   CP.ReserveNowResponse   Response,
                                   TimeSpan                Runtime);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    public delegate Task

        CancelReservationRequestDelegate(DateTime                      Timestamp,
                                         IEventSender                  Sender,
                                         CS.CancelReservationRequest   Request);


    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CancelReservationResponse>

        OnCancelReservationDelegate(DateTime                      Timestamp,
                                    IEventSender                  Sender,
                                    CS.CancelReservationRequest   Request,
                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="Response">The cancel reservation response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        CancelReservationResponseDelegate(DateTime                       Timestamp,
                                          IEventSender                   Sender,
                                          CS.CancelReservationRequest    Request,
                                          CP.CancelReservationResponse   Response,
                                          TimeSpan                       Runtime);

    #endregion

    #region OnRemoteStartTransaction

    /// <summary>
    /// A remote start transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote start transaction request.</param>
    public delegate Task

        RemoteStartTransactionRequestDelegate(DateTime                           Timestamp,
                                              IEventSender                       Sender,
                                              CS.RemoteStartTransactionRequest   Request);


    /// <summary>
    /// A remote start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote start transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RemoteStartTransactionResponse>

        OnRemoteStartTransactionDelegate(DateTime                           Timestamp,
                                         IEventSender                       Sender,
                                         CS.RemoteStartTransactionRequest   Request,
                                         CancellationToken                  CancellationToken);


    /// <summary>
    /// A remote start transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The remote start transaction request.</param>
    /// <param name="Response">The remote start transaction response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        RemoteStartTransactionResponseDelegate(DateTime                            Timestamp,
                                               IEventSender                        Sender,
                                               CS.RemoteStartTransactionRequest    Request,
                                               CP.RemoteStartTransactionResponse   Response,
                                               TimeSpan                            Runtime);

    #endregion

    #region OnRemoteStopTransaction

    /// <summary>
    /// A remote stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    public delegate Task

        RemoteStopTransactionRequestDelegate(DateTime                          Timestamp,
                                             IEventSender                      Sender,
                                             CS.RemoteStopTransactionRequest   Request);


    /// <summary>
    /// A remote stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RemoteStopTransactionResponse>

        OnRemoteStopTransactionDelegate(DateTime                          Timestamp,
                                        IEventSender                      Sender,
                                        CS.RemoteStopTransactionRequest   Request,
                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A remote stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    /// <param name="Response">The remote stop transaction response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        RemoteStopTransactionResponseDelegate(DateTime                           Timestamp,
                                              IEventSender                       Sender,
                                              CS.RemoteStopTransactionRequest    Request,
                                              CP.RemoteStopTransactionResponse   Response,
                                              TimeSpan                           Runtime);

    #endregion

    #region OnSetChargingProfile

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        SetChargingProfileRequestDelegate(DateTime                       Timestamp,
                                          IEventSender                   Sender,
                                          CS.SetChargingProfileRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetChargingProfileResponse>

        OnSetChargingProfileDelegate(DateTime                       Timestamp,
                                     IEventSender                   Sender,
                                     CS.SetChargingProfileRequest   Request,
                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        SetChargingProfileResponseDelegate(DateTime                        Timestamp,
                                           IEventSender                    Sender,
                                           CS.SetChargingProfileRequest    Request,
                                           CP.SetChargingProfileResponse   Response,
                                           TimeSpan                        Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ClearChargingProfileRequestDelegate(DateTime                         Timestamp,
                                            IEventSender                     Sender,
                                            CS.ClearChargingProfileRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearChargingProfileResponse>

        OnClearChargingProfileDelegate(DateTime                         Timestamp,
                                       IEventSender                     Sender,
                                       CS.ClearChargingProfileRequest   Request,
                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ClearChargingProfileResponseDelegate(DateTime                          Timestamp,
                                             IEventSender                      Sender,
                                             CS.ClearChargingProfileRequest    Request,
                                             CP.ClearChargingProfileResponse   Response,
                                             TimeSpan                          Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        GetCompositeScheduleRequestDelegate(DateTime                         Timestamp,
                                            IEventSender                     Sender,
                                            CS.GetCompositeScheduleRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCompositeScheduleResponse>

        OnGetCompositeScheduleDelegate(DateTime                         Timestamp,
                                       IEventSender                     Sender,
                                       CS.GetCompositeScheduleRequest   Request,
                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        GetCompositeScheduleResponseDelegate(DateTime                          Timestamp,
                                             IEventSender                      Sender,
                                             CS.GetCompositeScheduleRequest    Request,
                                             CP.GetCompositeScheduleResponse   Response,
                                             TimeSpan                          Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        UnlockConnectorRequestDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       CS.UnlockConnectorRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UnlockConnectorResponse>

        OnUnlockConnectorDelegate(DateTime                    Timestamp,
                                  IEventSender                Sender,
                                  CS.UnlockConnectorRequest   Request,
                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        UnlockConnectorResponseDelegate(DateTime                     Timestamp,
                                        IEventSender                 Sender,
                                        CS.UnlockConnectorRequest    Request,
                                        CP.UnlockConnectorResponse   Response,
                                        TimeSpan                     Runtime);

    #endregion


    #region OnGetLocalListVersion

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        GetLocalListVersionRequestDelegate(DateTime                        Timestamp,
                                           IEventSender                    Sender,
                                           CS.GetLocalListVersionRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetLocalListVersionResponse>

        OnGetLocalListVersionDelegate(DateTime                        Timestamp,
                                      IEventSender                    Sender,
                                      CS.GetLocalListVersionRequest   Request,
                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        GetLocalListVersionResponseDelegate(DateTime                         Timestamp,
                                            IEventSender                     Sender,
                                            CS.GetLocalListVersionRequest    Request,
                                            CP.GetLocalListVersionResponse   Response,
                                            TimeSpan                         Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        SendLocalListRequestDelegate(DateTime                  Timestamp,
                                     IEventSender              Sender,
                                     CS.SendLocalListRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendLocalListResponse>

        OnSendLocalListDelegate(DateTime                  Timestamp,
                                IEventSender              Sender,
                                CS.SendLocalListRequest   Request,
                                CancellationToken         CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        SendLocalListResponseDelegate(DateTime                   Timestamp,
                                      IEventSender               Sender,
                                      CS.SendLocalListRequest    Request,
                                      CP.SendLocalListResponse   Response,
                                      TimeSpan                   Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        ClearCacheRequestDelegate(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  CS.ClearCacheRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearCacheResponse>

        OnClearCacheDelegate(DateTime               Timestamp,
                             IEventSender           Sender,
                             CS.ClearCacheRequest   Request,
                             CancellationToken      CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        ClearCacheResponseDelegate(DateTime                Timestamp,
                                   IEventSender            Sender,
                                   CS.ClearCacheRequest    Request,
                                   CP.ClearCacheResponse   Response,
                                   TimeSpan                Runtime);

    #endregion

}
