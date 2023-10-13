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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnReset

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnResetRequestDelegate(DateTime       Timestamp,
                               IEventSender   Sender,
                               ResetRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ResetResponse>

        OnResetDelegate(DateTime                    Timestamp,
                        IEventSender                Sender,
                        WebSocketClientConnection   Connection,
                        ResetRequest                Request,
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

        OnResetResponseDelegate(DateTime        Timestamp,
                                IEventSender    Sender,
                                ResetRequest    Request,
                                ResetResponse   Response,
                                TimeSpan        Runtime);

    #endregion

    #region OnUpdateFirmware

    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateFirmwareRequestDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        UpdateFirmwareRequest   Request);


    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateFirmwareResponse>

        OnUpdateFirmwareDelegate(DateTime                    Timestamp,
                                 IEventSender                Sender,
                                 WebSocketClientConnection   Connection,
                                 UpdateFirmwareRequest       Request,
                                 CancellationToken           CancellationToken);


    /// <summary>
    /// An update firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateFirmwareResponseDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         UpdateFirmwareRequest    Request,
                                         UpdateFirmwareResponse   Response,
                                         TimeSpan                 Runtime);

    #endregion

    #region OnPublishFirmware

    /// <summary>
    /// A publish firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnPublishFirmwareRequestDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         PublishFirmwareRequest   Request);


    /// <summary>
    /// A publish firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<PublishFirmwareResponse>

        OnPublishFirmwareDelegate(DateTime                    Timestamp,
                                  IEventSender                Sender,
                                  WebSocketClientConnection   Connection,
                                  PublishFirmwareRequest      Request,
                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A publish firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnPublishFirmwareResponseDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          PublishFirmwareRequest    Request,
                                          PublishFirmwareResponse   Response,
                                          TimeSpan                  Runtime);

    #endregion

    #region OnUnpublishFirmware

    /// <summary>
    /// An unpublish firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUnpublishFirmwareRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           UnpublishFirmwareRequest   Request);


    /// <summary>
    /// An unpublish firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UnpublishFirmwareResponse>

        OnUnpublishFirmwareDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    UnpublishFirmwareRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// An unpublish firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUnpublishFirmwareResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            UnpublishFirmwareRequest    Request,
                                            UnpublishFirmwareResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnGetBaseReport

    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetBaseReportRequestDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       GetBaseReportRequest   Request);


    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetBaseReportResponse>

        OnGetBaseReportDelegate(DateTime                    Timestamp,
                                IEventSender                Sender,
                                WebSocketClientConnection   Connection,
                                GetBaseReportRequest        Request,
                                CancellationToken           CancellationToken);


    /// <summary>
    /// A get base report response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetBaseReportResponseDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        GetBaseReportRequest    Request,
                                        GetBaseReportResponse   Response,
                                        TimeSpan                Runtime);

    #endregion

    #region OnGetReport

    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetReportRequestDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   GetReportRequest   Request);


    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetReportResponse>

        OnGetReportDelegate(DateTime                    Timestamp,
                            IEventSender                Sender,
                            WebSocketClientConnection   Connection,
                            GetReportRequest            Request,
                            CancellationToken           CancellationToken);


    /// <summary>
    /// A get base report response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetReportResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    GetReportRequest    Request,
                                    GetReportResponse   Response,
                                    TimeSpan            Runtime);

    #endregion

    #region OnGetLog

    /// <summary>
    /// A get log request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetLogRequestDelegate(DateTime        Timestamp,
                                IEventSender    Sender,
                                GetLogRequest   Request);


    /// <summary>
    /// A get log request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetLogResponse>

        OnGetLogDelegate(DateTime                    Timestamp,
                         IEventSender                Sender,
                         WebSocketClientConnection   Connection,
                         GetLogRequest               Request,
                         CancellationToken           CancellationToken);


    /// <summary>
    /// A get log response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetLogResponseDelegate(DateTime         Timestamp,
                                 IEventSender     Sender,
                                 GetLogRequest    Request,
                                 GetLogResponse   Response,
                                 TimeSpan         Runtime);

    #endregion

    #region OnSetVariables

    /// <summary>
    /// A set variables request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetVariablesRequestDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      SetVariablesRequest   Request);


    /// <summary>
    /// A set variables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetVariablesResponse>

        OnSetVariablesDelegate(DateTime                    Timestamp,
                               IEventSender                Sender,
                               WebSocketClientConnection   Connection,
                               SetVariablesRequest         Request,
                               CancellationToken           CancellationToken);


    /// <summary>
    /// A set variables response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetVariablesResponseDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       SetVariablesRequest    Request,
                                       SetVariablesResponse   Response,
                                       TimeSpan               Runtime);

    #endregion

    #region OnGetVariables

    /// <summary>
    /// A get variables request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetVariablesRequestDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      GetVariablesRequest   Request);


    /// <summary>
    /// A get variables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetVariablesResponse>

        OnGetVariablesDelegate(DateTime                    Timestamp,
                               IEventSender                Sender,
                               WebSocketClientConnection   Connection,
                               GetVariablesRequest         Request,
                               CancellationToken           CancellationToken);


    /// <summary>
    /// A get variables response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetVariablesResponseDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       GetVariablesRequest    Request,
                                       GetVariablesResponse   Response,
                                       TimeSpan               Runtime);

    #endregion

    #region OnSetMonitoringBase

    /// <summary>
    /// A set monitoring base request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetMonitoringBaseRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           SetMonitoringBaseRequest   Request);


    /// <summary>
    /// A set monitoring base request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetMonitoringBaseResponse>

        OnSetMonitoringBaseDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    SetMonitoringBaseRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A set monitoring base response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetMonitoringBaseResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetMonitoringBaseRequest    Request,
                                            SetMonitoringBaseResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnGetMonitoringReport

    /// <summary>
    /// A get monitoring report request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetMonitoringReportRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             GetMonitoringReportRequest   Request);


    /// <summary>
    /// A get monitoring report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetMonitoringReportResponse>

        OnGetMonitoringReportDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      GetMonitoringReportRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A get monitoring report response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetMonitoringReportResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetMonitoringReportRequest    Request,
                                              GetMonitoringReportResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnSetMonitoringLevel

    /// <summary>
    /// A set monitoring level request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetMonitoringLevelRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetMonitoringLevelRequest   Request);


    /// <summary>
    /// A set monitoring level request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetMonitoringLevelResponse>

        OnSetMonitoringLevelDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     SetMonitoringLevelRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A set monitoring level response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetMonitoringLevelResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             SetMonitoringLevelRequest    Request,
                                             SetMonitoringLevelResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnSetVariableMonitoring

    /// <summary>
    /// A set variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetVariableMonitoringRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               SetVariableMonitoringRequest   Request);


    /// <summary>
    /// A set variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetVariableMonitoringResponse>

        OnSetVariableMonitoringDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        WebSocketClientConnection      Connection,
                                        SetVariableMonitoringRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A set variable monitoring response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetVariableMonitoringResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                SetVariableMonitoringRequest    Request,
                                                SetVariableMonitoringResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnClearVariableMonitoring

    /// <summary>
    /// A clear variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearVariableMonitoringRequestDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 ClearVariableMonitoringRequest   Request);


    /// <summary>
    /// A clear variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearVariableMonitoringResponse>

        OnClearVariableMonitoringDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          WebSocketClientConnection        Connection,
                                          ClearVariableMonitoringRequest   Request,
                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A clear variable monitoring response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearVariableMonitoringResponseDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  ClearVariableMonitoringRequest    Request,
                                                  ClearVariableMonitoringResponse   Response,
                                                  TimeSpan                          Runtime);

    #endregion

    #region OnSetNetworkProfile

    /// <summary>
    /// A set network profile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetNetworkProfileRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           SetNetworkProfileRequest   Request);


    /// <summary>
    /// A set network profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetNetworkProfileResponse>

        OnSetNetworkProfileDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    SetNetworkProfileRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A set network profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetNetworkProfileResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetNetworkProfileRequest    Request,
                                            SetNetworkProfileResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnChangeAvailability

    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnChangeAvailabilityRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            ChangeAvailabilityRequest   Request);


    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeAvailabilityResponse>

        OnChangeAvailabilityDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     ChangeAvailabilityRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A change availability response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnChangeAvailabilityResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             ChangeAvailabilityRequest    Request,
                                             ChangeAvailabilityResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnTriggerMessage

    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnTriggerMessageRequestDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        TriggerMessageRequest   Request);


    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<TriggerMessageResponse>

        OnTriggerMessageDelegate(DateTime                    Timestamp,
                                 IEventSender                Sender,
                                 WebSocketClientConnection   Connection,
                                 TriggerMessageRequest       Request,
                                 CancellationToken           CancellationToken);


    /// <summary>
    /// A trigger message response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnTriggerMessageResponseDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         TriggerMessageRequest    Request,
                                         TriggerMessageResponse   Response,
                                         TimeSpan                 Runtime);

    #endregion

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                   Timestamp,
                                              IEventSender               Sender,
                                              CSMS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       WebSocketClientConnection   Connection,
                                       CSMS.DataTransferRequest    Request,
                                       CancellationToken           CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                   Timestamp,
                                               IEventSender               Sender,
                                               CSMS.DataTransferRequest   Request,
                                               CS.DataTransferResponse    Response,
                                               TimeSpan                   Runtime);

    #endregion


    #region OnCertificateSigned

    /// <summary>
    /// A certificate signed request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCertificateSignedRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           CertificateSignedRequest   Request);


    /// <summary>
    /// A certificate signed request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CertificateSignedResponse>

        OnCertificateSignedDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    CertificateSignedRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A certificate signed response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCertificateSignedResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            CertificateSignedRequest    Request,
                                            CertificateSignedResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnInstallCertificate

    /// <summary>
    /// An install certificate request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnInstallCertificateRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            InstallCertificateRequest   Request);


    /// <summary>
    /// An install certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<InstallCertificateResponse>

        OnInstallCertificateDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     InstallCertificateRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// An install certificate response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnInstallCertificateResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             InstallCertificateRequest    Request,
                                             InstallCertificateResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnGetInstalledCertificateIds

    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    GetInstalledCertificateIdsRequest   Request);


    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetInstalledCertificateIdsResponse>

        OnGetInstalledCertificateIdsDelegate(DateTime                            Timestamp,
                                             IEventSender                        Sender,
                                             WebSocketClientConnection           Connection,
                                             GetInstalledCertificateIdsRequest   Request,
                                             CancellationToken                   CancellationToken);


    /// <summary>
    /// A get installed certificate ids response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsResponseDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     GetInstalledCertificateIdsRequest    Request,
                                                     GetInstalledCertificateIdsResponse   Response,
                                                     TimeSpan                             Runtime);

    #endregion

    #region OnDeleteCertificate

    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnDeleteCertificateRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           DeleteCertificateRequest   Request);


    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DeleteCertificateResponse>

        OnDeleteCertificateDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    DeleteCertificateRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A delete certificate response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnDeleteCertificateResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            DeleteCertificateRequest    Request,
                                            DeleteCertificateResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnNotifyCRL

    /// <summary>
    /// A NotifyCRL request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnNotifyCRLRequestDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   NotifyCRLRequest   Request);


    /// <summary>
    /// A NotifyCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyCRLResponse>

        OnNotifyCRLDelegate(DateTime                    Timestamp,
                            IEventSender                Sender,
                            WebSocketClientConnection   Connection,
                            NotifyCRLRequest            Request,
                            CancellationToken           CancellationToken);


    /// <summary>
    /// A NotifyCRL response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnNotifyCRLResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    NotifyCRLRequest    Request,
                                    NotifyCRLResponse   Response,
                                    TimeSpan            Runtime);

    #endregion


    #region OnGetLocalListVersion

    /// <summary>
    /// A get local list request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetLocalListVersionRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             GetLocalListVersionRequest   Request);


    /// <summary>
    /// A get local list request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetLocalListVersionResponse>

        OnGetLocalListVersionDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      GetLocalListVersionRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A get local list response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetLocalListVersionResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetLocalListVersionRequest    Request,
                                              GetLocalListVersionResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnSendLocalList

    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSendLocalListRequestDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       SendLocalListRequest   Request);


    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendLocalListResponse>

        OnSendLocalListDelegate(DateTime                    Timestamp,
                                IEventSender                Sender,
                                WebSocketClientConnection   Connection,
                                SendLocalListRequest        Request,
                                CancellationToken           CancellationToken);


    /// <summary>
    /// A send local list response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSendLocalListResponseDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        SendLocalListRequest    Request,
                                        SendLocalListResponse   Response,
                                        TimeSpan                Runtime);

    #endregion

    #region OnClearCache

    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearCacheRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    ClearCacheRequest   Request);


    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearCacheResponse>

        OnClearCacheDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             ClearCacheRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// A clear cache response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearCacheResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     ClearCacheRequest    Request,
                                     ClearCacheResponse   Response,
                                     TimeSpan             Runtime);

    #endregion


    #region OnReserveNow

    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnReserveNowRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    ReserveNowRequest   Request);


    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReserveNowResponse>

        OnReserveNowDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             ReserveNowRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// A reserve now response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnReserveNowResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     ReserveNowRequest    Request,
                                     ReserveNowResponse   Response,
                                     TimeSpan             Runtime);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    public delegate Task

        OnCancelReservationRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           CancelReservationRequest   Request);


    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CancelReservationResponse>

        OnCancelReservationDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    CancelReservationRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="Response">The cancel reservation response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCancelReservationResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            CancelReservationRequest    Request,
                                            CancelReservationResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnRequestStartTransaction

    /// <summary>
    /// A request start transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnRequestStartTransactionRequestDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 RequestStartTransactionRequest   Request);


    /// <summary>
    /// A request start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestStartTransactionResponse>

        OnRequestStartTransactionDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          WebSocketClientConnection        Connection,
                                          RequestStartTransactionRequest   Request,
                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A request start transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnRequestStartTransactionResponseDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  RequestStartTransactionRequest    Request,
                                                  RequestStartTransactionResponse   Response,
                                                  TimeSpan                          Runtime);

    #endregion

    #region OnRequestStopTransaction

    /// <summary>
    /// A request stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnRequestStopTransactionRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                RequestStopTransactionRequest   Request);


    /// <summary>
    /// A request stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestStopTransactionResponse>

        OnRequestStopTransactionDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         WebSocketClientConnection       Connection,
                                         RequestStopTransactionRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A request stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnRequestStopTransactionResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 RequestStopTransactionRequest    Request,
                                                 RequestStopTransactionResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion

    #region OnGetTransactionStatus

    /// <summary>
    /// A get transaction status request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetTransactionStatusRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetTransactionStatusRequest   Request);


    /// <summary>
    /// A get transaction status request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetTransactionStatusResponse>

        OnGetTransactionStatusDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       WebSocketClientConnection     Connection,
                                       GetTransactionStatusRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A get transaction status response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetTransactionStatusResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               GetTransactionStatusRequest    Request,
                                               GetTransactionStatusResponse   Response,
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

        OnSetChargingProfileRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetChargingProfileRequest   Request);


    /// <summary>
    /// A set charging profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetChargingProfileResponse>

        OnSetChargingProfileDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     SetChargingProfileRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A set charging profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetChargingProfileResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             SetChargingProfileRequest    Request,
                                             SetChargingProfileResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnGetChargingProfiles

    /// <summary>
    /// A get charging profiles request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetChargingProfilesRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             GetChargingProfilesRequest   Request);


    /// <summary>
    /// A get charging profiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetChargingProfilesResponse>

        OnGetChargingProfilesDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      GetChargingProfilesRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A get charging profiles response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetChargingProfilesResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetChargingProfilesRequest    Request,
                                              GetChargingProfilesResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnClearChargingProfile

    /// <summary>
    /// A clear charging profile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearChargingProfileRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              ClearChargingProfileRequest   Request);


    /// <summary>
    /// A clear charging profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearChargingProfileResponse>

        OnClearChargingProfileDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       WebSocketClientConnection     Connection,
                                       ClearChargingProfileRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A clear charging profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearChargingProfileResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               ClearChargingProfileRequest    Request,
                                               ClearChargingProfileResponse   Response,
                                               TimeSpan                       Runtime);

    #endregion

    #region OnGetCompositeSchedule

    /// <summary>
    /// A get composite schedule request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetCompositeScheduleRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetCompositeScheduleRequest   Request);


    /// <summary>
    /// A get composite schedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCompositeScheduleResponse>

        OnGetCompositeScheduleDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       WebSocketClientConnection     Connection,
                                       GetCompositeScheduleRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A get composite schedule response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetCompositeScheduleResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               GetCompositeScheduleRequest    Request,
                                               GetCompositeScheduleResponse   Response,
                                               TimeSpan                       Runtime);

    #endregion

    #region OnUpdateDynamicSchedule

    /// <summary>
    /// A UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateDynamicScheduleRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               UpdateDynamicScheduleRequest   Request);


    /// <summary>
    /// A UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateDynamicScheduleResponse>

        OnUpdateDynamicScheduleDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        WebSocketClientConnection      Connection,
                                        UpdateDynamicScheduleRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A UpdateDynamicSchedule response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateDynamicScheduleResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                UpdateDynamicScheduleRequest    Request,
                                                UpdateDynamicScheduleResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnNotifyAllowedEnergyTransfer

    /// <summary>
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferRequestDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     NotifyAllowedEnergyTransferRequest   Request);


    /// <summary>
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyAllowedEnergyTransferResponse>

        OnNotifyAllowedEnergyTransferDelegate(DateTime                             Timestamp,
                                              IEventSender                         Sender,
                                              WebSocketClientConnection            Connection,
                                              NotifyAllowedEnergyTransferRequest   Request,
                                              CancellationToken                    CancellationToken);


    /// <summary>
    /// A NotifyAllowedEnergyTransfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferResponseDelegate(DateTime                              Timestamp,
                                                      IEventSender                          Sender,
                                                      NotifyAllowedEnergyTransferRequest    Request,
                                                      NotifyAllowedEnergyTransferResponse   Response,
                                                      TimeSpan                              Runtime);

    #endregion

    #region OnUsePriorityCharging

    /// <summary>
    /// A UsePriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUsePriorityChargingRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             UsePriorityChargingRequest   Request);


    /// <summary>
    /// A UsePriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UsePriorityChargingResponse>

        OnUsePriorityChargingDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      UsePriorityChargingRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A UsePriorityCharging response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUsePriorityChargingResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              UsePriorityChargingRequest    Request,
                                              UsePriorityChargingResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnUnlockConnector

    /// <summary>
    /// An unlock connector request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUnlockConnectorRequestDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         UnlockConnectorRequest   Request);


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
                                  WebSocketClientConnection   Connection,
                                  UnlockConnectorRequest      Request,
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

        OnUnlockConnectorResponseDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          UnlockConnectorRequest    Request,
                                          UnlockConnectorResponse   Response,
                                          TimeSpan                  Runtime);

    #endregion


    #region OnAFRRSignal

    /// <summary>
    /// An AFRR signal request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnAFRRSignalRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    AFRRSignalRequest   Request);


    /// <summary>
    /// An AFRR signal request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AFRRSignalResponse>

        OnAFRRSignalDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             AFRRSignalRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// An AFRR signal response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnAFRRSignalResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     AFRRSignalRequest    Request,
                                     AFRRSignalResponse   Response,
                                     TimeSpan             Runtime);

    #endregion


    #region OnSetDisplayMessage

    /// <summary>
    /// A set display message request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetDisplayMessageRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           SetDisplayMessageRequest   Request);


    /// <summary>
    /// A set display message request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetDisplayMessageResponse>

        OnSetDisplayMessageDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    SetDisplayMessageRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A set display message response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetDisplayMessageResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetDisplayMessageRequest    Request,
                                            SetDisplayMessageResponse   Response,
                                            TimeSpan                    Runtime);

    #endregion

    #region OnGetDisplayMessages

    /// <summary>
    /// A get display messages request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetDisplayMessagesRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            GetDisplayMessagesRequest   Request);


    /// <summary>
    /// A get display messages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetDisplayMessagesResponse>

        OnGetDisplayMessagesDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     GetDisplayMessagesRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A get display messages response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetDisplayMessagesResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             GetDisplayMessagesRequest    Request,
                                             GetDisplayMessagesResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnClearDisplayMessage

    /// <summary>
    /// A clear display message request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearDisplayMessageRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             ClearDisplayMessageRequest   Request);


    /// <summary>
    /// A clear display message request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearDisplayMessageResponse>

        OnClearDisplayMessageDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      ClearDisplayMessageRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A clear display message response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearDisplayMessageResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              ClearDisplayMessageRequest    Request,
                                              ClearDisplayMessageResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnCostUpdated

    /// <summary>
    /// A cost updated request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCostUpdatedRequestDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     CostUpdatedRequest   Request);


    /// <summary>
    /// A cost updated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CostUpdatedResponse>

        OnCostUpdatedDelegate(DateTime                    Timestamp,
                              IEventSender                Sender,
                              WebSocketClientConnection   Connection,
                              CostUpdatedRequest          Request,
                              CancellationToken           CancellationToken);


    /// <summary>
    /// A cost updated response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCostUpdatedResponseDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      CostUpdatedRequest    Request,
                                      CostUpdatedResponse   Response,
                                      TimeSpan              Runtime);

    #endregion

    #region OnCustomerInformation

    /// <summary>
    /// A customer information request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCustomerInformationRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             CustomerInformationRequest   Request);


    /// <summary>
    /// A customer information request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CustomerInformationResponse>

        OnCustomerInformationDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      CustomerInformationRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A customer information response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCustomerInformationResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              CustomerInformationRequest    Request,
                                              CustomerInformationResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion


}
