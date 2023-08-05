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

using cloud.charging.open.protocols.OCPPv2_0_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    #region OnReset

    /// <summary>
    /// A delegate called whenever a reset request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnResetRequestDelegate(DateTime       Timestamp,
                                                IEventSender   Sender,
                                                ResetRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a reset request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnResetResponseDelegate(DateTime        Timestamp,
                                                 IEventSender    Sender,
                                                 ResetRequest    Request,
                                                 ResetResponse   Response,
                                                 TimeSpan        Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// A delegate called whenever a update firmware request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUpdateFirmwareRequestDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         UpdateFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a update firmware request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUpdateFirmwareResponseDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          UpdateFirmwareRequest    Request,
                                                          UpdateFirmwareResponse   Response,
                                                          TimeSpan                 Runtime);

    #endregion

    #region OnPublishFirmware

    /// <summary>
    /// A delegate called whenever a publish firmware request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnPublishFirmwareRequestDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          PublishFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a publish firmware request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnPublishFirmwareResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           PublishFirmwareRequest    Request,
                                                           PublishFirmwareResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion

    #region OnUnpublishFirmware

    /// <summary>
    /// A delegate called whenever a unpublish firmware request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUnpublishFirmwareRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            UnpublishFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a unpublish firmware request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUnpublishFirmwareResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             UnpublishFirmwareRequest    Request,
                                                             UnpublishFirmwareResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnGetBaseReport

    /// <summary>
    /// A delegate called whenever a get base report request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetBaseReportRequestDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        GetBaseReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get base report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetBaseReportResponseDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         GetBaseReportRequest    Request,
                                                         GetBaseReportResponse   Response,
                                                         TimeSpan                Runtime);

    #endregion

    #region OnGetReport

    /// <summary>
    /// A delegate called whenever a get report request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetReportRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    GetReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetReportResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     GetReportRequest    Request,
                                                     GetReportResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion

    #region OnGetLog

    /// <summary>
    /// A delegate called whenever a get log request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetLogRequestDelegate(DateTime        Timestamp,
                                                 IEventSender    Sender,
                                                 GetLogRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get log request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetLogResponseDelegate(DateTime         Timestamp,
                                                  IEventSender     Sender,
                                                  GetLogRequest    Request,
                                                  GetLogResponse   Response,
                                                  TimeSpan         Runtime);

    #endregion

    #region OnSetVariables

    /// <summary>
    /// A delegate called whenever a set variables request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetVariablesRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       SetVariablesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set variables request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetVariablesResponseDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        SetVariablesRequest    Request,
                                                        SetVariablesResponse   Response,
                                                        TimeSpan               Runtime);

    #endregion

    #region OnGetVariables

    /// <summary>
    /// A delegate called whenever a get variables request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetVariablesRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       GetVariablesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get variables request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetVariablesResponseDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        GetVariablesRequest    Request,
                                                        GetVariablesResponse   Response,
                                                        TimeSpan               Runtime);

    #endregion

    #region OnSetMonitoringBase

    /// <summary>
    /// A delegate called whenever a set monitoring base request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetMonitoringBaseRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            SetMonitoringBaseRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set monitoring base request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetMonitoringBaseResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetMonitoringBaseRequest    Request,
                                                             SetMonitoringBaseResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnGetMonitoringReport

    /// <summary>
    /// A delegate called whenever a get monitoring report request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetMonitoringReportRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              GetMonitoringReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get monitoring report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetMonitoringReportResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetMonitoringReportRequest    Request,
                                                               GetMonitoringReportResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion

    #region OnSetMonitoringLevel

    /// <summary>
    /// A delegate called whenever a set monitoring level request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetMonitoringLevelRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetMonitoringLevelRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set monitoring level request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetMonitoringLevelResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              SetMonitoringLevelRequest    Request,
                                                              SetMonitoringLevelResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnSetVariableMonitoring

    /// <summary>
    /// A delegate called whenever a set variable monitoring request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetVariableMonitoringRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                SetVariableMonitoringRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set variable monitoring request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetVariableMonitoringResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 SetVariableMonitoringRequest    Request,
                                                                 SetVariableMonitoringResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnClearVariableMonitoring

    /// <summary>
    /// A delegate called whenever a clear variable monitoring request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearVariableMonitoringRequestDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  ClearVariableMonitoringRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear variable monitoring request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearVariableMonitoringResponseDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   ClearVariableMonitoringRequest    Request,
                                                                   ClearVariableMonitoringResponse   Response,
                                                                   TimeSpan                          Runtime);

    #endregion

    #region OnSetNetworkProfile

    /// <summary>
    /// A delegate called whenever a set network profile request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetNetworkProfileRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            SetNetworkProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set network profile request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetNetworkProfileResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetNetworkProfileRequest    Request,
                                                             SetNetworkProfileResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A delegate called whenever a change availability request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnChangeAvailabilityRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             ChangeAvailabilityRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a change availability request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnChangeAvailabilityResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              ChangeAvailabilityRequest    Request,
                                                              ChangeAvailabilityResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A delegate called whenever a trigger message request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnTriggerMessageRequestDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         TriggerMessageRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a trigger message request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnTriggerMessageResponseDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          TriggerMessageRequest    Request,
                                                          TriggerMessageResponse   Response,
                                                          TimeSpan                 Runtime);

    #endregion

    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDataTransferRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       DataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDataTransferResponseDelegate(DateTime                  Timestamp,
                                                        IEventSender              Sender,
                                                        DataTransferRequest       Request,
                                                        CS.DataTransferResponse   Response,
                                                        TimeSpan                  Runtime);

    #endregion


    #region OnCertificateSigned

    /// <summary>
    /// A delegate called whenever a certificate signed request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCertificateSignedRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            CertificateSignedRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a certificate signed request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCertificateSignedResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             CertificateSignedRequest    Request,
                                                             CertificateSignedResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnInstallCertificate

    /// <summary>
    /// A delegate called whenever an install certificate request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnInstallCertificateRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             InstallCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an install certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnInstallCertificateResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              InstallCertificateRequest    Request,
                                                              InstallCertificateResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnGetInstalledCertificateIds

    /// <summary>
    /// A delegate called whenever a get installed certificate ids request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetInstalledCertificateIdsRequestDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     GetInstalledCertificateIdsRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get installed certificate ids request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetInstalledCertificateIdsResponseDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      GetInstalledCertificateIdsRequest    Request,
                                                                      GetInstalledCertificateIdsResponse   Response,
                                                                      TimeSpan                             Runtime);

    #endregion

    #region OnDeleteCertificate

    /// <summary>
    /// A delegate called whenever a delete certificate request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDeleteCertificateRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            DeleteCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a delete certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDeleteCertificateResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             DeleteCertificateRequest    Request,
                                                             DeleteCertificateResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion


    #region OnGetLocalListVersion

    /// <summary>
    /// A delegate called whenever a get local list version request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetLocalListVersionRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              GetLocalListVersionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get local list version request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetLocalListVersionResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetLocalListVersionRequest    Request,
                                                               GetLocalListVersionResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A delegate called whenever a send local list request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSendLocalListRequestDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        SendLocalListRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a send local list request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSendLocalListResponseDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         SendLocalListRequest    Request,
                                                         SendLocalListResponse   Response,
                                                         TimeSpan                Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A delegate called whenever a clear cache request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearCacheRequestDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     ClearCacheRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear cache request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearCacheResponseDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      ClearCacheRequest    Request,
                                                      ClearCacheResponse   Response,
                                                      TimeSpan             Runtime);

    #endregion


    #region OnReserveNow

    /// <summary>
    /// A delegate called whenever a reserve now request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnReserveNowRequestDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     ReserveNowRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a reserve now request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnReserveNowResponseDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      ReserveNowRequest    Request,
                                                      ReserveNowResponse   Response,
                                                      TimeSpan             Runtime);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A delegate called whenever a cancel reservation request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCancelReservationRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            CancelReservationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a cancel reservation request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCancelReservationResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             CancelReservationRequest    Request,
                                                             CancelReservationResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnRequestStartTransaction

    /// <summary>
    /// A delegate called whenever a request start transaction request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnRequestStartTransactionRequestDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  RequestStartTransactionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a request start transaction request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnRequestStartTransactionResponseDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   RequestStartTransactionRequest    Request,
                                                                   RequestStartTransactionResponse   Response,
                                                                   TimeSpan                          Runtime);

    #endregion

    #region OnRequestStopTransaction

    /// <summary>
    /// A delegate called whenever a request stop transaction request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnRequestStopTransactionRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 RequestStopTransactionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a request stop transaction request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnRequestStopTransactionResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  RequestStopTransactionRequest    Request,
                                                                  RequestStopTransactionResponse   Response,
                                                                  TimeSpan                         Runtime);

    #endregion

    #region OnGetTransactionStatus

    /// <summary>
    /// A delegate called whenever a get transaction status request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetTransactionStatusRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetTransactionStatusRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get transaction status request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetTransactionStatusResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                GetTransactionStatusRequest    Request,
                                                                GetTransactionStatusResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion

    #region OnSetChargingProfile

    /// <summary>
    /// A delegate called whenever a set charging profile request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetChargingProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set charging profile request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              SetChargingProfileRequest    Request,
                                                              SetChargingProfileResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnGetChargingProfiles

    /// <summary>
    /// A delegate called whenever a get charging profiles request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetChargingProfilesRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              GetChargingProfilesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get charging profiles request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetChargingProfilesResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetChargingProfilesRequest    Request,
                                                               GetChargingProfilesResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A delegate called whenever a clear charging profile request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearChargingProfileRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               ClearChargingProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear charging profile request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearChargingProfileResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                ClearChargingProfileRequest    Request,
                                                                ClearChargingProfileResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A delegate called whenever a get composite schedule request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetCompositeScheduleRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetCompositeScheduleRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get composite schedule request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetCompositeScheduleResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                GetCompositeScheduleRequest    Request,
                                                                GetCompositeScheduleResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// A delegate called whenever an unlock connector request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUnlockConnectorRequestDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          UnlockConnectorRequest   Request);

    /// <summary>
    /// A delegate called whenever an response to a unlock connector request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           UnlockConnectorRequest    Request,
                                                           UnlockConnectorResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion


    #region OnSetDisplayMessage

    /// <summary>
    /// A delegate called whenever a set display message request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetDisplayMessageRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            SetDisplayMessageRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set display message request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetDisplayMessageResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetDisplayMessageRequest    Request,
                                                             SetDisplayMessageResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion

    #region OnGetDisplayMessages

    /// <summary>
    /// A delegate called whenever a get display messages request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetDisplayMessagesRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             GetDisplayMessagesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get display messages request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetDisplayMessagesResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              GetDisplayMessagesRequest    Request,
                                                              GetDisplayMessagesResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnClearDisplayMessage

    /// <summary>
    /// A delegate called whenever a clear display message request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearDisplayMessageRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              ClearDisplayMessageRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear display message request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearDisplayMessageResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               ClearDisplayMessageRequest    Request,
                                                               ClearDisplayMessageResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion

    #region OnCostUpdated

    /// <summary>
    /// A delegate called whenever a cost updated request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCostUpdatedRequestDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      CostUpdatedRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a cost updated request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCostUpdatedResponseDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       CostUpdatedRequest    Request,
                                                       CostUpdatedResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion

    #region OnCustomerInformation

    /// <summary>
    /// A delegate called whenever a customer information request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCustomerInformationRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              CustomerInformationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a customer information request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCustomerInformationResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               CustomerInformationRequest    Request,
                                                               CustomerInformationResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion


}
