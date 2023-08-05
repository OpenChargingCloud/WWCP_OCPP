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

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Request">The boot notification request.</param>
    public delegate Task

        OnBootNotificationRequestDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          BootNotificationRequest   Request);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this boot notification request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                  Timestamp,
                                   IEventSender              Sender,
                                   BootNotificationRequest   Request,
                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification response.</param>
    /// <param name="Sender">The sender of the boot notification response.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="Response">The boot notification response.</param>
    /// <param name="Runtime">The runtime of the boot notification response.</param>
    public delegate Task

        OnBootNotificationResponseDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           BootNotificationRequest    Request,
                                           BootNotificationResponse   Response,
                                           TimeSpan                   Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>

    public delegate Task

        OnFirmwareStatusNotificationRequestDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    FirmwareStatusNotificationRequest   Request);


    /// <summary>
    /// A firmware status notification from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                            Timestamp,
                                             IEventSender                        Sender,
                                             FirmwareStatusNotificationRequest   Request,
                                             CancellationToken                   CancellationToken);


    /// <summary>
    /// A firmware status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="Response">The firmware status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnFirmwareStatusNotificationResponseDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     FirmwareStatusNotificationRequest    Request,
                                                     FirmwareStatusNotificationResponse   Response,
                                                     TimeSpan                             Runtime);

    #endregion

    #region OnPublishFirmwareStatusNotification

    /// <summary>
    /// A publish firmware status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>

    public delegate Task

        OnPublishFirmwareStatusNotificationRequestDelegate(DateTime                                   Timestamp,
                                                           IEventSender                               Sender,
                                                           PublishFirmwareStatusNotificationRequest   Request);


    /// <summary>
    /// A publish firmware status notification from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<PublishFirmwareStatusNotificationResponse>

        OnPublishFirmwareStatusNotificationDelegate(DateTime                                   Timestamp,
                                                    IEventSender                               Sender,
                                                    PublishFirmwareStatusNotificationRequest   Request,
                                                    CancellationToken                          CancellationToken);


    /// <summary>
    /// A publish firmware status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="Response">The firmware status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationResponseDelegate(DateTime                                    Timestamp,
                                                            IEventSender                                Sender,
                                                            PublishFirmwareStatusNotificationRequest    Request,
                                                            PublishFirmwareStatusNotificationResponse   Response,
                                                            TimeSpan                                    Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    public delegate Task

        OnHeartbeatRequestDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   HeartbeatRequest   Request);


    /// <summary>
    /// A heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The heartbeat charging station heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this charging station heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime            Timestamp,
                            IEventSender        Sender,
                            HeartbeatRequest    Request,
                            CancellationToken   CancellationToken);


    /// <summary>
    /// A heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat response.</param>
    /// <param name="Sender">The sender of the charging station heartbeat response.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    /// <param name="Response">The charging station heartbeat response.</param>
    /// <param name="Runtime">The runtime of the charging station heartbeat response.</param>
    public delegate Task

        OnHeartbeatResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    HeartbeatRequest    Request,
                                    HeartbeatResponse   Response,
                                    TimeSpan            Runtime);

    #endregion

    #region OnNotifyEvent

    /// <summary>
    /// A notify event request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyEventRequestDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     NotifyEventRequest   Request);


    /// <summary>
    /// A notify event at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyEventResponse>

        OnNotifyEventDelegate(DateTime             Timestamp,
                              IEventSender         Sender,
                              NotifyEventRequest   Request,
                              CancellationToken    CancellationToken);


    /// <summary>
    /// A notify event response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyEventResponseDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      NotifyEventRequest    Request,
                                      NotifyEventResponse   Response,
                                      TimeSpan              Runtime);

    #endregion

    #region OnSecurityEventNotification

    /// <summary>
    /// A security event notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSecurityEventNotificationRequestDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   SecurityEventNotificationRequest   Request);


    /// <summary>
    /// A security event notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecurityEventNotificationResponse>

        OnSecurityEventNotificationDelegate(DateTime                           Timestamp,
                                            IEventSender                       Sender,
                                            SecurityEventNotificationRequest   Request,
                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A security event notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSecurityEventNotificationResponseDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    SecurityEventNotificationRequest    Request,
                                                    SecurityEventNotificationResponse   Response,
                                                    TimeSpan                            Runtime);

    #endregion

    #region OnNotifyReport

    /// <summary>
    /// A notify report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyReportRequestDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      NotifyReportRequest   Request);


    /// <summary>
    /// A notify report at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyReportResponse>

        OnNotifyReportDelegate(DateTime              Timestamp,
                               IEventSender          Sender,
                               NotifyReportRequest   Request,
                               CancellationToken     CancellationToken);


    /// <summary>
    /// A notify report response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyReportResponseDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       NotifyReportRequest    Request,
                                       NotifyReportResponse   Response,
                                       TimeSpan               Runtime);

    #endregion

    #region OnNotifyMonitoringReport

    /// <summary>
    /// A notify monitoring report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyMonitoringReportRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyMonitoringReportRequest   Request);


    /// <summary>
    /// A notify monitoring report at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyMonitoringReportResponse>

        OnNotifyMonitoringReportDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         NotifyMonitoringReportRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A notify monitoring report response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyMonitoringReportResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 NotifyMonitoringReportRequest    Request,
                                                 NotifyMonitoringReportResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion

    #region OnLogStatusNotification

    /// <summary>
    /// A log status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnLogStatusNotificationRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               LogStatusNotificationRequest   Request);


    /// <summary>
    /// A log status notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<LogStatusNotificationResponse>

        OnLogStatusNotificationDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        LogStatusNotificationRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A log status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnLogStatusNotificationResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                LogStatusNotificationRequest    Request,
                                                LogStatusNotificationResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                 Timestamp,
                                              IEventSender             Sender,
                                              CS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer from the given charging station.
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
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                    Timestamp,
                                               IEventSender                Sender,
                                               CS.DataTransferRequest      Request,
                                               CSMS.DataTransferResponse   Response,
                                               TimeSpan                    Runtime);

    #endregion


    #region OnSignCertificate

    /// <summary>
    /// A sign certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSignCertificateRequestDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         SignCertificateRequest   Request);


    /// <summary>
    /// A sign certificate at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SignCertificateResponse>

        OnSignCertificateDelegate(DateTime                 Timestamp,
                                  IEventSender             Sender,
                                  SignCertificateRequest   Request,
                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A sign certificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSignCertificateResponseDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          SignCertificateRequest    Request,
                                          SignCertificateResponse   Response,
                                          TimeSpan                  Runtime);

    #endregion

    #region OnGet15118EVCertificate

    /// <summary>
    /// A get 15118 EV certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnGet15118EVCertificateRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               Get15118EVCertificateRequest   Request);


    /// <summary>
    /// A get 15118 EV certificate at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<Get15118EVCertificateResponse>

        OnGet15118EVCertificateDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        Get15118EVCertificateRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A get 15118 EV certificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnGet15118EVCertificateResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                Get15118EVCertificateRequest    Request,
                                                Get15118EVCertificateResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnGetCertificateStatus

    /// <summary>
    /// A get certificate status request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnGetCertificateStatusRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetCertificateStatusRequest   Request);


    /// <summary>
    /// A get certificate status at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCertificateStatusResponse>

        OnGetCertificateStatusDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       GetCertificateStatusRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A get certificate status response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnGetCertificateStatusResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               GetCertificateStatusRequest    Request,
                                               GetCertificateStatusResponse   Response,
                                               TimeSpan                       Runtime);

    #endregion


    #region OnReservationStatusUpdate

    /// <summary>
    /// A reservation status update request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnReservationStatusUpdateRequestDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 ReservationStatusUpdateRequest   Request);


    /// <summary>
    /// A reservation status update at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReservationStatusUpdateResponse>

        OnReservationStatusUpdateDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          ReservationStatusUpdateRequest   Request,
                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A reservation status update response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnReservationStatusUpdateResponseDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  ReservationStatusUpdateRequest    Request,
                                                  ReservationStatusUpdateResponse   Response,
                                                  TimeSpan                          Runtime);

    #endregion

    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime          Timestamp,
                                   IEventSender      Sender,
                                   AuthorizeRequest  Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime            Timestamp,
                            IEventSender        Sender,
                            AuthorizeRequest    Request,
                            CancellationToken   CancellationToken);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize response.</param>
    /// <param name="Sender">The sender of the authorize response.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize response.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    AuthorizeRequest    Request,
                                    AuthorizeResponse   Response,
                                    TimeSpan            Runtime);

    #endregion

    #region OnNotifyEVChargingNeeds

    /// <summary>
    /// A notify EV charging needs request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyEVChargingNeedsRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               NotifyEVChargingNeedsRequest   Request);


    /// <summary>
    /// A notify EV charging needs at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyEVChargingNeedsResponse>

        OnNotifyEVChargingNeedsDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        NotifyEVChargingNeedsRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A notify EV charging needs response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyEVChargingNeedsResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyEVChargingNeedsRequest    Request,
                                                NotifyEVChargingNeedsResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnTransactionEvent

    /// <summary>
    /// A transaction event request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    public delegate Task

        OnTransactionEventRequestDelegate(DateTime                 Timestamp,
                                          IEventSender             Sender,
                                          TransactionEventRequest  Request);


    /// <summary>
    /// A transaction event.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="CancellationToken">A token to cancel this transaction event request.</param>
    public delegate Task<TransactionEventResponse>

        OnTransactionEventDelegate(DateTime                  Timestamp,
                                   IEventSender              Sender,
                                   TransactionEventRequest   Request,
                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A transaction event response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event response.</param>
    /// <param name="Sender">The sender of the transaction event response.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="Response">The transaction event response.</param>
    /// <param name="Runtime">The runtime of the transaction event response.</param>
    public delegate Task

        OnTransactionEventResponseDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           TransactionEventRequest    Request,
                                           TransactionEventResponse   Response,
                                           TimeSpan                   Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            StatusNotificationRequest   Request);


    /// <summary>
    /// Send a status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     StatusNotificationRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="Response">The status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             StatusNotificationRequest    Request,
                                             StatusNotificationResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     MeterValuesRequest   Request);


    /// <summary>
    /// Send meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime             Timestamp,
                              IEventSender         Sender,
                              MeterValuesRequest   Request,
                              CancellationToken    CancellationToken);


    /// <summary>
    /// A meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="Response">The meter values response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      MeterValuesRequest    Request,
                                      MeterValuesResponse   Response,
                                      TimeSpan              Runtime);

    #endregion

    #region OnNotifyChargingLimit

    /// <summary>
    /// A notify charging limit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyChargingLimitRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             NotifyChargingLimitRequest   Request);


    /// <summary>
    /// A notify charging limit at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyChargingLimitResponse>

        OnNotifyChargingLimitDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      NotifyChargingLimitRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A notify charging limit response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyChargingLimitResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              NotifyChargingLimitRequest    Request,
                                              NotifyChargingLimitResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion

    #region OnClearedChargingLimit

    /// <summary>
    /// A cleared charging limit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnClearedChargingLimitRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              ClearedChargingLimitRequest   Request);


    /// <summary>
    /// A cleared charging limit at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearedChargingLimitResponse>

        OnClearedChargingLimitDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       ClearedChargingLimitRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A cleared charging limit response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnClearedChargingLimitResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               ClearedChargingLimitRequest    Request,
                                               ClearedChargingLimitResponse   Response,
                                               TimeSpan                       Runtime);

    #endregion

    #region OnReportChargingProfiles

    /// <summary>
    /// A report charging profiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnReportChargingProfilesRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                ReportChargingProfilesRequest   Request);


    /// <summary>
    /// A report charging profiles at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReportChargingProfilesResponse>

        OnReportChargingProfilesDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         ReportChargingProfilesRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A report charging profiles response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnReportChargingProfilesResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 ReportChargingProfilesRequest    Request,
                                                 ReportChargingProfilesResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion


    #region OnNotifyDisplayMessages

    /// <summary>
    /// A notify display messages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyDisplayMessagesRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               NotifyDisplayMessagesRequest   Request);


    /// <summary>
    /// A notify display messages at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyDisplayMessagesResponse>

        OnNotifyDisplayMessagesDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        NotifyDisplayMessagesRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A notify display messages response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyDisplayMessagesResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyDisplayMessagesRequest    Request,
                                                NotifyDisplayMessagesResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion

    #region OnNotifyCustomerInformation

    /// <summary>
    /// A notify customer information request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyCustomerInformationRequestDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   NotifyCustomerInformationRequest   Request);


    /// <summary>
    /// A notify customer information at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyCustomerInformationResponse>

        OnNotifyCustomerInformationDelegate(DateTime                           Timestamp,
                                            IEventSender                       Sender,
                                            NotifyCustomerInformationRequest   Request,
                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A notify customer information response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyCustomerInformationResponseDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    NotifyCustomerInformationRequest    Request,
                                                    NotifyCustomerInformationResponse   Response,
                                                    TimeSpan                            Runtime);

    #endregion


}
