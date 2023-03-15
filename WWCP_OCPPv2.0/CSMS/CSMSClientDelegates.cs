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

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    #region OnReset

    /// <summary>
    /// A delegate called whenever a reset request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnResetRequestDelegate(DateTime               LogTimestamp,
                                                ICSMSClient   Sender,
                                                ResetRequest           Request);

    /// <summary>
    /// A delegate called whenever a response to a reset request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnResetResponseDelegate(DateTime               LogTimestamp,
                                                 ICSMSClient   Sender,
                                                 CSMS.ResetRequest        Request,
                                                 CS.ResetResponse       Response,
                                                 TimeSpan               Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A delegate called whenever a change availability request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnChangeAvailabilityRequestDelegate(DateTime                    LogTimestamp,
                                                             ICSMSClient        Sender,
                                                             ChangeAvailabilityRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a change availability request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnChangeAvailabilityResponseDelegate(DateTime                        LogTimestamp,
                                                              ICSMSClient            Sender,
                                                              CSMS.ChangeAvailabilityRequest    Request,
                                                              CS.ChangeAvailabilityResponse   Response,
                                                              TimeSpan                        Runtime);

    #endregion

    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDataTransferRequestDelegate(DateTime                 LogTimestamp,
                                                       ICSMSClient     Sender,
                                                       CSMS.DataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDataTransferResponseDelegate(DateTime                  LogTimestamp,
                                                        ICSMSClient      Sender,
                                                        CSMS.DataTransferRequest    Request,
                                                        CS.DataTransferResponse   Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A delegate called whenever a trigger message request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnTriggerMessageRequestDelegate(DateTime                LogTimestamp,
                                                         ICSMSClient    Sender,
                                                         TriggerMessageRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a trigger message request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnTriggerMessageResponseDelegate(DateTime                    LogTimestamp,
                                                          ICSMSClient        Sender,
                                                          CSMS.TriggerMessageRequest    Request,
                                                          CS.TriggerMessageResponse   Response,
                                                          TimeSpan                    Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// A delegate called whenever a update firmware request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUpdateFirmwareRequestDelegate(DateTime                LogTimestamp,
                                                         ICSMSClient    Sender,
                                                         UpdateFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a update firmware request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUpdateFirmwareResponseDelegate(DateTime                    LogTimestamp,
                                                          ICSMSClient        Sender,
                                                          CSMS.UpdateFirmwareRequest    Request,
                                                          CS.UpdateFirmwareResponse   Response,
                                                          TimeSpan                    Runtime);

    #endregion


    #region OnReserveNow

    /// <summary>
    /// A delegate called whenever a reserve now request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnReserveNowRequestDelegate(DateTime               LogTimestamp,
                                                     ICSMSClient   Sender,
                                                     ReserveNowRequest      Request);

    /// <summary>
    /// A delegate called whenever a response to a reserve now request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnReserveNowResponseDelegate(DateTime               LogTimestamp,
                                                      ICSMSClient   Sender,
                                                      CSMS.ReserveNowRequest   Request,
                                                      CS.ReserveNowResponse  Response,
                                                      TimeSpan               Runtime);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A delegate called whenever a cancel reservation request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCancelReservationRequestDelegate(DateTime                   LogTimestamp,
                                                            ICSMSClient       Sender,
                                                            CancelReservationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a cancel reservation request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCancelReservationResponseDelegate(DateTime                       LogTimestamp,
                                                             ICSMSClient           Sender,
                                                             CSMS.CancelReservationRequest    Request,
                                                             CS.CancelReservationResponse   Response,
                                                             TimeSpan                       Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// A delegate called whenever a unlock connector request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUnlockConnectorRequestDelegate(DateTime                 LogTimestamp,
                                                          ICSMSClient     Sender,
                                                          UnlockConnectorRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a unlock connector request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTime                     LogTimestamp,
                                                           ICSMSClient         Sender,
                                                           CSMS.UnlockConnectorRequest    Request,
                                                           CS.UnlockConnectorResponse   Response,
                                                           TimeSpan                     Runtime);

    #endregion

    #region OnSetChargingProfile

    /// <summary>
    /// A delegate called whenever a set charging profile request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTime                    LogTimestamp,
                                                             ICSMSClient        Sender,
                                                             SetChargingProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set charging profile request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                        LogTimestamp,
                                                              ICSMSClient            Sender,
                                                              CSMS.SetChargingProfileRequest    Request,
                                                              CS.SetChargingProfileResponse   Response,
                                                              TimeSpan                        Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A delegate called whenever a clear charging profile request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearChargingProfileRequestDelegate(DateTime                      LogTimestamp,
                                                               ICSMSClient          Sender,
                                                               ClearChargingProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear charging profile request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearChargingProfileResponseDelegate(DateTime                          LogTimestamp,
                                                                ICSMSClient              Sender,
                                                                CSMS.ClearChargingProfileRequest    Request,
                                                                CS.ClearChargingProfileResponse   Response,
                                                                TimeSpan                          Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A delegate called whenever a get composite schedule request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetCompositeScheduleRequestDelegate(DateTime                      LogTimestamp,
                                                               ICSMSClient          Sender,
                                                               GetCompositeScheduleRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get composite schedule request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetCompositeScheduleResponseDelegate(DateTime                          LogTimestamp,
                                                                ICSMSClient              Sender,
                                                                CSMS.GetCompositeScheduleRequest    Request,
                                                                CS.GetCompositeScheduleResponse   Response,
                                                                TimeSpan                          Runtime);

    #endregion


    #region OnGetLocalListVersion

    /// <summary>
    /// A delegate called whenever a get local list version request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetLocalListVersionRequestDelegate(DateTime                     LogTimestamp,
                                                              ICSMSClient         Sender,
                                                              GetLocalListVersionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get local list version request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetLocalListVersionResponseDelegate(DateTime                         LogTimestamp,
                                                               ICSMSClient             Sender,
                                                               CSMS.GetLocalListVersionRequest    Request,
                                                               CS.GetLocalListVersionResponse   Response,
                                                               TimeSpan                         Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A delegate called whenever a send local list request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSendLocalListRequestDelegate(DateTime               LogTimestamp,
                                                        ICSMSClient   Sender,
                                                        SendLocalListRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a send local list request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSendLocalListResponseDelegate(DateTime                   LogTimestamp,
                                                         ICSMSClient       Sender,
                                                         CSMS.SendLocalListRequest    Request,
                                                         CS.SendLocalListResponse   Response,
                                                         TimeSpan                   Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A delegate called whenever a clear cache request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearCacheRequestDelegate(DateTime               LogTimestamp,
                                                     ICSMSClient   Sender,
                                                     ClearCacheRequest      Request);

    /// <summary>
    /// A delegate called whenever a response to a clear cache request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearCacheResponseDelegate(DateTime                LogTimestamp,
                                                      ICSMSClient    Sender,
                                                      CSMS.ClearCacheRequest    Request,
                                                      CS.ClearCacheResponse   Response,
                                                      TimeSpan                Runtime);

    #endregion


    // Security extensions...

    #region OnCertificateSigned

    /// <summary>
    /// A delegate called whenever an install certificate request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCertificateSignedRequestDelegate(DateTime                   LogTimestamp,
                                                            ICSMSClient       Sender,
                                                            CertificateSignedRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an install certificate request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCertificateSignedResponseDelegate(DateTime                       LogTimestamp,
                                                             ICSMSClient           Sender,
                                                             CSMS.CertificateSignedRequest    Request,
                                                             CS.CertificateSignedResponse   Response,
                                                             TimeSpan                       Runtime);

    #endregion

    #region OnDeleteCertificate

    /// <summary>
    /// A delegate called whenever a delete certificate request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDeleteCertificateRequestDelegate(DateTime                   LogTimestamp,
                                                            ICSMSClient       Sender,
                                                            DeleteCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a delete certificate request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDeleteCertificateResponseDelegate(DateTime                       LogTimestamp,
                                                             ICSMSClient           Sender,
                                                             CSMS.DeleteCertificateRequest    Request,
                                                             CS.DeleteCertificateResponse   Response,
                                                             TimeSpan                       Runtime);

    #endregion

    #region OnGetInstalledCertificateIds

    /// <summary>
    /// A delegate called whenever a get installed certificate ids request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetInstalledCertificateIdsRequestDelegate(DateTime                            LogTimestamp,
                                                                     ICSMSClient                Sender,
                                                                     GetInstalledCertificateIdsRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get installed certificate ids request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetInstalledCertificateIdsResponseDelegate(DateTime                                LogTimestamp,
                                                                      ICSMSClient                    Sender,
                                                                      CSMS.GetInstalledCertificateIdsRequest    Request,
                                                                      CS.GetInstalledCertificateIdsResponse   Response,
                                                                      TimeSpan                                Runtime);

    #endregion

    #region OnGetLog

    /// <summary>
    /// A delegate called whenever a get log request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetLogRequestDelegate(DateTime               LogTimestamp,
                                                 ICSMSClient   Sender,
                                                 GetLogRequest          Request);

    /// <summary>
    /// A delegate called whenever a response to a get log request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetLogResponseDelegate(DateTime               LogTimestamp,
                                                  ICSMSClient   Sender,
                                                  CSMS.GetLogRequest       Request,
                                                  CS.GetLogResponse      Response,
                                                  TimeSpan               Runtime);

    #endregion

    #region OnInstallCertificate

    /// <summary>
    /// A delegate called whenever an install certificate request will be send to a charge point.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnInstallCertificateRequestDelegate(DateTime                    LogTimestamp,
                                                             ICSMSClient        Sender,
                                                             InstallCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an install certificate request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnInstallCertificateResponseDelegate(DateTime                        LogTimestamp,
                                                              ICSMSClient            Sender,
                                                              CSMS.InstallCertificateRequest    Request,
                                                              CS.InstallCertificateResponse   Response,
                                                              TimeSpan                        Runtime);

    #endregion


}
