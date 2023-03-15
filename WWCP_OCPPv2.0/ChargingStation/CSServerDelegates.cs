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

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    #region OnReset

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnResetRequestDelegate(DateTime          Timestamp,
                               IEventSender      Sender,
                               CSMS.ResetRequest   Request);


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
                        CSMS.ResetRequest     Request,
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

        OnResetResponseDelegate(DateTime           Timestamp,
                                IEventSender       Sender,
                                CSMS.ResetRequest    Request,
                                CS.ResetResponse   Response,
                                TimeSpan           Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnChangeAvailabilityRequestDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.ChangeAvailabilityRequest   Request);


    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeAvailabilityResponse>

        OnChangeAvailabilityDelegate(DateTime                       Timestamp,
                                     IEventSender                   Sender,
                                     CSMS.ChangeAvailabilityRequest   Request,
                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A change availability response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnChangeAvailabilityResponseDelegate(DateTime                        Timestamp,
                                             IEventSender                    Sender,
                                             CSMS.ChangeAvailabilityRequest    Request,
                                             CS.ChangeAvailabilityResponse   Response,
                                             TimeSpan                        Runtime);

    #endregion

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                 Timestamp,
                                              IEventSender             Sender,
                                              CSMS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                 Timestamp,
                                       IEventSender             Sender,
                                       CSMS.DataTransferRequest   Request,
                                       CancellationToken        CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                  Timestamp,
                                               IEventSender              Sender,
                                               CSMS.DataTransferRequest    Request,
                                               CS.DataTransferResponse   Response,
                                               TimeSpan                  Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnTriggerMessageRequestDelegate(DateTime                   Timestamp,
                                        IEventSender               Sender,
                                        CSMS.TriggerMessageRequest   Request);


    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<TriggerMessageResponse>

        OnTriggerMessageDelegate(DateTime                   Timestamp,
                                 IEventSender               Sender,
                                 CSMS.TriggerMessageRequest   Request,
                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A trigger message response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnTriggerMessageResponseDelegate(DateTime                    Timestamp,
                                         IEventSender                Sender,
                                         CSMS.TriggerMessageRequest    Request,
                                         CS.TriggerMessageResponse   Response,
                                         TimeSpan                    Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateFirmwareRequestDelegate(DateTime                   Timestamp,
                                        IEventSender               Sender,
                                        CSMS.UpdateFirmwareRequest   Request);


    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateFirmwareResponse>

        OnUpdateFirmwareDelegate(DateTime                   Timestamp,
                                 IEventSender               Sender,
                                 CSMS.UpdateFirmwareRequest   Request,
                                 CancellationToken          CancellationToken);


    /// <summary>
    /// An update firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateFirmwareResponseDelegate(DateTime                    Timestamp,
                                         IEventSender                Sender,
                                         CSMS.UpdateFirmwareRequest    Request,
                                         CS.UpdateFirmwareResponse   Response,
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

        OnReserveNowRequestDelegate(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    CSMS.ReserveNowRequest   Request);


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
                             CSMS.ReserveNowRequest   Request,
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

        OnReserveNowResponseDelegate(DateTime                Timestamp,
                                     IEventSender            Sender,
                                     CSMS.ReserveNowRequest    Request,
                                     CS.ReserveNowResponse   Response,
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

        OnCancelReservationRequestDelegate(DateTime                      Timestamp,
                                           IEventSender                  Sender,
                                           CSMS.CancelReservationRequest   Request);


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
                                    CSMS.CancelReservationRequest   Request,
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

        OnCancelReservationResponseDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.CancelReservationRequest    Request,
                                            CS.CancelReservationResponse   Response,
                                            TimeSpan                       Runtime);

    #endregion

    #region OnSetChargingProfile

    /// <summary>
    /// A set charging profile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetChargingProfileRequestDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.SetChargingProfileRequest   Request);


    /// <summary>
    /// A set charging profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetChargingProfileResponse>

        OnSetChargingProfileDelegate(DateTime                       Timestamp,
                                     IEventSender                   Sender,
                                     CSMS.SetChargingProfileRequest   Request,
                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A set charging profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetChargingProfileResponseDelegate(DateTime                        Timestamp,
                                             IEventSender                    Sender,
                                             CSMS.SetChargingProfileRequest    Request,
                                             CS.SetChargingProfileResponse   Response,
                                             TimeSpan                        Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A clear charging profile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearChargingProfileRequestDelegate(DateTime                         Timestamp,
                                              IEventSender                     Sender,
                                              CSMS.ClearChargingProfileRequest   Request);


    /// <summary>
    /// A clear charging profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearChargingProfileResponse>

        OnClearChargingProfileDelegate(DateTime                         Timestamp,
                                       IEventSender                     Sender,
                                       CSMS.ClearChargingProfileRequest   Request,
                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A clear charging profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearChargingProfileResponseDelegate(DateTime                          Timestamp,
                                               IEventSender                      Sender,
                                               CSMS.ClearChargingProfileRequest    Request,
                                               CS.ClearChargingProfileResponse   Response,
                                               TimeSpan                          Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A get composite schedule request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetCompositeScheduleRequestDelegate(DateTime                         Timestamp,
                                              IEventSender                     Sender,
                                              CSMS.GetCompositeScheduleRequest   Request);


    /// <summary>
    /// A get composite schedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCompositeScheduleResponse>

        OnGetCompositeScheduleDelegate(DateTime                         Timestamp,
                                       IEventSender                     Sender,
                                       CSMS.GetCompositeScheduleRequest   Request,
                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A get composite schedule response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetCompositeScheduleResponseDelegate(DateTime                          Timestamp,
                                               IEventSender                      Sender,
                                               CSMS.GetCompositeScheduleRequest    Request,
                                               CS.GetCompositeScheduleResponse   Response,
                                               TimeSpan                          Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// An unlock connector request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUnlockConnectorRequestDelegate(DateTime                    Timestamp,
                                         IEventSender                Sender,
                                         CSMS.UnlockConnectorRequest   Request);


    /// <summary>
    /// An unlock connector request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UnlockConnectorResponse>

        OnUnlockConnectorDelegate(DateTime                    Timestamp,
                                  IEventSender                Sender,
                                  CSMS.UnlockConnectorRequest   Request,
                                  CancellationToken           CancellationToken);


    /// <summary>
    /// An unlock connector response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUnlockConnectorResponseDelegate(DateTime                     Timestamp,
                                          IEventSender                 Sender,
                                          CSMS.UnlockConnectorRequest    Request,
                                          CS.UnlockConnectorResponse   Response,
                                          TimeSpan                     Runtime);

    #endregion


    #region OnGetLocalListVersion

    /// <summary>
    /// A get local list request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetLocalListVersionRequestDelegate(DateTime                        Timestamp,
                                             IEventSender                    Sender,
                                             CSMS.GetLocalListVersionRequest   Request);


    /// <summary>
    /// A get local list request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetLocalListVersionResponse>

        OnGetLocalListVersionDelegate(DateTime                        Timestamp,
                                      IEventSender                    Sender,
                                      CSMS.GetLocalListVersionRequest   Request,
                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A get local list response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetLocalListVersionResponseDelegate(DateTime                         Timestamp,
                                              IEventSender                     Sender,
                                              CSMS.GetLocalListVersionRequest    Request,
                                              CS.GetLocalListVersionResponse   Response,
                                              TimeSpan                         Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSendLocalListRequestDelegate(DateTime                  Timestamp,
                                       IEventSender              Sender,
                                       CSMS.SendLocalListRequest   Request);


    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendLocalListResponse>

        OnSendLocalListDelegate(DateTime                  Timestamp,
                                IEventSender              Sender,
                                CSMS.SendLocalListRequest   Request,
                                CancellationToken         CancellationToken);


    /// <summary>
    /// A send local list response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSendLocalListResponseDelegate(DateTime                   Timestamp,
                                        IEventSender               Sender,
                                        CSMS.SendLocalListRequest    Request,
                                        CS.SendLocalListResponse   Response,
                                        TimeSpan                   Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearCacheRequestDelegate(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    CSMS.ClearCacheRequest   Request);


    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearCacheResponse>

        OnClearCacheDelegate(DateTime               Timestamp,
                             IEventSender           Sender,
                             CSMS.ClearCacheRequest   Request,
                             CancellationToken      CancellationToken);


    /// <summary>
    /// A clear cache response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearCacheResponseDelegate(DateTime                Timestamp,
                                     IEventSender            Sender,
                                     CSMS.ClearCacheRequest    Request,
                                     CS.ClearCacheResponse   Response,
                                     TimeSpan                Runtime);

    #endregion


    #region OnCertificateSigned

    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCertificateSignedRequestDelegate(DateTime                      Timestamp,
                                           IEventSender                  Sender,
                                           CSMS.CertificateSignedRequest   Request);


    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CertificateSignedResponse>

        OnCertificateSignedDelegate(DateTime                      Timestamp,
                                    IEventSender                  Sender,
                                    CSMS.CertificateSignedRequest   Request,
                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A clear cache response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCertificateSignedResponseDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.CertificateSignedRequest    Request,
                                            CS.CertificateSignedResponse   Response,
                                            TimeSpan                       Runtime);

    #endregion

    #region OnDeleteCertificate

    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnDeleteCertificateRequestDelegate(DateTime                      Timestamp,
                                           IEventSender                  Sender,
                                           CSMS.DeleteCertificateRequest   Request);


    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DeleteCertificateResponse>

        OnDeleteCertificateDelegate(DateTime                      Timestamp,
                                    IEventSender                  Sender,
                                    CSMS.DeleteCertificateRequest   Request,
                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A delete certificate response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnDeleteCertificateResponseDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.DeleteCertificateRequest    Request,
                                            CS.DeleteCertificateResponse   Response,
                                            TimeSpan                       Runtime);

    #endregion

    #region OnGetInstalledCertificateIds

    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestDelegate(DateTime                               Timestamp,
                                                    IEventSender                           Sender,
                                                    CSMS.GetInstalledCertificateIdsRequest   Request);


    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetInstalledCertificateIdsResponse>

        OnGetInstalledCertificateIdsDelegate(DateTime                               Timestamp,
                                             IEventSender                           Sender,
                                             CSMS.GetInstalledCertificateIdsRequest   Request,
                                             CancellationToken                      CancellationToken);


    /// <summary>
    /// A get installed certificate ids response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsResponseDelegate(DateTime                                Timestamp,
                                                     IEventSender                            Sender,
                                                     CSMS.GetInstalledCertificateIdsRequest    Request,
                                                     CS.GetInstalledCertificateIdsResponse   Response,
                                                     TimeSpan                                Runtime);

    #endregion

    #region OnGetLog

    /// <summary>
    /// A get log request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetLogRequestDelegate(DateTime           Timestamp,
                                IEventSender       Sender,
                                CSMS.GetLogRequest   Request);


    /// <summary>
    /// A get log request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetLogResponse>

        OnGetLogDelegate(DateTime            Timestamp,
                         IEventSender        Sender,
                         CSMS.GetLogRequest    Request,
                         CancellationToken   CancellationToken);


    /// <summary>
    /// A get log response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetLogResponseDelegate(DateTime            Timestamp,
                                 IEventSender        Sender,
                                 CSMS.GetLogRequest    Request,
                                 CS.GetLogResponse   Response,
                                 TimeSpan            Runtime);

    #endregion

    #region OnInstallCertificate

    /// <summary>
    /// An install certificate request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnInstallCertificateRequestDelegate(DateTime                       Timestamp,
                                            IEventSender                   Sender,
                                            CSMS.InstallCertificateRequest   Request);


    /// <summary>
    /// An install certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<InstallCertificateResponse>

        OnInstallCertificateDelegate(DateTime                       Timestamp,
                                     IEventSender                   Sender,
                                     CSMS.InstallCertificateRequest   Request,
                                     CancellationToken              CancellationToken);


    /// <summary>
    /// An install certificate response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnInstallCertificateResponseDelegate(DateTime                        Timestamp,
                                             IEventSender                    Sender,
                                             CSMS.InstallCertificateRequest    Request,
                                             CS.InstallCertificateResponse   Response,
                                             TimeSpan                        Runtime);

    #endregion


}
