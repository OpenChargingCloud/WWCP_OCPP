/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="BootNotificationRequest">A boot notification request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                    Timestamp,
                                   CentralSystemSOAPServer     Sender,
                                   CancellationToken           CancellationToken,
                                   EventTracking_Id            EventTrackingId,

                                   ChargeBox_Id                ChargeBoxIdentity,
                                   CP.BootNotificationRequest  BootNotificationRequest);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charge point heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="HeartbeatRequest">A heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime                 Timestamp,
                            CentralSystemSOAPServer  Sender,
                            CancellationToken        CancellationToken,
                            EventTracking_Id         EventTrackingId,

                            ChargeBox_Id             ChargeBoxIdentity,
                            CP.HeartbeatRequest      HeartbeatRequest);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="AuthorizeRequest">An authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime                 Timestamp,
                            CentralSystemSOAPServer  Sender,
                            CancellationToken        CancellationToken,
                            EventTracking_Id         EventTrackingId,

                            ChargeBox_Id             ChargeBoxIdentity,
                            CP.AuthorizeRequest      AuthorizeRequest);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A start transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StartTransactionRequest">A start transaction request.</param>
    public delegate Task<StartTransactionResponse>

        OnStartTransactionDelegate(DateTime                    Timestamp,
                                   CentralSystemSOAPServer     Sender,
                                   CancellationToken           CancellationToken,
                                   EventTracking_Id            EventTrackingId,

                                   ChargeBox_Id                ChargeBoxIdentity,
                                   CP.StartTransactionRequest  StartTransactionRequest);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StatusNotificationRequest">A status notification request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     CentralSystemSOAPServer       Sender,
                                     CancellationToken             CancellationToken,
                                     EventTracking_Id              EventTrackingId,

                                     ChargeBox_Id                  ChargeBoxIdentity,
                                     CP.StatusNotificationRequest  StatusNotificationRequest);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// Send charge point meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="MeterValuesRequest">A meter values request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime                 Timestamp,
                              CentralSystemSOAPServer  Sender,
                              CancellationToken        CancellationToken,
                              EventTracking_Id         EventTrackingId,

                              ChargeBox_Id             ChargeBoxIdentity,
                              CP.MeterValuesRequest    MeterValuesRequest);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A stop transaction at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="StopTransactionRequest">A stop transaction request.</param>
    public delegate Task<StopTransactionResponse>

        OnStopTransactionDelegate(DateTime                   Timestamp,
                                  CentralSystemSOAPServer    Sender,
                                  CancellationToken          CancellationToken,
                                  EventTracking_Id           EventTrackingId,

                                  ChargeBox_Id               ChargeBoxIdentity,
                                  CP.StopTransactionRequest  StopTransactionRequest);

    #endregion


    #region OnDataTransfer

    /// <summary>
    /// A data transfer from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="DataTransferRequest">A data transfer request.</param>
    public delegate Task<DataTransferResponse>

        OnDataTransferDelegate(DateTime                 Timestamp,
                               CentralSystemSOAPServer  Sender,
                               CancellationToken        CancellationToken,
                               EventTracking_Id         EventTrackingId,

                               ChargeBox_Id             ChargeBoxIdentity,
                               CP.DataTransferRequest   DataTransferRequest);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A diagnostics status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request.</param>
    public delegate Task<DiagnosticsStatusNotificationResponse>

        OnDiagnosticsStatusNotificationDelegate(DateTime                                 Timestamp,
                                                CentralSystemSOAPServer                  Sender,
                                                CancellationToken                        CancellationToken,
                                                EventTracking_Id                         EventTrackingId,

                                                ChargeBox_Id                             ChargeBoxIdentity,
                                                CP.DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware installation status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="CancellationToken">A token to cancel this task.</param>
    /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
    /// 
    /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
    /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             CentralSystemSOAPServer               Sender,
                                             CancellationToken                     CancellationToken,
                                             EventTracking_Id                      EventTrackingId,

                                             ChargeBox_Id                          ChargeBoxIdentity,
                                             CP.FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest);

    #endregion

}
