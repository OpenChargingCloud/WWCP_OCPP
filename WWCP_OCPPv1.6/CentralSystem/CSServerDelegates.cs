/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
    /// <param name="Request">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   CP.BootNotificationRequest  Request,
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
    /// <param name="Request">The heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CP.HeartbeatRequest  Request,
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
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        AuthorizeRequestDelegate(DateTime             Timestamp,
                                 IEventSender         Sender,
                                 CP.AuthorizeRequest  Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CP.AuthorizeRequest  Request,
                            CancellationToken    CancellationToken);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        AuthorizeResponseDelegate(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  CP.AuthorizeRequest    Request,
                                  CS.AuthorizeResponse   Response,
                                  TimeSpan               Runtime);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The start transaction request.</param>
    public delegate Task

        StartTransactionRequestDelegate(DateTime                    Timestamp,
                                        IEventSender                Sender,
                                        CP.StartTransactionRequest  Request);


    /// <summary>
    /// A start transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The start transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StartTransactionResponse>

        OnStartTransactionDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   CP.StartTransactionRequest  Request,
                                   CancellationToken           CancellationToken);


    /// <summary>
    /// A start transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The start transaction request.</param>
    /// <param name="Response">The start transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        StartTransactionResponseDelegate(DateTime                     Timestamp,
                                         IEventSender                 Sender,
                                         CP.StartTransactionRequest   Request,
                                         CS.StartTransactionResponse  Response,
                                         TimeSpan                     Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    public delegate Task

        StatusNotificationRequestDelegate(DateTime                      Timestamp,
                                          IEventSender                  Sender,
                                          CP.StatusNotificationRequest  Request);


    /// <summary>
    /// Send a charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     IEventSender                  Sender,
                                     CP.StatusNotificationRequest  Request,
                                     CancellationToken             CancellationToken);


    /// <summary>
    /// A charge point status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="Response">The status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        StatusNotificationResponseDelegate(DateTime                       Timestamp,
                                           IEventSender                   Sender,
                                           CP.StatusNotificationRequest   Request,
                                           CS.StatusNotificationResponse  Response,
                                           TimeSpan                       Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A charge point meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    public delegate Task

        MeterValuesRequestDelegate(DateTime               Timestamp,
                                   IEventSender           Sender,
                                   CP.MeterValuesRequest  Request);


    /// <summary>
    /// Send charge point meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime               Timestamp,
                              IEventSender           Sender,
                              CP.MeterValuesRequest  Request,
                              CancellationToken      CancellationToken);


    /// <summary>
    /// A charge point meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="Response">The meter values response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        MeterValuesResponseDelegate(DateTime                Timestamp,
                                    IEventSender            Sender,
                                    CP.MeterValuesRequest   Request,
                                    CS.MeterValuesResponse  Response,
                                    TimeSpan                Runtime);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        StopTransactionRequestDelegate(DateTime                   Timestamp,
                                       IEventSender               Sender,
                                       CP.StopTransactionRequest  Request);


    /// <summary>
    /// A stop transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StopTransactionResponse>

        OnStopTransactionDelegate(DateTime                   Timestamp,
                                  IEventSender               Sender,
                                  CP.StopTransactionRequest  Request,
                                  CancellationToken          CancellationToken);


    /// <summary>
    /// A stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        StopTransactionResponseDelegate(DateTime                    Timestamp,
                                        IEventSender                Sender,
                                        CP.StopTransactionRequest   Request,
                                        CS.StopTransactionResponse  Response,
                                        TimeSpan                    Runtime);

    #endregion


    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        IncomingDataTransferRequestDelegate(DateTime                Timestamp,
                                            IEventSender            Sender,
                                            CP.DataTransferRequest  Request);


    /// <summary>
    /// An incoming data transfer from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                Timestamp,
                                       IEventSender            Sender,
                                       CP.DataTransferRequest  Request,
                                       CancellationToken       CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        IncomingDataTransferResponseDelegate(DateTime                 Timestamp,
                                             IEventSender             Sender,
                                             CP.DataTransferRequest   Request,
                                             CS.DataTransferResponse  Response,
                                             TimeSpan                 Runtime);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A diagnostics status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The diagnostics status notification request.</param>
    public delegate Task

        DiagnosticsStatusNotificationRequestDelegate(DateTime                                 Timestamp,
                                                     IEventSender                             Sender,
                                                     CP.DiagnosticsStatusNotificationRequest  Request);


    /// <summary>
    /// A diagnostics status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    /// <param name="Request">The diagnostics status notification request.</param>
    public delegate Task<DiagnosticsStatusNotificationResponse>

        OnDiagnosticsStatusNotificationDelegate(DateTime                                 Timestamp,
                                                IEventSender                             Sender,
                                                CP.DiagnosticsStatusNotificationRequest  Request,
                                                CancellationToken                        CancellationToken);


    /// <summary>
    /// A diagnostics status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The diagnostics status notification request.</param>
    /// <param name="Response">The diagnostics status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        DiagnosticsStatusNotificationResponseDelegate(DateTime                                  Timestamp,
                                                      IEventSender                              Sender,
                                                      CP.DiagnosticsStatusNotificationRequest   Request,
                                                      CS.DiagnosticsStatusNotificationResponse  Response,
                                                      TimeSpan                                  Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware installation status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>

    public delegate Task

        FirmwareStatusNotificationRequestDelegate(DateTime                              Timestamp,
                                                  IEventSender                          Sender,
                                                  CP.FirmwareStatusNotificationRequest  Request);


    /// <summary>
    /// A firmware installation status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             IEventSender                          Sender,
                                             CP.FirmwareStatusNotificationRequest  Request,
                                             CancellationToken                     CancellationToken);


    /// <summary>
    /// A firmware installation status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="Response">The firmware status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        FirmwareStatusNotificationResponseDelegate(DateTime                               Timestamp,
                                                   IEventSender                           Sender,
                                                   CP.FirmwareStatusNotificationRequest   Request,
                                                   CS.FirmwareStatusNotificationResponse  Response,
                                                   TimeSpan                               Runtime);

    #endregion

}
