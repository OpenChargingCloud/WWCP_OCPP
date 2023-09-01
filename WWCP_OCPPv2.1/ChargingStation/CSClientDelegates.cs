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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnBootNotification

    /// <summary>
    /// A delegate called whenever a boot notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnBootNotificationRequestDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           BootNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a boot notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBootNotificationResponseDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            BootNotificationRequest    Request,
                                                            BootNotificationResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a firmware status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnFirmwareStatusNotificationRequestDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     FirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a firmware status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnFirmwareStatusNotificationResponseDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      FirmwareStatusNotificationRequest    Request,
                                                                      FirmwareStatusNotificationResponse   Response,
                                                                      TimeSpan                             Runtime);

    #endregion

    #region OnPublishFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a publish firmware status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnPublishFirmwareStatusNotificationRequestDelegate(DateTime                                   Timestamp,
                                                                            IEventSender                               Sender,
                                                                            PublishFirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a publish firmware status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnPublishFirmwareStatusNotificationResponseDelegate(DateTime                                    Timestamp,
                                                                             IEventSender                                Sender,
                                                                             PublishFirmwareStatusNotificationRequest    Request,
                                                                             PublishFirmwareStatusNotificationResponse   Response,
                                                                             TimeSpan                                    Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A delegate called whenever a heartbeat request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnHeartbeatRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    HeartbeatRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a heartbeat request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnHeartbeatResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     HeartbeatRequest    Request,
                                                     HeartbeatResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion

    #region OnNotifyEvent

    /// <summary>
    /// A delegate called whenever a notify event request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnNotifyEventRequestDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      NotifyEventRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify event request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyEventResponseDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       NotifyEventRequest    Request,
                                                       NotifyEventResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion

    #region OnSecurityEventNotification

    /// <summary>
    /// A delegate called whenever a security event notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnSecurityEventNotificationRequestDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    SecurityEventNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a security event notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSecurityEventNotificationResponseDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     SecurityEventNotificationRequest    Request,
                                                                     SecurityEventNotificationResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion

    #region OnNotifyReport

    /// <summary>
    /// A delegate called whenever a notify report request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnNotifyReportRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       NotifyReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyReportResponseDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        NotifyReportRequest    Request,
                                                        NotifyReportResponse   Response,
                                                        TimeSpan               Runtime);

    #endregion

    #region OnNotifyMonitoringReport

    /// <summary>
    /// A delegate called whenever a notify monitoring report request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnNotifyMonitoringReportRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyMonitoringReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify monitoring report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyMonitoringReportResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  NotifyMonitoringReportRequest    Request,
                                                                  NotifyMonitoringReportResponse   Response,
                                                                  TimeSpan                         Runtime);

    #endregion

    #region OnLogStatusNotification

    /// <summary>
    /// A delegate called whenever a log status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnLogStatusNotificationRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                LogStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a log status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnLogStatusNotificationResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 LogStatusNotificationRequest    Request,
                                                                 LogStatusNotificationResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be sent to the CSMS.
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
    public delegate Task OnDataTransferResponseDelegate(DateTime                    Timestamp,
                                                        IEventSender                Sender,
                                                        DataTransferRequest         Request,
                                                        CSMS.DataTransferResponse   Response,
                                                        TimeSpan                    Runtime);

    #endregion


    #region OnSignCertificate

    /// <summary>
    /// A delegate called whenever a sign certificate request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSignCertificateRequestDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          SignCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a sign certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSignCertificateResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           SignCertificateRequest    Request,
                                                           SignCertificateResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion

    #region OnGet15118EVCertificate

    /// <summary>
    /// A delegate called whenever a get 15118 EV certificate request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGet15118EVCertificateRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                Get15118EVCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get 15118 EV certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGet15118EVCertificateResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 Get15118EVCertificateRequest    Request,
                                                                 Get15118EVCertificateResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnGetCertificateStatus

    /// <summary>
    /// A delegate called whenever a get certificate status request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetCertificateStatusRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               GetCertificateStatusRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get certificate status request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetCertificateStatusResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                GetCertificateStatusRequest    Request,
                                                                GetCertificateStatusResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion

    #region OnGetCRL

    /// <summary>
    /// A delegate called whenever a get certificate revocation list request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetCRLRequestDelegate(DateTime        Timestamp,
                                                 IEventSender    Sender,
                                                 GetCRLRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get certificate revocation list request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetCRLResponseDelegate(DateTime         Timestamp,
                                                  IEventSender     Sender,
                                                  GetCRLRequest    Request,
                                                  GetCRLResponse   Response,
                                                  TimeSpan         Runtime);

    #endregion


    #region OnReservationStatusUpdate

    /// <summary>
    /// A delegate called whenever a reservation status update request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnReservationStatusUpdateRequestDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  ReservationStatusUpdateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a reservation status update request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnReservationStatusUpdateResponseDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   ReservationStatusUpdateRequest    Request,
                                                                   ReservationStatusUpdateResponse   Response,
                                                                   TimeSpan                          Runtime);

    #endregion

    #region OnAuthorize

    /// <summary>
    /// A delegate called whenever an authorize request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task OnAuthorizeRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    AuthorizeRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize request.</param>
    public delegate Task OnAuthorizeResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     AuthorizeRequest    Request,
                                                     AuthorizeResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion

    #region OnNotifyEVChargingNeeds

    /// <summary>
    /// A delegate called whenever a notify EV charging needs request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyEVChargingNeedsRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                NotifyEVChargingNeedsRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify EV charging needs request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyEVChargingNeedsResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyEVChargingNeedsRequest    Request,
                                                                 NotifyEVChargingNeedsResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnTransactionEvent

    /// <summary>
    /// A delegate called whenever a transaction event request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    public delegate Task OnTransactionEventRequestDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           TransactionEventRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a transaction event request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="Response">The transaction event response.</param>
    /// <param name="Runtime">The runtime of the transaction event request.</param>
    public delegate Task OnTransactionEventResponseDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            TransactionEventRequest    Request,
                                                            TransactionEventResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A delegate called whenever a status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStatusNotificationRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             StatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStatusNotificationResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              StatusNotificationRequest    Request,
                                                              StatusNotificationResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A delegate called whenever a meter values request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnMeterValuesRequestDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      MeterValuesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a meter values request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnMeterValuesResponseDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       MeterValuesRequest    Request,
                                                       MeterValuesResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion

    #region OnNotifyChargingLimit

    /// <summary>
    /// A delegate called whenever a notify charging limit request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyChargingLimitRequestDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              NotifyChargingLimitRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify charging limit request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyChargingLimitResponseDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               NotifyChargingLimitRequest    Request,
                                                               NotifyChargingLimitResponse   Response,
                                                               TimeSpan                      Runtime);

    #endregion

    #region OnClearedChargingLimit

    /// <summary>
    /// A delegate called whenever a cleared charging limit request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearedChargingLimitRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               ClearedChargingLimitRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a cleared charging limit request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearedChargingLimitResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                ClearedChargingLimitRequest    Request,
                                                                ClearedChargingLimitResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion

    #region OnReportChargingProfiles

    /// <summary>
    /// A delegate called whenever a ReportChargingProfiles request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnReportChargingProfilesRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 ReportChargingProfilesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a ReportChargingProfiles request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnReportChargingProfilesResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  ReportChargingProfilesRequest    Request,
                                                                  ReportChargingProfilesResponse   Response,
                                                                  TimeSpan                         Runtime);

    #endregion

    #region OnNotifyEVChargingSchedule

    /// <summary>
    /// A delegate called whenever a report charging profiles request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyEVChargingScheduleRequestDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   NotifyEVChargingScheduleRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a report charging profiles request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyEVChargingScheduleResponseDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    NotifyEVChargingScheduleRequest    Request,
                                                                    NotifyEVChargingScheduleResponse   Response,
                                                                    TimeSpan                           Runtime);

    #endregion

    #region OnNotifyPriorityCharging

    /// <summary>
    /// A delegate called whenever a NotifyPriorityCharging request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyPriorityChargingRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyPriorityChargingRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a NotifyPriorityCharging request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyPriorityChargingResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  NotifyPriorityChargingRequest    Request,
                                                                  NotifyPriorityChargingResponse   Response,
                                                                  TimeSpan                         Runtime);

    #endregion

    #region OnPullDynamicScheduleUpdate

    /// <summary>
    /// A delegate called whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnPullDynamicScheduleUpdateRequestDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    PullDynamicScheduleUpdateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a PullDynamicScheduleUpdate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnPullDynamicScheduleUpdateResponseDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     PullDynamicScheduleUpdateRequest    Request,
                                                                     PullDynamicScheduleUpdateResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion


    #region OnNotifyDisplayMessages

    /// <summary>
    /// A delegate called whenever a notify display messages request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyDisplayMessagesRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                NotifyDisplayMessagesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify display messages request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyDisplayMessagesResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyDisplayMessagesRequest    Request,
                                                                 NotifyDisplayMessagesResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnNotifyCustomerInformation

    /// <summary>
    /// A delegate called whenever a notify customer information request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyCustomerInformationRequestDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    NotifyCustomerInformationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify customer information request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyCustomerInformationResponseDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     NotifyCustomerInformationRequest    Request,
                                                                     NotifyCustomerInformationResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion


}
