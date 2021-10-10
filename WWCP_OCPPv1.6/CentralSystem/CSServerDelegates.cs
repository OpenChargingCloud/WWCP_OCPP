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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The boot notification request.</param>
    public delegate Task

        BootNotificationRequestDelegate(DateTime                    Timestamp,
                                        IEventSender                Sender,
                                        CP.BootNotificationRequest  Request);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="BootNotificationRequest">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    public delegate Task<BootNotificationResponse>

        BootNotificationDelegate(DateTime                    Timestamp,
                                 IEventSender                Sender,
                                 CP.BootNotificationRequest  BootNotificationRequest,
                                 CancellationToken           CancellationToken);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="Response">The boot notification response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        BootNotificationResponseDelegate(DateTime                     Timestamp,
                                         IEventSender                 Sender,
                                         CP.BootNotificationRequest   Request,
                                         CS.BootNotificationResponse  Response,
                                         TimeSpan                     Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charge point heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The heartbeat request.</param>
    public delegate Task

        HeartbeatRequestDelegate(DateTime             Timestamp,
                                 IEventSender         Sender,
                                 CP.HeartbeatRequest  Request);


    /// <summary>
    /// A charge point heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="HeartbeatRequest">A heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    public delegate Task<HeartbeatResponse>

        HeartbeatDelegate(DateTime             Timestamp,
                          IEventSender         Sender,
                          CP.HeartbeatRequest  HeartbeatRequest,
                          CancellationToken    CancellationToken);


    /// <summary>
    /// A charge point heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The heartbeat request.</param>
    /// <param name="Response">The heartbeat response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        HeartbeatResponseDelegate(DateTime              Timestamp,
                                  IEventSender          Sender,
                                  CP.HeartbeatRequest   Request,
                                  CS.HeartbeatResponse  Response,
                                  TimeSpan              Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="AuthorizeRequest">An authorize request.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime             Timestamp,
                                   Object               Sender,
                                   EventTracking_Id     EventTrackingId,
                                   CP.AuthorizeRequest  AuthorizeRequest);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="AuthorizeRequest">An authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime             Timestamp,
                            Object               Sender,
                            CancellationToken    CancellationToken,
                            EventTracking_Id     EventTrackingId,
                            CP.AuthorizeRequest  AuthorizeRequest);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="AuthorizeRequest">An authorize request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="IdTagInfo">An identification tag info.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime             Timestamp,
                                    Object               Sender,
                                    EventTracking_Id     EventTrackingId,
                                    CP.AuthorizeRequest  AuthorizeRequest,

                                    Result               Result,
                                    IdTagInfo            IdTagInfo,
                                    TimeSpan             Runtime);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StartTransactionRequest">A start transaction request.</param>
    public delegate Task

        OnStartTransactionRequestDelegate(DateTime                    Timestamp,
                                          Object                      Sender,
                                          EventTracking_Id            EventTrackingId,
                                          CP.StartTransactionRequest  StartTransactionRequest);


    /// <summary>
    /// A start transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StartTransactionRequest">A start transaction request.</param>
    public delegate Task<StartTransactionResponse>

        OnStartTransactionDelegate(DateTime                    Timestamp,
                                   Object                      Sender,
                                   CancellationToken           CancellationToken,
                                   EventTracking_Id            EventTrackingId,
                                   CP.StartTransactionRequest  StartTransactionRequest);


    /// <summary>
    /// A start transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StartTransactionRequest">A start transaction request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="TransactionId">The transaction identification assigned by the central system.</param>
    /// <param name="IdTagInfo">An identification tag info.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStartTransactionResponseDelegate(DateTime                    Timestamp,
                                           Object                      Sender,
                                           EventTracking_Id            EventTrackingId,
                                           CP.StartTransactionRequest  StartTransactionRequest,

                                           Result                      Result,
                                           Transaction_Id              TransactionId,
                                           IdTagInfo                   IdTagInfo,
                                           TimeSpan                    Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusNotificationRequest">A status notification request.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                      Timestamp,
                                            Object                        Sender,
                                            EventTracking_Id              EventTrackingId,
                                            CP.StatusNotificationRequest  StatusNotificationRequest);


    /// <summary>
    /// Send a charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusNotificationRequest">A status notification request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     Object                        Sender,
                                     CancellationToken             CancellationToken,
                                     EventTracking_Id              EventTrackingId,
                                     CP.StatusNotificationRequest  StatusNotificationRequest);


    /// <summary>
    /// A charge point status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StatusNotificationRequest">A status notification request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                      Timestamp,
                                             Object                        Sender,
                                             EventTracking_Id              EventTrackingId,
                                             CP.StatusNotificationRequest  StatusNotificationRequest,

                                             Result                        Result,
                                             TimeSpan                      Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A charge point meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="MeterValuesRequest">A meter values request.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime               Timestamp,
                                     Object                 Sender,
                                     EventTracking_Id       EventTrackingId,
                                     CP.MeterValuesRequest  MeterValuesRequest);


    /// <summary>
    /// Send charge point meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="MeterValuesRequest">A meter values request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime               Timestamp,
                              Object                 Sender,
                              CancellationToken      CancellationToken,
                              EventTracking_Id       EventTrackingId,
                              CP.MeterValuesRequest  MeterValuesRequest);


    /// <summary>
    /// A charge point meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="MeterValuesRequest">A meter values request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime               Timestamp,
                                      Object                 Sender,
                                      EventTracking_Id       EventTrackingId,
                                      CP.MeterValuesRequest  MeterValuesRequest,

                                      Result                 Result,
                                      TimeSpan               Runtime);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StopTransactionRequest">A stop transaction request.</param>
    public delegate Task

        OnStopTransactionRequestDelegate(DateTime                   Timestamp,
                                         Object                     Sender,
                                         EventTracking_Id           EventTrackingId,
                                         CP.StopTransactionRequest  StopTransactionRequest);


    /// <summary>
    /// A stop transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StopTransactionRequest">A stop transaction request.</param>
    public delegate Task<StopTransactionResponse>

        OnStopTransactionDelegate(DateTime                   Timestamp,
                                  Object                     Sender,
                                  CancellationToken          CancellationToken,
                                  EventTracking_Id           EventTrackingId,
                                  CP.StopTransactionRequest  StopTransactionRequest);


    /// <summary>
    /// A stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="StopTransactionRequest">A stop transaction request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="IdTagInfo">An identification tag info.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStopTransactionResponseDelegate(DateTime                   Timestamp,
                                          Object                     Sender,
                                          EventTracking_Id           EventTrackingId,
                                          CP.StopTransactionRequest  StopTransactionRequest,

                                          Result                     Result,
                                          IdTagInfo?                 IdTagInfo,
                                          TimeSpan                   Runtime);

    #endregion


    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DataTransferRequest">A data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                Timestamp,
                                              Object                  Sender,
                                              EventTracking_Id        EventTrackingId,
                                              CP.DataTransferRequest  DataTransferRequest);


    /// <summary>
    /// An incoming data transfer from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DataTransferRequest">A data transfer request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                Timestamp,
                                       Object                  Sender,
                                       CancellationToken       CancellationToken,
                                       EventTracking_Id        EventTrackingId,
                                       CP.DataTransferRequest  DataTransferRequest);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DataTransferRequest">A data transfer request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Status">The success or failure status of the data transfer.</param>
    /// <param name="ResponseData">Optional response data.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                Timestamp,
                                               Object                  Sender,
                                               EventTracking_Id        EventTrackingId,
                                               CP.DataTransferRequest  DataTransferRequest,

                                               Result                  Result,
                                               DataTransferStatus      Status,
                                               String                  ResponseData,
                                               TimeSpan                Runtime);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A diagnostics status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request.</param>
    public delegate Task

        OnDiagnosticsStatusNotificationRequestDelegate(DateTime                                 Timestamp,
                                                       Object                                   Sender,
                                                       EventTracking_Id                         EventTrackingId,
                                                       CP.DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest);


    /// <summary>
    /// A diagnostics status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request.</param>
    public delegate Task<DiagnosticsStatusNotificationResponse>

        OnDiagnosticsStatusNotificationDelegate(DateTime                                 Timestamp,
                                                Object                                   Sender,
                                                CancellationToken                        CancellationToken,
                                                EventTracking_Id                         EventTrackingId,
                                                CP.DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest);


    /// <summary>
    /// A diagnostics status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnDiagnosticsStatusNotificationResponseDelegate(DateTime                                 Timestamp,
                                                        Object                                   Sender,
                                                        EventTracking_Id                         EventTrackingId,
                                                        CP.DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,

                                                        Result                                   Result,
                                                        TimeSpan                                 Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware installation status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request.</param>

    public delegate Task

        OnFirmwareStatusNotificationRequestDelegate(DateTime                              Timestamp,
                                                    Object                                Sender,
                                                    EventTracking_Id                      EventTrackingId,
                                                    CP.FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest);


    /// <summary>
    /// A firmware installation status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             Object                                Sender,
                                             CancellationToken                     CancellationToken,
                                             EventTracking_Id                      EventTrackingId,
                                             CP.FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest);


    /// <summary>
    /// A firmware installation status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request.</param>
    /// 
    /// <param name="Result">The general OCPP result.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnFirmwareStatusNotificationResponseDelegate(DateTime                              Timestamp,
                                                     Object                                Sender,
                                                     EventTracking_Id                      EventTrackingId,
                                                     CP.FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,

                                                     Result                                Result,
                                                     TimeSpan                              Runtime);

    #endregion

}
